// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

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

    // TODO add pubsub and other params to example and remarks
    /// <summary>
    /// Creates a new <see cref="GlideClusterClient" /> instance and establishes a connection to a cluster of Valkey servers.<br />
    /// </summary>
    /// <remarks>
    /// <b>Remarks:</b>
    /// Use this static method to create and connect a <see cref="GlideClusterClient" /> to a Valkey Cluster.<br />
    /// The client will automatically handle connection establishment, including cluster topology discovery and handling of authentication and TLS configurations.
    /// <list type="bullet">
    ///   <item>
    ///     <b>Authentication</b>: If credentials are provided, the client will attempt to authenticate using the specified username and password.
    ///   </item>
    ///   <item>
    ///     <b>TLS</b>: If <see cref="ClientConfigurationBuilder{T}.UseTls" /> is set to <see langword="true" />, the client will establish a secure connection using <c>TLS</c>.
    ///   </item>
    /// </list>
    /// <example>
    /// <code>
    /// using Valkey.Glide;
    /// using static Valkey.Glide.ConnectionConfiguration;
    ///
    /// var config = new ClusterClientConfigurationBuilder()
    ///     .WithAddress("address1.example.com", 6379)
    ///     .WithAddress("address2.example.com", 6379)
    ///     .WithAuthentication("user1", "passwordA")
    ///     .WithTls()
    ///     .Build();
    /// await using GlideClusterClient client = await GlideClusterClient.CreateClient(config);
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="config">The configuration options for the client, including cluster addresses, authentication credentials, TLS settings, periodic checks, and Pub/Sub subscriptions.</param>
    /// <returns>A task that resolves to a connected <see cref="GlideClient" /> instance.</returns>
    public static async Task<GlideClusterClient> CreateClient(ClusterClientConfiguration config)
        => await CreateClient(config, () => new GlideClusterClient());

    /// <inheritdoc/>
    public async Task<object?[]?> Exec(ClusterBatch batch, bool raiseOnError)
        => await Batch(batch, raiseOnError);

    /// <inheritdoc/>
    public async Task<object?[]?> Exec(ClusterBatch batch, bool raiseOnError, ClusterBatchOptions options)
        => batch.IsAtomic && options.RetryStrategy is not null
            ? throw new RequestException("Retry strategy is not supported for atomic batches (transactions).")
            : await Batch(batch, raiseOnError, options);

    /// <inheritdoc/>
    public async Task<ClusterValue<object?>> CustomCommand(IEnumerable<GlideString> args)
        => await Command(Request.CustomCommand([.. args], resp => ResponseConverters.HandleCustomCommandClusterValue(resp)));

    /// <inheritdoc/>
    public async Task<ClusterValue<object?>> CustomCommand(IEnumerable<GlideString> args, Route route)
        => await Command(Request.CustomCommand([.. args], resp => ResponseConverters.HandleCustomCommandClusterValue(resp, route)), route);

    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> InfoAsync() => await InfoAsync([]);

    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> InfoAsync(IEnumerable<InfoOptions.Section> sections)
        => await Command(Request.Info([.. sections]).ToMultiNodeValue());

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> InfoAsync(Route route) => await InfoAsync([], route);

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> InfoAsync(IEnumerable<InfoOptions.Section> sections, Route route)
        => await Command(Request.Info([.. sections]).ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route)
    {
        return await Command(Request.Echo(message).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc/>
    public override async Task<ValkeyValue> EchoAsync(ValkeyValue message)
        => await Command(Request.Echo(message), Route.Random);

    /// <inheritdoc/>
    public override async Task<ValkeyValue> PingAsync()
        => await Command(Request.Ping(), AllPrimaries);

    /// <inheritdoc/>
    public override async Task<ValkeyValue> PingAsync(ValkeyValue message)
        => await Command(Request.Ping(message), AllPrimaries);

    /// <inheritdoc/>
    public async Task<ValkeyValue> PingAsync(Route route)
        => await Command(Request.Ping(), route);

    /// <inheritdoc/>
    public async Task<ValkeyValue> PingAsync(ValkeyValue message, Route route)
        => await Command(Request.Ping(message), route);

    /// <inheritdoc/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern = default)
    {
        return await Command(Request.ConfigGetAsync(pattern).ToClusterValue(false), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route)
    {
        return await Command(Request.ConfigGetAsync(pattern).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc/>
    public async Task ConfigResetStatisticsAsync()
    {
        _ = await Command(Request.ConfigResetStatisticsAsync(), AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task ConfigResetStatisticsAsync(Route route)
    {
        _ = await Command(Request.ConfigResetStatisticsAsync(), route);
    }

    /// <inheritdoc/>
    public async Task ConfigRewriteAsync()
    {
        _ = await Command(Request.ConfigRewriteAsync(), Route.Random);
    }

    /// <inheritdoc/>
    public async Task ConfigRewriteAsync(Route route)
    {
        _ = await Command(Request.ConfigRewriteAsync(), route);
    }

    /// <inheritdoc/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
    {
        _ = await Command(Request.ConfigSetAsync(setting, value), AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route)
    {
        _ = await Command(Request.ConfigSetAsync(setting, value), route);
    }

    /// <inheritdoc/>
    public async Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters)
    {
        _ = await Command(Request.ConfigSetAsync(parameters), AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters, Route route)
    {
        _ = await Command(Request.ConfigSetAsync(parameters), route);
    }

    /// <inheritdoc/>
    public async Task<long> DatabaseSizeAsync()
        => await DatabaseSizeAsync(AllPrimaries);

    /// <inheritdoc/>
    public async Task<long> DatabaseSizeAsync(Route route)
    {
        ClusterValue<long> result = await Command(Request.DatabaseSizeAsync().ToClusterValue(false), route);
        return result.HasMultiData ? result.MultiValue.Values.Sum() : result.SingleValue;
    }

    /// <inheritdoc/>
    public async Task FlushAllDatabasesAsync()
        => await FlushAllDatabasesAsync(AllPrimaries);

    /// <inheritdoc/>
    public async Task FlushAllDatabasesAsync(FlushMode mode)
        => await FlushAllDatabasesAsync(mode, AllPrimaries);

    /// <inheritdoc/>
    public async Task FlushAllDatabasesAsync(Route route)
        => _ = await Command(Request.FlushAllDatabasesAsync(), route);

    /// <inheritdoc/>
    public async Task FlushAllDatabasesAsync(FlushMode mode, Route route)
        => _ = await Command(Request.FlushAllDatabasesAsync(mode), route);

    /// <inheritdoc/>
    public async Task FlushDatabaseAsync()
        => await FlushDatabaseAsync(AllPrimaries);

    /// <inheritdoc/>
    public async Task FlushDatabaseAsync(FlushMode mode)
        => await FlushDatabaseAsync(mode, AllPrimaries);

    /// <inheritdoc/>
    public async Task FlushDatabaseAsync(Route route)
        => _ = await Command(Request.FlushDatabaseAsync(), route);

    /// <inheritdoc/>
    public async Task FlushDatabaseAsync(FlushMode mode, Route route)
        => _ = await Command(Request.FlushDatabaseAsync(mode), route);

    /// <inheritdoc/>
    public async Task<Dictionary<string, DateTime>> LastSaveAsync()
    {
        ClusterValue<DateTime> result = await Command(Request.LastSaveAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, DateTime> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<DateTime>> LastSaveAsync(Route route)
    {
        return await Command(Request.LastSaveAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, DateTime>> TimeAsync()
    {
        ClusterValue<DateTime> result = await Command(Request.TimeAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, DateTime> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<DateTime>> TimeAsync(Route route)
    {
        return await Command(Request.TimeAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> LolwutAsync()
    {
        ClusterValue<string> result = await Command(Request.LolwutAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, string> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, string>> LolwutAsync(LolwutOptions options)
    {
        ClusterValue<string> result = await Command(Request.LolwutAsync(options).ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        return new Dictionary<string, string> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> LolwutAsync(Route route)
    {
        return await Command(Request.LolwutAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> LolwutAsync(LolwutOptions options, Route route)
    {
        return await Command(Request.LolwutAsync(options).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(IEnumerable<ValkeyValue> patterns)
    {
        return await Command(Request.ConfigGetAsync(patterns).ToClusterValue(false), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(IEnumerable<ValkeyValue> patterns, Route route)
    {
        return await Command(Request.ConfigGetAsync(patterns).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc/>
    public async Task<long[]> WaitAofAsync(long numlocal, long numreplicas, TimeSpan timeout, Route route)
        => await Command(Request.WaitAofAsync(numlocal, numreplicas, timeout), route);

    /// <inheritdoc/>
    public override async Task<ValkeyValue> ClientGetNameAsync()
        => await Command(Request.ClientGetName(), Route.Random);

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route)
        => await Command(Request.ClientGetNameCluster(route), route);

    /// <inheritdoc/>
    public override async Task<long> ClientIdAsync()
        => await Command(Request.ClientId(), Route.Random);

    /// <inheritdoc/>
    public async Task<ClusterValue<long>> ClientIdAsync(Route route)
    {
        return await Command(Request.ClientId().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc/>
    public override async Task SelectAsync(long index)
        => _ = await Command(Request.Select(index), Route.Random);

    /// <inheritdoc/>
    public async Task WatchAsync(IEnumerable<ValkeyKey> keys)
    {
        _ = await Command(Request.Watch(keys));
    }

    /// <inheritdoc/>
    public async Task UnwatchAsync()
    {
        _ = await Command(Request.Unwatch(), AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task UnwatchAsync(Route route)
    {
        _ = await Command(Request.Unwatch(), route);
    }

    /// <inheritdoc/>
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
    /// Iterates incrementally over keys in the cluster.
    /// </summary>
    /// <param name="cursor">The cursor to use for this iteration.</param>
    /// <param name="options">Optional scan options to filter results.</param>
    /// <returns>A tuple containing the next cursor and the keys found in this iteration.</returns>
    /// <seealso cref="ClusterScanCursor"/>
    /// <seealso cref="ScanOptions"/>
    public async Task<(ClusterScanCursor cursor, ValkeyKey[] keys)> ScanAsync(ClusterScanCursor cursor, ScanOptions? options = null)
    {
        string[] args = options?.ToArgs() ?? [];
        var (nextCursorId, keys) = await ClusterScanCommand(cursor.CursorId, args);
        return (new ClusterScanCursor(nextCursorId), keys);
    }

    /// <summary>
    /// Executes a cluster scan command with the given cursor and arguments.
    /// </summary>
    /// <param name="cursor">The cursor for the scan iteration.</param>
    /// <param name="args">Additional arguments for the scan command.</param>
    /// <returns>A tuple containing the next cursor and the keys found in this iteration.</returns>
    private async Task<(string cursor, ValkeyKey[] keys)> ClusterScanCommand(string cursor, string[] args)
    {
        var message = MessageContainer.GetMessageForCall();
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
            RequestClusterScanFfi(ClientPointer, (ulong)message.Index, cursorPtr, (ulong)args.Length, argsPtr, argLengthsPtr);
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
}
