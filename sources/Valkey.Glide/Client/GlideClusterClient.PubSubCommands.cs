// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient
{
    /// <inheritdoc cref="IGlideClusterClient.PubSubShardChannelsAsync()"/>
    public async Task<ISet<ValkeyKey>> PubSubShardChannelsAsync()
        => await Command(Request.PubSubShardChannels());

    /// <inheritdoc cref="IGlideClusterClient.PubSubShardChannelsAsync(ValkeyKey)"/>
    public async Task<ISet<ValkeyKey>> PubSubShardChannelsAsync(ValkeyKey pattern)
        => await Command(Request.PubSubShardChannels(pattern));

    /// <inheritdoc cref="IGlideClusterClient.PubSubShardNumSubAsync(IEnumerable{ValkeyKey})"/>
    public async Task<Dictionary<ValkeyKey, long>> PubSubShardNumSubAsync(IEnumerable<ValkeyKey> shardedChannels)
        => await Command(Request.PubSubShardNumSub(shardedChannels.ToGlideStrings()));

    /// <inheritdoc cref="IGlideClusterClient.SPublishAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<long> SPublishAsync(ValkeyKey shardedChannel, ValkeyValue message)
        => await Command(Request.SPublish(shardedChannel, message));

    /// <inheritdoc cref="IGlideClusterClient.SSubscribeAsync(ValkeyKey, TimeSpan)"/>
    public async Task SSubscribeAsync(ValkeyKey shardedChannel, TimeSpan timeout)
        => _ = await Command(Request.SSubscribeBlocking([shardedChannel], timeout));

    /// <inheritdoc cref="IGlideClusterClient.SSubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    public async Task SSubscribeAsync(IEnumerable<ValkeyKey> shardedChannels, TimeSpan timeout)
        => _ = await Command(Request.SSubscribeBlocking(shardedChannels.ToGlideStrings(), timeout));

    /// <inheritdoc cref="IGlideClusterClient.SSubscribeLazyAsync(ValkeyKey)"/>
    public async Task SSubscribeLazyAsync(ValkeyKey shardedChannel)
        => await Command(Request.SSubscribe([shardedChannel]));

    /// <inheritdoc cref="IGlideClusterClient.SSubscribeLazyAsync(IEnumerable{ValkeyKey})"/>
    public async Task SSubscribeLazyAsync(IEnumerable<ValkeyKey> shardedChannels)
        => await Command(Request.SSubscribe(shardedChannels.ToGlideStrings()));

    /// <inheritdoc cref="IGlideClusterClient.SUnsubscribeAsync(TimeSpan)"/>
    public async Task SUnsubscribeAsync(TimeSpan timeout)
        => _ = await Command(Request.SUnsubscribeBlocking([], timeout));

    /// <inheritdoc cref="IGlideClusterClient.SUnsubscribeAsync(ValkeyKey, TimeSpan)"/>
    public async Task SUnsubscribeAsync(ValkeyKey shardedChannel, TimeSpan timeout)
        => _ = await Command(Request.SUnsubscribeBlocking([shardedChannel], timeout));

    /// <inheritdoc cref="IGlideClusterClient.SUnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    public async Task SUnsubscribeAsync(IEnumerable<ValkeyKey> shardedChannels, TimeSpan timeout)
        => _ = await Command(Request.SUnsubscribeBlocking(shardedChannels.ToGlideStrings(), timeout));

    /// <inheritdoc cref="IGlideClusterClient.SUnsubscribeLazyAsync()"/>
    public async Task SUnsubscribeLazyAsync()
        => await Command(Request.SUnsubscribe([]));

    /// <inheritdoc cref="IGlideClusterClient.SUnsubscribeLazyAsync(ValkeyKey)"/>
    public async Task SUnsubscribeLazyAsync(ValkeyKey shardedChannel)
        => await Command(Request.SUnsubscribe([shardedChannel]));

    /// <inheritdoc cref="IGlideClusterClient.SUnsubscribeLazyAsync(IEnumerable{ValkeyKey})"/>
    public async Task SUnsubscribeLazyAsync(IEnumerable<ValkeyKey> shardedChannels)
        => await Command(Request.SUnsubscribe(shardedChannels.ToGlideStrings()));
}
