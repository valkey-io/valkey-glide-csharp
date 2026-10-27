// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes a hash-field (a name/value pair).
/// </summary>
/// <remarks>
/// Initializes a <see cref="HashEntry"/> value.
/// </remarks>
/// <param name="name">The name for this hash entry.</param>
/// <param name="value">The value for this hash entry.</param>
public readonly struct HashEntry(ValkeyValue name, ValkeyValue value) : IEquatable<HashEntry>
{
    /// <summary>
    /// The name of the hash field.
    /// </summary>
    public ValkeyValue Name { get; } = name;

    /// <summary>
    /// The value of the hash field.
    /// </summary>
    public ValkeyValue Value { get; } = value;

    /// <summary>
    /// Converts to a key/value pair.
    /// </summary>
    /// <param name="value">The <see cref="HashEntry"/> to create a <see cref="KeyValuePair{TKey, TValue}"/> from.</param>
    public static implicit operator KeyValuePair<ValkeyValue, ValkeyValue>(HashEntry value)
        => new(value.Name, value.Value);

    /// <summary>
    /// Converts from a key/value pair.
    /// </summary>
    /// <param name="value">The <see cref="KeyValuePair{TKey, TValue}"/> to get a <see cref="HashEntry"/> from.</param>
    public static implicit operator HashEntry(KeyValuePair<ValkeyValue, ValkeyValue> value)
        => new(value.Key, value.Value);

    /// <summary>
    /// A "{name}: {value}" string representation of this entry.
    /// </summary>
    public override string ToString()
        => Name + ": " + Value;

    /// <inheritdoc/>
    public override int GetHashCode()
        => Name.GetHashCode() ^ Value.GetHashCode();

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="obj">The <see cref="HashEntry"/> to compare to.</param>
    public override bool Equals(object? obj)
        => obj is HashEntry heObj && Equals(heObj);

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="other">The <see cref="HashEntry"/> to compare to.</param>
    public bool Equals(HashEntry other)
        => Name == other.Name && Value == other.Value;

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="x">The first <see cref="HashEntry"/> to compare.</param>
    /// <param name="y">The second <see cref="HashEntry"/> to compare.</param>
    public static bool operator ==(HashEntry x, HashEntry y)
        => x.Name == y.Name && x.Value == y.Value;

    /// <summary>
    /// Compares two values for non-equality.
    /// </summary>
    /// <param name="x">The first <see cref="HashEntry"/> to compare.</param>
    /// <param name="y">The second <see cref="HashEntry"/> to compare.</param>
    public static bool operator !=(HashEntry x, HashEntry y)
        => x.Name != y.Name || x.Value != y.Value;
}
