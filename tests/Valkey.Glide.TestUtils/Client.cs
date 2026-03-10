// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Test utilities for Valkey GLIDE clients.
/// </summary>
public static class Client
{
    // Intervals for assertions
    private static readonly TimeSpan ASSERT_RETRY = TimeSpan.FromSeconds(0.5);
    private static readonly TimeSpan ASSERT_TIMEOUT = TimeSpan.FromSeconds(10);

    private static readonly GlideString[] ClientListCommandArgs = ["CLIENT", "LIST"];

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
       // Retry connection until successful for timeout occurs.
       using var cts = new CancellationTokenSource(ASSERT_TIMEOUT);

       while(!cts.Token.IsCancellationRequested)
       {
            try
            {
                await AssertConnected(client);
                return;
            }

            catch (Exception ex) when (ex is Errors.TimeoutException or Errors.ConnectionException)
            {
                await Task.Delay(ASSERT_RETRY);
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

    /// <summary>
    /// Returns the Valkey server version.
    /// </summary>
    /// <param name="client">A client that is connected to the server.</param>
    /// <returns>The server version.</returns>
    public static Version GetVersion(BaseClient client)
    {
        string info =
            client is GlideClient
            ? ((GlideClient)client).InfoAsync().GetAwaiter().GetResult()
            : ((GlideClusterClient)client).InfoAsync(Route.Random).GetAwaiter().GetResult().SingleValue;

        string[] lines = info.Split();
        string versionLine = lines.FirstOrDefault(l => l.Contains("valkey_version")) ?? lines.First(l => l.Contains("redis_version"));
        return new(versionLine.Split(':')[1]);
    }
}
