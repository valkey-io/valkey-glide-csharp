// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for connection management for cluster clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/#connection">valkey.io</see>.
/// </summary>
public interface IConnectionManagementClusterCommands
{
    /// <summary>
    /// Gets the name of the current connection.
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The name of the client connection as a <see cref="ValkeyValue"/>.
    /// If no name is set, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ClientGetNameAsync();
    /// if (result != ValkeyValue.Null)
    /// {
    ///     Console.WriteLine($"Connection name: {result}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("No connection name set");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the name of the current connection.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
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
    Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the current connection ID.
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-id"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The ID of the client connection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long connectionId = await client.ClientIdAsync();
    /// Console.WriteLine($"Connection ID: {connectionId}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ClientIdAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the current connection ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-id"/>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <c>route</c>.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
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
    Task<ClusterValue<long>> ClientIdAsync(Route route, CommandFlags flags = CommandFlags.None);
}
