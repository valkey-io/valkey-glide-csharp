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
    // Skip tests if Valkey GLIDE version is less than 7.0.0
    private static readonly bool IsSharedPubSubSupported = TestConfiguration.IsVersionAtLeast("7.0.0");
    private static readonly string SkipSharedPubSubMessage = "Sharded PubSub is supported since 7.0.0";

    // Messages for testing.
    private static readonly ValkeyValue Message1 = "message1";
    private static readonly ValkeyValue Message2 = "message2";

    // Durations for testing.
    private static readonly TimeSpan SubscribeDelay = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan UnsubscribeDelay = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan AssertRetryInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan AssertTimeout = TimeSpan.FromSeconds(5);

    // Parametrized data to test both standalone and cluster clients.
    public static TheoryData<bool> IsCluster => [true, false];

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Literal_Subscribe_Handler(bool isCluster)
    {
        var literal = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with handler.
        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(literal, handler);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await publisher.PublishAsync(literal, Message1);
        await AssertHandlerReceives(received, [(literal, Message1)]);

        // Unsubscribe handler.
        await subscriber.UnsubscribeAsync(literal, handler);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify no messages received.
        await publisher.PublishAsync(literal, Message2);
        await AssertHandlerReceives(received, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Literal_Subscribe_Queue(bool isCluster)
    {
        var literal = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with queue.
        var queue = await subscriber.SubscribeAsync(literal);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await publisher.PublishAsync(literal, Message1);
        await AssertQueueReceives(queue, [(literal, Message1)]);

        // Unsubscribe queue.
        await queue.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify no messages received.
        await publisher.PublishAsync(literal, Message2);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Pattern_Subscribe_Handler(bool isCluster)
    {
        var pattern = BuildPattern();
        var channel = BuildLiteralForPattern(pattern);
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with handler.
        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(pattern, handler);
        await Task.Delay(SubscribeDelay);

        // Publish to matching channel and verify receipt.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(pattern, Message1)]);

        // Unsubscribe handler.
        await subscriber.UnsubscribeAsync(pattern, handler);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify no messages received.
        await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task Pattern_Subscribe_Queue(bool isCluster)
    {
        var pattern = BuildPattern();
        var channel = BuildLiteralForPattern(pattern);
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with queue.
        var queue = await subscriber.SubscribeAsync(pattern);
        await Task.Delay(SubscribeDelay);

        // Publish to matching channel and verify receipt.
        await publisher.PublishAsync(channel, Message1);
        await AssertQueueReceives(queue, [(pattern, Message1)]);

        // Unsubscribe queue.
        await queue.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify no messages received.
        await publisher.PublishAsync(channel, Message2);
        await AssertQueueReceives(queue, []);
    }

    [Fact]
    public async Task Sharded_Subscribe_Handler()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        // Only supported in cluster mode.
        bool isCluster = true;

        var channel = BuildSharded();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with handler.
        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);

        // Unsubscribe handler.
        await subscriber.UnsubscribeAsync(channel, handler);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify no messages received.
        await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
    }

    [Fact]
    public async Task Sharded_Subscribe_Queue()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        // Only supported in cluster mode.
        bool isCluster = true;

        var channel = BuildSharded();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with queue.
        var queue = await subscriber.SubscribeAsync(channel);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify receipt.
        await publisher.PublishAsync(channel, Message1);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        // Unsubscribe queue.
        await queue.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify no messages received.
        await publisher.PublishAsync(channel, Message2);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task MultipleHandlers(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with multiple handlers.
        var received1 = new ConcurrentBag<MessageInfo>();
        var handler1 = BuildHandler(received1);
        await subscriber.SubscribeAsync(channel, handler1);

        var received2 = new ConcurrentBag<MessageInfo>();
        var handler2 = BuildHandler(received2);
        await subscriber.SubscribeAsync(channel, handler2);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify both handlers receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received1, [(channel, Message1)]);
        await AssertHandlerReceives(received2, [(channel, Message1)]);

        // Unsubscribe first handler.
        await subscriber.UnsubscribeAsync(channel, handler1);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify only second handler receives.
        await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received1, []);
        await AssertHandlerReceives(received2, [(channel, Message2)]);

        // Unsubscribe second handler.
        await subscriber.UnsubscribeAsync(channel, handler2);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify no handlers receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received1, []);
        await AssertHandlerReceives(received2, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task MultipleQueues(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with multiple queues.
        var queue1 = await subscriber.SubscribeAsync(channel);
        var queue2 = await subscriber.SubscribeAsync(channel);
        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify both queues receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertQueueReceives(queue1, [(channel, Message1)]);
        await AssertQueueReceives(queue2, [(channel, Message1)]);

        // Unsubscribe first queue.
        await queue1.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify only second queue receives.
        await publisher.PublishAsync(channel, Message2);
        await AssertQueueReceives(queue1, []);
        await AssertQueueReceives(queue2, [(channel, Message2)]);

        // Unsubscribe second queue.
        await queue2.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify no queues receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertQueueReceives(queue1, []);
        await AssertQueueReceives(queue2, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task QueueAndHandler_UnsubscribeHandler(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with handler and queue.
        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        var queue = await subscriber.SubscribeAsync(channel);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify both receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        // Unsubscribe the handler.
        await subscriber.UnsubscribeAsync(channel, handler);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify only queue receives.
        await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, [(channel, Message2)]);

        // Unsubscribe the queue.
        await queue.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify neither receives.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task QueueAndHandler_UnsubscribeQueue(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with handler and queue.
        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        var queue = await subscriber.SubscribeAsync(channel);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify both receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        // Unsubscribe the queue.
        await queue.UnsubscribeAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify only handler receives.
        await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, [(channel, Message2)]);
        await AssertQueueReceives(queue, []);

        // Unsubscribe the handler.
        await subscriber.UnsubscribeAsync(channel, handler);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify neither receives.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task QueueAndHandler_UnsubscribeChannel(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe with handler and queue.
        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        var queue = await subscriber.SubscribeAsync(channel);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify both receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        // Unsubscribe all subscriptions on the channel.
        await subscriber.UnsubscribeAsync(channel);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify neither receives.
        await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task QueueAndHandler_UnsubscribePattern(bool isCluster)
    {
        var pattern = BuildPattern();
        var channel = BuildLiteralForPattern(pattern);
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe to pattern with handler and queue.
        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(pattern, handler);
        var queue = await subscriber.SubscribeAsync(pattern);

        await Task.Delay(SubscribeDelay);

        // Publish to matching channel and verify both receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(pattern, Message1)]);
        await AssertQueueReceives(queue, [(pattern, Message1)]);

        // Unsubscribe all subscriptions on the pattern.
        await subscriber.UnsubscribeAsync(pattern);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify neither receives.
        await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Fact]
    public async Task QueueAndHandler_UnsubscribeSharded()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        // Only supported in cluster mode.
        bool isCluster = true;

        var channel = BuildSharded();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe to sharded channel with handler and queue.
        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        var queue = await subscriber.SubscribeAsync(channel);

        await Task.Delay(SubscribeDelay);

        // Publish to channel and verify both receive.
        await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        // Unsubscribe all subscriptions on the sharded channel.
        await subscriber.UnsubscribeAsync(channel);
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify neither receives.
        await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task QueueAndHandler_UnsubscribeAll(bool isCluster)
    {
        var literalChannel = BuildLiteral();
        var pattern = BuildPattern();
        var patternChannel = BuildLiteralForPattern(pattern);
        var shardedChannel = BuildSharded();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        // Subscribe to channel with handler and queue.
        var literalReceived = new ConcurrentBag<MessageInfo>();
        var literalHandler = BuildHandler(literalReceived);
        await subscriber.SubscribeAsync(literalChannel, literalHandler);
        var literalQueue = await subscriber.SubscribeAsync(literalChannel);

        // Subscribe to pattern with handler and queue.
        var patternReceived = new ConcurrentBag<MessageInfo>();
        var patternHandler = BuildHandler(patternReceived);
        await subscriber.SubscribeAsync(pattern, patternHandler);
        var patternQueue = await subscriber.SubscribeAsync(pattern);

        // Subscribe to sharded channel with handler and queue (cluster only).
        ConcurrentBag<MessageInfo> shardedReceived = [];
        var shardedHandler = BuildHandler(shardedReceived);
        ChannelMessageQueue? shardedQueue = null;

        if (isCluster && IsSharedPubSubSupported)
        {
            await subscriber.SubscribeAsync(shardedChannel, shardedHandler);
            shardedQueue = await subscriber.SubscribeAsync(shardedChannel);
        }

        await Task.Delay(SubscribeDelay);

        // Publish to channels and verify all receive.
        await publisher.PublishAsync(literalChannel, Message1);
        await AssertHandlerReceives(literalReceived, [(literalChannel, Message1)]);
        await AssertQueueReceives(literalQueue, [(literalChannel, Message1)]);

        await publisher.PublishAsync(patternChannel, Message1);
        await AssertHandlerReceives(patternReceived, [(pattern, Message1)]);
        await AssertQueueReceives(patternQueue, [(pattern, Message1)]);

        if (isCluster && IsSharedPubSubSupported)
        {
            await publisher.PublishAsync(shardedChannel, Message1);
            await AssertHandlerReceives(shardedReceived, [(shardedChannel, Message1)]);
            await AssertQueueReceives(shardedQueue!, [(shardedChannel, Message1)]);
        }

        // Unsubscribe all subscriptions.
        await subscriber.UnsubscribeAllAsync();
        await Task.Delay(UnsubscribeDelay);

        // Publish and verify none receive.
        await publisher.PublishAsync(literalChannel, Message2);
        await AssertHandlerReceives(literalReceived, []);
        await AssertQueueReceives(literalQueue, []);

        await publisher.PublishAsync(patternChannel, Message2);
        await AssertHandlerReceives(patternReceived, []);
        await AssertQueueReceives(patternQueue, []);

        if (isCluster && IsSharedPubSubSupported)
        {
            await publisher.PublishAsync(shardedChannel, Message2);
            await AssertHandlerReceives(shardedReceived, []);
            await AssertQueueReceives(shardedQueue!, []);
        }
    }

    /// <summary>
    /// Builds and returns two new subscribers for testing.
    /// </summary>
    private static ISubscriber BuildSubscriber(bool isCluster)
    {
        ConnectionMultiplexer connection = isCluster
            ? TestConfiguration.DefaultCompatibleClusterConnection()
            : TestConfiguration.DefaultCompatibleConnection();
        return connection.GetSubscriber();
    }

    /// <summary>
    /// Builds and returns a literal channel for testing.
    /// </summary>
    private static ValkeyChannel BuildLiteral()
        => ValkeyChannel.Literal($"test-{Guid.NewGuid()}-channel");

    /// <summary>
    /// Builds and returns a pattern channel for testing.
    /// </summary>
    private static ValkeyChannel BuildPattern()
        => ValkeyChannel.Pattern($"test-{Guid.NewGuid()}-*");

    /// <summary>
    /// Builds and returns a sharded channel for testing (cluster only).
    /// </summary>
    private static ValkeyChannel BuildSharded()
        => ValkeyChannel.Sharded($"test-{Guid.NewGuid()}-shard-channel");

    /// <summary>
    /// Builds and returns a literal channel for testing that matches the specified pattern.
    /// </summary>
    private static ValkeyChannel BuildLiteralForPattern(ValkeyChannel pattern)
    {
        Assert.True(pattern.IsPattern);

        // Replace '*' in pattern with random digits to create a matching literal channel.
        var patternStr = pattern.ToString();
        var literalStr = patternStr.Replace("*", Random.Shared.Next(1000, 9999).ToString());

        return ValkeyChannel.Literal(literalStr);
    }

    /// <summary>
    /// Builds and returns a message handler that adds received messages to the specified bag.
    /// </summary>
    /// <param name="bag"></param>
    public Action<ValkeyChannel, ValkeyValue> BuildHandler(ConcurrentBag<MessageInfo> bag)
        => (channel, message) => bag.Add((channel, message));

    /// <summary>
    /// Asserts that the messages with the given channels and values are received by the specified queue.
    /// </summary>
    private static async Task AssertQueueReceives(ChannelMessageQueue queue, List<MessageInfo> expected)
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
    private static async Task AssertHandlerReceives(ConcurrentBag<MessageInfo> actual, List<MessageInfo> expected)
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

    [Fact]
    public async Task Queue_UnsubscribeOneOfMultiple_OtherQueuesStillReceive()
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber();

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

    [Fact]
    public async Task MixedHandlersAndQueues_UnsubscribeHandler_QueueStillReceives()
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber();

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

    [Fact]
    public async Task MixedHandlersAndQueues_UnsubscribeQueue_HandlerStillReceives()
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber();

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

    [Fact]
    public async Task MultipleSubscribers_ShareState()
    {
        var channel = BuildLiteral();
        var subscriber = await BuildSubscriber();
        var publisher = await BuildSubscriber();

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
    private static async Task<ISubscriber> BuildSubscriber()
    {
        var address = TestConfiguration.STANDALONE_ADDRESS;
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
