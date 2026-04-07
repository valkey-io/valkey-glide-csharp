// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Base connection management commands shared by standalone and cluster clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#connection">Valkey – Connection Management Commands</seealso>
public interface IConnectionManagementBaseCommands
{
    /// <summary>
    /// Changes the currently selected database.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/select"/>
    /// <param name="index">The index of the database to select.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SelectAsync(1);
    /// </code>
    /// </example>
    /// </remarks>
    Task SelectAsync(long index);
}
