namespace Valkey.Glide;

/// <summary>
/// The result of an operation to clear the expiry for a hash field.
/// </summary>
/// <seealso href="https://valkey.io/commands/hpersist/"/>
public enum PersistResult
{
    /// <summary>
    /// The expiry was removed from the hash key field.
    /// </summary>
    Success = 1,

    /// <summary>
    /// Field exists in the provided hash key, but has no expiration associated with it.
    /// </summary>
    /// <remarks>
    /// This name is inherited from StackExchange.Redis for compatibility,
    /// even though it does not accurately describe this response.
    /// </remarks>
    ConditionNotMet = -1,

    /// <summary>
    /// No such field.
    /// </summary>
    NoSuchField = -2,
}
