// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Valkey.Glide.Internals;

/// <summary>
/// Status of a custom IAM credentials provider invocation.
/// Must match the <c>STATUS_*</c> constants in <c>rust/src/iam.rs</c>.
/// </summary>
internal enum IamCredentialsStatus
{
    Success = 0,
    NullTask = 1,
    NullResult = 2,
    Faulted = 3,
    Cancelled = 4,
    Busy = 5,
    TimedOut = 6,
    InvalidCredentials = 7,
    Internal = 8,
}

/// <summary>
/// Bridges native IAM credentials requests to a managed <see cref="GlideCredentialProvider"/>.
/// </summary>
/// <remarks>
/// <para>
/// One bridge is registered per client. The native client owns the registered handle and releases it
/// exactly once through <see cref="ReleasePointer"/> when its last reference to the provider is dropped.
/// </para>
/// <para>
/// Each invocation runs the provider on the thread pool and allows <see cref="InvocationTimeout"/> in total
/// for scheduling, the synchronous provider body, and its returned task. A new invocation is rejected while
/// a previous synchronous provider body is still running.
/// </para>
/// </remarks>
internal sealed class IamCredentialsBridge
{
    #region Constants

    /// <summary>
    /// Total time allowed for one provider invocation. Must finish before the core's 10-second deadline.
    /// </summary>
    internal static readonly TimeSpan InvocationTimeout = TimeSpan.FromSeconds(9);

    /// <summary>
    /// UTF-8 encoding that throws on invalid input instead of substituting replacement characters.
    /// </summary>
    private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    #endregion
    #region Private Fields

    private readonly GlideCredentialProvider _provider;
    private readonly TimeProvider _timeProvider;
    private readonly CancellationTokenSource _disposalSource = new();

    private GCHandle _handle;
    private int _bodyRunning;
    private int _cancelled;
    private int _released;

    #endregion
    #region Constructors

    /// <summary>
    /// Creates a new bridge for the given provider.
    /// </summary>
    /// <param name="provider">The managed credentials provider.</param>
    /// <param name="timeProvider">The time provider used for the invocation deadline. Defaults to <see cref="TimeProvider.System"/>.</param>
    internal IamCredentialsBridge(GlideCredentialProvider provider, TimeProvider? timeProvider = null)
    {
        ArgumentNullException.ThrowIfNull(provider);

        _provider = provider;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    #endregion
    #region Internal Properties

    /// <summary>
    /// Native function pointer for <see cref="InvokeFromNative"/>.
    /// </summary>
    internal static IntPtr InvokePointer
        => (IntPtr)(delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)&InvokeFromNative;

    /// <summary>
    /// Native function pointer for <see cref="ReleaseFromNative"/>.
    /// </summary>
    internal static IntPtr ReleasePointer
        => (IntPtr)(delegate* unmanaged[Cdecl]<IntPtr, void>)&ReleaseFromNative;

    /// <summary>
    /// Whether the registration has been released.
    /// </summary>
    internal bool IsReleased => Volatile.Read(ref _released) != 0;

    /// <summary>
    /// Whether a synchronous provider body is currently running.
    /// </summary>
    internal bool IsBodyRunning => Volatile.Read(ref _bodyRunning) != 0;

    /// <summary>
    /// Whether the handle returned by <see cref="Register"/> is still allocated.
    /// </summary>
    internal bool IsHandleAllocated => _handle.IsAllocated;

    #endregion
    #region Internal Methods

    /// <summary>
    /// Allocates the handle passed to the native client.
    /// </summary>
    /// <returns>The opaque handle for this bridge.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the bridge is already registered.</exception>
    internal IntPtr Register()
    {
        if (_handle.IsAllocated || IsReleased)
        {
            throw new InvalidOperationException("The credentials provider bridge is already registered.");
        }

        _handle = GCHandle.Alloc(this);
        return GCHandle.ToIntPtr(_handle);
    }

    /// <summary>
    /// Invokes the provider and passes the encoded credentials to <paramref name="sink"/>.
    /// </summary>
    /// <param name="sink">
    /// Copies the credentials. The pointers in the argument are only valid until the sink returns.
    /// Returns <see langword="false"/> if the credentials could not be copied.
    /// </param>
    /// <returns>The invocation status.</returns>
    internal IamCredentialsStatus Invoke(Func<FFI.AwsCredentialsFfi, bool> sink)
    {
        if (Volatile.Read(ref _cancelled) != 0)
        {
            return IamCredentialsStatus.Cancelled;
        }

        if (Interlocked.CompareExchange(ref _bodyRunning, 1, 0) != 0)
        {
            return IamCredentialsStatus.Busy;
        }

        // Start the deadline before scheduling so it also covers queueing.
        var deadlineSource = new CancellationTokenSource(InvocationTimeout, _timeProvider);
        var invocationSource = new CancellationTokenSource();

        // Cancel through the guard so a throwing provider callback cannot crash the timer or disposal thread.
        _ = deadlineSource.Token.UnsafeRegister(static state => TryCancel((CancellationTokenSource)state!), invocationSource);
        CancellationTokenRegistration disposalLink = _disposalSource.Token.UnsafeRegister(static state => TryCancel((CancellationTokenSource)state!), invocationSource);
        CancellationToken invocationToken = invocationSource.Token;
        Task<(IamCredentialsStatus Status, AwsCredentials? Credentials)> work = RunAsync(invocationToken);

        (IamCredentialsStatus Status, AwsCredentials? Credentials) result;
        try
        {
            result = work.WaitAsync(invocationToken).GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
            result = (IamCredentialsStatus.Cancelled, null);
        }
        finally
        {
            // Unlink from the bridge so a provider task that never completes is not kept alive by it.
            _ = disposalLink.Unregister();

            // A late provider task may still use the token sources.
            DisposeOnCompletion(work, invocationSource, deadlineSource);
        }

        // A provider that honors the expired deadline token completes cancelled; report that as a timeout.
        bool timedOut = deadlineSource.IsCancellationRequested && !_disposalSource.IsCancellationRequested;
        if (result.Status == IamCredentialsStatus.Cancelled && timedOut)
        {
            return IamCredentialsStatus.TimedOut;
        }

        return result.Status == IamCredentialsStatus.Success
            ? CopyCredentials(result.Credentials!, sink)
            : result.Status;
    }

    /// <summary>
    /// Rejects new invocations and requests cancellation of any in-flight invocation.
    /// </summary>
    internal void Cancel()
    {
        if (Interlocked.Exchange(ref _cancelled, 1) == 0)
        {
            TryCancel(_disposalSource);
        }
    }

    /// <summary>
    /// Cancels the bridge and frees its handle. Only the first call has any effect.
    /// </summary>
    internal void Release()
    {
        if (Interlocked.Exchange(ref _released, 1) != 0)
        {
            return;
        }

        Cancel();
        if (_handle.IsAllocated)
        {
            _handle.Free();
        }
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Native entry point that invokes the provider for the bridge identified by <paramref name="handle"/>.
    /// </summary>
    /// <param name="handle">The handle returned by <see cref="Register"/>.</param>
    /// <param name="sink">The native sink to pass to <see cref="FFI.SetIamCredentialsFfi"/>.</param>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe int InvokeFromNative(IntPtr handle, IntPtr sink)
    {
        try
        {
            var bridge = (IamCredentialsBridge)GCHandle.FromIntPtr(handle).Target!;
            return (int)bridge.Invoke(credentials => FFI.SetIamCredentialsFfi(sink, &credentials));
        }
        catch (Exception ex)
        {
            // Log only the exception type to avoid exposing credentials.
            Logger.Log(Level.Error, "IamCredentials", $"Credentials provider bridge failed: {ex.GetType().Name}");
            return (int)IamCredentialsStatus.Internal;
        }
    }

    /// <summary>
    /// Native entry point that releases the bridge identified by <paramref name="handle"/>.
    /// </summary>
    /// <param name="handle">The handle returned by <see cref="Register"/>.</param>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void ReleaseFromNative(IntPtr handle)
    {
        try
        {
            ((IamCredentialsBridge)GCHandle.FromIntPtr(handle).Target!).Release();
        }
        catch (Exception ex)
        {
            Logger.Log(Level.Error, "IamCredentials", $"Credentials provider release failed: {ex.GetType().Name}");
        }
    }

    /// <summary>
    /// Requests cancellation without letting exceptions from cancellation callbacks escape.
    /// </summary>
    /// <param name="source">The token source to cancel.</param>
    private static void TryCancel(CancellationTokenSource source)
    {
        try
        {
            source.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // The invocation already completed.
        }
        catch (AggregateException)
        {
            // A provider cancellation callback threw. Cancellation was still requested.
        }
    }

    /// <summary>
    /// Disposes the token sources once <paramref name="work"/> completes.
    /// </summary>
    /// <param name="work">The provider invocation.</param>
    /// <param name="invocationSource">The token source passed to the provider.</param>
    /// <param name="deadlineSource">The deadline token source.</param>
    private static void DisposeOnCompletion(Task work, CancellationTokenSource invocationSource, CancellationTokenSource deadlineSource)
        => _ = work.ContinueWith(
            static (_, state) =>
            {
                var (invocation, deadline) = ((CancellationTokenSource, CancellationTokenSource))state!;
                invocation.Dispose();
                deadline.Dispose();
            },
            (invocationSource, deadlineSource),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

    /// <summary>
    /// Encodes the credentials as UTF-8 and passes them to <paramref name="sink"/> while pinned.
    /// </summary>
    /// <param name="credentials">The credentials returned by the provider.</param>
    /// <param name="sink">Copies the encoded credentials.</param>
    private static IamCredentialsStatus CopyCredentials(AwsCredentials credentials, Func<FFI.AwsCredentialsFfi, bool> sink)
    {
        byte[] accessKeyId = [];
        byte[] secretAccessKey = [];
        byte[] sessionToken = [];
        try
        {
            accessKeyId = StrictUtf8.GetBytes(credentials.AccessKeyId);
            secretAccessKey = StrictUtf8.GetBytes(credentials.SecretAccessKey);
            sessionToken = credentials.SessionToken is null ? [] : StrictUtf8.GetBytes(credentials.SessionToken);
            long expiresAtUnixMilliseconds = credentials.ExpiresAt?.ToUnixTimeMilliseconds() ?? 0;

            fixed (byte* accessKeyIdPointer = accessKeyId)
            fixed (byte* secretAccessKeyPointer = secretAccessKey)
            fixed (byte* sessionTokenPointer = sessionToken)
            {
                FFI.AwsCredentialsFfi ffiCredentials = new(
                    (IntPtr)accessKeyIdPointer,
                    (nuint)accessKeyId.Length,
                    (IntPtr)secretAccessKeyPointer,
                    (nuint)secretAccessKey.Length,
                    credentials.SessionToken is not null,
                    (IntPtr)sessionTokenPointer,
                    (nuint)sessionToken.Length,
                    expiresAtUnixMilliseconds > 0,
                    expiresAtUnixMilliseconds);

                return sink(ffiCredentials) ? IamCredentialsStatus.Success : IamCredentialsStatus.InvalidCredentials;
            }
        }
        catch (EncoderFallbackException)
        {
            return IamCredentialsStatus.InvalidCredentials;
        }
        finally
        {
            CryptographicOperations.ZeroMemory(accessKeyId);
            CryptographicOperations.ZeroMemory(secretAccessKey);
            CryptographicOperations.ZeroMemory(sessionToken);
        }
    }

    /// <summary>
    /// Runs the provider body on the thread pool and awaits its task.
    /// </summary>
    /// <param name="cancellationToken">The token passed to the provider.</param>
    private async Task<(IamCredentialsStatus Status, AwsCredentials? Credentials)> RunAsync(CancellationToken cancellationToken)
    {
        Task<AwsCredentials>? providerTask;
        try
        {
            // Task.Factory.StartNew does not unwrap the returned task, so a null task can be detected.
            providerTask = await Task.Factory.StartNew(
                () => RunBody(cancellationToken),
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            return (IamCredentialsStatus.Cancelled, null);
        }
        catch (Exception)
        {
            return (IamCredentialsStatus.Faulted, null);
        }

        if (providerTask is null)
        {
            return (IamCredentialsStatus.NullTask, null);
        }

        try
        {
            AwsCredentials? credentials = await providerTask.ConfigureAwait(false);
            return credentials is null
                ? (IamCredentialsStatus.NullResult, null)
                : (IamCredentialsStatus.Success, credentials);
        }
        catch (OperationCanceledException)
        {
            return (IamCredentialsStatus.Cancelled, null);
        }
        catch (Exception)
        {
            return (IamCredentialsStatus.Faulted, null);
        }
    }

    /// <summary>
    /// Runs the synchronous provider body and releases the busy slot as soon as it returns or throws.
    /// </summary>
    /// <param name="cancellationToken">The token passed to the provider.</param>
    private Task<AwsCredentials>? RunBody(CancellationToken cancellationToken)
    {
        try
        {
            // Skip the provider if the invocation timed out or was cancelled before it was scheduled.
            cancellationToken.ThrowIfCancellationRequested();
            return _provider(cancellationToken);
        }
        finally
        {
            Volatile.Write(ref _bodyRunning, 0);
        }
    }

    #endregion
}
