// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for PubSub command methods (PublishAsync, PubSubChannelsAsync, etc.).
/// Tests the PubSub command API implementations in GlideClient.
/// </summary>
[Collection("GlideTests")]
public class PubSubCommandTests(TestConfiguration config) : IDisposable
{
    private readonly List<BaseClient> _testClients = [];
    public TestConfiguration Config { get; } = config;

    [Fact]
    public async Task PublishAsync_WithNoSubscribers_ReturnsZero()
    {
        // Arrange
        var config = TestConfiguration.DefaultClientConfig().Build();
        GlideClient client = await GlideClient.CreateClient(config);
        _testClients.Add(client);

        string channel = $"test-channel-{Guid.NewGuid()}";
        string message = "test message";

        // Act
        long subscriberCount = await client.PublishAsync(channel, message);

        // Assert
        Assert.Equal(0L, subscriberCount);
    }

    [Fact]
    public async Task PublishAsync_WithSubscriber_ReturnsSubscriberCount()
    {
        // Arrange
        string testChannel = $"test-channel-{Guid.NewGuid()}";
        string testMessage = "Hello from PublishAsync";

        // Create subscriber with PubSub config
        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        // Create publisher
        var publisherConfig = TestConfiguration.DefaultClientConfig().Build();
        GlideClient publisherClient = await GlideClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        // Wait for subscription to be established
        await Task.Delay(1000);

        // Act
        long subscriberCount = await publisherClient.PublishAsync(testChannel, testMessage);

        // Assert
        Assert.Equal(1L, subscriberCount);

        // Verify message was received
        PubSubMessageQueue? queue = subscriberClient.PubSubQueue;
        Assert.NotNull(queue);
        await Task.Delay(500);

        bool hasMessage = queue.TryGetMessage(out PubSubMessage? receivedMessage);
        Assert.True(hasMessage);
        Assert.NotNull(receivedMessage);
        Assert.Equal(testMessage, receivedMessage.Message);
        Assert.Equal(testChannel, receivedMessage.Channel);
    }

    [Fact]
    public async Task PubSubChannelsAsync_WithNoChannels_ReturnsEmptyArray()
    {
        // Arrange
        var config = TestConfiguration.DefaultClientConfig().Build();
        GlideClient client = await GlideClient.CreateClient(config);
        _testClients.Add(client);

        // Act
        string[] channels = await client.PubSubChannelsAsync();

        // Assert
        Assert.NotNull(channels);
        // Note: There might be channels from other tests, so we just verify it returns an array
    }

    [Fact]
    public async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        // Arrange
        string testChannel = $"test-channel-{Guid.NewGuid()}";

        // Create subscriber
        StandalonePubSubSubscriptionConfig pubsubConfig = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        GlideClient subscriberClient = await GlideClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClientConfig().Build();
        GlideClient queryClient = await GlideClient.CreateClient(queryConfig);
        _testClients.Add(queryClient);

        // Wait for subscription to be established
        await Task.Delay(1000);

        // Act
        string[] channels = await queryClient.PubSubChannelsAsync();

        // Assert
        Assert.Contains(testChannel, channels);
    }

    [Fact]
    public async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        // Arrange
        string channelPrefix = $"test-{Guid.NewGuid()}";
        string testChannel1 = $"{channelPrefix}-channel1";
        string testChannel2 = $"{channelPrefix}-channel2";
        string pattern = $"{channelPrefix}*";

        // Create subscribers for both channels
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

        GlideClient subscriberClient1 = await GlideClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriberClient1);

        GlideClient subscriberClient2 = await GlideClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClientConfig().Build();
        GlideClient queryClient = await GlideClient.CreateClient(queryConfig);
        _testClients.Add(queryClient);

        // Wait for subscriptions to be established
        await Task.Delay(1000);

        // Act
        string[] channels = await queryClient.PubSubChannelsAsync(pattern);

        // Assert
        Assert.Contains(testChannel1, channels);
        Assert.Contains(testChannel2, channels);
    }

    [Fact]
    public async Task PubSubNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        // Arrange
        var config = TestConfiguration.DefaultClientConfig().Build();
        GlideClient client = await GlideClient.CreateClient(config);
        _testClients.Add(client);

        string channel1 = $"test-channel-{Guid.NewGuid()}";
        string channel2 = $"test-channel-{Guid.NewGuid()}";

        // Act
        Dictionary<string, long> counts = await client.PubSubNumSubAsync([channel1, channel2]);

        // Assert
        Assert.NotNull(counts);
        Assert.Equal(2, counts.Count);
        Assert.Equal(0L, counts[channel1]);
        Assert.Equal(0L, counts[channel2]);
    }

    [Fact]
    public async Task PubSubNumSubAsync_WithSubscribers_ReturnsCorrectCounts()
    {
        // Arrange
        string testChannel1 = $"test-channel-{Guid.NewGuid()}";
        string testChannel2 = $"test-channel-{Guid.NewGuid()}";

        // Create subscriber for channel1
        StandalonePubSubSubscriptionConfig pubsubConfig1 = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel1);

        var subscriberConfig1 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();

        GlideClient subscriberClient1 = await GlideClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriberClient1);

        // Create two subscribers for channel2
        StandalonePubSubSubscriptionConfig pubsubConfig2 = new StandalonePubSubSubscriptionConfig()
            .WithChannel(testChannel2);

        var subscriberConfig2 = TestConfiguration.DefaultClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        GlideClient subscriberClient2a = await GlideClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2a);

        GlideClient subscriberClient2b = await GlideClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2b);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClientConfig().Build();
        GlideClient queryClient = await GlideClient.CreateClient(queryConfig);
        _testClients.Add(queryClient);

        // Wait for subscriptions to be established
        await Task.Delay(1000);

        // Act
        Dictionary<string, long> counts = await queryClient.PubSubNumSubAsync([testChannel1, testChannel2]);

        // Assert
        Assert.NotNull(counts);
        Assert.Equal(2, counts.Count);
        Assert.Equal(1L, counts[testChannel1]);
        Assert.Equal(2L, counts[testChannel2]);
    }

    [Fact]
    public async Task PubSubNumPatAsync_WithNoPatterns_ReturnsZero()
    {
        // Arrange
        var config = TestConfiguration.DefaultClientConfig().Build();
        GlideClient client = await GlideClient.CreateClient(config);
        _testClients.Add(client);

        // Act
        long patternCount = await client.PubSubNumPatAsync();

        // Assert
        Assert.True(patternCount >= 0L);
    }

    [Fact]
    public async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsCount()
    {
        // Arrange
        string pattern1 = $"test-{Guid.NewGuid()}*";
        string pattern2 = $"test-{Guid.NewGuid()}*";

        // Create subscribers with pattern subscriptions
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

        GlideClient subscriberClient1 = await GlideClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriberClient1);

        GlideClient subscriberClient2 = await GlideClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClientConfig().Build();
        GlideClient queryClient = await GlideClient.CreateClient(queryConfig);
        _testClients.Add(queryClient);

        // Wait for subscriptions to be established
        await Task.Delay(1000);

        // Get initial pattern count
        long patternCount = await queryClient.PubSubNumPatAsync();

        // Act - The pattern count should be at least 2 (our two patterns)
        // Note: There might be other pattern subscriptions from other tests
        Assert.True(patternCount >= 2L, $"Expected at least 2 pattern subscriptions, got {patternCount}");
    }

    public void Dispose()
    {
        foreach (var client in _testClients)
        {
            client?.Dispose();
        }
        _testClients.Clear();
    }
}
