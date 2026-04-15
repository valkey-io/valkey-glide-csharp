// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes a sorted-set element with the corresponding value.
/// </summary>
/// <remarks>
/// Initializes a <see cref="SortedSetEntry"/> value.
/// </remarks>
/// <param name="element">The <see cref="ValkeyValue"/> to get an entry for.</param>
/// <param name="score">The score for <paramref name="element"/>.</param>
public readonly struct SortedSetEntry(ValkeyValue element, double score) : IEquatable<SortedSetEntry>, IComparable, IComparable<SortedSetEntry>
{
    /// <summary>
    /// The sorted set member.
    /// </summary>
    public readonly ValkeyValue Element = element;

    /// <summary>
    /// The score for the sorted set member.
    /// </summary>
    public readonly double Score = score;

    /// <summary>
    /// Converts to a key/value pair.
    /// </summary>
    /// <param name="value">The <see cref="SortedSetEntry"/> to get a <see cref="KeyValuePair{TKey, TValue}"/> for.</param>
    public static implicit operator KeyValuePair<ValkeyValue, double>(SortedSetEntry value)
        => new(value.Element, value.Score);

    /// <summary>
    /// Converts from a key/value pair.
    /// </summary>
    /// <param name="value">The  <see cref="KeyValuePair{TKey, TValue}"/> to get a <see cref="SortedSetEntry"/> for.</param>
    public static implicit operator SortedSetEntry(KeyValuePair<ValkeyValue, double> value)
        => new(value.Key, value.Value);

    /// <inheritdoc/>
    public override string ToString()
        => Element.ToString() + ": " + Score.ToString();

    /// <inheritdoc/>
    public override int GetHashCode()
        => Element.GetHashCode() ^ Score.GetHashCode();

    /// <inheritdoc/>
    public bool Equals(SortedSetEntry other)
        => Element == other.Element && Score == other.Score;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is SortedSetEntry entry && Equals(entry);

    /// <inheritdoc/>
    public int CompareTo(SortedSetEntry other)
        => Score.CompareTo(other.Score);

    /// <inheritdoc/>
    public int CompareTo(object? obj)
        => obj is SortedSetEntry ssObj ? CompareTo(ssObj) : -1;

    /// <inheritdoc/>
    public static bool operator ==(SortedSetEntry x, SortedSetEntry y)
        => x.Score == y.Score && x.Element == y.Element;

    /// <inheritdoc/>
    public static bool operator !=(SortedSetEntry x, SortedSetEntry y)
        => x.Score != y.Score || x.Element != y.Element;
}
