// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection.Metadata;

using Valkey.Glide.Commands;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for <cref="GlideClient.PubSubCommands"/>.
public class PubSubCommandTests()
{
    #region PublishCommands

    [Fact]
    public async Task PublishAsync_WithNoSubscribers_ReturnsZero()
    {
        var msg = GetMessage();
        var (message, channel) = (msg.Message, msg.Channel);

        using var client = TestConfiguration.DefaultStandaloneClient();

        // Publish to channel and verify no subscribers.
        Assert.Equal(0L, await client.PublishAsync(channel, message));
        await AssertNotReceived(client, message);
    }

    [Fact]
    public async Task PublishAsync_WithSubscriber_ReturnsSubscriberCount()
    {
        var msg = GetMessage();
        var (message, channel) = (msg.Message, msg.Channel);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(channel);
        await AssertSubscribed(publisher, channel);

        // Publish to channel and verify one subscriber.
        Assert.Equal(1L, await publisher.PublishAsync(channel, message));
        await AssertReceived(subscriber, message, channel);
    }

    #endregion
    #region SubscribeCommands

    [Fact]
    public async Task SubscribeAsync_OneChannel_ReceivesMessage()
    {
        var msg = GetMessage();
        var (message, channel) = (msg.Message, msg.Channel);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(channel);
        await AssertSubscribed(publisher, channel);

        // Publish to channel and verify message received.
        await publisher.PublishAsync(channel, message);
        await AssertReceived(subscriber, message, channel);
    }

    [Fact]
    public async Task SubscribeAsync_MultipleChannels_ReceivesMessages()
    {
        var msg1 = GetMessage();
        var (message1, channel1) = (msg1.Message, msg1.Channel);

        var msg2 = GetMessage();
        var (message2, channel2) = (msg2.Message, msg2.Channel);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channels and verify subscriptions.
        await subscriber.SubscribeAsync([channel1, channel2]);
        await AssertSubscribed(publisher, [channel1, channel2]);

        // Publish to channels and verify messages received.
        await publisher.PublishAsync(channel1, message1);
        await AssertReceived(subscriber, message1, channel1);

        await publisher.PublishAsync(channel2, message2);
        await AssertReceived(subscriber, message2, channel2);
    }

    [Fact]
    public async Task PSubscribeAsync_OnePattern_ReceivesMessage()
    {
        var msg = GetMessage();
        var (message, channel, pattern) = (msg.Message, msg.Channel, msg.Pattern);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to pattern and wait to ensure subscription completes.
        await subscriber.PSubscribeAsync(pattern);
        await Task.Delay(500);

        // Publish to channel and verify message received.
        await publisher.PublishAsync(channel, message);
        await AssertReceived(subscriber, message, channel, pattern);
    }

    [Fact]
    public async Task PSubscribeAsync_MultiplePatterns_ReceivesMessages()
    {
        var msg1 = GetMessage();
        var (message1, channel1, pattern1) = (msg1.Message, msg1.Channel, msg1.Pattern);

        var msg2 = GetMessage();
        var (message2, channel2, pattern2) = (msg2.Message, msg2.Channel, msg2.Pattern);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to patterns and wait to ensure subscriptions complete.
        await subscriber.PSubscribeAsync([pattern1, pattern2]);
        await Task.Delay(500);

        // Publish to channels and verify messages received.
        await publisher.PublishAsync(channel1, message1);
        await AssertReceived(subscriber, message1, channel1, pattern1);

        await publisher.PublishAsync(channel2, message2);
        await AssertReceived(subscriber, message2, channel2, pattern2);
    }

    #endregion
    #region UnsubscribeCommands

    [Fact]
    public async Task UnsubscribeAsync_AllChannels_StopsReceivingMessages()
    {
        var msg1 = GetMessage();
        var (message1, channel1) = (msg1.Message, msg1.Channel);

        var msg2 = GetMessage();
        var (message2, channel2) = (msg2.Message, msg2.Channel);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([channel1, channel2]);
        await AssertSubscribed(publisher, [channel1, channel2]);

        // Unsubscribe from all channels and verify unsubscription.
        await subscriber.UnsubscribeAsync();
        AssertUnsubscribed(publisher, [channel1, channel2]);

        // Publish to channels and verify no messages received.
        await publisher.PublishAsync(channel1, message1);
        await AssertNotReceived(subscriber, message1);

        await publisher.PublishAsync(channel2, message2);
        await AssertNotReceived(subscriber, message2);
    }

    [Fact]
    public async Task UnsubscribeAsync_OneChannel_StopsReceivingMessages()
    {
        var msg1 = GetMessage();
        var (message1, channel1) = (msg1.Message, msg1.Channel);

        var msg2 = GetMessage();
        var (message2, channel2) = (msg2.Message, msg2.Channel);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([channel1, channel2]);
        await AssertSubscribed(publisher, [channel1, channel2]);

        // Unsubscribe from one channel and verify unsubscription.
        await subscriber.UnsubscribeAsync(channel1);
        AssertUnsubscribed(publisher, channel1);

        // Publish to unsubscribed channel and verify no message received.
        await publisher.PublishAsync(channel1, message1);
        await AssertNotReceived(subscriber, message1);

        // Publish to subscribed channel and verify message received.
        await publisher.PublishAsync(channel2, message2);
        await AssertReceived(subscriber, message2, channel2);
    }

    // [Fact]
    public async Task UnsubscribeAsync_MultipleChannels_StopsReceivingMessages()
    {
        var msg1 = GetMessage();
        var (message1, channel1) = (msg1.Message, msg1.Channel);

        var msg2 = GetMessage();
        var (message2, channel2) = (msg2.Message, msg2.Channel);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([channel1, channel2]);
        await AssertSubscribed(publisher, [channel1, channel2]);

        // Unsubscribe from both channels and verify unsubscriptions.
        await subscriber.UnsubscribeAsync([channel1, channel2]);
        AssertUnsubscribed(publisher, [channel1, channel2]);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(channel1, message1);
        await AssertNotReceived(subscriber, message1);

        await publisher.PublishAsync(channel2, message2);
        await AssertNotReceived(subscriber, message2);
    }

    [Fact]
    public async Task PUnsubscribeAsync_AllPatterns_StopsReceivingMessages()
    {
        var msg1 = GetMessage();
        var (message1, channel1, pattern1) = (msg1.Message, msg1.Channel, msg1.Pattern);

        var msg2 = GetMessage();
        var (message2, channel2, pattern2) = (msg2.Message, msg2.Channel, msg2.Pattern);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to both patterns and wait for subscriptions to complete.
        await subscriber.PSubscribeAsync(pattern1);
        await subscriber.PSubscribeAsync(pattern2);
        await Task.Delay(500);

        // Unsubscribe from all patterns and wait for unsubscriptions to complete.
        await subscriber.PUnsubscribeAsync();
        await Task.Delay(500);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(channel1, message1);
        await AssertNotReceived(subscriber, message1);

        await publisher.PublishAsync(channel2, message2);
        await AssertNotReceived(subscriber, message2);
    }

    [Fact]
    public async Task PUnsubscribeAsync_OnePattern_StopsReceivingMessages()
    {
        var msg1 = GetMessage();
        var (message1, channel1, pattern1) = (msg1.Message, msg1.Channel, msg1.Pattern);

        var msg2 = GetMessage();
        var (message2, channel2, pattern2) = (msg2.Message, msg2.Channel, msg2.Pattern);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to both patterns and wait for subscriptions to complete.
        await subscriber.PSubscribeAsync(pattern1);
        await subscriber.PSubscribeAsync(pattern2);
        await Task.Delay(500);

        // Unsubscribe from one pattern and wait for unsubscription to complete.
        await subscriber.PUnsubscribeAsync(pattern1);
        await Task.Delay(500);

        // Publish to unsubscribed pattern channel and verify no message received.
        await publisher.PublishAsync(channel1, message1);
        await AssertNotReceived(subscriber, message1);

        // Publish to subscribed pattern channel and verify message received.
        await publisher.PublishAsync(channel2, message2);
        await AssertReceived(subscriber, message2, channel2, pattern2);
    }

    [Fact]
    public async Task PUnsubscribeAsync_MultipleChannels_StopsReceivingMessages()
    {
        var msg1 = GetMessage();
        var (message1, channel1, pattern1) = (msg1.Message, msg1.Channel, msg1.Pattern);

        var msg2 = GetMessage();
        var (message2, channel2, pattern2) = (msg2.Message, msg2.Channel, msg2.Pattern);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to both patterns and wait for subscriptions to complete.
        await subscriber.PSubscribeAsync(pattern1);
        await subscriber.PSubscribeAsync(pattern2);
        await Task.Delay(500);

        // Unsubscribe from both patterns and wait for unsubscriptions to complete.
        await subscriber.PUnsubscribeAsync([pattern1, pattern2]);
        await Task.Delay(500);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(channel1, message1);
        await AssertNotReceived(subscriber, message1);

        await publisher.PublishAsync(channel2, message2);
        await AssertNotReceived(subscriber, message2);
    }

    #endregion
    #region PubSubInfoCommands

    [Fact]
    public async Task PubSubChannelsAsync_WithNoChannels_ReturnsEmptyArray()
    {
        using var server = new StandaloneServer();
        using var client = await GlideClient.CreateClient(server.CreateConfigBuilder().Build());

        // Verify no active channels.
        Assert.Empty(await client.PubSubChannelsAsync());
    }

    [Fact]
    public async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        var msg = GetMessage();
        var channel = msg.Channel;

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(channel);
        await AssertSubscribed(publisher, channel);

        // Verify that channel is active.
        Assert.Contains(channel, await publisher.PubSubChannelsAsync());
    }

    [Fact]
    public async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        var msg = GetMessage();
        var (channel, pattern) = (msg.Channel, msg.Pattern);

        using var subscriber = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(channel);
        await AssertSubscribed(publisher, channel);

        // Verify that channel is active for pattern.
        Assert.Contains(channel, await publisher.PubSubChannelsAsync(pattern));
    }

    [Fact]
    public async Task PubSubNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        var msg1 = GetMessage();
        var channel1 = msg1.Channel;

        var msg2 = GetMessage();
        var channel2 = msg2.Channel;

        using var client = TestConfiguration.DefaultStandaloneClient();

        // Verify no subscribers for both channels.
        var expected = new Dictionary<string, long> { { channel1, 0L }, { channel2, 0L } };
        var actual = await client.PubSubNumSubAsync([channel1, channel2]);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task PubSubNumSubAsync_WithSubscribers_ReturnsCorrectCounts()
    {
        var msg1 = GetMessage();
        var channel1 = msg1.Channel;

        var msg2 = GetMessage();
        var channel2 = msg2.Channel;

        using var subscriber1 = TestConfiguration.DefaultStandaloneClient();
        using var subscriber2 = TestConfiguration.DefaultStandaloneClient();
        using var publisher = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channels and verify subscriptions.
        await subscriber1.SubscribeAsync(channel1);
        AssertSubscribed(publisher, channel1);

        await subscriber2.SubscribeAsync(channel2);
        AssertSubscribed(publisher, channel2);

        // Verify subscription counts for both channels.
        var expected = new Dictionary<string, long> { { channel1, 1L }, { channel2, 1L } };
        var actual = await publisher.PubSubNumSubAsync([channel1, channel2]);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task PubSubNumPatAsync_WithNoPatterns_ReturnsZero()
    {
        using var server = new StandaloneServer();
        using var client = await GlideClient.CreateClient(server.CreateConfigBuilder().Build());

        // Verify no active pattern subscriptions.
        Assert.Equal(0L, await client.PubSubNumPatAsync());
    }

    [Fact]
    public async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsCount()
    {
        var msg1 = GetMessage();
        var pattern1 = msg1.Pattern;

        var msg2 = GetMessage();
        var pattern2 = msg2.Pattern;

        using var server = new StandaloneServer();
        var config = server.CreateConfigBuilder().Build();

        using var subscriber1 = await GlideClient.CreateClient(config);
        using var subscriber2 = await GlideClient.CreateClient(config);
        using var publisher = await GlideClient.CreateClient(config);

        // Subscribe to patterns and wait to ensure subscriptions complete.
        await subscriber1.PSubscribeAsync(pattern1);
        await subscriber2.PSubscribeAsync(pattern2);
        await Task.Delay(500);

        // Verify active pattern subscription count.
        Assert.Equal(2L, await publisher.PubSubNumPatAsync());
    }

    #endregion

    /// <summary>
    /// Returns a unique message for testing.
    /// </summary>
    private static PubSubMessage GetMessage()
    {
        var id = Guid.NewGuid().ToString();
        return new PubSubMessage(
            message: $"test-{id}-message",
            channel: $"test-{id}-channel",
            pattern: $"test-{id}-*"); // Pattern always matches channel.
    }

    /// <summary>
    /// Asserts that the specified message is received on the specified channel (and pattern, if provided).
    /// </summary>
    private static async Task AssertReceived(BaseClient client, string message, string channel, string pattern = null)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var received = await queue.GetMessageAsync(cts.Token);

        Assert.Equal(message, received.Message);
        Assert.Equal(channel, received.Channel);
        Assert.Equal(pattern, received.Pattern);
    }

    /// <summary>
    /// Asserts that the specified message is not received.
    /// </summary>
    private static async Task AssertNotReceived(BaseClient client, string message)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        queue.TryGetMessage(out PubSubMessage? received);
        if (received == null) return;

        Assert.NotEqual(message, received.Message);
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to the specified channel.
    /// </summary>
    private static async Task AssertSubscribed(GlideClient client, string channel)
    {
        var channelCounts = await client.PubSubNumSubAsync([channel]);
        Assert.True(channelCounts[channel] > 0, $"Expected at least 1 subscriber for channel '{channel}', got {channelCounts[channel]}");
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified channels.
    /// </summary>
    private static async Task AssertSubscribed(GlideClient client, string[] channels)
    {
        var channelCounts = await client.PubSubNumSubAsync(channels);
        foreach (var item in channelCounts)
            Assert.True(item.Value > 0, $"Expected at least 1 subscriber for channel '{item.Key}', got {item.Value}");
    }

    /// <summary>
    /// Asserts that there are no subscribers to the specified channel.
    /// </summary>
    private static async Task AssertUnsubscribed(GlideClient client, string channel)
    {
        var channelCounts = await client.PubSubNumSubAsync([channel]);
        Assert.True(channelCounts[channel] == 0, $"Expected no subscribers for channel '{channel}', got {channelCounts[channel]}");
    }

    /// <summary>
    /// Asserts that there are no subscribers to each of the specified channels.
    /// </summary>
    private static async Task AssertUnsubscribed(GlideClient client, string[] channels)
    {
        var channelCounts = await client.PubSubNumSubAsync(channels);
        foreach (var item in channelCounts)
            Assert.True(item.Value == 0, $"Expected no subscribers for channel '{item.Key}', got {item.Value}");
    }
}
