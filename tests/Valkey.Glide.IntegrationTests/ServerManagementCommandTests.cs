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

    #region Constants

    /// <summary>
    /// Expected valid responses for <c>BGREWRITEAOF</c> command.
    /// </summary>
    private static readonly string[] BgRewriteAofResponses =
    [
        "Background append only file rewriting started",
        "Background append only file rewriting scheduled",
    ];

    ///  <summary>
    /// Route to a primary node.
    /// </summary>
    private static readonly SlotKeyRoute PrimaryRoute = new("1", SlotType.Primary);

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
        await WaitForFlushedAsync(isCluster: false);
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
        await WaitForFlushedAsync(isCluster: false);
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
        await WaitForFlushedAsync(isCluster: true);
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
        await WaitForFlushedAsync(isCluster: true);
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
        await WaitForFlushedAsync(isCluster: true);
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
    #region BGREWRITEAOF Tests

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task BgRewriteAofAsync_ReturnsValidStatus(bool clusterMode)
    {
        await WaitForSaveNotInProgressAsync(clusterMode);

        IEnumerable<string> responses = clusterMode
            ? (await ClusterClient.BgRewriteAofAsync()).MultiValue.Values
            : [await StandaloneClient.BgRewriteAofAsync()];

        Assert.All(responses, r => Assert.Contains(r, BgRewriteAofResponses));
    }

    [Fact]
    public async Task BgRewriteAofAsync_Cluster_WithRoute_ReturnsSingleValue()
    {
        await WaitForSaveNotInProgressAsync(true);

        var result = await ClusterClient.BgRewriteAofAsync(Route.Random);
        Assert.Contains(result.SingleValue, BgRewriteAofResponses);
    }

    #endregion
    #region Latency Tests

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task LatencyHistoryAsync(bool isCluster)
    {
        var beforeSpike = await GetServerTimeAsync(isCluster);
        await TriggerLatencySpikeAsync(isCluster);

        LatencyEntry[] allEntries = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyHistoryAsync("command"))
            : await StandaloneClient.LatencyHistoryAsync("command");

        Assert.NotEmpty(allEntries);
        foreach (var entry in allEntries)
        {
            Assert.True(entry.Time >= beforeSpike);
            Assert.True(entry.Duration > TimeSpan.Zero);
        }

        // Non-existent event returns empty.
        var unknown = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyHistoryAsync("nonexistent"))
            : await StandaloneClient.LatencyHistoryAsync("nonexistent");
        Assert.Empty(unknown);
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task LatencyLatestAsync(bool isCluster)
    {
        var beforeSpike = await GetServerTimeAsync(isCluster);
        await TriggerLatencySpikeAsync(isCluster);

        // Verify latest events.
        LatencyEventInfo[] allEntries = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyLatestAsync())
            : await StandaloneClient.LatencyLatestAsync();

        Assert.True(allEntries.Length >= 1);

        // Verify the "command" event.
        var commandInfo = allEntries.First(e => e.EventName == "command");

        Assert.True(commandInfo.LatestTime >= beforeSpike);
        Assert.True(commandInfo.LatestDuration > TimeSpan.Zero);
        Assert.True(commandInfo.MaxDuration >= commandInfo.LatestDuration);

        BaseClient client = isCluster ? ClusterClient : StandaloneClient;
        if (Client.GetVersion(client) >= new Version(8, 1, 0))
        {
            Assert.True(Assert.NotNull(commandInfo.Sum) > TimeSpan.Zero);
            Assert.True(Assert.NotNull(commandInfo.Count) > 0);
        }
        else
        {
            Assert.Null(commandInfo.Sum);
            Assert.Null(commandInfo.Count);
        }
    }

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task LatencyResetAsync(bool isCluster)
    {
        BaseClient client = isCluster ? ClusterClient : StandaloneClient;

        // Trigger spike then reset all events.
        await TriggerLatencySpikeAsync(isCluster);
        Assert.True(await client.LatencyResetAsync() > 0);

        var history = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyHistoryAsync("command"))
            : await StandaloneClient.LatencyHistoryAsync("command");
        Assert.Empty(history);

        // Trigger spike then reset specific event.
        await TriggerLatencySpikeAsync(isCluster);
        Assert.True(await client.LatencyResetAsync("command") > 0);

        history = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyHistoryAsync("command"))
            : await StandaloneClient.LatencyHistoryAsync("command");
        Assert.Empty(history);

        // Trigger spike then reset unknown event.
        await TriggerLatencySpikeAsync(isCluster);
        Assert.Equal(0, await client.LatencyResetAsync("unknown"));

        history = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyHistoryAsync("command"))
            : await StandaloneClient.LatencyHistoryAsync("command");
        Assert.NotEmpty(history);
    }

    [Fact]
    public async Task LatencyHistoryAsync_Cluster_WithRoute()
    {
        await TriggerLatencySpikeAsync(isCluster: true);

        // Verify default route returns multiple values.
        var multiHistory = await ClusterClient.LatencyHistoryAsync("command");
        Assert.True(multiHistory.HasMultiData);
        Assert.NotEmpty(FlattenClusterValueLists(multiHistory));

        // Verify that primary node route returns single value.
        var singleHistory = await ClusterClient.LatencyHistoryAsync("command", PrimaryRoute);
        Assert.True(singleHistory.HasSingleData);
        Assert.NotEmpty(singleHistory.SingleValue);
    }

    [Fact]
    public async Task LatencyLatestAsync_Cluster_WithRoute()
    {
        await TriggerLatencySpikeAsync(isCluster: true);

        // Verify default route returns multiple values.
        var multiLatest = await ClusterClient.LatencyLatestAsync();
        Assert.True(multiLatest.HasMultiData);
        Assert.NotEmpty(FlattenClusterValueLists(multiLatest));

        // Verify that single primary node route returns single value.
        var singleLatest = await ClusterClient.LatencyLatestAsync(PrimaryRoute);
        Assert.True(singleLatest.HasSingleData);
        Assert.NotEmpty(singleLatest.SingleValue);
    }

    [Fact]
    public async Task LatencyResetAsync_Cluster_WithRoute()
    {
        await TriggerLatencySpikeAsync(isCluster: true);
        Assert.True(await ClusterClient.LatencyResetAsync(PrimaryRoute) > 0);

        await TriggerLatencySpikeAsync(isCluster: true);
        Assert.True(await ClusterClient.LatencyResetAsync(AllNodes) > 0);
    }

    #endregion
    #region Helpers

    /// <summary>
    /// Asserts that the result contains the expected server name.
    /// </summary>
    private static void AssertContainsServerName(string result)
        => Assert.Contains(["VALKEY", "REDIS"], name => result.Contains(name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Flattens a cluster value of arrays into a single flat array.
    /// </summary>
    private static T[] FlattenClusterValueLists<T>(ClusterValue<T[]> clusterValue)
        => clusterValue.HasMultiData
            ? [.. clusterValue.MultiValue.Values.SelectMany(v => v)]
            : clusterValue.SingleValue;

    /// <summary>
    /// Gets the current server time.
    /// </summary>
    private async Task<DateTimeOffset> GetServerTimeAsync(bool isCluster)
    {
        DateTimeOffset time = isCluster
            ? (await ClusterClient.TimeAsync(Route.Random)).SingleValue
            : await StandaloneClient.TimeAsync();

        // Truncate the result to second precision to match latency entry timestamps.
        return DateTimeOffset.FromUnixTimeSeconds(time.ToUnixTimeSeconds());
    }

    /// <summary>
    /// Triggers a latency spike for the "command" event.
    /// </summary>
    private async Task TriggerLatencySpikeAsync(bool isCluster)
    {
        var latencyThresholdParam = "latency-monitor-threshold";
        BaseClient client = isCluster ? ClusterClient : StandaloneClient;

        // Reset any existing latency data first so the spike is recorded against a clean baseline.
        _ = await client.LatencyResetAsync();

        // Save the current threshold so we can restore it after the spike.
        KeyValuePair<string, string>[] prevConfigs;
        if (isCluster)
        {
            var prev = await ClusterClient.ConfigGetAsync(latencyThresholdParam);
            prevConfigs = prev.HasSingleData ? prev.SingleValue : prev.MultiValue.Values.First();
        }
        else
        {
            prevConfigs = await StandaloneClient.ConfigGetAsync(latencyThresholdParam);
        }

        var prevThreshold = prevConfigs.FirstOrDefault(p => p.Key == latencyThresholdParam).Value ?? "0";

        // Enable latency monitoring with a 1 ms threshold and trigger a latency spike.
        await client.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue> { { latencyThresholdParam, "1" } });

        // Trigger a latency spike for the "command" event.
        GlideString[] debugSleepArgs = ["DEBUG", "SLEEP", "0.05"];
        _ = isCluster
            ? await ClusterClient.CustomCommand(debugSleepArgs, AllNodes)
            : await StandaloneClient.CustomCommand(debugSleepArgs);

        // Restore the original threshold.
        await client.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue> { { latencyThresholdParam, prevThreshold } });
    }

    /// <summary>
    /// Polls until the database is empty.
    /// </summary>
    private Task WaitForFlushedAsync(bool isCluster)
        => Polling.WaitForAsync(async () =>
        {
            long size = isCluster
                ? await ClusterClient.DatabaseSizeAsync()
                : await StandaloneClient.DatabaseSizeAsync();

            return size == 0;
        }, "Timed out waiting for database empty");

    /// <summary>
    /// Polls until the specified key does not exist.
    /// </summary>
    private Task WaitForKeyRemovedAsync(string key)
        => Polling.WaitForAsync(
            async () => !await ClusterClient.ExistsAsync(key),
            $"Timed out waiting for key '{key}' to be removed");

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

    #endregion
}
