// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for combined pub/sub subscriptions (channel, pattern, and sharded channel).
/// </summary>
[Collection(typeof(PubSubCombinedTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubCombinedTests
{
    [Theory]
    [MemberData(nameof(ClusterAndSubscribeModeData))]
    public static async Task Combined_OneClient_ReceivesAllTypes(bool isCluster, SubscribeMode subscribeMode)
    {
        bool isSharded = IsShardedSupported(isCluster);

        // Build messages.
        var messages = new List<PubSubMessage>
        {
            BuildMessage(PubSubChannelMode.Exact),
            BuildMessage(PubSubChannelMode.Pattern)
        };

        if (isSharded)
            messages.Add(BuildMessage(PubSubChannelMode.Sharded));

        using var subscriber = await BuildSubscriber(isCluster, messages, subscribeMode);

        // Publish messages and verify receipt.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterAndSubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Combined_MultipleClients_ReceiveAllTypes(bool isCluster)
    {
        var isSharded = IsShardedSupported(isCluster);

        // Build messages.
        var messages = new List<PubSubMessage>
        {
            BuildMessage(PubSubChannelMode.Exact),
            BuildMessage(PubSubChannelMode.Pattern)
        };

        if (isSharded)
            messages.Add(BuildMessage(PubSubChannelMode.Sharded));

        using var subscriber1 = await BuildSubscriber(isCluster, messages);
        using var subscriber2 = await BuildSubscriber(isCluster, messages);

        // Publish messages and verify receipt.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);

        await AssertReceivedAsync(subscriber1, messages);
        await AssertReceivedAsync(subscriber2, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task DifferentChannelsWithSameName_ExactPatternSharded_IsolatedCorrectly(bool isCluster)
    {
        bool isSharded = IsShardedSupported(isCluster);

        // Build messages that all use the same name.
        var channel = BuildChannel();
        var message = "message";

        var messages = new List<PubSubMessage>
        {
            PubSubMessage.FromChannel(message, channel),
            PubSubMessage.FromPattern(message, channel, channel)
        };

        if (isSharded)
            messages.Add(PubSubMessage.FromShardedChannel(message, channel));

        using var subscriber = await BuildSubscriber(isCluster, messages);

        // Publish to channel and sharded channel.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);
    }
}
