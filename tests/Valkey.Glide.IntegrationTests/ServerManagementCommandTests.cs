// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.TestUtils;

using static Valkey.Glide.Commands.Options.InfoOptions;
using static Valkey.Glide.Errors;
using static Valkey.Glide.Route;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for server management commands
/// </summary>
[Collection(typeof(ServerManagementCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ServerManagementCommandTests(ClientFixture fixture) : IClassFixture<ClientFixture>
{
    private GlideClient StandaloneClient => fixture.StandaloneClient;
    private GlideClusterClient ClusterClient => fixture.ClusterClient;

    /// <summary>
    /// Polls until no save (RDB or AOF rewrite) is in progress on the standalone client.
    /// </summary>
    private static async Task WaitForSaveNotInProgressAsync(GlideClient client)
    {
        await Polling.WaitForAsync(async () =>
        {
            string info = await client.InfoAsync([InfoOptions.Section.PERSISTENCE]);
            return !info.Contains("rdb_bgsave_in_progress:1")
                && !info.Contains("aof_rewrite_in_progress:1");
        }, "Timed out waiting for save to complete");
    }

    /// <summary>
    /// Polls until no save (RDB or AOF rewrite) is in progress on the cluster client.
    /// </summary>
    private async Task WaitForClusterSaveNotInProgressAsync()
    {
        await Polling.WaitForAsync(async () =>
        {
            Dictionary<string, string> infos = await ClusterClient.InfoAsync([InfoOptions.Section.PERSISTENCE]);
            string combined = string.Join("\n", infos.Values);
            return !combined.Contains("rdb_bgsave_in_progress:1")
                && !combined.Contains("aof_rewrite_in_progress:1");
        }, "Timed out waiting for save to complete");
    }

    #region SAVE Tests

    [Fact]
    public async Task SaveAsync_Standalone_Succeeds()
    {
        await WaitForSaveNotInProgressAsync(StandaloneClient);
        await StandaloneClient.SaveAsync();
    }

    [Fact]
    public async Task SaveAsync_Cluster_Succeeds()
    {
        await WaitForClusterSaveNotInProgressAsync();
        await ClusterClient.SaveAsync();
    }

    [Fact]
    public async Task SaveAsync_Cluster_WithRoute_Succeeds()
    {
        await WaitForClusterSaveNotInProgressAsync();
        await ClusterClient.SaveAsync(Route.AllPrimaries);
    }

    #endregion

    #region FlushMode Tests

    [Fact]
    public async Task FlushDatabaseAsync_Standalone_WithSyncMode()
    {
        string key = $"flush-sync-test-{Guid.NewGuid()}";
        await StandaloneClient.SetAsync(key, "test-value");
        Assert.True(await StandaloneClient.ExistsAsync(key));

        await StandaloneClient.FlushDatabaseAsync(FlushMode.Sync);

        Assert.False(await StandaloneClient.ExistsAsync(key));
        Assert.Equal(0, await StandaloneClient.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Standalone_WithAsyncMode()
    {
        string key = $"flush-async-test-{Guid.NewGuid()}";
        await StandaloneClient.SetAsync(key, "test-value");
        Assert.True(await StandaloneClient.ExistsAsync(key));

        await StandaloneClient.FlushDatabaseAsync(FlushMode.Async);
        await WaitForFlushedAsync(StandaloneClient);
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Standalone_WithSyncMode()
    {
        string key = $"flushall-sync-test-{Guid.NewGuid()}";
        await StandaloneClient.SetAsync(key, "test-value");
        Assert.True(await StandaloneClient.ExistsAsync(key));

        await StandaloneClient.FlushAllDatabasesAsync(FlushMode.Sync);

        Assert.False(await StandaloneClient.ExistsAsync(key));
        Assert.Equal(0, await StandaloneClient.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Standalone_WithAsyncMode()
    {
        string key = $"flushall-async-test-{Guid.NewGuid()}";
        await StandaloneClient.SetAsync(key, "test-value");
        Assert.True(await StandaloneClient.ExistsAsync(key));

        await StandaloneClient.FlushAllDatabasesAsync(FlushMode.Async);
        await WaitForFlushedAsync(StandaloneClient);
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithSyncMode()
    {
        string key = $"flush-cluster-sync-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        await ClusterClient.FlushDatabaseAsync(FlushMode.Sync);

        Assert.False(await ClusterClient.ExistsAsync(key));
        Assert.Equal(0, await ClusterClient.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithAsyncMode()
    {
        string key = $"flush-cluster-async-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        await ClusterClient.FlushDatabaseAsync(FlushMode.Async);
        await WaitForFlushedAsync(ClusterClient);
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithSyncMode_AndRoute()
    {
        string key = $"flush-cluster-sync-route-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        var route = new SlotKeyRoute(key, SlotType.Primary);
        await ClusterClient.FlushDatabaseAsync(FlushMode.Sync, route);

        await WaitForKeyRemovedAsync(key);
    }

    #endregion
    #region LOLWUT Tests

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task LolwutAsync_WithVersion(bool clusterMode)
    {
        var options = new LolwutOptions { Version = 5 };
        var result = clusterMode
            ? await ClusterClient.LolwutAsync(options)
            : await StandaloneClient.LolwutAsync(options);

        AssertContainsServerName(result);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task LolwutAsync_WithVersionAndParameters(bool clusterMode)
    {
        var optipns = new LolwutOptions { Version = 5, Parameters = [40, 20] };
        var result = clusterMode
            ? await ClusterClient.LolwutAsync(optipns)
            : await StandaloneClient.LolwutAsync(optipns);

        AssertContainsServerName(result);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task LolwutAsync_WithParametersOnly(bool clusterMode)
    {
        var options = new LolwutOptions { Parameters = [40, 20] };
        var result = clusterMode
            ? await ClusterClient.LolwutAsync(options)
            : await StandaloneClient.LolwutAsync(options);

        AssertContainsServerName(result);
    }

    #endregion
    #region CONFIG GET/SET Multi-Parameter Tests

    [Fact]
    public async Task ConfigGetAsync_Standalone_MultiplePatterns()
    {
        var result = await StandaloneClient.ConfigGetAsync(
            ["maxmemory", "lfu-decay-time"]);

        Assert.True(result.Length >= 2);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
        Assert.Contains(result, kvp => kvp.Key == "lfu-decay-time");
    }

    [Fact]
    public async Task ConfigSetAsync_Standalone_MultipleParameters()
    {
        var original = await StandaloneClient.ConfigGetAsync(
            ["lfu-decay-time", "maxmemory", "lfu-log-factor"]);

        string originalDecayTime = original.First(kvp => kvp.Key == "lfu-decay-time").Value;
        string originalLogFactor = original.First(kvp => kvp.Key == "lfu-log-factor").Value;

        try
        {
            await StandaloneClient.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", "5" },
                { "lfu-log-factor", "20" }
            });

            var result = await StandaloneClient.ConfigGetAsync(
                ["lfu-decay-time", "lfu-log-factor"]);

            Assert.Equal("5", result.First(kvp => kvp.Key == "lfu-decay-time").Value);
            Assert.Equal("20", result.First(kvp => kvp.Key == "lfu-log-factor").Value);
        }
        finally
        {
            await StandaloneClient.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", originalDecayTime },
                { "lfu-log-factor", originalLogFactor }
            });
        }
    }

    [Fact]
    public async Task ConfigGetAsync_Cluster_MultiplePatterns()
    {
        var result = await ClusterClient.ConfigGetAsync(
            ["maxmemory", "lfu-decay-time"]);

        Assert.True(result.Length >= 2);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
        Assert.Contains(result, kvp => kvp.Key == "lfu-decay-time");
    }

    [Fact]
    public async Task ConfigSetAsync_Cluster_MultipleParameters()
    {
        var original = await ClusterClient.ConfigGetAsync(
            ["lfu-decay-time", "lfu-log-factor"], AllPrimaries);

        var firstNodeOriginal = original.HasMultiData
            ? original.MultiValue.Values.First()
            : original.SingleValue;

        string originalDecayTime = firstNodeOriginal.First(kvp => kvp.Key == "lfu-decay-time").Value;
        string originalLogFactor = firstNodeOriginal.First(kvp => kvp.Key == "lfu-log-factor").Value;

        try
        {
            await ClusterClient.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", "5" },
                { "lfu-log-factor", "20" }
            });

            var result = await ClusterClient.ConfigGetAsync(
                ["lfu-decay-time", "lfu-log-factor"], AllPrimaries);

            var firstNodeResult = result.HasMultiData
                ? result.MultiValue.Values.First()
                : result.SingleValue;

            Assert.Equal("5", firstNodeResult.First(kvp => kvp.Key == "lfu-decay-time").Value);
            Assert.Equal("20", firstNodeResult.First(kvp => kvp.Key == "lfu-log-factor").Value);
        }
        finally
        {
            await ClusterClient.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", originalDecayTime },
                { "lfu-log-factor", originalLogFactor }
            });
        }
    }

    #endregion
    #region FlushAllDatabases Cluster Tests

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_ClearsAllDatabases()
    {
        string key = $"flushall-cluster-test-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        await ClusterClient.FlushAllDatabasesAsync();
        await WaitForFlushedAsync(ClusterClient);
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_WithSyncMode()
    {
        string key = $"flushall-cluster-sync-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        await ClusterClient.FlushAllDatabasesAsync(FlushMode.Sync);

        Assert.False(await ClusterClient.ExistsAsync(key));
        Assert.Equal(0, await ClusterClient.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_WithAsyncMode()
    {
        string key = $"flushall-cluster-async-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        await ClusterClient.FlushAllDatabasesAsync(FlushMode.Async);
        await WaitForFlushedAsync(ClusterClient);
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_WithRoute()
    {
        string key = $"flushall-cluster-route-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        var route = new SlotKeyRoute(key, SlotType.Primary);
        await ClusterClient.FlushAllDatabasesAsync(route);

        await WaitForKeyRemovedAsync(key);
    }

    #endregion
    #region WAITAOF Tests

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task WaitAofAsync_ReturnsResults(bool clusterMode)
    {
        var client = fixture.GetClient(clusterMode);

        string key = $"waitaof-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");

        var result = await client.WaitAofAsync(false, 0, TimeSpan.FromSeconds(2));

        Assert.Equal(2, result.Length);
        Assert.True(result[0] >= 0);
        Assert.True(result[1] >= 0);
    }

    #endregion
    #region BgSave Tests

    /// <summary>
    /// Expected valid responses for <c>BGSAVE</c> and <c>BGSAVE SCHEDULE</c> commands.
    /// </summary>
    private static readonly string[] BgSaveResponses =
    [
        "Background saving started",
        "Background saving scheduled",
    ];

    /// <summary>
    /// Expected server error response for <c>BGSAVE CANCEL</c> when no save is in progress.
    /// </summary>
    private const string BgSaveNotCancelledResponse = "Background saving is currently not in progress or scheduled";

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task BackgroundSaveAsync_ReturnsExpectedResponse(bool clusterMode)
    {
        await WaitForSaveNotInProgressAsync(clusterMode);

        IEnumerable<string> responses = clusterMode
            ? (await ClusterClient.BackgroundSaveAsync()).MultiValue.Values
            : [await StandaloneClient.BackgroundSaveAsync()];

        Assert.All(responses, r => Assert.Contains(r, BgSaveResponses));
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task BackgroundSaveScheduleAsync_ReturnsExpectedResponse(bool clusterMode)
    {
        await WaitForSaveNotInProgressAsync(clusterMode);

        IEnumerable<string> responses = clusterMode
            ? (await ClusterClient.BackgroundSaveScheduleAsync()).MultiValue.Values
            : [await StandaloneClient.BackgroundSaveScheduleAsync()];

        Assert.All(responses, r => Assert.Contains(r, BgSaveResponses));
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task BackgroundSaveCancelAsync_ThrowsWhenNoSaveInProgress(bool clusterMode)
    {
        Skip.IfBgSaveCancelNotSupported();
        await WaitForSaveNotInProgressAsync(clusterMode);

        BaseClient client = clusterMode ? ClusterClient : StandaloneClient;
        Func<Task> act = clusterMode
            ? ClusterClient.BackgroundSaveCancelAsync
            : StandaloneClient.BackgroundSaveCancelAsync;

        var ex = await Assert.ThrowsAsync<RequestException>(act);
        Assert.Contains(BgSaveNotCancelledResponse, ex.Message);
    }

    [Fact]
    public async Task BackgroundSaveAsync_Cluster_WithRoute_ReturnsSingleValue()
    {
        await WaitForSaveNotInProgressAsync(true);

        var result = await ClusterClient.BackgroundSaveAsync(Route.Random);
        Assert.Contains(result.SingleValue, BgSaveResponses);
    }

    [Fact]
    public async Task BackgroundSaveScheduleAsync_Cluster_WithRoute_ReturnsSingleValue()
    {
        await WaitForSaveNotInProgressAsync(true);

        var result = await ClusterClient.BackgroundSaveScheduleAsync(Route.Random);
        Assert.Contains(result.SingleValue, BgSaveResponses);
    }

    [Fact]
    public async Task BackgroundSaveCancelAsync_Cluster_WithRoute_ThrowsWhenNoSaveInProgress()
    {
        Skip.IfBgSaveCancelNotSupported();
        await WaitForSaveNotInProgressAsync(true);

        var ex = await Assert.ThrowsAsync<RequestException>(() => ClusterClient.BackgroundSaveCancelAsync(Route.Random));
        Assert.Contains(BgSaveNotCancelledResponse, ex.Message);
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Asserts that the result contains the expected server name.
    /// </summary>
    private static void AssertContainsServerName(string result)
        => Assert.Contains(["VALKEY", "REDIS"], name => result.Contains(name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Polls until the database is empty.
    /// </summary>
    private static Task WaitForFlushedAsync(BaseClient client)
        => Polling.WaitForAsync(async () =>
        {
            long size = client switch
            {
                GlideClusterClient cluster => await cluster.DatabaseSizeAsync(),
                GlideClient standalone => await standalone.DatabaseSizeAsync(),
                _ => throw new ArgumentException("Unsupported client type", nameof(client)),
            };

            return size == 0;
        }, "Timed out waiting for database empty");

    /// <summary>
    /// Polls until no RDB save or AOF rewrite is in progress on any node.
    /// </summary>
    private Task WaitForSaveNotInProgressAsync(bool clusterMode)
        => Polling.WaitForAsync(async () =>
        {
            var args = new[] { Section.PERSISTENCE };
            IEnumerable<string> infoValues = clusterMode
                ? (await ClusterClient.InfoAsync(args)).Values
                : [await StandaloneClient.InfoAsync(args)];

            return infoValues.All(info =>
                !info.Contains("rdb_bgsave_in_progress:1")
                && !info.Contains("aof_rewrite_in_progress:1"));
        }, "Timed out waiting for save to complete");

    /// <summary>
    /// Polls until the specified key does not exist.
    /// </summary>
    private Task WaitForKeyRemovedAsync(string key)
        => Polling.WaitForAsync(
            async () => !await ClusterClient.ExistsAsync(key),
            $"Timed out waiting for key '{key}' to be removed");

    #endregion
}
