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
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task ResubscribeAfterConnectionKill_Channel_ResubscribesAutomatically(bool isCluster, PubSubChannelMode channelMode)
    {
        SkipUnlessChannelModeSupported(isCluster, channelMode);

        var message = BuildMessage(channelMode);

        using var subscriber = await BuildSubscriber(isCluster, message);
        using var publisher = BuildPublisher(isCluster);

        // Kill connections and wait for reconnection.
        await KillConnections(publisher);
        await Task.Delay(TimeSpan.FromSeconds(2));

        // Verify subscription after kill.
        await AssertSubscribedAsync(subscriber, message);

        // Publish message after kill and verify receipt.
        await PublishAsync(publisher, message);
        await AssertReceivedAsync(subscriber, message);
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
