// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for client-side caching configuration and metrics.
/// </summary>
public class ClientSideCacheTests
{
    #region Helpers

    private static async Task<GlideClient> CreateStandaloneClientWithCache(ClientSideCacheConfig cache)
    {
        var config = TestConfiguration.DefaultClientConfig()
            .WithClientSideCache(cache)
            .Build();
        return await GlideClient.CreateClient(config);
    }

    private static async Task<GlideClusterClient> CreateClusterClientWithCache(ClientSideCacheConfig cache)
    {
        var config = TestConfiguration.DefaultClusterClientConfig()
            .WithClientSideCache(cache)
            .Build();
        return await GlideClusterClient.CreateClient(config);
    }

    private static async Task<BaseClient> CreateClientWithCache(ClientSideCacheConfig cache, bool clusterMode)
    {
        if (clusterMode)
        {
            return await CreateClusterClientWithCache(cache);
        }

        return await CreateStandaloneClientWithCache(cache);
    }

    #endregion

    #region Basic Cache Hit With Metrics

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task BasicCacheHitWithMetrics(bool clusterMode)
    {
        var cache = new ClientSideCacheConfig(1024, TimeSpan.FromMinutes(1))
            .WithMetrics(true);

        await using var client = await CreateClientWithCache(cache, clusterMode);

        string key = $"cache_test_{Guid.NewGuid()}";
        string expectedValue = Guid.NewGuid().ToString();

        // SET a key
        await client.SetAsync(key, expectedValue);

        // First GET — cache miss
        var value = await client.GetAsync(key);
        Assert.Equal(expectedValue, value.ToString());

        // Entry count should be 1
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(1, entryCount);

        // Second GET — cache hit
        value = await client.GetAsync(key);
        Assert.Equal(expectedValue, value.ToString());

        // Third GET — cache hit
        value = await client.GetAsync(key);
        Assert.Equal(expectedValue, value.ToString());

        // Verify metrics: 1 miss + 2 hits = 3 total
        double hitRate = await client.GetCacheHitRateAsync();
        Assert.Equal(2.0 / 3.0, hitRate, precision: 3);

        double missRate = await client.GetCacheMissRateAsync();
        Assert.Equal(1.0 / 3.0, missRate, precision: 3);

        // Rates should sum to 1.0
        Assert.Equal(1.0, hitRate + missRate, precision: 3);

        // Total lookups should be 3
        long totalLookups = await client.GetCacheTotalLookupsAsync();
        Assert.Equal(3, totalLookups);
    }

    #endregion

    #region Without Metrics

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task WithoutMetrics_MetricCallsFail(bool clusterMode)
    {
        var cache = new ClientSideCacheConfig(1024, TimeSpan.FromMinutes(1))
            .WithMetrics(false);

        await using var client = await CreateClientWithCache(cache, clusterMode);

        string key = $"no_metrics_{Guid.NewGuid()}";

        // Cache should work
        await client.SetAsync(key, "value");
        var value = await client.GetAsync(key);
        Assert.Equal("value", value.ToString());

        // Second GET — from cache
        value = await client.GetAsync(key);
        Assert.Equal("value", value.ToString());

        // Metrics that require EnableMetrics should fail
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheHitRateAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheMissRateAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheEvictionsAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheExpirationsAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheTotalLookupsAsync());

        // Entry count should still work (doesn't require metrics)
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(1, entryCount);
    }

    #endregion

    #region No Cache Configured

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task NoCacheConfigured_AllMetricsFail(BaseClient client)
    {
        // Without cache configured, all metric calls should fail
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheHitRateAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheMissRateAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheEntryCountAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheEvictionsAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheExpirationsAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheTotalLookupsAsync());
    }

    #endregion

    #region Multiple Keys

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task MultipleKeys(bool clusterMode)
    {
        var cache = new ClientSideCacheConfig(1024, TimeSpan.FromMinutes(1))
            .WithMetrics(true);

        await using var client = await CreateClientWithCache(cache, clusterMode);

        string prefix = Guid.NewGuid().ToString();

        // Set 3 keys
        for (int i = 1; i <= 3; i++)
        {
            await client.SetAsync($"{prefix}_key{i}", $"value{i}");
        }

        // GET each key twice (miss + hit)
        for (int i = 1; i <= 3; i++)
        {
            // First GET — miss
            var value = await client.GetAsync($"{prefix}_key{i}");
            Assert.Equal($"value{i}", value.ToString());

            // Second GET — hit
            value = await client.GetAsync($"{prefix}_key{i}");
            Assert.Equal($"value{i}", value.ToString());
        }

        // Entry count should be 3
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(3, entryCount);

        // 3 misses + 3 hits = 50% hit rate
        double hitRate = await client.GetCacheHitRateAsync();
        Assert.Equal(0.5, hitRate, precision: 3);

        // Total lookups should be 6
        long totalLookups = await client.GetCacheTotalLookupsAsync();
        Assert.Equal(6, totalLookups);
    }

    #endregion

    #region TTL Expiration

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task TTLExpiration(bool clusterMode)
    {
        var cache = new ClientSideCacheConfig(1024, TimeSpan.FromSeconds(2))
            .WithMetrics(true);

        await using var client = await CreateClientWithCache(cache, clusterMode);

        string key = $"ttl_{Guid.NewGuid()}";

        await client.SetAsync(key, "ttl_value");

        // First GET — miss
        var value = await client.GetAsync(key);
        Assert.Equal("ttl_value", value.ToString());

        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(1, entryCount);

        // Second GET — from cache
        value = await client.GetAsync(key);
        Assert.Equal("ttl_value", value.ToString());

        // Wait for TTL to expire
        await Task.Delay(TimeSpan.FromSeconds(3));

        // GET after expiration — should fetch from server again
        value = await client.GetAsync(key);
        Assert.Equal("ttl_value", value.ToString());

        // Expiration count should be 1
        long expirations = await client.GetCacheExpirationsAsync();
        Assert.Equal(1, expirations);

        // Miss rate should be 2 misses out of 3 total = 66.67%
        double missRate = await client.GetCacheMissRateAsync();
        Assert.Equal(2.0 / 3.0, missRate, precision: 3);
    }

    #endregion

    #region Eviction Policy LRU

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task EvictionPolicyLRU(bool clusterMode)
    {
        // Each entry: ~42 byte key + 1000 byte value ≈ ~1 KB with overhead.
        // 4 KB cache fits 3 entries but not 5.
        var cache = new ClientSideCacheConfig(4, TimeSpan.FromMinutes(10))
            .WithEvictionPolicy(EvictionPolicy.LRU)
            .WithMetrics(true);

        await using var client = await CreateClientWithCache(cache, clusterMode);

        string prefix = Guid.NewGuid().ToString();
        string largeValue = new('x', 1000);

        // Set and cache 3 keys
        for (int i = 1; i <= 3; i++)
        {
            await client.SetAsync($"{prefix}_lru{i}", largeValue);
            var value = await client.GetAsync($"{prefix}_lru{i}");
            Assert.Equal(largeValue, value.ToString());
        }

        // Cache should have 3 entries
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(3, entryCount);

        // Access key1 to make it recently used
        var retrieved = await client.GetAsync($"{prefix}_lru1");
        Assert.Equal(largeValue, retrieved.ToString());

        // Add 2 more keys — should evict key2 and key3 (least recently used)
        for (int i = 4; i <= 5; i++)
        {
            await client.SetAsync($"{prefix}_lru{i}", largeValue);
            var value = await client.GetAsync($"{prefix}_lru{i}");
            Assert.Equal(largeValue, value.ToString());
        }

        // Verify 2 evictions occurred
        long evictions = await client.GetCacheEvictionsAsync();
        Assert.Equal(2, evictions);

        // Verify cache is working (hit rate > 0)
        double hitRate = await client.GetCacheHitRateAsync();
        Assert.True(hitRate > 0.0);

        // Check that key1 is still cached
        retrieved = await client.GetAsync($"{prefix}_lru1");
        Assert.Equal(largeValue, retrieved.ToString());
        double newHitRate = await client.GetCacheHitRateAsync();
        Assert.True(newHitRate > hitRate, "Key1 should still be in cache");

        // Check that key2 and key3 are evicted (miss rate should increase)
        double oldMissRate = await client.GetCacheMissRateAsync();
        _ = await client.GetAsync($"{prefix}_lru2");
        _ = await client.GetAsync($"{prefix}_lru3");
        double newMissRate = await client.GetCacheMissRateAsync();
        Assert.True(newMissRate > oldMissRate, "Key2 and Key3 should be evicted from cache");
    }

    #endregion

    #region Eviction Policy LFU

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task EvictionPolicyLFU(bool clusterMode)
    {
        // Each entry: ~42 byte key + 1000 byte value ≈ ~1 KB with overhead.
        // 4 KB cache fits 3 entries but not 4.
        var cache = new ClientSideCacheConfig(4, TimeSpan.FromMinutes(10))
            .WithEvictionPolicy(EvictionPolicy.LFU)
            .WithMetrics(true);

        await using var client = await CreateClientWithCache(cache, clusterMode);

        string prefix = Guid.NewGuid().ToString();
        string largeValue = new('x', 1000);

        // Set key1 and access it multiple times (high frequency)
        await client.SetAsync($"{prefix}_key1", largeValue);
        for (int i = 0; i < 5; i++)
        {
            var value = await client.GetAsync($"{prefix}_key1");
            Assert.Equal(largeValue, value.ToString());
        }
        // key1 frequency: 5

        // Set key2 with medium frequency
        await client.SetAsync($"{prefix}_key2", largeValue);
        for (int i = 0; i < 2; i++)
        {
            var value = await client.GetAsync($"{prefix}_key2");
            Assert.Equal(largeValue, value.ToString());
        }
        // key2 frequency: 2

        // Set key3 with low frequency
        await client.SetAsync($"{prefix}_key3", largeValue);
        var retrieved = await client.GetAsync($"{prefix}_key3");
        Assert.Equal(largeValue, retrieved.ToString());
        // key3 frequency: 1

        // Verify cache is working
        double hitRate = await client.GetCacheHitRateAsync();
        Assert.True(hitRate > 0.0, "Cache should have some hits");

        // Cache should have 3 entries
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(3, entryCount);

        // Set key4 — should trigger eviction of key3 (lowest frequency)
        await client.SetAsync($"{prefix}_key4", largeValue);
        retrieved = await client.GetAsync($"{prefix}_key4");
        Assert.Equal(largeValue, retrieved.ToString());
        // key4 frequency: 1

        // Check that cache entry count is still 3
        entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(3, entryCount);

        // Verify 1 eviction occurred
        long evictions = await client.GetCacheEvictionsAsync();
        Assert.Equal(1, evictions);

        // Check that key1 (highest frequency) is still cached
        double oldHitRate = await client.GetCacheHitRateAsync();
        retrieved = await client.GetAsync($"{prefix}_key1");
        Assert.Equal(largeValue, retrieved.ToString());
        double newHitRate = await client.GetCacheHitRateAsync();
        Assert.True(newHitRate > oldHitRate, "key1 (highest frequency) should still be cached");

        // Check that key3 (lowest frequency) was evicted
        double oldMissRate = await client.GetCacheMissRateAsync();
        retrieved = await client.GetAsync($"{prefix}_key3");
        Assert.Equal(largeValue, retrieved.ToString());
        double newMissRate = await client.GetCacheMissRateAsync();
        Assert.True(newMissRate > oldMissRate, "key3 (lowest frequency) should have been evicted");

        // key2 (medium frequency) should still be cached
        oldHitRate = await client.GetCacheHitRateAsync();
        retrieved = await client.GetAsync($"{prefix}_key2");
        Assert.Equal(largeValue, retrieved.ToString());
        newHitRate = await client.GetCacheHitRateAsync();
        Assert.True(newHitRate > oldHitRate, "key2 (medium frequency) should still be cached");
    }

    #endregion
}
