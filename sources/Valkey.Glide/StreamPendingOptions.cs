// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Options for XPENDING command.
/// </summary>
public class StreamPendingOptions
{
    /// <summary>
    /// Filter by consumer name.
    /// </summary>
    public ValkeyValue? Consumer { get; set; }

    /// <summary>
    /// Minimum idle time in milliseconds (IDLE).
    /// </summary>
    public long? MinIdleTime { get; set; }
}
