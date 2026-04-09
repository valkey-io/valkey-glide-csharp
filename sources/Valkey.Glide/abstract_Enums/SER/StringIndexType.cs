using System;

namespace Valkey.Glide;

/// <summary>
/// Indicates if we index into a string based on bits or bytes.
/// </summary>
public enum StringIndexType
{
    /// <summary>
    /// Indicates the index is the number of bytes into a string.
    /// </summary>
    Byte,

    /// <summary>
    /// Indicates the index is the number of bits into a string.
    /// </summary>
    Bit,
}

internal static class StringIndexTypeExtensions
{
    internal static ValkeyValue ToLiteral(this StringIndexType indexType) => indexType switch
    {
        StringIndexType.Bit => ValkeyLiterals.BIT,
        StringIndexType.Byte => ValkeyLiterals.BYTE,
        _ => throw new ArgumentOutOfRangeException(nameof(indexType)),
    };

    /// <summary>
    /// Converts a <see cref="StringIndexType"/> to <see cref="BitmapIndexType"/>.
    /// </summary>
    internal static BitmapIndexType ToBitmapIndexType(this StringIndexType indexType) => indexType switch
    {
        StringIndexType.Bit => BitmapIndexType.Bit,
        StringIndexType.Byte => BitmapIndexType.Byte,
        _ => throw new ArgumentOutOfRangeException(nameof(indexType)),
    };
}
