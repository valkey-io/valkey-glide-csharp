// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Transaction Commands" group for cluster clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/?group=transactions">valkey.io</see>.
/// </summary>
public interface ITransactionClusterCommands : ITransactionBaseCommands
{
    /// <summary>
    /// Flushes all the previously watched keys for a transaction. Executing a transaction will
    /// automatically flush all previously watched keys.
    /// The command will be routed to all primary nodes.
    /// </summary>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>"OK" if the keys were successfully unwatched.</returns>
    /// <example>
    /// <code>
    /// await client.WatchAsync(["sampleKey"]);
    /// string result = await client.UnwatchAsync();
    /// // result is "OK", "sampleKey" is no longer watched on all primary nodes
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/unwatch/"/>
    Task<string> UnwatchAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Flushes all the previously watched keys for a transaction. Executing a transaction will
    /// automatically flush all previously watched keys.
    /// </summary>
    /// <param name="route">Specifies the routing configuration for the command. The client will route the
    /// command to the nodes defined by <paramref name="route"/>.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>"OK" if the keys were successfully unwatched.</returns>
    /// <example>
    /// <code>
    /// await client.WatchAsync(["sampleKey"]);
    /// string result = await client.UnwatchAsync(Route.AllPrimaries);
    /// // result is "OK", "sampleKey" is no longer watched on all primary nodes
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/unwatch/"/>
    Task<string> UnwatchAsync(Route route, CommandFlags flags = CommandFlags.None);
}