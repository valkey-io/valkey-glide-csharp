// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Tests;

public class CompressionConfigTests
{
    private const int CustomCompressionLevel = 5;
    private const nuint CustomMinCompressionSize = 128;
    private const nuint InvalidMinCompressionSize = 5;
    private const ulong CustomMaxDecompressedSize = 256 * 1024 * 1024; // 256 MB

    [Fact]
    public void CompressionConfig_Zstd_CreatesValidConfig()
    {
        var config = CompressionConfig.Zstd();

        Assert.Equal(CompressionBackend.Zstd, config.Backend);
        Assert.Null(config.CompressionLevel);
        Assert.Equal(CompressionConfig.DefaultMinCompressionSize, config.MinCompressionSize);
        Assert.Null(config.MaxDecompressedSize);
    }

    [Fact]
    public void CompressionConfig_Lz4_CreatesValidConfig()
    {
        var config = CompressionConfig.Lz4();

        Assert.Equal(CompressionBackend.Lz4, config.Backend);
        Assert.Null(config.CompressionLevel);
        Assert.Equal(CompressionConfig.DefaultMinCompressionSize, config.MinCompressionSize);
        Assert.Null(config.MaxDecompressedSize);
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
    public void CompressionConfig_WithMaxDecompressedSize_SetsMaxDecompressedSize()
    {
        var config = CompressionConfig.Zstd(maxDecompressedSize: CustomMaxDecompressedSize);

        Assert.Equal(CustomMaxDecompressedSize, config.MaxDecompressedSize);
    }

    [Fact]
    public void CompressionConfig_Lz4_WithMaxDecompressedSize_SetsMaxDecompressedSize()
    {
        var config = CompressionConfig.Lz4(maxDecompressedSize: CustomMaxDecompressedSize);

        Assert.Equal(CustomMaxDecompressedSize, config.MaxDecompressedSize);
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
        Assert.Null(config.MaxDecompressedSize);
    }

    [Fact]
    public void CompressionConfig_Constructor_WithAllOptions_CreatesValidConfig()
    {
        var config = new CompressionConfig(
            CompressionBackend.Zstd,
            CustomCompressionLevel,
            CustomMinCompressionSize,
            CustomMaxDecompressedSize);

        Assert.Equal(CompressionBackend.Zstd, config.Backend);
        Assert.Equal(CustomCompressionLevel, config.CompressionLevel);
        Assert.Equal(CustomMinCompressionSize, config.MinCompressionSize);
        Assert.Equal(CustomMaxDecompressedSize, config.MaxDecompressedSize);
    }

    [Fact]
    public void CompressionConfig_ToFfi_ConvertsCorrectly()
    {
        var config = new CompressionConfig(
            CompressionBackend.Zstd,
            CustomCompressionLevel,
            CustomMinCompressionSize,
            CustomMaxDecompressedSize);

        var ffi = config.ToFfi();

        Assert.Equal(CompressionBackend.Zstd, ffi.Backend);
        Assert.True(ffi.HasCompressionLevel);
        Assert.Equal(CustomCompressionLevel, ffi.CompressionLevel);
        Assert.Equal(CustomMinCompressionSize, ffi.MinCompressionSize);
        Assert.True(ffi.HasMaxDecompressedSize);
        Assert.Equal(CustomMaxDecompressedSize, ffi.MaxDecompressedSize);
        Assert.True(ffi.Enabled);
    }

    [Fact]
    public void CompressionConfig_ToFfi_WithoutOptionalFields_ConvertsCorrectly()
    {
        var config = CompressionConfig.Zstd();

        var ffi = config.ToFfi();

        Assert.Equal(CompressionBackend.Zstd, ffi.Backend);
        Assert.False(ffi.HasCompressionLevel);
        Assert.Equal(CompressionConfig.DefaultMinCompressionSize, ffi.MinCompressionSize);
        Assert.False(ffi.HasMaxDecompressedSize);
        Assert.True(ffi.Enabled);
    }
}
