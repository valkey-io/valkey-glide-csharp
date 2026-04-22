// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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

    #endregion

    #region Basic Cache Hit With Metrics

    [Fact]
    public async Task BasicCacheHitWithMetrics_Standalone()
    {
        var cache = new ClientSideCacheConfig(1024)
            .WithEntryTtlMs(60000)
            .WithMetrics(true);

        await using var client = await CreateStandaloneClientWithCache(cache);

        // SET a key
        await client.SetAsync("cache_test_key", "cache_test_value");

        // First GET — cache miss
        var value = await client.GetAsync("cache_test_key");
        Assert.Equal("cache_test_value", value.ToString());

        // Entry count should be 1
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(1, entryCount);

        // Second GET — cache hit
        value = await client.GetAsync("cache_test_key");
        Assert.Equal("cache_test_value", value.ToString());

        // Third GET — cache hit
        value = await client.GetAsync("cache_test_key");
        Assert.Equal("cache_test_value", value.ToString());

        // Verify metrics: 1 miss + 2 hits = 3 total
        double hitRate = await client.GetCacheHitRateAsync();
        Assert.InRange(hitRate, 2.0 / 3.0 - 0.001, 2.0 / 3.0 + 0.001);

        double missRate = await client.GetCacheMissRateAsync();
        Assert.InRange(missRate, 1.0 / 3.0 - 0.001, 1.0 / 3.0 + 0.001);

        // Rates should sum to 1.0
        Assert.InRange(hitRate + missRate, 0.9999, 1.0001);
    }

    [Fact]
    public async Task BasicCacheHitWithMetrics_Cluster()
    {
        var cache = new ClientSideCacheConfig(1024)
            .WithEntryTtlMs(60000)
            .WithMetrics(true);

        await using var client = await CreateClusterClientWithCache(cache);

        await client.SetAsync("cache_test_key", "cache_test_value");

        // First GET — cache miss
        var value = await client.GetAsync("cache_test_key");
        Assert.Equal("cache_test_value", value.ToString());

        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(1, entryCount);

        // Second GET — cache hit
        value = await client.GetAsync("cache_test_key");
        Assert.Equal("cache_test_value", value.ToString());

        // Third GET — cache hit
        value = await client.GetAsync("cache_test_key");
        Assert.Equal("cache_test_value", value.ToString());

        double hitRate = await client.GetCacheHitRateAsync();
        Assert.InRange(hitRate, 2.0 / 3.0 - 0.001, 2.0 / 3.0 + 0.001);

        double missRate = await client.GetCacheMissRateAsync();
        Assert.InRange(missRate, 1.0 / 3.0 - 0.001, 1.0 / 3.0 + 0.001);

        Assert.InRange(hitRate + missRate, 0.9999, 1.0001);
    }

    #endregion

    #region Without Metrics

    [Fact]
    public async Task WithoutMetrics_MetricCallsFail_Standalone()
    {
        var cache = new ClientSideCacheConfig(1024)
            .WithEntryTtlMs(60000)
            .WithMetrics(false);

        await using var client = await CreateStandaloneClientWithCache(cache);

        // Cache should work
        await client.SetAsync("key", "value");
        var value = await client.GetAsync("key");
        Assert.Equal("value", value.ToString());

        // Second GET — from cache
        value = await client.GetAsync("key");
        Assert.Equal("value", value.ToString());

        // Metrics that require EnableMetrics should fail
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheHitRateAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheMissRateAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheEvictionsAsync());
        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => client.GetCacheExpirationsAsync());

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

    [Fact]
    public async Task MultipleKeys_Standalone()
    {
        var cache = new ClientSideCacheConfig(1024)
            .WithEntryTtlMs(60000)
            .WithMetrics(true);

        await using var client = await CreateStandaloneClientWithCache(cache);

        // Set 3 keys
        for (int i = 1; i <= 3; i++)
        {
            await client.SetAsync($"key{i}", $"value{i}");
        }

        // GET each key twice (miss + hit)
        for (int i = 1; i <= 3; i++)
        {
            // First GET — miss
            var value = await client.GetAsync($"key{i}");
            Assert.Equal($"value{i}", value.ToString());

            // Second GET — hit
            value = await client.GetAsync($"key{i}");
            Assert.Equal($"value{i}", value.ToString());
        }

        // Entry count should be 3
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(3, entryCount);

        // 3 misses + 3 hits = 50% hit rate
        double hitRate = await client.GetCacheHitRateAsync();
        Assert.Equal(0.5, hitRate, precision: 3);
    }

    #endregion

    #region TTL Expiration

    [Fact]
    public async Task TTLExpiration_Standalone()
    {
        var cache = new ClientSideCacheConfig(1024)
            .WithEntryTtlMs(2000) // 2 seconds
            .WithMetrics(true);

        await using var client = await CreateStandaloneClientWithCache(cache);

        await client.SetAsync("ttl_key", "ttl_value");

        // First GET — miss
        var value = await client.GetAsync("ttl_key");
        Assert.Equal("ttl_value", value.ToString());

        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(1, entryCount);

        // Second GET — from cache
        value = await client.GetAsync("ttl_key");
        Assert.Equal("ttl_value", value.ToString());

        // Wait for TTL to expire
        await Task.Delay(TimeSpan.FromSeconds(3));

        // GET after expiration — should fetch from server again
        value = await client.GetAsync("ttl_key");
        Assert.Equal("ttl_value", value.ToString());

        // Expiration count should be 1
        long expirations = await client.GetCacheExpirationsAsync();
        Assert.Equal(1, expirations);

        // Miss rate should be 2 misses out of 3 total = 66.67%
        double missRate = await client.GetCacheMissRateAsync();
        Assert.InRange(missRate, 2.0 / 3.0 - 0.001, 2.0 / 3.0 + 0.001);
    }

    #endregion

    #region Eviction Policy LRU

    [Fact]
    public async Task EvictionPolicyLRU_Standalone()
    {
        var cache = new ClientSideCacheConfig(1) // 1 KB to force eviction
            .WithEvictionPolicy(EvictionPolicy.LRU)
            .WithMetrics(true);

        await using var client = await CreateStandaloneClientWithCache(cache);

        string largeValue = new('x', 250); // ~250 bytes

        // Set and cache 3 keys
        for (int i = 1; i <= 3; i++)
        {
            await client.SetAsync($"lru_key{i}", largeValue);
            var value = await client.GetAsync($"lru_key{i}");
            Assert.Equal(largeValue, value.ToString());
        }

        // Cache should have 3 entries
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(3, entryCount);

        // Access key1 to make it recently used
        var retrieved = await client.GetAsync("lru_key1");
        Assert.Equal(largeValue, retrieved.ToString());

        // Add 2 more keys — should evict least recently used
        for (int i = 4; i <= 5; i++)
        {
            await client.SetAsync($"lru_key{i}", largeValue);
            var value = await client.GetAsync($"lru_key{i}");
            Assert.Equal(largeValue, value.ToString());
        }

        // Verify evictions occurred
        long evictions = await client.GetCacheEvictionsAsync();
        Assert.Equal(2, evictions);

        // Hit rate should be > 0
        double hitRate = await client.GetCacheHitRateAsync();
        Assert.True(hitRate > 0.0);
    }

    #endregion

    #region Eviction Policy LFU

    [Fact]
    public async Task EvictionPolicyLFU_Standalone()
    {
        var cache = new ClientSideCacheConfig(1) // 1 KB to force eviction
            .WithEvictionPolicy(EvictionPolicy.LFU)
            .WithMetrics(true);

        await using var client = await CreateStandaloneClientWithCache(cache);

        string largeValue = new('x', 250);

        // Set key1 and access it multiple times (high frequency)
        await client.SetAsync("key1", largeValue);
        for (int i = 0; i < 5; i++)
        {
            var value = await client.GetAsync("key1");
            Assert.Equal(largeValue, value.ToString());
        }

        // Set key2 with medium frequency
        await client.SetAsync("key2", largeValue);
        for (int i = 0; i < 2; i++)
        {
            var value = await client.GetAsync("key2");
            Assert.Equal(largeValue, value.ToString());
        }

        // Set key3 with low frequency
        await client.SetAsync("key3", largeValue);
        var retrieved = await client.GetAsync("key3");
        Assert.Equal(largeValue, retrieved.ToString());

        // Cache should have 3 entries
        long entryCount = await client.GetCacheEntryCountAsync();
        Assert.Equal(3, entryCount);

        // Set key4 — should trigger eviction of key3 (lowest frequency)
        await client.SetAsync("key4", largeValue);
        retrieved = await client.GetAsync("key4");
        Assert.Equal(largeValue, retrieved.ToString());

        // Verify 1 eviction occurred
        long evictions = await client.GetCacheEvictionsAsync();
        Assert.Equal(1, evictions);

        // key1 (highest frequency) should still be cached
        double oldHitRate = await client.GetCacheHitRateAsync();
        retrieved = await client.GetAsync("key1");
        Assert.Equal(largeValue, retrieved.ToString());
        double newHitRate = await client.GetCacheHitRateAsync();
        Assert.True(newHitRate > oldHitRate);
    }

    #endregion

    #region Config Validation

    [Fact]
    public void ClientSideCacheConfig_MaxCacheKbMustBePositive()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientSideCacheConfig(0));
    }

    [Fact]
    public void ClientSideCacheConfig_EntryTtlMsMustBePositive()
    {
        var config = new ClientSideCacheConfig(1024);
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => config.WithEntryTtlMs(0));
    }

    [Fact]
    public void ClientSideCacheConfig_FluentBuilderChaining()
    {
        var config = new ClientSideCacheConfig(2048)
            .WithEntryTtlMs(120000)
            .WithEvictionPolicy(EvictionPolicy.LFU)
            .WithMetrics(true);

        Assert.Equal(2048UL, config.MaxCacheKb);
        Assert.Equal(120000UL, config.EntryTtlMs);
        Assert.Equal(EvictionPolicy.LFU, config.EvictionPolicy);
        Assert.True(config.EnableMetrics);
        Assert.NotNull(config.CacheId);
        Assert.NotEmpty(config.CacheId);
    }

    [Fact]
    public void ClientSideCacheConfig_UniqueCacheIds()
    {
        var config1 = new ClientSideCacheConfig(1024);
        var config2 = new ClientSideCacheConfig(1024);

        Assert.NotEqual(config1.CacheId, config2.CacheId);
    }

    [Fact]
    public void ClientSideCacheConfig_DefaultValues()
    {
        var config = new ClientSideCacheConfig(512);

        Assert.Equal(512UL, config.MaxCacheKb);
        Assert.Equal(0UL, config.EntryTtlMs);
        Assert.Null(config.EvictionPolicy);
        Assert.False(config.EnableMetrics);
    }

    #endregion
}
