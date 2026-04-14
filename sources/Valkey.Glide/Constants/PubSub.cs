// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Constants for pub/sub commands.
/// </summary>
public static class PubSub
{
    /// <summary>
    /// Unsubscribes from all channels.
    /// See <see cref="BaseClient.UnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    /// and <see cref="BaseClient.UnsubscribeLazyAsync(IEnumerable{ValkeyKey})"/>.
    /// </summary>
    public static readonly IEnumerable<ValkeyKey> AllChannels = [];

    /// <summary>
    /// Unsubscribes from all patterns.
    /// See <see cref="BaseClient.PUnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    /// and <see cref="BaseClient.PUnsubscribeLazyAsync(IEnumerable{ValkeyKey})"/>.
    /// </summary>
    public static readonly IEnumerable<ValkeyKey> AllPatterns = [];

    /// <summary>
    /// Unsubscribes from all sharded channels.
    /// See <see cref="GlideClusterClient.SUnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    /// and <see cref="GlideClusterClient.SUnsubscribeLazyAsync(IEnumerable{ValkeyKey})"/>.
    /// </summary>
    public static readonly IEnumerable<ValkeyKey> AllShardedChannels = [];
}
