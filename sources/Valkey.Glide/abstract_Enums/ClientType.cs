// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
    Normal = 0,

    /// <summary>
    /// Replica node connection.
    /// </summary>
    Replica = 1,

    /// <summary>
    /// Pub/sub subscriber connection.
    /// </summary>
    PubSub = 2,

    /// <summary>
    /// Primary connection.
    /// </summary>
    Primary = 3,
}
