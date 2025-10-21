// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Utility class for retrieving connection information from Valkey servers.
/// </summary>
internal static class ConnectionInfo
{
    private static readonly GlideString[] ClientListCommandArgs = ["CLIENT", "LIST"];

    /// <summary>
    /// Returns the total number of client connections to a standalone Valkey server.
    /// </summary>
    /// <param name="client">The standalone client instance.</param>
    /// <returns>A task that resolves to the total number of client connections.</returns>
    public static async Task<int> GetConnectionCount(GlideClient client)
    {
        var result = await client.CustomCommand(ClientListCommandArgs);
        return result!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    /// <summary>
    /// Returns the total number of client connections to a cluster Valkey server.
    /// </summary>
    /// <param name="client">The cluster client instance.</param>
    /// <returns>A task that resolves to the total number of client connections.</returns>
    public static async Task<int> GetConnectionCount(GlideClusterClient client)
    {
        var result = await client.CustomCommand(ClientListCommandArgs, new Route.AllPrimariesRoute());
        return result!.MultiValue.Values.Sum(nodeResult =>
            nodeResult!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
    }
}
