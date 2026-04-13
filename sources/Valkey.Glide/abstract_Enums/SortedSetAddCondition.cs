namespace Valkey.Glide;

/// <summary>
/// The condition for an operation to add or change sorted set members.
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
    /// Only add new members or update existing members if the new score is greater than the current score (GT).
    /// </summary>
    OnlyIfNotExistsOrGreaterThan,

    /// <summary>
    /// Only add new members or update existing members if the new score is less than the current score (LT).
    /// </summary>
    OnlyIfNotExistsOrLessThan,

    /// <summary>
    /// Only update existing members when the new score is greater than the current score (XX + GT).
    /// </summary>
    OnlyIfGreaterThan,

    /// <summary>
    /// Only update existing members when the new score is less than the current score (XX + LT).
    /// </summary>
    OnlyIfLessThan,
}
