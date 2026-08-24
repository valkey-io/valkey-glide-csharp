// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// The options for a command to set the value and expiry for a field or key.
/// </summary>
/// <seealso href="https://valkey.io/commands/set/"/>
/// <seealso href="https://valkey.io/commands/hsetex/"/>
public sealed class SetExpiryOptions
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

    private SetExpiryOptions(TimeSpan? duration = null, DateTimeOffset? timestamp = null)
    {
        Duration = duration;
        Timestamp = timestamp;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Set expiry to a duration from now (EX/PX).
    /// </summary>
    /// <param name="duration">The duration until expiry.</param>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions ExpireIn(TimeSpan duration) => new(duration: duration);

    /// <summary>
    /// Set expiry to a timestamp (EXAT/PXAT).
    /// </summary>
    /// <param name="timestamp">The expiry timestamp.</param>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions ExpireAt(DateTimeOffset timestamp) => new(timestamp: timestamp);

    /// <summary>
    /// Retain existing expiry (KEEPTTL).
    /// </summary>
    /// <returns>A new <see cref="SetExpiryOptions"/> instance.</returns>
    public static SetExpiryOptions KeepTimeToLive() => new();

    #endregion
}
