// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Configuration for transparent compression feature.
/// Enables automatic compression of values before sending to the server
/// and decompression when receiving from the server.
/// </summary>
public sealed class CompressionConfig
{
    /// <summary>
    /// Minimum allowed value for <see cref="MinCompressionSize"/>.
    /// </summary>
    public const nuint MinCompressionSizeLimit = 16;

    /// <summary>
    /// Default minimum compression size in bytes.
    /// </summary>
    public const nuint DefaultMinCompressionSize = 64;

    /// <summary>
    /// Minimum value size in bytes to compress.
    /// Values smaller than this will not be compressed.
    /// </summary>
    public nuint MinCompressionSize { get; }

    /// <summary>
    /// Compression level for the backend.
    /// 0 means use backend default.
    /// </summary>
    public int CompressionLevel { get; }

    /// <summary>
    /// The compression backend to use.
    /// </summary>
    public CompressionBackend Backend { get; }

    /// <summary>
    /// Whether compression is enabled.
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// Creates a new compression configuration.
    /// </summary>
    /// <param name="backend">The compression backend to use.</param>
    /// <param name="compressionLevel">Optional compression level. If null, uses backend default.</param>
    /// <param name="minCompressionSize">Minimum value size to compress (default: 64 bytes).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if minCompressionSize is less than 16 bytes.</exception>
    public CompressionConfig(
        CompressionBackend backend,
        int? compressionLevel = null,
        nuint minCompressionSize = DefaultMinCompressionSize)
    {
        if (minCompressionSize < MinCompressionSizeLimit)
        {
            throw new ArgumentOutOfRangeException(nameof(minCompressionSize),
                $"minCompressionSize must be at least {MinCompressionSizeLimit} bytes");
        }

        MinCompressionSize = minCompressionSize;
        CompressionLevel = compressionLevel ?? 0;
        Backend = backend;
        Enabled = true;
    }

    /// <summary>
    /// Creates a compression configuration with Zstd backend.
    /// </summary>
    /// <param name="compressionLevel">Optional compression level (1-22).</param>
    /// <param name="minCompressionSize">Minimum value size to compress (default: 64 bytes).</param>
    /// <returns>A new CompressionConfig instance.</returns>
    public static CompressionConfig Zstd(int? compressionLevel = null, nuint minCompressionSize = DefaultMinCompressionSize) =>
        new(CompressionBackend.Zstd, compressionLevel, minCompressionSize);

    /// <summary>
    /// Creates a compression configuration with LZ4 backend.
    /// </summary>
    /// <param name="compressionLevel">Optional compression level (0-12).</param>
    /// <param name="minCompressionSize">Minimum value size to compress (default: 64 bytes).</param>
    /// <returns>A new CompressionConfig instance.</returns>
    public static CompressionConfig Lz4(int? compressionLevel = null, nuint minCompressionSize = DefaultMinCompressionSize) =>
        new(CompressionBackend.Lz4, compressionLevel, minCompressionSize);

    /// <summary>
    /// Converts to the FFI representation for marshalling to Rust core.
    /// </summary>
    internal Internals.FFI.CompressionConfig ToFfi() => new()
    {
        MinCompressionSize = MinCompressionSize,
        CompressionLevel = CompressionLevel,
        Backend = Backend,
        Enabled = Enabled,
    };
}
