namespace Valkey.Glide;

/// <summary>
/// The result of an operation to clear the expiry for a hash field.
/// </summary>
/// <seealso href="https://valkey.io/commands/hpersist/"/>
public enum HashPersistResult
{
    /// <summary>
    /// The expiry was removed from the hash field.
    /// </summary>
    ExpiryRemoved = 1,

    /// <summary>
    /// The hash field exists, but has no expiry set.
    /// </summary>
    NoExpiry = -1,

    /// <summary>
    /// The hash field does not exist.
    /// </summary>
    NoField = -2,
}
