// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for PubSub command methods (PublishAsync, PubSubChannelsAsync, etc.).
/// Tests the PubSub command API implementations in GlideClient.
/// </summary>
[Collection("GlideTests")]
public class PubSubCommandTests()
{
    #region PublishCommands

    [Fact]
    public async Task PublishAsync_WithNoSubscribers_ReturnsZero()
    {
        string channel = $"test-channel-{Guid.NewGuid()}";
        string message = "test message";

        using var client = TestConfiguration.DefaultStandaloneClient();

        long subscriberCount = await client.PublishAsync(channel, message);
        Assert.Equal(0L, subscriberCount);
    }

    [Fact]
    public async Task PublishAsync_WithSubscriber_ReturnsSubscriberCount()
    {
        string testChannel = $"test-channel-{Guid.NewGuid()}";
        string testMessage = "Hello from PublishAsync";

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        await using var subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        await using var publisherClient = TestConfiguration.DefaultStandaloneClient();

        await Task.Delay(1000);

        // Publish message to channel
        long subscriberCount = await publisherClient.PublishAsync(testChannel, testMessage);
        Assert.Equal(1L, subscriberCount);

        // Verify that the message was received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);
        await Task.Delay(500);

        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.True(hasMessage);
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
    }

    #endregion
    #region SubscribeCommands

    [Fact]
    public async Task SubscribeAsync_OneChannel_ReceivesMessage()
    {
        string channel = $"test-channel-{Guid.NewGuid()}";
        string message = "Hello from SubscribeAsync";

        using var subscriberClient = TestConfiguration.DefaultStandaloneClient();
        using var publisherClient = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channel
        await subscriberClient.SubscribeAsync(channel);
        await Task.Delay(1000);

        // Verify number of subscribers
        var channelCounts = await subscriberClient.PubSubNumSubAsync([channel]);
        Assert.Equal(1L, channelCounts[channel]);

        // Publish message to channel
        long subscriberCount = await publisherClient.PublishAsync(channel, message);
        Assert.Equal(1L, subscriberCount);

        // Verify that the message was received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        var received = await queue.GetMessageAsync();
        Assert.Equal(message, received.Message);
        Assert.Equal(channel, received.Channel);
        Assert.Null(received.Pattern);
    }

    [Fact]
    public async Task SubscribeAsync_MultipleChannels_ReceivesMessages()
    {
        // TODO #193
    }

    [Fact]
    public async Task PSubscribeAsync_OnePattern_ReceivesMessage()
    {
        string channel = $"test-{Guid.NewGuid()}-news";
        string pattern = $"test-{Guid.NewGuid()}*";
        string message = "Pattern match message";

        using var subscriberClient = TestConfiguration.DefaultStandaloneClient();
        using var publisherClient = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channel
        await subscriberClient.PSubscribeAsync(pattern);
        await Task.Delay(1000);

        // Verify number of subscribers
        var channelCounts = await subscriberClient.PubSubNumSubAsync([channel]);
        Assert.Equal(1L, channelCounts[channel]);

        // Publish message to channel
        long subscriberCount = await publisherClient.PublishAsync(channel, message);
        Assert.Equal(1L, subscriberCount);

        // Verify that message was received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        var received = await queue.GetMessageAsync();
        Assert.Equal(message, received.Message);
        Assert.Equal(channel, received.Channel);
        Assert.Equal(pattern, received.Pattern);
    }

    [Fact]
    public async Task PSubscribeAsync_MultiplePatterns_ReceivesMessages()
    {
        // TODO #193
    }

    #endregion
    #region UnsubscribeCommands

    [Fact]
    public async Task UnsubscribeAsync_AllChannels_StopsReceivingMessages()
    {
        string channel1 = $"test-channel-{Guid.NewGuid()}";
        string channel2 = $"test-channel-{Guid.NewGuid()}";
        string message1 = "message1";
        string message2 = "message2";

        using var subscriberClient = TestConfiguration.DefaultStandaloneClient();
        using var publisherClient = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channels
        await subscriberClient.SubscribeAsync([channel1, channel2]);
        await Task.Delay(1000);

        // Verify number of subscribers
        var channelCounts = await subscriberClient.PubSubNumSubAsync([channel1, channel2]);
        Assert.Equal(1L, channelCounts[channel1]);
        Assert.Equal(1L, channelCounts[channel2]);

        // Unsubscribe from all channels
        await subscriberClient.UnsubscribeAsync([]);
        await Task.Delay(1000);

        // Verify number of subscribers
        channelCounts = await subscriberClient.PubSubNumSubAsync([channel1, channel2]);
        Assert.Equal(0L, channelCounts[channel1]);
        Assert.Equal(0L, channelCounts[channel2]);

        // Publish messages to channels
        long count1 = await publisherClient.PublishAsync(channel1, message1);
        long count2 = await publisherClient.PublishAsync(channel2, message2);

        Assert.Equal(0L, count1);
        Assert.Equal(0L, count2);

        // Verify that no messages were received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.False(hasMessage);
        Assert.Null(receivedMessage);
    }

    [Fact]
    public async Task UnsubscribeAsync_OneChannel_StopsReceivingMessages()
    {
        string channel = $"test-channel-{Guid.NewGuid()}";
        string message = "Should not receive this";

        using var subscriberClient = TestConfiguration.DefaultStandaloneClient();
        using var publisherClient = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channel
        await subscriberClient.SubscribeAsync(channel);
        await Task.Delay(1000);

        // Verify number of subscribers
        var channelCounts = await subscriberClient.PubSubNumSubAsync([channel]);
        Assert.Equal(1L, channelCounts[channel]);

        // Unsubscribe from channel
        await subscriberClient.UnsubscribeAsync(channel);
        await Task.Delay(1000);

        // Verify number of subscribers
        channelCounts = await subscriberClient.PubSubNumSubAsync([channel]);
        Assert.Equal(1L, channelCounts[channel]);

        // Publish message to channel
        long subscriberCount = await publisherClient.PublishAsync(channel, message);
        Assert.Equal(0L, subscriberCount);

        // Verify that no message was received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.False(hasMessage);
        Assert.Null(receivedMessage);
    }

    [Fact]
    public async Task UnsubscribeAsync_MultipleChannels_StopsReceivingMessages()
    {
        string channel1 = $"test-channel-{Guid.NewGuid()}";
        string channel2 = $"test-channel-{Guid.NewGuid()}";
        string message1 = "message1";
        string message2 = "message2";

        using var subscriberClient = TestConfiguration.DefaultStandaloneClient();
        using var publisherClient = TestConfiguration.DefaultStandaloneClient();

        // Subscribe to channels
        await subscriberClient.SubscribeAsync([channel1, channel2]);
        await Task.Delay(1000);

        // Verify number of subscribers
        var channelCounts = await subscriberClient.PubSubNumSubAsync([channel1, channel2]);
        Assert.Equal(1L, channelCounts[channel1]);
        Assert.Equal(1L, channelCounts[channel2]);

        // Unsubscribe from all channels
        await subscriberClient.UnsubscribeAsync([channel1, channel2]);
        await Task.Delay(1000);

        // Verify number of subscribers
        channelCounts = await subscriberClient.PubSubNumSubAsync([channel1, channel2]);
        Assert.Equal(0L, channelCounts[channel1]);
        Assert.Equal(0L, channelCounts[channel2]);

        // Publish messages to channels
        long count1 = await publisherClient.PublishAsync(channel1, message1);
        long count2 = await publisherClient.PublishAsync(channel2, message2);

        Assert.Equal(0L, count1);
        Assert.Equal(0L, count2);

        // Verify that no messages were received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.False(hasMessage);
        Assert.Null(receivedMessage);
    }

    [Fact]
    public async Task PUnsubscribeAsync_AllChannels_StopsReceivingMessages()
    {
        string pattern1 = $"test-{Guid.NewGuid()}*";
        string pattern2 = $"test-{Guid.NewGuid()}*";
        string channel1 = $"{pattern1.TrimEnd('*')}-channel";
        string channel2 = $"{pattern2.TrimEnd('*')}-channel";
        string message1 = "message1";
        string message2 = "message2";

        var subscriberConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var subscriberClient = await GlideClient.CreateClient(subscriberConfig);

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var publisherClient = await GlideClient.CreateClient(publisherConfig);

        // Subscribe to patterns
        await subscriberClient.PSubscribeAsync([pattern1, pattern2]);
        await Task.Delay(1000);

        // Verify pattern subscriptions exist
        long initialPatternCount = await subscriberClient.PubSubNumPatAsync();
        Assert.True(initialPatternCount >= 2L);

        // Unsubscribe from all patterns
        await subscriberClient.PUnsubscribeAsync([]);
        await Task.Delay(1000);

        // Verify pattern subscriptions removed
        long finalPatternCount = await subscriberClient.PubSubNumPatAsync();
        Assert.True(finalPatternCount < initialPatternCount);

        // Publish messages to matching channels
        await publisherClient.PublishAsync(channel1, message1);
        await publisherClient.PublishAsync(channel2, message2);
        await Task.Delay(500);

        // Verify that no messages were received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.False(hasMessage);
        Assert.Null(receivedMessage);
    }

    [Fact]
    public async Task PUnsubscribeAsync_OneChannel_StopsReceivingMessages()
    {
        string pattern = $"test-{Guid.NewGuid()}*";
        string channel = $"{pattern.TrimEnd('*')}-channel";
        string message = "test message";

        var subscriberConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var subscriberClient = await GlideClient.CreateClient(subscriberConfig);

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var publisherClient = await GlideClient.CreateClient(publisherConfig);

        // Subscribe to pattern
        await subscriberClient.PSubscribeAsync(pattern);
        await Task.Delay(1000);

        // Verify pattern subscription exists
        long initialPatternCount = await subscriberClient.PubSubNumPatAsync();
        Assert.True(initialPatternCount >= 1L);

        // Unsubscribe from pattern
        await subscriberClient.PUnsubscribeAsync(pattern);
        await Task.Delay(1000);

        // Verify pattern subscription removed
        long finalPatternCount = await subscriberClient.PubSubNumPatAsync();
        Assert.True(finalPatternCount < initialPatternCount);

        // Publish message to matching channel
        long subscriberCount = await publisherClient.PublishAsync(channel, message);
        Assert.Equal(0L, subscriberCount);

        // Verify that no message was received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.False(hasMessage);
        Assert.Null(receivedMessage);
    }

    [Fact]
    public async Task PUnsubscribeAsync_MultipleChannels_StopsReceivingMessages()
    {
        string pattern1 = $"test-{Guid.NewGuid()}*";
        string pattern2 = $"test-{Guid.NewGuid()}*";
        string channel1 = $"{pattern1.TrimEnd('*')}-channel";
        string channel2 = $"{pattern2.TrimEnd('*')}-channel";
        string message1 = "message1";
        string message2 = "message2";

        var subscriberConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var subscriberClient = await GlideClient.CreateClient(subscriberConfig);

        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var publisherClient = await GlideClient.CreateClient(publisherConfig);

        // Subscribe to patterns
        await subscriberClient.PSubscribeAsync([pattern1, pattern2]);
        await Task.Delay(1000);

        // Verify pattern subscriptions exist
        long initialPatternCount = await subscriberClient.PubSubNumPatAsync();
        Assert.True(initialPatternCount >= 2L);

        // Unsubscribe from all patterns
        await subscriberClient.PUnsubscribeAsync([pattern1, pattern2]);
        await Task.Delay(1000);

        // Verify pattern subscriptions removed
        long finalPatternCount = await subscriberClient.PubSubNumPatAsync();
        Assert.True(finalPatternCount < initialPatternCount);

        // Publish messages to matching channels
        await publisherClient.PublishAsync(channel1, message1);
        await publisherClient.PublishAsync(channel2, message2);
        await Task.Delay(500);

        // Verify that no messages were received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);

        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.False(hasMessage);
        Assert.Null(receivedMessage);
    }

    #endregion
    #region PubSubInfoCommands

    [Fact]
    public async Task PubSubChannelsAsync_WithNoChannels_ReturnsEmptyArray()
    {
        var config = TestConfiguration.DefaultClientConfig().Build();
        await using var client = await GlideClient.CreateClient(config);

        string[] channels = await client.PubSubChannelsAsync();

        Assert.NotNull(channels);
    }

    [Fact]
    public async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        string testChannel = $"test-channel-{Guid.NewGuid()}";

        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        await using var subscriberClient = await GlideClient.CreateClient(subscriberConfig);

        var queryConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var queryClient = await GlideClient.CreateClient(queryConfig);

        await Task.Delay(1000);

        string[] channels = await queryClient.PubSubChannelsAsync();

        Assert.Contains(testChannel, channels);
    }

    [Fact]
    public async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        string channelPrefix = $"test-{Guid.NewGuid()}";
        string testChannel1 = $"{channelPrefix}-channel1";
        string testChannel2 = $"{channelPrefix}-channel2";
        string pattern = $"{channelPrefix}*";

        StandalonePubSubSubscriptionConfig pubsubConfig1 = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel1);
        StandalonePubSubSubscriptionConfig pubsubConfig2 = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel2);

        var subscriberConfig1 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();
        var subscriberConfig2 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        await using var subscriberClient1 = await GlideClient.CreateClient(subscriberConfig1);
        await using var subscriberClient2 = await GlideClient.CreateClient(subscriberConfig2);

        var queryConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var queryClient = await GlideClient.CreateClient(queryConfig);

        await Task.Delay(1000);

        string[] channels = await queryClient.PubSubChannelsAsync(pattern);

        Assert.Contains(testChannel1, channels);
        Assert.Contains(testChannel2, channels);
    }

    [Fact]
    public async Task PubSubNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        var config = TestConfiguration.DefaultClientConfig().Build();
        await using var client = await GlideClient.CreateClient(config);

        string channel1 = $"test-channel-{Guid.NewGuid()}";
        string channel2 = $"test-channel-{Guid.NewGuid()}";

        Dictionary<string, long> counts = await client.PubSubNumSubAsync([channel1, channel2]);

        Assert.NotNull(counts);
        Assert.Equal(2, counts.Count);
        Assert.Equal(0L, counts[channel1]);
        Assert.Equal(0L, counts[channel2]);
    }

    [Fact]
    public async Task PubSubNumSubAsync_WithSubscribers_ReturnsCorrectCounts()
    {
        string testChannel1 = $"test-channel-{Guid.NewGuid()}";
        string testChannel2 = $"test-channel-{Guid.NewGuid()}";

        StandalonePubSubSubscriptionConfig pubsubConfig1 = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel1);

        var subscriberConfig1 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();

        await using var subscriberClient1 = await GlideClient.CreateClient(subscriberConfig1);

        StandalonePubSubSubscriptionConfig pubsubConfig2 = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel2);

        var subscriberConfig2 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        await using var subscriberClient2a = await GlideClient.CreateClient(subscriberConfig2);
        await using var subscriberClient2b = await GlideClient.CreateClient(subscriberConfig2);

        var queryConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var queryClient = await GlideClient.CreateClient(queryConfig);

        await Task.Delay(1000);

        Dictionary<string, long> counts = await queryClient.PubSubNumSubAsync([testChannel1, testChannel2]);

        Assert.NotNull(counts);
        Assert.Equal(2, counts.Count);
        Assert.Equal(1L, counts[testChannel1]);
        Assert.Equal(2L, counts[testChannel2]);
    }

    [Fact]
    public async Task PubSubNumPatAsync_WithNoPatterns_ReturnsZero()
    {
        var config = TestConfiguration.DefaultClientConfig().Build();
        await using var client = await GlideClient.CreateClient(config);

        long patternCount = await client.PubSubNumPatAsync();

        Assert.True(patternCount >= 0L);
    }

    [Fact]
    public async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsCount()
    {
        string pattern1 = $"test-{Guid.NewGuid()}*";
        string pattern2 = $"test-{Guid.NewGuid()}*";

        StandalonePubSubSubscriptionConfig pubsubConfig1 = new StandalonePubSubSubscriptionConfig()
            .WithPattern(pattern1);
        StandalonePubSubSubscriptionConfig pubsubConfig2 = new StandalonePubSubSubscriptionConfig()
            .WithPattern(pattern2);

        var subscriberConfig1 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();
        var subscriberConfig2 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        await using var subscriberClient1 = await GlideClient.CreateClient(subscriberConfig1);
        await using var subscriberClient2 = await GlideClient.CreateClient(subscriberConfig2);

        var queryConfig = TestConfiguration.DefaultClientConfig().Build();
        await using var queryClient = await GlideClient.CreateClient(queryConfig);

        await Task.Delay(1000);

        long patternCount = await queryClient.PubSubNumPatAsync();

        Assert.True(patternCount >= 2L, $"Expected at least 2 pattern subscriptions, got {patternCount}");
    }

    #endregion
}
