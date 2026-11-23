// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

using Microsoft.Extensions.Time.Testing;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.UnitTests;

/// <summary>
/// Tests for <see cref="IamCredentialsBridge"/> invocation, deadline, and release behavior.
/// </summary>
public class IamCredentialsBridgeTests
{
    #region Constants

    private const string AccessKeyId = "ACCESS_KEY_ID";
    private const string SecretAccessKey = "SECRET_ACCESS_KEY";
    private const string Secret = "SECRET_VALUE";

    /// <summary>
    /// Real-time bound for test synchronization. Deadlines under test use a fake clock.
    /// </summary>
    private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);

    private static readonly AwsCredentials Credentials = new(AccessKeyId, SecretAccessKey);

    #endregion
    #region Delegates

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SuccessAction(ulong index, IntPtr response);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FailureAction(ulong index, IntPtr message, uint errorType);

    #endregion
    #region Tests

    [Fact]
    public void CopiesCredentialFields()
    {
        // Multibyte text, surrounding whitespace, a session token over 2 KiB, and a positive expiry.
        const string accessKeyId = " AKID-ключ-🔑 ";
        const string secretAccessKey = "secret/日本語+=";
        string sessionToken = string.Concat(Enumerable.Repeat("token-é-", 400));
        const long expiresAtUnixMs = 1_900_000_000_123;

        var captured = InvokeAndCapture(new AwsCredentials(
            accessKeyId,
            secretAccessKey,
            sessionToken,
            DateTimeOffset.FromUnixTimeMilliseconds(expiresAtUnixMs).ToOffset(TimeSpan.FromHours(5))));

        Assert.Equal(accessKeyId, captured.AccessKeyId);
        Assert.Equal(secretAccessKey, captured.SecretAccessKey);
        Assert.True(captured.HasSessionToken);
        Assert.True(Encoding.UTF8.GetByteCount(sessionToken) > 2048);
        Assert.Equal(sessionToken, captured.SessionToken);
        Assert.True(captured.HasExpiresAt);
        Assert.Equal(expiresAtUnixMs, captured.ExpiresAtUnixMs);

        // Absent session token and expiry.
        captured = InvokeAndCapture(new AwsCredentials(AccessKeyId, SecretAccessKey));

        Assert.False(captured.HasSessionToken);
        Assert.False(captured.HasExpiresAt);

        // Empty session token is preserved, and non-positive expiry is absent.
        captured = InvokeAndCapture(new AwsCredentials(AccessKeyId, SecretAccessKey, string.Empty, DateTimeOffset.UnixEpoch));

        Assert.True(captured.HasSessionToken);
        Assert.Equal(string.Empty, captured.SessionToken);
        Assert.False(captured.HasExpiresAt);

        captured = InvokeAndCapture(new AwsCredentials(AccessKeyId, SecretAccessKey, null, DateTimeOffset.UnixEpoch.AddDays(-1)));

        Assert.False(captured.HasExpiresAt);
    }

    [Theory]
    [InlineData("throwing body")]
    [InlineData("faulted task")]
    [InlineData("cancelled task")]
    [InlineData("null task")]
    [InlineData("null result")]
    [InlineData("invalid UTF-16")]
    public void ProviderFailuresReturnSanitizedStatus(string failure)
    {
        (GlideCredentialProvider provider, IamCredentialsStatus expected) = failure switch
        {
            "throwing body" => ((GlideCredentialProvider)(_ => throw new InvalidOperationException(Secret)), IamCredentialsStatus.Faulted),
            "faulted task" => (_ => Task.FromException<AwsCredentials>(new InvalidOperationException(Secret)), IamCredentialsStatus.Faulted),
            "cancelled task" => (_ => Task.FromCanceled<AwsCredentials>(new CancellationToken(canceled: true)), IamCredentialsStatus.Cancelled),
            "null task" => (_ => null!, IamCredentialsStatus.NullTask),
            "null result" => (_ => Task.FromResult<AwsCredentials>(null!), IamCredentialsStatus.NullResult),
            "invalid UTF-16" => (_ => Task.FromResult(new AwsCredentials("\uD800", SecretAccessKey)), IamCredentialsStatus.InvalidCredentials),
            _ => throw new ArgumentOutOfRangeException(nameof(failure)),
        };

        var bridge = new IamCredentialsBridge(provider);
        int sinkCalls = 0;

        var status = bridge.Invoke(_ =>
        {
            sinkCalls++;
            return true;
        });

        // Only the status crosses the native boundary, so no exception text or secret can be reported.
        Assert.Equal(expected, status);
        Assert.Equal(0, sinkCalls);
        Assert.False(bridge.IsBodyRunning);
    }

    [Fact]
    public async Task BodyAndTaskShareDeadline()
    {
        var timeProvider = new FakeTimeProvider();
        using var bodyEntered = new ManualResetEventSlim();
        using var releaseBody = new ManualResetEventSlim();
        var pendingTask = new TaskCompletionSource<AwsCredentials>(TaskCreationOptions.RunContinuationsAsynchronously);
        CancellationToken providerToken = default;

        var bridge = new IamCredentialsBridge(
            cancellationToken =>
            {
                providerToken = cancellationToken;
                bodyEntered.Set();
                _ = releaseBody.Wait(WaitTimeout, TestContext.Current.CancellationToken);
                return pendingTask.Task;
            },
            timeProvider);

        var invocation = Task.Run(() => bridge.Invoke(_ => true), TestContext.Current.CancellationToken);
        Assert.True(bodyEntered.Wait(WaitTimeout, TestContext.Current.CancellationToken));

        // Six seconds elapse while the synchronous body is still running.
        timeProvider.Advance(TimeSpan.FromSeconds(6));
        Assert.False(invocation.IsCompleted);

        // The body returns an incomplete task, which only has the remaining three seconds.
        releaseBody.Set();
        await WaitUntilAsync(() => !bridge.IsBodyRunning);

        timeProvider.Advance(TimeSpan.FromSeconds(3) - TimeSpan.FromMilliseconds(1));
        Assert.False(invocation.IsCompleted);
        Assert.False(providerToken.IsCancellationRequested);

        timeProvider.Advance(TimeSpan.FromMilliseconds(1));

        Assert.Equal(IamCredentialsStatus.TimedOut, await invocation.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken));
        Assert.True(providerToken.IsCancellationRequested);
        Assert.False(pendingTask.Task.IsCompleted);
    }

    [Fact]
    public async Task DeadlineReportsTimeoutWhenProviderHonorsCancellation()
    {
        var timeProvider = new FakeTimeProvider();
        using var bodyEntered = new ManualResetEventSlim();

        // The provider completes its task as cancelled as soon as its token is signaled.
        var bridge = new IamCredentialsBridge(
            cancellationToken =>
            {
                var pendingTask = new TaskCompletionSource<AwsCredentials>();
                _ = cancellationToken.Register(() => pendingTask.TrySetCanceled(cancellationToken));
                bodyEntered.Set();
                return pendingTask.Task;
            },
            timeProvider);

        var invocation = Task.Run(() => bridge.Invoke(_ => true), TestContext.Current.CancellationToken);
        Assert.True(bodyEntered.Wait(WaitTimeout, TestContext.Current.CancellationToken));
        await WaitUntilAsync(() => !bridge.IsBodyRunning);

        timeProvider.Advance(IamCredentialsBridge.InvocationTimeout);

        Assert.Equal(IamCredentialsStatus.TimedOut, await invocation.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken));
    }

    [Fact]
    public void RejectedCopyReturnsInvalidCredentials()
    {
        var bridge = new IamCredentialsBridge(_ => Task.FromResult(Credentials));

        Assert.Equal(IamCredentialsStatus.InvalidCredentials, bridge.Invoke(_ => false));
    }

    [Fact]
    public async Task RejectsBusyBodyAndRecovers()
    {
        var timeProvider = new FakeTimeProvider();
        using var bodyEntered = new ManualResetEventSlim();
        using var releaseBody = new ManualResetEventSlim();
        int providerCalls = 0;

        var bridge = new IamCredentialsBridge(
            cancellationToken =>
            {
                if (Interlocked.Increment(ref providerCalls) == 1)
                {
                    // Ignore cancellation and block until the test releases the body.
                    bodyEntered.Set();
                    _ = releaseBody.Wait(WaitTimeout, TestContext.Current.CancellationToken);
                }

                return Task.FromResult(Credentials);
            },
            timeProvider);

        var first = Task.Run(() => bridge.Invoke(_ => true), TestContext.Current.CancellationToken);
        Assert.True(bodyEntered.Wait(WaitTimeout, TestContext.Current.CancellationToken));

        timeProvider.Advance(IamCredentialsBridge.InvocationTimeout);
        Assert.Equal(IamCredentialsStatus.TimedOut, await first.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken));

        // The first body is still running, so a new call is rejected without invoking the provider.
        Assert.True(bridge.IsBodyRunning);
        Assert.Equal(IamCredentialsStatus.Busy, bridge.Invoke(_ => true));
        Assert.Equal(1, Volatile.Read(ref providerCalls));

        // Once the body exits, a new call succeeds.
        releaseBody.Set();
        await WaitUntilAsync(() => !bridge.IsBodyRunning);

        Assert.Equal(IamCredentialsStatus.Success, bridge.Invoke(_ => true));
        Assert.Equal(2, Volatile.Read(ref providerCalls));
    }

    [Theory]
    [InlineData(true)]  // IAM without TLS is rejected by the core.
    [InlineData(false)] // A provider without IAM authentication is rejected by the binding.
    public void RegistrationReleasedOnce_OnFailedNativeCreate(bool useIam)
    {
        int providerCalls = 0;
        var bridge = new IamCredentialsBridge(cancellationToken =>
        {
            _ = Interlocked.Increment(ref providerCalls);
            return Task.FromResult(Credentials);
        });

        using var iamConfig = new IamAuthConfig("cluster", ServiceType.ElastiCache, "us-east-1");
        var builder = new StandaloneClientConfigurationBuilder().WithAddress("localhost", 1);
        _ = useIam
            ? builder.WithAuthentication("username", iamConfig)
            : builder.WithAuthentication("username", "password");

        string? error = CreateNativeClient(builder.Build().Request, bridge);

        Assert.NotNull(error);
        if (useIam)
        {
            Assert.DoesNotContain("Credentials provider requires IAM authentication", error);
        }
        else
        {
            Assert.Contains("Credentials provider requires IAM authentication", error);
        }

        Assert.True(bridge.IsReleased);
        Assert.Equal(1, bridge.ReleaseRequestCount);
        Assert.Equal(0, Volatile.Read(ref providerCalls));
    }

    [Fact]
    public async Task RegistrationReleasedOnce_OnDisposalDuringInvocation()
    {
        // The fake clock is never advanced, so only disposal can end the invocation.
        var timeProvider = new FakeTimeProvider();
        using var bodyEntered = new ManualResetEventSlim();
        using var releaseBody = new ManualResetEventSlim();

        // Continuations run inline, so a late completion is fully processed when SetResult returns.
        var pendingTask = new TaskCompletionSource<AwsCredentials>();
        CancellationToken providerToken = default;
        int sinkCalls = 0;

        var bridge = new IamCredentialsBridge(
            cancellationToken =>
            {
                providerToken = cancellationToken;
                bodyEntered.Set();
                _ = releaseBody.Wait(WaitTimeout, TestContext.Current.CancellationToken);
                return pendingTask.Task;
            },
            timeProvider);
        _ = bridge.Register();

        var invocation = Task.Run(
            () => bridge.Invoke(credentials =>
            {
                _ = Interlocked.Increment(ref sinkCalls);
                return true;
            }),
            TestContext.Current.CancellationToken);
        Assert.True(bodyEntered.Wait(WaitTimeout, TestContext.Current.CancellationToken));

        // Disposing the client cancels the in-flight invocation while the body is still running.
        Assert.True(bridge.IsBodyRunning);
        bridge.Cancel();

        Assert.Equal(IamCredentialsStatus.Cancelled, await invocation.WaitAsync(WaitTimeout, TestContext.Current.CancellationToken));
        Assert.True(providerToken.IsCancellationRequested);

        // New invocations are rejected after disposal.
        Assert.Equal(IamCredentialsStatus.Cancelled, bridge.Invoke(_ => true));

        // Releasing again has no effect.
        bridge.Release();
        bridge.Release();

        Assert.True(bridge.IsReleased);

        // A late provider completion never reaches the native sink.
        releaseBody.Set();
        await WaitUntilAsync(() => !bridge.IsBodyRunning);
        pendingTask.SetResult(Credentials);

        Assert.Equal(0, Volatile.Read(ref sinkCalls));
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Invokes a bridge whose provider returns <paramref name="credentials"/> and copies the credentials
    /// out of the pinned buffers, as the native client does.
    /// </summary>
    private static CapturedCredentials InvokeAndCapture(AwsCredentials credentials)
    {
        var bridge = new IamCredentialsBridge(_ => Task.FromResult(credentials));
        CapturedCredentials? captured = null;

        var status = bridge.Invoke(ffiCredentials =>
        {
            captured = new CapturedCredentials(
                ReadUtf8(ffiCredentials.AccessKeyId, ffiCredentials.AccessKeyIdLen),
                ReadUtf8(ffiCredentials.SecretAccessKey, ffiCredentials.SecretAccessKeyLen),
                ffiCredentials.HasSessionToken == 1,
                ffiCredentials.HasSessionToken == 1 ? ReadUtf8(ffiCredentials.SessionToken, ffiCredentials.SessionTokenLen) : null,
                ffiCredentials.HasExpiresAt == 1,
                ffiCredentials.ExpiresAtUnixMs);
            return true;
        });

        Assert.Equal(IamCredentialsStatus.Success, status);
        return captured!;
    }

    private static string ReadUtf8(IntPtr pointer, nuint length)
    {
        byte[] bytes = new byte[(int)length];
        if (length > 0)
        {
            Marshal.Copy(pointer, bytes, 0, (int)length);
        }

        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Calls the native <c>create_client</c> entry point with the bridge's registration.
    /// </summary>
    /// <returns>The error message reported by the failure callback, or <see langword="null"/> on success.</returns>
    private static string? CreateNativeClient(ConnectionConfig config, IamCredentialsBridge bridge)
    {
        string? error = null;
        bool succeeded = false;

        SuccessAction onSuccess = (_, _) => succeeded = true;
        FailureAction onFailure = (_, message, _) => error = Marshal.PtrToStringUTF8(message);

        using FFI.ConnectionConfig request = config.ToFfi();
        FFI.CreateClientFfi(
            request.ToPtr(),
            Marshal.GetFunctionPointerForDelegate(onSuccess),
            Marshal.GetFunctionPointerForDelegate(onFailure),
            IntPtr.Zero,
            IntPtr.Zero,
            bridge.Register(),
            IamCredentialsBridge.InvokePointer,
            IamCredentialsBridge.ReleasePointer);

        GC.KeepAlive(onSuccess);
        GC.KeepAlive(onFailure);

        Assert.False(succeeded);
        return error;
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(WaitTimeout);
        while (!condition())
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10), timeout.Token);
        }
    }

    /// <summary>
    /// Credentials copied out of an <see cref="FFI.AwsCredentialsFfi"/>.
    /// </summary>
    private sealed record CapturedCredentials(
        string AccessKeyId,
        string SecretAccessKey,
        bool HasSessionToken,
        string? SessionToken,
        bool HasExpiresAt,
        long ExpiresAtUnixMs);

    #endregion
}
