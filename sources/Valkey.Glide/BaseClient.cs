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
            if (_clientPointer == IntPtr.Zero)
            {
                return;
            }
            _messageContainer.DisposeWithError(null);
            CloseClientFfi(_clientPointer);
            _clientPointer = IntPtr.Zero;
        }
    }

    public async ValueTask DisposeAsync() => await Task.Run(Dispose);

    public override string ToString() => $"{GetType().Name} {{ 0x{_clientPointer:X} {_clientInfo} }}";

    public override int GetHashCode() => (int)_clientPointer;

    #endregion public methods

    #region protected methods
    protected static async Task<T> CreateClient<T>(BaseClientConfiguration config, Func<T> ctor) where T : BaseClient
    {
        T client = ctor();

        nint successCallbackPointer = Marshal.GetFunctionPointerForDelegate(client._successCallbackDelegate);
        nint failureCallbackPointer = Marshal.GetFunctionPointerForDelegate(client._failureCallbackDelegate);

        using FFI.ConnectionConfig request = config.Request.ToFfi();
        Message message = client._messageContainer.GetMessageForCall();
        CreateClientFfi(request.ToPtr(), successCallbackPointer, failureCallbackPointer);
        client._clientPointer = await message; // This will throw an error thru failure callback if any

        if (client._clientPointer == IntPtr.Zero)
        {
            throw new ConnectionException("Failed creating a client");
        }

        return client;
    }

    protected BaseClient()
    {
        _successCallbackDelegate = SuccessCallback;
        _failureCallbackDelegate = FailureCallback;
        _messageContainer = new(this);
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
        Message message = _messageContainer.GetMessageForCall();
        CommandFfi(_clientPointer, (ulong)message.Index, cmd.ToPtr(), ffiRoute?.ToPtr() ?? IntPtr.Zero);

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
        Message message = _messageContainer.GetMessageForCall();
        BatchFfi(_clientPointer, (ulong)message.Index, ffiBatch.ToPtr(), raiseOnError, ffiOptions?.ToPtr() ?? IntPtr.Zero);

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

    #region private methods
    private void SuccessCallback(ulong index, IntPtr ptr) =>
        // Work needs to be offloaded from the calling thread, because otherwise we might starve the client's thread pool.
        Task.Run(() => _messageContainer.GetMessage((int)index).SetResult(ptr));

    private void FailureCallback(ulong index, IntPtr strPtr, RequestErrorType errType)
    {
        string str = Marshal.PtrToStringAnsi(strPtr)!;
        // Work needs to be offloaded from the calling thread, because otherwise we might starve the client's thread pool.
        _ = Task.Run(() => _messageContainer.GetMessage((int)index).SetException(Create(errType, str)));
    }

    ~BaseClient() => Dispose();

    internal void SetInfo(string info) => _clientInfo = info;

    protected abstract Task<Version> GetServerVersionAsync();

    /// <summary>
    /// Executes a cluster scan command with the given cursor and arguments.
    /// </summary>
    /// <param name="cursor">The cursor for the scan iteration.</param>
    /// <param name="args">Additional arguments for the scan command.</param>
    /// <returns>A tuple containing the next cursor and the keys found in this iteration.</returns>
    protected async Task<(string cursor, ValkeyKey[] keys)> ClusterScanCommand(string cursor, string[] args)
    {
        var message = _messageContainer.GetMessageForCall();
        IntPtr cursorPtr = Marshal.StringToHGlobalAnsi(cursor);

        IntPtr[]? argPtrs = null;
        IntPtr argsPtr = IntPtr.Zero;
        IntPtr argLengthsPtr = IntPtr.Zero;

        try
        {
            if (args.Length > 0)
            {
                // 1. Get a pointer to the array of argument string pointers.
                // Example: if args = ["MATCH", "key*"], then argPtrs[0] points
                // to "MATCH", argPtrs[1] points to "key*", and argsPtr points
                // to the argsPtrs array.
                argPtrs = [.. args.Select(Marshal.StringToHGlobalAnsi)];
                argsPtr = Marshal.AllocHGlobal(IntPtr.Size * args.Length);
                Marshal.Copy(argPtrs, 0, argsPtr, args.Length);

                // 2. Get a pointer to an array of argument string lengths.
                // Example: if args = ["MATCH", "key*"], then argLengths[0] = 5
                // (length of "MATCH"), argLengths[1] = 4 (length of "key*"),
                // and argLengthsPtr points to the argLengths array.
                var argLengths = args.Select(arg => (ulong)arg.Length).ToArray();
                argLengthsPtr = Marshal.AllocHGlobal(sizeof(ulong) * args.Length);
                Marshal.Copy(argLengths.Select(l => (long)l).ToArray(), 0, argLengthsPtr, args.Length);
            }

            // Submit request to Rust and wait for response.
            RequestClusterScanFfi(_clientPointer, (ulong)message.Index, cursorPtr, (ulong)args.Length, argsPtr, argLengthsPtr);
            IntPtr response = await message;

            try
            {
                var result = HandleResponse(response);
                var array = (object[])result!;
                var nextCursor = array[0]!.ToString()!;
                var keys = ((object[])array[1]!).Select(k => new ValkeyKey(k!.ToString())).ToArray();
                return (nextCursor, keys);
            }
            finally
            {
                FreeResponse(response);
            }
        }
        finally
        {
            // Clean up args memory
            if (argLengthsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(argLengthsPtr);
            }

            if (argsPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(argsPtr);
            }

            if (argPtrs != null)
            {
                Array.ForEach(argPtrs, Marshal.FreeHGlobal);
            }

            // Clean up cursor in Rust
            RemoveClusterScanCursorFfi(cursorPtr);
            Marshal.FreeHGlobal(cursorPtr);
        }
    }

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

    /// Raw pointer to the underlying native client.
    private IntPtr _clientPointer;
    private readonly MessageContainer _messageContainer;
    private readonly object _lock = new();
    private string _clientInfo = ""; // used to distinguish and identify clients during tests

    #endregion private fields
}
