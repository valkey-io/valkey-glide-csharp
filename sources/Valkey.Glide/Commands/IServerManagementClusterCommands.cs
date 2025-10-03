// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Options.InfoOptions;

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Server Management" group for a cluster client.
/// <br />
/// See <see href="https://valkey.io/commands#server">Server Management Commands</see>.
/// </summary>
public interface IServerManagementClusterCommands
{
    /// <summary>
    /// Get information and statistics about the server using <see cref="Section.DEFAULT" /> option.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// string response = await client.InfoAsync();
    /// response.Split().First(l => l.Contains("total_net_input_bytes"))
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
    /// string response = await client.InfoAsync([ Section.STATS ]);
    /// response.Split().First(l => l.Contains("total_net_input_bytes"))
    /// </code>
    /// </example>
    /// </remarks>
    /// <inheritdoc cref="InfoAsync(Section[], Route)" path="/param" />
    /// <returns>A <see langword="string" /> containing the information for the sections requested per cluster node.</returns>
    Task<Dictionary<string, string>> InfoAsync(Section[] sections);

    /// <summary>
    /// Get information and statistics about the server using <see cref="Section.DEFAULT" /> option.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/info/"/>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, string&gt; response = (await client.InfoAsync(Route.AllNodes)).MultiValue;
    /// response.Select(pair =>
    ///         (Node: pair.Key, Value: pair.Value.Split().First(l => l.Contains("total_net_input_bytes")))
    ///     ).ToDictionary(p => p.Key, p => p.Value)
    /// </code>
    /// </example>
    /// </remarks>
    /// <inheritdoc cref="InfoAsync(Section[], Route)" path="/param" />
    /// <returns>
    /// <inheritdoc cref="InfoAsync(Section[], Route)" />
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
    /// Dictionary&lt;string, string&gt; response = (await client.InfoAsync([ Section.STATS ], Route.AllNodes)).MultiValue;
    /// response.Select(pair =>
    ///         (Node: pair.Key, Value: pair.Value.Split().First(l => l.Contains("total_net_input_bytes")))
    ///     ).ToDictionary(p => p.Key, p => p.Value)
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
    Task<ClusterValue<string>> InfoAsync(Section[] sections, Route route);

    /// <summary>
    /// Echo the given message back from the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/echo/"/>
    /// <param name="message">The message to echo</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the echoed message as a <see cref="ValkeyValue"/>.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, ValkeyValue&gt;</c> with each address as the key and its corresponding
    /// echoed message. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;ValkeyValue&gt; response = await client.EchoAsync("Hello World", Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Ping the server and measure the round-trip time.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>The round-trip time as a <see cref="TimeSpan"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan latency = await client.PingAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Ping the server with a message and measure the round-trip time.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="message">The message to send with the ping</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>The round-trip time as a <see cref="TimeSpan"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan latency = await client.PingAsync("test message");
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan> PingAsync(ValkeyValue message, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Ping the server and measure the round-trip time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>The round-trip time as a <see cref="TimeSpan"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan response = await client.PingAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan> PingAsync(Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Ping the server with a message and measure the round-trip time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="message">The message to send with the ping</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags. Currently flags are ignored.</param>
    /// <returns>The round-trip time as a <see cref="TimeSpan"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// TimeSpan response = await client.PingAsync("Hello World", Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeSpan> PingAsync(ValkeyValue message, Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the values of configuration parameters.<br />
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/"/>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>All matching configuration parameters per cluster node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;KeyValuePair&lt;string, string&gt;[]&gt; config = await client.ConfigGetAsync("max*");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the values of configuration parameters.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-get/"/>
    /// <param name="pattern">The pattern of config values to get.</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing all matching configuration parameters.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, KeyValuePair&lt;string, string&gt;[]&gt;</c> with each address as the key and its corresponding
    /// configuration parameters. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;KeyValuePair&lt;string, string&gt;[]&gt; config = await client.ConfigGetAsync("max*", Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Resets the statistics reported by the server using the INFO and LATENCY HISTOGRAM.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-resetstat/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigResetStatisticsAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigResetStatisticsAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Resets the statistics reported by the server using the INFO and LATENCY HISTOGRAM.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-resetstat/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigResetStatisticsAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigResetStatisticsAsync(Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file the server was started with,
    /// applying the minimal changes needed to make it reflecting the configuration currently
    /// used by the server, that may be different compared to the original one because of the use of the CONFIG SET command.<br />
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigRewriteAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigRewriteAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The CONFIG REWRITE command rewrites the valkey.conf file the server was started with,
    /// applying the minimal changes needed to make it reflecting the configuration currently
    /// used by the server, that may be different compared to the original one because of the use of the CONFIG SET command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-rewrite/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigRewriteAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigRewriteAsync(Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// You can change both trivial parameters or switch from one to another persistence option using this command.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/"/>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigSetAsync("maxmemory", "100mb");
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The CONFIG SET command is used in order to reconfigure the server at runtime without the need to restart Valkey.
    /// You can change both trivial parameters or switch from one to another persistence option using this command.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/config-set/"/>
    /// <param name="setting">The setting name.</param>
    /// <param name="value">The new setting value.</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ConfigSetAsync("maxmemory", "100mb", Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of keys in the currently-selected database.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dbsize/"/>
    /// <param name="database">The database to check. GLIDE does not support this.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>The number of keys in the database per cluster node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, long&gt; sizes = await client.DatabaseSizeAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, long>> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of keys in the currently-selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/dbsize/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="database">The database to check. GLIDE does not support this.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the number of keys in the database.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, long&gt;</c> with each address as the key and its corresponding
    /// database size. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;long&gt; size = await client.DatabaseSizeAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<long>> DatabaseSizeAsync(Route route, int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Deletes all the keys of all the existing databases.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushAllDatabasesAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushAllDatabasesAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Deletes all the keys of all the existing databases.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushall/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushAllDatabasesAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushAllDatabasesAsync(Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Deletes all the keys of the currently selected database.<br />
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/"/>
    /// <param name="database">The database to check. GLIDE does not support this.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushDatabaseAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Deletes all the keys of the currently selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/flushdb/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="database">The database to check. GLIDE does not support this.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FlushDatabaseAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FlushDatabaseAsync(Route route, int database = -1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// A client may check if a BGSAVE command succeeded reading the LASTSAVE value, then issuing a BGSAVE command
    /// and checking at regular intervals every N seconds if LASTSAVE changed.<br />
    /// The command is routed to a random node by default, which is safe for read-only commands.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>UNIX TIME of the last DB save executed with success per cluster node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, DateTime&gt; lastSaves = await client.LastSaveAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, DateTime>> LastSaveAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the time of the last DB save executed with success.
    /// A client may check if a BGSAVE command succeeded reading the LASTSAVE value, then issuing a BGSAVE command
    /// and checking at regular intervals every N seconds if LASTSAVE changed.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lastsave/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing UNIX TIME of the last DB save executed with success.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, DateTime&gt;</c> with each address as the key and its corresponding
    /// last save time. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;DateTime&gt; lastSave = await client.LastSaveAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// Use the <see cref="DateTime.ToLocalTime"/> method to get local time.<br />
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>The server's current time per cluster node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, DateTime&gt; times = await client.TimeAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, DateTime>> TimeAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// The TIME command returns the current server time in UTC format.
    /// Use the <see cref="DateTime.ToLocalTime"/> method to get local time.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/time/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the server's current time.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, DateTime&gt;</c> with each address as the key and its corresponding
    /// server time. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;DateTime&gt; time = await client.TimeAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Displays a piece of generative computer art of the specific Valkey version and it's optional arguments.<br />
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/"/>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>A string containing the Valkey version and generative art per cluster node.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// Dictionary&lt;string, string&gt; art = await client.LolwutAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<Dictionary<string, string>> LolwutAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Displays a piece of generative computer art of the specific Valkey version and it's optional arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lolwut/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">The command flags to use. Currently flags are ignored</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the Valkey version and generative art.<br />
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, string&gt;</c> with each address as the key and its corresponding
    /// lolwut output. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; art = await client.LolwutAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags = CommandFlags.None);
    Task<TimeSpan> PingAsync(ValkeyValue message, Route route);

    /// <summary>
    /// Changes the currently selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/select"/>
    /// <param name="index">The index of the database to select.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>A simple "OK" response.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string result = await client.SelectAsync(1);
    /// Console.WriteLine(result); // Output: "OK"
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> SelectAsync(long index, CommandFlags flags = CommandFlags.None);
}
