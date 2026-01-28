// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub cluster commands.
/// See <see cref="IPubSubClusterCommands"/> and <see cref="GlideClusterClient"/>.
/// </summary>
[Collection(typeof(PubSubClusterCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubClusterCommandTests()
{
    // Skip tests if Valkey GLIDE version is less than 7.0.0
    private static readonly bool IsSharedPubSubSupported = TestConfiguration.IsVersionAtLeast("7.0.0");
    private static readonly string SkipMessage = "Sharded PubSub is supported since 7.0.0";

    #region PublishCommands

    [Fact]
    public async Task SPublishAsync_WithNoSubscribers_ReturnsZero()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg = PubSub.GetPubSubMessage();
        var client = TestConfiguration.DefaultClusterClient();

        // Publish to shard channel and verify no subscribers.
        Assert.Equal(0L, await client.SPublishAsync(msg.Channel, msg.Message));
    }

    [Fact]
    public async Task SPublishAsync_WithSubscriber_ReturnsSubscriberCount()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg = PubSub.GetPubSubMessage();

        var subscriber = TestConfiguration.DefaultClusterClient();
        var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeAsync(msg.Channel);
        await PubSub.AssertSSubscribed(publisher, [msg.Channel]);

        // Publish to shard channel and verify subscriber count.
        // Retry publishing until message is received or timeout occurs.
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (!cts.Token.IsCancellationRequested)
        {
            if (await publisher.SPublishAsync(msg.Channel, msg.Message) == 1L)
                return;

            await Task.Delay(500);
        }

        Assert.Fail("Expected 1 subscriber to receive the published message.");
    }

    #endregion
    #region SubscribeCommands

    [Fact]
    public async Task SSubscribeAsync_OneChannel_ReceivesMessage()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg = PubSub.GetPubSubMessage();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeAsync(msg.Channel);
        await PubSub.AssertSSubscribed(publisher, [msg.Channel]);

        // Publish to shard channel and verify message received.
        await publisher.SPublishAsync(msg.Channel, msg.Message);
        await PubSub.AssertReceived(subscriber, msg);
    }

    [Fact]
    public async Task SSubscribeAsync_MultipleChannels_ReceivesMessages()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channels and verify subscriptions.
        await subscriber.SSubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSSubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to shard channels and verify messages received.
        await publisher.SPublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertReceived(subscriber, msg1);

        await publisher.SPublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertReceived(subscriber, msg2);
    }

    #endregion
    #region UnsubscribeCommands

    [Fact]
    public async Task SUnsubscribeAsync_AllChannels_ReceivesNoMessages()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to both shard channels and verify subscriptions.
        await subscriber.SSubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSSubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from all shard channels and verify unsubscription.
        await subscriber.SUnsubscribeAsync();
        await PubSub.AssertUnsubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to shard channels and verify no messages received.
        await publisher.SPublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        await publisher.SPublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertNotReceived(subscriber, msg2);
    }

    [Fact]
    public async Task SUnsubscribeAsync_OneChannel_ReceivesNoMessages()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to both shard channels and verify subscriptions.
        await subscriber.SSubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSSubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from one shard channel and verify unsubscription.
        await subscriber.SUnsubscribeAsync(msg1.Channel);
        await PubSub.AssertSUnsubscribed(publisher, [msg1.Channel]);

        // Publish to unsubscribed shard channel and verify no message received.
        await publisher.SPublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        // Publish to subscribed shard channel and verify message received.
        await publisher.SPublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertReceived(subscriber, msg2);
    }

    [Fact]
    public async Task SUnsubscribeAsync_MultipleChannels_ReceivesNoMessages()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to both shard channels and verify subscriptions.
        await subscriber.SSubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSSubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from both shard channels and verify unsubscriptions.
        await subscriber.SUnsubscribeAsync([msg1.Channel, msg2.Channel]);
        await PubSub.AssertSUnsubscribed(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to both shard channels and verify no messages received.
        await publisher.SPublishAsync(msg1.Channel, msg1.Message);
        await PubSub.AssertNotReceived(subscriber, msg1);

        await publisher.SPublishAsync(msg2.Channel, msg2.Message);
        await PubSub.AssertNotReceived(subscriber, msg2);
    }

    #endregion
    #region PubSubInfoCommands

    [Fact]
    public async Task PubSubShardChannelsAsync_WithNoChannels_ReturnsEmpty()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        using var server = new ClusterServer();
        using var client = await server.CreateClusterClient();

        // Verify no active channels.
        Assert.Empty(await client.PubSubShardChannelsAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg = PubSub.GetPubSubMessage();

        var publisher = TestConfiguration.DefaultClusterClient();
        var subscriber = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeAsync(msg.Channel);
        await PubSub.AssertSSubscribed(publisher, [msg.Channel]);

        // Verify that shard channel is active.
        Assert.Contains(msg.Channel, await publisher.PubSubShardChannelsAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg = PubSub.GetPubSubMessage(pattern: true);

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeAsync(msg.Channel);
        await PubSub.AssertSSubscribed(publisher, [msg.Channel]);

        // Verify that shard channel matching pattern is active.
        Assert.Contains(msg.Channel, await publisher.PubSubShardChannelsAsync(msg.Pattern!));
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg = PubSub.GetPubSubMessage();
        using var client = TestConfiguration.DefaultClusterClient();

        // Verify no subscribers to shard channel.
        var expected = new Dictionary<string, long> { { msg.Channel, 0L } };
        var actual = await client.PubSubShardNumSubAsync([msg.Channel]);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithSubscribers_ReturnsShardChannelCounts()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipMessage);

        var msg1 = PubSub.GetPubSubMessage();
        var msg2 = PubSub.GetPubSubMessage();

        using var subscriber1 = TestConfiguration.DefaultClusterClient();
        using var subscriber2 = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channels and verify subscriptions.
        await subscriber1.SSubscribeAsync(msg1.Channel);
        await PubSub.AssertSSubscribed(publisher, [msg1.Channel]);

        await subscriber2.SSubscribeAsync(msg2.Channel);
        await PubSub.AssertSSubscribed(publisher, [msg2.Channel]);

        // Verify subscription counts for both shard channels.
        var expected = new Dictionary<string, long> { { msg1.Channel, 1L }, { msg2.Channel, 1L } };
        var actual = await publisher.PubSubShardNumSubAsync([msg1.Channel, msg2.Channel]);
        Assert.Equivalent(expected, actual);
    }

    #endregion
}
