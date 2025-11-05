// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

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
        var options = new ScanOptions { MatchPattern = $"{prefix}:*" };
        var matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Equivalent(new[] { key1, key2 }, matchingKeys);

        // Get all keys with non-existent prefix.
        options = new ScanOptions { MatchPattern = $"nonexistent:*" };
        matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Empty(matchingKeys);

        // Remove keys.
        await client.KeyDeleteAsync([key1, key2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestScanAsync_TypeFiltering(BaseClient client)
    {
        // Add keys with different types.
        string prefix = Guid.NewGuid().ToString();
        var stringKey = new ValkeyKey($"{prefix}:string");
        var listKey = new ValkeyKey($"{prefix}:list");
        var setKey = new ValkeyKey($"{prefix}:set");

        await client.StringSetAsync(stringKey, "value");
        await client.ListLeftPushAsync(listKey, "item");
        await client.SetAddAsync(setKey, "member");

        // Get all keys with string type.
        var options = new ScanOptions { MatchPattern = $"{prefix}:*", Type = ValkeyType.String };
        var matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Equivalent(new[] { stringKey }, matchingKeys);

        // Get all keys with set type.
        options = new ScanOptions { MatchPattern = $"{prefix}:*", Type = ValkeyType.Set };
        matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Equivalent(new[] { setKey }, matchingKeys);

        // Get all keys with non-existent type.
        options = new ScanOptions { MatchPattern = $"{prefix}:*", Type = ValkeyType.Hash };
        matchingKeys = await ExecuteScanAsync(client, options);

        Assert.Empty(matchingKeys);

        // Remove keys.
        await client.KeyDeleteAsync([stringKey, listKey, setKey]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestScanAsync_CombinedOptions(BaseClient client)
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
        await client.KeyDeleteAsync([matchStringKey, matchListKey, otherStringKey]);
    }

    [Fact]
    public async Task TestScanAsync_InvalidCursorId()
    {
        await Assert.ThrowsAsync<Valkey.Glide.Errors.RequestException>(() =>
        {
            return TestConfiguration.DefaultStandaloneClient().ScanAsync("invalid");
        });

        await Assert.ThrowsAsync<Valkey.Glide.Errors.RequestException>(() =>
        {
            return TestConfiguration.DefaultClusterClient().ScanAsync(new ClusterScanCursor("invalid"));
        });
    }

    private static async Task<ValkeyKey[]> ExecuteScanAsync(BaseClient client, ScanOptions? options = null)
    {
        var allKeys = new List<ValkeyKey>();

        if (client is GlideClient)
        {
            string cursor = "0";
            do
            {
                (cursor, var keys) = await ((GlideClient)client).ScanAsync(cursor, options);
                allKeys.AddRange(keys);
            } while (cursor != "0");
        }
        else
        {
            var cursor = ClusterScanCursor.InitialCursor();
            while (!cursor.IsFinished)
            {
                (cursor, var keys) = await ((GlideClusterClient)client).ScanAsync(cursor, options);
                allKeys.AddRange(keys);
            }
        }

        return [.. allKeys];
    }
}
