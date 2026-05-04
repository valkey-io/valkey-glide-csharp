// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Transaction commands for cluster clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#transactions">Valkey – Transaction Commands</seealso>
public interface ITransactionClusterCommands : ITransactionBaseCommands
{
    /// <summary>
    /// Flushes all the previously watched keys for a transaction.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unwatch/">Valkey commands – UNWATCH</seealso>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.WatchAsync(["sampleKey"]);
    /// await clusterClient.UnwatchAsync();
    /// // "sampleKey" is no longer watched on all primary nodes
    /// </code>
    /// </example>
    /// </remarks>
    Task UnwatchAsync();

    /// <summary>
    /// Flushes all the previously watched keys for a transaction.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unwatch/">Valkey commands – UNWATCH</seealso>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <paramref name="route"/>.</param>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.WatchAsync(["sampleKey"]);
    /// await clusterClient.UnwatchAsync(Route.AllPrimaries);
    /// // "sampleKey" is no longer watched on all primary nodes
    /// </code>
    /// </example>
    /// </remarks>
    Task UnwatchAsync(Route route);
}
