// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;

namespace Valkey.Glide;

/// <summary>
/// Result of the XAUTOCLAIM command.
/// </summary>
public readonly struct StreamAutoClaimResult
{
    internal StreamAutoClaimResult(ValkeyValue nextStartId, StreamEntry[] claimedEntries, ValkeyValue[] deletedIds)
    {
        NextStartId = nextStartId;
        ClaimedEntries = claimedEntries;
        DeletedIds = deletedIds;
    }

    /// <summary>
    /// A null StreamAutoClaimResult, indicating no results.
    /// </summary>
    public static StreamAutoClaimResult Null { get; } = new StreamAutoClaimResult(ValkeyValue.Null, Array.Empty<StreamEntry>(), Array.Empty<ValkeyValue>());

    /// <summary>
    /// Whether this object is null/empty.
    /// </summary>
    public bool IsNull => NextStartId.IsNull && ClaimedEntries == Array.Empty<StreamEntry>() && DeletedIds == Array.Empty<ValkeyValue>();

    /// <summary>
    /// The stream ID to be used in the next call to StreamAutoClaim.
    /// </summary>
    public ValkeyValue NextStartId { get; }

    /// <summary>
    /// An array of StreamEntry for the successfully claimed entries.
    /// </summary>
    public StreamEntry[] ClaimedEntries { get; }

    /// <summary>
    /// An array of message IDs deleted from the stream.
    /// </summary>
    public ValkeyValue[] DeletedIds { get; }
}
