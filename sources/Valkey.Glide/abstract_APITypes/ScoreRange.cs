// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A score-based sorted set range.
/// </summary>
/// <seealso href="https://valkey.io/commands/zcount/"/>
/// <seealso href="https://valkey.io/commands/zrangebyscore/"/>
/// <seealso href="https://valkey.io/commands/zremrangebyscore/"/>
public sealed class ScoreRange : Range
{
    #region Constants

    /// <summary>
    /// A range spanning all scores.
    /// </summary>
    public static readonly ScoreRange All = new(ScoreBound.Min, ScoreBound.Max);

    #endregion
    #region Fields

    private readonly ScoreBound _min;
    private readonly ScoreBound _max;

    #endregion
    #region Constructors

    private ScoreRange(ScoreBound min, ScoreBound max)
    {
        _min = min;
        _max = max;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a score range from a minimum bound.
    /// </summary>
    /// <param name="min">The minimum score bound.</param>
    /// <returns>A <see cref="ScoreRange"/> from a minimum bound.</returns>
    public static ScoreRange From(ScoreBound min)
        => new(min, ScoreBound.Max);

    /// <summary>
    /// Creates a score range to a maximum bound.
    /// </summary>
    /// <param name="max">The maximum score bound.</param>
    /// <returns>A <see cref="ScoreRange"/> to a maximum bound.</returns>
    public static ScoreRange To(ScoreBound max)
        => new(ScoreBound.Min, max);

    /// <summary>
    /// Creates a score range between minimum and maximum bounds.
    /// </summary>
    /// <param name="min">The minimum score bound.</param>
    /// <param name="max">The maximum score bound.</param>
    /// <returns>A <see cref="ScoreRange"/> between minimum and maximum bounds.</returns>
    public static ScoreRange Between(ScoreBound min, ScoreBound max)
        => new(min, max);

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override bool IsUnbounded()
        => _min.IsMin && _max.IsMax;

    /// <inheritdoc/>
    internal override GlideString[] ToArgs()
        => [.. _min.ToArgs(), .. _max.ToArgs()];

    #endregion
}
