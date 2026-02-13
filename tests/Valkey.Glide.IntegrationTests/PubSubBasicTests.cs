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
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ChannelSubscription_SingleChannel_ReceivesMessage(bool isCluster)
    {
        var message = BuildChannelMessage();

        using var subscriber = await BuildSubscriber(isCluster, channels: [message.Channel]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message.Channel, message.Message);

        await AssertMessagesReceivedAsync(subscriber, [message]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ChannelSubscription_ManyChannels_ReceivesAllMessages(bool isCluster)
    {
        var message1 = BuildChannelMessage();
        var message2 = BuildChannelMessage();

        using var subscriber = await BuildSubscriber(isCluster, channels: [message1.Channel, message2.Channel]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message1.Channel, message1.Message);
        await publisher.PublishAsync(message2.Channel, message2.Message);

        await AssertMessagesReceivedAsync(subscriber, [message1, message2]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ChannelSubscription_MultipleSubscribers_AllReceiveMessage(bool isCluster)
    {
        var message = BuildChannelMessage();

        using var subscriber1 = await BuildSubscriber(isCluster, channels: [message.Channel]);
        using var subscriber2 = await BuildSubscriber(isCluster, channels: [message.Channel]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message.Channel, message.Message);

        await AssertMessagesReceivedAsync(subscriber1, [message]);
        await AssertMessagesReceivedAsync(subscriber2, [message]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PatternSubscription_SinglePattern_ReceivesMatchingMessages(bool isCluster)
    {
        var message = BuildPatternMessage();

        using var subscriber = await BuildSubscriber(isCluster, patterns: [message.Pattern!]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message.Channel, message.Message);

        await AssertMessagesReceivedAsync(subscriber, [message]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PatternSubscription_ManyPatterns_ReceivesAllMatchingMessages(bool isCluster)
    {
        var message1 = BuildPatternMessage();
        var message2 = BuildPatternMessage();

        using var subscriber = await BuildSubscriber(isCluster, patterns: [message1.Pattern!, message2.Pattern!]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message1.Channel, message1.Message);
        await publisher.PublishAsync(message2.Channel, message2.Message);

        await AssertMessagesReceivedAsync(subscriber, [message1, message2]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PatternSubscription_MultipleSubscribers_AllReceiveMessage(bool isCluster)
    {
        var message = BuildPatternMessage();

        using var subscriber1 = await BuildSubscriber(isCluster, patterns: [message.Pattern!]);
        using var subscriber2 = await BuildSubscriber(isCluster, patterns: [message.Pattern!]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message.Channel, message.Message);

        await AssertMessagesReceivedAsync(subscriber1, [message]);
        await AssertMessagesReceivedAsync(subscriber2, [message]);
    }

    [Fact]
    public async Task ShardChannelSubscription_SingleChannel_ReceivesMessage()
    {
        Assert.SkipUnless(IsShardedSupported(), SkipShardedPubSubMessage);

        var message = BuildShardChannelMessage();

        using var subscriber = await BuildClusterSubscriber(shardChannels: [message.Channel]);
        using var publisher = await GlideClusterClient.CreateClient(TestConfiguration.DefaultClusterClientConfig().Build());

        await publisher.SPublishAsync(message.Channel, message.Message);

        await AssertMessagesReceivedAsync(subscriber, [message]);
    }

    [Fact]
    public async Task ShardChannelSubscription_ManyChannels_ReceivesAllMessages()
    {
        Assert.SkipUnless(IsShardedSupported(), SkipShardedPubSubMessage);

        var message1 = BuildShardChannelMessage();
        var message2 = BuildShardChannelMessage();

        using var subscriber = await BuildClusterSubscriber(shardChannels: [message1.Channel, message2.Channel]);
        using var publisher = await GlideClusterClient.CreateClient(TestConfiguration.DefaultClusterClientConfig().Build());

        await publisher.SPublishAsync(message1.Channel, message1.Message);
        await publisher.SPublishAsync(message2.Channel, message2.Message);

        await AssertMessagesReceivedAsync(subscriber, [message1, message2]);
    }

    [Fact]
    public async Task ShardChannelSubscription_MultipleSubscribers_AllReceiveMessage()
    {
        Assert.SkipUnless(IsShardedSupported(), SkipShardedPubSubMessage);

        var message = BuildShardChannelMessage();

        using var subscriber1 = await BuildClusterSubscriber(shardChannels: [message.Channel]);
        using var subscriber2 = await BuildClusterSubscriber(shardChannels: [message.Channel]);
        using var publisher = BuildClusterClient();

        await publisher.SPublishAsync(message.Channel, message.Message);

        await AssertMessagesReceivedAsync(subscriber1, [message]);
        await AssertMessagesReceivedAsync(subscriber2, [message]);
    }
}
