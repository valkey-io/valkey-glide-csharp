// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Specifies whether start/end arguments are BIT indices or BYTE indices.
/// Used by BITCOUNT and BITPOS commands in the Valkey GLIDE API.
/// </summary>
/// <remarks>
/// This enum is used by the Valkey GLIDE native API methods (e.g., <c>BitCountAsync</c>, <c>BitPosAsync</c>).
/// For StackExchange.Redis compatibility, use <see cref="StringIndexType"/> instead.
/// </remarks>
/// <seealso cref="StringIndexType"/>
public enum BitmapIndexType
{
    /// <summary>
    /// Specifies a byte index. This is the default behavior.
    /// </summary>
    Byte,

    /// <summary>
    /// Specifies a bit index.
    /// </summary>
    Bit,
}

internal static class BitmapIndexTypeExtensions
{
    /// <summary>
    /// Converts the <see cref="BitmapIndexType"/> to its wire format literal.
    /// </summary>
    internal static ValkeyValue ToLiteral(this BitmapIndexType indexType) => indexType switch
    {
        BitmapIndexType.Bit => ValkeyLiterals.BIT,
        BitmapIndexType.Byte => ValkeyLiterals.BYTE,
        _ => throw new ArgumentOutOfRangeException(nameof(indexType)),
    };
}
