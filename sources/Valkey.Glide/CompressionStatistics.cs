// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

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
