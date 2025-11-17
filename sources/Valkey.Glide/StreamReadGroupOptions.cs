// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Options for XREADGROUP command.
/// </summary>
public class StreamReadGroupOptions
{
    /// <summary>
    /// Maximum number of entries to return per stream.
    /// </summary>
    public int? Count { get; set; }

    /// <summary>
    /// Maximum time to block waiting for entries (in milliseconds). Use null for non-blocking.
    /// </summary>
    public int? Block { get; set; }

    /// <summary>
    /// If true, messages are not added to the pending entries list (NOACK).
    /// </summary>
    public bool NoAck { get; set; }
}
