// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.IntegrationTests.PubSubUtils;
using static Valkey.Glide.TestUtils.Data;

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

        await Polling.WaitForTrue(
            () => queue.Count > 0,
            "Expected message was not received.",
            timeout: MaxDuration,
            interval: RetryInterval);

        var received = await queue.GetMessageAsync(TestContext.Current.CancellationToken);
        Assert.Equal(message, received);
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
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

        await Polling.WaitForTrue(
            () => queue!.Count >= messageCount,
            $"Expected {messageCount} messages but only received {queue.Count}.",
            timeout: MaxDuration,
            interval: RetryInterval);

        Assert.Equal(messageCount, queue.Count);
    }
}
