// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub subscription operations.
/// Tests more subscribe scenarios compared to <see cref="PubSubBasicTests"/>.
/// </summary>
[Collection(typeof(PubSubSubscribeTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubSubscribeTests
{
    [Theory]
    [MemberData(nameof(ClusterChannelAndSubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_SingleChannelMode_SubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode, SubscribeMode subscribeMode)
    {
        // Build many messages for the specified channel mode.
        var messagesCount = 256;
        var messages = Enumerable.Range(0, messagesCount)
            .Select(i => BuildMessage(channelMode))
            .ToArray();

        // Build subscriber using the specified subscribe mode.
        using var subscriber = await BuildSubscriber(isCluster, messages, subscribeMode);
        await AssertSubscribedAsync(subscriber, messages, subscribeMode);

        // Publish messages and verify receipt.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndSubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_AllChannelModes_SubscribesSuccessfully(bool isCluster, SubscribeMode subscribeMode)
    {
        var isSharded = IsShardedSupported(isCluster);

        // Build one message for each channel mode.
        var channelMessage = BuildMessage(PubSubChannelMode.Exact);
        var patternMessage = BuildMessage(PubSubChannelMode.Pattern);
        var shardedChannelMessage = isSharded ? BuildMessage(PubSubChannelMode.Sharded) : null;

        var expectedMessages = new List<PubSubMessage> { channelMessage, patternMessage };
        if (isSharded) expectedMessages.Add(shardedChannelMessage!);

        // Build subscriber using the specified subscribe mode.
        using var subscriber = await BuildSubscriber(isCluster, expectedMessages, subscribeMode);
        await AssertSubscribedAsync(subscriber, expectedMessages, subscribeMode);

        // Publish messages and verify receipt.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, expectedMessages);
        await AssertReceivedAsync(subscriber, expectedMessages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndSubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_AllChannelModes_SubscribesSuccessfully(bool isCluster, SubscribeMode subscribeMode)
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

        // Build subscriber using the specified subscribe mode.
        using var subscriber = await BuildSubscriber(isCluster, messages, subscribeMode);
        await AssertSubscribedAsync(subscriber, messages, subscribeMode);

        // Publish messages and verify receipt.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_AllSubscribeModes_SubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        // Build one message for each subscribe mode.
        var configMessage = BuildMessage(channelMode);
        var lazyMessage = BuildMessage(channelMode);
        var blockingMessage = BuildMessage(channelMode);
        var messages = new[] { configMessage, lazyMessage, blockingMessage };

        // Subscribe to each message using the corresponding subscribe mode.
        using var subscriber = await BuildSubscriber(isCluster, [configMessage], SubscribeMode.Config);
        await AssertSubscribedAsync(subscriber, [configMessage], SubscribeMode.Config);

        await SubscribeAsync(subscriber, blockingMessage);
        await AssertSubscribedAsync(subscriber, [blockingMessage], SubscribeMode.Blocking);

        await SubscribeLazyAsync(subscriber, lazyMessage);
        await AssertSubscribedAsync(subscriber, [lazyMessage], SubscribeMode.Lazy);

        // Publish messages and verify receipt.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_AllSubscribeModes_SubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        // Build many messages for each subscribe mode.
        var messageCount = 128;
        var configMessages = Enumerable.Range(0, messageCount).Select(_ => BuildMessage(channelMode)).ToArray();
        var lazyMessages = Enumerable.Range(0, messageCount).Select(_ => BuildMessage(channelMode)).ToArray();
        var blockingMessages = Enumerable.Range(0, messageCount).Select(_ => BuildMessage(channelMode)).ToArray();

        var messages = new List<PubSubMessage>();
        messages.AddRange(configMessages);
        messages.AddRange(lazyMessages);
        messages.AddRange(blockingMessages);

        // Subscribe to all messages using the corresponding subscribe mode.
        using var subscriber = await BuildSubscriber(isCluster, configMessages, SubscribeMode.Config);
        await AssertSubscribedAsync(subscriber, configMessages, SubscribeMode.Config);

        await SubscribeAsync(subscriber, blockingMessages);
        await AssertSubscribedAsync(subscriber, blockingMessages, SubscribeMode.Blocking);

        await SubscribeLazyAsync(subscriber, lazyMessages);
        await AssertSubscribedAsync(subscriber, lazyMessages, SubscribeMode.Lazy);

        // Publish messages and verify receipt.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);
    }
}
