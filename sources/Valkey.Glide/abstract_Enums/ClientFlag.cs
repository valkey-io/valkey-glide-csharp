// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Client connection flags.
/// </summary>
/// <seealso href="https://valkey.io/commands/client-kill/" />
/// <seealso href="https://valkey.io/commands/client-list/" />
public enum ClientFlag
{
    /// <summary>
    /// Connection to be closed.
    /// </summary>
    CloseAsap = 'A',

    /// <summary>
    /// Client is waiting in a blocking operation.
    /// </summary>
    Blocked = 'b',

    /// <summary>
    /// Connection to be closed after writing entire reply.
    /// </summary>
    CloseAfterReply = 'c',

    /// <summary>
    /// A watched key has been modified.
    /// </summary>
    DirtyExec = 'd',

    /// <summary>
    /// Client is excluded from the client eviction mechanism.
    /// </summary>
    NoEvict = 'e',

    /// <summary>
    /// Client is an import source.
    /// </summary>
    ImportSource = 'I',

    /// <summary>
    /// Client is a primary.
    /// </summary>
    Primary = 'M',

    /// <summary>
    /// No specific flag set.
    /// </summary>
    None = 'N',

    /// <summary>
    /// Client is in <c>MONITOR</c> mode.
    /// </summary>
    Monitor = 'O',

    /// <summary>
    /// Client is a pub/sub subscriber.
    /// </summary>
    PubSub = 'P',

    /// <summary>
    /// Client is in readonly mode against a cluster node.
    /// </summary>
    ReadOnly = 'r',

    /// <summary>
    /// Client tracking target is invalid.
    /// </summary>
    TrackingTargetInvalid = 'R',

    /// <summary>
    /// Client is a replica node connection to this instance.
    /// </summary>
    Replica = 'S',

    /// <summary>
    /// Client enabled keys tracking for client-side caching.
    /// </summary>
    Tracking = 't',

    /// <summary>
    /// Client will not touch the LRU/LFU of keys it accesses.
    /// </summary>
    NoTouch = 'T',

    /// <summary>
    /// Client is unblocked.
    /// </summary>
    Unblocked = 'u',

    /// <summary>
    /// Client is connected via a Unix domain socket.
    /// </summary>
    UnixSocket = 'U',

    /// <summary>
    /// Client is in a <c>MULTI</c>/<c>EXEC</c> context.
    /// </summary>
    Multi = 'x',

    /// <summary>
    /// Client enabled broadcast tracking mode.
    /// </summary>
    BroadcastTracking = 'B',
}
