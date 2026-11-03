// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Options.InfoOptions;

namespace Valkey.Glide;

/// <summary>
/// Extensions for working with <see cref="GlideString" />.
/// </summary>
public static class GlideStringExtensions
{
    /// <summary>
    /// Convert a <paramref name="string"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="string">A <see langword="string" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this string @string) => new(@string);

    /// <summary>
    /// Convert a <paramref name="int"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="int">A <see langword="int" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this int @int) => new(@int.ToString());

    /// <summary>
    /// Convert a <paramref name="ushort"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="ushort">A <see langword="ushort" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this ushort @ushort) => new(@ushort.ToString());

    /// <summary>
    /// Convert a <paramref name="long"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="long">A <see langword="long" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this long @long) => new(@long.ToString());

    /// <summary>
    /// Convert a <paramref name="ulong"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="ulong">A <see langword="ulong" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this ulong @ulong) => new(@ulong.ToString());

    /// <summary>
    /// Convert a <paramref name="double"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="double">A <see langword="double" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this double @double)
        => double.IsPositiveInfinity(@double) ? new("+inf")
            : double.IsNegativeInfinity(@double) ? new("-inf")
            : double.IsNaN(@double) ? new("nan")
            : new(@double.ToString("G17", System.Globalization.CultureInfo.InvariantCulture));

    /// <summary>
    /// Convert a <paramref name="bool"/> to a <see cref="GlideString" /> ("1" for true, "0" for false).
    /// </summary>
    /// <param name="bool">A <see langword="bool" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this bool @bool) => new(@bool ? "1" : "0");

    /// <summary>
    /// Convert a <paramref name="bytes"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="bytes">A <see langword="byte[]" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this byte[] bytes) => new(bytes);

    /// <summary>
    /// Convert a <paramref name="key"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="key">A <see langword="ValkeyKey" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this ValkeyKey key) => (GlideString)key;

    /// <summary>
    /// Convert a <paramref name="value"/> to a <see cref="GlideString" />.
    /// </summary>
    /// <param name="value">A <see langword="ValkeyValue" /> to convert.</param>
    /// <returns>A <see cref="GlideString" />.</returns>
    public static GlideString ToGlideString(this ValkeyValue value) => (GlideString)value;

    /// <summary>
    /// Convert an <see langword="GlideString[]" /> to an <see langword="string[]" />.<br />
    /// <b>Note:</b> a resulting <see langword="string" /> may be incorrect if original <see cref="GlideString" />
    /// stores a non-UTF8 compatible sequence of bytes.
    /// </summary>
    /// <param name="strings">An array of <see cref="GlideString" />s to convert.</param>
    /// <returns>An array of <see langword="string" />s.</returns>
    public static string[] ToStrings(this GlideString[]? strings) => strings is null ? [] : [.. strings.Select(s => s.ToString())];

    /// <summary>
    /// Convert an <see langword="GlideString[]" /> to an array of <see langword="byte[]" />.<br />
    /// </summary>
    /// <param name="strings">An array of <see cref="GlideString" />s to convert.</param>
    /// <returns>An array of <see langword="byte[]" />.</returns>
    public static byte[][] ToByteArrays(this GlideString[]? strings) => strings is null ? [] : [.. strings.Select(s => s.Bytes)];

    #region ToGlideStrings

    /// <summary>
    /// Convert a <see langword="byte[][]"/> to a <see langword="GlideString[]"/>.
    /// </summary>
    /// <param name="strings">A <see langword="byte[][]"/> to convert.</param>
    /// <returns>A <see cref="GlideString"/> array.</returns>
    public static GlideString[] ToGlideStrings(this byte[][]? strings)
        => strings is null ? [] : [.. strings.Select(s => new GlideString(s))];

    /// <summary>
    /// Convert an <see cref="IEnumerable{Double}"/> to a <see langword="GlideString[]"/>.
    /// </summary>
    /// <param name="values">A <see langword="double"/> collection to convert.</param>
    /// <returns>A <see cref="GlideString"/> array.</returns>
    public static GlideString[] ToGlideStrings(this IEnumerable<double> values)
        => [.. values.Select(v => v.ToGlideString())];

    /// <summary>
    /// Convert an <see cref="IEnumerable{String}"/> to a <see langword="GlideString[]"/>.
    /// </summary>
    /// <param name="strings">A <see langword="string"/> collection to convert.</param>
    /// <returns>A <see cref="GlideString"/> array.</returns>
    public static GlideString[] ToGlideStrings(this IEnumerable<string> strings)
        => [.. strings.Select(s => new GlideString(s))];

    /// <summary>
    /// Convert an <see cref="IEnumerable{ValkeyKey}"/> to a <see langword="GlideString[]"/>.
    /// </summary>
    /// <param name="keys">A <see cref="ValkeyKey"/> collection to convert.</param>
    /// <returns>A <see cref="GlideString"/> array.</returns>
    public static GlideString[] ToGlideStrings(this IEnumerable<ValkeyKey> keys)
        => [.. keys.Select(k => (GlideString)k)];

    /// <summary>
    /// Convert an <see cref="IEnumerable{ValkeyValue}"/> to a <see langword="GlideString[]"/>.
    /// </summary>
    /// <param name="values">A <see cref="ValkeyValue"/> collection to convert.</param>
    /// <returns>A <see cref="GlideString"/> array.</returns>
    public static GlideString[] ToGlideStrings(this IEnumerable<ValkeyValue> values)
        => [.. values.Select(v => v.ToGlideString())];

    /// <summary>
    /// Convert a <see cref="Section"/>[] to a <see langword="GlideString[]"/>.
    /// </summary>
    /// <param name="sections">A <see cref="Section"/>[] to convert.</param>
    /// <returns>A <see cref="GlideString"/> array.</returns>
    public static GlideString[] ToGlideStrings(this Section[]? sections)
        => sections is null ? [] : [.. sections.Select(s => new GlideString(s.ToString()))];

    /// <summary>
    /// Convert a <see langword="KeyValuePair{ValkeyKey, ValkeyValue}"/> collection to an interleaved <see langword="GlideString[]" /> array.
    /// </summary>
    /// <param name="pairs">An <see langword="KeyValuePair{ValkeyKey, ValkeyValue}"/> array to convert.</param>
    /// <returns>A <see cref="GlideString"/> array.</returns>
    public static GlideString[] ToGlideStrings(this KeyValuePair<ValkeyKey, ValkeyValue>[] pairs)
    {
        GlideString[] args = new GlideString[pairs.Length * 2];
        for (int i = 0; i < pairs.Length; i++)
        {
            args[i * 2] = pairs[i].Key.ToGlideString();
            args[(i * 2) + 1] = pairs[i].Value.ToGlideString();
        }

        return args;
    }

    #endregion
}
