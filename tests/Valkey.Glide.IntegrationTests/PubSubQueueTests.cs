// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub queue.
/// </summary>
[Collection(typeof(PubSubQueueTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubQueueTests
{
    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task Queue_Channel_ReceivesMessage(bool isCluster)
    {
        var message = BuildChannelMessage();

        using var subscriber = await BuildSubscriber(isCluster, channels: [message.Channel]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message.Channel, message.Message);
        await AssertMessagesReceivedAsync(subscriber, [message]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task Queue_Pattern_ReceivesMessage(bool isCluster)
    {
        var message = BuildPatternMessage();

        using var subscriber = await BuildSubscriber(isCluster, patterns: [message.Pattern!]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(message.Channel, message.Message);
        await AssertMessagesReceivedAsync(subscriber, [message]);
    }

    [Fact]
    public async Task Queue_ShardChannel_ReceivesMessage()
    {
        SkipUnlessShardedSupported();

        var message = BuildShardChannelMessage();

        using var subscriber = await BuildClusterSubscriber(shardChannels: [message.Channel]);
        using var publisher = BuildClusterClient();

        await publisher.SPublishAsync(message.Channel, message.Message);
        await AssertMessagesReceivedAsync(subscriber, [message]);
    }

    [Fact]
    public async Task Queue_WithMultipleMessages_PreservesOrder()
    {
        var channel = BuildChannel();
        var messageCount = 20;
        var expectedMessages = Enumerable.Range(0, messageCount).Select(i => PubSubMessage.FromChannel($"Message-{i:D3}", channel)).ToList();

        using var subscriber = await BuildStandaloneSubscriber(channels: [channel]);
        using var publisher = BuildStandaloneClient();

        // Publish all messages.
        foreach (var msg in expectedMessages)
            await publisher.PublishAsync(msg.Channel, msg.Message);

        // Verify that order is preserved.
        await AssertMessagesReceivedAsync(subscriber, expectedMessages);
    }

    [Fact]
    public async Task Queue_WithHighVolume_HandlesAllMessages()
    {
        var channel = BuildChannel();
        var messageCount = 100;

        using var subscriber = await BuildStandaloneSubscriber(channels: [channel]);
        using var publisher = BuildStandaloneClient();

        // Publish many messages rapidly.
        for (int i = 0; i < messageCount; i++)
            await publisher.PublishAsync(channel, $"Message-{i:D3}");

        // Verify that all messages are received.
        PubSubMessageQueue queue = subscriber.PubSubQueue!;

        using var cts = new CancellationTokenSource(MaxDuration);
        while (!cts.Token.IsCancellationRequested)
        {
            if (queue!.Count >= messageCount) break;
            await Task.Delay(100);
        }

        Assert.Equal(messageCount, queue.Count);
    }
}
