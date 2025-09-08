// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for connection management for standalone clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/#connection">valkey.io</see>.
/// </summary>
public interface IConnectionManagementCommands
{
    /// <summary>
    /// Gets the name of the current connection.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The name of the client connection as a <see cref="ValkeyValue"/>.
    /// If no name is assigned, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ClientGetNameAsync();
    /// if (result != ValkeyValue.Null)
    /// {
    ///     Console.WriteLine($"Connection name: {result}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("No connection name set");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the current connection ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-id"/>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The ID of the client connection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long connectionId = await client.ClientIdAsync();
    /// Console.WriteLine($"Connection ID: {connectionId}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ClientIdAsync(CommandFlags flags = CommandFlags.None);
}
