// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Connection management commands for standalone clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#connection">Valkey – Connection Management Commands</seealso>
// NOTE: Methods should only be added to this interface if they are implemented by both Valkey GLIDE clients
// and StackExchange.Redis databases.
public interface IConnectionManagementCommands
{
    /// <summary>
    /// Gets the current connection ID.
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
}
