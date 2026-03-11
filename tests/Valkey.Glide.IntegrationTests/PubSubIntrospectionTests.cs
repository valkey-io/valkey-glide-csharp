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
        using Server server = BuildServer(isCluster);
        using BaseClient subscriber = await server.CreateClientAsync();

        // Verify no active channels.
        Assert.Empty(await subscriber.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel(bool isCluster)
    {
        PubSubMessage message = BuildMessage(PubSubChannelMode.Exact);
        using BaseClient subscriber = await BuildSubscriber(isCluster, message);

        // Verify channel is active.
        Assert.Contains(message.Channel, await subscriber.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels(bool isCluster)
    {
        PubSubMessage message = BuildMessage(PubSubChannelMode.Exact);
        using BaseClient subscriber = await BuildSubscriber(isCluster, message);

        // Create matching pattern by replacing 'channel' with wildcard.
        string pattern = message.Channel.Replace("channel", "*");

        // Verify that channel matching pattern is returned.
        Assert.Contains(message.Channel, await subscriber.PubSubChannelsAsync(pattern));
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubNumSubAsync_WithNoSubscribers_ReturnsZeroCounts(bool isCluster)
    {
        string channel = BuildChannel();
        using BaseClient subscriber = await BuildSubscriber(isCluster);

        // Verify no subscribers to channel.
        Dictionary<string, long> expected = new() { { channel, 0L } };
        Dictionary<string, long> actual = await subscriber.PubSubNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubNumSubAsync_WithSubscribers_ReturnsChannelCounts(bool isCluster)
    {
        PubSubMessage message1 = BuildMessage();
        PubSubMessage message2 = BuildMessage();

        using BaseClient subscriber = await BuildSubscriber(isCluster, [message1, message2]);

        // Verify subscription counts for both channels.
        Dictionary<string, long> expected = new() { { message1.Channel, 1L }, { message2.Channel, 1L } };
        Dictionary<string, long> actual = await subscriber.PubSubNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubNumPatAsync_WithNoPatternSubscriptions_ReturnsZero(bool isCluster)
    {
        using Server server = BuildServer(isCluster);
        using BaseClient subscriber = await server.CreateClientAsync();

        // Verify no active pattern subscriptions.
        Assert.Equal(0L, await subscriber.PubSubNumPatAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsPatternCount(bool isCluster)
    {
        using Server server = BuildServer(isCluster);
        using BaseClient subscriber = await server.CreateClientAsync();

        // Subscribe to patterns and wait to ensure subscriptions complete.
        string[] patterns = ["pattern1*", "pattern2*"];
        await subscriber.PSubscribeLazyAsync(patterns);
        await Task.Delay(RetryInterval, TestContext.Current.CancellationToken);

        // Verify active pattern subscriptions.
        Assert.Equal(2L, await subscriber.PubSubNumPatAsync());
    }

    [Fact]
    public static async Task PubSubShardChannelsAsync_WithNoChannels_ReturnsEmpty()
    {
        SkipUnlessShardedSupported();

        using ClusterServer server = new();
        using GlideClusterClient subscriber = await server.CreateClusterClientAsync();

        // Verify no active channels.
        Assert.Empty(await subscriber.PubSubShardChannelsAsync());
    }

    [Fact]
    public static async Task PubSubShardChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        SkipUnlessShardedSupported();

        PubSubMessage message = BuildMessage(PubSubChannelMode.Sharded);
        using GlideClusterClient subscriber = await BuildClusterSubscriber(message);

        // Verify that sharded channel is active.
        Assert.Contains(message.Channel, await subscriber.PubSubShardChannelsAsync());
    }

    [Fact]
    public static async Task PubSubShardChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        SkipUnlessShardedSupported();

        PubSubMessage message = BuildMessage(PubSubChannelMode.Sharded);
        using GlideClusterClient subscriber = await BuildClusterSubscriber(message);

        // Verify that sharded channel is active.
        Assert.Equivalent(new[] { message.Channel }, await subscriber.PubSubShardChannelsAsync(message.Channel));
    }

    [Fact]
    public static async Task PubSubShardNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        SkipUnlessShardedSupported();

        string channel = BuildChannel();
        using GlideClusterClient client = TestConfiguration.DefaultClusterClient();

        // Verify no subscribers to sharded channel.
        Dictionary<string, long> expected = new() { { channel, 0L } };
        Dictionary<string, long> actual = await client.PubSubShardNumSubAsync([.. expected.Keys]);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public static async Task PubSubShardNumSubAsync_WithSubscribers_ReturnsShardedChannelCounts()
    {
        SkipUnlessShardedSupported();

        PubSubMessage message1 = BuildMessage(PubSubChannelMode.Sharded);
        PubSubMessage message2 = BuildMessage(PubSubChannelMode.Sharded);

        PubSubMessage[] messages = [message1, message2];
        using GlideClusterClient subscriber = await BuildClusterSubscriber(messages);

        // Verify subscription counts for both sharded channels.
        Dictionary<string, long> expected = new() { { message1.Channel, 1L }, { message2.Channel, 1L } };
        Dictionary<string, long> actual = await subscriber.PubSubShardNumSubAsync([message1.Channel, message2.Channel]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task GetSubscriptionsAsync_NoSubscriptions(bool isCluster)
    {
        using BaseClient subscriber = await BuildSubscriber(isCluster, []);
        PubSubState state = await subscriber.GetSubscriptionsAsync();

        // Verify subscriptions.
        foreach (IReadOnlyDictionary<PubSubChannelMode, IReadOnlySet<string>>? subscriptions in new[] { state.Desired, state.Actual })
        {
            foreach (PubSubChannelMode mode in Enum.GetValues<PubSubChannelMode>())
            {
                Assert.Empty(subscriptions[mode]);
            }
        }
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task GetSubscriptionsAsync_WithSubscriptions(bool isCluster)
    {
        // Build messages
        List<PubSubMessage> messages =
        [
            BuildMessage(PubSubChannelMode.Exact),
            BuildMessage(PubSubChannelMode.Pattern),
        ];

        if (IsShardedSupported(isCluster))
        {
            messages.Add(BuildMessage(PubSubChannelMode.Sharded));
        }

        using BaseClient subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Verify subscriptions.
        PubSubState state = await subscriber.GetSubscriptionsAsync();

        Dictionary<PubSubChannelMode, HashSet<string>> targets = BuildSubscriptions(messages);
        Assert.Equivalent(targets, state.Desired);
        Assert.Equivalent(targets, state.Actual);
    }
}
