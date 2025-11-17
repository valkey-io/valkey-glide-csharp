// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Pipeline;

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

// TODO: even though collections aren't executed in parallel, tests in a collection still parallelized
//       better to run tests in the named collections sequentially
[Collection(typeof(SharedBatchTests))]
[CollectionDefinition(DisableParallelization = true)]
public class SharedBatchTests
{
#pragma warning disable xUnit1047 // Avoid using TheoryDataRow arguments that might not be serializable
    public static IEnumerable<TheoryDataRow<BaseClient, bool>> GetTestClientWithAtomic =>
        TestConfiguration.TestClients.SelectMany(r => new TheoryDataRow<BaseClient, bool>[] { new(r.Data, true), new(r.Data, false) });
#pragma warning restore xUnit1047 // Avoid using TheoryDataRow arguments that might not be serializable

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(GetTestClientWithAtomic))]
    public async Task BatchRaiseOnError(BaseClient client, bool isAtomic)
    {
        bool isCluster = client is GlideClusterClient;
        string key1 = "{BatchRaiseOnError}" + Guid.NewGuid();
        string key2 = "{BatchRaiseOnError}" + Guid.NewGuid();

        Pipeline.IBatch batch = isCluster ? new ClusterBatch(isAtomic) : new Batch(isAtomic);
        // TODO replace custom command
        _ = batch.StringSet(key1, "hello").CustomCommand(["lpop", key1]).CustomCommand(["del", key1]).CustomCommand(["rename", key1, key2]);

        object?[] res = isCluster
            ? (await ((GlideClusterClient)client).Exec((ClusterBatch)batch, false))!
            : (await ((GlideClient)client).Exec((Batch)batch, false))!;

        // Exceptions aren't raised, but stored in the result set
        Assert.Multiple(
            () => Assert.Equal(4, res.Length),
            () => Assert.Equal(true, res[0]),
            () => Assert.Equal(1L, (long)res[2]!),
            () => Assert.IsType<RequestException>(res[1]),
            () => Assert.IsType<RequestException>(res[3]),
            () => Assert.Contains("wrong kind of value", (res[1] as RequestException)!.Message),
            () => Assert.Contains("no such key", (res[3] as RequestException)!.Message)
        );

        // First exception is raised, all data lost
        Exception err = await Assert.ThrowsAsync<RequestException>(async () => _ = isCluster
                ? await ((GlideClusterClient)client).Exec((ClusterBatch)batch, true)
                : await ((GlideClient)client).Exec((Batch)batch, true));
        Assert.Contains("wrong kind of value", err.Message);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(GetTestClientWithAtomic))]
    public async Task BatchDumpAndRestore(BaseClient client, bool isAtomic)
    {
        bool isCluster = client is GlideClusterClient;
        string key1 = "{DumpRestore}" + Guid.NewGuid();
        string key2 = "{DumpRestore}" + Guid.NewGuid();

        Pipeline.IBatch batch = isCluster ? new ClusterBatch(isAtomic) : new Batch(isAtomic);
        _ = batch.StringSet(key1, "hello").KeyDump(key1);

        object?[] res = isCluster
            ? (await ((GlideClusterClient)client).Exec((ClusterBatch)batch, false))!
            : (await ((GlideClient)client).Exec((Batch)batch, false))!;

        Assert.Multiple(
            () => Assert.Equal(2, res.Length),
            () => Assert.True((bool)res[0]!),
            () => Assert.IsType<byte[]?>(res[1])
        );

        Pipeline.IBatch batch2 = isCluster ? new ClusterBatch(isAtomic) : new Batch(isAtomic);
        _ = batch2.KeyDelete([key1, key2]).KeyRestore(key1, (byte[])res[1]!).KeyRestoreDateTime(key2, (byte[])res[1]!);

        res = isCluster
            ? (await ((GlideClusterClient)client).Exec((ClusterBatch)batch2, false))!
            : (await ((GlideClient)client).Exec((Batch)batch2, false))!;

        Assert.Multiple(
            () => Assert.Equal(1L, (long)res[0]!),
            () => Assert.Equal("OK", res[1]),
            () => Assert.Equal("OK", res[2])
        );

    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task WatchTransactionTest(BaseClient client)
    {
        string key1 = "{key}-1" + Guid.NewGuid();
        string key2 = "{key}-2" + Guid.NewGuid();
        string key3 = "{key}-3" + Guid.NewGuid();
        string foobarString = "foobar";
        string helloString = "hello";
        ValkeyKey[] keys = [key1, key2, key3];

        bool isCluster = client is GlideClusterClient;

        // Returns null when a watched key is modified before transaction execution
        if (isCluster)
        {
            await ((GlideClusterClient)client).WatchAsync(keys);
        }
        else
        {
            await ((GlideClient)client).WatchAsync(keys);
        }

        await client.StringSetAsync(key2, helloString);

        object?[]? execResult;
        if (isCluster)
        {
            var clusterBatch = new ClusterBatch(true);
            _ = clusterBatch.StringSetAsync(key1, foobarString)
                           .StringSetAsync(key2, foobarString)
                           .StringSetAsync(key3, foobarString);
            execResult = await ((GlideClusterClient)client).Exec(clusterBatch, true);
        }
        else
        {
            var batch = new Batch(true);
            _ = batch.StringSetAsync(key1, foobarString)
                    .StringSetAsync(key2, foobarString)
                    .StringSetAsync(key3, foobarString);
            execResult = await ((GlideClient)client).Exec(batch, true);
        }

        // The transaction should fail (return null) because key2 was modified after being watched
        Assert.Null(execResult);

        // Verify the key values: transaction was aborted, so only key2 (set before transaction) should have a value
        var key1Value = await client.StringGetAsync(key1);
        var key2Value = await client.StringGetAsync(key2);
        var key3Value = await client.StringGetAsync(key3);

        Assert.True(key1Value.IsNull); // key1 should not be set
        Assert.Equal(helloString, key2Value); // key2 should have the value set before transaction
        Assert.True(key3Value.IsNull); // key3 should not be set
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task UnwatchTest(BaseClient client)
    {
        string key1 = "{key}-1" + Guid.NewGuid();
        string key2 = "{key}-2" + Guid.NewGuid();
        string foobarString = "foobar";
        string helloString = "hello";
        ValkeyKey[] keys = [key1, key2];

        bool isCluster = client is GlideClusterClient;

        // UNWATCH succeeds when there are no watched keys
        if (isCluster)
        {
            await ((GlideClusterClient)client).UnwatchAsync();
        }
        else
        {
            await ((GlideClient)client).UnwatchAsync();
        }

        // Transaction executes successfully after modifying a watched key then calling UNWATCH
        if (isCluster)
        {
            await ((GlideClusterClient)client).WatchAsync(keys);
        }
        else
        {
            await ((GlideClient)client).WatchAsync(keys);
        }
        await client.StringSetAsync(key2, helloString);
        if (isCluster)
        {
            await ((GlideClusterClient)client).UnwatchAsync();
        }
        else
        {
            await ((GlideClient)client).UnwatchAsync();
        }

        object?[]? execResult;
        if (isCluster)
        {
            var clusterBatch = new ClusterBatch(true);
            _ = clusterBatch.StringSetAsync(key1, foobarString).StringSetAsync(key2, foobarString);
            execResult = await ((GlideClusterClient)client).Exec(clusterBatch, true);
        }
        else
        {
            var batch = new Batch(true);
            _ = batch.StringSetAsync(key1, foobarString).StringSetAsync(key2, foobarString);
            execResult = await ((GlideClient)client).Exec(batch, true);
        }

        Assert.NotNull(execResult); // Transaction should succeed after unwatch
        Assert.Equal(2, execResult.Length);
        Assert.Equal(foobarString, await client.StringGetAsync(key1));
        Assert.Equal(foobarString, await client.StringGetAsync(key2));
    }
}
