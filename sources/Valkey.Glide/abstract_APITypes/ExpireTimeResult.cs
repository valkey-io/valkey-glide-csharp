namespace Valkey.Glide;

/// <summary>
/// Represents the result of an expire time operation.
/// </summary>
/// <seealso href="https://valkey.io/commands/expiretime/"/>
/// <seealso href="https://valkey.io/commands/pexpiretime/"/>
/// <seealso href="https://valkey.io/commands/hexpiretime/"/>
/// <seealso href="https://valkey.io/commands/hpexpiretime/"/>
public readonly struct ExpireTimeResult
{
    internal readonly long ExpireTimeMs;

    /// <summary>
    /// Whether the key or field exists.
    /// </summary>
    public bool Exists => ExpireTimeMs != -2L;

    /// <summary>
    /// Whether the key or field has an expiry set.
    /// </summary>
    public bool HasExpiry => ExpireTimeMs >= 0;

    /// <summary>
    /// The expiry timestamp, or <see langword="null"/> if the key
    /// or field does not exist or does not have an expiry set.
    /// </summary>
    public DateTimeOffset? Expiry => HasExpiry ? DateTimeOffset.FromUnixTimeMilliseconds(ExpireTimeMs) : null;

    /// <summary>
    /// Creates an <see cref="ExpireTimeResult"/> from the given value.
    /// </summary>
    internal ExpireTimeResult(long expireTimeMs)
    {
        ExpireTimeMs = expireTimeMs;
    }
}
