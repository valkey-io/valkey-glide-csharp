// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for combined pub/sub subscriptions (channel, pattern, and shard channel).
/// </summary>
[Collection(typeof(PubSubCombinedTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubCombinedTests
{
    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task Combined_OneClient_ReceivesAllTypes(bool isCluster)
    {
        bool isSharded = IsShardedSupported(isCluster);

        // Build expected messages.
        var exactMessage = BuildChannelMessage();
        var patternMessage = BuildPatternMessage();
        var shardChannelMessage = BuildShardChannelMessage();

        var expectedMessages = new List<PubSubMessage> { exactMessage, patternMessage };
        if (isSharded) expectedMessages.Add(shardChannelMessage);

        // Build client with all channel mode subscriptions.
        var channels = new string[] { exactMessage.Channel };
        var patterns = new string[] { patternMessage.Pattern! };
        var shardChannels = isSharded ? new string[] { shardChannelMessage.Channel } : null;

        using var subscriber = await BuildSubscriber(isCluster,
            channels: channels,
            patterns: patterns,
            shardChannels: shardChannels);
        using var publisher = BuildClient(isCluster);

        // Publish messages for all channel modes.
        await publisher.PublishAsync(exactMessage.Channel, exactMessage.Message);
        await publisher.PublishAsync(patternMessage.Channel, patternMessage.Message);
        if (isSharded) await ((GlideClusterClient)publisher).SPublishAsync(shardChannelMessage.Channel, shardChannelMessage.Message);

        // Verify that all messages are received.
        await AssertReceivedAsync(subscriber, expectedMessages);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task Combined_MultipleClients_ReceiveAllTypes(bool isCluster)
    {
        var isSharded = IsShardedSupported(isCluster);

        // Build expected messages.
        var exactMessage = BuildChannelMessage();
        var patternMessage = BuildPatternMessage();
        var shardChannelMessage = BuildShardChannelMessage();

        var expectedMessages = new List<PubSubMessage> { exactMessage, patternMessage };
        if (isSharded) expectedMessages.Add(shardChannelMessage);

        // Build clients with all channel mode subscriptions.
        var channels = new string[] { exactMessage.Channel };
        var patterns = new string[] { patternMessage.Pattern! };
        var shardChannels = isSharded ? new string[] { shardChannelMessage.Channel } : null;

        using var subscriber1 = await BuildSubscriber(isCluster,
            channels: channels,
            patterns: patterns,
            shardChannels: shardChannels);
        using var subscriber2 = await BuildSubscriber(isCluster,
            channels: channels,
            patterns: patterns,
            shardChannels: shardChannels);
        using var publisher = BuildClient(isCluster);

        // Publish messages for all channel modes.
        await publisher.PublishAsync(exactMessage.Channel, exactMessage.Message);
        await publisher.PublishAsync(patternMessage.Channel, patternMessage.Message);
        if (isSharded) await ((GlideClusterClient)publisher).SPublishAsync(shardChannelMessage.Channel, shardChannelMessage.Message);

        // Verify that all messages are received.
        await AssertReceivedAsync(subscriber1, expectedMessages);
        await AssertReceivedAsync(subscriber2, expectedMessages);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task DifferentChannelsWithSameName_ExactPatternSharded_IsolatedCorrectly(bool isCluster)
    {
        bool isSharded = IsShardedSupported(isCluster);

        // Build expected messages that all match the same name.
        var message = "test-message";
        var (channel, pattern) = BuildChannelAndPattern();

        var channelMessage = PubSubMessage.FromChannel(message, channel);
        var patternMessage = PubSubMessage.FromPattern(message, channel, pattern);
        var shardChannelMessage = PubSubMessage.FromShardChannel(message, channel);

        var expectedMessages = new List<PubSubMessage> { channelMessage, patternMessage };
        if (isSharded) expectedMessages.Add(shardChannelMessage);

        // Build client that is subscribed to all channel modes for channel name.
        var channels = new string[] { channel };
        var patterns = new string[] { pattern };
        var shardChannels = isSharded ? new string[] { channel } : null;

        using var subscriber = await BuildSubscriber(isCluster,
            channels: channels,
            patterns: patterns,
            shardChannels: shardChannels);
        using var publisher = BuildClient(isCluster);

        // Publish to channel and shard channel.
        await publisher.PublishAsync(channel, channelMessage.Message);
        if (isSharded) await ((GlideClusterClient)publisher).SPublishAsync(channel, shardChannelMessage.Message);

        // Verify that all messages are received.
        await AssertReceivedAsync(subscriber, expectedMessages);
    }
}
