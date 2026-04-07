namespace Valkey.Glide;

/// <summary>
/// Describes the expiry type for a key or field.
/// </summary>
public enum ExpiryType
{
    /// <summary>
    /// The key or field has an expiry set.
    /// </summary>
    Expiring,

    /// <summary>
    /// The key or field was found but has no expiry.
    /// </summary>
    Persistent,

    /// <summary>
    /// The key or field was not found.
    /// </summary>
    NotFound,
}

/// <summary>
/// Represents the expiry result for a key or field.
/// </summary>
public readonly struct GetExpiryResult
{
    /// <summary>
    /// The expiry type of the key or field.
    /// </summary>
    public ExpiryType ExpiryType { get; }

    /// <summary>
    /// The expiration time, or <see langword="null"/> if the key or field is persistent or does not exist.
    /// </summary>
    public DateTimeOffset? Expiry { get; }

    /// <summary>
    /// Creates an <see cref="GetExpiryResult"/> from the raw millisecond value.
    /// </summary>
    internal GetExpiryResult(long rawMilliseconds)
    {
        switch (rawMilliseconds)
        {
            case -2:
                ExpiryType = ExpiryType.NotFound;
                Expiry = null;
                break;
            case -1:
                ExpiryType = ExpiryType.Persistent;
                Expiry = null;
                break;
            default:
                ExpiryType = ExpiryType.Expiring;
                Expiry = DateTimeOffset.FromUnixTimeMilliseconds(rawMilliseconds);
                break;
        }
    }
}
