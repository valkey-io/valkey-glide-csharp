// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for <see cref="ValkeyServer" /> (IServer) methods.
/// </summary>
public class ValkeyServerTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task KeysAsync_ReturnsMatchingKeys(ConnectionMultiplexer conn)
    {
        string prefix = Guid.NewGuid().ToString();
        string key1 = $"{prefix}:key1";
        string key2 = $"{prefix}:key2";
        string key3 = $"{prefix}:key3";
        string otherKey = "other:key";

        var endpoint = conn.GetEndPoints(true)[0];
        var server = conn.GetServer(endpoint);
        var db = conn.GetDatabase();

        // Set up test keys
        _ = await db.StringSetAsync(key1, "value1");
        _ = await db.StringSetAsync(key2, "value2");
        _ = await db.StringSetAsync(key3, "value3");
        _ = await db.StringSetAsync(otherKey, "other");

        // Test scanning all keys with pattern
        List<ValkeyKey> keys = [];
        await foreach (ValkeyKey key in server.KeysAsync(pattern: $"{prefix}:*"))
        {
            keys.Add(key);
        }

        Assert.Equal(3, keys.Count);
        Assert.Contains(key1, keys.Select(k => k.ToString()));
        Assert.Contains(key2, keys.Select(k => k.ToString()));
        Assert.Contains(key3, keys.Select(k => k.ToString()));
        Assert.DoesNotContain(otherKey, keys.Select(k => k.ToString()));

        // Test scanning with pageSize
        keys.Clear();
        await foreach (ValkeyKey key in server.KeysAsync(pattern: $"{prefix}:*", pageSize: 1))
        {
            keys.Add(key);
        }
        Assert.Equal(3, keys.Count);

        // Test scanning with pageOffset
        keys.Clear();
        await foreach (ValkeyKey key in server.KeysAsync(pattern: $"{prefix}:*", pageOffset: 1))
        {
            keys.Add(key);
        }
        Assert.True(keys.Count >= 2);

        // Test scanning non-existent pattern
        keys.Clear();
        await foreach (ValkeyKey key in server.KeysAsync(pattern: "nonexistent:*"))
        {
            keys.Add(key);
        }
        Assert.Empty(keys);
    }
}
