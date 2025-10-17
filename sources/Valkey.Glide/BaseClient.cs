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

            // Clean up PubSub resources
            CleanupPubSubResources();

            _messageContainer.DisposeWithError(null);
            CloseClientFfi(_clientPointer);
            _clientPointer = IntPtr.Zero;
        }
    }

    public async ValueTask DisposeAsync() => await Task.Run(Dispose);

    public override string ToString() => $"{GetType().Name} {{ 0x{_clientPointer:X} {_clientInfo} }}";

    public override int GetHashCode() => (int)_clientPointer;

    /// <summary>
    /// Get the PubSub message queue for manual message retrieval.
    /// Returns null if no PubSub subscriptions are configured.
    /// Uses thread-safe access to prevent race conditions.
    /// </summary>
    public PubSubMessageQueue? PubSubQueue
    {
        get
        {
            lock (_pubSubLock)
            {
                return _pubSubHandler?.GetQueue();
            }
        }
    }

    /// <summary>
    /// Indicates whether this client has PubSub subscriptions configured.
    /// Uses volatile read for thread-safe access without locking.
    /// </summary>
    public bool HasPubSubSubscriptions => _pubSubHandler != null;

    #endregion public methods

    #region protected methods
    protected static async Task<T> CreateClient<T>(BaseClientConfiguration config, Func<T> ctor) where T : BaseClient
    {
        T client = ctor();

        nint successCallbackPointer = Marshal.GetFunctionPointerForDelegate(client._successCallbackDelegate);
        nint failureCallbackPointer = Marshal.GetFunctionPointerForDelegate(client._failureCallbackDelegate);

        // Get PubSub callback pointer if PubSub subscriptions are configured
        nint pubsubCallbackPointer = IntPtr.Zero;
        if (config.Request.PubSubSubscriptions != null)
        {
            pubsubCallbackPointer = Marshal.GetFunctionPointerForDelegate(client._pubsubCallbackDelegate);
        }

        using FFI.ConnectionConfig request = config.Request.ToFfi();
        Message message = client._messageContainer.GetMessageForCall();
        CreateClientFfi(request.ToPtr(), successCallbackPointer, failureCallbackPointer, pubsubCallbackPointer);
        client._clientPointer = await message; // This will throw an error thru failure callback if any

        if (client._clientPointer != IntPtr.Zero)
        {
            // Initialize PubSub handler if subscriptions are configured
            client.InitializePubSubHandler(config.Request.PubSubSubscriptions);

            // Initialize server version after successful connection
            await client.InitializeServerVersionAsync();
            return client;
        }

        throw new ConnectionException("Failed creating a client");
    }

    protected BaseClient()
    {
        _successCallbackDelegate = SuccessCallback;
        _failureCallbackDelegate = FailureCallback;
        _pubsubCallbackDelegate = PubSubCallback;
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
    #endregion protected methods

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

    private void PubSubCallback(
        uint pushKind,
        IntPtr messagePtr,
        long messageLen,
        IntPtr channelPtr,
        long channelLen,
        IntPtr patternPtr,
        long patternLen)
    {
        // Work needs to be offloaded from the calling thread, because otherwise we might starve the client's thread pool.
        _ = Task.Run(() =>
        {
            try
            {
                // Only process actual message notifications, ignore subscription confirmations
                if (!IsMessageNotification((PushKind)pushKind))
                {
                    Logger.Log(Level.Debug, "PubSubCallback", $"PubSub notification received: {(PushKind)pushKind}");
                    return;
                }

                // Marshal the message from FFI callback parameters
                PubSubMessage message = MarshalPubSubMessage(
                    (PushKind)pushKind,
                    messagePtr,
                    messageLen,
                    channelPtr,
                    channelLen,
                    patternPtr,
                    patternLen);

                // Process the message through the handler
                HandlePubSubMessage(message);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, "PubSubCallback", $"Error in PubSub callback: {ex.Message}", ex);
            }
        });
    }

    private static bool IsMessageNotification(PushKind pushKind) =>
        pushKind switch
        {
            PushKind.PushMessage => true,      // Regular channel message
            PushKind.PushPMessage => true,     // Pattern-based message
            PushKind.PushSMessage => true,     // Sharded channel message
            _ => false                         // All other types are confirmations/notifications
        };

    private static PubSubMessage MarshalPubSubMessage(
        PushKind pushKind,
        IntPtr messagePtr,
        long messageLen,
        IntPtr channelPtr,
        long channelLen,
        IntPtr patternPtr,
        long patternLen)
    {
        // Marshal the raw byte pointers to byte arrays
        byte[] messageBytes = new byte[messageLen];
        Marshal.Copy(messagePtr, messageBytes, 0, (int)messageLen);

        byte[] channelBytes = new byte[channelLen];
        Marshal.Copy(channelPtr, channelBytes, 0, (int)channelLen);

        byte[]? patternBytes = null;
        if (patternPtr != IntPtr.Zero && patternLen > 0)
        {
            patternBytes = new byte[patternLen];
            Marshal.Copy(patternPtr, patternBytes, 0, (int)patternLen);
        }

        // Convert to strings (assuming UTF-8 encoding)
        string message = System.Text.Encoding.UTF8.GetString(messageBytes);
        string channel = System.Text.Encoding.UTF8.GetString(channelBytes);
        string? pattern = patternBytes != null ? System.Text.Encoding.UTF8.GetString(patternBytes) : null;

        // Create the appropriate PubSubMessage based on whether pattern is present
        return pattern != null
            ? new PubSubMessage(message, channel, pattern)
            : new PubSubMessage(message, channel);
    }

    ~BaseClient() => Dispose();

    internal void SetInfo(string info) => _clientInfo = info;

    protected abstract Task InitializeServerVersionAsync();

    /// <summary>
    /// Initializes PubSub message handling if PubSub subscriptions are configured.
    /// Uses thread-safe initialization to ensure proper visibility across threads.
    /// </summary>
    /// <param name="config">The PubSub subscription configuration.</param>
    private void InitializePubSubHandler(BasePubSubSubscriptionConfig? config)
    {
        if (config == null)
        {
            return;
        }

        // Create the PubSub message handler with thread-safe initialization
        lock (_pubSubLock)
        {
            _pubSubHandler = new PubSubMessageHandler(config.Callback, config.Context);
        }
    }

    /// <summary>
    /// Handles incoming PubSub messages from the FFI layer.
    /// This method is called directly by the FFI callback and uses thread-safe access to the handler.
    /// </summary>
    /// <param name="message">The PubSub message to handle.</param>
    internal virtual void HandlePubSubMessage(PubSubMessage message)
    {
        // Thread-safe access to handler - use local copy to avoid race conditions
        PubSubMessageHandler? handler;
        lock (_pubSubLock)
        {
            handler = _pubSubHandler;
        }

        if (handler != null)
        {
            try
            {
                handler.HandleMessage(message);
            }
            catch (Exception ex)
            {
                // Log the error but don't let exceptions escape
                Logger.Log(Level.Error, "BaseClient", $"Error handling PubSub message: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Cleans up PubSub resources during client disposal with proper synchronization.
    /// Uses locking to coordinate safe disposal and prevent conflicts with concurrent message processing.
    /// </summary>
    private void CleanupPubSubResources()
    {
        PubSubMessageHandler? handler = null;

        // Acquire lock and capture handler reference, then set to null
        lock (_pubSubLock)
        {
            handler = _pubSubHandler;
            _pubSubHandler = null;
        }

        // Dispose outside of lock to prevent deadlocks
        if (handler != null)
        {
            try
            {
                // Create a task to dispose the handler with timeout
                var disposeTask = Task.Run(() => handler.Dispose());

                // Wait for disposal with timeout (5 seconds)
                if (!disposeTask.Wait(TimeSpan.FromSeconds(5)))
                {
                    Logger.Log(Level.Warn, "BaseClient",
                        "PubSub handler disposal did not complete within timeout (5 seconds)");
                }
            }
            catch (AggregateException ex)
            {
                // Log the error but continue with disposal
                Logger.Log(Level.Warn, "BaseClient",
                    $"Error cleaning up PubSub resources: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Log the error but continue with disposal
                Logger.Log(Level.Warn, "BaseClient",
                    $"Error cleaning up PubSub resources: {ex.Message}", ex);
            }
        }
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SuccessAction(ulong index, IntPtr ptr);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void FailureAction(ulong index, IntPtr strPtr, RequestErrorType err);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void PubSubAction(
        uint pushKind,
        IntPtr messagePtr,
        long messageLen,
        IntPtr channelPtr,
        long channelLen,
        IntPtr patternPtr,
        long patternLen);
    #endregion private methods

    #region private fields

    /// Held as a measure to prevent the delegate being garbage collected. These are delegated once
    /// and held in order to prevent the cost of marshalling on each function call.
    private readonly FailureAction _failureCallbackDelegate;

    /// Held as a measure to prevent the delegate being garbage collected. These are delegated once
    /// and held in order to prevent the cost of marshalling on each function call.
    private readonly SuccessAction _successCallbackDelegate;

    /// Held as a measure to prevent the delegate being garbage collected. These are delegated once
    /// and held in order to prevent the cost of marshalling on each function call.
    private readonly PubSubAction _pubsubCallbackDelegate;

    /// Raw pointer to the underlying native client.
    private IntPtr _clientPointer;
    private readonly MessageContainer _messageContainer;
    private readonly object _lock = new();
    private string _clientInfo = ""; // used to distinguish and identify clients during tests
    protected Version? _serverVersion; // cached server version

    /// PubSub message handler for routing messages to callbacks or queues.
    /// Uses volatile to ensure visibility across threads without locking on every read.
    private volatile PubSubMessageHandler? _pubSubHandler;

    /// Lock object for coordinating PubSub handler access and disposal.
    private readonly object _pubSubLock = new();

    #endregion private fields
}
