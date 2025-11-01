// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

using Valkey.Glide.Internals;
using Valkey.Glide.Pipeline;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;
using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.ResponseConverters;
using static Valkey.Glide.Internals.ResponseHandler;
using static Valkey.Glide.Pipeline.Options;

namespace Valkey.Glide;

public abstract partial class BaseClient : IDisposable, IAsyncDisposable
{
    #region public methods
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        lock (_lock)
        {
            if (ClientPointer == IntPtr.Zero)
            {
                return;
            }
            MessageContainer.DisposeWithError(null);
            CloseClientFfi(ClientPointer);
            ClientPointer = IntPtr.Zero;
        }
    }

    public async ValueTask DisposeAsync() => await Task.Run(Dispose);

    public override string ToString() => $"{GetType().Name} {{ 0x{ClientPointer:X} {_clientInfo} }}";

    public override int GetHashCode() => (int)ClientPointer;

    /// <summary>
    /// Manually refresh the IAM authentication token.
    /// This method is only available when the client is configured with IAM authentication.
    /// </summary>
    /// <returns>A task that completes when the refresh attempt finishes.</returns>
    public async Task RefreshIamTokenAsync()
    {
        Message message = MessageContainer.GetMessageForCall();
        RefreshIamTokenFfi(ClientPointer, (ulong)message.Index);
        IntPtr response = await message;
        try
        {
            HandleResponse(response);
        }
        finally
        {
            FreeResponse(response);
        }
    }

    #endregion public methods

    #region protected methods
    protected static async Task<T> CreateClient<T>(BaseClientConfiguration config, Func<T> ctor) where T : BaseClient
    {
        T client = ctor();

        nint successCallbackPointer = Marshal.GetFunctionPointerForDelegate(client._successCallbackDelegate);
        nint failureCallbackPointer = Marshal.GetFunctionPointerForDelegate(client._failureCallbackDelegate);

        using FFI.ConnectionConfig request = config.Request.ToFfi();
        Message message = client.MessageContainer.GetMessageForCall();
        CreateClientFfi(request.ToPtr(), successCallbackPointer, failureCallbackPointer);
        client.ClientPointer = await message; // This will throw an error thru failure callback if any

        if (client.ClientPointer == IntPtr.Zero)
        {
            throw new ConnectionException("Failed creating a client");
        }

        return client;
    }

    protected BaseClient()
    {
        _successCallbackDelegate = SuccessCallback;
        _failureCallbackDelegate = FailureCallback;
        MessageContainer = new(this);
    }

    protected internal delegate T ResponseHandler<T>(IntPtr response);

    /// <typeparam name="R">Type received from server.</typeparam>
    /// <typeparam name="T">Type we return to the user.</typeparam>
    /// <param name="command"></param>
    /// <param name="route"></param>
    internal virtual async Task<T> Command<R, T>(Cmd<R, T> command, Route? route = null)
    {
        // 1. Create Cmd which wraps CmdInfo and manages all memory allocations
        using Cmd cmd = command.ToFfi();

        // 2. Allocate memory for route
        using FFI.Route? ffiRoute = route?.ToFfi();

        // 3. Sumbit request to the rust part
        Message message = MessageContainer.GetMessageForCall();
        CommandFfi(ClientPointer, (ulong)message.Index, cmd.ToPtr(), ffiRoute?.ToPtr() ?? IntPtr.Zero);

        // 4. Get a response and Handle it
        IntPtr response = await message;
        try
        {
            return HandleServerValue(HandleResponse(response), command.IsNullable, command.Converter, command.AllowConverterToHandleNull);
        }
        finally
        {
            FreeResponse(response);
        }

        // All memory allocated is auto-freed by `using` operator
    }

    internal async Task<object?[]?> Batch<T>(BaseBatch<T> batch, bool raiseOnError, BaseBatchOptions? options = null) where T : BaseBatch<T>
    {
        // 1. Allocate memory for batch, which allocates all nested Cmds
        using FFI.Batch ffiBatch = batch.ToFFI();

        // 2. Allocate memory for options
        using FFI.BatchOptions? ffiOptions = options?.ToFfi();

        // 3. Sumbit request to the rust part
        Message message = MessageContainer.GetMessageForCall();
        BatchFfi(ClientPointer, (ulong)message.Index, ffiBatch.ToPtr(), raiseOnError, ffiOptions?.ToPtr() ?? IntPtr.Zero);

        // 4. Get a response and Handle it
        IntPtr response = await message;
        try
        {
            return batch.ConvertResponse(HandleServerValue(HandleResponse(response), true, (object?[]? o) => o));
        }
        finally
        {
            FreeResponse(response);
        }

        // All memory allocated is auto-freed by `using` operator
    }
    protected Version? ParseServerVersion(string response)
    {
        var versionMatch = System.Text.RegularExpressions.Regex.Match(response, @"(?:valkey_version|redis_version):([\d\.]+)");
        return versionMatch.Success ? new(versionMatch.Groups[1].Value) : null;
    }
    #endregion protected methods

    #region protected fields
    protected Version? _serverVersion; // cached server version
    protected static readonly Version DefaultServerVersion = new(8, 0, 0);
    #endregion protected fields

    #region internal fields
    /// Raw pointer to the underlying native client.
    internal IntPtr ClientPointer;
    internal readonly MessageContainer MessageContainer;
    #endregion internal fields

    #region private methods
    private void SuccessCallback(ulong index, IntPtr ptr) =>
        // Work needs to be offloaded from the calling thread, because otherwise we might starve the client's thread pool.
        Task.Run(() => MessageContainer.GetMessage((int)index).SetResult(ptr));

    private void FailureCallback(ulong index, IntPtr strPtr, RequestErrorType errType)
    {
        string str = Marshal.PtrToStringAnsi(strPtr)!;
        // Work needs to be offloaded from the calling thread, because otherwise we might starve the client's thread pool.
        _ = Task.Run(() => MessageContainer.GetMessage((int)index).SetException(Create(errType, str)));
    }

    ~BaseClient() => Dispose();

    internal void SetInfo(string info) => _clientInfo = info;

    protected abstract Task<Version> GetServerVersionAsync();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SuccessAction(ulong index, IntPtr ptr);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FailureAction(ulong index, IntPtr strPtr, RequestErrorType err);
    #endregion private methods

    #region private fields

    /// Held as a measure to prevent the delegate being garbage collected. These are delegated once
    /// and held in order to prevent the cost of marshalling on each function call.
    private readonly FailureAction _failureCallbackDelegate;

    /// Held as a measure to prevent the delegate being garbage collected. These are delegated once
    /// and held in order to prevent the cost of marshalling on each function call.
    private readonly SuccessAction _successCallbackDelegate;

    private readonly object _lock = new();
    private string _clientInfo = ""; // used to distinguish and identify clients during tests

    #endregion private fields
}
