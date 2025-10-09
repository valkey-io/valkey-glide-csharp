// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Pipeline;

using static Valkey.Glide.Pipeline.Options;

using TimeoutException = Valkey.Glide.Errors.TimeoutException;

namespace Valkey.Glide.IntegrationTests;

// We separated these tests with sleep commands into their own collection to prevent interference with other tests
// as C# runs them in parallel.
[Collection(typeof(DebugSleepTests))]
[CollectionDefinition(DisableParallelization = true)]
public class DebugSleepTests
{
    [Fact]
    public async Task ErrorIfTimedOut()
    {
        using GlideClient client = TestConfiguration.LowTimeoutStandaloneClient();
        _ = await Assert.ThrowsAsync<TimeoutException>(async () =>
            _ = await client.CustomCommand(["debug", "sleep", "0.5"])
        );
    }

#pragma warning disable xUnit1047 // Avoid using TheoryDataRow arguments that might not be serializable
    public static IEnumerable<TheoryDataRow<BaseClient, bool>> GetTestClientWithAtomic =>
        TestConfiguration.TestClients.SelectMany(r => new TheoryDataRow<BaseClient, bool>[] { new(r.Data, true), new(r.Data, false) });
#pragma warning restore xUnit1047 // Avoid using TheoryDataRow arguments that might not be serializable

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(GetTestClientWithAtomic))]
    public async Task BatchTimeout(BaseClient client, bool isAtomic)
    {
        bool isCluster = client is GlideClusterClient;
        Pipeline.IBatch batch = isCluster ? new ClusterBatch(isAtomic) : new Batch(isAtomic);
        _ = batch.CustomCommand(["DEBUG", "sleep", "0.5"]);
        BaseBatchOptions options = isCluster ? new ClusterBatchOptions(timeout: 100) : new BatchOptions(timeout: 100);

        // Expect a timeout exception on short timeout
        _ = await Assert.ThrowsAsync<TimeoutException>(() => isCluster
                ? ((GlideClusterClient)client).Exec((ClusterBatch)batch, true, (ClusterBatchOptions)options)
                : ((GlideClient)client).Exec((Batch)batch, true, (BatchOptions)options));

        // Wait for server to wake up
        Thread.Sleep(TimeSpan.FromSeconds(1));

        // Retry with a longer timeout and expect [OK]
        options = isCluster ? new ClusterBatchOptions(timeout: 1000, route: Route.Random) : new BatchOptions(timeout: 1000);
        object?[]? res = isCluster
            ? await ((GlideClusterClient)client).Exec((ClusterBatch)batch, true, (ClusterBatchOptions)options)
            : await ((GlideClient)client).Exec((Batch)batch, true, (BatchOptions)options);
        Assert.Equal(["OK"], res);
    }
}
