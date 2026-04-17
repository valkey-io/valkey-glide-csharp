// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for server management commands: FlushMode, LOLWUT version/parameters, CONFIG GET/SET multi-parameter.
/// </summary>
public class ServerManagementCommandTests
{
    /// <summary>
    /// Polls until the database is empty or the timeout expires.
    /// Used by async-flush tests where the server returns immediately while the flush continues in the background.
    /// </summary>
    private static async Task WaitForEmptyDatabaseAsync(Func<Task<long>> databaseSize, TimeSpan? timeout = null)
    {
        TimeSpan effectiveTimeout = timeout ?? TimeSpan.FromSeconds(5);
        DateTime deadline = DateTime.UtcNow + effectiveTimeout;

        while (DateTime.UtcNow < deadline)
        {
            if (await databaseSize() == 0)
                return;

            await Task.Delay(100);
        }

        Assert.Equal(0, await databaseSize());
    }
    #region FlushMode Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_Standalone_WithSyncMode(GlideClient client)
    {
        string key = $"flush-sync-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Sync);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_Standalone_WithAsyncMode(GlideClient client)
    {
        string key = $"flush-async-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Async);

        // Async flush returns immediately; poll until the database is empty.
        await WaitForEmptyDatabaseAsync(client.DatabaseSizeAsync);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_Standalone_WithSyncMode(GlideClient client)
    {
        string key = $"flushall-sync-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(FlushMode.Sync);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_Standalone_WithAsyncMode(GlideClient client)
    {
        string key = $"flushall-async-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(FlushMode.Async);

        // Async flush returns immediately; poll until the database is empty.
        await WaitForEmptyDatabaseAsync(client.DatabaseSizeAsync);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_Cluster_WithSyncMode(GlideClusterClient client)
    {
        string key = $"flush-cluster-sync-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Sync);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_Cluster_WithAsyncMode(GlideClusterClient client)
    {
        string key = $"flush-cluster-async-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Async);

        // Async flush returns immediately; poll until the database is empty.
        await WaitForEmptyDatabaseAsync(client.DatabaseSizeAsync);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_Cluster_WithSyncMode_AndRoute(GlideClusterClient client)
    {
        string key = $"flush-cluster-sync-route-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Sync, Route.AllPrimaries);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    #endregion

    #region LOLWUT Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task LolwutAsync_Standalone_WithVersion(GlideClient client)
    {
        string result = await client.LolwutAsync(new LolwutOptions { Version = 5 });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task LolwutAsync_Standalone_WithVersionAndParameters(GlideClient client)
    {
        string result = await client.LolwutAsync(new LolwutOptions { Version = 5, Parameters = [40, 20] });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task LolwutAsync_Cluster_WithVersion(GlideClusterClient client)
    {
        string result = await client.LolwutAsync(new LolwutOptions { Version = 5 });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task LolwutAsync_Cluster_WithVersionAndParameters(GlideClusterClient client)
    {
        string result = await client.LolwutAsync(new LolwutOptions { Version = 5, Parameters = [40, 20] });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region CONFIG GET/SET Multi-Parameter Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigGetAsync_Standalone_MultiplePatterns(GlideClient client)
    {
        KeyValuePair<string, string>[] result = await client.ConfigGetAsync(
            [(ValkeyValue)"maxmemory", (ValkeyValue)"lfu-decay-time"]);

        Assert.NotNull(result);
        Assert.True(result.Length >= 2);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
        Assert.Contains(result, kvp => kvp.Key == "lfu-decay-time");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigSetAsync_Standalone_MultipleParameters(GlideClient client)
    {
        // Get original values
        KeyValuePair<string, string>[] original = await client.ConfigGetAsync(
            [(ValkeyValue)"lfu-decay-time", (ValkeyValue)"lfu-log-factor"]);

        string originalDecayTime = original.First(kvp => kvp.Key == "lfu-decay-time").Value;
        string originalLogFactor = original.First(kvp => kvp.Key == "lfu-log-factor").Value;

        try
        {
            // Set multiple parameters at once
            await client.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", "5" },
                { "lfu-log-factor", "20" }
            });

            // Verify the values were set
            KeyValuePair<string, string>[] result = await client.ConfigGetAsync(
                [(ValkeyValue)"lfu-decay-time", (ValkeyValue)"lfu-log-factor"]);

            Assert.Equal("5", result.First(kvp => kvp.Key == "lfu-decay-time").Value);
            Assert.Equal("20", result.First(kvp => kvp.Key == "lfu-log-factor").Value);
        }
        finally
        {
            // Restore original values
            await client.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", originalDecayTime },
                { "lfu-log-factor", originalLogFactor }
            });
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigGetAsync_Cluster_MultiplePatterns(GlideClusterClient client)
    {
        KeyValuePair<string, string>[] result = await client.ConfigGetAsync(
            [(ValkeyValue)"maxmemory", (ValkeyValue)"lfu-decay-time"]);

        Assert.NotNull(result);
        Assert.True(result.Length >= 2);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
        Assert.Contains(result, kvp => kvp.Key == "lfu-decay-time");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task ConfigSetAsync_Cluster_MultipleParameters(GlideClusterClient client)
    {
        // Get original values
        ClusterValue<KeyValuePair<string, string>[]> original = await client.ConfigGetAsync(
            [(ValkeyValue)"lfu-decay-time", (ValkeyValue)"lfu-log-factor"],
            Route.AllPrimaries);

        // Get original values from first node
        KeyValuePair<string, string>[] firstNodeOriginal = original.HasMultiData
            ? original.MultiValue.Values.First()
            : original.SingleValue;

        string originalDecayTime = firstNodeOriginal.First(kvp => kvp.Key == "lfu-decay-time").Value;
        string originalLogFactor = firstNodeOriginal.First(kvp => kvp.Key == "lfu-log-factor").Value;

        try
        {
            // Set multiple parameters at once
            await client.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", "5" },
                { "lfu-log-factor", "20" }
            });

            // Verify the values were set
            ClusterValue<KeyValuePair<string, string>[]> result = await client.ConfigGetAsync(
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
            // Restore original values
            await client.ConfigSetAsync(new Dictionary<ValkeyValue, ValkeyValue>
            {
                { "lfu-decay-time", originalDecayTime },
                { "lfu-log-factor", originalLogFactor }
            });
        }
    }

    #endregion

    #region FlushAllDatabases Cluster Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_Cluster_ClearsAllDatabases(GlideClusterClient client)
    {
        string key = $"flushall-cluster-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync();

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_Cluster_WithSyncMode(GlideClusterClient client)
    {
        string key = $"flushall-cluster-sync-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(FlushMode.Sync);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_Cluster_WithAsyncMode(GlideClusterClient client)
    {
        string key = $"flushall-cluster-async-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(FlushMode.Async);

        // Async flush returns immediately; poll until the database is empty.
        await WaitForEmptyDatabaseAsync(client.DatabaseSizeAsync);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_Cluster_WithRoute(GlideClusterClient client)
    {
        string key = $"flushall-cluster-route-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(Route.AllPrimaries);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    #endregion

    #region WAITAOF Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task WaitAofAsync_Standalone_ReturnsResults(GlideClient client)
    {
        string key = $"waitaof-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");

        long[] result = await client.WaitAofAsync(false, 0, TimeSpan.FromSeconds(2));

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        // result[0] = local AOF count, result[1] = replica AOF count
        // With no replicas, we expect [0 or 1, 0]
        Assert.True(result[0] >= 0);
        Assert.True(result[1] >= 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task WaitAofAsync_Cluster_ReturnsResults(GlideClusterClient client)
    {
        string key = $"waitaof-cluster-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");

        long[] result = await client.WaitAofAsync(false, 0, TimeSpan.FromSeconds(2));

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.True(result[0] >= 0);
        Assert.True(result[1] >= 0);
    }

    #endregion

    #region LOLWUT Params-Only Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task LolwutAsync_Standalone_WithParametersOnly(GlideClient client)
    {
        string result = await client.LolwutAsync(new LolwutOptions { Parameters = [40, 20] });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task LolwutAsync_Cluster_WithParametersOnly(GlideClusterClient client)
    {
        string result = await client.LolwutAsync(new LolwutOptions { Parameters = [40, 20] });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}
