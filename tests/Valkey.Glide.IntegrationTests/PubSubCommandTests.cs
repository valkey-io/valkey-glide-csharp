// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.PubSub;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub commands.
/// See <see cref="IPubSubCommands"/> and <see cref="GlideClient"/>.
/// </summary>
[Collection(typeof(PubSubCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubCommandTests()
{
    // Parametrized data to test both <see cref="GlideClusterClient"/> and <see cref="GlideStandaloneClient"/>.
    public static TheoryData<bool> IsCluster => [true, false];

    #region PublishCommands

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PublishAsync_WithNoSubscribers_ReturnsZero(bool isCluster)
    {
        var msg = ChannelMessage();
        using var client = BuildClient(isCluster);

        // Publish to channel and verify no subscribers.
        Assert.Equal(0L, await client.PublishAsync(msg.Channel, msg.Message));
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PublishAsync_WithSubscriber_ReturnsSubscriberCount(bool isCluster)
    {
        var msg = ChannelMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(msg.Channel);
        await AssertSubscribedAsync(publisher, [msg.Channel]);

        // Publish to channel and verify subscriber count.
        await AssertPublishAsync(publisher, msg);
    }

    #endregion
    #region SubscribeCommands

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task SubscribeAsync_OneChannel_ReceivesMessage(bool isCluster)
    {
        var msg = ChannelMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(msg.Channel);
        await AssertSubscribedAsync(publisher, [msg.Channel]);

        // Publish to channel and verify message received.
        await publisher.PublishAsync(msg.Channel, msg.Message);
        await AssertReceivedAsync(subscriber, msg);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task SubscribeAsync_MultipleChannels_ReceivesMessages(bool isCluster)
    {
        var msg1 = ChannelMessage();
        var msg2 = ChannelMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channels and verify subscriptions.
        await subscriber.SubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to channels and verify messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await AssertReceivedAsync(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await AssertReceivedAsync(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PSubscribeAsync_OnePattern_ReceivesMessage(bool isCluster)
    {
        var msg = PatternMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to pattern and wait to ensure subscription completes.
        await subscriber.PSubscribeAsync(msg.Pattern!);
        await Task.Delay(RetryInterval);

        // Publish to channel and verify message received.
        await publisher.PublishAsync(msg.Channel, msg.Message);
        await AssertReceivedAsync(subscriber, msg);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PSubscribeAsync_MultiplePatterns_ReceivesMessages(bool isCluster)
    {
        var msg1 = PatternMessage();
        var msg2 = PatternMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to patterns and wait to ensure subscriptions complete.
        await subscriber.PSubscribeAsync([msg1.Pattern!, msg2.Pattern!]);
        await Task.Delay(RetryInterval);

        // Publish to channels and verify messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await AssertReceivedAsync(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await AssertReceivedAsync(subscriber, msg2);
    }

    #endregion
    #region UnsubscribeCommands

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task UnsubscribeAsync_AllChannels_ReceivesNoMessages(bool isCluster)
    {
        var msg1 = ChannelMessage();
        var msg2 = ChannelMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from all channels and verify unsubscription.
        await subscriber.UnsubscribeAsync();
        await AssertUnsubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to channels and verify no messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await AssertNotReceivedAsync(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task UnsubscribeAsync_OneChannel(bool isCluster)
    {
        var msg1 = ChannelMessage();
        var msg2 = ChannelMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from one channel and verify unsubscription.
        await subscriber.UnsubscribeAsync(msg1.Channel);
        await AssertUnsubscribedAsync(publisher, [msg1.Channel]);

        // Publish to unsubscribed channel and verify no message received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        // Publish to subscribed channel and verify message received.
        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await AssertReceivedAsync(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task UnsubscribeAsync_MultipleChannels_ReceivesNoMessages(bool isCluster)
    {
        var msg1 = ChannelMessage();
        var msg2 = ChannelMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to both channels and verify subscriptions.
        await subscriber.SubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertSubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Unsubscribe from both channels and verify unsubscriptions.
        await subscriber.UnsubscribeAsync([msg1.Channel, msg2.Channel]);
        await AssertUnsubscribedAsync(publisher, [msg1.Channel, msg2.Channel]);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await AssertNotReceivedAsync(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PUnsubscribeAsync_AllPatterns_ReceivesNoMessages(bool isCluster)
    {
        var msg1 = PatternMessage();
        var msg2 = PatternMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to both patterns.
        await subscriber.PSubscribeAsync(msg1.Pattern!);
        await subscriber.PSubscribeAsync(msg2.Pattern!);

        // Unsubscribe from all patterns and wait for unsubscriptions to complete.
        await subscriber.PUnsubscribeAsync();
        await Task.Delay(RetryInterval);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await AssertNotReceivedAsync(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PUnsubscribeAsync_OnePattern_ReceivesNoMessages(bool isCluster)
    {
        var msg1 = PatternMessage();
        var msg2 = PatternMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to both patterns.
        await subscriber.PSubscribeAsync([msg1.Pattern!, msg2.Pattern!]);

        // Unsubscribe from one pattern and wait for unsubscription to complete.
        await subscriber.PUnsubscribeAsync(msg1.Pattern!);
        await Task.Delay(RetryInterval);

        // Publish to unsubscribed pattern channel and verify no message received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        // Publish to subscribed pattern channel and verify message received.
        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await AssertReceivedAsync(subscriber, msg2);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PUnsubscribeAsync_MultiplePatterns_ReceivesNoMessages(bool isCluster)
    {
        var msg1 = PatternMessage();
        var msg2 = PatternMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to both patterns.
        await subscriber.PSubscribeAsync([msg1.Pattern!, msg2.Pattern!]);

        // Unsubscribe from both patterns and wait for unsubscriptions to complete.
        await subscriber.PUnsubscribeAsync([msg1.Pattern!, msg2.Pattern!]);
        await Task.Delay(RetryInterval);

        // Publish to both channels and verify no messages received.
        await publisher.PublishAsync(msg1.Channel, msg1.Message);
        await AssertNotReceivedAsync(subscriber, msg1);

        await publisher.PublishAsync(msg2.Channel, msg2.Message);
        await AssertNotReceivedAsync(subscriber, msg2);
    }

    #endregion
    #region PubSubInfoCommands

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PubSubChannelsAsync_WithNoChannels_ReturnsEmpty(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var client = await server.CreateClient();

        // Verify no active channels.
        Assert.Empty(await client.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PubSubChannelsAsync_WithActiveSubscription_ReturnsChannel(bool isCluster)
    {
        var msg = ChannelMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channel aqnd verify subscription.
        await subscriber.SubscribeAsync(msg.Channel);
        await AssertSubscribedAsync(publisher, [msg.Channel]);

        // Verify that channel is active.
        Assert.Contains(msg.Channel, await publisher.PubSubChannelsAsync());
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PubSubChannelsAsync_WithPattern_ReturnsMatchingChannels(bool isCluster)
    {
        var msg = PatternMessage();

        using var subscriber = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channel and verify subscription.
        await subscriber.SubscribeAsync(msg.Channel);
        await AssertSubscribedAsync(publisher, [msg.Channel]);

        // Verify that channel matching pattern is active.
        Assert.Contains(msg.Channel, await publisher.PubSubChannelsAsync(msg.Pattern!));
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PubSubNumSubAsync_WithNoSubscribers_ReturnsZeroCounts(bool isCluster)
    {
        var msg = ChannelMessage();
        using var client = BuildClient(isCluster);

        // Verify no subscribers to channel.
        var expected = new Dictionary<string, long> { { msg.Channel, 0L } };
        var actual = await client.PubSubNumSubAsync([msg.Channel]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PubSubNumSubAsync_WithSubscribers_ReturnsChannelCounts(bool isCluster)
    {
        var msg1 = ChannelMessage();
        var msg2 = ChannelMessage();

        using var subscriber1 = BuildClient(isCluster);
        using var subscriber2 = BuildClient(isCluster);
        using var publisher = BuildClient(isCluster);

        // Subscribe to channels and verify subscriptions.
        await subscriber1.SubscribeAsync(msg1.Channel);
        await AssertSubscribedAsync(publisher, [msg1.Channel]);

        await subscriber2.SubscribeAsync(msg2.Channel);
        await AssertSubscribedAsync(publisher, [msg2.Channel]);

        // Verify subscription counts for both channels.
        var expected = new Dictionary<string, long> { { msg1.Channel, 1L }, { msg2.Channel, 1L } };
        var actual = await publisher.PubSubNumSubAsync([msg1.Channel, msg2.Channel]);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PubSubNumPatAsync_WithNoPatternSubscriptions_ReturnsZero(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var client = await server.CreateClient();

        // Verify no active pattern subscriptions.
        Assert.Equal(0L, await client.PubSubNumPatAsync());
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task PubSubNumPatAsync_WithPatternSubscriptions_ReturnsPatternCount(bool isCluster)
    {
        using var server = BuildServer(isCluster);
        using var subscriber = await server.CreateClient();
        using var publisher = await server.CreateClient();

        // Subscribe to patterns and wait to ensure subscriptions complete.
        await subscriber.PSubscribeAsync(["pattern1*", "pattern2*"]);
        await Task.Delay(RetryInterval);

        // Verify active pattern subscriptions.
        Assert.Equal(2L, await publisher.PubSubNumPatAsync());
    }

    #endregion
    #region HelperMethods

    /// <summary>
    /// Builds and returns a Valkey server.
    /// </summary>
    private static Server BuildServer(bool isCluster)
    {
        return isCluster
        ? new ClusterServer()
        : new StandaloneServer();
    }

    /// <summary>
    /// Builds and returns a Valkey client.
    /// </summary>
    private static BaseClient BuildClient(bool isCluster)
    {
        return isCluster
        ? TestConfiguration.DefaultStandaloneClient()
        : TestConfiguration.DefaultClusterClient();
    }

    /// <summary>
    /// Asserts that publishing the specified message results in at least one subscriber receiving it.
    /// </summary>
    /// <returns></returns>
    private static async Task AssertPublishAsync(BaseClient client, PubSubMessage message)
    {
        // Retry until published or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            if (await client.PublishAsync(message.Channel, message.Message) > 0L)
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least one subscriber for channel '{message.Channel}'.");
    }

    /// <summary>
    /// Asserts that there is at least one subscriber to each of the specified channels.
    /// </summary>
    private static async Task AssertSubscribedAsync(BaseClient client, string[] channels)
    {
        // Retry until subscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var channelCounts = await client.PubSubNumSubAsync(channels);
            if (channelCounts.All(kvp => kvp.Value > 0))
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected at least 1 subscriber for channels '{string.Join(", ", channels)}'");
    }

    /// <summary>
    /// Asserts that there are no subscribers to each of the specified channels.
    /// </summary>
    private static async Task AssertUnsubscribedAsync(BaseClient client, string[] channels)
    {
        // Retry until unsubscribed or timeout occurs.
        using var cts = new CancellationTokenSource(MaxDuration);

        while (!cts.Token.IsCancellationRequested)
        {
            var channelCounts = await client.PubSubNumSubAsync(channels);
            if (channelCounts.All(kvp => kvp.Value == 0))
                return;

            await Task.Delay(RetryInterval);
        }

        Assert.Fail($"Expected 0 subscribers for channels '{string.Join(", ", channels)}'");
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task GetSubscriptionsAsync_NoSubscriptions(bool isCluster)
    {
        using var client = BuildClient(isCluster);
        var state = await client.GetSubscriptionsAsync();

        var desired = state.Desired;
        Assert.Empty(desired[PubSubChannelMode.Exact]);
        Assert.Empty(desired[PubSubChannelMode.Pattern]);

        var actual = state.Actual;
        Assert.Empty(actual[PubSubChannelMode.Exact]);
        Assert.Empty(actual[PubSubChannelMode.Pattern]);
    }

    [Theory]
    [MemberData(nameof(IsCluster))]
    public async Task GetSubscriptionsAsync_WithSubscriptions(bool isCluster)
    {
        var msg = BuildMessage(pattern: true);
        using var client = BuildClient(isCluster);

        await client.SubscribeAsync(msg.Channel);
        await client.PSubscribeAsync(msg.Pattern!);

        var state = await client.GetSubscriptionsAsync();

        var desired = state.Desired;
        Assert.Equivalent([msg.Channel], desired[PubSubChannelMode.Exact]);
        Assert.Equivalent([msg.Pattern!], desired[PubSubChannelMode.Pattern]);

        var actual = state.Actual;
        Assert.Empty(actual[PubSubChannelMode.Exact]);
        Assert.Empty(actual[PubSubChannelMode.Pattern]);

        // Default reconciliation delay is 3 seconds.
        await Task.Delay(TimeSpan.FromSeconds(3));

        state = await client.GetSubscriptionsAsync();

        desired = state.Desired;
        Assert.Equivalent([msg.Channel], desired[PubSubChannelMode.Exact]);
        Assert.Equivalent([msg.Pattern!], desired[PubSubChannelMode.Pattern]);

        actual = state.Actual;
        Assert.Equivalent([msg.Channel], actual[PubSubChannelMode.Exact]);
        Assert.Equivalent([msg.Pattern!], actual[PubSubChannelMode.Pattern]);
    }

    #endregion
}
