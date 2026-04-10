// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IServer"/> pub/sub introspection commands.
/// </summary>
public class PubSubIntrospectionCommandTests(PubSubIntrospectionFixture fixture) : IClassFixture<PubSubIntrospectionFixture>
{
    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion
    #region Tests

    [Fact]
    public async Task SubscriptionChannelsAsync_NoSubscribers_ReturnsEmpty()
    {
        ValkeyChannel[] channels = await fixture.Server.SubscriptionChannelsAsync();

        Assert.Empty(channels);
    }

    [Fact]
    public async Task SubscriptionChannelsAsync_WithSubscriber_ReturnsChannel()
    {
        var channelName = $"test-pubsub-channels-{Guid.NewGuid()}";
        var channel = ValkeyChannel.Literal(channelName);

        await fixture.Subscriber.SubscribeAsync(channel, (_, _) => { });
        try
        {
            await Task.Delay(500);

            ValkeyChannel[] channels = await fixture.Server.SubscriptionChannelsAsync();

            Assert.Contains(channels, c => c.ToString() == channelName);
        }
        finally
        {
            await fixture.Subscriber.UnsubscribeAsync(channel);
            await Task.Delay(500);
        }
    }

    [Fact]
    public async Task SubscriptionChannelsAsync_WithPattern_FiltersChannels()
    {
        var prefix = $"pubsub-filter-{Guid.NewGuid():N}";
        var channel1 = ValkeyChannel.Literal($"{prefix}-a");
        var channel2 = ValkeyChannel.Literal($"{prefix}-b");
        var otherChannel = ValkeyChannel.Literal($"other-{Guid.NewGuid()}");

        await fixture.Subscriber.SubscribeAsync(channel1, (_, _) => { });
        await fixture.Subscriber.SubscribeAsync(channel2, (_, _) => { });
        await fixture.Subscriber.SubscribeAsync(otherChannel, (_, _) => { });
        try
        {
            await Task.Delay(500);

            var pattern = ValkeyChannel.Literal($"{prefix}-*");
            ValkeyChannel[] channels = await fixture.Server.SubscriptionChannelsAsync(pattern);

            Assert.Equal(2, channels.Length);
            Assert.Contains(channels, c => c.ToString() == $"{prefix}-a");
            Assert.Contains(channels, c => c.ToString() == $"{prefix}-b");
        }
        finally
        {
            await fixture.Subscriber.UnsubscribeAsync(channel1);
            await fixture.Subscriber.UnsubscribeAsync(channel2);
            await fixture.Subscriber.UnsubscribeAsync(otherChannel);
            await Task.Delay(500);
        }
    }

    [Fact]
    public async Task SubscriptionChannelsAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.SubscriptionChannelsAsync(flags: UnsupportedCommandFlag));

    [Fact]
    public async Task SubscriptionPatternCountAsync_NoPatterns_ReturnsZero()
    {
        long count = await fixture.Server.SubscriptionPatternCountAsync();

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task SubscriptionPatternCountAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.SubscriptionPatternCountAsync(flags: UnsupportedCommandFlag));

    [Fact]
    public async Task SubscriptionSubscriberCountAsync_NoSubscribers_ReturnsZero()
    {
        var channel = ValkeyChannel.Literal($"numsub-empty-{Guid.NewGuid()}");

        long count = await fixture.Server.SubscriptionSubscriberCountAsync(channel);

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task SubscriptionSubscriberCountAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.SubscriptionSubscriberCountAsync(
                ValkeyChannel.Literal("test-channel"),
                flags: UnsupportedCommandFlag));

    #endregion
}

/// <summary>
/// Fixture class for <see cref="PubSubIntrospectionCommandTests" />.
/// </summary>
public class PubSubIntrospectionFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer;
    private readonly ConnectionMultiplexer _connection;

    public IServer Server { get; }
    public IDatabase Database { get; }
    public ISubscriber Subscriber { get; }

    public PubSubIntrospectionFixture()
    {
        _standaloneServer = new();
        var (host, port) = _standaloneServer.Addresses.First();

        ConfigurationOptions config = new();
        config.EndPoints.Add(host, port);
        _connection = ConnectionMultiplexer.Connect(config);

        Server = _connection.GetServer(host, port);
        Database = _connection.GetDatabase();
        Subscriber = _connection.GetSubscriber();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _standaloneServer.Dispose();
    }
}
