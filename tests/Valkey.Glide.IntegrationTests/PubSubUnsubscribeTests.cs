// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub unsubscribe operations.
/// Tests more unsunscribe scenarios compared to <see cref="PubSubBasicTests"/>.
/// </summary>
[Collection(typeof(PubSubUnsubscribeTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubUnsubscribeTests
{
    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_SingleChannelMode_UnsubscribesManySuccessfully(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build many messages for the specified channel mode.
        var messagesCount = 256;
        var messages = Enumerable.Range(0, messagesCount)
            .Select(_ => BuildMessage(channelMode))
            .ToArray();

        // Build subscriber and verify its active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from all channels/pattern except one and verify subscriptions.
        await UnsubscribeAsync(subscriber, messages[..^1], unsubscribeMode);
        await AssertNotSubscribedAsync(subscriber, messages[..^1], unsubscribeMode);
        await AssertSubscribedAsync(subscriber, messages[^1]);
    }

    [Theory]
    [MemberData(nameof(ClusterChannelAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task AllSubscriptions_SingleChannelMode_UnsubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        // Build messages for the specified channel mode.
        var messageCount = 256;
        var messages = Enumerable.Range(0, messageCount)
            .Select(_ => BuildMessage(channelMode))
            .ToArray();

        // Test unsubscribe with constants
        // -------------------------------

        // Build subscriber and verify its active.
        using var allSubscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(allSubscriber, messages);

        // Unsubscribe from all and verify subscriptions.
        if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Exact)
            await allSubscriber.UnsubscribeLazyAsync(PubSub.AllChannels);
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Exact)
            await allSubscriber.UnsubscribeAsync(PubSub.AllChannels);
        else if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Pattern)
            await allSubscriber.PUnsubscribeLazyAsync(PubSub.AllPatterns);
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Pattern)
            await allSubscriber.PUnsubscribeAsync(PubSub.AllPatterns);
        else if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Sharded)
            await ((GlideClusterClient)allSubscriber).SUnsubscribeLazyAsync(PubSub.AllShardedChannels);
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Sharded)
            await ((GlideClusterClient)allSubscriber).SUnsubscribeAsync(PubSub.AllShardedChannels);

        await AssertNotSubscribedAsync(allSubscriber, messages, unsubscribeMode);

        // Test unsubscribe with no arguments
        // ----------------------------------

        // Build subscriber and verify its active.
        using var emptySubscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(allSubscriber, messages);

        // Unsubscribe with empty collection and verify subscriptions.
        if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Exact)
            await allSubscriber.UnsubscribeLazyAsync();
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Exact)
            await allSubscriber.UnsubscribeAsync();
        else if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Pattern)
            await allSubscriber.PUnsubscribeLazyAsync();
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Pattern)
            await allSubscriber.PUnsubscribeAsync();
        else if (unsubscribeMode == UnsubscribeMode.Lazy && channelMode == PubSubChannelMode.Sharded)
            await ((GlideClusterClient)allSubscriber).SUnsubscribeLazyAsync();
        else if (unsubscribeMode == UnsubscribeMode.Blocking && channelMode == PubSubChannelMode.Sharded)
            await ((GlideClusterClient)allSubscriber).SUnsubscribeAsync();

        await AssertNotSubscribedAsync(allSubscriber, messages, unsubscribeMode);
    }

    [Theory]
    [MemberData(nameof(ClusterAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_AllChannelModes_UnsubscribesSuccessfully(bool isCluster, UnsubscribeMode unsubscribeMode)
    {
        var isSharded = IsShardedSupported(isCluster);

        // Build one message for each channel mode.
        var channelMessage = BuildMessage(PubSubChannelMode.Exact);
        var patternMessage = BuildMessage(PubSubChannelMode.Pattern);
        var shardedChannelMessage = isSharded ? BuildMessage(PubSubChannelMode.Sharded) : null;

        var messages = new List<PubSubMessage> { channelMessage, patternMessage };
        if (isSharded) messages.Add(shardedChannelMessage!);

        // Build subscriber and verify its active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from each channel/pattern and verify subscriptions.
        await UnsubscribeAsync(subscriber, messages, unsubscribeMode);
        await AssertNotSubscribedAsync(subscriber, messages, unsubscribeMode);
    }

    [Theory]
    [MemberData(nameof(ClusterAndUnsubscribeModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_AllChannelModes_UnsubscribesSuccessfully(bool isCluster, UnsubscribeMode unsubscribeMode)
    {
        var isSharded = IsShardedSupported(isCluster);

        // Build many messages for each channel mode.
        var messagesPerChannelMode = 128;
        var channelMessages = Enumerable.Range(0, messagesPerChannelMode).Select(_ => BuildMessage(PubSubChannelMode.Exact)).ToArray();
        var patternMessages = Enumerable.Range(0, messagesPerChannelMode).Select(_ => BuildMessage(PubSubChannelMode.Pattern)).ToArray();
        var shardedChannelMessages = isSharded ? Enumerable.Range(0, messagesPerChannelMode).Select(_ => BuildMessage(PubSubChannelMode.Sharded)).ToArray() : null;

        var messages = new List<PubSubMessage>();
        messages.AddRange(channelMessages);
        messages.AddRange(patternMessages);
        if (isSharded) messages.AddRange(shardedChannelMessages!);

        // Build subscriber and verify its active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from each channel/pattern and verify subscriptions.
        await UnsubscribeAsync(subscriber, messages, unsubscribeMode);
        await AssertNotSubscribedAsync(subscriber, messages, unsubscribeMode);
    }

    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_AllUnsubscribeModes_UnsubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        // Build one message for each unsubscribe mode.
        var lazyMessage = BuildMessage(channelMode);
        var blockingMessage = BuildMessage(channelMode);
        var messages = new List<PubSubMessage> { lazyMessage, blockingMessage };

        // Build subscriber and verify its active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from each channel/pattern using the corresponding unsubscribe mode and verify subscriptions.
        await UnsubscribeAsync(subscriber, lazyMessage, UnsubscribeMode.Lazy);
        await AssertNotSubscribedAsync(subscriber, lazyMessage, UnsubscribeMode.Lazy);

        await UnsubscribeAsync(subscriber, blockingMessage, UnsubscribeMode.Blocking);
        await AssertNotSubscribedAsync(subscriber, blockingMessage, UnsubscribeMode.Blocking);
    }

    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_AllUnsubscribeModes_UnsubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        // Build many messages for each unsubscribe mode.
        var messageCount = 128;
        var lazyMessages = Enumerable.Range(0, messageCount).Select(i => BuildMessage(channelMode)).ToArray();
        var blockingMessages = Enumerable.Range(0, messageCount).Select(i => BuildMessage(channelMode)).ToArray();

        var messages = new List<PubSubMessage>();
        messages.AddRange(lazyMessages);
        messages.AddRange(blockingMessages);

        // Build subscriber and verify its active.
        using var subscriber = await BuildSubscriber(isCluster, messages);
        await AssertSubscribedAsync(subscriber, messages);

        // Unsubscribe from all channel/pattern using the corresponding unsubscribe mode and verify subscriptions.
        await UnsubscribeAsync(subscriber, lazyMessages, UnsubscribeMode.Lazy);
        await AssertNotSubscribedAsync(subscriber, lazyMessages, UnsubscribeMode.Lazy);

        await UnsubscribeAsync(subscriber, blockingMessages, UnsubscribeMode.Blocking);
        await AssertNotSubscribedAsync(subscriber, blockingMessages, UnsubscribeMode.Blocking);
    }
}
