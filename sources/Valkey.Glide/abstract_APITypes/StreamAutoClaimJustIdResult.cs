// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System;

namespace Valkey.Glide;

/// <summary>
/// Represents an <c>XAUTOCLAIM</c> response with the <c>JUSTID</c> option.
/// </summary>
/// <seealso href="https://valkey.io/commands/xautoclaim/"/>
public readonly struct StreamAutoClaimJustIdResult
{
    #region Constants

    /// <summary>
    /// A null <see cref="StreamAutoClaimJustIdResult"/>, indicating no results.
    /// </summary>
    public static StreamAutoClaimJustIdResult Null { get; }
        = new StreamAutoClaimJustIdResult(ValkeyValue.Null, Array.Empty<ValkeyValue>(), Array.Empty<ValkeyValue>());

    #endregion
    #region Public Properties


    /// <summary>
    /// Whether this object is null/empty.
    /// </summary>
    public bool IsNull => NextStartId.IsNull
        && ClaimedIds == Array.Empty<ValkeyValue>()
        && DeletedIds == Array.Empty<ValkeyValue>();

    /// <summary>
    /// The stream ID to be used in the next call to StreamAutoClaim.
    /// </summary>
    public ValkeyValue NextStartId { get; }

    /// <summary>
    /// Array of IDs claimed by the command.
    /// </summary>
    public ValkeyValue[] ClaimedIds { get; }

    /// <summary>
    /// Array of message IDs deleted from the stream.
    /// </summary>
    public ValkeyValue[] DeletedIds { get; }

    #endregion
    #region Constructors

    internal StreamAutoClaimJustIdResult(ValkeyValue nextStartId, ValkeyValue[] claimedIds, ValkeyValue[] deletedIds)
    {
        NextStartId = nextStartId;
        ClaimedIds = claimedIds;
        DeletedIds = deletedIds;
    }

    #endregion
}
