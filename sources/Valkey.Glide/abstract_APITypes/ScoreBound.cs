// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A sorted set score bound.
/// </summary>
/// <seealso href="https://valkey.io/commands/zcount/"/>
/// <seealso href="https://valkey.io/commands/zrangebyscore/"/>
/// <seealso href="https://valkey.io/commands/zremrangebyscore/"/>
public sealed class ScoreBound : Bound
{
    #region Constants

    /// <summary>
    /// The minimum score bound (negative infinity).
    /// </summary>
    public static readonly ScoreBound Min = new(ValkeyLiterals.RangeMinScore);

    /// <summary>
    /// The maximum score bound (positive infinity).
    /// </summary>
    public static readonly ScoreBound Max = new(ValkeyLiterals.RangeMaxScore);

    #endregion
    #region Fields

    private readonly ValkeyValue _value;

    #endregion
    #region Constructors

    private ScoreBound(ValkeyValue value)
    {
        _value = value;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates an inclusive score bound.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>An inclusive <see cref="ScoreBound"/>.</returns>
    public static ScoreBound Inclusive(double value)
        => new((ValkeyValue)value);

    /// <summary>
    /// Creates an exclusive score bound.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>An exclusive <see cref="ScoreBound"/>.</returns>
    public static ScoreBound Exclusive(double value)
        => new(ValkeyLiterals.RangeExclusive + (ValkeyValue)value);

    /// <summary>
    /// Implicitly converts a <see langword="double"/> to an inclusive <see cref="ScoreBound"/>.
    /// Infinity values are mapped to <see cref="Min"/> and <see cref="Max"/> respectively.
    /// </summary>
    public static implicit operator ScoreBound(double value) => value switch
    {
        double.NegativeInfinity => Min,
        double.PositiveInfinity => Max,
        _ => Inclusive(value),
    };

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() => [_value];

    /// <summary>
    /// Returns <see langword="true"/> if this bound represents the minimum score sentinel.
    /// </summary>
    internal bool IsMin => ReferenceEquals(this, Min);

    /// <summary>
    /// Returns <see langword="true"/> if this bound represents the maximum score sentinel.
    /// </summary>
    internal bool IsMax => ReferenceEquals(this, Max);

    #endregion
}
