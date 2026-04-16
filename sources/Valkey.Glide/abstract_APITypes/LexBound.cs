// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A sorted set lexicographic bound.
/// </summary>
/// <seealso href="https://valkey.io/commands/zlexcount/"/>
/// <seealso href="https://valkey.io/commands/zrangebylex/"/>
/// <seealso href="https://valkey.io/commands/zremrangebylex/"/>
public sealed class LexBound : Bound, IEquatable<LexBound>, IComparable<LexBound>
{
    #region Constants

    /// <summary>
    /// The minimum lexicographic bound.
    /// </summary>
    public static readonly LexBound Min = new(ValkeyLiterals.LexRangeMin, isInclusive: false);

    /// <summary>
    /// The maximum lexicographic bound.
    /// </summary>
    public static readonly LexBound Max = new(ValkeyLiterals.LexRangeMax, isInclusive: false);

    #endregion
    #region Fields

    private readonly ValkeyValue _value;
    private readonly bool _isInclusive;

    #endregion
    #region Constructors

    private LexBound(ValkeyValue value, bool isInclusive)
    {
        _value = value;

        // Min/max values are never inclusive.
        _isInclusive = isInclusive
            && value != ValkeyLiterals.LexRangeMin
            && value != ValkeyLiterals.LexRangeMax;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates an inclusive lexicographic bound.
    /// </summary>
    /// <param name="value">The lexicographic value.</param>
    /// <returns>An inclusive <see cref="LexBound"/>.</returns>
    public static LexBound Inclusive(ValkeyValue value) => new(value, isInclusive: true);

    /// <summary>
    /// Creates an exclusive lexicographic bound.
    /// </summary>
    /// <param name="value">The lexicographic value.</param>
    /// <returns>An exclusive <see cref="LexBound"/>.</returns>
    public static LexBound Exclusive(ValkeyValue value) => new(value, isInclusive: false);

    /// <summary>
    /// Converts a string to an inclusive lexicographic bound.
    /// </summary>
    public static implicit operator LexBound(string value) => Inclusive(value);

    /// <summary>
    /// Converts a byte array to an inclusive lexicographic bound.
    /// </summary>
    public static implicit operator LexBound(byte[] value) => Inclusive(value);

    /// <summary>
    /// Converts a <see cref="ValkeyValue"/> to an inclusive lexicographic bound.
    /// </summary>
    public static implicit operator LexBound(ValkeyValue value) => Inclusive((ValkeyValue)value);

    /// <inheritdoc/>
    public bool Equals(LexBound? other)
        => other is not null
        && _value == other._value
        && _isInclusive == other._isInclusive;

    /// <inheritdoc/>
    public override bool Equals(Bound? other) => Equals(other as LexBound);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_value, _isInclusive);

    /// <inheritdoc/>
    public int CompareTo(LexBound? other)
    {
        if (other is null)
        {
            return 1;
        }

        bool otherIsMin = other._value == ValkeyLiterals.LexRangeMin;
        bool otherIsMax = other._value == ValkeyLiterals.LexRangeMax;

        // Min is less than everything except itself.
        if (_value == ValkeyLiterals.LexRangeMin)
        {
            return otherIsMin ? 0 : -1;
        }

        if (otherIsMin)
        {
            return 1;
        }

        // Max is greater than everything except itself.
        if (_value == ValkeyLiterals.LexRangeMax)
        {
            return otherIsMax ? 0 : 1;
        }

        if (otherIsMax)
        {
            return -1;
        }

        return _value.CompareTo(other._value);
    }

    /// <inheritdoc/>
    public static bool operator <(LexBound left, LexBound right) => left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(LexBound left, LexBound right) => left.CompareTo(right) <= 0;

    /// <inheritdoc/>
    public static bool operator >(LexBound left, LexBound right) => left.CompareTo(right) > 0;

    /// <inheritdoc/>
    public static bool operator >=(LexBound left, LexBound right) => left.CompareTo(right) >= 0;

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs()
    {
        if (_value == ValkeyLiterals.LexRangeMin)
        {
            return [ValkeyLiterals.LexRangeMin];
        }

        else if (_value == ValkeyLiterals.LexRangeMax)
        {
            return [ValkeyLiterals.LexRangeMax];
        }

        GlideString value = _value;
        return _isInclusive
            ? [ValkeyLiterals.RangeInclusive.ToGlideString() + value]
            : [ValkeyLiterals.RangeExclusive.ToGlideString() + value];
    }

    #endregion
}
