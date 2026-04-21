// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for server management commands
/// </summary>
[Collection(typeof(ServerManagementCommandTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ServerManagementCommandTests(ServerManagementCommandFixture fixture) : IClassFixture<ServerManagementCommandFixture>
{
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

        await ClusterClient.FlushDatabaseAsync(FlushMode.Sync, Route.AllPrimaries);

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
            Route.AllPrimaries);

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
                Route.AllPrimaries);

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

        await ClusterClient.FlushAllDatabasesAsync(Route.AllPrimaries);

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

    /// <summary>
    /// Asserts that the result contains the expected server name.
    /// </summary>
    private static void AssertContainsServerName(string result)
        => Assert.Contains(["VALKEY", "REDIS"], name => result.Contains(name, StringComparison.OrdinalIgnoreCase));
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
