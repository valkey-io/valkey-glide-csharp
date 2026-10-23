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
        StringIndexType.Byte => ValkeyLiterals.BYTE,
        StringIndexType.Bit => ValkeyLiterals.BIT,
        _ => throw new ArgumentOutOfRangeException(nameof(indexType)),
    };

    /// <summary>
    /// Converts a <see cref="StringIndexType"/> to <see cref="BitmapIndexType"/>.
    /// </summary>
    /// <param name="indexType">The index type to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="indexType"/> is not a supported <see cref="StringIndexType"/> value.</exception>
    internal static BitmapIndexType ToBitmapIndexType(this StringIndexType indexType) => indexType switch
    {
        StringIndexType.Byte => BitmapIndexType.Byte,
        StringIndexType.Bit => BitmapIndexType.Bit,
        _ => throw new ArgumentOutOfRangeException(nameof(indexType)),
    };
}
