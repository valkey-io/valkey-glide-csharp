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
    public static readonly ScoreBound Min = new(double.NegativeInfinity, isExclusive: false);

    /// <summary>
    /// The maximum score bound (positive infinity).
    /// </summary>
    public static readonly ScoreBound Max = new(double.PositiveInfinity, isExclusive: false);

    #endregion
    #region Fields

    private readonly double _score;
    private readonly bool _isExclusive;

    #endregion
    #region Constructors

    private ScoreBound(double score, bool isExclusive)
    {
        _score = score;
        _isExclusive = isExclusive;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates an inclusive score bound.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>An inclusive <see cref="ScoreBound"/>.</returns>
    public static ScoreBound Inclusive(double value)
        => new(value, isExclusive: false);

    /// <summary>
    /// Creates an exclusive score bound.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>An exclusive <see cref="ScoreBound"/>.</returns>
    public static ScoreBound Exclusive(double value)
        => new(value, isExclusive: true);

    /// <summary>
    /// Converts a <see cref="ValkeyValue"/> to an inclusive lexicographic bound.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static implicit operator ScoreBound(ValkeyValue value)
    {
        if (value.IsNull)
        {
            throw new ArgumentNullException(nameof(value), "Use ScoreBound.Min or ScoreBound.Max for unbounded ranges.");
        }

        else if (value.Equals(ValkeyLiterals.LexRangeMin))
        {
            return Min;
        }

        else if (value.Equals(ValkeyLiterals.LexRangeMax))
        {
            return Max;
        }

        return Inclusive((double)value);
    }

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
    internal override GlideString[] ToArgs()
    {
        GlideString scoreStr = _score.ToGlideString();
        return _isExclusive ? [(GlideString)ValkeyLiterals.RangeExclusive + scoreStr] : [scoreStr];
    }

    #endregion
}
