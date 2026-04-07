// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The type of get expiry operation.
/// </summary>
public enum GetExpiryType
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
    /// Remove expiry (make persistent).
    /// </summary>
    Persist,
}

/// <summary>
/// Options for a get expiry operation.
/// </summary>
public sealed class GetExpiryOptions
{
    internal GetExpiryType Type { get; }
    internal TimeSpan? Duration { get; }
    internal DateTimeOffset? Timestamp { get; }

    private GetExpiryOptions(
        GetExpiryType type,
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
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions ExpireIn(TimeSpan duration)
        => new(GetExpiryType.ExpireIn, duration: duration);

    /// <summary>
    /// Set expiration to an absolute timestamp.
    /// </summary>
    /// <param name="timestamp">The expiry timestamp.</param>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions ExpireAt(DateTimeOffset timestamp)
        => new(GetExpiryType.ExpireAt, timestamp: timestamp);

    /// <summary>
    /// Remove expiry (make persistent).
    /// </summary>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions Persist()
        => new(GetExpiryType.Persist);
}
