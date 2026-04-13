// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.IntegrationTests.PubSubUtils;
using static Valkey.Glide.TestUtils.Data;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub introspection commands.
/// </summary>
[Collection(typeof(PubSubIntrospectionTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubIntrospectionTests(PubSubIntrospectionFixture fixture) : IClassFixture<PubSubIntrospectionFixture>
{
    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubChannelsAsync_ReturnsEmpty(bool isCluster)
    {
        BaseClient client = isCluster ? fixture.EmptyClusterClient! : fixture.EmptyStandaloneClient!;
        Assert.Empty(await client.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel(bool isCluster)
    {
        BaseClient client = isCluster ? fixture.ClusterClient! : fixture.StandaloneClient!;
        Assert.Equivalent(new[] { fixture.Channel }, await client.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels(bool isCluster)
    {
        BaseClient client = isCluster ? fixture.ClusterClient! : fixture.StandaloneClient!;
        Assert.Equivalent(new[] { fixture.Channel }, await client.PubSubChannelsAsync(fixture.MatchChannel));
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubNumSubAsync_WithNoSubscribers_ReturnsZeroCounts(bool isCluster)
    {
        var channel = fixture.Channel;
        BaseClient client = isCluster ? fixture.EmptyClusterClient! : fixture.EmptyStandaloneClient!;

        Assert.Equivalent(
            new Dictionary<ValkeyKey, long> { { channel, 0L } },
            await client.PubSubNumSubAsync([channel]));
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubNumSubAsync_WithSubscribers_ReturnsChannelCounts(bool isCluster)
    {
        var channel = fixture.Channel;
        BaseClient client = isCluster ? fixture.ClusterClient! : fixture.StandaloneClient!;

        Assert.Equivalent(
            new Dictionary<ValkeyKey, long> { { channel, 1L } },
            await client.PubSubNumSubAsync([channel]));
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubNumSubAsync_SingleChannel_WithNoSubscribers_ReturnsZero(bool isCluster)
    {
        var channel = fixture.Channel;
        BaseClient client = isCluster ? fixture.EmptyClusterClient! : fixture.EmptyStandaloneClient!;
        Assert.Equal(0L, await client.PubSubNumSubAsync(channel));
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubNumSubAsync_SingleChannel_WithSubscribers_ReturnsCount(bool isCluster)
    {
        var channel = fixture.Channel;
        BaseClient client = isCluster ? fixture.ClusterClient! : fixture.StandaloneClient!;
        Assert.Equal(1L, await client.PubSubNumSubAsync(channel));
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubNumPatAsync_WithNoPatternSubscriptions_ReturnsZero(bool isCluster)
    {
        BaseClient client = isCluster ? fixture.EmptyClusterClient! : fixture.EmptyStandaloneClient!;
        Assert.Equal(0L, await client.PubSubNumPatAsync());
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsPatternCount(bool isCluster)
    {
        BaseClient client = isCluster ? fixture.ClusterClient! : fixture.StandaloneClient!;
        Assert.Equal(1L, await client.PubSubNumPatAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithNoChannels_ReturnsEmpty()
    {
        SkipUnlessShardedSupported();

        GlideClusterClient client = fixture.EmptyClusterClient!;
        Assert.Empty(await client.PubSubShardChannelsAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        SkipUnlessShardedSupported();

        GlideClusterClient client = fixture.ClusterClient!;
        Assert.Equivalent(new[] { fixture.ShardedChannel }, await client.PubSubShardChannelsAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        SkipUnlessShardedSupported();

        GlideClusterClient client = fixture.ClusterClient!;
        Assert.Equivalent(new[] { fixture.ShardedChannel }, await client.PubSubShardChannelsAsync(fixture.MatchShardedChannel));
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        SkipUnlessShardedSupported();

        var channel = fixture.ShardedChannel;
        GlideClusterClient client = fixture.EmptyClusterClient!;

        Assert.Equivalent(
            new Dictionary<ValkeyKey, long> { { channel, 0L } },
            await client.PubSubShardNumSubAsync([channel]));
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithSubscribers_ReturnsShardedChannelCounts()
    {
        SkipUnlessShardedSupported();

        var shardChannel = fixture.ShardedChannel;
        GlideClusterClient client = fixture.ClusterClient!;

        Assert.Equivalent(
            new Dictionary<ValkeyKey, long> { { shardChannel, 1L } },
            await client.PubSubShardNumSubAsync([shardChannel]));
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task GetSubscriptionsAsync_NoSubscriptions(bool isCluster)
    {
        BaseClient client = isCluster ? fixture.EmptyClusterClient! : fixture.EmptyStandaloneClient!;
        PubSubState state = await client.GetSubscriptionsAsync();

        // Verify subscriptions.
        foreach (var subscriptions in new[] { state.Desired, state.Actual })
        {
            foreach (PubSubChannelMode mode in Enum.GetValues<PubSubChannelMode>())
            {
                Assert.Empty(subscriptions[mode]);
            }
        }
    }

    [Theory]
    [MemberData(nameof(ClusterMode), MemberType = typeof(Data))]
    public async Task GetSubscriptionsAsync_WithSubscriptions(bool isCluster)
    {
        BaseClient client = isCluster ? fixture.ClusterClient! : fixture.StandaloneClient!;
        PubSubState state = await client.GetSubscriptionsAsync();

        var expected = new Dictionary<PubSubChannelMode, HashSet<ValkeyKey>>()
        {
            [PubSubChannelMode.Exact] = [fixture.Channel],
            [PubSubChannelMode.Pattern] = [fixture.Pattern],
            [PubSubChannelMode.Sharded] = IsShardedSupported(isCluster) ? [fixture.ShardedChannel] : []
        };

        Assert.Equivalent(expected, state.Desired);
        Assert.Equivalent(expected, state.Actual);
    }
}

/// <summary>
/// Fixture class to manage server lifecycle for pub/sub introspection tests.
/// </summary>
public class PubSubIntrospectionFixture : IAsyncLifetime
{
    // Empty and populated servers.
    private StandaloneServer? _emptyStandaloneServer;
    private ClusterServer? _emptyClusterServer;
    private StandaloneServer? _standaloneServer;
    private ClusterServer? _clusterServer;

    // Empty and populated clients.
    public GlideClient? EmptyStandaloneClient;
    public GlideClusterClient? EmptyClusterClient;
    public GlideClient? StandaloneClient;
    public GlideClusterClient? ClusterClient;

    // Pub/sub channels for clients.
    public readonly ValkeyKey Channel = "CHANNEL";
    public readonly ValkeyKey Pattern = "PATTERN";
    public readonly ValkeyKey ShardedChannel = "SHARDED_CHANNEL";

    public readonly ValkeyKey MatchChannel = "CHANNEL*";
    public readonly ValkeyKey MatchShardedChannel = "SHARDED_CHANNEL*";

    public async ValueTask InitializeAsync()
    {
        // Build servers.
        _emptyStandaloneServer = new StandaloneServer();
        _emptyClusterServer = new ClusterServer();
        _standaloneServer = new StandaloneServer();
        _clusterServer = new ClusterServer();

        // Build clients.
        EmptyStandaloneClient = await _emptyStandaloneServer.CreateStandaloneClientAsync();
        EmptyClusterClient = await _emptyClusterServer.CreateClusterClientAsync();
        StandaloneClient = await _standaloneServer.CreateStandaloneClientAsync();
        ClusterClient = await _clusterServer.CreateClusterClientAsync();

        // Subscribe clients.
        foreach (BaseClient client in new BaseClient[] { StandaloneClient, ClusterClient })
        {
            await client.SubscribeAsync(Channel);
            await client.PSubscribeAsync(Pattern);
        }

        if (IsShardedSupported())
        {
            await ClusterClient.SSubscribeAsync(ShardedChannel);
        }
    }

    public ValueTask DisposeAsync()
    {
        // Dispose clients.
        BaseClient[] clients = [EmptyStandaloneClient!, EmptyClusterClient!, StandaloneClient!, ClusterClient!];
        foreach (BaseClient client in clients)
        {
            client.Dispose();
        }

        // Dispose servers.
        Server[] servers = [_emptyStandaloneServer!, _emptyClusterServer!, _standaloneServer!, _clusterServer!];
        foreach (Server server in servers)
        {
            server.Dispose();
        }

        return new ValueTask();
    }
}
