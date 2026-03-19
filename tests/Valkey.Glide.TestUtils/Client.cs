// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE clients.
/// </summary>
public static class Client
{
    // Assertion time spans.
    private static readonly TimeSpan ReconnectTimeSpan = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan AssertTimeSpan = TimeSpan.FromSeconds(0.5);
    private static readonly TimeSpan RetryTimeSpan = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// Asserts that the given client is connected.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertConnected(BaseClient client)
    {
        Task<ValkeyValue> assertTask;
        if (client is GlideClient standaloneClient)
        {
            assertTask = standaloneClient.PingAsync();
        }
        else if (client is GlideClusterClient clusterClient)
        {
            assertTask = clusterClient.PingAsync();
        }
        else
        {
            Assert.Fail("Unknown client type.");
            return;
        }

        ValkeyValue response = await assertTask.WaitAsync(AssertTimeSpan);
        Assert.Equal("PONG", response.ToString());
    }

    /// <summary>
    /// Asserts that the given client is reconnected.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertReconnected(BaseClient client)
    {
        // Retry connection until successful for timeout occurs.
        using CancellationTokenSource cts = new(ReconnectTimeSpan);

        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                Task assertTask = AssertConnected(client);
                await assertTask.WaitAsync(AssertTimeSpan);
                return;
            }

            catch (Exception)
            {
                await Task.Delay(RetryTimeSpan);
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
