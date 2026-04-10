namespace Valkey.Glide;

/// <summary>
/// The result of an operation to get the expiry for a key or hash field.
/// </summary>
/// <seealso href="https://valkey.io/commands/expiretime/"/>
/// <seealso href="https://valkey.io/commands/pexpiretime/"/>
/// <seealso href="https://valkey.io/commands/hexpiretime/"/>
/// <seealso href="https://valkey.io/commands/hpexpiretime/"/>
public readonly struct ExpireTimeResult
{
    internal readonly long ExpireTimeMs;

    // Special expire time values.
    internal const long DoesNotExist = -2;
    internal const long NoExpiry = -1;

    /// <summary>
    /// Whether the key or hash field exists.
    /// </summary>
    public bool Exists => ExpireTimeMs != DoesNotExist;

    /// <summary>
    /// Whether the key or hash field has an expiry set.
    /// </summary>
    public bool HasExpiry => ExpireTimeMs >= 0;

    /// <summary>
    /// The expiry timestamp, or <see langword="null"/> if the key
    /// or field does not exist or does not have an expiry set.
    /// </summary>
    public DateTimeOffset? Expiry
        => HasExpiry ? DateTimeOffset.FromUnixTimeMilliseconds(ExpireTimeMs) : null;

    /// <summary>
    /// Creates an <see cref="ExpireTimeResult"/> from the given value.
    /// </summary>
    internal ExpireTimeResult(long expireTimeMs)
    {
        ExpireTimeMs = expireTimeMs;
    }
}
