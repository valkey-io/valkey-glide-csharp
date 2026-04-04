// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Tests;

public class CompressionConfigTests
{
    private const int DefaultCompressionLevel = 0;
    private const nuint DefaultMinCompressionSize = 64;
    private const int CustomCompressionLevel = 5;
    private const nuint CustomMinCompressionSize = 128;
    private const nuint InvalidMinCompressionSize = 10;
    private const nuint ValidMinCompressionSize = 100;
    private const ulong TestOriginalBytes = 1000;
    private const ulong TestCompressedBytes500 = 500;
    private const ulong TestCompressedBytes600 = 600;
    private const double ExpectedRatio2x = 2.0;
    private const double ExpectedRatioZero = 0.0;
    private const ulong ExpectedSpaceSaved400 = 400;
    private const double ExpectedSpaceSavedPercent40 = 40.0;

    [Fact]
    public void CompressionConfig_Zstd_CreatesValidConfig()
    {
        var config = CompressionConfig.Zstd();

        Assert.Equal(CompressionBackend.Zstd, config.Backend);
        Assert.Equal(DefaultCompressionLevel, config.CompressionLevel);
        Assert.Equal(DefaultMinCompressionSize, config.MinCompressionSize);
    }

    [Fact]
    public void CompressionConfig_Lz4_CreatesValidConfig()
    {
        var config = CompressionConfig.Lz4();

        Assert.Equal(CompressionBackend.Lz4, config.Backend);
        Assert.Equal(DefaultCompressionLevel, config.CompressionLevel);
        Assert.Equal(DefaultMinCompressionSize, config.MinCompressionSize);
    }

    [Fact]
    public void CompressionConfig_WithCustomLevel_SetsLevel()
    {
        var config = CompressionConfig.Zstd(compressionLevel: CustomCompressionLevel);

        Assert.Equal(CustomCompressionLevel, config.CompressionLevel);
    }

    [Fact]
    public void CompressionConfig_WithCustomMinSize_SetsMinSize()
    {
        var config = CompressionConfig.Lz4(minCompressionSize: CustomMinCompressionSize);

        Assert.Equal(CustomMinCompressionSize, config.MinCompressionSize);
    }

    [Fact]
    public void CompressionConfig_MinSizeTooSmall_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            CompressionConfig.Zstd(minCompressionSize: InvalidMinCompressionSize));

        Assert.Contains("minCompressionSize must be at least 16 bytes", exception.Message);
    }

    [Fact]
    public void CompressionConfig_Constructor_CreatesValidConfig()
    {
        var config = new CompressionConfig(CompressionBackend.Zstd, CustomCompressionLevel, ValidMinCompressionSize);

        Assert.Equal(CompressionBackend.Zstd, config.Backend);
        Assert.Equal(CustomCompressionLevel, config.CompressionLevel);
        Assert.Equal(ValidMinCompressionSize, config.MinCompressionSize);
    }

    [Fact]
    public void Statistics_CompressionRatio_CalculatesCorrectly()
    {
        var stats = new Statistics(0, 0, 0, 0, TestOriginalBytes, TestCompressedBytes500, 0, 0, 0, 0);
        Assert.Equal(ExpectedRatio2x, stats.CompressionRatio);
    }

    [Fact]
    public void Statistics_CompressionRatio_ZeroWhenNoData()
    {
        var stats = new Statistics(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        Assert.Equal(ExpectedRatioZero, stats.CompressionRatio);
    }

    [Fact]
    public void Statistics_SpaceSaved_CalculatesCorrectly()
    {
        var stats = new Statistics(0, 0, 0, 0, TestOriginalBytes, TestCompressedBytes600, 0, 0, 0, 0);
        Assert.Equal(ExpectedSpaceSaved400, stats.SpaceSaved);
    }

    [Fact]
    public void Statistics_SpaceSavedPercent_CalculatesCorrectly()
    {
        var stats = new Statistics(0, 0, 0, 0, TestOriginalBytes, TestCompressedBytes600, 0, 0, 0, 0);
        Assert.Equal(ExpectedSpaceSavedPercent40, stats.SpaceSavedPercent);
    }
}
