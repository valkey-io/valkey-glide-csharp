// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;
using Valkey.Glide.Pipeline;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;
using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.ResponseHandler;
using static Valkey.Glide.Pipeline.Options;
using static Valkey.Glide.Route;

namespace Valkey.Glide;

/// <summary>
/// Client used for connection to cluster servers. Use <see cref="CreateClient"/> to request a client.
/// </summary>
/// <seealso href="https://glide.valkey.io/how-to/client-initialization/">Valkey GLIDE – Client Initialization</seealso>
public sealed partial class GlideClusterClient :
    BaseClient,
    IGlideClusterClient
{
    private GlideClusterClient() { }

    /// <summary>
    /// Creates a new <see cref="GlideClusterClient" /> instance and establishes a connection to a cluster of Valkey servers.<br />
    /// </summary>
    /// <param name="config">The configuration options for the client.</param>
    /// <returns>A task that resolves to a connected <see cref="GlideClusterClient" /> instance.</returns>
    public static async Task<GlideClusterClient> CreateClient(ClusterClientConfiguration config)
        => await CreateClient(config, () => new GlideClusterClient());

    /// <inheritdoc cref="IGenericClusterCommands.CustomCommand(IEnumerable{GlideString})"/>
    public async Task<ClusterValue<object?>> CustomCommand(IEnumerable<GlideString> args)
        => await Command(Request.CustomCommand([.. args], resp => ResponseConverters.HandleCustomCommandClusterValue(resp)));

    /// <inheritdoc cref="IGenericClusterCommands.CustomCommand(IEnumerable{GlideString}, Route)"/>
    public async Task<ClusterValue<object?>> CustomCommand(IEnumerable<GlideString> args, Route route)
        => await Command(Request.CustomCommand([.. args], resp => ResponseConverters.HandleCustomCommandClusterValue(resp, route)), route);

    /// <inheritdoc cref="IGenericClusterCommands.Exec(ClusterBatch, bool)"/>
    public async Task<object?[]?> Exec(ClusterBatch batch, bool raiseOnError)
        => await Batch(batch, raiseOnError);

    /// <inheritdoc cref="IGenericClusterCommands.Exec(ClusterBatch, bool, ClusterBatchOptions)"/>
    public async Task<object?[]?> Exec(ClusterBatch batch, bool raiseOnError, ClusterBatchOptions options)
        => batch.IsAtomic && options.RetryStrategy is not null
            ? throw new RequestException("Retry strategy is not supported for atomic batches (transactions).")
            : await Batch(batch, raiseOnError, options);

    /// <inheritdoc cref="BaseClient.GetServerVersionAsync()"/>
    protected override async Task<Version> GetServerVersionAsync()
    {
        if (_serverVersion == null)
        {
            try
            {
                var infoResponse = await Command(Request.Info([InfoOptions.Section.SERVER]).ToClusterValue(true), Route.Random);
                _serverVersion = ParseServerVersion(infoResponse.SingleValue) ?? DefaultServerVersion;
            }
            catch
            {
                _serverVersion = DefaultServerVersion;
            }
        }

        return _serverVersion;
    }

    /// <summary>
    /// Executes a cluster scan command with the given cursor and arguments.
    /// </summary>
    /// <param name="cursor">The cursor for the scan iteration.</param>
    /// <param name="args">Additional arguments for the scan command.</param>
    /// <returns>A tuple containing the next cursor and the keys found in this iteration.</returns>
    private async Task<(ClusterScanCursor cursor, ValkeyKey[] keys)> ClusterScanCommand(ClusterScanCursor cursor, string[] args)
    {
        var message = MessageContainer.GetMessageForCall();
        IntPtr cursorPtr = Marshal.StringToHGlobalAnsi(cursor.CursorId);

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
            RequestClusterScanFfi(ClientPointer, (ulong)message.Index, cursorPtr, (ulong)args.Length, argsPtr, argLengthsPtr);
            IntPtr response = await message;

            try
            {
                var result = HandleResponse(response);
                var array = (object[])result!;
                var nextCursor = new ClusterScanCursor(array[0]!.ToString()!);
                var keys = ((object[])array[1]!).Cast<GlideString>().Select(gs => (ValkeyKey)gs.Bytes).ToArray();
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
}
