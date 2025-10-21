// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the HGETEX command.
/// </summary>
/// <remarks>
/// See <a href="https://valkey.io/commands/hgetex/">valkey.io</a>
/// </remarks>
public class HashGetExOptions
{
    /// <summary>
    /// Expiration settings for the hash fields.
    /// </summary>
    public HGetExExpiry? Expiry { get; set; }

    /// <summary>
    /// Sets the expiration for the hash fields.
    /// </summary>
    /// <param name="expiry">The expiration settings</param>
    /// <returns>This HashGetExOptions instance for method chaining</returns>
    public HashGetExOptions SetExpiry(HGetExExpiry expiry)
    {
        Expiry = expiry;
        return this;
    }
}

/// <summary>
/// Expiration settings for HGETEX command.
/// </summary>
public class HGetExExpiry
{
    /// <summary>
    /// The expiration type.
    /// </summary>
    public HGetExExpiryType Type { get; }

    /// <summary>
    /// The expiration value.
    /// </summary>
    public long? Value { get; }

    private HGetExExpiry(HGetExExpiryType type, long? value = null)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Set expiration in seconds.
    /// </summary>
    /// <param name="seconds">The expiration time in seconds</param>
    /// <returns>HGetExExpiry instance</returns>
    public static HGetExExpiry Seconds(long seconds) => new(HGetExExpiryType.Seconds, seconds);

    /// <summary>
    /// Set expiration in milliseconds.
    /// </summary>
    /// <param name="milliseconds">The expiration time in milliseconds</param>
    /// <returns>HGetExExpiry instance</returns>
    public static HGetExExpiry Milliseconds(long milliseconds) => new(HGetExExpiryType.Milliseconds, milliseconds);

    /// <summary>
    /// Set expiration to Unix timestamp in seconds.
    /// </summary>
    /// <param name="unixSeconds">The Unix timestamp in seconds</param>
    /// <returns>HGetExExpiry instance</returns>
    public static HGetExExpiry UnixSeconds(long unixSeconds) => new(HGetExExpiryType.UnixSeconds, unixSeconds);

    /// <summary>
    /// Set expiration to Unix timestamp in milliseconds.
    /// </summary>
    /// <param name="unixMilliseconds">The Unix timestamp in milliseconds</param>
    /// <returns>HGetExExpiry instance</returns>
    public static HGetExExpiry UnixMilliseconds(long unixMilliseconds) => new(HGetExExpiryType.UnixMilliseconds, unixMilliseconds);

    /// <summary>
    /// Remove expiration (make persistent).
    /// </summary>
    /// <returns>HGetExExpiry instance</returns>
    public static HGetExExpiry Persist() => new(HGetExExpiryType.Persist);
}

/// <summary>
/// Expiration type for HGETEX command.
/// </summary>
public enum HGetExExpiryType
{
    /// <summary>
    /// Expiration in seconds.
    /// </summary>
    Seconds,

    /// <summary>
    /// Expiration in milliseconds.
    /// </summary>
    Milliseconds,

    /// <summary>
    /// Expiration as Unix timestamp in seconds.
    /// </summary>
    UnixSeconds,

    /// <summary>
    /// Expiration as Unix timestamp in milliseconds.
    /// </summary>
    UnixMilliseconds,

    /// <summary>
    /// Remove expiration (make persistent).
    /// </summary>
    Persist
}
