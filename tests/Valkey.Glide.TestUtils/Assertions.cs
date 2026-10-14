// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.TestUtils;

/// <summary>
/// Custom test assertions for Valkey GLIDE integration tests.
/// </summary>
public static class Assertions
{
    #region Constants

    private static readonly TimeSpan ReconnectTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan RetryInterval = TimeSpan.FromSeconds(0.5);

    #endregion
    #region Collection Assertions

    /// <summary>
    /// Asserts that the given collections contain equal elements with equal multiplicities, in any order.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="expected">The expected elements.</param>
    /// <param name="actual">The actual elements.</param>
    public static void AssertEqualIgnoreOrder<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        List<T> unexpected = [.. actual];
        List<T> missing = [];

        // Match by index to avoid removing a different element that compares equal to default(T).
        foreach (T item in expected)
        {
            int index = unexpected.FindIndex(a => EqualityComparer<T>.Default.Equals(a, item));
            if (index < 0)
            {
                missing.Add(item);
            }
            else
            {
                unexpected.RemoveAt(index);
            }
        }

        if (missing.Count > 0 || unexpected.Count > 0)
        {
            Assert.Fail(
                "Collections differ (ignoring order).\n" +
                    $"Missing:    [{string.Join(", ", missing)}]\n" +
                    $"Unexpected: [{string.Join(", ", unexpected)}]");
        }
    }

    #endregion
    #region Server Assertions

    /// <summary>
    /// Asserts that the given server is connected.
    /// </summary>
    /// <param name="server">The server to test.</param>
    public static async Task AssertConnected(IServer server)
        => Assert.True(await server.PingAsync() > TimeSpan.Zero);

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

    #endregion
    #region Client Assertions

    /// <summary>
    /// Asserts that the given client is connected.
    /// </summary>
    /// <param name="client">The client to test.</param>
    public static async Task AssertConnected(BaseClient client)
        => Assert.Equal("PONG", await client.PingAsync());

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

    #endregion
}
