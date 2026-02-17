// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub subscription methods (config-based and lazy).
/// </summary>
[Collection(typeof(PubSubSubscriptionMethodsTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubSubscriptionMethodsTests
{
    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ConfigSubscription_Channel_SubscribesImmediately(bool isCluster)
    {
        var message = BuildChannelMessage();
        var channels = new[] { message.Channel };

        using var subscriber = await BuildSubscriber(isCluster, channels: channels);
        await AssertSubscribedAsync(subscriber, channels);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ConfigSubscription_Pattern_SubscribesImmediately(bool isCluster)
    {
        var message = BuildPatternMessage();
        var patterns = new[] { message.Pattern! };

        using var subscriber = await BuildSubscriber(isCluster, patterns: patterns);
        await AssertPSubscribedAsync(subscriber, patterns);
    }

    [Fact]
    public async Task ConfigSubscription_ShardChannel_SubscribesImmediately()
    {
        SkipUnlessShardedSupported();

        var message = BuildShardChannelMessage();
        var shardChannels = new[] { message.Channel };

        using var subscriber = await BuildClusterSubscriber(shardChannels: shardChannels);
        await AssertSSubscribedAsync(subscriber, shardChannels);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ConfigSubscription_AllChannelModes_SubscribesImmediately(bool isCluster)
    {
        var isSharded = IsShardedSupported(isCluster);

        var channelMessage = BuildChannelMessage();
        var patternMessage = BuildPatternMessage();
        var shardMessage = BuildShardChannelMessage();

        var channels = new[] { channelMessage.Channel };
        var patterns = new[] { patternMessage.Pattern! };
        var shardChannels = isSharded ? new[] { shardMessage.Channel } : [];

        using var subscriber = await BuildSubscriber(
            isCluster,
            channels: channels,
            patterns: patterns,
            shardChannels: shardChannels);

        await AssertSubscribedAsync(subscriber, channels);
        await AssertPSubscribedAsync(subscriber, patterns);
        if (isSharded) await AssertSSubscribedAsync((GlideClusterClient)subscriber, shardChannels);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task LazySubscription_Channel_SubscribesAsynchronously(bool isCluster)
    {
        var message = BuildChannelMessage();
        var channels = new string[] { message.Channel };

        using var subscriber = await BuildSubscriber(isCluster);
        await subscriber.SubscribeLazyAsync(channels);

        await AssertSubscribedAsync(subscriber, channels);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task LazySubscription_Channels_SubscribesAsynchronously(bool isCluster)
    {
        var message1 = BuildChannelMessage();
        var message2 = BuildChannelMessage();
        var channels = new string[] { message1.Channel, message2.Channel };

        using var subscriber = await BuildSubscriber(isCluster);
        await subscriber.SubscribeLazyAsync(channels);

        await AssertSubscribedAsync(subscriber, channels);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task LazySubscription_Pattern_SubscribesAsynchronously(bool isCluster)
    {
        var message = BuildPatternMessage();
        var patterns = new string[] { message.Pattern! };

        using var subscriber = await BuildSubscriber(isCluster);
        await subscriber.PSubscribeLazyAsync(patterns);

        await AssertPSubscribedAsync(subscriber, patterns);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task LazySubscription_Patterns_SubscribesAsynchronously(bool isCluster)
    {
        var message1 = BuildPatternMessage();
        var message2 = BuildPatternMessage();
        var patterns = new string[] { message1.Pattern!, message2.Pattern! };

        using var subscriber = await BuildSubscriber(isCluster);
        await subscriber.PSubscribeLazyAsync(patterns);

        await AssertPSubscribedAsync(subscriber, patterns);
    }

    [Fact]
    public async Task LazySubscription_ShardChannel_SubscribesAsynchronously()
    {
        SkipUnlessShardedSupported();

        var message = BuildShardChannelMessage();
        var shardChannels = new string[] { message.Channel };

        using var subscriber = await BuildClusterSubscriber();
        await subscriber.SSubscribeLazyAsync(shardChannels);

        await AssertSSubscribedAsync(subscriber, shardChannels);
    }

    [Fact]
    public async Task LazySubscription_ShardChannels_SubscribesAsynchronously()
    {
        SkipUnlessShardedSupported();

        var message1 = BuildShardChannelMessage();
        var message2 = BuildShardChannelMessage();
        var shardChannels = new string[] { message1.Channel, message2.Channel };

        using var subscriber = await BuildClusterSubscriber();
        await subscriber.SSubscribeLazyAsync(shardChannels);

        await AssertSSubscribedAsync(subscriber, shardChannels);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task LazySubscription_AllChannelModes_SubscribesAsynchronously(bool isCluster)
    {
        var isSharded = IsShardedSupported(isCluster);

        var channelMessage = BuildChannelMessage();
        var patternMessage = BuildPatternMessage();
        var shardMessage = BuildShardChannelMessage();

        using var subscriber = await BuildSubscriber(isCluster);

        await subscriber.SubscribeLazyAsync(channelMessage.Channel);
        await subscriber.PSubscribeLazyAsync(patternMessage.Pattern!);
        if (isSharded) await ((GlideClusterClient)subscriber).SSubscribeLazyAsync(shardMessage.Channel);

        await AssertSubscribedAsync(subscriber, [channelMessage.Channel]);
        await AssertPSubscribedAsync(subscriber, [patternMessage.Pattern!]);
        if (isSharded) await AssertSSubscribedAsync((GlideClusterClient)subscriber, [shardMessage.Channel]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task MixedSubscription_AllTypes_AllWork(bool isCluster)
    {
        var configMessage = BuildChannelMessage();
        var lazyMessage = BuildChannelMessage();

        using var subscriber = await BuildSubscriber(isCluster, channels: [configMessage.Channel]);

        await AssertSubscribedAsync(subscriber, [configMessage.Channel]);
        await AssertNotSubscribedAsync(subscriber, [lazyMessage.Channel]);

        await subscriber.SubscribeLazyAsync(lazyMessage.Channel);

        await AssertSubscribedAsync(subscriber, [configMessage.Channel, lazyMessage.Channel]);
    }
}
