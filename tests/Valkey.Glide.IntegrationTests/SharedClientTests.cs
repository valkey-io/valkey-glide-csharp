// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

[Collection(typeof(SharedClientTests))]
[CollectionDefinition(DisableParallelization = true)]
public class SharedClientTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    private static readonly string SmallString = new("0");
    private static readonly string VeryLargeString = new('0', 1 << 16); // 64KB (2^16 characters)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task HandleVeryLargeKey(BaseClient client)
    {
        await StringCommandTests.GetAndSetValuesAsync(client, VeryLargeString, SmallString);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task HandleVeryLargeValue(BaseClient client)
    {
        await StringCommandTests.GetAndSetValuesAsync(client, SmallString, VeryLargeString);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task HandleVeryLargeKeyAndValue(BaseClient client)
    {
        await StringCommandTests.GetAndSetValuesAsync(client, VeryLargeString, VeryLargeString);
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
