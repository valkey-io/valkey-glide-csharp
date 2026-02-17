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
    [Theory]
    [MemberData(nameof(SubscriptionAndChannelModeOptions), MemberType = typeof(PubSubUtils))]
    public static async Task SingleSubscription_SubscribesSuccessfully(bool isCluster, SubscribeMode subscriptionMode, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);
        var messages = new[] { message };

        using var subscriber = await BuildSubscriber(isCluster, channelMode, subscriptionMode, messages);
        await AssertSubscribedAsync(subscriber, channelMode, messages);
    }

    [Theory]
    [MemberData(nameof(SubscriptionAndChannelModeOptions), MemberType = typeof(PubSubUtils))]
    public static async Task MultipleSubscriptions_SubscribesSuccessfully(bool isCluster, SubscribeMode subscriptionMode, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        using var subscriber = await BuildSubscriber(isCluster, channelMode, subscriptionMode, messages);
        await AssertSubscribedAsync(subscriber, channelMode, messages);
    }

    [Theory]
    [MemberData(nameof(SubscribeModeOptions), MemberType = typeof(PubSubUtils))]
    public static async Task AllChannelModes_SubscribesSuccessfully(bool isCluster, SubscribeMode subscriptionMode)
    {
        var isSharded = IsShardedSupported(isCluster);

        var channelMessage = BuildChannelMessage();
        var patternMessage = BuildPatternMessage();
        var shardChannelMessage = isSharded ? BuildShardChannelMessage() : null;

        using var subscriber = await BuildSubscriber(isCluster, subscriptionMode,
            channels: [channelMessage.Channel],
            patterns: [patternMessage.Pattern!],
            shardChannels: isSharded ? [shardChannelMessage!.Channel] : null);

        await AssertSubscribedAsync(subscriber, [channelMessage.Channel]);
        await AssertPSubscribedAsync(subscriber, [patternMessage.Pattern!]);
        if (isSharded) await AssertSSubscribedAsync((GlideClusterClient)subscriber, [shardChannelMessage!.Channel]);
    }

    [Theory]
    [MemberData(nameof(ChannelModeOptions), MemberType = typeof(PubSubUtils))]
    public static async Task AllSubscribeModes_SubscribesSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var configMessage = BuildMessage(channelMode);
        var lazyMessage = BuildMessage(channelMode);
        var blockingMessage = BuildMessage(channelMode);

        using var subscriber = await BuildSubscriber(isCluster, channelMode, SubscribeMode.Config, [configMessage]);
        await AssertSubscribedAsync(subscriber, channelMode, [configMessage]);

        await SubscribeAsync(subscriber, blockingMessage);
        await AssertSubscribedAsync(subscriber, channelMode, [blockingMessage]);

        await SubscribeLazyAsync(subscriber, lazyMessage);
        await AssertSubscribedAsync(subscriber, channelMode, [lazyMessage]);
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
