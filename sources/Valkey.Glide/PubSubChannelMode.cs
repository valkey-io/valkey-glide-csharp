// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Pub/sub channel subscription mode.
/// </summary>
public enum PubSubChannelMode
{
    /// <summary>
    /// Exact channel name subscription.
    /// </summary>
    Exact = 0,

    /// <summary>
    /// Pattern-based subscription.
    /// </summary>
    Pattern = 1,

    /// <summary>
    /// Shard channel subscription.
    /// </summary>
    Sharded = 2
}
