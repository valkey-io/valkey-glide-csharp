// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub unsubscribe operations.
/// </summary>
[Collection(typeof(PubSubUnsubscribeTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubUnsubscribeTests
{
    [Theory]
    [MemberData(nameof(UnsubscribeData), MemberType = typeof(PubSubUtils))]
    public static async Task Unsubscribe_Single_RemovesOne(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        using var subscriber = await BuildSubscriber(isCluster, channelMode, SubscribeMode.Config, messages);
        await UnsubscribeAsync(subscriber, channelMode, unsubscribeMode, [message1]);

        await AssertNotSubscribedAsync(subscriber, channelMode, [message1]);
        await AssertSubscribedAsync(subscriber, channelMode, [message2]);
    }

    [Theory]
    [MemberData(nameof(UnsubscribeData), MemberType = typeof(PubSubUtils))]
    public static async Task Unsubscribe_Multiple_RemovesMultiple(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var message3 = BuildMessage(channelMode);
        var messages = new[] { message1, message2, message3 };

        using var subscriber = await BuildSubscriber(isCluster, channelMode, SubscribeMode.Config, messages);
        await UnsubscribeAsync(subscriber, channelMode, unsubscribeMode, [message1, message2]);

        await AssertNotSubscribedAsync(subscriber, channelMode, [message1, message2]);
        await AssertSubscribedAsync(subscriber, channelMode, [message3]);
    }

    [Theory]
    [MemberData(nameof(UnsubscribeData), MemberType = typeof(PubSubUtils))]
    public static async Task Unsubscribe_All_RemovesAll(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        using var subscriber = await BuildSubscriber(isCluster, channelMode, SubscribeMode.Config, messages);
        await UnsubscribeAllAsync(subscriber, channelMode, unsubscribeMode);

        await AssertNotSubscribedAsync(subscriber, channelMode, channelMode == PubSubChannelMode.Pattern ? PubSub.AllPatterns : channelMode == PubSubChannelMode.Sharded ? PubSub.AllShardChannels : PubSub.AllChannels);
    }

    [Theory]
    [MemberData(nameof(UnsubscribeData), MemberType = typeof(PubSubUtils))]
    public static async Task Unsubscribe_Empty_RemovesAll(bool isCluster, PubSubChannelMode channelMode, UnsubscribeMode unsubscribeMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message1 = BuildMessage(channelMode);
        var message2 = BuildMessage(channelMode);
        var messages = new[] { message1, message2 };

        using var subscriber = await BuildSubscriber(isCluster, channelMode, SubscribeMode.Config, messages);
        await UnsubscribeEmptyAsync(subscriber, channelMode, unsubscribeMode);

        await AssertNotSubscribedAsync(subscriber, channelMode, channelMode == PubSubChannelMode.Pattern ? PubSub.AllPatterns : channelMode == PubSubChannelMode.Sharded ? PubSub.AllShardChannels : PubSub.AllChannels);
    }
}
