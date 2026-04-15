// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The options for an operation to retrieve a sorted set range.
/// </summary>
/// <seealso href="https://valkey.io/commands/zrange/"/>
/// <seealso href="https://valkey.io/commands/zrangebylex/"/>
/// <seealso href="https://valkey.io/commands/zrangebyscore/"/>
/// <seealso href="https://valkey.io/commands/zrangestore/"/>
/// <seealso href="https://valkey.io/commands/zrevrange/"/>
/// <seealso href="https://valkey.io/commands/zrevrangebylex/"/>
/// <seealso href="https://valkey.io/commands/zrevrangebyscore/"/>
public readonly struct RangeOptions()
{
    #region Constants

    // Default offset and count values.
    private const long NoOffset = 0L;
    private const long NoCount = -1L;

    #endregion
    #region Public Properties

    /// <summary>
    /// The range to query.
    /// Defaults to <see cref="RankRange.All"/> (all elements by rank).
    /// </summary>
    public Range Range { get; init; } = RankRange.All;

    /// <summary>
    /// The sort direction for the range query.
    /// Defaults to <see cref="Order.Ascending"/>.
    /// </summary>
    public Order Order { get; init; } = Order.Ascending;

    /// <summary>
    /// The offset for the range query.
    /// Defaults to no offset (start from first match).
    /// </summary>
    public long Offset { get; init; } = NoOffset;

    /// <summary>
    /// The count for the range query.
    /// Defaults to no count (return all matches).
    /// </summary>
    public long Count { get; init; } = NoCount;

    #endregion
    #region Internal Methods

    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal GlideString[] ToArgs()
    {
        Range range = Range ?? RankRange.All;

        List<GlideString> args = [];

        args.AddRange(range.ToArgs());

        if (range is ScoreRange)
        {
            args.Add(ValkeyLiterals.BYSCORE);
        }
        else if (range is LexRange)
        {
            args.Add(ValkeyLiterals.BYLEX);
        }

        if (Order == Order.Descending)
        {
            args.Add(ValkeyLiterals.REV);
        }

        if (Offset != NoOffset || Count != NoCount)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add(Offset.ToGlideString());
            args.Add(Count.ToGlideString());
        }

        return [.. args];
    }

    #endregion
}
