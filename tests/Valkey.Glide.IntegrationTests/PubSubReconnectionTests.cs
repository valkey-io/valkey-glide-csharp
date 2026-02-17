// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub reconnection.
/// </summary>
[Collection(typeof(PubSubReconnectionTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubReconnectionTests
{
    private static readonly GlideString[] ClientKillArgs = ["CLIENT", "KILL", "TYPE", "NORMAL", "SKIPME", "yes"];

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ResubscribeAfterConnectionKill_Channel_ResubscribesAutomatically(bool isCluster)
    {
        var channel = BuildChannel();
        var channels = new[] { channel };

        using var subscriber = await BuildSubscriber(
            isCluster,
            channels: channels);
        using var publisher = BuildClient(isCluster);

        // Verify subscription before kill.
        await AssertSubscribedAsync(subscriber, channels);

        var beforeMessage = PubSubMessage.FromChannel("before_kill", channel);
        await publisher.PublishAsync(channel, beforeMessage.Message);
        await AssertMessagesReceivedAsync(subscriber, [beforeMessage]);

        // Kill connections and wait for reconnection.
        await KillConnections(publisher);
        await Task.Delay(TimeSpan.FromSeconds(2));

        // Verify subscription after kill.
        await AssertSubscribedAsync(subscriber, channels);

        var afterMessage = PubSubMessage.FromChannel("after_kill", channel);
        await publisher.PublishAsync(channel, afterMessage.Message);
        await AssertMessagesReceivedAsync(subscriber, [afterMessage]);
    }

    [Theory]
    [MemberData(nameof(IsCluster), MemberType = typeof(PubSubUtils))]
    public async Task ResubscribeAfterConnectionKill_Pattern_ResubscribesAutomatically(bool isCluster)
    {
        var (channel, pattern) = BuildChannelAndPattern();
        var patterns = new[] { pattern };

        using var subscriber = await BuildSubscriber(
            isCluster,
            patterns: patterns);
        using var publisher = BuildClient(isCluster);

        // Verify subscription before kill.
        await AssertPSubscribedAsync(subscriber, patterns);

        var beforeMessage = PubSubMessage.FromPattern("before_kill", channel, pattern);
        await publisher.PublishAsync(channel, beforeMessage.Message);
        await AssertMessagesReceivedAsync(subscriber, [beforeMessage]);

        // Kill connections and wait for reconnection.
        await KillConnections(publisher);
        await Task.Delay(TimeSpan.FromSeconds(2));

        // Verify subscription after kill.
        await AssertPSubscribedAsync(subscriber, patterns);

        var afterMessage = PubSubMessage.FromPattern("after_kill", channel, pattern);
        await publisher.PublishAsync(channel, afterMessage.Message);
        await AssertMessagesReceivedAsync(subscriber, [afterMessage]);
    }

    [Fact]
    public async Task ResubscribeAfterConnectionKill_ShardChannel_ResubscribesAutomatically()
    {
        SkipUnlessShardedSupported();

        var shardChannel = BuildChannel();
        var shardChannels = new[] { shardChannel };

        using var subscriber = await BuildClusterSubscriber(
            shardChannels: shardChannels);
        using var publisher = BuildClusterClient();

        // Verify subscription before kill.
        await AssertSSubscribedAsync(subscriber, shardChannels);

        var beforeMessage = PubSubMessage.FromShardChannel("before_kill", shardChannel);
        await publisher.SPublishAsync(shardChannel, beforeMessage.Message);
        await AssertMessagesReceivedAsync(subscriber, [beforeMessage]);

        // Kill connections and wait for reconnection.
        await KillConnections(publisher);
        await Task.Delay(TimeSpan.FromSeconds(2));

        // Verify subscription after kill.
        await AssertSSubscribedAsync(subscriber, shardChannels);

        var afterMessage = PubSubMessage.FromShardChannel("after_kill", shardChannel);
        await publisher.SPublishAsync(shardChannel, afterMessage.Message);
        await AssertMessagesReceivedAsync(subscriber, [afterMessage]);
    }

    /// <summary>
    /// Kills all normal client connections to the server used by the given client.
    /// </summary>
    private static async Task KillConnections(BaseClient client)
    {
        try
        {
            if (client is GlideClusterClient clusterClient)
                await clusterClient.CustomCommand(ClientKillArgs);
            else if (client is GlideClient standaloneClient)
                await standaloneClient.CustomCommand(ClientKillArgs);
        }
        catch
        {
            // Expected - connection will be killed
        }
    }
}
