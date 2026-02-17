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
    [MemberData(nameof(SubscriptionData), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_ReceivesMessage(bool isCluster, PubSubChannelMode channelMode, SubscriptionMode subscriptionMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);

        using var subscriber = await BuildSubscriber(isCluster, channelMode, subscriptionMode, [message]);
        using var publisher = BuildClient(isCluster);

        await PublishMessageAsync(publisher, channelMode, message);
        await AssertMessagesReceivedAsync(subscriber, [message]);
    }

    [Theory]
    [MemberData(nameof(SubscriptionData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_ReceivesAllMessages(bool isCluster, PubSubChannelMode channelMode, SubscriptionMode subscriptionMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);

        using var subscriber = await BuildSubscriber(isCluster, channelMode, subscriptionMode, [message1, message2]);
        using var publisher = BuildClient(isCluster);

        await PublishMessageAsync(publisher, channelMode, message1);
        await PublishMessageAsync(publisher, channelMode, message2);
        await AssertMessagesReceivedAsync(subscriber, [message1, message2]);
    }

    [Theory]
    [MemberData(nameof(SubscriptionData), MemberType = typeof(PubSubUtils))]
    public static async Task MultipleSubscribers_AllReceiveMessage(bool isCluster, PubSubChannelMode channelMode, SubscriptionMode subMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);

        using var subscriber1 = await BuildSubscriber(isCluster, channelMode, subMode, [message]);
        using var subscriber2 = await BuildSubscriber(isCluster, channelMode, subMode, [message]);
        using var publisher = BuildClient(isCluster);

        await PublishMessageAsync(publisher, channelMode, message);

        await AssertMessagesReceivedAsync(subscriber1, [message]);
        await AssertMessagesReceivedAsync(subscriber2, [message]);
    }
}
