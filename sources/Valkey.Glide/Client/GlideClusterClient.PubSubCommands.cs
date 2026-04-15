// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient
{
    #region PublishCommands

    /// <inheritdoc/>
    public async Task<long> SPublishAsync(ValkeyKey shardedChannel, ValkeyValue message)
        => await Command(Request.SPublish(shardedChannel, message));

    #endregion
    #region SubscribeCommands

    /// <inheritdoc/>
    public async Task SSubscribeAsync(ValkeyKey shardedChannel, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.SSubscribeBlocking([shardedChannel], timeout));
    }

    /// <inheritdoc/>
    public async Task SSubscribeAsync(IEnumerable<ValkeyKey> shardedChannels, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.SSubscribeBlocking(shardedChannels.ToGlideStrings(), timeout));
    }

    /// <inheritdoc/>
    public async Task SSubscribeLazyAsync(ValkeyKey shardedChannel)
        => await Command(Request.SSubscribe([shardedChannel]));

    /// <inheritdoc/>
    public async Task SSubscribeLazyAsync(IEnumerable<ValkeyKey> shardedChannels)
        => await Command(Request.SSubscribe(shardedChannels.ToGlideStrings()));

    #endregion
    #region UnsubscribeCommands

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.SUnsubscribeBlocking([], timeout));
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(ValkeyKey shardedChannel, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.SUnsubscribeBlocking([shardedChannel], timeout));
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(IEnumerable<ValkeyKey> shardedChannels, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.SUnsubscribeBlocking(shardedChannels.ToGlideStrings(), timeout));
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeLazyAsync()
        => await Command(Request.SUnsubscribe([]));

    /// <inheritdoc/>
    public async Task SUnsubscribeLazyAsync(ValkeyKey shardedChannel)
        => await Command(Request.SUnsubscribe([shardedChannel]));

    /// <inheritdoc/>
    public async Task SUnsubscribeLazyAsync(IEnumerable<ValkeyKey> shardedChannels)
        => await Command(Request.SUnsubscribe(shardedChannels.ToGlideStrings()));

    #endregion
    #region IntrospectionCommands

    /// <inheritdoc/>
    public async Task<ISet<ValkeyKey>> PubSubShardChannelsAsync()
        => await Command(Request.PubSubShardChannels());

    /// <inheritdoc/>
    public async Task<ISet<ValkeyKey>> PubSubShardChannelsAsync(ValkeyKey pattern)
        => await Command(Request.PubSubShardChannels(pattern));

    /// <inheritdoc/>
    public async Task<Dictionary<ValkeyKey, long>> PubSubShardNumSubAsync(IEnumerable<ValkeyKey> shardedChannels)
        => await Command(Request.PubSubShardNumSub(shardedChannels.ToGlideStrings()));

    #endregion
}
