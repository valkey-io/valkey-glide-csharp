namespace Valkey.Glide;

/// <summary>
/// Specifies the condition under which to set the value and expiry for hash fields.
/// </summary>
/// <seealso href="https://valkey.io/commands/hsetex/"/>
public enum HashSetCondition
{
    /// <summary>
    /// Always set the value and expiry for the fields.
    /// </summary>
    Always,

    /// <summary>
    /// Only set the expiry if none of the fields already exist (FNX).
    /// </summary>
    OnlyIfNoneExist,

    /// <summary>
    /// Only set the expiry if all of the fields already exist (FXX).
    /// </summary>
    OnlyIfAllExist,
}
