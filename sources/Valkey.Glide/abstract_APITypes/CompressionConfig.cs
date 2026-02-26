// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Runtime.InteropServices;

namespace Valkey.Glide;

/// <summary>
/// Configuration for transparent compression feature.
/// Enables automatic compression of values before sending to the server
/// and decompression when receiving from the server.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CompressionConfig
{
    /// <summary>
    /// Minimum allowed value for minCompressionSize.
    /// </summary>
    public const nuint MinCompressionSizeLimit = 16;

    /// <summary>
    /// Minimum value size in bytes to compress.
    /// Values smaller than this will not be compressed.
    /// Default: 64 bytes
    /// Minimum: 16 bytes
    /// </summary>
    public nuint MinCompressionSize;

    /// <summary>
    /// Compression level for the backend.
    /// 0 means use backend default.
    /// - Zstd: 1-22 (default: 3)
    /// - LZ4: 0-12 (default: 0)
    /// </summary>
    public int CompressionLevel;

    /// <summary>
    /// The compression backend to use.
    /// </summary>
    public CompressionBackend Backend;

    /// <summary>
    /// Whether compression is enabled.
    /// </summary>
    [MarshalAs(UnmanagedType.U1)]
    public bool Enabled;

    /// <summary>
    /// Creates a new compression configuration.
    /// </summary>
    /// <param name="backend">The compression backend to use.</param>
    /// <param name="compressionLevel">Optional compression level. If null, uses backend default.</param>
    /// <param name="minCompressionSize">Minimum value size to compress (default: 64 bytes).</param>
    /// <exception cref="ArgumentException">Thrown if minCompressionSize is less than 16 bytes.</exception>
    public CompressionConfig(
        CompressionBackend backend,
        int? compressionLevel = null,
        nuint minCompressionSize = 64)
    {
        if (minCompressionSize < MinCompressionSizeLimit)
        {
            throw new ArgumentException($"minCompressionSize must be at least {MinCompressionSizeLimit} bytes", nameof(minCompressionSize));
        }

        MinCompressionSize = minCompressionSize;
        CompressionLevel = compressionLevel ?? 0;
        Backend = backend;
        Enabled = true;
    }

    /// <summary>
    /// Creates a compression configuration with Zstd backend.
    /// </summary>
    /// <param name="compressionLevel">Compression level (1-22, default: 3).</param>
    /// <param name="minCompressionSize">Minimum value size to compress (default: 64 bytes).</param>
    /// <returns>A new CompressionConfig instance.</returns>
    public static CompressionConfig Zstd(int? compressionLevel = null, nuint minCompressionSize = 64) =>
        new(CompressionBackend.Zstd, compressionLevel, minCompressionSize);

    /// <summary>
    /// Creates a compression configuration with LZ4 backend.
    /// </summary>
    /// <param name="compressionLevel">Compression level (0-12, default: 0).</param>
    /// <param name="minCompressionSize">Minimum value size to compress (default: 64 bytes).</param>
    /// <returns>A new CompressionConfig instance.</returns>
    public static CompressionConfig Lz4(int? compressionLevel = null, nuint minCompressionSize = 64) =>
        new(CompressionBackend.Lz4, compressionLevel, minCompressionSize);
}

/// <summary>
/// Statistics about compression operations.
/// </summary>
public sealed class CompressionStatistics
{
    /// <summary>
    /// Total number of values that were compressed.
    /// </summary>
    public ulong TotalValuesCompressed { get; init; }

    /// <summary>
    /// Total size in bytes of original (uncompressed) data.
    /// </summary>
    public ulong TotalOriginalBytes { get; init; }

    /// <summary>
    /// Total size in bytes after compression.
    /// </summary>
    public ulong TotalBytesCompressed { get; init; }

    /// <summary>
    /// Number of times compression was skipped (e.g., value too small).
    /// </summary>
    public ulong CompressionSkippedCount { get; init; }

    /// <summary>
    /// Compression ratio (original size / compressed size).
    /// Returns 0 if no data has been compressed.
    /// </summary>
    public double CompressionRatio =>
        TotalBytesCompressed > 0 ? (double)TotalOriginalBytes / TotalBytesCompressed : 0.0;

    /// <summary>
    /// Space saved in bytes (original size - compressed size).
    /// </summary>
    public ulong SpaceSaved =>
        TotalOriginalBytes > TotalBytesCompressed ? TotalOriginalBytes - TotalBytesCompressed : 0;

    /// <summary>
    /// Space saved as a percentage.
    /// </summary>
    public double SpaceSavedPercent =>
        TotalOriginalBytes > 0 ? (double)SpaceSaved / TotalOriginalBytes * 100.0 : 0.0;
}
