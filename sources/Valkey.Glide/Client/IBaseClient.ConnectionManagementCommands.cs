// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by Valkey GLIDE clients but NOT by StackExchange.Redis databases. Methods implemented
/// by both should be added to <see cref="IConnectionManagementBaseCommands"/> instead.

/// <seealso href="https://valkey.io/commands/#connection">Valkey – Connection Management Commands</seealso>
public partial interface IBaseClient : IConnectionManagementBaseCommands
{
    /// <summary>
    /// Gets the name of the current connection.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname/">Valkey commands – CLIENT GETNAME</seealso>
    /// <returns>
    /// The name of the client connection as a <see cref="ValkeyValue"/>,
    /// or <see cref="ValkeyValue.Null"/> if no name is assigned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var name = await client.ClientGetNameAsync();
    /// Console.WriteLine($"Connection name: {name}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ClientGetNameAsync();

    /// <summary>
    /// Gets the current connection ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-id/">Valkey commands – CLIENT ID</seealso>
    /// <returns>The ID of the client connection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var id = await client.ClientIdAsync();
    /// Console.WriteLine($"Connection ID: {id}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ClientIdAsync();

    /// <summary>
    /// Echoes the given message back from the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/echo/">Valkey commands – ECHO</seealso>
    /// <param name="message">The message to echo.</param>
    /// <returns>The echoed message as a <see cref="ValkeyValue"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var echoed = await client.EchoAsync("Hello World");  // "Hello World"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> EchoAsync(ValkeyValue message);

    /// <summary>
    /// Pings the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/">Valkey commands – PING</seealso>
    /// <returns>The server's response as a <see cref="ValkeyValue"/> containing <c>"PONG"</c>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await client.PingAsync();  // "PONG"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync();

    /// <summary>
    /// Pings the server with a message.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/">Valkey commands – PING</seealso>
    /// <param name="message">The message to send with the ping.</param>
    /// <returns>The echoed message as a <see cref="ValkeyValue"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await client.PingAsync("Hello World");  // "Hello World"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync(ValkeyValue message);

    /// <summary>
    /// Suspends all clients for the specified timeout.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-pause/">Valkey commands - CLIENT PAUSE</seealso>
    /// <param name="timeout">The time to pause clients.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ClientPauseAsync(TimeSpan.FromSeconds(1));
    /// </code>
    /// </example>
    /// </remarks>
    Task ClientPauseAsync(TimeSpan timeout);

    /// <summary>
    /// Suspends write commands for all clients for the specified timeout.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-pause/">Valkey commands - CLIENT PAUSE</seealso>
    /// <param name="timeout">The time to pause clients.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ClientPauseWriteAsync(TimeSpan.FromSeconds(1));
    /// </code>
    /// </example>
    /// </remarks>
    Task ClientPauseWriteAsync(TimeSpan timeout);

    /// <summary>
    /// Resumes processing commands on all clients.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-unpause/">Valkey commands - CLIENT UNPAUSE</seealso>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ClientUnpauseAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ClientUnpauseAsync();

    /// <summary>
    /// Returns information about the current client connection's use of the
    /// server-assisted client-side caching feature.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-trackinginfo/">Valkey commands – CLIENT TRACKINGINFO</seealso>
    /// <returns>The tracking state for this connection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var info = await client.ClientTrackingInfoAsync();
    /// Console.WriteLine($"Flags: {string.Join(", ", info.Flags)}");  // "Flags: off"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClientTrackingInfo> ClientTrackingInfoAsync();
}
