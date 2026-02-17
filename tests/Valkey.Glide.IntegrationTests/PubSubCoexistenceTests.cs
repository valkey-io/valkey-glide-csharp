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
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ManyChannels_Coexistence_NoInterference(bool isCluster)
    {
        var messages = Enumerable.Range(0, 5)
            .Select(_ => BuildChannelMessage())
            .ToArray();

        using var subscriber = await BuildSubscriber(
            isCluster,
            channels: [.. messages.Select(m => m.Channel)]);
        using var publisher = BuildClient(isCluster);

        foreach (var message in messages)
            await publisher.PublishAsync(message.Channel, message.Message);

        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ManyPatterns_Coexistence_NoInterference(bool isCluster)
    {
        var messages = Enumerable.Range(0, 5)
            .Select(_ => BuildPatternMessage())
            .ToArray();

        using var subscriber = await BuildSubscriber(
            isCluster,
            patterns: [.. messages.Select(m => m.Pattern!)]);
        using var publisher = BuildClient(isCluster);

        foreach (var message in messages)
            await publisher.PublishAsync(message.Channel, message.Message);

        await AssertReceivedAsync(subscriber, messages);
    }

    [Fact]
    public async Task ManyShardChannels_Coexistence_NoInterference()
    {
        SkipUnlessShardedSupported();

        var messages = Enumerable.Range(0, 5)
            .Select(_ => BuildShardChannelMessage())
            .ToArray();

        using var subscriber = await BuildClusterSubscriber(
            shardChannels: [.. messages.Select(m => m.Channel!)]);
        using var publisher = BuildClusterClient();

        foreach (var message in messages)
            await publisher.SPublishAsync(message.Channel, message.Message);

        await AssertReceivedAsync(subscriber, messages);
    }

    [Theory]
    [MemberData(nameof(PubSubUtils.IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task CustomPublishCommand_WithPubSub_WorksCorrectly(bool isCluster)
    {
        var message = BuildChannelMessage();

        using var subscriber = await BuildSubscriber(
            isCluster,
            channels: [message.Channel]);

        // Publish to channel with custom command.
        var args = new GlideString[] { "PUBLISH", message.Channel, message.Message };

        if (isCluster)
        {
            using var publisher = BuildClusterClient();
            await publisher.CustomCommand(args);
        }
        else
        {
            using var publisher = BuildStandaloneClient();
            await publisher.CustomCommand(args);
        }

        await AssertReceivedAsync(subscriber, [message]);
    }

    [Fact]
    public async Task CustomSPublishCommand_WithPubSub_WorksCorrectly()
    {
        SkipUnlessShardedSupported();

        var message = BuildShardChannelMessage();

        using var subscriber = await BuildClusterSubscriber(
            shardChannels: [message.Channel]);
        using var publisher = BuildClusterClient();

        // Publish to shard channel with custom command.
        var args = new GlideString[] { "SPUBLISH", message.Channel, message.Message };
        await publisher.CustomCommand(args);

        await AssertReceivedAsync(subscriber, [message]);
    }
}
