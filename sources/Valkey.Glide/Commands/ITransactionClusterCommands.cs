// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Transaction commands for cluster clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#transactions">Valkey – Transaction Commands</seealso>
public interface ITransactionClusterCommands : ITransactionBaseCommands
{
    /// <summary>
    /// Flushes all the previously watched keys for a transaction. Executing a transaction will
    /// automatically flush all previously watched keys.
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    /// <example>
    /// <code>
    /// await client.WatchAsync(["sampleKey"]);
    /// await client.UnwatchAsync();
    /// // "sampleKey" is no longer watched on all primary nodes
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/unwatch/"/>
    Task UnwatchAsync();

    /// <summary>
    /// Flushes all the previously watched keys for a transaction. Executing a transaction will
    /// automatically flush all previously watched keys.
    /// </summary>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <paramref name="route"/>.</param>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    /// <example>
    /// <code>
    /// await client.WatchAsync(["sampleKey"]);
    /// await client.UnwatchAsync(Route.AllPrimaries);
    /// // "sampleKey" is no longer watched on all primary nodes
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/unwatch/"/>
    Task UnwatchAsync(Route route);
}
