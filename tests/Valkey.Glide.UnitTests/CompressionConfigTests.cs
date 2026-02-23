// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Xunit;

namespace Valkey.Glide.Tests;

public class CompressionConfigTests
{
    [Fact]
    public void CompressionConfig_Zstd_CreatesValidConfig()
    {
        var config = CompressionConfig.Zstd();

        Assert.True(config.Enabled);
        Assert.Equal(CompressionBackend.Zstd, config.Backend);
        Assert.False(config.HasCompressionLevel);
        Assert.Equal((nuint)64, config.MinCompressionSize);
    }

    [Fact]
    public void CompressionConfig_Lz4_CreatesValidConfig()
    {
        var config = CompressionConfig.Lz4();

        Assert.True(config.Enabled);
        Assert.Equal(CompressionBackend.Lz4, config.Backend);
        Assert.False(config.HasCompressionLevel);
        Assert.Equal((nuint)64, config.MinCompressionSize);
    }

    [Fact]
    public void CompressionConfig_WithCustomLevel_SetsLevel()
    {
        var config = CompressionConfig.Zstd(compressionLevel: 5);

        Assert.True(config.HasCompressionLevel);
        Assert.Equal(5, config.CompressionLevel);
    }

    [Fact]
    public void CompressionConfig_WithCustomMinSize_SetsMinSize()
    {
        var config = CompressionConfig.Lz4(minCompressionSize: 128);

        Assert.Equal((nuint)128, config.MinCompressionSize);
    }

    [Fact]
    public void CompressionConfig_MinSizeTooSmall_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            CompressionConfig.Zstd(minCompressionSize: 10));

        Assert.Contains("minCompressionSize must be at least 16 bytes", exception.Message);
    }

    [Fact]
    public void CompressionConfig_Constructor_CreatesValidConfig()
    {
        var config = new CompressionConfig(CompressionBackend.Zstd, 10, 100);

        Assert.True(config.Enabled);
        Assert.Equal(CompressionBackend.Zstd, config.Backend);
        Assert.True(config.HasCompressionLevel);
        Assert.Equal(10, config.CompressionLevel);
        Assert.Equal((nuint)100, config.MinCompressionSize);
    }

    [Fact]
    public void CompressionStatistics_CompressionRatio_CalculatesCorrectly()
    {
        var stats = new CompressionStatistics
        {
            TotalOriginalBytes = 1000,
            TotalBytesCompressed = 500
        };

        Assert.Equal(2.0, stats.CompressionRatio);
    }

    [Fact]
    public void CompressionStatistics_CompressionRatio_ZeroWhenNoData()
    {
        var stats = new CompressionStatistics
        {
            TotalOriginalBytes = 0,
            TotalBytesCompressed = 0
        };

        Assert.Equal(0.0, stats.CompressionRatio);
    }

    [Fact]
    public void CompressionStatistics_SpaceSaved_CalculatesCorrectly()
    {
        var stats = new CompressionStatistics
        {
            TotalOriginalBytes = 1000,
            TotalBytesCompressed = 600
        };

        Assert.Equal((ulong)400, stats.SpaceSaved);
    }

    [Fact]
    public void CompressionStatistics_SpaceSavedPercent_CalculatesCorrectly()
    {
        var stats = new CompressionStatistics
        {
            TotalOriginalBytes = 1000,
            TotalBytesCompressed = 600
        };

        Assert.Equal(40.0, stats.SpaceSavedPercent);
    }
}
