// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

public class ScanTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestScanAsync_PrefixFiltering(BaseClient client)
    {
        // Add keys.
        string prefix = Guid.NewGuid().ToString();
        var key1 = new ValkeyKey($"{prefix}:key1");
        var key2 = new ValkeyKey($"{prefix}:key2");

        await client.StringSetAsync(key1, "value1");
        await client.StringSetAsync(key2, "value2");

        // Get all keys with matching prefix.
        var matchingKeys = await ExecuteScanAsync(client, $"{prefix}:*");

        Assert.Equivalent(new[] { key1, key2 }, matchingKeys);

        // Get all keys with non-existent prefix.
        matchingKeys = await ExecuteScanAsync(client, "nonexistent:*");

        Assert.Empty(matchingKeys);

        // Remove keys.
        _ = await client.DeleteAsync([key1, key2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestScanAsync_BasicIteration(BaseClient client)
    {
        // Add keys with different types.
        string prefix = Guid.NewGuid().ToString();
        var stringKey = new ValkeyKey($"{prefix}:string");
        var listKey = new ValkeyKey($"{prefix}:list");
        var setKey = new ValkeyKey($"{prefix}:set");

        await client.StringSetAsync(stringKey, "value");
        _ = await client.ListLeftPushAsync(listKey, "item");
        _ = await client.SetAddAsync(setKey, "member");

        // Get all keys with prefix.
        var matchingKeys = await ExecuteScanAsync(client, $"{prefix}:*");

        Assert.Equivalent(new[] { stringKey, listKey, setKey }, matchingKeys);

        // Remove keys.
        _ = await client.DeleteAsync([stringKey, listKey, setKey]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestScanAsync_WithPageSize(BaseClient client)
    {
        // Add keys with different prefixes.
        string prefix = Guid.NewGuid().ToString();
        var matchStringKey = new ValkeyKey($"{prefix}:match:string");
        var matchListKey = new ValkeyKey($"{prefix}:match:list");
        var otherStringKey = new ValkeyKey($"{prefix}:other:string");

        await client.StringSetAsync(matchStringKey, "value");
        _ = await client.ListLeftPushAsync(matchListKey, "item");
        await client.StringSetAsync(otherStringKey, "value");

        // Get all keys with matching prefix using small page size.
        var matchingKeys = await ExecuteScanAsync(client, $"{prefix}:match:*", pageSize: 1);

        Assert.Equivalent(new[] { matchStringKey, matchListKey }, matchingKeys);

        // Remove keys.
        _ = await client.DeleteAsync([matchStringKey, matchListKey, otherStringKey]);
    }

    private static async Task<ValkeyKey[]> ExecuteScanAsync(BaseClient client, ValkeyValue pattern = default, int pageSize = 250)
    {
        var allKeys = new List<ValkeyKey>();

        if (client is GlideClient standaloneClient)
        {
            await foreach (var key in standaloneClient.ScanAsync(pattern, pageSize))
            {
                allKeys.Add(key);
            }
        }
        else
        {
            await foreach (var key in ((GlideClusterClient)client).ScanAsync(pattern, pageSize))
            {
                allKeys.Add(key);
            }
        }

        return [.. allKeys];
    }
}
