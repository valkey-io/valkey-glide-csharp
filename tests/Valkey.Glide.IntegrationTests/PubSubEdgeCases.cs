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
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task LargeMessage_DeliversSuccessfully(bool isCluster, PubSubChannelMode channelMode)
    {
        // Build large message.
        var channel = BuildChannel();
        var largeMessage = GenerateLargeMessage();
        var message = channelMode switch
        {
            PubSubChannelMode.Exact => PubSubMessage.FromChannel(largeMessage, channel),
            PubSubChannelMode.Pattern => PubSubMessage.FromPattern(largeMessage, channel, channel),
            PubSubChannelMode.Sharded => PubSubMessage.FromShardedChannel(largeMessage, channel),
            _ => throw new ArgumentOutOfRangeException(nameof(channelMode))
        };

        using var subscriber = await BuildSubscriber(isCluster, message);
        using var publisher = BuildPublisher(isCluster);

        // Publish large message and verify receipt.
        await PublishAsync(publisher, message);
        await AssertReceivedAsync(subscriber, message);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task UnicodeAndSpecialCharacters_DeliversCorrectly(bool isCluster)
    {
        // Build messages with various Unicode and special characters.
        string channel = BuildChannel();
        var messages = new[] {
            PubSubMessage.FromChannel("Simple ASCII", channel),
            PubSubMessage.FromChannel("Unicode: 你好世界 🌍", channel),
            PubSubMessage.FromChannel("Special chars: !@#$%^&*()", channel),
            PubSubMessage.FromChannel("Emoji: 🎉🚀💻", channel),
            PubSubMessage.FromChannel("Mixed: Hello世界!🌟", channel)
        };

        using var subscriber = await BuildSubscriber(isCluster, messages);
        using var publisher = BuildPublisher(isCluster);

        // Publish messages and verify receipt.
        await PublishAsync(publisher, messages);
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
