// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Connection management commands for cluster clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#connection">Valkey – Connection Management Commands</seealso>
// NOTE: Methods should only be added to this interface if they are implemented by both Valkey GLIDE clients
// and StackExchange.Redis databases.
public interface IConnectionManagementClusterCommands
{
    /// <summary>
    /// Gets the current connection ID.
    /// The command will be routed to a random node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-id"/>
    /// <returns>The ID of the client connection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long connectionId = await client.ClientIdAsync();
    /// Console.WriteLine($"Connection ID: {connectionId}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ClientIdAsync();

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
}
