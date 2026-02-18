// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for multiple pub/sub subscription types and message retrieval methods.
/// </summary>
[Collection(typeof(PubSubCoexistenceTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubCoexistenceTests
{
    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ManySubscriptions_Coexistence_NoInterference(bool isCluster, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        // Build messages.
        var messages = Enumerable.Range(0, 5)
            .Select(_ => BuildMessage(channelMode))
            .ToArray();

        using var subscriber = await BuildSubscriber(isCluster, messages);
        using var publisher = BuildPublisher(isCluster);

        // Publish messages and verify receipt.
        await PublishAsync(publisher, messages);
        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(ClusterModeData), MemberType = typeof(PubSubUtils))]
    public static async Task CustomPublishCommand_WithPubSub_WorksCorrectly(bool isCluster)
    {
        var message = BuildMessage();

        using var subscriber = await BuildSubscriber(isCluster, message);
        using var publisher = BuildPublisher(isCluster);

        // Publish to channel with custom command and verify receipt.
        var args = new GlideString[] { "PUBLISH", message.Channel, message.Message };

        if (isCluster)
            await ((GlideClusterClient)publisher).CustomCommand(args);
        else
            await ((GlideClient)publisher).CustomCommand(args);

        await AssertReceivedAsync(subscriber, [message]);
    }

    [Fact]
    public static async Task CustomSPublishCommand_WithPubSub_WorksCorrectly()
    {
        SkipUnlessShardedSupported();

        var message = BuildMessage(PubSubChannelMode.Sharded);

        using var subscriber = await BuildClusterSubscriber(message);
        using var publisher = BuildPublisher(isCluster: true);

        // Publish to sharded channel with custom command and verify receipt.
        await ((GlideClusterClient)publisher).CustomCommand(["SPUBLISH", message.Channel, message.Message]);
        await AssertReceivedAsync(subscriber, message);
    }
}
