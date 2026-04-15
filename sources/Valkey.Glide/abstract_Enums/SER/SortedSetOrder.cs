// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Enum to manage ordering in sorted sets.
/// </summary>
public enum SortedSetOrder
{
    /// <summary>
    /// Bases ordering off of the rank in the sorted set. This means that your start and stop inside the sorted set will be some offset into the set.
    /// </summary>
    ByRank,

    /// <summary>
    /// Bases ordering off of the score in the sorted set. This means your start/stop will be some number which is the score for each member in the sorted set.
    /// </summary>
    ByScore,

    /// <summary>
    /// Bases ordering off of lexicographical order, this is only appropriate in an instance where all the members of your sorted set are given the same score.
    /// </summary>
    ByLex,
}

internal static class SortedSetOrderExtensions
{
    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal static GlideString[] ToArgs(this SortedSetOrder sortedSetOrder)
        => sortedSetOrder switch
        {
            SortedSetOrder.ByRank => [],
            SortedSetOrder.ByScore => [ValkeyLiterals.BYSCORE],
            SortedSetOrder.ByLex => [ValkeyLiterals.BYLEX],
            _ => throw new ArgumentOutOfRangeException(nameof(sortedSetOrder)),
        };
}
