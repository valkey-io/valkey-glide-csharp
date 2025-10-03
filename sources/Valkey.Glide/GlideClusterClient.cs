// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;
using Valkey.Glide.Pipeline;

using static Valkey.Glide.ConnectionConfiguration;
using static Valkey.Glide.Errors;
using static Valkey.Glide.Pipeline.Options;
using static Valkey.Glide.Route;

namespace Valkey.Glide;

// TODO add wiki link
/// <summary>
/// Client used for connection to cluster servers. Use <see cref="CreateClient"/> to request a client.
/// </summary>
public sealed class GlideClusterClient : BaseClient, IGenericClusterCommands, IServerManagementClusterCommands, IConnectionManagementClusterCommands
{
    private GlideClusterClient() { }

    // TODO add pubsub and other params to example and remarks
    /// <summary>
    /// Creates a new <see cref="GlideClusterClient" /> instance and establishes a connection to a cluster of Valkey servers.<br />
    /// Use this static method to create and connect a <see cref="GlideClusterClient" /> to a Valkey Cluster. The client will
    /// automatically handle connection establishment, including cluster topology discovery and handling of authentication and TLS configurations.
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
    /// using Glide;
    /// using static Glide.ConnectionConfiguration;
    ///
    /// var config = new ClusterClientConfigurationBuilder()
    ///     .WithAddress("address1.example.com", 6379)
    ///     .WithAddress("address2.example.com", 6379)
    ///     .WithAuthentication("user1", "passwordA")
    ///     .WithTls()
    ///     .Build();
    /// using GlideClusterClient client = await GlideClusterClient.CreateClient(config);
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="config">The configuration options for the client, including cluster addresses, authentication credentials, TLS settings, periodic checks, and Pub/Sub subscriptions.</param>
    /// <returns>A task that resolves to a connected <see cref="GlideClient" /> instance.</returns>
    public static async Task<GlideClusterClient> CreateClient(ClusterClientConfiguration config)
        => await CreateClient(config, () => new GlideClusterClient());

    public async Task<object?[]?> Exec(ClusterBatch batch, bool raiseOnError)
        => await Batch(batch, raiseOnError);

    public async Task<object?[]?> Exec(ClusterBatch batch, bool raiseOnError, ClusterBatchOptions options)
        => batch.IsAtomic && options.RetryStrategy is not null
            ? throw new RequestException("Retry strategy is not supported for atomic batches (transactions).")
            : await Batch(batch, raiseOnError, options);

    public async Task<ClusterValue<object?>> CustomCommand(GlideString[] args)
        => await Command(Request.CustomCommand(args, resp => ResponseConverters.HandleCustomCommandClusterValue(resp)));

    public async Task<ClusterValue<object?>> CustomCommand(GlideString[] args, Route route)
        => await Command(Request.CustomCommand(args, resp => ResponseConverters.HandleCustomCommandClusterValue(resp, route)), route);

    public async Task<Dictionary<string, string>> InfoAsync() => await InfoAsync([]);

    public async Task<Dictionary<string, string>> InfoAsync(InfoOptions.Section[] sections)
        => await Command(Request.Info(sections).ToMultiNodeValue());

    public async Task<ClusterValue<string>> InfoAsync(Route route) => await InfoAsync([], route);

    public async Task<ClusterValue<string>> InfoAsync(InfoOptions.Section[] sections, Route route)
        => await Command(Request.Info(sections).ToClusterValue(route is SingleNodeRoute), route);

    public async Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Echo(message).ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Echo(message), Route.Random);
    }

    public async Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Ping(), AllPrimaries);
    }

    public async Task<TimeSpan> PingAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Ping(message), AllPrimaries);
    }

    public async Task<TimeSpan> PingAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Ping(), route);
    }

    public async Task<TimeSpan> PingAsync(ValkeyValue message, Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Ping(message), route);
    }

    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ConfigGetAsync(pattern).ToClusterValue(false), Route.AllPrimaries);
    }

    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ConfigGetAsync(pattern).ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigResetStatisticsAsync(), AllPrimaries);
    }

    public async Task ConfigResetStatisticsAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigResetStatisticsAsync(), route);
    }

    public async Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigRewriteAsync(), Route.Random);
    }

    public async Task ConfigRewriteAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigRewriteAsync(), route);
    }

    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigSetAsync(setting, value), AllPrimaries);
    }

    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.ConfigSetAsync(setting, value), route);
    }

    public async Task<Dictionary<string, long>> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        Utils.Requires<ArgumentException>(database == -1, "Different databases for this command are not supported by GLIDE");
        ClusterValue<long> result = await Command(Request.DatabaseSizeAsync(database).ToClusterValue(false), AllPrimaries);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }

        // If we got a single value, create a dictionary with a single entry
        // This can happen when the server aggregates results or there's only one primary node
        return new Dictionary<string, long> { ["aggregated"] = result.SingleValue };
    }

    public async Task<ClusterValue<long>> DatabaseSizeAsync(Route route, int database = -1, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        Utils.Requires<ArgumentException>(database == -1, "Different databases for this command are not supported by GLIDE");
        return await Command(Request.DatabaseSizeAsync(database).ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.FlushAllDatabasesAsync(), AllPrimaries);
    }

    public async Task FlushAllDatabasesAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        _ = await Command(Request.FlushAllDatabasesAsync(), route);
    }

    public async Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        Utils.Requires<ArgumentException>(database == -1, "Different databases for this command are not supported by GLIDE");
        _ = await Command(Request.FlushDatabaseAsync(database), AllPrimaries);
    }

    public async Task FlushDatabaseAsync(Route route, int database = -1, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        Utils.Requires<ArgumentException>(database == -1, "Different databases for this command are not supported by GLIDE");
        _ = await Command(Request.FlushDatabaseAsync(database), route);
    }

    public async Task<Dictionary<string, DateTime>> LastSaveAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        ClusterValue<DateTime> result = await Command(Request.LastSaveAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, DateTime> { ["single_node"] = result.SingleValue };
    }

    public async Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.LastSaveAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task<Dictionary<string, DateTime>> TimeAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        ClusterValue<DateTime> result = await Command(Request.TimeAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, DateTime> { ["single_node"] = result.SingleValue };
    }

    public async Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.TimeAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task<Dictionary<string, string>> LolwutAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        ClusterValue<string> result = await Command(Request.LolwutAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, string> { ["single_node"] = result.SingleValue };
    }

    public async Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.LolwutAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task<TimeSpan> PingAsync(ValkeyValue message, Route route)
        => await Command(Request.Ping(message), route);

    public async Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ClientGetName(), Route.Random);
    }

    public async Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ClientGetNameCluster(route), route);
    }

    public async Task<long> ClientIdAsync(CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ClientId(), Route.Random);
    }

    public async Task<ClusterValue<long>> ClientIdAsync(Route route, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.ClientId().ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task<string> SelectAsync(long index, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.Select(index), Route.Random);
    }

    protected override async Task InitializeServerVersionAsync()
    {
        try
        {
            var infoResponse = await Command(Request.Info([InfoOptions.Section.SERVER]).ToClusterValue(true), Route.Random);
            var versionMatch = System.Text.RegularExpressions.Regex.Match(infoResponse.SingleValue, @"(?:valkey_version|redis_version):([\d\.]+)");
            if (versionMatch.Success)
            {
                _serverVersion = new Version(versionMatch.Groups[1].Value);
            }
        }
        catch
        {
            // If we can't get version, assume newer version (use SORT_RO)
            _serverVersion = new Version(8, 0, 0);
        }
    }
}
