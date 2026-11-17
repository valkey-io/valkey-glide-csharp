// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Builders;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for <see cref="MonitorClient"/>.
/// </summary>
[Collection(typeof(MonitorClientTests))]
[CollectionDefinition(DisableParallelization = true)]
public class MonitorClientTests(StandaloneClientFixture fixture) : IClassFixture<StandaloneClientFixture>
{
    private GlideClient Client => fixture.Client;

    #region Tests

    [Fact]
    public async Task GetMessagesAsync_ReceivesCommands()
    {
        await using var monitor = await BuildMonitorClientAsync();

        var key = $"monitor-test-{Guid.NewGuid()}";
        var before = await Client.TimeAsync();
        await Client.SetAsync(key, "hello");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        try
        {
            await foreach (var msg in monitor.GetMessagesAsync(cts.Token))
            {
                if (msg.Command == "SET" && msg.Args.Contains(key))
                {
                    Assert.True(msg.Timestamp > before);
                    Assert.Equal(0, msg.Database);
                    Assert.StartsWith(fixture.Server.Address.Host, msg.ClientAddress);
                    Assert.Equal(2, msg.Args.Count);
                    Assert.Equal(key, msg.Args[0]);
                    Assert.Equal("hello", msg.Args[1]);

                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
            Assert.Fail("SET command not found in monitor output");
        }
    }

    [Fact]
    public async Task GetMessagesAsync_Cancellation()
    {
        await using var monitor = await BuildMonitorClientAsync();

        using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(100));

        // Should cancel before or during iteration
        _ = await Assert.ThrowsAsync<OperationCanceledException>(async ()
            =>
        { await foreach (var _ in monitor.GetMessagesAsync(cts.Token)) { } });
    }

    [Fact]
    public async Task GetMessagesAsync_ThrowsWhenDisposed()
    {
        MonitorClient monitor = await BuildMonitorClientAsync();
        await monitor.DisposeAsync();

        _ = await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
        {
            await foreach (MonitorMessage _ in monitor.GetMessagesAsync(TestContext.Current.CancellationToken))
            {
            }
        });
    }

    [Fact]
    public async Task Dispose_Idempotent()
    {
        var monitor = await BuildMonitorClientAsync();

        monitor.Dispose();
        monitor.Dispose(); // Should not throw
    }

    [Fact]
    public async Task DisposeAsync_Idempotent()
    {
        var monitor = await BuildMonitorClientAsync();

        await monitor.DisposeAsync();
        await monitor.DisposeAsync(); // Should not throw
    }

    [Fact]
    public async Task Finalizer_ClosesUndisposedClient()
    {
        var monitorRef = await BuildMonitorClientWeakReferenceAsync();
        await WaitForMonitorOpenAsync();

        // Collect the undisposed client and run its finalizer.
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        Assert.False(monitorRef.IsAlive);

        // Commands must not invoke the collected callback delegate.
        await Client.SetAsync($"monitor-test-{Guid.NewGuid()}", "hello");
        await WaitForMonitorCloseAsync();
    }

    [Fact]
    public async Task Monitor_ReportsConfiguredLibNameAndTag()
    {
        Version.SkipIfLessThan(Version.Valkey7_2);

        const string tag = "monitor-tag:1.0";
        var addr = fixture.Server.Address;
        using var config = BuildMonitorConfig(addr.Host, addr.Port).WithClientInfoTag(tag);
        await using var monitor = await MonitorClient.CreateClient(config);

        await WaitForLibNameAsync($"GlideC#({tag})");
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Builds and returns a new monitor client.
    /// </summary>
    private async Task<MonitorClient> BuildMonitorClientAsync()
    {
        var addr = fixture.Server.Address;
        using var config = BuildMonitorConfig(addr.Host, addr.Port);
        return await MonitorClient.CreateClient(config);
    }

    /// <summary>
    /// Builds a new monitor client and returns a weak reference to it.
    /// </summary>
    private async Task<WeakReference> BuildMonitorClientWeakReferenceAsync()
        => new(await BuildMonitorClientAsync());

    /// <summary>
    /// Returns the <c>CLIENT LIST</c> fields of the MONITOR connection (flag <c>O</c>),
    /// or <see langword="null"/> if there is none.
    /// </summary>
    private async Task<Dictionary<string, string>?> GetMonitorConnectionAsync()
    {
        // Get one line per client connection.
        GlideString[] clientListCommand = ["CLIENT", "LIST"];
        object? result = await Client.CustomCommand(clientListCommand);
        string[] lines = result!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            // Parse line (e.g. <c>id=7 addr=127.0.0.1:51234 flags=O</c>)
            // into a dictionary mapping each field name to its value.
            Dictionary<string, string> fields = [];
            foreach (string field in line.Split(' '))
            {
                string[] pair = field.Split('=', 2);
                if (pair.Length == 2)
                {
                    fields[pair[0]] = pair[1];
                }
            }

            // Return the results if the connection is in MONITOR mode ('O' flag);
            if (fields.TryGetValue("flags", out string? flags) && flags.Contains('O'))
            {
                return fields;
            }
        }

        return null;
    }

    /// <summary>
    /// Polls until a monitor connection is open.
    /// </summary>
    private async Task WaitForMonitorOpenAsync()
        => await Polling.WaitForAsync(
            async () => await GetMonitorConnectionAsync() is not null,
            "Monitor connection did not open in time.");

    /// <summary>
    /// Polls until no monitor connections are open.
    /// </summary>
    private async Task WaitForMonitorCloseAsync()
        => await Polling.WaitForAsync(
            async () => await GetMonitorConnectionAsync() is null,
            "Monitor connection did not close in time.");

    /// <summary>
    /// Polls until the monitor connection reports the specified library name.
    /// </summary>
    private async Task WaitForLibNameAsync(string expected)
        => await Polling.WaitForAsync(
            async () => (await GetMonitorConnectionAsync())?.GetValueOrDefault("lib-name") == expected,
            $"Monitor connection did not report lib-name '{expected}' in time.");

    #endregion
}
