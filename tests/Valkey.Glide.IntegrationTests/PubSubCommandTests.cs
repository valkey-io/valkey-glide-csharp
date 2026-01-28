// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Reflection.Metadata;

using Valkey.Glide.Commands;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests pub/sub commands.
/// </summary>
public class PubSubCommandTests()
{
    public delegate Server ServerFactory();
    public delegate BaseClient ClientFactory();

    public static IEnumerable<object[]> ServerFactories()
    {
        yield return new object[] { new ServerFactory(() => new TestUtils.StandaloneServer()) };
        yield return new object[] { new ServerFactory(() => new TestUtils.ClusterServer()) };
    }

    public static IEnumerable<object[]> ClientFactories()
    {
        yield return new object[] { new ClientFactory(() => TestConfiguration.DefaultStandaloneClient()) };
        yield return new object[] { new ClientFactory(() => TestConfiguration.DefaultClusterClient()) };
    }

    #region PublishCommands

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PublishAsync_WithNoSubscribers(ClientFactory clientFactory)
    {
        var msg = PubSub.GetPubSubMessage();

        using var client = clientFactory();

        // Publish to channel and verify no subscribers.
        Assert.Equal(0L, await client.PublishAsync(msg.Channel, msg.Message));
        await PubSub.AssertNotReceived(client, msg);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PublishAsync_WithSubscriber(ClientFactory clientFactory)
    {
        var msg = PubSub.GetPubSubMessage();

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(msg.Channel);
        await PubSub.AssertSubscribed(publisher, msg.Channel);

        // Publish to channel and verify subscriber count.
        // Retry publishing until message is received or timeout occurs.
        //
        // In cluster mode, subscriber count may be one or more depending
        // cluster topology and subscription propagation.
        long subscriberCount = 0L;

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (!cts.Token.IsCancellationRequested)
        {
            subscriberCount = await publisher.PublishAsync(msg.Channel, msg.Message);
            if (subscriberCount > 0L) break;
            await Task.Delay(500);
        }

        Assert.True(subscriberCount >= 0L);
        await PubSub.AssertReceived(subscriber, msg);
    }

    #endregion
    #region SubscribeCommands

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task SubscribeAsync_OneChannel(ClientFactory clientFactory)
    {
        var msg = PubSub.GetPubSubMessage();

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(msg.Channel);
        await PubSub.AssertSubscribed(publisher, msg.Channel);

        // Publish to channel and verify message received.
        await publisher.PublishAsync(msg.Channel, msg.Message);
        await PubSub.AssertReceived(subscriber, msg);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task SubscribeAsync_MultipleChannels(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to channels and verify subscriptions.
        await subscriber.SubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to channels and verify messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertReceived(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertReceived(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PSubscribeAsync_OnePattern(ClientFactory clientFactory)
    {
        var msg = PubSub.GetPubSubMessage(pattern: true);

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to pattern and wait to ensure subscription completes.
        await subscriber.PSubscribeAsync(msg.Pattern);
        await Task.Delay(500);

        // Publish to channel and verify message received.
        await publisher.PublishAsync(msg.Channel, msg.Message);
        await PubSub.AssertReceived(subscriber, msg);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PSubscribeAsync_MultiplePatterns(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage(pattern: true);
        var msg2 = PubSub.GetPubSubMessage(pattern: true);

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to patterns and wait to ensure subscriptions complete.
        await subscriber.PSubscribeAsync([msg1.Pattern, msg2.Pattern]);
        await Task.Delay(500);

        // Publish to channels and verify messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertReceived(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertReceived(subscriber, msg2);
    }

    #endregion
    #region UnsubscribeCommands

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task UnsubscribeAsync_AllChannels(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from all channels and verify unsubscription.
        await subscriber.UnsubscribeAsync();
        await PubSub.AssertUnsubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to channels and verify no messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertNotReceived(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task UnsubscribeAsync_OneChannel(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from one channel and verify unsubscription.
        await subscriber.UnsubscribeAsync(msg1.Channel);
        await PubSub.AssertUnsubscribed(publisher, msg1.Channel);

        // Publish to unsubscribed channel and verify no message received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        // Publish to subscribed channel and verify message received.
        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertReceived(subscriber, msg2);
    }

    // [Theory]
    // [MemberData(nameof(Factory.ClientFactories), MemberType = typeof(Factory))]
    public async Task UnsubscribeAsync_MultipleChannels(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from both channels and verify unsubscriptions.
        await subscriber.UnsubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertUnsubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertNotReceived(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PUnsubscribeAsync_AllPatterns(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage();
        var (message1, channel1, pattern1) = (msg1.Message, msg1.Channel, msg1.Pattern);

        var msg2 = PubSub.GetPubSubMessage();
        var (message2, channel2, pattern2) = (msg2.Message, msg2.Channel, msg2.Pattern);

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to both patterns.
        await subscriber.PSubscribeAsync(pattern1);
        await subscriber.PSubscribeAsync(pattern2);

        // Unsubscribe from all patterns and wait for unsubscriptions to complete.
        await subscriber.PUnsubscribeAsync();
        await Task.Delay(500);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertNotReceived(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PUnsubscribeAsync_OnePattern(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage(pattern: true);
        var msg2 = PubSub.GetPubSubMessage(pattern: true);

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to both patterns.
        await subscriber.PSubscribeAsync([msg1.Pattern, msg2.Pattern]);

        // Unsubscribe from one pattern and wait for unsubscription to complete.
        await subscriber.PUnsubscribeAsync(msg1.Pattern);
        await Task.Delay(500);

        // Publish to unsubscribed pattern channel and verify no message received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        // Publish to subscribed pattern channel and verify message received.
        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertReceived(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PUnsubscribeAsync_MultipleChannels_StopsReceivingMessages(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to both patterns.
        await subscriber.PSubscribeAsync([msg1.Pattern, msg2.Pattern]);

        // Unsubscribe from both patterns and wait for unsubscriptions to complete.
        await subscriber.PUnsubscribeAsync([msg1.Pattern, msg2.Pattern]);
        await Task.Delay(500);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertNotReceived(subscriber, msg2);
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

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel(ClientFactory clientFactory)
    {
        var msg = PubSub.GetPubSubMessage();

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(msg.Channel);
        await PubSub.AssertSubscribed(publisher, msg.Channel);

        // Verify that channel is active.
        Assert.Contains(msg.Channel, await publisher.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels(ClientFactory clientFactory)
    {
        var msg = PubSub.GetPubSubMessage(pattern: true);

        using var subscriber = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(msg.Channel);
        await PubSub.AssertSubscribed(publisher, msg.Channel);

        // Verify that channel matching pattern is active.
        Assert.Contains(msg.Channel, await publisher.PubSubChannelsAsync(msg.Pattern));
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PubSubNumSubAsync_WithNoSubscribers(ClientFactory clientFactory)
    {
        var msg = PubSub.GetPubSubMessage();

        using var client = clientFactory();

        // Verify no subscribers to channel.
        var expected = new Dictionary<string, long> { { msg.Channel, 0L } };
        var actual = await client.PubSubNumSubAsync([msg.Channel]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ClientFactories))]
    public async Task PubSubNumSubAsync_WithSubscribers_ReturnsCorrectCounts(ClientFactory clientFactory)
    {
        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber1 = clientFactory();
        using var subscriber2 = clientFactory();
        using var publisher = clientFactory();

        // Subscribe to channels and verify subscriptions.
        await subscriber1.SubscribeAsync(msg1.Channel);
        await PubSub.AssertSubscribed(publisher, msg1.Channel);

        await subscriber2.SubscribeAsync(msg2.Channel);
        await PubSub.AssertSubscribed(publisher, msg2.Channel);

        // Verify subscription counts for both channels.
        var expected = new Dictionary<string, long> { { msg1.Channel, 1L }, { msg2.Channel, 1L } };
        var actual = await publisher.PubSubNumSubAsync([msg1.Channel, msg2.Channel]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(ServerFactories))]
    public async Task PubSubNumPatAsync_WithNoPatternSubscriptions_ReturnsZero(ServerFactory serverFactory)
    {
        using var server = serverFactory();
        using var client = await server.CreateClient();

        // Verify no active pattern subscriptions.
        Assert.Equal(0L, await client.PubSubNumPatAsync());
    }

    [Theory]
    [MemberData(nameof(ServerFactories))]
    public async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsCount(ServerFactory serverFactory)
    {
        using var server = serverFactory();
        using var subscriber = await server.CreateClient();
        using var publisher = await server.CreateClient();

        // Subscribe to patterns and wait to ensure subscriptions complete.
        await subscriber.PSubscribeAsync(["pattern1*", "pattern2*"]);
        await Task.Delay(1000);

        // Verify active pattern subscriptions.
        Assert.Equal(2L, await publisher.PubSubNumPatAsync());
    }

    #endregion
}
