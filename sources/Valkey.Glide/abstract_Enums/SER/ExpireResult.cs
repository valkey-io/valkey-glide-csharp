namespace Valkey.Glide;

/// <summary>
/// The result of an operation to set the expiry for a hash field.
/// </summary>
/// <seealso href="https://valkey.io/commands/hexpire/"/>
public enum ExpireResult
{
    /// <summary>
    /// Field deleted because the specified expiration time is due.
    /// </summary>
    Due = 2,

    /// <summary>
    /// The expiry was set on the hash field.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Field exists in the provided hash key, but the condition was not met.
    /// </summary>
    /// <remarks>
    /// This name is inherited from StackExchange.Redis for compatibility,
    /// even though it does not accurately describe this response.
    /// </remarks>
    ConditionNotMet = 0,

    /// <summary>
    /// No such field.
    /// </summary>
    NoSuchField = -2,
}
