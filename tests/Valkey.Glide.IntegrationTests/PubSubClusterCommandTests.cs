// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.PubSub;

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
    private static readonly string SkipSharedPubSubMessage = "Sharded PubSub is supported since 7.0.0";

    #region PublishCommands

    [Fact]
    public async Task SPublishAsync_WithNoSubscribers_ReturnsZero()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg = FromShardChannel();
        var client = TestConfiguration.DefaultClusterClient();

        // Publish to shard channel and verify no subscribers.
        Assert.Equal(0L, await client.SPublishAsync(msg.Channel, msg.Message));
    }

    [Fact]
    public async Task SPublishAsync_WithSubscriber_ReturnsSubscriberCount()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg = FromShardChannel();

        var subscriber = TestConfiguration.DefaultClusterClient();
        var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeAsync(msg.Channel);
        await AssertSSubscribedAsync(publisher, [msg.Channel]);

        // Publish to shard channel and verify subscriber count.
        await AssertSPublishAsync(publisher, msg);
    }

    #endregion
    #region SubscribeCommands

    [Fact]
    public async Task SSubscribeAsync_OneChannel_ReceivesMessage()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg = FromShardChannel();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeAsync(msg.Channel);
        await AssertSSubscribedAsync(publisher, [msg.Channel]);

        // Publish to shard channel and verify message received.
        await publisher.SPublishAsync(msg.Channel, msg.Message);
        await AssertReceivedAsync(subscriber, msg);
    }

    [Fact]
    public async Task SSubscribeAsync_MultipleChannels_ReceivesMessages()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg1 = FromShardChannel();
        var msg2 = FromShardChannel();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channels and verify subscriptions.
        await subscriber.SSubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSSubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to shard channels and verify messages received.
        await publisher.SPublishAsync(msg1.Channel, msg1.Message);
        await AssertReceivedAsync(subscriber, msg1);

        await publisher.SPublishAsync(msg2.Channel, msg2.Message);
        await AssertReceivedAsync(subscriber, msg2);
    }

    #endregion
    #region UnsubscribeCommands

    [Fact]
    public async Task SUnsubscribeAsync_AllChannels_ReceivesNoMessages()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg1 = FromShardChannel();
        var msg2 = FromShardChannel();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to both shard channels and verify subscriptions.
        await subscriber.SSubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSSubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from all shard channels and verify unsubscription.
        await subscriber.SUnsubscribeAsync();
        await AssertSUnsubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to shard channels and verify no messages received.
        await publisher.SPublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        await publisher.SPublishAsync(msg2.Channel, msg2.Message);
        await AssertNotReceivedAsync(subscriber, msg2);
    }

    [Fact]
    public async Task SUnsubscribeAsync_OneChannel_ReceivesNoMessages()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg1 = FromShardChannel();
        var msg2 = FromShardChannel();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to both shard channels and verify subscriptions.
        await subscriber.SSubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSSubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from one shard channel and verify unsubscription.
        await subscriber.SUnsubscribeAsync(msg1.Channel);
        await AssertSUnsubscribedAsync(publisher, [msg1.Channel]);

        // Publish to unsubscribed shard channel and verify no message received.
        await publisher.SPublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        // Publish to subscribed shard channel and verify message received.
        await publisher.SPublishAsync(msg2.Channel, msg2.Message);
        await AssertReceivedAsync(subscriber, msg2);
    }

    [Fact]
    public async Task SUnsubscribeAsync_MultipleChannels_ReceivesNoMessages()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg1 = FromShardChannel();
        var msg2 = FromShardChannel();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to both shard channels and verify subscriptions.
        await subscriber.SSubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSSubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from both shard channels and verify unsubscriptions.
        await subscriber.SUnsubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSUnsubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to both shard channels and verify no messages received.
        await publisher.SPublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        await publisher.SPublishAsync(msg2.Channel, msg2.Message);
        await AssertNotReceivedAsync(subscriber, msg2);
    }

    #endregion
    #region PubSubInfoCommands

    [Fact]
    public async Task PubSubShardChannelsAsync_WithNoChannels_ReturnsEmpty()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        using var server = new ClusterServer();
        using var client = await server.CreateClusterClient();

        // Verify no active channels.
        Assert.Empty(await client.PubSubShardChannelsAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithActiveSubscription_ReturnsChannel()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg = FromShardChannel();

        var publisher = TestConfiguration.DefaultClusterClient();
        var subscriber = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeAsync(msg.Channel);
        await AssertSSubscribedAsync(publisher, [msg.Channel]);

        // Verify that shard channel is active.
        Assert.Contains(msg.Channel, await publisher.PubSubShardChannelsAsync());
    }

    [Fact]
    public async Task PubSubShardChannelsAsync_WithPattern_ReturnsMatchingChannels()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg = FromShardChannel();

        using var subscriber = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channel and verify subscription.
        await subscriber.SSubscribeAsync(msg.Channel);
        await AssertSSubscribedAsync(publisher, [msg.Channel]);

        // Verify that shard channel is active.
        Assert.Contains(msg.Channel, await publisher.PubSubShardChannelsAsync(msg.Channel));
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithNoSubscribers_ReturnsZeroCounts()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg = FromShardChannel();
        using var client = TestConfiguration.DefaultClusterClient();

        // Verify no subscribers to shard channel.
        var expected = new Dictionary<string, long> { { msg.Channel, 0L } };
        var actual = await client.PubSubShardNumSubAsync([msg.Channel]);
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task PubSubShardNumSubAsync_WithSubscribers_ReturnsShardChannelCounts()
    {
        Assert.SkipUnless(IsSharedPubSubSupported, SkipSharedPubSubMessage);

        var msg1 = FromShardChannel();
        var msg2 = FromShardChannel();

        using var subscriber1 = TestConfiguration.DefaultClusterClient();
        using var subscriber2 = TestConfiguration.DefaultClusterClient();
        using var publisher = TestConfiguration.DefaultClusterClient();

        // Subscribe to shard channels and verify subscriptions.
        await subscriber1.SSubscribeAsync(msg1.Channel);
        await AssertSSubscribedAsync(publisher, [msg1.Channel]);

        await subscriber2.SSubscribeAsync(msg2.Channel);
        await AssertSSubscribedAsync(publisher, [msg2.Channel]);

        // Verify subscription counts for both shard channels.
        var expected = new Dictionary<string, long> { { msg1.Channel, 1L }, { msg2.Channel, 1L } };
        var actual = await publisher.PubSubShardNumSubAsync([msg1.Channel, msg2.Channel]);
        Assert.Equivalent(expected, actual);
    }

    #endregion
    #region HelperMethods

    /// <summary>
    /// Asserts that publishing the specified message results in at least one subscriber receiving it.
    /// </summary>
    /// <returns></returns>
    private static async Task AssertSPublishAsync(GlideClusterClient client, PubSubMessage message)
    {
        // Retry until published or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            if (await client.SPublishAsync(message.Channel, message.Message) > 0L)
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least 1 subscriber for shard channel '{message.Channel}'");
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified shard channels.
    /// </summary>
    private static async Task AssertSSubscribedAsync(GlideClusterClient client, string[] shardChannels)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var channelCounts = await client.PubSubShardNumSubAsync(shardChannels);
            if (channelCounts.All(kvp => kvp.Value > 0))
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least 1 subscriber for shard channels '{string.Join(", ", shardChannels)}'");
    }

    /// <summary>
    /// Asserts that there are no subscribers to each of the specified shard channels.
    /// </summary>
    private static async Task AssertSUnsubscribedAsync(GlideClusterClient client, string[] shardChannels)
    {
        // Retry until unsubscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var channelCounts = await client.PubSubShardNumSubAsync(shardChannels);
            if (channelCounts.All(kvp => kvp.Value == 0))
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected 0 subscribers for shard channels '{string.Join(", ", shardChannels)}'");
    }

    #endregion
}
