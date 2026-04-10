// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Collections.Concurrent;

using Valkey.Glide.TestUtils;

using static Valkey.Glide.IntegrationTests.PubSubUtils;
// Type alias for readability.
using MessageInfo = (Valkey.Glide.ValkeyChannel Channel, Valkey.Glide.ValkeyValue Message);

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for StackExchange.Redis pub/sub commands including
/// <see cref="ISubscriber"/> subscribe/unsubscribe operations, and
/// <see cref="IServer"/> pub/sub introspection commands.
/// <see cref="IDatabaseAsync.PublishAsync(ValkeyChannel, ValkeyValue, CommandFlags)"/>,
/// </summary>
[Collection(typeof(PubSubCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Constants

    private static readonly bool IsSharedPubSubSupported = TestConfiguration.IsVersionAtLeast("7.0.0");
    private static readonly string SkipSharedPubSubMessage = "Sharded PubSub is supported since 7.0.0";

    private static readonly ValkeyValue Message1 = "message1";
    private static readonly ValkeyValue Message2 = "message2";

    private static readonly TimeSpan AssertRetryInterval = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan AssertTimeout = TimeSpan.FromSeconds(5);

    #endregion
    #region PublishAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task PublishAsync_NoSubscribers_ReturnsZero(IDatabaseAsync db)
    {
        var channel = ValkeyChannel.Literal($"test-channel-{Guid.NewGuid()}");
        Assert.Equal(0, await db.PublishAsync(channel, "hello"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task PublishAsync_WithSubscriber_ReturnsOne(IDatabaseAsync db)
    {
        var channel = ValkeyChannel.Literal($"test-channel-{Guid.NewGuid()}");

        using var subscriberConnection = TestConfiguration.DefaultCompatibleConnection();
        var subscriber = subscriberConnection.GetSubscriber();
        var queue = await subscriber.SubscribeAsync(channel);

        try
        {
            long result = await db.PublishAsync(channel, "hello");
            Assert.Equal(1, result);
        }
        finally
        {
            await queue.UnsubscribeAsync();
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task PublishAsync_NullOrEmptyChannel_Throws(IDatabaseAsync db)
    {
        var emptyChannel = new ValkeyChannel((byte[]?)null, ValkeyChannel.PatternMode.Literal);

        _ = await Assert.ThrowsAsync<ArgumentException>(
            () => db.PublishAsync(emptyChannel, "hello"));
    }

    #endregion
    #region SubscribeAsync

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task Literal_Subscribe_Handler(bool isCluster)
    {
        var literal = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(literal, handler);

        _ = await publisher.PublishAsync(literal, Message1);
        await AssertHandlerReceives(received, [(literal, Message1)]);

        await subscriber.UnsubscribeAsync(literal, handler);

        _ = await publisher.PublishAsync(literal, Message2);
        await AssertHandlerReceives(received, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task Literal_Subscribe_Queue(bool isCluster)
    {
        var literal = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var queue = await subscriber.SubscribeAsync(literal);

        _ = await publisher.PublishAsync(literal, Message1);
        await AssertQueueReceives(queue, [(literal, Message1)]);

        await queue.UnsubscribeAsync();

        _ = await publisher.PublishAsync(literal, Message2);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task Pattern_Subscribe_Handler(bool isCluster)
    {
        var pattern = BuildPattern();
        var channel = BuildLiteralForPattern(pattern);
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(pattern, handler);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(pattern, Message1)]);

        await subscriber.UnsubscribeAsync(pattern, handler);

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task Pattern_Subscribe_Queue(bool isCluster)
    {
        var pattern = BuildPattern();
        var channel = BuildLiteralForPattern(pattern);
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var queue = await subscriber.SubscribeAsync(pattern);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertQueueReceives(queue, [(pattern, Message1)]);

        await queue.UnsubscribeAsync();

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertQueueReceives(queue, []);
    }

    [Fact]
    public async Task Sharded_Subscribe_Handler()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        bool isCluster = true;

        var channel = BuildSharded();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);

        await subscriber.UnsubscribeAsync(channel, handler);

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
    }

    [Fact]
    public async Task Sharded_Subscribe_Queue()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        bool isCluster = true;

        var channel = BuildSharded();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var queue = await subscriber.SubscribeAsync(channel);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        await queue.UnsubscribeAsync();

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task MultipleHandlers(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received1 = new ConcurrentBag<MessageInfo>();
        var handler1 = BuildHandler(received1);
        await subscriber.SubscribeAsync(channel, handler1);

        var received2 = new ConcurrentBag<MessageInfo>();
        var handler2 = BuildHandler(received2);
        await subscriber.SubscribeAsync(channel, handler2);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received1, [(channel, Message1)]);
        await AssertHandlerReceives(received2, [(channel, Message1)]);

        await subscriber.UnsubscribeAsync(channel, handler1);

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received1, []);
        await AssertHandlerReceives(received2, [(channel, Message2)]);

        await subscriber.UnsubscribeAsync(channel, handler2);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received1, []);
        await AssertHandlerReceives(received2, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task MultipleQueues(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var queue1 = await subscriber.SubscribeAsync(channel);
        var queue2 = await subscriber.SubscribeAsync(channel);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertQueueReceives(queue1, [(channel, Message1)]);
        await AssertQueueReceives(queue2, [(channel, Message1)]);

        await queue1.UnsubscribeAsync();

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertQueueReceives(queue1, []);
        await AssertQueueReceives(queue2, [(channel, Message2)]);

        await queue2.UnsubscribeAsync();

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertQueueReceives(queue1, []);
        await AssertQueueReceives(queue2, []);
    }

    #endregion
    #region UnsubscribeAsync

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task QueueAndHandler_UnsubscribeHandler(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        var queue = await subscriber.SubscribeAsync(channel);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        await subscriber.UnsubscribeAsync(channel, handler);

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, [(channel, Message2)]);

        await queue.UnsubscribeAsync();

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task QueueAndHandler_UnsubscribeQueue(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        var queue = await subscriber.SubscribeAsync(channel);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        await queue.UnsubscribeAsync();

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, [(channel, Message2)]);
        await AssertQueueReceives(queue, []);

        await subscriber.UnsubscribeAsync(channel, handler);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task QueueAndHandler_UnsubscribeChannel(bool isCluster)
    {
        var channel = BuildLiteral();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        var queue = await subscriber.SubscribeAsync(channel);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        await subscriber.UnsubscribeAsync(channel);

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task QueueAndHandler_UnsubscribePattern(bool isCluster)
    {
        var pattern = BuildPattern();
        var channel = BuildLiteralForPattern(pattern);
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(pattern, handler);
        var queue = await subscriber.SubscribeAsync(pattern);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(pattern, Message1)]);
        await AssertQueueReceives(queue, [(pattern, Message1)]);

        await subscriber.UnsubscribeAsync(pattern);

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Fact]
    public async Task QueueAndHandler_UnsubscribeSharded()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        bool isCluster = true;

        var channel = BuildSharded();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var received = new ConcurrentBag<MessageInfo>();
        var handler = BuildHandler(received);
        await subscriber.SubscribeAsync(channel, handler);
        var queue = await subscriber.SubscribeAsync(channel);

        _ = await publisher.PublishAsync(channel, Message1);
        await AssertHandlerReceives(received, [(channel, Message1)]);
        await AssertQueueReceives(queue, [(channel, Message1)]);

        await subscriber.UnsubscribeAsync(channel);

        _ = await publisher.PublishAsync(channel, Message2);
        await AssertHandlerReceives(received, []);
        await AssertQueueReceives(queue, []);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task QueueAndHandler_UnsubscribeAll(bool isCluster)
    {
        var isSharded = IsShardedSupported(isCluster);

        var literalChannel = BuildLiteral();
        var pattern = BuildPattern();
        var patternChannel = BuildLiteralForPattern(pattern);
        var shardedChannel = BuildSharded();
        var subscriber = BuildSubscriber(isCluster);
        var publisher = BuildSubscriber(isCluster);

        var literalReceived = new ConcurrentBag<MessageInfo>();
        var literalHandler = BuildHandler(literalReceived);
        await subscriber.SubscribeAsync(literalChannel, literalHandler);
        var literalQueue = await subscriber.SubscribeAsync(literalChannel);

        var patternReceived = new ConcurrentBag<MessageInfo>();
        var patternHandler = BuildHandler(patternReceived);
        await subscriber.SubscribeAsync(pattern, patternHandler);
        var patternQueue = await subscriber.SubscribeAsync(pattern);

        ConcurrentBag<MessageInfo> shardedReceived = [];
        var shardedHandler = BuildHandler(shardedReceived);
        ChannelMessageQueue? shardedQueue = null;

        if (isSharded)
        {
            await subscriber.SubscribeAsync(shardedChannel, shardedHandler);
            shardedQueue = await subscriber.SubscribeAsync(shardedChannel);
        }

        _ = await publisher.PublishAsync(literalChannel, Message1);
        await AssertHandlerReceives(literalReceived, [(literalChannel, Message1)]);
        await AssertQueueReceives(literalQueue, [(literalChannel, Message1)]);

        _ = await publisher.PublishAsync(patternChannel, Message1);
        await AssertHandlerReceives(patternReceived, [(pattern, Message1)]);
        await AssertQueueReceives(patternQueue, [(pattern, Message1)]);

        if (isSharded)
        {
            _ = await publisher.PublishAsync(shardedChannel, Message1);
            await AssertHandlerReceives(shardedReceived, [(shardedChannel, Message1)]);
            await AssertQueueReceives(shardedQueue!, [(shardedChannel, Message1)]);
        }

        await subscriber.UnsubscribeAllAsync();

        _ = await publisher.PublishAsync(literalChannel, Message2);
        await AssertHandlerReceives(literalReceived, []);
        await AssertQueueReceives(literalQueue, []);

        _ = await publisher.PublishAsync(patternChannel, Message2);
        await AssertHandlerReceives(patternReceived, []);
        await AssertQueueReceives(patternQueue, []);

        if (isSharded)
        {
            _ = await publisher.PublishAsync(shardedChannel, Message2);
            await AssertHandlerReceives(shardedReceived, []);
            await AssertQueueReceives(shardedQueue!, []);
        }
    }

    #endregion
    #region SubscriptionChannelsAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task SubscriptionChannelsAsync_NoSubscribers_ReturnsEmpty(IServer server)
        => Assert.Empty(await server.SubscriptionChannelsAsync());

    [Fact]
    public async Task SubscriptionChannelsAsync_WithSubscriber_ReturnsChannel()
    {
        using var connection = TestConfiguration.DefaultCompatibleConnection();
        var server = connection.GetServers().First();
        var subscriber = connection.GetSubscriber();

        var channelName = $"test-pubsub-channels-{Guid.NewGuid()}";
        var channel = ValkeyChannel.Literal(channelName);

        await subscriber.SubscribeAsync(channel, (_, _) => { });
        try
        {
            await Task.Delay(500, TestContext.Current.CancellationToken);
            ValkeyChannel[] channels = await server.SubscriptionChannelsAsync();
            Assert.Contains(channels, c => c.ToString() == channelName);
        }
        finally
        {
            await subscriber.UnsubscribeAsync(channel);
            await Task.Delay(500, TestContext.Current.CancellationToken);
        }
    }

    [Fact]
    public async Task SubscriptionChannelsAsync_WithPattern_FiltersChannels()
    {
        using var connection = TestConfiguration.DefaultCompatibleConnection();
        var server = connection.GetServers().First();
        var subscriber = connection.GetSubscriber();

        var prefix = $"pubsub-filter-{Guid.NewGuid():N}";
        var channel1 = ValkeyChannel.Literal($"{prefix}-a");
        var channel2 = ValkeyChannel.Literal($"{prefix}-b");
        var otherChannel = ValkeyChannel.Literal($"other-{Guid.NewGuid()}");

        await subscriber.SubscribeAsync(channel1, (_, _) => { });
        await subscriber.SubscribeAsync(channel2, (_, _) => { });
        await subscriber.SubscribeAsync(otherChannel, (_, _) => { });
        try
        {
            await Task.Delay(500, TestContext.Current.CancellationToken);

            var pattern = ValkeyChannel.Literal($"{prefix}-*");
            ValkeyChannel[] channels = await server.SubscriptionChannelsAsync(pattern);

            Assert.Equal(2, channels.Length);
            Assert.Contains(channels, c => c.ToString() == $"{prefix}-a");
            Assert.Contains(channels, c => c.ToString() == $"{prefix}-b");
        }
        finally
        {
            await subscriber.UnsubscribeAsync(channel1);
            await subscriber.UnsubscribeAsync(channel2);
            await subscriber.UnsubscribeAsync(otherChannel);
            await Task.Delay(500, TestContext.Current.CancellationToken);
        }
    }

    #endregion
    #region SubscriptionPatternCountAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task SubscriptionPatternCountAsync_NoPatterns_ReturnsZero(IServer server)
        => Assert.Equal(0, await server.SubscriptionPatternCountAsync());

    #endregion
    #region SubscriptionSubscriberCountAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task SubscriptionSubscriberCountAsync_NoSubscribers_ReturnsZero(IServer server)
    {
        var channel = ValkeyChannel.Literal($"numsub-empty-{Guid.NewGuid()}");
        Assert.Equal(0, await server.SubscriptionSubscriberCountAsync(channel));
    }

    #endregion
    #region Helpers

    private static ISubscriber BuildSubscriber(bool isCluster)
    {
        ConnectionMultiplexer connection = isCluster
            ? TestConfiguration.DefaultCompatibleClusterConnection()
            : TestConfiguration.DefaultCompatibleConnection();
        return connection.GetSubscriber();
    }

    private static ValkeyChannel BuildLiteral()
        => ValkeyChannel.Literal($"test-{Guid.NewGuid()}-channel");

    private static ValkeyChannel BuildPattern()
        => ValkeyChannel.Pattern($"test-{Guid.NewGuid()}-*");

    private static ValkeyChannel BuildSharded()
        => ValkeyChannel.Sharded($"test-{Guid.NewGuid()}-shard-channel");

    private static ValkeyChannel BuildLiteralForPattern(ValkeyChannel pattern)
    {
        Assert.True(pattern.IsPattern);
        var patternStr = pattern.ToString();
        var literalStr = patternStr.Replace("*", Random.Shared.Next(1000, 9999).ToString());
        return ValkeyChannel.Literal(literalStr);
    }

    private static Action<ValkeyChannel, ValkeyValue> BuildHandler(ConcurrentBag<MessageInfo> bag)
        => (channel, message) => bag.Add((channel, message));

    private static async Task AssertQueueReceives(ChannelMessageQueue queue, List<MessageInfo> expected)
    {
        if (expected.Count == 0)
        {
            await Task.Delay(500);
            Assert.False(queue.TryRead(out _));
            return;
        }

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

    private static async Task AssertHandlerReceives(ConcurrentBag<MessageInfo> actual, List<MessageInfo> expected)
    {
        if (expected.Count == 0)
        {
            await Task.Delay(500);
            Assert.Empty(actual);
            return;
        }

        using var cts = new CancellationTokenSource(AssertTimeout);

        while (!cts.Token.IsCancellationRequested)
        {
            if (actual.Count >= expected.Count)
            {
                Assert.Equivalent(expected, actual);
                actual.Clear();
                return;
            }

            await Task.Delay(AssertRetryInterval);
        }

        Assert.Fail("Expected messages not received within the timeout period.");
    }

    #endregion
}
