// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The options for an operation to get the value and set the expiry for a field or key.
/// </summary>
/// <seealso href="https://valkey.io/commands/getex/"/>
/// <seealso href="https://valkey.io/commands/hgetex/"/>
public sealed class GetExpiryOptions
{
    #region Internal Properties

    /// <summary>
    /// The expiry duration to set, if specified.
    /// </summary>
    internal TimeSpan? Duration { get; }

    /// <summary>
    /// The expiry timestamp to set, if specified.
    /// </summary>
    internal DateTimeOffset? Timestamp { get; }

    #endregion
    #region Constructors

    private GetExpiryOptions(TimeSpan? duration = null, DateTimeOffset? timestamp = null)
    {
        // Only one expiry can be specified.
        if (duration.HasValue && timestamp.HasValue)
        {
            throw new ArgumentException("Duration and Timestamp cannot both be specified.");
        }

        Duration = duration;
        Timestamp = timestamp;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Set expiry to a duration from now (EX/PX).
    /// </summary>
    /// <param name="duration">The duration until expiry.</param>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions ExpireIn(TimeSpan duration) => new(duration: duration);

    /// <summary>
    /// Set expiry to a timestamp (EXAT/PXAT).
    /// </summary>
    /// <param name="timestamp">The expiry timestamp.</param>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions ExpireAt(DateTimeOffset timestamp) => new(timestamp: timestamp);

    /// <summary>
    /// Remove existing expiry (PERSIST).
    /// </summary>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions Persist() => new();

    #endregion
}
