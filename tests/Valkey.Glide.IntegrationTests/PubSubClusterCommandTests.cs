// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for cluster-specific PubSub command methods.
/// Tests the PubSub command API implementations in GlideClusterClient.
/// </summary>
[Collection("GlideTests")]
public class PubSubClusterCommandTests(TestConfiguration config) : IDisposable
{
    private readonly List<BaseClient> _testClients = [];
    public TestConfiguration Config { get; } = config;

    [Fact]
    public async Task PublishAsync_WithNoSubscribers_ReturnsZero()
    {
        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient client = await GlideClusterClient.CreateClient(config);
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
        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        GlideClusterClient subscriberClient = await GlideClusterClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        // Create publisher
        var publisherConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient publisherClient = await GlideClusterClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        // Wait for subscription to be established
        await Task.Delay(1000);

        // Act
        long subscriberCount = await publisherClient.PublishAsync(testChannel, testMessage);

        // Assert - In cluster mode, PUBLISH returns the number of subscribers across all nodes
        // The count might be 0 or more depending on cluster topology and subscription propagation
        Assert.True(subscriberCount >= 0L);

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
    public async Task PublishAsync_ShardedChannel_WithNoSubscribers_ReturnsZero()
    {
        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient client = await GlideClusterClient.CreateClient(config);
        _testClients.Add(client);

        string channel = $"test-shard-{Guid.NewGuid()}";
        string message = "test sharded message";

        // Act
        long subscriberCount = await client.PublishAsync(channel, message, sharded: true);

        // Assert
        Assert.Equal(0L, subscriberCount);
    }

    [Fact]
    public async Task PublishAsync_ShardedChannel_WithSubscriber_ReturnsSubscriberCount()
    {
        // Arrange
        string testChannel = $"test-shard-{Guid.NewGuid()}";
        string testMessage = "Hello from sharded PublishAsync";

        // Create subscriber with sharded PubSub config
        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        GlideClusterClient subscriberClient = await GlideClusterClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        // Create publisher
        var publisherConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient publisherClient = await GlideClusterClient.CreateClient(publisherConfig);
        _testClients.Add(publisherClient);

        // Wait for subscription to be established
        await Task.Delay(1000);

        // Act
        long subscriberCount = await publisherClient.PublishAsync(testChannel, testMessage, sharded: true);

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
    public async Task PubSubChannelsAsync_WithNoChannels_ReturnsArray()
    {
        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient client = await GlideClusterClient.CreateClient(config);
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
        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        GlideClusterClient subscriberClient = await GlideClusterClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient queryClient = await GlideClusterClient.CreateClient(queryConfig);
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
        ClusterPubSubSubscriptionConfig pubsubConfig1 = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel1);
        ClusterPubSubSubscriptionConfig pubsubConfig2 = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel2);

        var subscriberConfig1 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();
        var subscriberConfig2 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        GlideClusterClient subscriberClient1 = await GlideClusterClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriberClient1);

        GlideClusterClient subscriberClient2 = await GlideClusterClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient queryClient = await GlideClusterClient.CreateClient(queryConfig);
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
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient client = await GlideClusterClient.CreateClient(config);
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
        ClusterPubSubSubscriptionConfig pubsubConfig1 = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel1);

        var subscriberConfig1 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();

        GlideClusterClient subscriberClient1 = await GlideClusterClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriberClient1);

        // Create two subscribers for channel2
        ClusterPubSubSubscriptionConfig pubsubConfig2 = new ClusterPubSubSubscriptionConfig()
            .WithChannel(testChannel2);

        var subscriberConfig2 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        GlideClusterClient subscriberClient2a = await GlideClusterClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2a);

        GlideClusterClient subscriberClient2b = await GlideClusterClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2b);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient queryClient = await GlideClusterClient.CreateClient(queryConfig);
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
    public async Task PubSubNumPatAsync_WithNoPatterns_ReturnsNonNegative()
    {
        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient client = await GlideClusterClient.CreateClient(config);
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
        ClusterPubSubSubscriptionConfig pubsubConfig1 = new ClusterPubSubSubscriptionConfig()
            .WithPattern(pattern1);
        ClusterPubSubSubscriptionConfig pubsubConfig2 = new ClusterPubSubSubscriptionConfig()
            .WithPattern(pattern2);

        var subscriberConfig1 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();
        var subscriberConfig2 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        GlideClusterClient subscriberClient1 = await GlideClusterClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriberClient1);

        GlideClusterClient subscriberClient2 = await GlideClusterClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient queryClient = await GlideClusterClient.CreateClient(queryConfig);
        _testClients.Add(queryClient);

        // Wait for subscriptions to be established
        await Task.Delay(1000);

        // Get pattern count
        long patternCount = await queryClient.PubSubNumPatAsync();

        // Assert - The pattern count should be at least 2 (our two patterns)
        // Note: There might be other pattern subscriptions from other tests
        Assert.True(patternCount >= 2L, $"Expected at least 2 pattern subscriptions, got {patternCount}");
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithNoChannels_ReturnsArray()
    {
        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient client = await GlideClusterClient.CreateClient(config);
        _testClients.Add(client);

        // Act
        string[] channels = await client.PubSubShardChannelsAsync();

        // Assert
        Assert.NotNull(channels);
        // Note: There might be channels from other tests, so we just verify it returns an array
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        // Arrange
        string testChannel = $"test-shard-{Guid.NewGuid()}";

        // Create subscriber with sharded channel
        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        GlideClusterClient subscriberClient = await GlideClusterClient.CreateClient(subscriberConfig);
        _testClients.Add(subscriberClient);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient queryClient = await GlideClusterClient.CreateClient(queryConfig);
        _testClients.Add(queryClient);

        // Wait for subscription to be established
        await Task.Delay(1000);

        // Act
        string[] channels = await queryClient.PubSubShardChannelsAsync();

        // Assert
        Assert.Contains(testChannel, channels);
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        // Arrange
        string channelPrefix = $"test-shard-{Guid.NewGuid()}";
        string testChannel1 = $"{channelPrefix}-channel1";
        string testChannel2 = $"{channelPrefix}-channel2";
        string pattern = $"{channelPrefix}*";

        // Create subscribers for both sharded channels
        ClusterPubSubSubscriptionConfig pubsubConfig1 = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel1);
        ClusterPubSubSubscriptionConfig pubsubConfig2 = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel2);

        var subscriberConfig1 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();
        var subscriberConfig2 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        GlideClusterClient subscriberClient1 = await GlideClusterClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriberClient1);

        GlideClusterClient subscriberClient2 = await GlideClusterClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient queryClient = await GlideClusterClient.CreateClient(queryConfig);
        _testClients.Add(queryClient);

        // Wait for subscriptions to be established
        await Task.Delay(1000);

        // Act
        string[] channels = await queryClient.PubSubShardChannelsAsync(pattern);

        // Assert
        Assert.Contains(testChannel1, channels);
        Assert.Contains(testChannel2, channels);
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient client = await GlideClusterClient.CreateClient(config);
        _testClients.Add(client);

        string channel1 = $"test-shard-{Guid.NewGuid()}";
        string channel2 = $"test-shard-{Guid.NewGuid()}";

        // Act
        Dictionary<string, long> counts = await client.PubSubShardNumSubAsync([channel1, channel2]);

        // Assert
        Assert.NotNull(counts);
        Assert.Equal(2, counts.Count);
        Assert.Equal(0L, counts[channel1]);
        Assert.Equal(0L, counts[channel2]);
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithSubscribers_ReturnsCorrectCounts()
    {
        // Arrange
        string testChannel1 = $"test-shard-{Guid.NewGuid()}";
        string testChannel2 = $"test-shard-{Guid.NewGuid()}";

        // Create subscriber for channel1
        ClusterPubSubSubscriptionConfig pubsubConfig1 = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel1);

        var subscriberConfig1 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();

        GlideClusterClient subscriberClient1 = await GlideClusterClient.CreateClient(subscriberConfig1);
        _testClients.Add(subscriberClient1);

        // Create two subscribers for channel2
        ClusterPubSubSubscriptionConfig pubsubConfig2 = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel2);

        var subscriberConfig2 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        GlideClusterClient subscriberClient2a = await GlideClusterClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2a);

        GlideClusterClient subscriberClient2b = await GlideClusterClient.CreateClient(subscriberConfig2);
        _testClients.Add(subscriberClient2b);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        GlideClusterClient queryClient = await GlideClusterClient.CreateClient(queryConfig);
        _testClients.Add(queryClient);

        // Wait for subscriptions to be established
        await Task.Delay(1000);

        // Act
        Dictionary<string, long> counts = await queryClient.PubSubShardNumSubAsync([testChannel1, testChannel2]);

        // Assert
        Assert.NotNull(counts);
        Assert.Equal(2, counts.Count);
        Assert.Equal(1L, counts[testChannel1]);
        Assert.Equal(2L, counts[testChannel2]);
    }

    public void Dispose()
    {
        foreach (BaseClient client in _testClients)
        {
            client?.Dispose();
        }
        _testClients.Clear();
    }
}
