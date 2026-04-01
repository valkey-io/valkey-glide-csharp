// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Transaction commands for standalone clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/?group=transactions">Valkey – Transaction Commands</seealso>
public interface ITransactionCommands : ITransactionBaseCommands
{
    /// <summary>
    /// Flushes all the previously watched keys for a transaction. Executing a transaction will
    /// automatically flush all previously watched keys.
    /// </summary>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    /// <example>
    /// <code>
    /// await client.WatchAsync(["sampleKey"]);
    /// await client.UnwatchAsync();
    /// </code>
    /// </example>
    /// <seealso href="https://valkey.io/commands/unwatch/"/>
    Task UnwatchAsync();
}
