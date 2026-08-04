// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

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
    /// var name = (await clusterClient.ClientGetNameAsync(Route.Random)).SingleValue;
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
    /// var id = (await clusterClient.ClientIdAsync(Route.Random)).SingleValue;
    /// Console.WriteLine($"Connection ID: {id}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<long>> ClientIdAsync(Route route);

    /// <summary>
    /// Kills all client connections except the calling client on the specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-kill/">Valkey commands – CLIENT KILL</seealso>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>The total number of clients killed across all targeted nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var killed = await clusterClient.ClientKillAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ClientKillAsync(Route route);

    /// <summary>
    /// Kills the client connection identified by the given address, routed to the specified node.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-kill/">Valkey commands – CLIENT KILL</seealso>
    /// <param name="host">The hostname or IP address of the client to kill.</param>
    /// <param name="port">The port number of the client to kill.</param>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ClientKillAsync("127.0.0.1", 6380, Route.Random);
    /// </code>
    /// </example>
    /// </remarks>
    Task ClientKillAsync(string host, ushort port, Route route);

    /// <summary>
    /// Kills client connections matching the given filter options, routed to the specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/client-kill/">Valkey commands – CLIENT KILL</seealso>
    /// <param name="options">The options specifying which clients to kill.</param>
    /// <param name="route">Specifies the routing configuration for the command.</param>
    /// <returns>The total number of clients killed across all targeted nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var killed = await clusterClient.ClientKillAsync(
    ///     new ClientFilterOptions().WithId(42), Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ClientKillAsync(ClientFilterOptions options, Route route);

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
    /// var info = (await clusterClient.ClientTrackingInfoAsync(Route.Random)).SingleValue;
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
    /// var echoed = (await clusterClient.EchoAsync("Hello World", Route.Random)).SingleValue;  // "Hello World"
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
    /// var response = await clusterClient.PingAsync(Route.AllPrimaries);
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
    /// var response = await clusterClient.PingAsync("Hello World", Route.AllPrimaries);
    /// Console.WriteLine(response);  // "Hello World"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> PingAsync(ValkeyValue message, Route route);
}
