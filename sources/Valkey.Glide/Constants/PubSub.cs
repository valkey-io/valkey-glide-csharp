// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Constants for pub/sub commands.
/// </summary>
public static class PubSub
{
    /// <summary>
    /// Unsubscribes from all channels.
    /// See <see cref="BaseClient.UnsubscribeAsync(IEnumerable{string}, TimeSpan)"/>
    /// and <see cref="BaseClient.UnsubscribeLazyAsync(IEnumerable{string})"/>.
    /// </summary>
    public static readonly IEnumerable<string> AllChannels = [];

    /// <summary>
    /// Unsubscribes from all patterns.
    /// See <see cref="BaseClient.PUnsubscribeAsync(IEnumerable{string}, TimeSpan)"/>
    /// and <see cref="BaseClient.PUnsubscribeLazyAsync(IEnumerable{string})"/>.
    /// </summary>
    public static readonly IEnumerable<string> AllPatterns = [];

    /// <summary>
    /// Unsubscribes from all sharded channels.
    /// See <see cref="GlideClusterClient.SUnsubscribeAsync(IEnumerable{string}, TimeSpan)"/>
    /// and <see cref="GlideClusterClient.SUnsubscribeLazyAsync(IEnumerable{string})"/>.
    /// </summary>
    public static readonly IEnumerable<string> AllShardedChannels = [];
}
