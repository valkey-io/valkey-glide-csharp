// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Transaction Commands" group for standalone clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/?group=transactions">valkey.io</see>.
/// </summary>
public interface ITransactionCommands : ITransactionBaseCommands
{
    /// <summary>
    /// Flushes all the previously watched keys for a transaction. Executing a transaction will
    /// automatically flush all previously watched keys.
    /// </summary>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <exception cref="RequestException">Thrown if the command fails to execute on the server.</exception>
    /// <example>
    /// <code>
    /// await client.WatchAsync(["sampleKey"]);
    /// bool result = await client.UnwatchAsync();
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/unwatch/"/>
    Task UnwatchAsync(CommandFlags flags = CommandFlags.None);
}
