// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A sorted set score bound.
/// </summary>
/// <seealso href="https://valkey.io/commands/zcount/"/>
/// <seealso href="https://valkey.io/commands/zrangebyscore/"/>
/// <seealso href="https://valkey.io/commands/zremrangebyscore/"/>
public sealed class ScoreBound
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

        // Infinity values are never exclusive.
        _isExclusive = isExclusive && !double.IsInfinity(_score);
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates an inclusive score bound.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>An inclusive <see cref="ScoreBound"/>.</returns>
    public static ScoreBound Inclusive(double value) => new(value, isExclusive: false);

    /// <summary>
    /// Creates an exclusive score bound.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>An exclusive <see cref="ScoreBound"/>.</returns>
    public static ScoreBound Exclusive(double value) => new(value, isExclusive: true);

    /// <summary>
    /// Implicitly converts a <see langword="double"/> to an inclusive <see cref="ScoreBound"/>.
    /// </summary>
    public static implicit operator ScoreBound(double value) => new(value, isExclusive: false);

    /// <inheritdoc/>
    public bool Equals(ScoreBound? other)
        => other is not null
        && _score == other._score
        && _isExclusive == other._isExclusive;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ScoreBound);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_score, _isExclusive);

    /// <inheritdoc/>
    public int CompareTo(ScoreBound? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return _score.CompareTo(other._score);
    }

    /// <inheritdoc/>
    public static bool operator <(ScoreBound left, ScoreBound right) => left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator <=(ScoreBound left, ScoreBound right) => left.CompareTo(right) <= 0;

    /// <inheritdoc/>
    public static bool operator >(ScoreBound left, ScoreBound right) => left.CompareTo(right) > 0;

    /// <inheritdoc/>
    public static bool operator >=(ScoreBound left, ScoreBound right) => left.CompareTo(right) >= 0;

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        if (_score == double.NegativeInfinity)
        {
            return [ValkeyLiterals.ScoreRangeMin];
        }

        if (_score == double.PositiveInfinity)
        {
            return [ValkeyLiterals.ScoreRangeMax];
        }

        return _isExclusive
            ? [ValkeyLiterals.RangeExclusive + _score.ToGlideString()]
            : [_score.ToGlideString()];
    }

    #endregion
}
