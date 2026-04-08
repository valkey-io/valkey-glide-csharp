namespace Valkey.Glide;

/// <summary>
/// Represents the result of an operation to clear the expiry for a hash field.
/// </summary>
/// <seealso href="https://valkey.io/commands/hpersist/"/>
public enum HashPersistResult
{
    /// <summary>
    /// The expiry was removed from the hash key field.
    /// </summary>
    ExpiryRemoved = 1,

    /// <summary>
    /// Field exists in the provided hash key, but has no expiration associated with it.
    /// </summary>
    NoExpiry = -1,

    /// <summary>
    /// The field does not exist in the provided hash key, or the hash key does not exist.
    /// </summary>
    NoField = -2,
}
