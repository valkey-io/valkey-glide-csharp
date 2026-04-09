namespace Valkey.Glide;

/// <summary>
/// The result of an operation to set the expiry for a hash field.
/// </summary>
/// <seealso href="https://valkey.io/commands/hexpire/"/>
public enum HashExpireResult
{
    /// <summary>
    /// The hash field was deleted because the expiry is in the past.
    /// </summary>
    Deleted = 2,

    /// <summary>
    /// The expiry was set on the hash field.
    /// </summary>
    ExpirySet = 1,

    /// <summary>
    /// The expiry condition was not met.
    /// </summary>
    ConditionNotMet = 0,

    /// <summary>
    /// The hash field does not exist.
    /// </summary>
    NoField = -2,
}
