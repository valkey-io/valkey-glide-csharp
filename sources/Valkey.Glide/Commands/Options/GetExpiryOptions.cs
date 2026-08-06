// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

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
    /// The expiry duration.
    /// </summary>
    internal ulong? DurationMs { get; }

    /// <summary>
    /// The expiry timestamp.
    /// </summary>
    internal DateTimeOffset? Timestamp { get; }

    #endregion
    #region Constructors

    private GetExpiryOptions(ulong? durationMs = null, DateTimeOffset? timestamp = null)
    {
        DurationMs = durationMs;
        Timestamp = timestamp;
    }

    #endregion
    #region Public Methods

    /// <summary>
    /// Set expiry to a duration from now (EX/PX).
    /// </summary>
    /// <param name="duration">The duration until expiry.</param>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="duration"/> is not positive.</exception>
    public static GetExpiryOptions ExpireIn(TimeSpan duration)
        => new(durationMs: TimeUtils.ToPositiveULongMs(duration, nameof(duration)));

    /// <summary>
    /// Set expiry to a timestamp (EXAT/PXAT).
    /// </summary>
    /// <param name="timestamp">The expiry timestamp.</param>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions ExpireAt(DateTimeOffset timestamp)
        => new(timestamp: timestamp);

    /// <summary>
    /// Remove existing expiry (PERSIST).
    /// </summary>
    /// <returns>A new <see cref="GetExpiryOptions"/> instance.</returns>
    public static GetExpiryOptions Persist() => new();

    #endregion
    #region Internal Methods

    internal GlideString[] ToArgs()
    {
        if (DurationMs.HasValue)
        {
            return [ValkeyLiterals.PX, DurationMs.Value.ToGlideString()];
        }

        if (Timestamp.HasValue)
        {
            return [ValkeyLiterals.PXAT, Timestamp.Value.ToUnixTimeMilliseconds().ToGlideString()];
        }

        return [ValkeyLiterals.PERSIST];
    }

    #endregion
}
