// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A sorted set lexicographic bound.
/// </summary>
/// <seealso href="https://valkey.io/commands/zlexcount/"/>
/// <seealso href="https://valkey.io/commands/zrangebylex/"/>
/// <seealso href="https://valkey.io/commands/zremrangebylex/"/>
public sealed class LexBound : Bound
{
    #region Constants

    /// <summary>
    /// The minimum lexicographic bound.
    /// </summary>
    public static readonly LexBound Min = new(ValkeyLiterals.RangeMinLex);

    /// <summary>
    /// The maximum lexicographic bound.
    /// </summary>
    public static readonly LexBound Max = new(ValkeyLiterals.RangeMaxLex);

    #endregion
    #region Fields

    private readonly ValkeyValue _value;

    #endregion
    #region Constructors

    private LexBound(ValkeyValue value)
    {
        _value = value;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates an inclusive lexicographic bound.
    /// </summary>
    /// <param name="value">The lexicographic value.</param>
    /// <returns>An inclusive <see cref="LexBound"/>.</returns>
    public static LexBound Inclusive(ValkeyValue value)
        => new((ValkeyValue)$"{ValkeyLiterals.RangeInclusive}{value}");

    /// <summary>
    /// Creates an exclusive lexicographic bound.
    /// </summary>
    /// <param name="value">The lexicographic value.</param>
    /// <returns>An exclusive <see cref="LexBound"/>.</returns>
    public static LexBound Exclusive(ValkeyValue value)
        => new((ValkeyValue)$"{ValkeyLiterals.RangeExclusive}{value}");

    /// <summary>
    /// Converts a string to an inclusive lexicographic bound.
    /// </summary>
    public static implicit operator LexBound(string value)
        => Inclusive((ValkeyValue)value);

    /// <summary>
    /// Converts a <see cref="ValkeyValue"/> to an inclusive lexicographic bound.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static implicit operator LexBound(ValkeyValue value)
    {
        if (value.IsNull)
        {
            throw new ArgumentNullException(nameof(value), "Use LexBound.Min or LexBound.Max for unbounded ranges.");
        }

        return Inclusive(value);
    }

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() => [_value];

    /// <summary>
    /// Returns <see langword="true"/> if this bound represents the minimum lex sentinel.
    /// </summary>
    internal bool IsMin => ReferenceEquals(this, Min);

    /// <summary>
    /// Returns <see langword="true"/> if this bound represents the maximum lex sentinel.
    /// </summary>
    internal bool IsMax => ReferenceEquals(this, Max);

    #endregion
}
