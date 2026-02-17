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
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task UnsubscribeLazy_Channel_RemovesOne(bool isCluster)
    {
        var message1 = BuildChannelMessage();
        var message2 = BuildChannelMessage();

        using var subscriber = await BuildSubscriber(isCluster, channels: [message1.Channel, message2.Channel]);
        await subscriber.UnsubscribeLazyAsync(message1.Channel);

        await AssertNotSubscribedAsync(subscriber, [message1.Channel]);
        await AssertSubscribedAsync(subscriber, [message2.Channel]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task UnsubscribeLazy_Channels_RemovesMultiple(bool isCluster)
    {
        var message1 = BuildChannelMessage();
        var message2 = BuildChannelMessage();
        var message3 = BuildChannelMessage();

        var channels = new List<string> { message1.Channel, message2.Channel, message3.Channel };
        using var subscriber = await BuildSubscriber(isCluster, channels: channels);

        await subscriber.UnsubscribeLazyAsync(channels[0..2]);

        await AssertNotSubscribedAsync(subscriber, channels[0..2]);
        await AssertSubscribedAsync(subscriber, channels[2..3]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task UnsubscribeLazy_AllChannels_RemovesAll(bool isCluster)
    {
        var message1 = BuildChannelMessage();
        var message2 = BuildChannelMessage();

        using var subscriber = await BuildSubscriber(isCluster, channels: [message1.Channel, message2.Channel]);
        await subscriber.UnsubscribeLazyAsync(PubSub.AllChannels);

        await AssertNotSubscribedAsync(subscriber, PubSub.AllChannels);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task UnsubscribeLazy_NoChannels_RemovesAll(bool isCluster)
    {
        var message1 = BuildChannelMessage();
        var message2 = BuildChannelMessage();

        using var subscriber = await BuildSubscriber(isCluster, channels: [message1.Channel, message2.Channel]);
        await subscriber.UnsubscribeLazyAsync();

        await AssertNotSubscribedAsync(subscriber, PubSub.AllChannels);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PUnsubscribeLazy_Pattern_RemovesOne(bool isCluster)
    {
        var message1 = BuildPatternMessage();
        var message2 = BuildPatternMessage();

        using var subscriber = await BuildSubscriber(isCluster, patterns: [message1.Pattern!, message2.Pattern!]);
        await subscriber.PUnsubscribeLazyAsync(message1.Pattern!);

        await AssertNotPSubscribedAsync(subscriber, [message1.Pattern!]);
        await AssertPSubscribedAsync(subscriber, [message2.Pattern!]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PUnsubscribeLazy_Patterns_RemovesMultiple(bool isCluster)
    {
        var message1 = BuildPatternMessage();
        var message2 = BuildPatternMessage();
        var message3 = BuildPatternMessage();

        var patterns = new string[] { message1.Pattern!, message2.Pattern!, message3.Pattern! };
        using var subscriber = await BuildSubscriber(isCluster, patterns: patterns);

        await subscriber.PUnsubscribeLazyAsync(patterns[0..2]);

        await AssertNotPSubscribedAsync(subscriber, patterns[0..2]);
        await AssertPSubscribedAsync(subscriber, patterns[2..3]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PUnsubscribeLazy_AllPatterns_RemovesAll(bool isCluster)
    {
        var message1 = BuildPatternMessage();
        var message2 = BuildPatternMessage();

        using var subscriber = await BuildSubscriber(isCluster, patterns: [message1.Pattern!, message2.Pattern!]);

        await subscriber.PUnsubscribeLazyAsync(PubSub.AllPatterns);
        await AssertNotPSubscribedAsync(subscriber, PubSub.AllPatterns);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task PUnsubscribeLazy_NoPatterns_RemovesAll(bool isCluster)
    {
        var message1 = BuildPatternMessage();
        var message2 = BuildPatternMessage();

        using var subscriber = await BuildSubscriber(isCluster, patterns: [message1.Pattern!, message2.Pattern!]);

        await subscriber.PUnsubscribeLazyAsync();
        await AssertNotPSubscribedAsync(subscriber, PubSub.AllPatterns);
    }

    [Fact]
    public async Task SUnsubscribeLazy_ShardChannel_RemovesOne()
    {
        SkipUnlessShardedSupported();

        var message1 = BuildShardChannelMessage();
        var message2 = BuildShardChannelMessage();

        using var subscriber = await BuildClusterSubscriber(shardChannels: [message1.Channel, message2.Channel]);
        await subscriber.SUnsubscribeLazyAsync(message1.Channel);

        await AssertNotSSubscribedAsync(subscriber, [message1.Channel]);
        await AssertSSubscribedAsync(subscriber, [message2.Channel]);
    }

    [Fact]
    public async Task SUnsubscribeLazy_ShardChannels_RemovesMultiple()
    {
        SkipUnlessShardedSupported();

        var message1 = BuildShardChannelMessage();
        var message2 = BuildShardChannelMessage();
        var message3 = BuildShardChannelMessage();

        var shardChannels = new string[] { message1.Channel, message2.Channel, message3.Channel };
        using var subscriber = await BuildClusterSubscriber(shardChannels: shardChannels);

        await subscriber.SUnsubscribeLazyAsync(shardChannels[0..2]);

        await AssertNotSSubscribedAsync(subscriber, shardChannels[0..2]);
        await AssertSSubscribedAsync(subscriber, shardChannels[2..3]);
    }

    [Fact]
    public async Task SUnsubscribeLazy_AllShardChannels_RemovesAll()
    {
        SkipUnlessShardedSupported();

        var message1 = BuildShardChannelMessage();
        var message2 = BuildShardChannelMessage();

        using var subscriber = await BuildClusterSubscriber(shardChannels: [message1.Channel, message2.Channel]);

        await subscriber.SUnsubscribeLazyAsync(PubSub.AllShardChannels);
        await AssertNotSSubscribedAsync(subscriber, PubSub.AllShardChannels);
    }

    [Fact]
    public async Task SUnsubscribeLazy_NoShardChannels_RemovesAll()
    {
        SkipUnlessShardedSupported();

        var message1 = BuildShardChannelMessage();
        var message2 = BuildShardChannelMessage();

        using var subscriber = await BuildClusterSubscriber(shardChannels: [message1.Channel, message2.Channel]);
        await subscriber.SUnsubscribeLazyAsync();

        await AssertNotSSubscribedAsync(subscriber, PubSub.AllShardChannels);
    }
}
