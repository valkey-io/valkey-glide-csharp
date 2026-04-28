// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Tests;

public class ClientSideCacheConfigTests
{
    [Fact]
    public void ClientSideCacheConfig_MaxCacheKbMustBePositive()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientSideCacheConfig(0, TimeSpan.FromMinutes(1)));
    }

    [Fact]
    public void ClientSideCacheConfig_EntryTtlMustNotBeNegative()
    {
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => new ClientSideCacheConfig(1024, TimeSpan.FromMilliseconds(-1)));
    }

    [Fact]
    public void ClientSideCacheConfig_EntryTtlZeroIsAllowed()
    {
        var config = new ClientSideCacheConfig(1024, TimeSpan.Zero);

        Assert.Equal(TimeSpan.Zero, config.EntryTtl);
    }

    [Fact]
    public void ClientSideCacheConfig_FluentBuilderChaining()
    {
        var config = new ClientSideCacheConfig(2048, TimeSpan.FromMinutes(2))
            .WithEvictionPolicy(EvictionPolicy.LFU)
            .WithMetrics(true);

        Assert.Equal(2048UL, config.MaxCacheKb);
        Assert.Equal(TimeSpan.FromMinutes(2), config.EntryTtl);
        Assert.Equal(EvictionPolicy.LFU, config.EvictionPolicy);
        Assert.True(config.EnableMetrics);
    }

    [Fact]
    public void ClientSideCacheConfig_UniqueCacheIds()
    {
        var config1 = new ClientSideCacheConfig(1024, TimeSpan.FromMinutes(1));
        var config2 = new ClientSideCacheConfig(1024, TimeSpan.FromMinutes(1));

        Assert.NotEqual(config1.CacheId, config2.CacheId);
    }

    [Fact]
    public void ClientSideCacheConfig_DefaultValues()
    {
        var config = new ClientSideCacheConfig(512, TimeSpan.FromSeconds(30));

        Assert.Equal(512UL, config.MaxCacheKb);
        Assert.Equal(TimeSpan.FromSeconds(30), config.EntryTtl);
        Assert.Equal(EvictionPolicy.LRU, config.EvictionPolicy);
        Assert.False(config.EnableMetrics);
    }
}
