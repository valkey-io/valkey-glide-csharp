// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// A rank-based sorted set range.
/// </summary>
/// <seealso href="https://valkey.io/commands/zrange/"/>
/// <seealso href="https://valkey.io/commands/zremrangebyrank/"/>
public sealed class RankRange : Range
{
    #region Constants

    /// <summary>
    /// A range spanning all ranks.
    /// </summary>
    public static readonly RankRange All = new();

    private const long MinRank = 0L;
    private const long MaxRank = -1L;

    #endregion
    #region Fields

    private readonly long _start;
    private readonly long _stop;

    #endregion
    #region Constructors

    private RankRange(long start = MinRank, long stop = MaxRank)
    {
        _start = start;
        _stop = stop;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates a range from a start rank to the end of the sorted set.
    /// </summary>
    /// <param name="start">The start rank.</param>
    /// <returns>A <see cref="RankRange"/> from a start rank.</returns>
    public static RankRange From(long start) => new(start: start);

    /// <summary>
    /// Creates a range from the beginning of the sorted set to a stop rank.
    /// </summary>
    /// <param name="stop">The stop rank.</param>
    /// <returns>A <see cref="RankRange"/> to a stop rank.</returns>
    public static RankRange To(long stop) => new(stop: stop);

    /// <summary>
    /// Creates a rank range between start and stop ranks.
    /// </summary>
    /// <param name="start">The start rank.</param>
    /// <param name="stop">The stop rank.</param>
    /// <returns>A <see cref="RankRange"/> between start and stop ranks.</returns>
    public static RankRange Between(long start, long stop) => new(start, stop);

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override bool IsUnbounded() => _start == MinRank && _stop == MaxRank;

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() => [_start.ToGlideString(), _stop.ToGlideString()];

    #endregion
}
