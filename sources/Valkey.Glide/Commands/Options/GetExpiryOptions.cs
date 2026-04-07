// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Options for a get/expiry operation.
/// </summary>
public sealed class GetExpiryOptions
{
    internal TimeSpan? Duration { get; }
    internal DateTimeOffset? Timestamp { get; }

    private GetExpiryOptions(
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
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions ExpireIn(TimeSpan duration) => new(duration: duration);

    /// <summary>
    /// Set expiry to an absolute timestamp.
    /// </summary>
    /// <param name="timestamp">The expiry timestamp.</param>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions ExpireAt(DateTimeOffset timestamp) => new(timestamp: timestamp);

    /// <summary>
    /// Remove expiry (make persistent).
    /// </summary>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions Persist() => new();
}
