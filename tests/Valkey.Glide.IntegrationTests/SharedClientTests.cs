// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(SharedClientTests))]
[CollectionDefinition(DisableParallelization = true)]
public class SharedClientTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    // Large argument test constants.
    private static readonly string SmallString = new("0");
    private static readonly string LargeString = new('0', 1 << 16); // 64 KiB (65,536 characters)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CommandsWithLargeKey(BaseClient client)
        => await StringCommandTests.GetAndSetValuesAsync(client, LargeString, SmallString);

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CommandsWithLargeValue(BaseClient client)
        => await StringCommandTests.GetAndSetValuesAsync(client, SmallString, LargeString);

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CommandsWithLargeKeyAndValue(BaseClient client)
        => await StringCommandTests.GetAndSetValuesAsync(client, LargeString, LargeString);

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptWithLargeKey(BaseClient client)
    {
        using var script = new Script("return KEYS[1]");
        var options = new ScriptOptions().WithKeys(LargeString);

        var result = await client.ScriptInvokeAsync(script, options, TestContext.Current.CancellationToken);

        Assert.Equal(LargeString, result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptWithLargeArgument(BaseClient client)
    {
        using var script = new Script("return ARGV[1]");
        var options = new ScriptOptions().WithArgs(LargeString);

        ValkeyResult result = await client.ScriptInvokeAsync(script, options, TestContext.Current.CancellationToken);

        Assert.Equal(LargeString, result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptWithLargeKeyAndArgument(BaseClient client)
    {
        using var script = new Script("return {KEYS[1], ARGV[1]}");
        var options = new ScriptOptions()
            .WithKeys(LargeString)
            .WithArgs(LargeString);

        var result = await client.ScriptInvokeAsync(script, options, TestContext.Current.CancellationToken);

        Assert.Equal((string[])[LargeString, LargeString], result.AsStringArray());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientPause_ReadsPausedUntilExpires(BaseClient client)
    {
        var key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "value");

        var sw = Stopwatch.StartNew();

        var pauseFor = TimeSpan.FromSeconds(2);
        await client.ClientPauseAsync(pauseFor);

        // Verify that read commands are blocked until the pause expires.
        _ = await client.GetAsync(key);
        Assert.True(sw.Elapsed >= pauseFor);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientPause_WritesPausedUntilExpires(BaseClient client)
    {
        var key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "before");

        var sw = Stopwatch.StartNew();

        var pauseFor = TimeSpan.FromSeconds(2);
        await client.ClientPauseAsync(pauseFor);

        // Verify that write commands are blocked until the pause expires.
        await client.SetAsync(key, "after");
        Assert.True(sw.Elapsed >= pauseFor);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientPauseWrite_ReadsNotPaused(BaseClient client)
    {
        var key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "before");

        var pauseFor = TimeSpan.FromSeconds(2);
        await client.ClientPauseWriteAsync(pauseFor);

        var sw = Stopwatch.StartNew();

        // Verify that read commands are not blocked.
        Assert.Equal("before", await client.GetAsync(key));
        Assert.True(sw.Elapsed < pauseFor);

        await client.ClientUnpauseAsync();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientPauseWrite_ThenUnpause(BaseClient client)
    {
        var key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "before");

        var pausedFor = TimeSpan.FromMinutes(1);
        await client.ClientPauseWriteAsync(pausedFor);

        var sw = Stopwatch.StartNew();

        // Verify that write commands are unblocked once unpaused.
        await client.ClientUnpauseAsync();
        await client.SetAsync(key, "after");
        Assert.True(sw.Elapsed < pausedFor);
    }
    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestReset_ResetsConnectionState(BaseClient client)
    {
        // Verify that ResetAsync completes without error and the client recovers.
        await client.ResetAsync();
        Assert.Equal("PONG", (await client.PingAsync()).ToString());

        // For standalone, verify that RESET clears WATCH state.
        if (client is GlideClient standaloneClient)
        {
            var key = Guid.NewGuid().ToString();

            // WATCH a key, then modify it — normally this aborts a subsequent transaction.
            await standaloneClient.WatchAsync([key]);
            await client.SetAsync(key, "modified");

            // RESET clears the watch.
            await client.ResetAsync();

            // Transaction should succeed because RESET cleared the watch.
            var batch = new Pipeline.Batch(true);
            _ = batch.SetAsync(key, "after-reset");
            object?[]? execResult = await standaloneClient.Exec(batch, true);

            Assert.NotNull(execResult);
            Assert.Equal("after-reset", await client.GetAsync(key));

            // Cleanup.
            _ = await client.DeleteAsync(key);
        }
    }

    // This test is slow, but it caught timing and releasing issues in the past,
    // so it's being kept.
    [Theory(DisableDiscoveryEnumeration = true)]
    [Trait("duration", "long")]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ConcurrentOperationsWork(BaseClient client)
    {
        List<Task> operations = [];

        for (int i = 0; i < 100; ++i)
        {
            int index = i;
            operations.Add(Task.Run(async () =>
            {
                for (int i = 0; i < 1000; ++i)
                {
                    if ((i + index) % 2 == 0)
                    {
                        await StringCommandTests.GetAndSetRandomValuesAsync(client);
                    }
                    else
                    {
                        ValkeyValue result = await client.GetAsync(Guid.NewGuid().ToString());
                        Assert.True(result.IsNull);
                    }
                }
            }, TestContext.Current.CancellationToken));
        }

        await Task.WhenAll(operations);
    }
}
