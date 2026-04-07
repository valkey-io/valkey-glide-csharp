// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The type of set expiry operation.
/// </summary>
public enum SetExpiryType
{
    /// <summary>
    /// Set expiry as a duration from now.
    /// </summary>
    ExpireIn,

    /// <summary>
    /// Set expiry to a timestamp.
    /// </summary>
    ExpireAt,

    /// <summary>
    /// Keep the existing expiry.
    /// </summary>
    KeepTimeToLive,
}

/// <summary>
/// The condition for the set expiry operation.
/// </summary>
public enum SetExpiryCondition
{
    /// <summary>
    /// Always set the keys or fields.
    /// </summary>
    Always,

    /// <summary>
    /// Only set if none of the specified keys or fields exist (FNX).
    /// </summary>
    OnlyIfNoneExist,

    /// <summary>
    /// Only set if all of the specified keys or fields exist (FXX).
    /// </summary>
    OnlyIfAllExist,
}

/// <summary>
/// Options for a set expiry operation.
/// </summary>
public sealed class SetExpiryOptions
{
    internal SetExpiryType Type { get; }
    internal TimeSpan? Duration { get; }
    internal DateTimeOffset? Timestamp { get; }

    private SetExpiryOptions(
        SetExpiryType type,
        TimeSpan? duration = null,
        DateTimeOffset? timestamp = null)
    {
        Type = type;
        Duration = duration;
        Timestamp = timestamp;
    }

    /// <summary>
    /// Set expiry to a durationfrom now.
    /// </summary>
    /// <param name="duration">The duration until expiry.</param>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions ExpireIn(TimeSpan duration)
        => new(SetExpiryType.ExpireIn, duration: duration);

    /// <summary>
    /// Set expiry to a timestamp.
    /// </summary>
    /// <param name="timestamp">The expiry timestamp.</param>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions ExpireAt(DateTimeOffset timestamp)
        => new(SetExpiryType.ExpireAt, timestamp: timestamp);

    /// <summary>
    /// Keep the existing expiry.
    /// </summary>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions KeepTimeToLive()
        => new(SetExpiryType.KeepTimeToLive);
}
