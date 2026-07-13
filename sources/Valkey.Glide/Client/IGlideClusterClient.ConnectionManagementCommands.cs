// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Connection management commands for cluster clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#connection">Valkey – Connection Management Commands</seealso>
public partial interface IGlideClusterClient
{
    /// <summary>
    /// Gets the name of the current connection.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-getname/">Valkey commands – CLIENT GETNAME</seealso>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing the connection names.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var name = (await client.ClientGetNameAsync(Route.Random)).SingleValue;
    /// Console.WriteLine($"Connection name: {name}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route);

    /// <summary>
    /// Gets the current connection ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-id/">Valkey commands – CLIENT ID</seealso>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing the connection IDs.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var id = (await client.ClientIdAsync(Route.Random)).SingleValue;
    /// Console.WriteLine($"Connection ID: {id}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<long>> ClientIdAsync(Route route);

    /// <summary>
    /// Returns information about the current client connection's use of the
    /// server-assisted client-side caching feature.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-trackinginfo/">Valkey commands – CLIENT TRACKINGINFO</seealso>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>A <see cref="ClusterValue{T}" /> containing tracking states for this connection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var info = (await client.ClientTrackingInfoAsync(Route.Random)).SingleValue;
    /// Console.WriteLine($"Flags: {string.Join(", ", info.Flags)}");  // "Flags: off"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ClientTrackingInfo>> ClientTrackingInfoAsync(Route route);

    /// <summary>
    /// Echoes the given message back from the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/echo/">Valkey commands – ECHO</seealso>
    /// <param name="message">The message to echo.</param>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing the echoed messages.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var echoed = (await client.EchoAsync("Hello World", Route.Random)).SingleValue;
    /// Console.WriteLine(response);  // "Hello World"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route);

    /// <summary>
    /// Pings the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/">Valkey commands – PING</seealso>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>The server response (<c>"PONG"</c>).</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await client.PingAsync(Route.AllPrimaries);
    /// Console.WriteLine(response);  // "PONG"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync(Route route);

    /// <summary>
    /// Pings the server with a message.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ping/">Valkey commands – PING</seealso>
    /// <param name="message">The message to send with the ping.</param>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>The echoed message.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var response = await client.PingAsync("Hello World", Route.AllPrimaries);
    /// Console.WriteLine(response);  // "Hello World"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync(ValkeyValue message, Route route);
}
