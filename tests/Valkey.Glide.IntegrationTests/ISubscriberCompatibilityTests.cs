// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for ISubscriber compatibility with <c>StackExchange.Redis</c>.
/// These tests mirror key scenarios from <c>StackExchange.Redis.Tests.PubSubTests</c>.
/// </summary>
[Collection(typeof(ISubscriberCompatibilityTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ISubscriberCompatibilityTests
{
    [Fact]
    public async Task Subscribe_WithHandler_ReceivesMessages()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var channel = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        var received = new List<ValkeyValue>();
        var messageEvent = new ManualResetEventSlim(false);

        await sub.SubscribeAsync(channel, (ch, msg) =>
        {
            lock (received) { received.Add(msg); }
            messageEvent.Set();
        });

        await Task.Delay(100);
        await sub.PublishAsync(channel, "test-message");

        Assert.True(messageEvent.Wait(TimeSpan.FromSeconds(5)));
        Assert.Single(received);
        Assert.Equal("test-message", received[0].ToString());
    }

    [Fact]
    public async Task Subscribe_WithQueue_ReceivesMessages()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var channel = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        var queue = await sub.SubscribeAsync(channel);

        await Task.Delay(100);
        await sub.PublishAsync(channel, "test-message");

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var message = await queue.ReadAsync(cts.Token);

        Assert.Equal(channel, message.Channel);
        Assert.Equal("test-message", message.Message.ToString());
    }

    [Fact]
    public async Task Subscribe_Pattern_ReceivesMatchingMessages()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var guid = Guid.NewGuid();
        var pattern = ValkeyChannel.Pattern($"test-{guid}:*");
        var channel1 = ValkeyChannel.Literal($"test-{guid}:one");
        var channel2 = ValkeyChannel.Literal($"test-{guid}:two");
        var received = new List<(ValkeyChannel, ValkeyValue)>();
        var messageEvent = new CountdownEvent(2);

        await sub.SubscribeAsync(pattern, (ch, msg) =>
        {
            lock (received) { received.Add((ch, msg)); }
            messageEvent.Signal();
        });

        await Task.Delay(100);
        await sub.PublishAsync(channel1, "message1");
        await sub.PublishAsync(channel2, "message2");

        Assert.True(messageEvent.Wait(TimeSpan.FromSeconds(5)));
        Assert.Equal(2, received.Count);
    }

    [Fact]
    public async Task Subscribe_MultipleHandlers_BothReceiveMessages()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var channel = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        int handler1Count = 0, handler2Count = 0;
        var messageEvent = new CountdownEvent(2);

        await sub.SubscribeAsync(channel, (ch, msg) =>
        {
            Interlocked.Increment(ref handler1Count);
            messageEvent.Signal();
        });

        await sub.SubscribeAsync(channel, (ch, msg) =>
        {
            Interlocked.Increment(ref handler2Count);
            messageEvent.Signal();
        });

        await Task.Delay(100);
        await sub.PublishAsync(channel, "test");

        Assert.True(messageEvent.Wait(TimeSpan.FromSeconds(5)));
        Assert.Equal(1, Volatile.Read(ref handler1Count));
        Assert.Equal(1, Volatile.Read(ref handler2Count));
    }

    [Fact]
    public async Task Subscribe_MultipleQueues_BothReceiveMessages()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var channel = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        var queue1 = await sub.SubscribeAsync(channel);
        var queue2 = await sub.SubscribeAsync(channel);

        await Task.Delay(100);
        await sub.PublishAsync(channel, "test");

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var received1 = false;
        var received2 = false;

        var task1 = Task.Run(async () =>
        {
            received1 = await queue1.ReadAsync(cts.Token) != null;
        });

        var task2 = Task.Run(async () =>
        {
            received2 = await queue2.ReadAsync(cts.Token) != null;
        });

        await Task.WhenAll(task1, task2);
        Assert.True(received1);
        Assert.True(received2);
    }

    [Fact]
    public async Task Unsubscribe_WithSpecificHandler_RemovesOnlyThatHandler()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var channel = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        int handler1Count = 0, handler2Count = 0;

        void Handler1(ValkeyChannel ch, ValkeyValue msg) => Interlocked.Increment(ref handler1Count);
        void Handler2(ValkeyChannel ch, ValkeyValue msg) => Interlocked.Increment(ref handler2Count);

        await sub.SubscribeAsync(channel, Handler1);
        await sub.SubscribeAsync(channel, Handler2);

        await Task.Delay(100);
        await sub.PublishAsync(channel, "message1");
        await Task.Delay(100);

        await sub.UnsubscribeAsync(channel, Handler1);
        await Task.Delay(100);
        await sub.PublishAsync(channel, "message2");
        await Task.Delay(100);

        Assert.Equal(1, Volatile.Read(ref handler1Count));
        Assert.Equal(2, Volatile.Read(ref handler2Count));
    }

    [Fact]
    public async Task Unsubscribe_WithoutHandler_RemovesAllHandlers()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var channel = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        int handler1Count = 0, handler2Count = 0;

        await sub.SubscribeAsync(channel, (ch, msg) => Interlocked.Increment(ref handler1Count));
        await sub.SubscribeAsync(channel, (ch, msg) => Interlocked.Increment(ref handler2Count));

        await Task.Delay(100);
        await sub.PublishAsync(channel, "message1");
        await Task.Delay(100);

        await sub.UnsubscribeAsync(channel);
        await Task.Delay(100);
        await sub.PublishAsync(channel, "message2");
        await Task.Delay(100);

        Assert.Equal(1, Volatile.Read(ref handler1Count));
        Assert.Equal(1, Volatile.Read(ref handler2Count));
    }

    [Fact]
    public async Task UnsubscribeAll_RemovesAllSubscriptions()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var channel1 = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        var channel2 = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        int count1 = 0, count2 = 0;

        await sub.SubscribeAsync(channel1, (ch, msg) => Interlocked.Increment(ref count1));
        await sub.SubscribeAsync(channel2, (ch, msg) => Interlocked.Increment(ref count2));

        await Task.Delay(100);
        await sub.UnsubscribeAllAsync();
        await Task.Delay(100);

        await sub.PublishAsync(channel1, "message1");
        await sub.PublishAsync(channel2, "message2");
        await Task.Delay(100);

        Assert.Equal(0, Volatile.Read(ref count1));
        Assert.Equal(0, Volatile.Read(ref count2));
    }

    [Fact]
    public async Task MultipleSubscribers_ShareState()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub1 = conn.GetSubscriber();
        var sub2 = conn.GetSubscriber();

        var channel = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        int receivedCount = 0;
        var messageEvent = new ManualResetEventSlim(false);

        await sub1.SubscribeAsync(channel, (ch, msg) =>
        {
            Interlocked.Increment(ref receivedCount);
            messageEvent.Set();
        });

        await Task.Delay(100);
        await sub2.PublishAsync(channel, "test-message");

        Assert.True(messageEvent.Wait(TimeSpan.FromSeconds(5)));
        Assert.Equal(1, receivedCount);
    }

    [Fact]
    public async Task Queue_UnsubscribeAsync_StopsReceivingMessages()
    {
        var config = ConfigurationOptions.Parse(TestConfiguration.STANDALONE_ADDRESS.ToString());
        config.Ssl = TestConfiguration.TLS;
        await using var conn = await ConnectionMultiplexer.ConnectAsync(config);
        var sub = conn.GetSubscriber();

        var channel = ValkeyChannel.Literal($"test-{Guid.NewGuid()}");
        var queue = await sub.SubscribeAsync(channel);
        var received = new List<string>();

        await Task.Delay(100);
        await sub.PublishAsync(channel, "message1");

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var message = await queue.ReadAsync(cts.Token);
        received.Add(message.Message.ToString());

        await queue.UnsubscribeAsync();
        await Task.Delay(100);
        await sub.PublishAsync(channel, "message2");
        await Task.Delay(100);

        Assert.Single(received);
        Assert.Equal("message1", received[0]);
    }
}
