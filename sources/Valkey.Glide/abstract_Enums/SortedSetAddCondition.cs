// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// The condition for an operation to add or update sorted set members.
/// </summary>
/// <seealso href="https://valkey.io/commands/zadd/"/>
public enum SortedSetAddCondition
{
    /// <summary>
    /// Always add or update the sorted set members.
    /// </summary>
    Always,

    /// <summary>
    /// Only add new sorted set members (NX).
    /// </summary>
    OnlyIfNotExists,

    /// <summary>
    /// Only update existing sorted set members (XX).
    /// </summary>
    OnlyIfExists,

    /// <summary>
    /// Only add new members, or update existing members if the new score is greater than the current score (GT).
    /// </summary>
    OnlyIfNotExistsOrGreaterThan,

    /// <summary>
    /// Only add new members, or update existing members if the new score is less than the current score (LT).
    /// </summary>
    OnlyIfNotExistsOrLessThan,

    /// <summary>
    /// Only update existing members if the new score is greater than the current score (XX + GT).
    /// </summary>
    OnlyIfGreaterThan,

    /// <summary>
    /// Only update existing members if the new score is less than the current score (XX + LT).
    /// </summary>
    OnlyIfLessThan,
}

internal static class SortedSetAddConditionExtensions
{
    /// <summary>
    /// Converts to command arguments.
    /// </summary>
    internal static GlideString[] ToArgs(this SortedSetAddCondition condition)
        => condition switch
        {
            SortedSetAddCondition.Always => [],
            SortedSetAddCondition.OnlyIfNotExists => [ValkeyLiterals.NX],
            SortedSetAddCondition.OnlyIfExists => [ValkeyLiterals.XX],
            SortedSetAddCondition.OnlyIfNotExistsOrGreaterThan => [ValkeyLiterals.GT],
            SortedSetAddCondition.OnlyIfNotExistsOrLessThan => [ValkeyLiterals.LT],
            SortedSetAddCondition.OnlyIfGreaterThan => [ValkeyLiterals.XX, ValkeyLiterals.GT],
            SortedSetAddCondition.OnlyIfLessThan => [ValkeyLiterals.XX, ValkeyLiterals.LT],
            _ => throw new ArgumentOutOfRangeException(nameof(condition)),
        };
}
