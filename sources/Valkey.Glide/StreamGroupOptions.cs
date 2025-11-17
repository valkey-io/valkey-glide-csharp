// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Options for XGROUP CREATE command.
/// </summary>
public class StreamGroupOptions
{
    /// <summary>
    /// If true, creates the stream if it doesn't exist (MKSTREAM).
    /// </summary>
    public bool CreateStream { get; set; }

    /// <summary>
    /// Valkey 7.0+: Sets the entries_read counter to an arbitrary value.
    /// </summary>
    public long? EntriesRead { get; set; }
}
