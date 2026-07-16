// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Channels;

using static Valkey.Glide.Errors;
using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide;

/// <summary>
/// A client that monitors all commands processed by the server.
/// </summary>
/// <seealso href="https://valkey.io/commands/monitor/">Valkey commands - MONITOR</seealso>
public sealed class MonitorClient : IAsyncDisposable, IDisposable
{
    #region Private Fields

    /// <summary>
    /// Pointer to the native monitor client.
    /// </summary>
    private IntPtr _clientPtr;

    /// <summary>
    /// Synchronization object for thread-safe disposal.
    /// </summary>
    private readonly object _lock = new();

    /// <summary>
    /// Indicates whether the client has been disposed.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Channel for passing messages from the FFI callback to consumers.
    /// </summary>
    private readonly Channel<MonitorMessage> _channel;

    /// <summary>
    /// Reference to callback delegate to prevent GC collection.
    /// </summary>
    private readonly MonitorMessageCallback _callbackDelegate;

    #endregion
    #region Constructors & Builders

    /// <summary>
    /// Creates a new <see cref="MonitorClient"/> instance.
    /// </summary>
    /// <param name="config">The configuration for the monitor client.</param>
    /// <returns>A connected <see cref="MonitorClient"/> instance.</returns>
    /// <exception cref="ConnectionException">Thrown when the client fails to connect to the server.</exception>
    public static Task<MonitorClient> CreateClient(MonitorConfig config)
    {
        var client = new MonitorClient(IntPtr.Zero);

        MonitorConfigFfi monitorConfig = new()
        {
            Host = config.Host,
            Port = config.Port,
            UseTls = config.UseTls,
            Database = config.Database,
            Username = config.Username,
            Password = config.Password is not null ? new string(config.Password) : null,
        };

        IntPtr configPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MonitorConfigFfi>());
        try
        {
            Marshal.StructureToPtr(monitorConfig, configPtr, false);
            IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(client._callbackDelegate);

            IntPtr responsePtr = CreateMonitorClientFfi(configPtr, callbackPtr);
            try
            {
                MonitorConnectionResponse response = Marshal.PtrToStructure<MonitorConnectionResponse>(responsePtr);

                if (response.ConnPtr == IntPtr.Zero)
                {
                    string errorMessage = response.ConnectionErrorMessage != IntPtr.Zero
                        ? Marshal.PtrToStringAnsi(response.ConnectionErrorMessage) ?? "Unknown error"
                        : "Failed to create monitor client";
                    throw new ConnectionException(errorMessage);
                }

                client._clientPtr = response.ConnPtr;
                return Task.FromResult(client);
            }
            catch
            {
                client._disposed = true;
                throw;
            }
            finally
            {
                FreeMonitorConnectionResponseFfi(responsePtr);
            }
        }
        finally
        {
            Marshal.DestroyStructure<MonitorConfigFfi>(configPtr);
            Marshal.FreeHGlobal(configPtr);
        }
    }

    private MonitorClient(IntPtr clientPtr)
    {
        _clientPtr = clientPtr;
        _channel = Channel.CreateUnbounded<MonitorMessage>(new UnboundedChannelOptions { SingleWriter = true });
        _callbackDelegate = OnMonitorMessage;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Returns an async stream of monitor messages.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the enumeration.</param>
    /// <returns>An <see cref="IAsyncEnumerable{MonitorMessage}"/> that yields messages as they arrive.</returns>
    /// <remarks>
    /// <note>
    /// The stream will not terminate automatically on connection loss. Users should always
    /// provide a <see cref="CancellationToken"/> to avoid hanging on stale connections.
    /// </note>
    /// <example>
    /// <code>
    /// using var config = new MonitorConfig("localhost", 6379);
    /// await using var monitor = await MonitorClient.CreateClient(config);
    ///
    /// using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
    /// await foreach (var msg in monitor.GetMessagesAsync(cts.Token))
    /// {
    ///     Console.WriteLine($"{msg.Timestamp}: {msg.Command} {string.Join(" ", msg.Args)}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public async IAsyncEnumerable<MonitorMessage> GetMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await foreach (MonitorMessage message in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return message;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _ = _channel.Writer.TryComplete();
            CloseMonitorClientFfi(_clientPtr);
            _clientPtr = IntPtr.Zero;
        }

        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    #endregion
    #region Private Methods

    private void OnMonitorMessage(
        double timestampUnix,
        ushort database,
        IntPtr clientAddrPtr,
        long clientAddrLen,
        IntPtr commandPtr,
        long commandLen,
        long argsCount,
        IntPtr argsPtrs,
        IntPtr argsLens)
    {
        try
        {
            var clientAddressBytes = new byte[clientAddrLen];
            Marshal.Copy(clientAddrPtr, clientAddressBytes, 0, (int)clientAddrLen);
            var clientAddress = System.Text.Encoding.UTF8.GetString(clientAddressBytes);

            var commandBytes = new byte[commandLen];
            Marshal.Copy(commandPtr, commandBytes, 0, (int)commandLen);
            var command = System.Text.Encoding.UTF8.GetString(commandBytes);

            var args = new string[argsCount];
            for (int i = 0; i < argsCount; i++)
            {
                var argPtr = Marshal.ReadIntPtr(argsPtrs, i * IntPtr.Size);
                var argLen = Marshal.ReadInt64(argsLens, i * sizeof(long));

                var argBytes = new byte[argLen];
                Marshal.Copy(argPtr, argBytes, 0, (int)argLen);
                args[i] = System.Text.Encoding.UTF8.GetString(argBytes);
            }

            var timestamp = DateTimeOffset.UnixEpoch.AddSeconds(timestampUnix);

            var message = new MonitorMessage(timestamp, database, clientAddress, command, args);
            _ = _channel.Writer.TryWrite(message);
        }
        catch (Exception ex)
        {
            Logger.Log(Level.Error, "MonitorClient", $"Failed to process monitor message: {ex.Message}");
        }
    }

    #endregion
}
