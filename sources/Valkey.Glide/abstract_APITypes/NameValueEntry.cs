// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes a value contained in a stream (a name/value pair).
/// </summary>
/// <remarks>
/// Initializes a <see cref="NameValueEntry"/> value.
/// </remarks>
/// <param name="name">The name for this entry.</param>
/// <param name="value">The value for this entry.</param>
public readonly struct NameValueEntry(ValkeyValue name, ValkeyValue value) : IEquatable<NameValueEntry>
{
    /// <summary>
    /// The name of the field.
    /// </summary>
    public ValkeyValue Name { get; } = name;

    /// <summary>
    /// The value of the field.
    /// </summary>
    public ValkeyValue Value { get; } = value;

    /// <summary>
    /// Converts to a key/value pair.
    /// </summary>
    /// <param name="value">The <see cref="NameValueEntry"/> to create a <see cref="KeyValuePair{TKey, TValue}"/> from.</param>
    public static implicit operator KeyValuePair<ValkeyValue, ValkeyValue>(NameValueEntry value)
        => new(value.Name, value.Value);

    /// <summary>
    /// Converts from a key/value pair.
    /// </summary>
    /// <param name="value">The <see cref="KeyValuePair{TKey, TValue}"/> to get a <see cref="NameValueEntry"/> from.</param>
    public static implicit operator NameValueEntry(KeyValuePair<ValkeyValue, ValkeyValue> value)
        => new(value.Key, value.Value);

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="x">The first <see cref="NameValueEntry"/> to compare.</param>
    /// <param name="y">The second <see cref="NameValueEntry"/> to compare.</param>
    public static bool operator ==(NameValueEntry x, NameValueEntry y)
        => x.Name == y.Name && x.Value == y.Value;

    /// <summary>
    /// Compares two values for non-equality.
    /// </summary>
    /// <param name="x">The first <see cref="NameValueEntry"/> to compare.</param>
    /// <param name="y">The second <see cref="NameValueEntry"/> to compare.</param>
    public static bool operator !=(NameValueEntry x, NameValueEntry y)
        => x.Name != y.Name || x.Value != y.Value;

    /// <summary>
    /// The "{name}: {value}" string representation.
    /// </summary>
    public override string ToString()
        => Name + ": " + Value;

    /// <inheritdoc/>
    public override int GetHashCode()
        => Name.GetHashCode() ^ Value.GetHashCode();

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="obj">The <see cref="NameValueEntry"/> to compare to.</param>
    public override bool Equals(object? obj)
        => obj is NameValueEntry heObj && Equals(heObj);

    /// <summary>
    /// Compares two values for equality.
    /// </summary>
    /// <param name="other">The <see cref="NameValueEntry"/> to compare to.</param>
    public bool Equals(NameValueEntry other)
        => Name == other.Name && Value == other.Value;

    #region Internal Methods

    /// <summary>
    /// Builds the command arguments for this entry.
    /// </summary>
    internal GlideString[] ToArgs()
        => [Name, Value];

    #endregion
}
