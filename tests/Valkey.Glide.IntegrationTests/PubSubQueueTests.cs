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
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Queue_Channel_ReceivesMessage(bool isCluster, PubSubChannelMode channelMode)
    {
        var message = BuildMessage(channelMode);
        using var subscriber = await BuildSubscriber(isCluster, message);

        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, message);

        // Verify that message is received.
        PubSubMessageQueue queue = subscriber.PubSubQueue!;

        using var cts = new CancellationTokenSource(MaxDuration);
        while (!cts.Token.IsCancellationRequested)
        {
            if (queue.Count > 0) break;
            await Task.Delay(RetryInterval);
        }

        var received = await queue.GetMessageAsync(cts.Token);
        Assert.Equal(message, received);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task Queue_WithHighVolume_HandlesAllMessages(bool isCluster)
    {
        var channel = BuildChannel();

        var messageCount = 100;
        var messages = Enumerable.Range(0, messageCount).Select(i => PubSubMessage.FromChannel($"Message-{i:D3}", channel)).ToArray();

        using var subscriber = await BuildSubscriber(isCluster, messages);

        // Publish many messages rapidly.
        using var publisher = BuildPublisher(isCluster);
        await PublishAsync(publisher, messages);

        // Verify that all messages are received.
        PubSubMessageQueue queue = subscriber.PubSubQueue!;

        using var cts = new CancellationTokenSource(MaxDuration);
        while (!cts.Token.IsCancellationRequested)
        {
            if (queue!.Count >= messageCount) break;
            await Task.Delay(RetryInterval);
        }

        Assert.Equal(messageCount, queue.Count);
    }
}
