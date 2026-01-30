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
    {
        await StringCommandTests.GetAndSetValuesAsync(client, LargeString, SmallString);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CommandsWithLargeValue(BaseClient client)
    {
        await StringCommandTests.GetAndSetValuesAsync(client, SmallString, LargeString);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task CommandsWithLargeKeyAndValue(BaseClient client)
    {
        await StringCommandTests.GetAndSetValuesAsync(client, LargeString, LargeString);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptWithLargeKey(BaseClient client)
    {
        using var script = new Script("return KEYS[1]");
        var options = new ScriptOptions().WithKeys(LargeString);

        var result = await client.ScriptInvokeAsync(script, options);

        Assert.Equal(LargeString, result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task ScriptWithLargeArgument(BaseClient client)
    {
        using var script = new Script("return ARGV[1]");
        var options = new ScriptOptions().WithArgs(LargeString);

        ValkeyResult result = await client.ScriptInvokeAsync(script, options);

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

        var result = await client.ScriptInvokeAsync(script, options);

        Assert.Equal((string[])[LargeString, LargeString], result.AsStringArray());
    }

    // This test is slow, but it caught timing and releasing issues in the past,
    // so it's being kept.
    [Theory(DisableDiscoveryEnumeration = true)]
    [Trait("duration", "long")]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public void ConcurrentOperationsWork(BaseClient client)
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
                        ValkeyValue result = await client.StringGetAsync(Guid.NewGuid().ToString());
                        Assert.True(result.IsNull);
                    }
                }
            }));
        }

        Task.WaitAll([.. operations]);
    }
}
