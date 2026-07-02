// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.TestUtils;

using static Valkey.Glide.Route;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for server management commands
/// </summary>
[Collection(typeof(ServerManagementCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ServerManagementCommandTests(ServerManagementCommandFixture fixture) : IClassFixture<ServerManagementCommandFixture>
{
    private static readonly SlotKeyRoute PrimarySlotRoute = new("1", SlotType.Primary);

    private GlideClient StandaloneClient => fixture.StandaloneClient!;
    private GlideClusterClient ClusterClient => fixture.ClusterClient!;

    /// <summary>
    /// Polls until the database is empty or the timeout expires.
    /// </summary>
    private static async Task WaitForEmptyDatabaseAsync(Func<Task<long>> databaseSize, TimeSpan? timeout = null)
    {
        TimeSpan effectiveTimeout = timeout ?? TimeSpan.FromSeconds(5);
        DateTimeOffset deadline = DateTimeOffset.UtcNow + effectiveTimeout;

        while (DateTimeOffset.UtcNow < deadline)
        {
            if (await databaseSize() == 0)
            {
                return;
            }

            await Task.Delay(100);
        }

        Assert.Equal(0, await databaseSize());
    }

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

        await WaitForEmptyDatabaseAsync(StandaloneClient.DatabaseSizeAsync);
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

        await WaitForEmptyDatabaseAsync(StandaloneClient.DatabaseSizeAsync);
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

        await WaitForEmptyDatabaseAsync(ClusterClient.DatabaseSizeAsync);
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithSyncMode_AndRoute()
    {
        string key = $"flush-cluster-sync-route-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        await ClusterClient.FlushDatabaseAsync(FlushMode.Sync, AllPrimaries);

        Assert.False(await ClusterClient.ExistsAsync(key));
        Assert.Equal(0, await ClusterClient.DatabaseSizeAsync());
    }

    #endregion
    #region LOLWUT Tests

    [Fact]
    public async Task LolwutAsync_Standalone_WithVersion()
        => AssertContainsServerName(await StandaloneClient.LolwutAsync(new LolwutOptions { Version = 5 }));

    [Fact]
    public async Task LolwutAsync_Standalone_WithVersionAndParameters()
        => AssertContainsServerName(await StandaloneClient.LolwutAsync(new LolwutOptions { Version = 5, Parameters = [40, 20] }));

    [Fact]
    public async Task LolwutAsync_Cluster_WithVersion()
        => AssertContainsServerName(await ClusterClient.LolwutAsync(new LolwutOptions { Version = 5 }));

    [Fact]
    public async Task LolwutAsync_Cluster_WithVersionAndParameters()
        => AssertContainsServerName(await ClusterClient.LolwutAsync(new LolwutOptions { Version = 5, Parameters = [40, 20] }));

    [Fact]
    public async Task LolwutAsync_Standalone_WithParametersOnly()
        => AssertContainsServerName(await StandaloneClient.LolwutAsync(new LolwutOptions { Parameters = [40, 20] }));

    [Fact]
    public async Task LolwutAsync_Cluster_WithParametersOnly()
        => AssertContainsServerName(await ClusterClient.LolwutAsync(new LolwutOptions { Parameters = [40, 20] }));

    #endregion
    #region CONFIG GET/SET Multi-Parameter Tests

    [Fact]
    public async Task ConfigGetAsync_Standalone_MultiplePatterns()
    {
        KeyValuePair<string, string>[] result = await StandaloneClient.ConfigGetAsync(
            [(ValkeyValue)"maxmemory", (ValkeyValue)"lfu-decay-time"]);

        Assert.True(result.Length >= 2);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
        Assert.Contains(result, kvp => kvp.Key == "lfu-decay-time");
    }

    [Fact]
    public async Task ConfigSetAsync_Standalone_MultipleParameters()
    {
        KeyValuePair<string, string>[] original = await StandaloneClient.ConfigGetAsync(
            [(ValkeyValue)"lfu-decay-time", (ValkeyValue)"lfu-log-factor"]);

        string originalDecayTime = original.First(kvp => kvp.Key == "lfu-decay-time").Value;
        string originalLogFactor = original.First(kvp => kvp.Key == "lfu-log-factor").Value;

        try
        {
            await StandaloneClient.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", "5" },
                { "lfu-log-factor", "20" }
            });

            KeyValuePair<string, string>[] result = await StandaloneClient.ConfigGetAsync(
                [(ValkeyValue)"lfu-decay-time", (ValkeyValue)"lfu-log-factor"]);

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
        KeyValuePair<string, string>[] result = await ClusterClient.ConfigGetAsync(
            [(ValkeyValue)"maxmemory", (ValkeyValue)"lfu-decay-time"]);

        Assert.True(result.Length >= 2);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
        Assert.Contains(result, kvp => kvp.Key == "lfu-decay-time");
    }

    [Fact]
    public async Task ConfigSetAsync_Cluster_MultipleParameters()
    {
        ClusterValue<KeyValuePair<string, string>[]> original = await ClusterClient.ConfigGetAsync(
            [(ValkeyValue)"lfu-decay-time", (ValkeyValue)"lfu-log-factor"],
            AllPrimaries);

        KeyValuePair<string, string>[] firstNodeOriginal = original.HasMultiData
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

            ClusterValue<KeyValuePair<string, string>[]> result = await ClusterClient.ConfigGetAsync(
                [(ValkeyValue)"lfu-decay-time", (ValkeyValue)"lfu-log-factor"],
                AllPrimaries);

            KeyValuePair<string, string>[] firstNodeResult = result.HasMultiData
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

        Assert.False(await ClusterClient.ExistsAsync(key));
        Assert.Equal(0, await ClusterClient.DatabaseSizeAsync());
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

        await WaitForEmptyDatabaseAsync(ClusterClient.DatabaseSizeAsync);
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_WithRoute()
    {
        string key = $"flushall-cluster-route-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");
        Assert.True(await ClusterClient.ExistsAsync(key));

        await ClusterClient.FlushAllDatabasesAsync(AllPrimaries);

        Assert.False(await ClusterClient.ExistsAsync(key));
        Assert.Equal(0, await ClusterClient.DatabaseSizeAsync());
    }

    #endregion
    #region WAITAOF Tests

    [Fact]
    public async Task WaitAofAsync_Standalone_ReturnsResults()
    {
        string key = $"waitaof-test-{Guid.NewGuid()}";
        await StandaloneClient.SetAsync(key, "test-value");

        long[] result = await StandaloneClient.WaitAofAsync(false, 0, TimeSpan.FromSeconds(2));

        Assert.Equal(2, result.Length);
        Assert.True(result[0] >= 0);
        Assert.True(result[1] >= 0);
    }

    [Fact]
    public async Task WaitAofAsync_Cluster_ReturnsResults()
    {
        string key = $"waitaof-cluster-test-{Guid.NewGuid()}";
        await ClusterClient.SetAsync(key, "test-value");

        long[] result = await ClusterClient.WaitAofAsync(false, 0, TimeSpan.FromSeconds(2));

        Assert.Equal(2, result.Length);
        Assert.True(result[0] >= 0);
        Assert.True(result[1] >= 0);
    }

    #endregion
    #region Latency Tests

    [Theory]
    [MemberData(nameof(Data.ClusterMode), MemberType = typeof(Data))]
    public async Task LatencyHistoryAsync(bool isCluster)
    {
        var beforeSpike = await GetServerTimeAsync(isCluster ? ClusterClient : StandaloneClient);
        await TriggerLatencySpikeAsync(isCluster ? ClusterClient : StandaloneClient);

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
        var beforeSpike = await GetServerTimeAsync(isCluster ? ClusterClient : StandaloneClient);
        await TriggerLatencySpikeAsync(isCluster ? ClusterClient : StandaloneClient);

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
        await TriggerLatencySpikeAsync(client);
        Assert.True(await client.LatencyResetAsync() > 0);

        var history = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyHistoryAsync("command"))
            : await StandaloneClient.LatencyHistoryAsync("command");
        Assert.Empty(history);

        // Trigger spike then reset specific event.
        await TriggerLatencySpikeAsync(client);
        Assert.True(await client.LatencyResetAsync("command") > 0);

        history = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyHistoryAsync("command"))
            : await StandaloneClient.LatencyHistoryAsync("command");
        Assert.Empty(history);

        // Trigger spike then reset unknown event.
        await TriggerLatencySpikeAsync(client);
        Assert.Equal(0, await client.LatencyResetAsync("unknown"));

        history = isCluster
            ? FlattenClusterValueLists(await ClusterClient.LatencyHistoryAsync("command"))
            : await StandaloneClient.LatencyHistoryAsync("command");
        Assert.NotEmpty(history);
    }

    [Fact]
    public async Task LatencyHistoryAsync_Cluster_WithRoute()
    {
        await TriggerLatencySpikeAsync(ClusterClient);

        // Verify default route returns multiple values.
        var multiHistory = await ClusterClient.LatencyHistoryAsync("command");
        Assert.True(multiHistory.HasMultiData);
        Assert.NotEmpty(FlattenClusterValueLists(multiHistory));

        // Verify that primary node route returns TODO.
        var singleHistory = await ClusterClient.LatencyHistoryAsync("command", PrimarySlotRoute);
        Assert.True(singleHistory.HasSingleData);
        Assert.NotEmpty(singleHistory.SingleValue);
    }

    [Fact]
    public async Task LatencyLatestAsync_Cluster_WithRoute()
    {
        await TriggerLatencySpikeAsync(ClusterClient);

        // Verify default route returns multiple values.
        var multiLatest = await ClusterClient.LatencyLatestAsync();
        Assert.True(multiLatest.HasMultiData);
        Assert.NotEmpty(FlattenClusterValueLists(multiLatest));

        // Verify that single primary node route TODO.
        var singleLatest = await ClusterClient.LatencyLatestAsync(PrimarySlotRoute);
        Assert.True(singleLatest.HasSingleData);
        Assert.NotEmpty(singleLatest.SingleValue);
    }

    [Fact]
    public async Task LatencyResetAsync_Cluster_WithRoute()
    {
        // Reset returns an aggregated count for all routes.
        await TriggerLatencySpikeAsync(ClusterClient);
        Assert.True(await ClusterClient.LatencyResetAsync(PrimarySlotRoute) > 0);

        await TriggerLatencySpikeAsync(ClusterClient);
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
    private static async Task<DateTimeOffset> GetServerTimeAsync(BaseClient client)
    {
        DateTimeOffset time = client is GlideClient standalone
            ? await standalone.TimeAsync()
            : (await ((GlideClusterClient)client).TimeAsync(Route.Random)).SingleValue;

        // Truncate the result to second precision to match latency entry timestamps.
        return DateTimeOffset.FromUnixTimeSeconds(time.ToUnixTimeSeconds());
    }

    /// <summary>
    /// Triggers a latency spike for the "command" event.
    /// </summary>
    private static async Task TriggerLatencySpikeAsync(BaseClient client)
    {
        // Reset any existing latency data first so the spike is recorded against a clean baseline.
        _ = await client.LatencyResetAsync();

        // Save the current threshold so we can restore it after the spike.
        KeyValuePair<string, string>[] prevConfigs;
        if (client is GlideClient standalone)
        {
            prevConfigs = await standalone.ConfigGetAsync("latency-monitor-threshold");
        }
        else
        {
            var prev = await ((GlideClusterClient)client).ConfigGetAsync("latency-monitor-threshold");
            prevConfigs = prev.HasSingleData ? prev.SingleValue : prev.MultiValue.Values.First();
        }

        var prevThreshold = prevConfigs.FirstOrDefault(p => p.Key == "latency-monitor-threshold").Value ?? "0";

        // Enable latency monitoring with a 1 ms threshold and trigger a latency spike.
        await client.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue> { { "latency-monitor-threshold", "1" } });

        // Trigger a latency spike for the "command" event.
        GlideString[] debugSleepArgs = ["DEBUG", "SLEEP", "0.05"];
        _ = client is GlideClient standaloneClient
            ? await standaloneClient.CustomCommand(debugSleepArgs)
            : await ((GlideClusterClient)client).CustomCommand(debugSleepArgs, Route.AllNodes);

        // Restore the original threshold.
        await client.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue> { { "latency-monitor-threshold", prevThreshold } });
    }

    #endregion
}

/// <summary>
/// Fixture that provides isolated Valkey server instances for server management tests.
/// Tests that call FlushAll/FlushDB need their own servers to avoid interfering with other tests.
/// </summary>
public class ServerManagementCommandFixture : IAsyncLifetime
{
    private StandaloneServer? _standaloneServer;
    private ClusterServer? _clusterServer;

    public GlideClient? StandaloneClient { get; private set; }
    public GlideClusterClient? ClusterClient { get; private set; }

    public async ValueTask InitializeAsync()
    {
        _standaloneServer = new StandaloneServer();
        _clusterServer = new ClusterServer();

        StandaloneClient = await _standaloneServer.CreateStandaloneClientAsync();
        ClusterClient = await _clusterServer.CreateClusterClientAsync();
    }

    public ValueTask DisposeAsync()
    {
        StandaloneClient?.Dispose();
        ClusterClient?.Dispose();

        _standaloneServer?.Dispose();
        _clusterServer?.Dispose();

        return new ValueTask();
    }
}
