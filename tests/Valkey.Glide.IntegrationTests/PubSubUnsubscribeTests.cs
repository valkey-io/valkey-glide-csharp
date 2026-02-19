// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub unsubscribe operations.
/// </summary>
[Collection(typeof(PubSubUnsubscribeTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubUnsubscribeTests
{
    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Unsubscribe_Single_RemovesOne(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build messages.
        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        // Build client and verify subscriptions.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from one and verify subscriptions.
        await UnsubscribeAsync(subscriber, unsubscribeMode, [message1]);
        await AssertNotSubscribedAsync(subscriber, [message1], unsubscribeMode);
        await AssertSubscribedAsync(subscriber, [message2]);
    }

    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Unsubscribe_Multiple_RemovesMultiple(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build messages.
        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var message3 = BuildMessage(channelMode);
        var messages = new[] { message1, message2, message3 };

        // Build client and verify subscriptions.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from multiple and verify subscriptions.
        await UnsubscribeAsync(subscriber, unsubscribeMode, [message1, message2]);
        await AssertNotSubscribedAsync(subscriber, [message1, message2], unsubscribeMode);
        await AssertSubscribedAsync(subscriber, [message3]);
    }

    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Unsubscribe_All_RemovesAll(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build messages.
        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        // Build client and verify subscriptions.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from all and verify subscriptions.
        if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Exact)
            await subscriber.UnsubscribeLazyAsync(PubSub.AllChannels);
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Exact)
            await subscriber.UnsubscribeAsync(PubSub.AllChannels);
        else if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Pattern)
            await subscriber.PUnsubscribeLazyAsync(PubSub.AllPatterns);
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Pattern)
            await subscriber.PUnsubscribeAsync(PubSub.AllPatterns);
        else if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Sharded)
            await ((GlideClusterClient)subscriber).SUnsubscribeLazyAsync(PubSub.AllShardedChannels);
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Sharded)
            await ((GlideClusterClient)subscriber).SUnsubscribeAsync(PubSub.AllShardedChannels);

        await AssertNotSubscribedAsync(subscriber, messages, unsubscribeMode);
    }

    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Unsubscribe_Empty_RemovesAll(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build messages.
        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        // Build client and verify subscriptions.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe with empty collection and verify subscriptions.
        if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Exact)
            await subscriber.UnsubscribeLazyAsync();
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Exact)
            await subscriber.UnsubscribeAsync();
        else if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Pattern)
            await subscriber.PUnsubscribeLazyAsync();
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Pattern)
            await subscriber.PUnsubscribeAsync();
        else if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Sharded)
            await ((GlideClusterClient)subscriber).SUnsubscribeLazyAsync();
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Sharded)
            await ((GlideClusterClient)subscriber).SUnsubscribeAsync();

        await AssertNotSubscribedAsync(subscriber, messages, unsubscribeMode);
    }
}
