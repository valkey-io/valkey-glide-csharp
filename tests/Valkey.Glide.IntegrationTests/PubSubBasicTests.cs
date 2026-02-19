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
    /// <summary>
    /// Theory data for basic tests that covers all valid combinations of cluster mode, channel mode, subscribe mode, and unsubscribe mode.
    /// </summary>
    public static TheoryData<bool, PubSubChannelMode, SubscribeMode, UnsubscribeMode> BasicTestsData
    {
        get
        {
            var data = new TheoryData<bool, PubSubChannelMode, SubscribeMode, UnsubscribeMode>();

            foreach (var isCluster in ClusterModeData)
            {
                foreach (var channelMode in ChannelModeData)
                {
                    if (!IsChannelModeSupported(isCluster, channelMode))
                        continue;

                    foreach (var subscribeMode in SubscribeModeData)
                    {
                        foreach (var unsubscribeMode in UnsubscribeModeData)
                            data.Add(isCluster, channelMode, subscribeMode, unsubscribeMode);
                    }
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(BasicTestsData))]
    public static async Task SingleSubscription_ReceivesMessage(bool isCluster, PubSubChannelMode channelMode, SubscribeMode subscriptionMode, UnsubscribeMode unsubscribeMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);

        // Build client and verify subscription.
        using var subscriber = await BuildSubscriber(isCluster, message, subscriptionMode);
        await AssertSubscribedAsync(subscriber, message);

        using var publisher = BuildPublisher(isCluster);

        // Publish messages and verify receipt.
        await PublishAsync(publisher, message);
        await AssertReceivedAsync(subscriber, message);

        // Unsubscribe and verify unsubscription.
        await UnsubscribeAsync(subscriber, unsubscribeMode, message);
        await AssertNotSubscribedAsync(subscriber, message);

        // Publish messages again and verify they are not received.
        await PublishAsync(publisher, message);
        await AssertNotReceivedAsync(subscriber, message);
    }

    [Theory]
    [MemberData(nameof(BasicTestsData))]
    public static async Task ManySubscriptions_ReceivesAllMessages(bool isCluster, PubSubChannelMode channelMode, SubscribeMode subscriptionMode, UnsubscribeMode unsubscribeMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        const int messageCount = 256;
        var messages = Enumerable.Range(0, messageCount).Select(_ => BuildMessage(channelMode)).ToArray();

        // Build client and verify subscription.
        using var subscriber = await BuildSubscriber(isCluster, messages, subscriptionMode);
        await AssertSubscribedAsync(subscriber, messages);

        using var publisher = BuildPublisher(isCluster);

        // Publish messages and verify receipt.
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);

        // Unsubscribe and verify unsubscription.
        await UnsubscribeAsync(subscriber, unsubscribeMode, messages);
        await AssertNotSubscribedAsync(subscriber, messages);

        // Publish messages again and verify they are not received.
        await PublishAsync(publisher, messages);
        await AssertNotReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(BasicTestsData))]
    public static async Task MultipleSubscribers_AllReceiveMessage(bool isCluster, PubSubChannelMode channelMode, SubscribeMode subscribeMode, UnsubscribeMode unsubscribeMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);

        // Build clients and verify subscriptions.
        const int numSubscribers = 10;

        var subscribers = new List<BaseClient>();
        for (int i = 0; i < numSubscribers; i++)
        {
            subscribers.Add(await BuildSubscriber(isCluster, message, subscribeMode));

            // Add a short delay to avoid overloading the connection multiplexer.
            await Task.Delay(100);
        }

        using var publisher = BuildPublisher(isCluster);

        // Publish message and verify receipt.
        await PublishAsync(publisher, message);

        foreach (var subscriber in subscribers)
            await AssertReceivedAsync(subscriber, message);

        // Unsubscribe and verify unsubscription.
        foreach (var subscriber in subscribers)
        {
            await UnsubscribeAsync(subscriber, unsubscribeMode, message);
            await AssertNotSubscribedAsync(subscriber, message);
        }

        // Publish message again and verify it is not received.
        await PublishAsync(publisher, message);

        foreach (var subscriber in subscribers)
            await AssertNotReceivedAsync(subscriber, message);
    }
}
