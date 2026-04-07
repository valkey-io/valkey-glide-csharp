// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for specifying bit/byte offset ranges in bitmap commands like BITCOUNT and BITPOS.
/// </summary>
/// <remarks>
/// <para>
/// This class provides a cleaner API for specifying start, end, and index type parameters
/// for bitmap commands, consistent with Java/Python Valkey GLIDE clients.
/// </para>
/// <example>
/// <code>
/// // Count bits in first 8 bits
/// var options = new BitOffsetOptions { Start = 0, End = 7, IndexType = BitmapIndexType.Bit };
/// long count = await client.BitCountAsync("mykey", options);
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
}
