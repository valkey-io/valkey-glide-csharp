// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Commands.Options.InfoOptions;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <summary>
/// Server management commands for Valkey GLIDE cluster client.
/// </summary>
/// <seealso href="https://valkey.io/commands/#server">Valkey – Server Management Commands</seealso>
public partial interface IGlideClusterClient
{
    /// <summary>
    /// Get information and statistics about the server using <see cref="Section.DEFAULT" /> option.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await clusterClient.InfoAsync();
    /// var firstNodeInfo = response.Values.First();
    /// </code>
    /// </example>
    /// </remarks>
    /// <returns>A <see langword="string" /> containing the information for the sections requested per cluster node.</returns>
    Task<Dictionary<string, string>> InfoAsync();

    /// <summary>
    /// Get information and statistics about the server.<br />
    /// Starting from server version 7, command supports multiple <see cref="Section" /> arguments.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await clusterClient.InfoAsync([Section.STATS]);
    /// var firstNodeInfo = response.Values.First();
    /// </code>
    /// </example>
    /// </remarks>
    /// <inheritdoc cref="InfoAsync(IEnumerable{Section}, Route)" path="/param" />
    /// <returns>A <see langword="string" /> containing the information for the sections requested per cluster node.</returns>
    Task<Dictionary<string, string>> InfoAsync(IEnumerable<Section> sections);

    /// <summary>
    /// Get information and statistics about the server using <see cref="Section.DEFAULT" /> option.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = (await clusterClient.InfoAsync(Route.AllNodes)).MultiValue;
    /// var filtered = response.Select(pair =>
    ///         (Node: pair.Key, Value: pair.Value.Split('\n').First(l => l.Contains("total_net_input_bytes")))
    ///     ).ToDictionary(p => p.Node, p => p.Value);
    /// </code>
    /// </example>
    /// </remarks>
    /// <inheritdoc cref="InfoAsync(IEnumerable{Section}, Route)" path="/param" />
    /// <returns>
    /// <inheritdoc cref="InfoAsync(IEnumerable{Section}, Route)" />
    /// </returns>
    Task<ClusterValue<string>> InfoAsync(Route route);

    /// <summary>
    /// Get information and statistics about the server.<br />
    /// Starting from server version 7, command supports multiple <see cref="Section" /> arguments.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = (await clusterClient.InfoAsync([Section.STATS], Route.AllNodes));
    /// var filtered = response.MultiValue.Select(pair =>
    ///         (Node: pair.Key, Value: pair.Value.Split('\n').First(l => l.Contains("total_net_input_bytes")))
    ///     ).ToDictionary(p => p.Node, p => p.Value);
    /// </code>
    /// </example>
    /// </remarks>
    /// <param name="sections">A list of <see cref="Section" /> values specifying which sections of information to
    /// retrieve. When no parameter is provided, the <see cref="Section.DEFAULT" /> option is assumed.</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the information for the sections requested.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, string&gt;</c> with each address as the key and its corresponding
    /// value is the information for the node. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    Task<ClusterValue<string>> InfoAsync(IEnumerable<Section> sections, Route route);

    /// <summary>
    /// Gets the values of configuration parameters.<br />
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/"/>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <returns>All matching configuration parameters per cluster node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var config = await clusterClient.ConfigGetAsync("max*");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern = default);

    /// <summary>
    /// Gets the values of configuration parameters.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/"/>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing all matching configuration parameters.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, KeyValuePair&lt;string, string&gt;[]&gt;</c> with each address as the key and its corresponding
    /// configuration parameters. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var config = await clusterClient.ConfigGetAsync("max*", Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route);

    /// <summary>
    /// Resets the statistics reported by the server using the INFO and LATENCY HISTOGRAM.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-resetstat/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ConfigResetStatisticsAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigResetStatisticsAsync();

    /// <summary>
    /// Resets the statistics reported by the server using the INFO and LATENCY HISTOGRAM.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-resetstat/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ConfigResetStatisticsAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigResetStatisticsAsync(Route route);

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file the server was started with,
    /// applying the minimal changes needed to make it reflecting the configuration currently
    /// used by the server, that may be different compared to the original one because of the use of the CONFIG SET command.<br />
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ConfigRewriteAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigRewriteAsync();

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file the server was started with,
    /// applying the minimal changes needed to make it reflecting the configuration currently
    /// used by the server, that may be different compared to the original one because of the use of the CONFIG SET command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ConfigRewriteAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigRewriteAsync(Route route);

    /// <summary>
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// You can change both trivial parameters or switch from one to another persistence option using this command.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/"/>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ConfigSetAsync("maxmemory", "100mb");
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value);

    /// <summary>
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// You can change both trivial parameters or switch from one to another persistence option using this command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/"/>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ConfigSetAsync("maxmemory", "100mb", Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route);

    /// <summary>
    /// Returns the number of keys in the database across all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dbsize/"/>
    /// <returns>The number of keys in the database across all primary nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long totalKeys = await clusterClient.DatabaseSizeAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> DatabaseSizeAsync();

    /// <summary>
    /// Returns the number of keys in the database across all routed nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dbsize/"/>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>The number of keys in the database across all routed nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long totalKeys = await clusterClient.DatabaseSizeAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> DatabaseSizeAsync(Route route);

    /// <summary>
    /// Deletes all the keys of all the existing databases across all routed nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/"/>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.FlushAllDatabasesAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushAllDatabasesAsync(Route route);

    /// <summary>
    /// Deletes all the keys in the database across all routed nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/"/>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.FlushDatabaseAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync(Route route);

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// A client may check if a BGSAVE command succeeded reading the LASTSAVE value, then issuing a BGSAVE command
    /// and checking at regular intervals every N seconds if LASTSAVE changed.<br />
    /// The command is routed to a random node by default, which is safe for read-only commands.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave/"/>
    /// <returns>UNIX TIME of the last DB save executed with success per cluster node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var lastSaves = await clusterClient.LastSaveAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, DateTimeOffset>> LastSaveAsync();

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// A client may check if a BGSAVE command succeeded reading the LASTSAVE value, then issuing a BGSAVE command
    /// and checking at regular intervals every N seconds if LASTSAVE changed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing UNIX TIME of the last DB save executed with success.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, DateTimeOffset&gt;</c> with each address as the key and its corresponding
    /// last save time. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var lastSave = await clusterClient.LastSaveAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<DateTimeOffset>> LastSaveAsync(Route route);

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// Use the <see cref="DateTimeOffset.ToLocalTime"/> method to get local time.<br />
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time/"/>
    /// <returns>The server's current time per cluster node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var times = await clusterClient.TimeAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, DateTimeOffset>> TimeAsync();

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// Use the <see cref="DateTimeOffset.ToLocalTime"/> method to get local time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the server's current time.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, DateTimeOffset&gt;</c> with each address as the key and its corresponding
    /// server time. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var time = await clusterClient.TimeAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<DateTimeOffset>> TimeAsync(Route route);

    /// <summary>
    /// Displays a piece of generative computer art of the specific Valkey version and it's optional arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the Valkey version and generative art.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, string&gt;</c> with each address as the key and its corresponding
    /// lolwut output. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var art = await clusterClient.LolwutAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> LolwutAsync(Route route);

    /// <summary>
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// This overload allows setting multiple configuration parameters in a single call.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/"/>
    /// <param name="parameters">A dictionary of configuration parameter names and their new values.</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ConfigSetAsync(new Dictionary&lt;ValkeyValue, ValkeyValue&gt;
    /// {
    ///     { "maxmemory", "100mb" },
    ///     { "maxmemory-policy", "allkeys-lru" }
    /// }, Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters, Route route);

    /// <summary>
    /// Gets the values of configuration parameters matching multiple patterns.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/"/>
    /// <param name="patterns">The patterns of config values to get.</param>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>All matching configuration parameters.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var config = await clusterClient.ConfigGetAsync(["max*", "bind*"], Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(IEnumerable<ValkeyValue> patterns, Route route);

    /// <summary>
    /// Deletes all the keys of all the existing databases across all routed nodes, with the specified flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/"/>
    /// <param name="mode">The flush mode to use. <see cref="FlushMode.Sync"/> waits for completion,
    /// <see cref="FlushMode.Async"/> returns immediately while flush continues in background.</param>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.FlushAllDatabasesAsync(FlushMode.Async, Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushAllDatabasesAsync(FlushMode mode, Route route);

    /// <summary>
    /// Deletes all the keys in the database across all routed nodes, with the specified flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/"/>
    /// <param name="mode">The flush mode to use. <see cref="FlushMode.Sync"/> waits for completion,
    /// <see cref="FlushMode.Async"/> returns immediately while flush continues in background.</param>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.FlushDatabaseAsync(FlushMode.Async, Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync(FlushMode mode, Route route);

    /// <summary>
    /// Displays a piece of generative computer art and the Valkey version.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/"/>
    /// <param name="options">The LOLWUT options specifying version and/or parameters.</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the Valkey version and generative art.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var art = await clusterClient.LolwutAsync(new LolwutOptions { Version = 6, Parameters = [40, 20] }, Route.AllNodes);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> LolwutAsync(LolwutOptions options, Route route);

    /// <summary>
    /// Blocks the current client until all the previous write commands are successfully transferred and acknowledged
    /// by at least the specified number of local and replica AOF-synced nodes.
    /// If the timeout is reached, the command returns even if the specified number of acknowledgments were not yet reached.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/waitaof"/>
    /// <note>Since Valkey 7.2.0.</note>
    /// <param name="localAof">Whether to wait for the local node to acknowledge AOF sync.</param>
    /// <param name="numreplicas">The number of replica nodes to wait for AOF sync.</param>
    /// <param name="timeout">The timeout to wait.</param>
    /// <param name="route">Specifies the routing configuration for the command. Typically, you should route
    /// to the primary that handled the write operation you want to wait for.</param>
    /// <returns>An array of two longs: the number of local and replica nodes that acknowledged the write commands.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await clusterClient.WaitAofAsync(true, 1, TimeSpan.FromSeconds(1), Route.AllPrimaries);
    /// // result[0] = number of local nodes, result[1] = number of replica nodes
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> WaitAofAsync(bool localAof, long numreplicas, TimeSpan timeout, Route route);
}
