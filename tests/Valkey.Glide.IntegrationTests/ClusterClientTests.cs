// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Text;

using Valkey.Glide.Pipeline;

using static Valkey.Glide.Commands.Options.InfoOptions;
using static Valkey.Glide.Errors;
using static Valkey.Glide.Route;

namespace Valkey.Glide.IntegrationTests;

public class ClusterClientTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

#pragma warning disable xUnit1046 // Avoid using TheoryDataRow arguments that are not serializable
    public static IEnumerable<TheoryDataRow<GlideClusterClient, bool>> ClusterClientWithAtomic =>
        TestConfiguration.TestClusterClients.SelectMany(r => new TheoryDataRow<GlideClusterClient, bool>[] { new(r.Data, true), new(r.Data, false) });
#pragma warning restore xUnit1046 // Avoid using TheoryDataRow arguments that are not serializable

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task CustomCommand(GlideClusterClient client)
    {
        // command which returns always a single value
        long res = (long)(await client.CustomCommand(["dbsize"])).SingleValue!;
        Assert.True(res >= 0);
        // command which returns a multi value by default
        Dictionary<string, object?> info = (await client.CustomCommand(["info"])).MultiValue;
        foreach (object? nodeInfo in info.Values)
        {
            Assert.Contains("# Server", (nodeInfo as gs)!);
        }
        // command which returns a map even on a single node route
        ClusterValue<object?> config = await client.CustomCommand(["config", "get", "*file"], Route.Random);
        Assert.True((config.SingleValue as Dictionary<gs, object?>)!.Count > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task CustomCommandWithRandomRoute(GlideClusterClient client)
    {
        // if a command isn't routed in 100 tries to different nodes, you are a lucker or have a bug
        SortedSet<string> ports = [];
        foreach (int i in Enumerable.Range(0, 100))
        {
            string res = ((await client.CustomCommand(["info", "server"], Route.Random)).SingleValue! as gs)!;
            foreach (string line in res!.Split("\r\n"))
            {
                if (line.Contains("tcp_port"))
                {
                    _ = ports.Add(line);
                    if (ports.Count > 1)
                    {
                        return;
                    }
                    break;
                }
            }
        }
        Assert.Fail($"All 100 commands were sent to: {ports.First()}");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task CustomCommandWithSingleNodeRoute(GlideClusterClient client)
    {
        string res = ((await client.CustomCommand(["info", "replication"], new SlotKeyRoute("abc", SlotType.Primary))).SingleValue! as gs)!;
        Assert.Contains("role:master", res);

        res = ((await client.CustomCommand(["info", "replication"], new SlotKeyRoute("abc", SlotType.Replica))).SingleValue! as gs)!;
        Assert.Contains("role:slave", res);

        res = ((await client.CustomCommand(["info", "replication"], new SlotIdRoute(42, SlotType.Primary))).SingleValue! as gs)!;
        Assert.Contains("role:master", res);

        res = ((await client.CustomCommand(["info", "replication"], new SlotIdRoute(42, SlotType.Replica))).SingleValue! as gs)!;
        Assert.Contains("role:slave", res);

        res = ((await client.CustomCommand(["info", "replication"], new ByAddressRoute(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port))).SingleValue! as gs)!;
        Assert.Contains("# Replication", res);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task CustomCommandWithMultiNodeRoute(GlideClusterClient client)
    {
        _ = await client.StringSetAsync("abc", "abc");
        _ = await client.StringSetAsync("klm", "klm");
        _ = await client.StringSetAsync("xyz", "xyz");

        long res = (long)(await client.CustomCommand(["dbsize"], AllPrimaries)).SingleValue!;
        Assert.True(res >= 3);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task RetryStrategyIsNotSupportedForTransactions(GlideClusterClient client)
        => _ = await Assert.ThrowsAsync<RequestException>(async () => _ = await client.Exec(new(true), true, new(retryStrategy: new())));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(ClusterClientWithAtomic))]
    public async Task BatchWithSingleNodeRoute(GlideClusterClient client, bool isAtomic)
    {
        ClusterBatch batch = new ClusterBatch(isAtomic).Info([Section.REPLICATION]);

        object?[]? res = await client.Exec(batch, true, new(route: new SlotKeyRoute("abc", SlotType.Primary)));
        Assert.Contains("role:master", res![0] as string);

        res = await client.Exec(batch, true, new(route: new SlotKeyRoute("abc", SlotType.Replica)));
        Assert.Contains("role:slave", res![0] as string);

        res = await client.Exec(batch, true, new(route: new SlotIdRoute(42, SlotType.Primary)));
        Assert.Contains("role:master", res![0] as string);

        res = await client.Exec(batch, true, new(route: new SlotIdRoute(42, SlotType.Replica)));
        Assert.Contains("role:slave", res![0] as string);

        res = await client.Exec(batch, true, new(route: new ByAddressRoute(TestConfiguration.CLUSTER_HOSTS[0].host, TestConfiguration.CLUSTER_HOSTS[0].port)));
        Assert.Contains("# Replication", res![0] as string);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task Info(GlideClusterClient client)
    {
        Dictionary<string, string> info = await client.InfoAsync();
        foreach (string nodeInfo in info.Values)
        {
            Assert.Multiple([
                () => Assert.Contains("# Server", nodeInfo),
                () => Assert.Contains("# Replication", nodeInfo),
                () => Assert.DoesNotContain("# Latencystats", nodeInfo),
            ]);
        }

        info = await client.InfoAsync([Section.REPLICATION]);
        foreach (string nodeInfo in info.Values)
        {
            Assert.Multiple([
                () => Assert.DoesNotContain("# Server", nodeInfo),
                () => Assert.Contains("# Replication", nodeInfo),
                () => Assert.DoesNotContain("# Latencystats", nodeInfo),
            ]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task InfoWithRoute(GlideClusterClient client)
    {
        ClusterValue<string> info = await client.InfoAsync(Route.Random);
        Assert.Multiple([
            () => Assert.Contains("# Server", info.SingleValue),
            () => Assert.Contains("# Replication", info.SingleValue),
            () => Assert.DoesNotContain("# Latencystats", info.SingleValue),
        ]);

        info = await client.InfoAsync(AllPrimaries);
        foreach (string nodeInfo in info.MultiValue.Values)
        {
            Assert.Multiple([
                () => Assert.Contains("# Server", nodeInfo),
                () => Assert.Contains("# Replication", nodeInfo),
                () => Assert.DoesNotContain("# Latencystats", nodeInfo),
            ]);
        }

        info = await client.InfoAsync([Section.ERRORSTATS], AllNodes);

        foreach (string nodeInfo in info.MultiValue.Values)
        {
            Assert.Multiple([
                () => Assert.DoesNotContain("# Server", nodeInfo),
                () => Assert.Contains("# Errorstats", nodeInfo),
            ]);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestPing_NoMessage(GlideClusterClient client)
    {
        TimeSpan result = await client.PingAsync();
        Assert.True(result >= TimeSpan.Zero);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestPing_NoMessage_WithRoute(GlideClusterClient client)
    {
        TimeSpan result = await client.PingAsync(AllNodes);
        Assert.True(result >= TimeSpan.Zero);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestPing_WithMessage(GlideClusterClient client)
    {
        ValkeyValue message = "Hello, Valkey!";
        TimeSpan result = await client.PingAsync(message);
        Assert.True(result >= TimeSpan.Zero);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestPing_WithMessage_WithRoute(GlideClusterClient client)
    {
        ValkeyValue message = "Hello, Valkey!";
        TimeSpan result = await client.PingAsync(message, AllNodes);
        Assert.True(result >= TimeSpan.Zero);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestEcho_SimpleMessage(GlideClusterClient client)
    {
        ValkeyValue message = "Hello, Valkey!";
        ValkeyValue result = await client.EchoAsync(message);
        Assert.Equal(message, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestEcho_SimpleMessage_WithRoute(GlideClusterClient client)
    {
        ValkeyValue message = "Hello, Valkey!";
        ClusterValue<ValkeyValue> result = await client.EchoAsync(message, AllNodes);

        Assert.True(result.HasMultiData);
        foreach (ValkeyValue echo in result.MultiValue.Values)
        {
            Assert.Equal(message, echo);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestEcho_BinaryData(GlideClusterClient client)
    {
        byte[] binaryData = [0x00, 0x01, 0x02, 0xFF, 0xFE];
        ValkeyValue result = await client.EchoAsync(binaryData);
        Assert.Equal(binaryData, (byte[]?)result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestEcho_BinaryData_WithRoute(GlideClusterClient client)
    {
        byte[] binaryData = [0x00, 0x01, 0x02, 0x03, 0x04];
        ClusterValue<ValkeyValue> result = await client.EchoAsync(binaryData, AllNodes);

        Assert.True(result.HasMultiData);
        foreach (ValkeyValue ev in result.MultiValue.Values)
        {
            string echo = ev!.ToString();
            byte[] bytes = Encoding.ASCII.GetBytes(echo);
            Assert.Equivalent(binaryData, bytes);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigGetAsync_ReturnsConfigurationPerNode(GlideClusterClient client)
    {
        // Test getting all configuration from all nodes
        var allConfig = await client.ConfigGetAsync("*");
        Assert.True(allConfig.HasMultiData);
        Assert.NotEmpty(allConfig.MultiValue);

        // Each node should have configuration
        foreach (var nodeConfig in allConfig.MultiValue.Values)
        {
            Assert.NotEmpty(nodeConfig);
            // Check for any configuration parameter (timeout is commonly available)
            Assert.True(nodeConfig.Any(), "Node should have at least one configuration parameter");
        }

        // Test with specific route using maxmemory-policy parameter (which should exist)
        var singleNodeConfig = await client.ConfigGetAsync("maxmemory-policy", Route.Random);
        Assert.NotNull(singleNodeConfig.SingleValue);
        Assert.Single(singleNodeConfig.SingleValue);
        Assert.Equal("maxmemory-policy", singleNodeConfig.SingleValue[0].Key);

        // Test ConfigSet and ConfigGet combination (like Go TestConfigSetGet)
        // Use maxmemory-policy instead of timeout as it's more reliably settable
        var originalPolicy = await client.ConfigGetAsync("maxmemory-policy", Route.Random);
        string originalValue = originalPolicy.SingleValue?.FirstOrDefault().Value ?? "noeviction";

        try
        {
            await client.ConfigSetAsync((ValkeyValue)"maxmemory-policy", (ValkeyValue)"allkeys-lru");

            var result = await client.ConfigGetAsync("maxmemory-policy");
            Assert.True(result.HasMultiData);
            Assert.NotEmpty(result.MultiValue);
            foreach (var nodeConfig in result.MultiValue.Values)
            {
                Assert.Contains(nodeConfig, kvp => kvp.Key == "maxmemory-policy" && kvp.Value == "allkeys-lru");
            }

            // Test with route options (like Go TestConfigSetGetWithOptions)
            // Set on all primaries and verify we can read from primaries
            await client.ConfigSetAsync((ValkeyValue)"maxmemory-policy", (ValkeyValue)"allkeys-lfu", Route.AllPrimaries);

            // Get from all primaries to verify the change was applied
            var primariesResult = await client.ConfigGetAsync("maxmemory-policy", Route.AllPrimaries);
            Assert.NotEmpty(primariesResult.MultiValue.Values);
            foreach (var nodeConfig in primariesResult.MultiValue.Values)
            {
                Assert.Contains(nodeConfig, kvp => kvp.Key == "maxmemory-policy" && kvp.Value == "allkeys-lfu");
            }

            // Test setting different value and verify it propagates to all primaries
            await client.ConfigSetAsync((ValkeyValue)"maxmemory-policy", (ValkeyValue)"volatile-lru", Route.AllPrimaries);
            var finalResult = await client.ConfigGetAsync("maxmemory-policy", Route.AllPrimaries);
            Assert.NotEmpty(finalResult.MultiValue.Values);
            foreach (var nodeConfig in finalResult.MultiValue.Values)
            {
                Assert.Contains(nodeConfig, kvp => kvp.Key == "maxmemory-policy" && kvp.Value == "volatile-lru");
            }
        }
        finally
        {
            // Restore original value
            await client.ConfigSetAsync((ValkeyValue)"maxmemory-policy", (ValkeyValue)originalValue, Route.AllPrimaries);
        }

        // Test invalid parameters
        await Assert.ThrowsAsync<RequestException>(async () =>
            await client.ConfigSetAsync((ValkeyValue)"invalid-config-param", (ValkeyValue)"value"));

        // Test getting non-existent configuration
        var nonExistentResult = await client.ConfigGetAsync("non-existent-config");
        Assert.True(nonExistentResult.HasMultiData);
        foreach (var nodeConfig in nonExistentResult.MultiValue.Values)
        {
            Assert.Empty(nodeConfig);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientId(GlideClusterClient client)
    {
        long clientId = await client.ClientIdAsync();
        Assert.True(clientId > 0, "Client ID should be a positive number");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientId_WithRoute(GlideClusterClient client)
    {
        // Test CLIENT ID with single node routing
        var singleNodeResult = await client.ClientIdAsync(Route.Random);
        Assert.True(singleNodeResult.HasSingleData);
        Assert.True(singleNodeResult.SingleValue > 0);

        // Test CLIENT ID with all nodes routing
        var allNodesResult = await client.ClientIdAsync(AllNodes);
        Assert.True(allNodesResult.HasMultiData);
        Assert.True(allNodesResult.MultiValue.Count > 0);

        foreach (var kvp in allNodesResult.MultiValue)
        {
            Assert.True(kvp.Value > 0, $"Client ID for node {kvp.Key} should be positive");
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task DatabaseSizeAsync_ReturnsSizePerNode(GlideClusterClient client)
    {
        string key = $"cluster-dbsize-test-{Guid.NewGuid()}";

        try
        {
            // Add a key
            await client.StringSetAsync(key, "test-value");

            // Get database size from all nodes
            var allSizes = await client.DatabaseSizeAsync();
            Assert.NotEmpty(allSizes);

            // Each node should have a size >= 0
            foreach (var size in allSizes.Values)
            {
                Assert.True(size >= 0);
            }

            // Test with specific route
            var singleNodeSize = await client.DatabaseSizeAsync(Route.Random);
            Assert.True(singleNodeSize.SingleValue >= 0);
        }
        finally
        {
            await client.KeyDeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientGetName(GlideClusterClient client)
    {
        // CLIENT GETNAME should return ValkeyValue null initially (no name set)
        ValkeyValue clientName = await client.ClientGetNameAsync();
        Assert.Equal(ValkeyValue.Null, clientName);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClientGetName_WithRoute(GlideClusterClient client)
    {
        // Test CLIENT GETNAME with single node routing
        var singleNodeResult = await client.ClientGetNameAsync(Route.Random);
        Assert.True(singleNodeResult.HasSingleData);
        Assert.Equal(ValkeyValue.Null, singleNodeResult.SingleValue);

        // Test CLIENT GETNAME with all nodes routing
        var allNodesResult = await client.ClientGetNameAsync(AllNodes);
        Assert.True(allNodesResult.HasMultiData);
        Assert.True(allNodesResult.MultiValue.Count > 0);

        foreach (var kvp in allNodesResult.MultiValue)
        {
            Assert.Equal(ValkeyValue.Null, kvp.Value); // No name should be set initially on any node
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TimeAsync_ReturnsTimePerNode(GlideClusterClient client)
    {
        // Get time from all nodes
        var allTimes = await client.TimeAsync();
        Assert.NotEmpty(allTimes);

        DateTime localTime = DateTime.UtcNow;

        // Each node should have a reasonable time
        foreach (var serverTime in allTimes.Values)
        {
            TimeSpan diff = (serverTime - localTime).Duration();
            Assert.True(diff < TimeSpan.FromSeconds(10),
                $"Server time {serverTime} differs from local time {localTime} by {diff}");
        }

        // Test with specific route
        var singleNodeTime = await client.TimeAsync(Route.Random);
        TimeSpan singleDiff = (singleNodeTime.SingleValue - localTime).Duration();
        Assert.True(singleDiff < TimeSpan.FromSeconds(10));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task LastSaveAsync_ReturnsLastSavePerNode(GlideClusterClient client)
    {
        // Get last save time from all nodes
        var allLastSaves = await client.LastSaveAsync();
        Assert.NotEmpty(allLastSaves);

        // Each node should have a valid last save time
        foreach (var lastSave in allLastSaves.Values)
        {
            Assert.NotEqual(DateTime.MinValue, lastSave);
            Assert.True(lastSave <= DateTime.UtcNow);
            Assert.True(lastSave >= DateTime.UtcNow.AddDays(-30));
        }

        // Test with specific route
        var singleNodeLastSave = await client.LastSaveAsync(Route.Random);
        Assert.NotEqual(DateTime.MinValue, singleNodeLastSave.SingleValue);
        Assert.True(singleNodeLastSave.SingleValue <= DateTime.UtcNow);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task LolwutAsync_ReturnsArtPerNode(GlideClusterClient client)
    {
        // Get lolwut from all nodes
        var allLolwut = await client.LolwutAsync();
        Assert.NotEmpty(allLolwut);

        // Each node should return some art
        foreach (var lolwut in allLolwut.Values)
        {
            Assert.NotEmpty(lolwut);
            // Accept both "Valkey" and "Redis" in the output since the server might be either
            Assert.True(lolwut.Contains("Valkey", StringComparison.OrdinalIgnoreCase) ||
                       lolwut.Contains("Redis", StringComparison.OrdinalIgnoreCase));
        }

        // Test with specific route
        var singleNodeLolwut = await client.LolwutAsync(Route.Random);
        Assert.NotEmpty(singleNodeLolwut.SingleValue);
        // Accept both "Valkey" and "Redis" in the output since the server might be either
        Assert.True(singleNodeLolwut.SingleValue.Contains("Valkey", StringComparison.OrdinalIgnoreCase) ||
                   singleNodeLolwut.SingleValue.Contains("Redis", StringComparison.OrdinalIgnoreCase));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSelect(GlideClusterClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "SELECT for Cluster Client is supported since 9.0.0"
        );
        string result = await client.SelectAsync(0);
        Assert.Equal("OK", result);
    }

    [Fact]
    public async Task TestClusterDatabaseId()
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "Multi-database support for Cluster Client requires Valkey 9.0+"
        );

        var config = TestConfiguration.DefaultClusterClientConfig()
            .WithDataBaseId(1)
            .Build();

        using var client = await GlideClusterClient.CreateClient(config);

        // Verify we can connect with database ID 1
        TimeSpan result = await client.PingAsync();
        Assert.True(result >= TimeSpan.Zero);

        // Verify database isolation by setting a key in database 1
        string testKey = Guid.NewGuid().ToString();
        string testValue = "test_value_db1";
        await client.StringSetAsync(testKey, testValue);

        // Verify the key exists in database 1
        ValkeyValue retrievedValue = await client.StringGetAsync(testKey);
        Assert.Equal(testValue, retrievedValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyMoveAsync(GlideClusterClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "MOVE command for Cluster Client requires Valkey 9.0+ with multi-database support"
        );

        string key = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key in the current database
        await client.StringSetAsync(key, value);

        // Move the key to database 1
        bool moveResult = await client.KeyMoveAsync(key, 1);
        Assert.True(moveResult);

        // Verify the key no longer exists in the current database
        Assert.False(await client.KeyExistsAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestKeyCopyAsync(GlideClusterClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "COPY command with database parameter for Cluster Client requires Valkey 9.0+ with multi-database support"
        );

        string sourceKey = Guid.NewGuid().ToString();
        string destKey = Guid.NewGuid().ToString();
        string value = "test_value";

        // Set a key in the current database
        await client.StringSetAsync(sourceKey, value);

        // Copy the key to database 1
        bool copyResult = await client.KeyCopyAsync(sourceKey, destKey, 1);
        Assert.True(copyResult);

        // Verify the source key still exists in the current database
        Assert.True(await client.KeyExistsAsync(sourceKey));
    }
}
