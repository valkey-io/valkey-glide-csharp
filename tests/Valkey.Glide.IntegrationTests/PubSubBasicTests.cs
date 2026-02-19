// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for basic pub/sub subscription and message delivery.
/// </summary>
[Collection(typeof(PubSubBasicTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubBasicTests
{
    [Theory]
    [MemberData(nameof(AllModesData), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_ReceivesMessage(bool isCluster, PubSubChannelMode channelMode, SubscribeMode subscribeMode, UnsubscribeMode unsubscribeMode)
    {
        var message = BuildMessage(channelMode);

        // Build client and verify subscription.
        using var subscriber = await BuildSubscriber(isCluster, message, subscribeMode);
        await AssertSubscribedAsync(subscriber, message, subscribeMode);

        using var publisher = BuildPublisher(isCluster);

        // Publish messages and verify receipt.
        await PublishAsync(publisher, message);
        await AssertReceivedAsync(subscriber, message);

        // Unsubscribe and verify unsubscription.
        await UnsubscribeAsync(subscriber, unsubscribeMode, message);
        await AssertNotSubscribedAsync(subscriber, message, unsubscribeMode);

        // Publish messages again and verify they are not received.
        await PublishAsync(publisher, message);
        await AssertNotReceivedAsync(subscriber, message);
    }

    [Theory]
    [MemberData(nameof(AllModesData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_ReceivesAllMessages(bool isCluster, PubSubChannelMode channelMode, SubscribeMode subscribeMode, UnsubscribeMode unsubscribeMode)
    {
        const int messageCount = 256;
        var messages = Enumerable.Range(0, messageCount).Select(_ => BuildMessage(channelMode)).ToArray();

        // Build client and verify subscription.
        using var subscriber = await BuildSubscriber(isCluster, messages, subscribeMode);
        await AssertSubscribedAsync(subscriber, messages, subscribeMode);

        using var publisher = BuildPublisher(isCluster);

        // Publish messages and verify receipt.
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);

        // Unsubscribe and verify unsubscription.
        await UnsubscribeAsync(subscriber, unsubscribeMode, messages);
        await AssertNotSubscribedAsync(subscriber, messages, unsubscribeMode);

        // Publish messages again and verify they are not received.
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(AllModesData), MemberType = typeof(PubSubUtils))]
    public static async Task MultipleSubscribers_AllReceiveMessage(bool isCluster, PubSubChannelMode channelMode, SubscribeMode subscribeMode, UnsubscribeMode unsubscribeMode)
    {
        var message = BuildMessage(channelMode);

        // Build clients and verify subscriptions.
        using var subscriber1 = await BuildSubscriber(isCluster, message, subscribeMode);
        await AssertSubscribedAsync(subscriber1, message, subscribeMode);

        using var subscriber2 = await BuildSubscriber(isCluster, message, subscribeMode);
        await AssertSubscribedAsync(subscriber2, message, subscribeMode);

        using var publisher = BuildPublisher(isCluster);

        // Publish message and verify receipt.
        await PublishAsync(publisher, message);
        await AssertReceivedAsync(subscriber1, message);
        await AssertReceivedAsync(subscriber2, message);

        // Unsubscribe and verify unsubscription.
        await UnsubscribeAsync(subscriber1, unsubscribeMode, message);
        await AssertNotSubscribedAsync(subscriber1, message, unsubscribeMode);

        await UnsubscribeAsync(subscriber2, unsubscribeMode, message);
        await AssertNotSubscribedAsync(subscriber2, message, unsubscribeMode);

        // Publish message again and verify it is not received.
        await PublishAsync(publisher, message);
        await AssertNotReceivedAsync(subscriber1, message);
        await AssertNotReceivedAsync(subscriber2, message);
    }
}
