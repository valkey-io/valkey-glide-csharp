// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub subscription methods (config-based and lazy).
/// </summary>
[Collection(typeof(PubSubSubscribeTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubSubscribeTests
{
    /// <summary>
    /// Theory data for all valid combinations of cluster mode, subscribe mode, and channel mode.
    /// </summary>
    public static TheoryData<bool, SubscribeMode, PubSubChannelMode> SubscribeTestData
    {
        get
        {
            var data = new TheoryData<bool, SubscribeMode, PubSubChannelMode>();
            foreach (var isCluster in ClusterModeData)
            {
                foreach (var subscriptionMode in SubscribeModeData)
                {
                    foreach (var channelMode in ChannelModeData)
                    {
                        if (IsChannelModeSupported(isCluster, channelMode))
                            data.Add(isCluster, subscriptionMode, channelMode);
                    }
                }
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(SubscribeTestData))]
    public static async Task SingleSubscription_SubscribesSuccessfully(bool isCluster, SubscribeMode subscriptionMode, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);
        var messages = new[] { message };

        using var subscriber = await BuildSubscriber(isCluster, messages, subscriptionMode);
        await AssertSubscribedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(SubscribeTestData))]
    public static async Task MultipleSubscriptions_SubscribesSuccessfully(bool isCluster, SubscribeMode subscriptionMode, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        using var subscriber = await BuildSubscriber(isCluster, messages, subscriptionMode);
        await AssertSubscribedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task AllChannelModes_SubscribesSuccessfully(bool isCluster)
    {
        var isSharded = IsShardedSupported(isCluster);

        var channelMessage = BuildMessage(PubSubChannelMode.Exact);
        var patternMessage = BuildMessage(PubSubChannelMode.Pattern);
        var shardedChannelMessage = isSharded ? BuildMessage(PubSubChannelMode.Sharded) : null;

        var expectedMessages = new List<PubSubMessage> { channelMessage, patternMessage };
        if (isSharded) expectedMessages.Add(shardedChannelMessage!);

        using var subscriber = await BuildSubscriber(isCluster, expectedMessages);
        await AssertSubscribedAsync(subscriber, [.. expectedMessages]);
    }

    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task AllSubscribeModes_SubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var configMessage = BuildMessage(channelMode);
        var lazyMessage = BuildMessage(channelMode);
        var blockingMessage = BuildMessage(channelMode);

        using var subscriber = await BuildSubscriber(isCluster, [configMessage], SubscribeMode.Config);
        await AssertSubscribedAsync(subscriber, [configMessage]);

        await SubscribeAsync(subscriber, blockingMessage);
        await AssertSubscribedAsync(subscriber, [blockingMessage]);

        await SubscribeLazyAsync(subscriber, lazyMessage);
        await AssertSubscribedAsync(subscriber, [lazyMessage]);
    }

    /// <summary>
    /// Subscribes the client to receive the given message and waits for server confirmation.
    /// </summary>
    private static Task SubscribeAsync(BaseClient subscriber, PubSubMessage message)
    {
        return message.ChannelMode switch
        {
            PubSubChannelMode.Exact => subscriber.SubscribeAsync(message.Channel),
            PubSubChannelMode.Pattern => subscriber.PSubscribeAsync(message.Pattern!),
            PubSubChannelMode.Sharded => ((GlideClusterClient)subscriber).SSubscribeAsync(message.Channel),
            _ => throw new ArgumentException($"Unsupported channel mode: {message.ChannelMode}")
        };
    }

    /// <summary>
    /// Subscribes the client to receive the given message and return without waiting for server confirmation.
    /// </summary>
    private static Task SubscribeLazyAsync(BaseClient subscriber, PubSubMessage message)
    {
        return message.ChannelMode switch
        {
            PubSubChannelMode.Exact => subscriber.SubscribeLazyAsync(message.Channel),
            PubSubChannelMode.Pattern => subscriber.PSubscribeLazyAsync(message.Pattern!),
            PubSubChannelMode.Sharded => ((GlideClusterClient)subscriber).SSubscribeLazyAsync(message.Channel),
            _ => throw new ArgumentException($"Unsupported channel mode: {message.ChannelMode}")
        };
    }
}
