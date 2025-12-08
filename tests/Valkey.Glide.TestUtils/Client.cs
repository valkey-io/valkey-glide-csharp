// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE clients.
/// </summary>
public static class Client
{
    private static readonly GlideString[] ClientListCommandArgs = ["CLIENT", "LIST"];

    /// <summary>
    /// Asserts that a client is connected by sending a PING command.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertConnected(BaseClient client)
    {
        if (client is GlideClient standaloneClient)
        {
            Assert.True(await standaloneClient.PingAsync() > TimeSpan.Zero);
        }
        else if (client is GlideClusterClient clusterClient)
        {
            Assert.True(await clusterClient.PingAsync() > TimeSpan.Zero);
        }
        else
        {
            Assert.Fail("Unknown client type.");
        }
    }

    /// <summary>
    /// Returns the total number of client connections to a server.
    /// </summary>
    /// <param name="client">A client that is connected to the server.</param>
    /// <returns>A task that resolves to the total number of client connections.</returns>
    public static async Task<int> GetConnectionCount(BaseClient client)
    {
        if (client is GlideClient standaloneClient)
        {
            var result = await standaloneClient.CustomCommand(ClientListCommandArgs);
            return result!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        }
        else if (client is GlideClusterClient clusterClient)
        {
            var result = await clusterClient.CustomCommand(ClientListCommandArgs, new Route.AllPrimariesRoute());
            return result!.MultiValue.Values.Sum(nodeResult =>
                nodeResult!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
        }
        else
        {
            throw new ArgumentException("Unknown client type.");
        }
    }
}
