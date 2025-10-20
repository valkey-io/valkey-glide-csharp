// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.IntegrationTests;

public class ClusterScanTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClusterScanAsync_BasicIteration(GlideClusterClient client)
    {
        // Add keys.
        string prefix = Guid.NewGuid().ToString();
        var key1 = new ValkeyKey($"{prefix}:key1");
        var key2 = new ValkeyKey($"{prefix}:key2");

        await client.StringSetAsync(key1, "value1");
        await client.StringSetAsync(key2, "value2");

        // Get all keys with matching prefix.
        var options = new ScanOptions { MatchPattern = $"{prefix}:*" };
        var matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Equivalent(new[] { key1, key2 }, matchingKeys);

        // Remove keys.
        await client.KeyDeleteAsync(new[] { key1, key2 });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClusterScanAsync_TypeFiltering(GlideClusterClient client)
    {
        // Add keys with different types.
        string prefix = Guid.NewGuid().ToString();
        var stringKey = new ValkeyKey($"{prefix}:string");
        var listKey = new ValkeyKey($"{prefix}:list");
        var setKey = new ValkeyKey($"{prefix}:set");

        await client.StringSetAsync(stringKey, "value");
        await client.ListLeftPushAsync(listKey, "item");
        await client.SetAddAsync(setKey, "member");

        // Get all keys with matching type.
        var options = new ScanOptions {MatchPattern = $"{prefix}:*", Type = ValkeyType.String };
        var matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Equivalent(new[] { stringKey }, matchingKeys);

        // Get all matching set keys.
        options = new ScanOptions { MatchPattern = $"{prefix}:*", Type = ValkeyType.Set };
        matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Equivalent(new[] { setKey }, matchingKeys);

        // Remove keys.
        client.KeyDeleteAsync(new[] { stringKey, listKey, setKey });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClusterScanAsync_CombinedOptions(GlideClusterClient client)
    {
        // Add keys with different prefixes and types.
        string prefix = Guid.NewGuid().ToString();
        var matchStringKey = new ValkeyKey($"{prefix}:match:string");
        var matchListKey = new ValkeyKey($"{prefix}:match:list");
        var otherStringKey = new ValkeyKey($"{prefix}:other:string");

        await client.StringSetAsync(matchStringKey, "value");
        await client.ListLeftPushAsync(matchListKey, "item");
        await client.StringSetAsync(otherStringKey, "value");

        // Get all keys with matching type and prefix.
        var options = new ScanOptions
        {
            MatchPattern = $"{prefix}:match:*",
            Count = 10,
            Type = ValkeyType.String
        };
        var matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Equivalent(new[] { matchStringKey }, matchingKeys);

        // Remove keys.
        client.KeyDeleteAsync(new[] { matchStringKey, matchListKey, otherStringKey });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task TestClusterScanAsync_EmptyResults(GlideClusterClient client)
    {
        // Get all keys with non-existent prefix.
        var options = new ScanOptions { MatchPattern = "nonexistent:*" };
        var matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Empty(matchingKeys);
    }
    private static async Task<ValkeyKey[]> ExecuteScanAsync(GlideClusterClient client, ScanOptions? options = null)
    {
        var cursor = ClusterScanCursor.InitialCursor();
        var allKeys = new List<ValkeyKey>();

        while (!cursor.IsFinished)
        {
            (cursor, var keys) = await client.ScanAsync(cursor, options);
            allKeys.AddRange(keys);
        }

        return allKeys.ToArray();
    }
}
