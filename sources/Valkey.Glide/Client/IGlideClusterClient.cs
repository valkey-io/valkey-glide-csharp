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
public partial interface IGlideClusterClient :
    IBaseClient,
    IGenericClusterCommands,
    IPubSubClusterCommands,
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
    /// var response = await clusterClient.ClientGetNameAsync(Route.AllPrimaries);
    /// if (response.HasSingleData)
    /// {
    ///     Console.WriteLine($"Connection name: {response.SingleValue}");
    /// }
    /// else
    /// {
    ///     foreach (var kvp in response.MultiValue)
    ///     {
    ///         string name = kvp.Value.IsNull ? "No name set" : kvp.Value.ToString();
    ///         Console.WriteLine($"Node {kvp.Key}: {name}");
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
    /// var response = await clusterClient.ClientIdAsync(Route.AllPrimaries);
    /// if (response.HasSingleData)
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
    /// var response = await clusterClient.EchoAsync("Hello World", Route.AllPrimaries);
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
    /// var response = await clusterClient.PingAsync(Route.AllPrimaries);  // "PONG"
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
    /// var response = await clusterClient.PingAsync("Hello World", Route.AllPrimaries);  // "Hello World"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync(ValkeyValue message, Route route);

    /// <summary>
    /// Incrementally iterates over the matching keys in the cluster.
    /// </summary>
    /// <param name="options">Optional scan options including pattern, count hint, and type filter.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching keys.</returns>
    /// <example>
    /// <code>
    /// // Scan all keys
    /// await foreach (var key in client.ScanAsync())
    /// {
    ///     Console.WriteLine(key);
    /// }
    ///
    /// // Scan with pattern and type filter
    /// var options = new ScanOptions { MatchPattern = "user:*", Type = ValkeyType.String };
    /// await foreach (var key in client.ScanAsync(options))
    /// {
    ///     Console.WriteLine(key);
    /// }
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/scan/">SCAN command</seealso>
    /// <seealso href="https://glide.valkey.io/how-to/scan-cluster/">Valkey GLIDE – Scan a Cluster</seealso>
    IAsyncEnumerable<ValkeyKey> ScanAsync(ScanOptions? options = null);
}
