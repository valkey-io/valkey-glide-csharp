// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Transaction commands for standalone clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#transactions">Valkey – Transaction Commands</seealso>
public interface ITransactionCommands : ITransactionBaseCommands
{
    /// <summary>
    /// Flushes all the previously watched keys for a transaction.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/unwatch/">Valkey commands – UNWATCH</seealso>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.WatchAsync(["sampleKey"]);
    /// await client.UnwatchAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task UnwatchAsync();
}
