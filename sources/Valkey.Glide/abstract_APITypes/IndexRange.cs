// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// An index-based (rank) sorted set range.
/// </summary>
/// <seealso href="https://valkey.io/commands/zrange/"/>
/// <seealso href="https://valkey.io/commands/zremrangebyrank/"/>
public sealed class IndexRange : Range
{
    #region Constants

    /// <summary>
    /// The first index in the sorted set (rank 0).
    /// </summary>
    public const long First = 0L;

    /// <summary>
    /// The last index in the sorted set (rank -1).
    /// </summary>
    public const long Last = -1L;

    /// <summary>
    /// An ascending range spanning all indices (first to last).
    /// </summary>
    public static readonly IndexRange FirstToLast = new(First, Last);

    /// <summary>
    /// A descending range spanning all indices (last to first).
    /// </summary>
    public static readonly IndexRange LastToFirst = new(Last, First);

    #endregion
    #region Fields

    private readonly long _start;
    private readonly long _stop;

    #endregion
    #region Constructors

    private IndexRange(long start, long stop)
    {
        _start = start;
        _stop = stop;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Creates an index range between start and stop indices.
    /// </summary>
    /// <param name="start">The start index.</param>
    /// <param name="stop">The stop index.</param>
    /// <returns>An <see cref="IndexRange"/> between start and stop indices.</returns>
    public static IndexRange Between(long start, long stop) => new(start, stop);

    #endregion
    #region Internal Methods

    /// <inheritdoc/>
    internal override GlideString[] ToArgs() => [_start.ToGlideString(), _stop.ToGlideString()];

    #endregion
}
