// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A sorted set score bound.
/// </summary>
/// <seealso href="https://valkey.io/commands/zcount/"/>
/// <seealso href="https://valkey.io/commands/zrangebyscore/"/>
/// <seealso href="https://valkey.io/commands/zremrangebyscore/"/>
public sealed class ScoreBound : Bound, IEquatable<ScoreBound>
{
    #region Constants

    /// <summary>
    /// The minimum score bound (negative infinity).
    /// </summary>
    public static readonly ScoreBound Min = new(double.NegativeInfinity, isInclusive: false);

    /// <summary>
    /// The maximum score bound (positive infinity).
    /// </summary>
    public static readonly ScoreBound Max = new(double.PositiveInfinity, isInclusive: false);

    #endregion
    #region Fields

    private readonly double _score;
    private readonly bool _isInclusive;

    #endregion
    #region Constructors

    private ScoreBound(double score, bool isInclusive)
    {
        _score = score;

        // Infinity values are never inclusive.
        _isInclusive = isInclusive && !double.IsInfinity(_score);
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates an inclusive score bound.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>An inclusive <see cref="ScoreBound"/>.</returns>
    public static ScoreBound Inclusive(double value) => new(value, isInclusive: true);

    /// <summary>
    /// Creates an exclusive score bound.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>An exclusive <see cref="ScoreBound"/>.</returns>
    public static ScoreBound Exclusive(double value) => new(value, isInclusive: false);

    /// <summary>
    /// Implicitly converts a <see langword="double"/> to an inclusive <see cref="ScoreBound"/>.
    /// </summary>
    public static implicit operator ScoreBound(double value) => new(value, isInclusive: true);

    /// <inheritdoc/>
    public bool Equals(ScoreBound? other)
        => other is not null
        && _score == other._score
        && _isInclusive == other._isInclusive;

    /// <inheritdoc/>
    public override bool Equals(Bound? other) => Equals(other as ScoreBound);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_score, _isInclusive);

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs()
    {
        if (_score == double.NegativeInfinity)
        {
            return [ValkeyLiterals.ScoreRangeMin];
        }

        else if (_score == double.PositiveInfinity)
        {
            return [ValkeyLiterals.ScoreRangeMax];
        }

        return _isInclusive
            ? [_score.ToGlideString()]
            : [ValkeyLiterals.RangeExclusive + _score.ToGlideString()];
    }

    #endregion
}
