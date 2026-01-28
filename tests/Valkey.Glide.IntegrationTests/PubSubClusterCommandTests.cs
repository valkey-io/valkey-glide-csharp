// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for cluster pub/sub commands.
/// </summary>
public class PubSubClusterCommandTests()
{
    [Fact]
    public async Task SPublishAsync_WithNoSubscribers_ReturnsZero()
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "Sharded PubSub is supported since 7.0.0");

        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        await using var client = await GlideClusterClient.CreateClient(config);

        string channel = $"test-shard-{Guid.NewGuid()}";
        string message = "test sharded message";

        // Act
        long subscriberCount = await client.SPublishAsync(channel, message);

        // Assert
        Assert.Equal(0L, subscriberCount);
    }

    [Fact]
    public async Task SPublishAsync_WithSubscriber_ReturnsSubscriberCount()
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "Sharded PubSub is supported since 7.0.0");

        // Arrange
        string testChannel = $"test-shard-{Guid.NewGuid()}";
        string testMessage = "Hello from sharded PublishAsync";

        // Create subscriber with sharded PubSub config
        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        await using var subscriberClient = await GlideClusterClient.CreateClient(subscriberConfig);

        // Create publisher
        var publisherConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        await using var publisherClient = await GlideClusterClient.CreateClient(publisherConfig);

        // Act - retry publishing until subscriber is registered or timeout.
        long subscriberCount = 0L;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (!cts.Token.IsCancellationRequested)
        {
            subscriberCount = await publisherClient.SPublishAsync(testChannel, testMessage);
            if (subscriberCount > 0L) break;
            await Task.Delay(500);
        }

        // Assert
        Assert.Equal(1L, subscriberCount);
        await AssertMessageReceived(subscriberClient, testChannel, testMessage);
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithNoChannels_ReturnsArray()
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "Sharded PubSub is supported since 7.0.0");

        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        await using var client = await GlideClusterClient.CreateClient(config);

        // Act
        string[] channels = await client.PubSubShardChannelsAsync();

        // Assert
        Assert.NotNull(channels);
        // Note: There might be channels from other tests, so we just verify it returns an array
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "Sharded PubSub is supported since 7.0.0");

        // Arrange
        string testChannel = $"test-shard-{Guid.NewGuid()}";

        // Create subscriber with sharded channel
        ClusterPubSubSubscriptionConfig pubsubConfig = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel);

        var subscriberConfig = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig)
            .Build();

        await using var subscriberClient = await GlideClusterClient.CreateClient(subscriberConfig);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        await using var queryClient = await GlideClusterClient.CreateClient(queryConfig);

        // Wait for subscription to be established - cluster mode may need more time
        await Task.Delay(2000);

        // Act
        string[] channels = await queryClient.PubSubShardChannelsAsync();

        // Assert
        Assert.Contains(testChannel, channels);
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "Sharded PubSub is supported since 7.0.0");

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

        await using var subscriberClient1 = await GlideClusterClient.CreateClient(subscriberConfig1);
        await using var subscriberClient2 = await GlideClusterClient.CreateClient(subscriberConfig2);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        await using var queryClient = await GlideClusterClient.CreateClient(queryConfig);

        // Wait for subscriptions to be established - cluster mode may need more time
        await Task.Delay(2000);

        // Act
        string[] channels = await queryClient.PubSubShardChannelsAsync(pattern);

        // Assert
        Assert.Contains(testChannel1, channels);
        Assert.Contains(testChannel2, channels);
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "Sharded PubSub is supported since 7.0.0");

        // Arrange
        var config = TestConfiguration.DefaultClusterClientConfig().Build();
        await using var client = await GlideClusterClient.CreateClient(config);

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
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "Sharded PubSub is supported since 7.0.0");

        // Arrange
        string testChannel1 = $"test-shard-{Guid.NewGuid()}";
        string testChannel2 = $"test-shard-{Guid.NewGuid()}";

        // Create subscriber for channel1
        ClusterPubSubSubscriptionConfig pubsubConfig1 = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel1);

        var subscriberConfig1 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig1)
            .Build();

        await using var subscriberClient1 = await GlideClusterClient.CreateClient(subscriberConfig1);

        // Create two subscribers for channel2
        ClusterPubSubSubscriptionConfig pubsubConfig2 = new ClusterPubSubSubscriptionConfig()
            .WithShardedChannel(testChannel2);

        var subscriberConfig2 = TestConfiguration.DefaultClusterClientConfig()
            .WithPubSubSubscriptions(pubsubConfig2)
            .Build();

        await using var subscriberClient2a = await GlideClusterClient.CreateClient(subscriberConfig2);
        await using var subscriberClient2b = await GlideClusterClient.CreateClient(subscriberConfig2);

        // Create query client
        var queryConfig = TestConfiguration.DefaultClusterClientConfig().Build();
        await using var queryClient = await GlideClusterClient.CreateClient(queryConfig);

        // Wait for subscriptions to be established - cluster mode may need more time
        await Task.Delay(2000);

        // Act
        Dictionary<string, long> counts = await queryClient.PubSubShardNumSubAsync([testChannel1, testChannel2]);

        // Assert
        Assert.NotNull(counts);
        Assert.Equal(2, counts.Count);
        Assert.Equal(1L, counts[testChannel1]);
        Assert.Equal(2L, counts[testChannel2]);
    }

    /// <summary>
    /// Asserts that the client receives a message on the expected channel with the expected content.
    /// </summary>
    /// <param name="client">The client expected to receive the message.</param>
    /// <param name="expectedChannel">The channel on which the message is expected.</param>
    /// <param name="expectedMessage">The expected message content.</param>
    private async Task AssertMessageReceived(GlideClusterClient client, string expectedChannel, string expectedMessage)
    {
        PubSubMessageQueue? queue = client.PubSubQueue;
        Assert.NotNull(queue);

        // Wait up to 5 seconds for the message.
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var receivedMessage = await queue.GetMessageAsync(cts.Token);

        Assert.Equal(expectedMessage, receivedMessage.Message);
        Assert.Equal(expectedChannel, receivedMessage.Channel);
    }
}
