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
    /// An ascending range spanning all scores (minimum to maximum).
    /// </summary>
    public static readonly ScoreRange MinToMax = new(ScoreBound.Min, ScoreBound.Max);

    /// <summary>
    /// A descending range spanning all scores (maximum to minimum).
    /// </summary>
    public static readonly ScoreRange MaxToMin = new(ScoreBound.Max, ScoreBound.Min);

    #endregion
    #region Fields

    private readonly ScoreBound _start;
    private readonly ScoreBound _stop;

    #endregion
    #region Constructors

    private ScoreRange(ScoreBound start, ScoreBound stop)
    {
        _start = start;
        _stop = stop;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a score range between two bounds.
    /// </summary>
    /// <param name="start">The start score bound.</param>
    /// <param name="stop">The stop score bound.</param>
    /// <returns>A <see cref="ScoreRange"/> between the two bounds.</returns>
    public static ScoreRange Between(ScoreBound start, ScoreBound stop) => new(start, stop);

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() => [.. _start.ToArgs(), .. _stop.ToArgs()];

    #endregion
}
