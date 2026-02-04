// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;

// Type alias for readability.
using MessageInfo = (Valkey.Glide.ValkeyChannel Channel, Valkey.Glide.ValkeyValue Message);

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for ISubscriber compatibility with <c>StackExchange.Redis</c>.
/// These tests mirror key scenarios from <c>StackExchange.Redis.Tests.PubSubTests</c>.
/// </summary>
[Collection(typeof(ISubscriberCompatibilityTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ISubscriberCompatibilityTests
{
    private static readonly string Message1 = "message1";
    private static readonly string Message2 = "message2";

    // Durations for testing.
    private static readonly TimeSpan SubscribeDelay = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan UnsubscribeDelay = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan AssertRetryInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan AssertTimeout = TimeSpan.FromSeconds(5);

    // Parametrized data to test both standalone and cluster clients.
    public static TheoryData<bool> IsCluster => [true, false];

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Subscribe_WithHandler_ReceivesMessages(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with handler.
        var received = new ConcurrentBag<MessageInfo>();
        await subscriber.SubscribeAsync(channel, (ch, msg) => { received.Add((ch, msg)); });
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(received, [(channel, Message1)]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Subscribe_WithQueue_ReceivesMessages(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with queue.
        var queue = await subscriber.SubscribeAsync(channel);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(queue, [(channel, Message1)]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Subscribe_Pattern_ReceivesMatchingMessages(bool isCluster)
    {
        var pattern = BuildPattern();
        var channel1 = BuildLiteral(pattern);
        var channel2 = BuildLiteral(pattern);
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with handler.
        var received = new ConcurrentBag<MessageInfo>();
        await subscriber.SubscribeAsync(pattern, (ch, msg) => { received.Add((ch, msg)); });
        await Task.Delay(SubscribeDelay);

        // Publish to channels and verify receipt.
        await subscriber.PublishAsync(channel1, Message1);
        await subscriber.PublishAsync(channel2, Message2);
        await AssertMessagesReceived(received, [(pattern, Message1), (pattern, Message2)]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Subscribe_MultipleHandlers_BothReceiveMessages(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with multiple handlers.
        var received1 = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        await subscriber.SubscribeAsync(channel, (ch, msg) => { received1.Add((ch, msg)); });

        var received2 = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        await subscriber.SubscribeAsync(channel, (ch, msg) => { received2.Add((ch, msg)); });

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(received1, [(channel, Message1)]);
        await AssertMessagesReceived(received2, [(channel, Message1)]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Subscribe_MultipleQueues_BothReceiveMessages(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with multiple queues.
        var queue1 = await subscriber.SubscribeAsync(channel);
        var queue2 = await subscriber.SubscribeAsync(channel);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(queue1, [(channel, Message1)]);
        await AssertMessagesReceived(queue2, [(channel, Message1)]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Unsubscribe_WithSpecificHandler_RemovesOnlyThatHandler(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with multiple handlers.
        var received1 = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        void Handler1(ValkeyChannel ch, ValkeyValue msg) { received1.Add((ch, msg)); }
        await subscriber.SubscribeAsync(channel, Handler1);

        var received2 = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        void Handler2(ValkeyChannel ch, ValkeyValue msg) { received2.Add((ch, msg)); }
        await subscriber.SubscribeAsync(channel, Handler2);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(received1, [(channel, Message1)]);
        await AssertMessagesReceived(received2, [(channel, Message1)]);

        // Unsubscribe first handler.
        await subscriber.UnsubscribeAsync(channel, Handler1);
        await Task.Delay(UnsubscribeDelay);

        // Publish again and verify only second handler receives.
        await subscriber.PublishAsync(channel, Message2);
        await AssertMessagesReceived(received1, []);
        await AssertMessagesReceived(received2, [(channel, Message2)]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Unsubscribe_WithoutHandler_RemovesAllHandlers(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with multiple handlers.
        var received1 = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        void Handler1(ValkeyChannel ch, ValkeyValue msg) { received1.Add((ch, msg)); }
        await subscriber.SubscribeAsync(channel, Handler1);

        var received2 = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        void Handler2(ValkeyChannel ch, ValkeyValue msg) { received2.Add((ch, msg)); }
        await subscriber.SubscribeAsync(channel, Handler2);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(received1, [(channel, Message1)]);
        await AssertMessagesReceived(received2, [(channel, Message1)]);

        // Unsubscribe all handlers.
        await subscriber.UnsubscribeAsync(channel);
        await Task.Delay(UnsubscribeDelay);

        // Publish again and verify no handlers receive.
        await subscriber.PublishAsync(channel, Message2);
        await AssertMessagesReceived(received1, []);
        await AssertMessagesReceived(received2, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task UnsubscribeAll_RemovesAllSubscriptions(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with multiple handlers.
        var received1 = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        await subscriber.SubscribeAsync(channel, (ch, msg) => { received1.Add((ch, msg)); });

        var received2 = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        await subscriber.SubscribeAsync(channel, (ch, msg) => { received2.Add((ch, msg)); });

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(received1, [(channel, Message1)]);
        await AssertMessagesReceived(received2, [(channel, Message1)]);

        // Unsubscribe all handlers.
        await subscriber.UnsubscribeAllAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish again and verify no handlers receive.
        await subscriber.PublishAsync(channel, Message2);
        await AssertMessagesReceived(received1, []);
        await AssertMessagesReceived(received2, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Queue_UnsubscribeAsync_StopsReceivingMessages(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with queue.
        var queue = await subscriber.SubscribeAsync(channel);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(queue, [(channel, Message1)]);

        // Unsubscribe the queue.
        await queue.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish again and verify no messages are received.
        await subscriber.PublishAsync(channel, Message2);
        await AssertMessagesReceived(queue, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Queue_UnsubscribeOneOfMultiple_OtherQueuesStillReceive(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with multiple queues.
        var queue1 = await subscriber.SubscribeAsync(channel);
        var queue2 = await subscriber.SubscribeAsync(channel);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(queue1, [(channel, Message1)]);

        // Unsubscribe first queue.
        await queue1.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish again and verify only second queue receives.
        await subscriber.PublishAsync(channel, Message2);
        await AssertMessagesReceived(queue1, []);
        await AssertMessagesReceived(queue2, [(channel, Message2)]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task MixedHandlersAndQueues_UnsubscribeHandler_QueueStillReceives(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with handler and queue.
        var received = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        void Handler1(ValkeyChannel ch, ValkeyValue msg) { received.Add((ch, msg)); }
        await subscriber.SubscribeAsync(channel, Handler1);
        var queue = await subscriber.SubscribeAsync(channel);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(received, [(channel, Message1)]);
        await AssertMessagesReceived(queue, [(channel, Message1)]);

        // Unsubscribe the handler.
        await subscriber.UnsubscribeAsync(channel, Handler1);
        await Task.Delay(UnsubscribeDelay);

        // Publish again and verify queue still receives.
        await subscriber.PublishAsync(channel, Message2);
        await AssertMessagesReceived(received, []);
        await AssertMessagesReceived(queue, [(channel, Message2)]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task MixedHandlersAndQueues_UnsubscribeQueue_HandlerStillReceives(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);

        // Subscribe with handler and queue.
        var received = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        void Handler1(ValkeyChannel ch, ValkeyValue msg) { received.Add((ch, msg)); }
        await subscriber.SubscribeAsync(channel, Handler1);
        var queue = await subscriber.SubscribeAsync(channel);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await subscriber.PublishAsync(channel, Message1);
        await AssertMessagesReceived(received, [(channel, Message1)]);
        await AssertMessagesReceived(queue, [(channel, Message1)]);

        // Unsubscribe the queue.
        await queue.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish again and verify handler still receives.
        await subscriber.PublishAsync(channel, Message2);
        await AssertMessagesReceived(received, [(channel, Message2)]);
        await AssertMessagesReceived(queue, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task MultipleSubscribers_ShareState(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber(isCluster);
        var publisher = await BuildSubscriber(isCluster);

        // Subscribe with handler.
        var received = new ConcurrentBag<(ValkeyChannel, ValkeyValue)>();
        await subscriber.SubscribeAsync(channel, (ch, msg) => { received.Add((ch, msg)); });
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await publisher.PublishAsync(channel, Message1);
        await AssertMessagesReceived(received, [(channel, Message1)]);
    }

    /// <summary>
    /// Builds and returns a new subscriber for testing.
    /// </summary>
    private static async Task<ISubscriber> BuildSubscriber(bool isCluster)
    {
        var address = isCluster ? TestConfiguration.CLUSTER_ADDRESS : TestConfiguration.STANDALONE_ADDRESS;
        var config = ConfigurationOptions.Parse(address.ToString());
        var connection = await ConnectionMultiplexer.ConnectAsync(config);

        return connection.GetSubscriber();
    }

    /// <summary>
    /// Builds and returns a literal channel for testing.
    /// </summary>
    private static ValkeyChannel BuildLiteral()
    {
        var channel = $"test-{Guid.NewGuid()}-channel";
        return ValkeyChannel.Literal(channel);
    }

    /// <summary>
    /// Builds and returns a literal channel for testing that matches the specified pattern.
    /// </summary>
    private static ValkeyChannel BuildLiteral(ValkeyChannel pattern)
    {
        Assert.True(pattern.IsPattern);

        // Replace '*' in pattern with random digits to create a matching literal channel.
        var patternStr = pattern.ToString();
        var literalStr = patternStr.Replace("*", Random.Shared.Next(1000, 9999).ToString());

        return ValkeyChannel.Literal(literalStr);
    }

    /// <summary>
    /// Builds and returns a pattern channel for testing.
    /// </summary>
    private static ValkeyChannel BuildPattern()
    {
        var pattern = $"test-{Guid.NewGuid()}-*";
        return ValkeyChannel.Pattern(pattern);
    }

    /// <summary>
    /// Asserts that the messages with the given channels and values are received by the specified queue.
    /// </summary>
    private async Task AssertMessagesReceived(ChannelMessageQueue queue, List<MessageInfo> expected)
    {
        // If no messages are expected, wait a short duration to ensure no messages are received.
        if (expected.Count == 0)
        {
            await Task.Delay(500);
            Assert.False(queue.TryRead(out _));
            return;
        }

        // Collect received messages until all expected are received or timeout occurs.
        using var cts = new CancellationTokenSource(AssertTimeout);

        List<MessageInfo> received = [];
        while (!cts.Token.IsCancellationRequested)
        {
            var msg = await queue.ReadAsync(cts.Token);
            received.Add((msg.Channel, msg.Message));

            if (received.Count == expected.Count)
            {
                Assert.Equivalent(expected, received);
                return;
            }

            await Task.Delay(AssertRetryInterval);
        }

        Assert.Fail("Expected messages not received within the timeout period.");
    }

    /// <summary>
    /// Asserts that messages with the given channel and value are received by the specified bag.
    /// </summary>
    private async Task AssertMessagesReceived(ConcurrentBag<MessageInfo> actual, List<MessageInfo> expected)
    {
        // If no messages are expected, wait a short duration to ensure no messages are received.
        if (expected.Count == 0)
        {
            await Task.Delay(500);
            Assert.Empty(actual);
            return;
        }

        // Check received messages until all expected are received or timeout occurs.
        using var cts = new CancellationTokenSource(AssertTimeout);

        while (!cts.Token.IsCancellationRequested)
        {
            if (actual.Count >= expected.Count)
            {
                Assert.Equivalent(expected, actual);

                // Clear actual messages that have been asserted.
                actual.Clear();

                return;
            }

            await Task.Delay(AssertRetryInterval);
        }

        Assert.Fail("Expected messages not received within the timeout period.");
    }
}
