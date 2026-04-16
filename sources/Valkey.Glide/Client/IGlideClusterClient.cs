// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <summary>
/// Interface for Valkey GLIDE cluster client.
/// </summary>
public interface IGlideClusterClient :
    IBaseClient,
    IGenericClusterCommands,
    IPubSubClusterCommands,
    IScriptingAndFunctionClusterCommands,
    IServerManagementClusterCommands,
    ITransactionClusterCommands
{
    /// <summary>
    /// Gets the name of the current connection.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the name of the client connection as a <see cref="ValkeyValue"/>.
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, ValkeyValue&gt;</c> with each address as the key and its corresponding
    /// connection name (or <see cref="ValkeyValue.Null"/> if no name is set). For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;ValkeyValue&gt; response = await client.ClientGetNameAsync(Route.AllPrimaries);
    /// if (response.HasSingleValue)
    /// {
    ///     Console.WriteLine($"Connection name: {response.SingleValue}");
    /// }
    /// else
    /// {
    ///     foreach (var kvp in response.MultiValue)
    ///     {
    ///         Console.WriteLine($"Node {kvp.Key}: {kvp.Value ?? "No name set"}");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route);

    /// <summary>
    /// Gets the current connection ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-id"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>
    /// A <see cref="ClusterValue{T}" /> containing the ID of the client connection.
    /// When specifying a <paramref name="route" /> other than a single node, it returns a multi-value <see cref="ClusterValue{T}" />
    /// with a <c>Dictionary&lt;string, long&gt;</c> with each address as the key and its corresponding
    /// connection ID. For a single node route it returns a <see cref="ClusterValue{T}" /> with a single value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;long&gt; response = await client.ClientIdAsync(Route.AllPrimaries);
    /// if (response.HasSingleValue)
    /// {
    ///     Console.WriteLine($"Connection ID: {response.SingleValue}");
    /// }
    /// else
    /// {
    ///     foreach (var kvp in response.MultiValue)
    ///     {
    ///         Console.WriteLine($"Node {kvp.Key}: Connection ID {kvp.Value}");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<long>> ClientIdAsync(Route route);

    /// <summary>
    /// Echo the given message back from the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/echo/"/>
    /// <param name="message">The message to echo</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
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
    Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route);

    /// <summary>
    /// Ping the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>The server's response as a <see cref="ValkeyValue"/> containing <c>"PONG"</c>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await client.PingAsync(Route.AllPrimaries);
    /// Console.WriteLine(response); // Output: "PONG"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync(Route route);

    /// <summary>
    /// Ping the server with a message.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="message">The message to send with the ping</param>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <returns>The echoed message as a <see cref="ValkeyValue"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await client.PingAsync("Hello World", Route.AllPrimaries);
    /// Console.WriteLine(response); // Output: "Hello World"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync(ValkeyValue message, Route route);

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
    /// await client.ConfigSetAsync(new Dictionary&lt;ValkeyValue, ValkeyValue&gt;
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
    /// ClusterValue&lt;KeyValuePair&lt;string, string&gt;[]&gt; config = await client.ConfigGetAsync(["max*", "bind*"], Route.AllPrimaries);
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
    /// await client.FlushAllDatabasesAsync(FlushMode.Async, Route.AllPrimaries);
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
    /// await client.FlushDatabaseAsync(FlushMode.Async, Route.AllPrimaries);
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
    /// ClusterValue&lt;string&gt; art = await client.LolwutAsync(new LolwutOptions { Version = 6, Parameters = [40, 20] }, Route.AllNodes);
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
    /// <param name="localAof">Whether to wait for the local node to acknowledge AOF sync.</param>
    /// <param name="numreplicas">The number of replica nodes to wait for AOF sync.</param>
    /// <param name="timeout">The timeout to wait.</param>
    /// <param name="route">Specifies the routing configuration for the command. Typically, you should route
    /// to the primary that handled the write operation you want to wait for.</param>
    /// <returns>An array of two longs: the number of local and replica nodes that acknowledged the write commands.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long[] result = await client.WaitAofAsync(true, 1, TimeSpan.FromSeconds(1), route);
    /// // result[0] = number of local nodes, result[1] = number of replica nodes
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> WaitAofAsync(bool localAof, long numreplicas, TimeSpan timeout, Route route);
}
