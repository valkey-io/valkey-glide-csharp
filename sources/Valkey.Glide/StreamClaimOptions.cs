// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Options for XCLAIM command.
/// </summary>
public class StreamClaimOptions
{
    /// <summary>
    /// Set the idle time (in milliseconds) of the message (IDLE).
    /// </summary>
    public long? Idle { get; set; }

    /// <summary>
    /// Set the idle time to a specific Unix time (in milliseconds) (TIME).
    /// </summary>
    public long? Time { get; set; }

    /// <summary>
    /// Set the retry counter to the specified value (RETRYCOUNT).
    /// </summary>
    public long? RetryCount { get; set; }

    /// <summary>
    /// Creates the pending message entry even if certain IDs are not already in the PEL (FORCE).
    /// </summary>
    public bool Force { get; set; }

    /// <summary>
    /// Return just the message IDs without the full message data (JUSTID).
    /// </summary>
    public bool JustId { get; set; }

    /// <summary>
    /// Update the last ID of the consumer group to the specified ID (LASTID).
    /// </summary>
    public ValkeyValue? LastId { get; set; }
}
