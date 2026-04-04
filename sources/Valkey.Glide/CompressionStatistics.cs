// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Statistics about compression operations.
/// </summary>
/// <param name="TotalValuesCompressed">Total number of values that were compressed.</param>
/// <param name="TotalOriginalBytes">Total size in bytes of original (uncompressed) data.</param>
/// <param name="TotalBytesCompressed">Total size in bytes after compression.</param>
/// <param name="CompressionSkippedCount">Number of times compression was skipped (e.g., value too small).</param>
public sealed record CompressionStatistics(
    ulong TotalValuesCompressed,
    ulong TotalOriginalBytes,
    ulong TotalBytesCompressed,
    ulong CompressionSkippedCount)
{
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
