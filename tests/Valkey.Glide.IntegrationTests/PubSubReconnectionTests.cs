// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.IntegrationTests.PubSubUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Integration tests for pub/sub reconnection.
/// </summary>
[Collection(typeof(PubSubReconnectionTests))]
[CollectionDefinition(DisableParallelization = true)]
public class PubSubReconnectionTests
{
    [Theory]
    [MemberData(nameof(ClusterAndChannelModeData), MemberType = typeof(PubSubUtils))]
    public static async Task AfterConnectionKill_ResubscribesAutomatically(bool isCluster, PubSubChannelMode channelMode)
    {
        var message = BuildMessage(channelMode);

        await using var subscriber = await BuildSubscriber(isCluster, message);
        await using var publisher = BuildPublisher(isCluster);

        // Kill connections and wait for reconnection.
        _ = await publisher.ClientKillAsync(new ClientFilterOptions().WithSkipMe(true));
        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        // Verify subscription after kill.
        await AssertSubscribedAsync(subscriber, message);

        // Publish message after kill and verify receipt.
        await PublishAsync(publisher, message);
        await AssertReceivedAsync(subscriber, message);
    }
}
