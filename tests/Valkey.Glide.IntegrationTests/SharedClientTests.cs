// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
    public async Task TestClientPause_ThenExpires(BaseClient client)
    {
        var key = $"client-pause-all-{Guid.NewGuid()}";
        await client.SetAsync(key, "before");

        var pauseFor = TimeSpan.FromSeconds(2);
        await client.ClientPauseAsync(pauseFor);

        var setTask = client.SetAsync(key, "after");
        var getTask = client.GetAsync(key);

        // Verify that all commands are paused.
        var completesDuringPause = TimeSpan.FromSeconds(1);
        _ = await Assert.ThrowsAsync<TimeoutException>(
            () => setTask.WaitAsync(completesDuringPause, TestContext.Current.CancellationToken));
        _ = await Assert.ThrowsAsync<TimeoutException>(
            () => getTask.WaitAsync(completesDuringPause, TestContext.Current.CancellationToken));

        // Verify that all of the commands complete once the pause expires.
        var completesAfterPause = TimeSpan.FromSeconds(5);
        await setTask.WaitAsync(completesAfterPause, TestContext.Current.CancellationToken);
        _ = await getTask.WaitAsync(completesAfterPause, TestContext.Current.CancellationToken);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientPauseWrite_ThenUnpause(BaseClient client)
    {
        var key = $"client-pause-write-{Guid.NewGuid()}";
        await client.SetAsync(key, "before");

        var pauseFor = TimeSpan.FromSeconds(2);
        await client.ClientPauseWriteAsync(pauseFor);

        var getTask = client.GetAsync(key);
        var setTask = client.SetAsync(key, "after");

        // Verify that only write commands are paused.
        var completesDuringPause = TimeSpan.FromSeconds(1);
        Assert.Equal("before", await getTask.WaitAsync(completesDuringPause, TestContext.Current.CancellationToken));
        _ = await Assert.ThrowsAsync<TimeoutException>(
            () => setTask.WaitAsync(completesDuringPause, TestContext.Current.CancellationToken));

        await client.ClientUnpauseAsync();

        // Verify that write commands completes once unpaused.
        var completesAfterPause = TimeSpan.FromSeconds(5);
        await setTask.WaitAsync(completesAfterPause, TestContext.Current.CancellationToken);
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
