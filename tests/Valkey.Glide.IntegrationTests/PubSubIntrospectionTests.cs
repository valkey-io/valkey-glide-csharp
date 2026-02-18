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
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubChannelsAsync_ReturnsEmpty(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var subscriber = await server.CreateClient();

        // Verify no active channels.
        Assert.Empty(await subscriber.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel(bool isCluster)
    {
        var message = BuildMessage(PubSubChannelMode.Exact);
        using var subscriber = await BuildSubscriber(isCluster, message);

        // Verify channel is active.
        Assert.Contains(message.Channel, await subscriber.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels(bool isCluster)
    {
        var message = BuildMessage(PubSubChannelMode.Exact);
        using var subscriber = await BuildSubscriber(isCluster, message);

        // Verify that channel matching pattern is returned.
        Assert.Contains(message.Channel, await subscriber.PubSubChannelsAsync(message.Channel));
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubNumSubAsync_WithNoSubscribers_ReturnsZeroCounts(bool isCluster)
    {
        var channel = BuildChannel();
        using var subscriber = await BuildSubscriber(isCluster);

        // Verify no subscribers to channel.
        var expected = new Dictionary<string, long> { { channel, 0L } };
        var actual = await subscriber.PubSubNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubNumSubAsync_WithSubscribers_ReturnsChannelCounts(bool isCluster)
    {
        var message1 = BuildMessage();
        var message2 = BuildMessage();

        using var subscriber = await BuildSubscriber(isCluster, [message1, message2]);

        // Verify subscription counts for both channels.
        var expected = new Dictionary<string, long> { { message1.Channel, 1L }, { message2.Channel, 1L } };
        var actual = await subscriber.PubSubNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubNumPatAsync_WithNoPatternSubscriptions_ReturnsZero(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var subscriber = await server.CreateClient();

        // Verify no active pattern subscriptions.
        Assert.Equal(0L, await subscriber.PubSubNumPatAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsPatternCount(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var subscriber = await server.CreateClient();

        // Subscribe to patterns and wait to ensure subscriptions complete.
        string[] patterns = ["pattern1*", "pattern2*"];
        await subscriber.PSubscribeLazyAsync(patterns);
        await Task.Delay(RetryInterval);

        // Verify active pattern subscriptions.
        Assert.Equal(2L, await subscriber.PubSubNumPatAsync());
    }

    [Fact]
    public static async Task PubSubShardChannelsAsync_WithNoChannels_ReturnsEmpty()
    {
        SkipUnlessShardedSupported();

        using var server = new ClusterServer();
        using var subscriber = await server.CreateClusterClient();

        // Verify no active channels.
        Assert.Empty(await subscriber.PubSubShardChannelsAsync());
    }

    [Fact]
    public static async Task PubSubShardChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        SkipUnlessShardedSupported();

        var message = BuildMessage(PubSubChannelMode.Sharded);
        using var subscriber = await BuildClusterSubscriber(message);

        // Verify that shard channel is active.
        Assert.Contains(message.Channel, await subscriber.PubSubShardChannelsAsync());
    }

    [Fact]
    public static async Task PubSubShardChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        SkipUnlessShardedSupported();

        var message = BuildMessage(PubSubChannelMode.Sharded);
        using var subscriber = await BuildClusterSubscriber(message);

        // Verify that shard channel is active.
        Assert.Equivalent(new[] { message.Channel }, await subscriber.PubSubShardChannelsAsync(message.Channel));
    }

    [Fact]
    public static async Task PubSubShardNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        SkipUnlessShardedSupported();

        var channel = BuildChannel();
        using var client = TestConfiguration.DefaultClusterClient();

        // Verify no subscribers to shard channel.
        var expected = new Dictionary<string, long> { { channel, 0L } };
        var actual = await client.PubSubShardNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public static async Task PubSubShardNumSubAsync_WithSubscribers_ReturnsShardChannelCounts()
    {
        SkipUnlessShardedSupported();

        var message1 = BuildMessage(PubSubChannelMode.Sharded);
        var message2 = BuildMessage(PubSubChannelMode.Sharded);

        var messages = new[] { message1, message2 };
        using var subscriber = await BuildClusterSubscriber(messages);

        // Verify subscription counts for both shard channels.
        var expected = new Dictionary<string, long> { { message1.Channel, 1L }, { message2.Channel, 1L } };
        var actual = await subscriber.PubSubShardNumSubAsync([message1.Channel, message2.Channel]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task GetSubscriptionsAsync_NoSubscriptions(bool isCluster)
    {
        using var subscriber = await BuildSubscriber(isCluster, []);
        var state = await subscriber.GetSubscriptionsAsync();

        // Verify subscriptions.
        foreach (var subscriptions in new[] { state.Desired, state.Actual })
        {
            foreach (var mode in Enum.GetValues<PubSubChannelMode>())
                Assert.Empty(subscriptions[mode]);
        }
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task GetSubscriptionsAsync_WithSubscriptions(bool isCluster)
    {
        // Build messages
        var messages = new List<PubSubMessage>()
        {
            BuildMessage(PubSubChannelMode.Exact),
            BuildMessage(PubSubChannelMode.Pattern),
        };

        if (IsShardedSupported(isCluster))
            messages.Add(BuildMessage(PubSubChannelMode.Sharded));

        using var subscriber = await BuildSubscriber(isCluster, messages, SubscribeMode.Config);
        await AssertSubscribedAsync(subscriber, messages);

        // Verify subscriptions.
        var state = await subscriber.GetSubscriptionsAsync();

        var targets = BuildSubscriptions(messages);
        Assert.Equivalent(targets, state.Desired);
        Assert.Equivalent(targets, state.Actual);
    }
}
