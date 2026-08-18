// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Custom test assertions for Valkey GLIDE integration tests.
/// </summary>
public static class Assertions
{
    private static readonly TimeSpan ReconnectTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// Asserts that the given client is connected.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertConnected(BaseClient client)
        => Assert.Equal("PONG", await client.PingAsync());

    /// <summary>
    /// Asserts that the given server is connected.
    /// </summary>
    /// <param name="server">The server to test.</param>
    public static async Task AssertConnected(IServer server)
        => Assert.True(await server.PingAsync() > TimeSpan.Zero);

    /// <summary>
    /// Asserts that the given client reconnects within the timeout.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertReconnected(BaseClient client)
    {
        using CancellationTokenSource cts = new(ReconnectTimeout);
        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                await AssertConnected(client);
                return;
            }
            catch when (!cts.Token.IsCancellationRequested)
            {
                await Task.Delay(RetryInterval);
            }
        }

        Assert.Fail("Reconnection failed.");
    }

    /// <summary>
    /// Asserts that the given server connection reconnects within the timeout.
    /// </summary>
    /// <param name="server">The server to test.</param>
    public static async Task AssertReconnected(IServer server)
    {
        using CancellationTokenSource cts = new(ReconnectTimeout);
        while (!cts.Token.IsCancellationRequested)
        {
            try
            {
                await AssertConnected(server);
                return;
            }
            catch when (!cts.Token.IsCancellationRequested)
            {
                await Task.Delay(RetryInterval);
            }
        }

        Assert.Fail("Reconnection failed.");
    }
}
