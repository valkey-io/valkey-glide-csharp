// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub unsubscribe operations.
/// Tests more unsunscribe scenarios compared to <see cref="PubSubBasicTests"/>.
/// </summary>
[Collection(typeof(PubSubUnsubscribeTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubUnsubscribeTests
{
    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_SingleChannelMode_UnsubscribesManySuccessfully(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build many messages for the specified channel mode.
        var messagesCount = 256;
        var messages = Enumerable.Range(0, messagesCount)
            .Select(_ => BuildMessage(channelMode))
            .ToArray();

        // Build subscriber and verify it's active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from all channels/pattern except one and verify subscriptions.
        var unsubscribeMessages = messages[..^1];
        var subscribedMessage = messages[^1];

        await UnsubscribeAsync(subscriber, unsubscribeMessages, unsubscribeMode);
        await AssertNotSubscribedAsync(subscriber, unsubscribeMessages, unsubscribeMode);
        await AssertSubscribedAsync(subscriber, subscribedMessage);

        // Publish messages and verify only the subscribed message is received.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, unsubscribeMessages);
        await AssertReceivedAsync(subscriber, subscribedMessage);
    }

    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task AllSubscriptions_All_SingleChannelMode_UnsubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build messages for the specified channel mode.
        var messageCount = 256;
        var messages = Enumerable.Range(0, messageCount)
            .Select(_ => BuildMessage(channelMode))
            .ToArray();

        // Build subscriber and verify it's active.
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

        // Publish messages and verify they are not received.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task AllSubscriptions_Empty_SingleChannelMode_UnsubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build messages for the specified channel mode.
        var messageCount = 256;
        var messages = Enumerable.Range(0, messageCount)
            .Select(_ => BuildMessage(channelMode))
            .ToArray();

        // Build subscriber and verify it's active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from all and verify subscriptions.
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

        // Publish messages and verify they are not received.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_AllChannelModes_UnsubscribesSuccessfully(bool isCluster, UnsubscribeMode unsubscribeMode)
    {
        var isSharded = IsShardedSupported(isCluster);

        // Build one message for each channel mode.
        var channelMessage = BuildMessage(PubSubChannelMode.Exact);
        var patternMessage = BuildMessage(PubSubChannelMode.Pattern);
        var shardedChannelMessage = isSharded ? BuildMessage(PubSubChannelMode.Sharded) : null;

        var messages = new List<PubSubMessage> { channelMessage, patternMessage };
        if (isSharded) messages.Add(shardedChannelMessage!);

        // Build subscriber and verify it's active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from each channel/pattern and verify subscriptions.
        await UnsubscribeAsync(subscriber, messages, unsubscribeMode);
        await AssertNotSubscribedAsync(subscriber, messages, unsubscribeMode);

        // Publish messages and verify they are not received.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_AllChannelModes_UnsubscribesSuccessfully(bool isCluster, UnsubscribeMode unsubscribeMode)
    {
        var isSharded = IsShardedSupported(isCluster);

        // Build many messages for each channel mode.
        var messagesPerChannelMode = 128;
        var channelMessages = Enumerable.Range(0, messagesPerChannelMode).Select(_ => BuildMessage(PubSubChannelMode.Exact)).ToArray();
        var patternMessages = Enumerable.Range(0, messagesPerChannelMode).Select(_ => BuildMessage(PubSubChannelMode.Pattern)).ToArray();
        var shardedChannelMessages = isSharded ? Enumerable.Range(0, messagesPerChannelMode).Select(_ => BuildMessage(PubSubChannelMode.Sharded)).ToArray() : null;

        var messages = new List<PubSubMessage>();
        messages.AddRange(channelMessages);
        messages.AddRange(patternMessages);
        if (isSharded) messages.AddRange(shardedChannelMessages!);

        // Build subscriber and verify it's active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from each channel/pattern and verify subscriptions.
        await UnsubscribeAsync(subscriber, messages, unsubscribeMode);
        await AssertNotSubscribedAsync(subscriber, messages, unsubscribeMode);

        // Publish messages and verify they are not received.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_AllUnsubscribeModes_UnsubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        // Build one message for each unsubscribe mode.
        var lazyMessage = BuildMessage(channelMode);
        var blockingMessage = BuildMessage(channelMode);
        var messages = new List<PubSubMessage> { lazyMessage, blockingMessage };

        // Build subscriber and verify it's active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from each channel/pattern using the corresponding unsubscribe mode and verify subscriptions.
        await UnsubscribeAsync(subscriber, lazyMessage, UnsubscribeMode.Lazy);
        await AssertNotSubscribedAsync(subscriber, lazyMessage, UnsubscribeMode.Lazy);

        await UnsubscribeAsync(subscriber, blockingMessage, UnsubscribeMode.Blocking);
        await AssertNotSubscribedAsync(subscriber, blockingMessage, UnsubscribeMode.Blocking);

        // Publish messages and verify they are not received.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_AllUnsubscribeModes_UnsubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        // Build many messages for each unsubscribe mode.
        var messageCount = 128;
        var lazyMessages = Enumerable.Range(0, messageCount).Select(i => BuildMessage(channelMode)).ToArray();
        var blockingMessages = Enumerable.Range(0, messageCount).Select(i => BuildMessage(channelMode)).ToArray();

        var messages = new List<PubSubMessage>();
        messages.AddRange(lazyMessages);
        messages.AddRange(blockingMessages);

        // Build subscriber and verify it's active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from all channel/pattern using the corresponding unsubscribe mode and verify subscriptions.
        await UnsubscribeAsync(subscriber, lazyMessages, UnsubscribeMode.Lazy);
        await AssertNotSubscribedAsync(subscriber, lazyMessages, UnsubscribeMode.Lazy);

        await UnsubscribeAsync(subscriber, blockingMessages, UnsubscribeMode.Blocking);
        await AssertNotSubscribedAsync(subscriber, blockingMessages, UnsubscribeMode.Blocking);

        // Publish messages and verify they are not received.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, messages);
    }
}
