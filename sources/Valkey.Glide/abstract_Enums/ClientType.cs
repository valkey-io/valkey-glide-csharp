namespace Valkey.Glide;

/// <summary>
/// The client connection type.
/// </summary>
/// <seealso href="https://valkey.io/commands/client-kill/" />
/// <seealso href="https://valkey.io/commands/client-list/" />
public enum ClientType
{
    /// <summary>
    /// Normal client connection.
    /// </summary>
    Normal,

    /// <summary>
    /// Primary connection.
    /// </summary>
    Primary,

    /// <summary>
    /// Replica node connection.
    /// </summary>
    Replica,

    /// <summary>
    /// Pub/sub subscriber connection.
    /// </summary>
    PubSub,
}
