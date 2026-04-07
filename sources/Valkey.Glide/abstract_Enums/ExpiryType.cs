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
