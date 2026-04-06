// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Interface for Valkey GLIDE cluster client.
/// </summary>
// NOTE: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.
public interface IGlideClusterClient :
    IBaseClient,
    IConnectionManagementClusterCommands,
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
}
