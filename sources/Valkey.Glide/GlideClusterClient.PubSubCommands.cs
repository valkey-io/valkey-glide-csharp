// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient : IPubSubClusterCommands
{
    #region PublishCommands

    public async Task<long> SPublishAsync(string channel, string message)
        => await Command(Request.SPublish(channel, message));

    #endregion
    #region SubscribeCommands

    public async Task SSubscribeAsync(string channel, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.SSubscribeBlocking([channel], (uint)timeout.TotalMilliseconds));
    }

    public async Task SSubscribeAsync(IEnumerable<string> channels, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.SSubscribeBlocking(channels.ToGlideStrings(), (uint)timeout.TotalMilliseconds));
    }

    public async Task SSubscribeLazyAsync(string channel)
        => await Command(Request.SSubscribe([channel]));

    public async Task SSubscribeLazyAsync(IEnumerable<string> channels)
        => await Command(Request.SSubscribe(channels.ToGlideStrings()));

    #endregion
    #region UnsubscribeCommands

    public async Task SUnsubscribeAsync(TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.SUnsubscribeBlocking([], (uint)timeout.TotalMilliseconds));
    }

    public async Task SUnsubscribeAsync(string channel, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.SUnsubscribeBlocking([channel], (uint)timeout.TotalMilliseconds));
    }

    public async Task SUnsubscribeAsync(IEnumerable<string> channels, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.SUnsubscribeBlocking(channels.ToGlideStrings(), (uint)timeout.TotalMilliseconds));
    }

    public async Task SUnsubscribeLazyAsync()
        => await Command(Request.SUnsubscribe([]));

    public async Task SUnsubscribeLazyAsync(string channel)
        => await Command(Request.SUnsubscribe([channel]));

    public async Task SUnsubscribeLazyAsync(IEnumerable<string> channels)
        => await Command(Request.SUnsubscribe(channels.ToGlideStrings()));

    #endregion
    #region IntrospectionCommands

    public async Task<ISet<string>> PubSubShardChannelsAsync()
        => await Command(Request.PubSubShardChannels());

    public async Task<ISet<string>> PubSubShardChannelsAsync(string pattern)
        => await Command(Request.PubSubShardChannels(pattern));

    public async Task<Dictionary<string, long>> PubSubShardNumSubAsync(IEnumerable<string> channels)
        => await Command(Request.PubSubShardNumSub(channels.ToGlideStrings()));

    #endregion
}
