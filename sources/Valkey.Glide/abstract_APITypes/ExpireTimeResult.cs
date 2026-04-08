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
    /// <summary>
    /// Whether the key or field exists.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    /// Whether the key or field has an expiry set.
    /// </summary>
    public bool HasExpiry { get; }

    /// <summary>
    /// The expiry timestamp, or <see langword="null"/> if the key
    /// or field does not exist or does not have an expiry set.
    /// </summary>
    public DateTimeOffset? Expiry { get; }

    /// <summary>
    /// Creates an <see cref="ExpireTimeResult"/> from the raw millisecond value.
    /// </summary>
    internal ExpireTimeResult(long rawMilliseconds)
    {
        switch (rawMilliseconds)
        {
            case -2:
                Exists = false;
                HasExpiry = false;
                Expiry = null;
                break;
            case -1:
                Exists = true;
                HasExpiry = false;
                Expiry = null;
                break;
            default:
                Exists = true;
                HasExpiry = true;
                Expiry = DateTimeOffset.FromUnixTimeMilliseconds(rawMilliseconds);
                break;
        }
    }
}
