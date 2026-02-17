// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub introspection commands.
/// </summary>
[Collection(typeof(PubSubIntrospectionTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubIntrospectionTests()
{
    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PubSubChannelsAsync_WithNoChannels_ReturnsEmpty(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var client = await server.CreateClient();

        // Verify no active channels.
        Assert.Empty(await client.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel(bool isCluster)
    {
        var msg = BuildChannelMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeLazyAsync(msg.Channel);
        await AssertSubscribedAsync(subscriber, [msg.Channel]);

        // Verify that channel is active.
        Assert.Contains(msg.Channel, await publisher.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels(bool isCluster)
    {
        var msg = BuildPatternMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeLazyAsync(msg.Channel);
        await AssertSubscribedAsync(subscriber, [msg.Channel]);

        // Verify that channel matching pattern is active.
        Assert.Contains(msg.Channel, await publisher.PubSubChannelsAsync(msg.Pattern!));
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PubSubNumSubAsync_WithNoSubscribers_ReturnsZeroCounts(bool isCluster)
    {
        var msg = BuildChannelMessage();
        using var client = BuildClient(isCluster);

        // Verify no subscribers to channel.
        var expected = new Dictionary<string, long> { { msg.Channel, 0L } };
        var actual = await client.PubSubNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PubSubNumSubAsync_WithSubscribers_ReturnsChannelCounts(bool isCluster)
    {
        var msg1 = BuildChannelMessage();
        var msg2 = BuildChannelMessage();

        using var subscriber1 = BuildClient(isCluster);
        using var subscriber2 = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channels and verify subscriptions.
        await subscriber1.SubscribeLazyAsync(msg1.Channel);
        await AssertSubscribedAsync(subscriber1, [msg1.Channel]);

        await subscriber2.SubscribeLazyAsync(msg2.Channel);
        await AssertSubscribedAsync(subscriber2, [msg2.Channel]);

        // Verify subscription counts for both channels.
        var expected = new Dictionary<string, long> { { msg1.Channel, 1L }, { msg2.Channel, 1L } };
        var actual = await publisher.PubSubNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PubSubNumPatAsync_WithNoPatternSubscriptions_ReturnsZero(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var client = await server.CreateClient();

        // Verify no active pattern subscriptions.
        Assert.Equal(0L, await client.PubSubNumPatAsync());
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsPatternCount(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var subscriber = await server.CreateClient();
        using var publisher = await server.CreateClient();

        // Subscribe to patterns and wait to ensure subscriptions complete.
        string[] patterns = ["pattern1*", "pattern2*"];
        await subscriber.PSubscribeLazyAsync(patterns);
        await Task.Delay(RetryInterval);

        // Verify active pattern subscriptions.
        Assert.Equal(2L, await publisher.PubSubNumPatAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithNoChannels_ReturnsEmpty()
    {
        SkipUnlessShardedSupported();

        using var server = new ClusterServer();
        using var client = await server.CreateClusterClient();

        // Verify no active channels.
        Assert.Empty(await client.PubSubShardChannelsAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        SkipUnlessShardedSupported();

        var msg = BuildShardChannelMessage();

        var publisher = TestConfiguration.DefaultClusterClient();
        var subscriber = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeLazyAsync(msg.Channel);
        await AssertSSubscribedAsync(subscriber, [msg.Channel]);

        // Verify that shard channel is active.
        Assert.Contains(msg.Channel, await publisher.PubSubShardChannelsAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        SkipUnlessShardedSupported();

        var msg = BuildShardChannelMessage();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeLazyAsync(msg.Channel);
        await AssertSSubscribedAsync(subscriber, [msg.Channel]);

        // Verify that shard channel is active.
        Assert.Contains(msg.Channel, await publisher.PubSubShardChannelsAsync(msg.Channel));
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        SkipUnlessShardedSupported();

        var msg = BuildShardChannelMessage();
        using var client = TestConfiguration.DefaultClusterClient();

        // Verify no subscribers to shard channel.
        var expected = new Dictionary<string, long> { { msg.Channel, 0L } };
        var actual = await client.PubSubShardNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithSubscribers_ReturnsShardChannelCounts()
    {
        SkipUnlessShardedSupported();

        var msg1 = BuildShardChannelMessage();
        var msg2 = BuildShardChannelMessage();
        var channels = new string[] { msg1.Channel, msg2.Channel };

        using var subscriber1 = TestConfiguration.DefaultClusterClient();
        using var subscriber2 = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channels and verify subscriptions.
        await subscriber1.SSubscribeLazyAsync(channels);
        await AssertSSubscribedAsync(subscriber1, channels);

        // Verify subscription counts for both shard channels.
        var expected = new Dictionary<string, long> { { msg1.Channel, 1L }, { msg2.Channel, 1L } };
        var actual = await publisher.PubSubShardNumSubAsync(channels);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task GetSubscriptionsAsync_NoSubscriptions(bool isCluster)
    {
        using var subscriber = await BuildSubscriber(isCluster);
        var state = await subscriber.GetSubscriptionsAsync();

        foreach (var subscriptions in new[] { state.Desired, state.Actual })
        {
            Assert.Empty(subscriptions[PubSubChannelMode.Exact]);
            Assert.Empty(subscriptions[PubSubChannelMode.Pattern]);
            Assert.Empty(subscriptions[PubSubChannelMode.Sharded]);
        }
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task GetSubscriptionsAsync_WithSubscriptions(bool isCluster)
    {
        var msg = BuildPatternMessage();
        var channels = new string[] { msg.Channel };
        var patterns = new string[] { msg.Pattern! };
        var shardChannels = IsShardedSupported(isCluster) ? new string[] { msg.Channel } : [];

        using var subscriber = await BuildSubscriber(isCluster,
            channels: channels,
            patterns: patterns,
            shardChannels: shardChannels);

        var state = await subscriber.GetSubscriptionsAsync();

        foreach (var subscriptions in new[] { state.Desired, state.Actual })
        {
            Assert.Equivalent(channels, subscriptions[PubSubChannelMode.Exact]);
            Assert.Equivalent(patterns, subscriptions[PubSubChannelMode.Pattern]);
            Assert.Equivalent(shardChannels, subscriptions[PubSubChannelMode.Sharded]);
        }
    }
}
