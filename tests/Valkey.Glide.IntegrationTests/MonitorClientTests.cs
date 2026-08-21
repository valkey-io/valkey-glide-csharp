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
    public async Task Monitor_ReportsConfiguredLibNameAndTag()
    {
        Skip.IfClientSetInfoNotSupported();

        const string tag = "monitor-tag:1.0";
        var addr = fixture.Server.Address;
        using var config = BuildMonitorConfig(addr.Host, addr.Port).WithClientInfoTag(tag);
        await using var monitor = await MonitorClient.CreateClient(config);

        string? monitorLibName = await PollMonitorLibNameAsync();
        Assert.Equal($"GlideC#({tag})", monitorLibName);
    }

    #endregion
    #region Helpers

    private async Task<MonitorClient> BuildMonitorClientAsync()
    {
        var addr = fixture.Server.Address;
        using var config = BuildMonitorConfig(addr.Host, addr.Port);
        return await MonitorClient.CreateClient(config);
    }

    /// <summary>
    /// Polls <c>CLIENT LIST</c> (via a normal client) for the MONITOR connection (flag <c>O</c>)
    /// and returns its reported <c>lib-name</c>, or <see langword="null"/> if not found in time.
    /// </summary>
    private async Task<string?> PollMonitorLibNameAsync()
    {
        GlideString[] clientListCommand = ["CLIENT", "LIST"];
        for (int attempt = 0; attempt < 30; attempt++)
        {
            object? result = await Client.CustomCommand(clientListCommand);
            foreach (string line in result!.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] fields = line.Split(' ');
                string? flags = fields.FirstOrDefault(f => f.StartsWith("flags=", StringComparison.Ordinal));
                if (flags is null || !flags["flags=".Length..].Contains('O'))
                {
                    continue;
                }

                string? libName = fields.FirstOrDefault(f => f.StartsWith("lib-name=", StringComparison.Ordinal));
                return libName?["lib-name=".Length..];
            }

            await Task.Delay(100);
        }

        return null;
    }

    #endregion
}
