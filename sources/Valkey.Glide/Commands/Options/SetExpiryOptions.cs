// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for a set expiry operation.
/// </summary>
public sealed class SetExpiryOptions
{
    internal TimeSpan? Duration { get; }
    internal DateTimeOffset? Timestamp { get; }

    private SetExpiryOptions(
        TimeSpan? duration = null,
        DateTimeOffset? timestamp = null)
    {
        Duration = duration;
        Timestamp = timestamp;
    }

    /// <summary>
    /// Set expiry to a duration from now.
    /// </summary>
    /// <param name="duration">The duration until expiry.</param>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions ExpireIn(TimeSpan duration) => new(duration: duration);

    /// <summary>
    /// Set expiry to a timestamp.
    /// </summary>
    /// <param name="timestamp">The expiry timestamp.</param>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions ExpireAt(DateTimeOffset timestamp) => new(timestamp: timestamp);

    /// <summary>
    /// Keep the existing expiry.
    /// </summary>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions KeepTimeToLive() => new();
}
