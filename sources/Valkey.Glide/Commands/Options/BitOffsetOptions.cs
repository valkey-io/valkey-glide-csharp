// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for specifying bit/byte offset ranges in bitmap commands like BITCOUNT and BITPOS.
/// </summary>
/// <remarks>
/// <example>
/// <code>
/// // Using factory methods (recommended)
/// var bitRange = BitOffsetOptions.InBitRange(0, 7);
/// var fromStart = BitOffsetOptions.FromStartByte(2);
///
/// // Using object initializer
/// var options = new BitOffsetOptions { Start = 0, End = 7, IndexType = BitmapIndexType.Bit };
/// </code>
/// </example>
/// </remarks>
public class BitOffsetOptions
{
    /// <summary>
    /// The starting offset. Default is 0.
    /// </summary>
    /// <remarks>
    /// Negative values indicate offsets from the end of the string.
    /// </remarks>
    public long Start { get; init; } = 0;

    /// <summary>
    /// The ending offset. Default is -1 (end of string).
    /// </summary>
    /// <remarks>
    /// Negative values indicate offsets from the end of the string.
    /// -1 means the last byte/bit.
    /// </remarks>
    public long End { get; init; } = -1;

    /// <summary>
    /// The index type specifying whether offsets are bit or byte indices.
    /// Default is <see cref="BitmapIndexType.Byte"/>.
    /// </summary>
    public BitmapIndexType IndexType { get; init; } = BitmapIndexType.Byte;

    /// <summary>
    /// Creates options starting from the specified bit offset to the end of the string.
    /// </summary>
    /// <param name="start">The starting bit offset.</param>
    /// <returns>A new <see cref="BitOffsetOptions"/> instance.</returns>
    public static BitOffsetOptions FromStartBit(long start) =>
        new() { Start = start, IndexType = BitmapIndexType.Bit };

    /// <summary>
    /// Creates options starting from the specified byte offset to the end of the string.
    /// </summary>
    /// <param name="start">The starting byte offset.</param>
    /// <returns>A new <see cref="BitOffsetOptions"/> instance.</returns>
    public static BitOffsetOptions FromStartByte(long start) =>
        new() { Start = start, IndexType = BitmapIndexType.Byte };

    /// <summary>
    /// Creates options from the beginning to the specified bit offset.
    /// </summary>
    /// <param name="end">The ending bit offset.</param>
    /// <returns>A new <see cref="BitOffsetOptions"/> instance.</returns>
    public static BitOffsetOptions ToEndBit(long end) =>
        new() { End = end, IndexType = BitmapIndexType.Bit };

    /// <summary>
    /// Creates options from the beginning to the specified byte offset.
    /// </summary>
    /// <param name="end">The ending byte offset.</param>
    /// <returns>A new <see cref="BitOffsetOptions"/> instance.</returns>
    public static BitOffsetOptions ToEndByte(long end) =>
        new() { End = end, IndexType = BitmapIndexType.Byte };

    /// <summary>
    /// Creates options for a bit range.
    /// </summary>
    /// <param name="start">The starting bit offset.</param>
    /// <param name="end">The ending bit offset.</param>
    /// <returns>A new <see cref="BitOffsetOptions"/> instance.</returns>
    public static BitOffsetOptions InBitRange(long start, long end) =>
        new() { Start = start, End = end, IndexType = BitmapIndexType.Bit };

    /// <summary>
    /// Creates options for a byte range.
    /// </summary>
    /// <param name="start">The starting byte offset.</param>
    /// <param name="end">The ending byte offset.</param>
    /// <returns>A new <see cref="BitOffsetOptions"/> instance.</returns>
    public static BitOffsetOptions InByteRange(long start, long end) =>
        new() { Start = start, End = end, IndexType = BitmapIndexType.Byte };
}
