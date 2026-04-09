// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Tests;

public class CompressionConfigTests
{
    private const int CustomCompressionLevel = 5;
    private const nuint CustomMinCompressionSize = 128;
    private const nuint InvalidMinCompressionSize = 5;

    [Fact]
    public void CompressionConfig_Zstd_CreatesValidConfig()
    {
        var config = CompressionConfig.Zstd();

        Assert.Equal(CompressionBackend.Zstd, config.Backend);
        Assert.Null(config.CompressionLevel);
        Assert.Equal(CompressionConfig.DefaultMinCompressionSize, config.MinCompressionSize);
    }

    [Fact]
    public void CompressionConfig_Lz4_CreatesValidConfig()
    {
        var config = CompressionConfig.Lz4();

        Assert.Equal(CompressionBackend.Lz4, config.Backend);
        Assert.Null(config.CompressionLevel);
        Assert.Equal(CompressionConfig.DefaultMinCompressionSize, config.MinCompressionSize);
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
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            CompressionConfig.Zstd(minCompressionSize: InvalidMinCompressionSize));

        Assert.Contains("minCompressionSize must be at least 6 bytes", exception.Message);
    }

    [Fact]
    public void CompressionConfig_Constructor_CreatesValidConfig()
    {
        var config = new CompressionConfig(CompressionBackend.Zstd, CustomCompressionLevel, CustomMinCompressionSize);

        Assert.Equal(CompressionBackend.Zstd, config.Backend);
        Assert.Equal(CustomCompressionLevel, config.CompressionLevel);
        Assert.Equal(CustomMinCompressionSize, config.MinCompressionSize);
    }

}
