// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub edge cases and limits.
/// </summary>
[Collection(typeof(PubSubEdgeCaseTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubEdgeCaseTests
{
    [Theory]
    [MemberData(nameof(PubSubUtils.IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task LargeMessage_Channel_DeliversSuccessfully(bool isCluster)
    {
        var channel = BuildChannel();
        var largeMessage = GenerateLargeMessage();

        using var subscriber = await BuildSubscriber(isCluster, channels: [channel]);
        using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(channel, largeMessage);

        var expected = PubSubMessage.FromChannel(largeMessage, channel);
        await AssertReceivedAsync(subscriber, [expected]);
    }

    [Theory]
    [MemberData(nameof(PubSubUtils.IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task LargeMessage_Pattern_DeliversSuccessfully(bool isCluster)
    {
        var (channel, pattern) = BuildChannelAndPattern();
        var largeMessage = GenerateLargeMessage();

        await using var subscriber = await BuildSubscriber(isCluster, patterns: [pattern]);
        await using var publisher = BuildClient(isCluster);

        await publisher.PublishAsync(channel, largeMessage);

        var expected = PubSubMessage.FromPattern(largeMessage, channel, pattern);
        await AssertReceivedAsync(subscriber, [expected]);
    }

    [Fact]
    public async Task LargeMessage_ShardChannel_DeliversSuccessfully()
    {
        SkipUnlessShardedSupported();

        var channel = BuildChannel();
        var largeMessage = GenerateLargeMessage();

        using var subscriber = await BuildClusterSubscriber(shardChannels: [channel]);
        using var publisher = BuildClusterClient();

        await publisher.SPublishAsync(channel, largeMessage);

        var expected = PubSubMessage.FromShardChannel(largeMessage, channel);
        await AssertReceivedAsync(subscriber, [expected]);
    }

    [Theory]
    [MemberData(nameof(PubSubUtils.IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task UnicodeAndSpecialCharacters_DeliversCorrectly(bool isCluster)
    {
        string channel = BuildChannel();
        var messages = new[] {
            PubSubMessage.FromChannel("Simple ASCII", channel),
            PubSubMessage.FromChannel("Unicode: 你好世界 🌍", channel),
            PubSubMessage.FromChannel("Special chars: !@#$%^&*()", channel),
            PubSubMessage.FromChannel("Emoji: 🎉🚀💻", channel),
            PubSubMessage.FromChannel("Mixed: Hello世界!🌟", channel)
        };

        using var subscriber = await BuildSubscriber(isCluster, channels: [channel]);
        using var publisher = BuildClient(isCluster);

        foreach (var message in messages)
            await publisher.PublishAsync(channel, message.Message);

        await AssertReceivedAsync(subscriber, messages);
    }

    /// <summary>
    /// Generates a large message (1MB).
    /// </summary>
    private static string GenerateLargeMessage()
    {
        const int size = 1024 * 1024; // 1MB
        return new string('A', size);
    }
}
