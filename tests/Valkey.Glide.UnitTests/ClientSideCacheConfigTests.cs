// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.UnitTests;

public class ClientSideCacheConfigTests
{
    [Fact]
    public void ClientSideCacheConfig_MaxCacheKbMustBePositive()
        => _ = Assert.Throws<ArgumentOutOfRangeException>(() => BuildConfig(maxCacheKb: 0));

    [Fact]
    public void ClientSideCacheConfig_EntryTtlMustNotBeNegative()
        => _ = Assert.Throws<ArgumentException>(() => BuildConfig(entryTtl: TimeSpan.FromTicks(-1)));

    [Fact]
    public void ClientSideCacheConfig_EntryTtlZeroIsAllowed()
    {
        var config = BuildConfig(entryTtl: TimeSpan.Zero);
        Assert.Equal(TimeSpan.Zero, config.EntryTtl);
    }

    [Fact]
    public void ClientSideCacheConfig_FluentBuilderChaining()
    {
        var config = BuildConfig()
            .WithEvictionPolicy(EvictionPolicy.LFU)
            .WithMetrics(true);

        Assert.Equal(EvictionPolicy.LFU, config.EvictionPolicy);
        Assert.True(config.EnableMetrics);
    }

    [Fact]
    public void ClientSideCacheConfig_UniqueCacheIds()
    {
        var config1 = BuildConfig(1024, TimeSpan.FromMinutes(1));
        var config2 = BuildConfig(1024, TimeSpan.FromMinutes(1));

        Assert.NotEqual(config1.CacheId, config2.CacheId);
    }

    [Fact]
    public void ClientSideCacheConfig_DefaultValues()
    {
        var config = BuildConfig(512u, TimeSpan.FromSeconds(30));

        Assert.Equal(512u, config.MaxCacheKb);
        Assert.Equal(TimeSpan.FromSeconds(30), config.EntryTtl);

        Assert.Null(config.EvictionPolicy);
        Assert.False(config.EnableMetrics);
        Assert.False(config.ServerAssisted);
    }

    [Fact]
    public void ClientSideCacheConfig_WithServerAssisted()
    {
        ClientSideCacheConfig config;

        config = BuildConfig().WithServerAssisted();
        Assert.True(config.ServerAssisted);

        config = BuildConfig().WithServerAssisted(true);
        Assert.True(config.ServerAssisted);

        config = BuildConfig().WithServerAssisted(false);
        Assert.False(config.ServerAssisted);
    }

    #region Helpers

    /// <summary>
    /// Builds and returns a client-side cache config for testing.
    /// If not specified, required arguments will be populated with default values.
    /// </summary>
    private static ClientSideCacheConfig BuildConfig(ulong? maxCacheKb = null, TimeSpan? entryTtl = null)
        => new(maxCacheKb ?? 1024, entryTtl ?? TimeSpan.FromTicks(1));

    #endregion
}
