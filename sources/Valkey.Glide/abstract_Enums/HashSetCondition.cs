namespace Valkey.Glide;

/// <summary>
/// The condition for an operation to set the value and expiry for hash fields.
/// </summary>
/// <seealso href="https://valkey.io/commands/hsetex/"/>
public enum HashSetCondition
{
    /// <summary>
    /// Always set the value and expiry for the hash fields.
    /// </summary>
    Always,

    /// <summary>
    /// Only set the expiry if none of the hash fields exist (FNX).
    /// </summary>
    OnlyIfNoneExist,

    /// <summary>
    /// Only set the expiry if all of the hash fields exist (FXX).
    /// </summary>
    OnlyIfAllExist,
}
