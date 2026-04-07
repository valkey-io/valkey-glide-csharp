// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

/// <seealso href="https://valkey.io/commands/#connection">Valkey – Connection Management Commands</seealso>
public partial interface IBaseClient : IConnectionManagementBaseCommands
{
    /// <summary>
    /// Gets the name of the current connection.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname"/>
    /// <returns>
    /// The name of the client connection as a <see cref="ValkeyValue"/>.
    /// If no name is assigned, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ClientGetNameAsync();
    /// Console.WriteLine($"Connection name: {result}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ClientGetNameAsync();

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

    /// <summary>
    /// Echo the given message back from the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/echo/"/>
    /// <param name="message">The message to echo</param>
    /// <returns>The echoed message as a <see cref="ValkeyValue"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.EchoAsync("Hello World");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> EchoAsync(ValkeyValue message);

    /// <summary>
    /// Ping the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <returns>The server's response as a <see cref="ValkeyValue"/> containing <c>"PONG"</c>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await client.PingAsync();
    /// Console.WriteLine(response); // Output: "PONG"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync();

    /// <summary>
    /// Ping the server with a message.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/"/>
    /// <param name="message">The message to send with the ping</param>
    /// <returns>The echoed message as a <see cref="ValkeyValue"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await client.PingAsync("Hello World");
    /// Console.WriteLine(response); // Output: "Hello World"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync(ValkeyValue message);
}
