// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands.Options;

/// <summary>
/// Optional arguments for the HSETEX command.
/// </summary>
/// <remarks>
/// See <a href="https://valkey.io/commands/hsetex/">valkey.io</a>
/// </remarks>
public class HashSetExOptions
{
    /// <summary>
    /// Expiration settings for the hash fields.
    /// </summary>
    public ExpirySet? Expiry { get; set; }

    /// <summary>
    /// When true, only set fields if none of them exist.
    /// </summary>
    public bool OnlyIfNoneExist { get; set; } = false;

    /// <summary>
    /// When true, only set fields if all of them exist.
    /// </summary>
    public bool OnlyIfAllExist { get; set; } = false;

    /// <summary>
    /// Sets the expiration for the hash fields.
    /// </summary>
    /// <param name="expiry">The expiration settings</param>
    /// <returns>This HashSetExOptions instance for method chaining</returns>
    public HashSetExOptions SetExpiry(ExpirySet expiry)
    {
        Expiry = expiry;
        return this;
    }

    /// <summary>
    /// Sets the option to only set fields if none of them exist.
    /// </summary>
    /// <returns>This HashSetExOptions instance for method chaining</returns>
    public HashSetExOptions SetOnlyIfNoneExist()
    {
        OnlyIfNoneExist = true;
        OnlyIfAllExist = false;
        return this;
    }

    /// <summary>
    /// Sets the option to only set fields if all of them exist.
    /// </summary>
    /// <returns>This HashSetExOptions instance for method chaining</returns>
    public HashSetExOptions SetOnlyIfAllExist()
    {
        OnlyIfAllExist = true;
        OnlyIfNoneExist = false;
        return this;
    }
}

/// <summary>
/// Expiration settings for HSETEX command.
/// </summary>
public class ExpirySet
{
    /// <summary>
    /// The expiration type.
    /// </summary>
    public ExpiryType Type { get; }

    /// <summary>
    /// The expiration value.
    /// </summary>
    public long? Value { get; }

    private ExpirySet(ExpiryType type, long? value = null)
    {
        Type = type;
        Value = value;
    }

    /// <summary>
    /// Set expiration in seconds.
    /// </summary>
    /// <param name="seconds">The expiration time in seconds</param>
    /// <returns>ExpirySet instance</returns>
    public static ExpirySet Seconds(long seconds) => new(ExpiryType.Seconds, seconds);

    /// <summary>
    /// Set expiration in milliseconds.
    /// </summary>
    /// <param name="milliseconds">The expiration time in milliseconds</param>
    /// <returns>ExpirySet instance</returns>
    public static ExpirySet Milliseconds(long milliseconds) => new(ExpiryType.Milliseconds, milliseconds);

    /// <summary>
    /// Keep existing expiration.
    /// </summary>
    /// <returns>ExpirySet instance</returns>
    public static ExpirySet KeepExisting() => new(ExpiryType.KeepExisting);
}

/// <summary>
/// Expiration type for HSETEX command.
/// </summary>
public enum ExpiryType
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
    /// Keep existing expiration.
    /// </summary>
    KeepExisting
}
