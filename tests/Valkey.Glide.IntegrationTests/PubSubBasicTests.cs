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
    [MemberData(nameof(SubscriptionAndChannelModeOptions), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_ReceivesMessage(bool isCluster, SubscribeMode subscriptionMode, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);
        var messages = new[] { message };

        using var subscriber = await BuildSubscriber(isCluster, channelMode, subscriptionMode, messages);
        await AssertSubscribedAsync(subscriber, channelMode, messages);

        using var publisher = BuildClient(isCluster);
        await PublishMessagesAsync(publisher, channelMode, messages);
        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(SubscriptionAndChannelModeOptions), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_ReceivesAllMessages(bool isCluster, SubscribeMode subscriptionMode, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        using var subscriber = await BuildSubscriber(isCluster, channelMode, subscriptionMode, messages);
        await AssertSubscribedAsync(subscriber, channelMode, messages);

        using var publisher = BuildClient(isCluster);
        await PublishMessagesAsync(publisher, channelMode, messages);
        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(SubscriptionAndChannelModeOptions), MemberType = typeof(PubSubUtils))]
    public static async Task MultipleSubscribers_AllReceiveMessage(bool isCluster, SubscribeMode subMode, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);
        var messages = new[] { message };

        using var subscriber1 = await BuildSubscriber(isCluster, channelMode, subMode, messages);
        await AssertSubscribedAsync(subscriber1, channelMode, messages);

        using var subscriber2 = await BuildSubscriber(isCluster, channelMode, subMode, messages);
        await AssertSubscribedAsync(subscriber2, channelMode, messages);

        using var publisher = BuildClient(isCluster);
        await PublishMessagesAsync(publisher, channelMode, messages);

        await AssertReceivedAsync(subscriber1, messages);
        await AssertReceivedAsync(subscriber2, messages);
    }
}
