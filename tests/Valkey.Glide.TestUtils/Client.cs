// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE clients.
/// </summary>
public static class Client
{
    /// <summary>
    /// Asserts that the given client is connected.
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
    /// Asserts that the given client is reconnected.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertReconnected(BaseClient client)
    {
        TimeSpan reconnedtTimeout = TimeSpan.FromSeconds(30);
        TimeSpan retryInterval = TimeSpan.FromSeconds(0.5);

        // Retry connection until successful for timeout occurs.
        using CancellationTokenSource cts = new(reconnedtTimeout);

        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                await AssertConnected(client);
                return;
            }

            catch (Exception ex) when (ex is Errors.TimeoutException or Errors.ConnectionException)
            {
                await Task.Delay(retryInterval);
            }
        }

        Assert.Fail("Reconnection failed.");
    }

    /// <summary>
    /// Returns the total number of client connections to a server.
    /// </summary>
    /// <param name="client">A client that is connected to the server.</param>
    /// <returns>A task that resolves to the total number of client connections.</returns>
    public static async Task<int> GetConnectionCount(BaseClient client)
    {
        GlideString[] clientListCommandArgs = ["CLIENT", "LIST"];
        if (client is GlideClient standaloneClient)
        {
            object? result = await standaloneClient.CustomCommand(clientListCommandArgs);
            return result!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        }
        else if (client is GlideClusterClient clusterClient)
        {
            ClusterValue<object?> result = await clusterClient.CustomCommand(clientListCommandArgs, new Route.AllPrimariesRoute());
            return result!.MultiValue.Values.Sum(static nodeResult =>
                nodeResult!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length);
        }
        else
        {
            throw new ArgumentException("Unknown client type.");
        }
    }

    /// <summary>
    /// Returns the Valkey server version.
    /// </summary>
    /// <param name="client">A client that is connected to the server.</param>
    /// <returns>The server version.</returns>
    public static Version GetVersion(BaseClient client)
    {
        string info =
            client is GlideClient standaloneClient
            ? standaloneClient.InfoAsync().GetAwaiter().GetResult()
            : ((GlideClusterClient)client).InfoAsync(Route.Random).GetAwaiter().GetResult().SingleValue;

        string[] lines = info.Split();
        string versionLine = lines.FirstOrDefault(static l => l.Contains("valkey_version")) ?? lines.First(static l => l.Contains("redis_version"));
        return new(versionLine.Split(':')[1]);
    }
}
