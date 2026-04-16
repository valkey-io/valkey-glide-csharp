// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for server management commands: FlushMode, LOLWUT version/parameters, CONFIG GET/SET multi-parameter.
/// </summary>
public class ServerManagementCommandTests(ServerManagementFixture fixture) : IClassFixture<ServerManagementFixture>
{
    #region FlushMode Tests

    [Fact]
    public async Task FlushDatabaseAsync_Standalone_WithSyncMode()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string key = $"flush-sync-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Sync);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Standalone_WithAsyncMode()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string key = $"flush-async-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Async);

        // Async flush may not be immediate, but the command should succeed
        // Wait briefly for async flush to complete
        await Task.Delay(500);
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Standalone_WithSyncMode()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string key = $"flushall-sync-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(FlushMode.Sync);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Standalone_WithAsyncMode()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string key = $"flushall-async-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(FlushMode.Async);

        // Wait briefly for async flush to complete
        await Task.Delay(500);
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithSyncMode()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flush-cluster-sync-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Sync);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithAsyncMode()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flush-cluster-async-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Async);

        // Wait briefly for async flush to complete
        await Task.Delay(500);
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithSyncMode_AndRoute()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flush-cluster-sync-route-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushDatabaseAsync(FlushMode.Sync, Route.AllPrimaries);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    #endregion

    #region LOLWUT Tests

    [Fact]
    public async Task LolwutAsync_Standalone_WithVersion()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string result = await client.LolwutAsync(new LolwutOptions { Version = 5 });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LolwutAsync_Standalone_WithVersionAndParameters()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string result = await client.LolwutAsync(new LolwutOptions { Version = 5, Parameters = [40, 20] });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LolwutAsync_Cluster_WithVersion()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        Dictionary<string, string> result = await client.LolwutAsync(new LolwutOptions { Version = 5 });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        foreach (string value in result.Values)
        {
            Assert.Contains("Valkey", value, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task LolwutAsync_Cluster_WithVersionAndParameters()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        Dictionary<string, string> result = await client.LolwutAsync(new LolwutOptions { Version = 5, Parameters = [40, 20] });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        foreach (string value in result.Values)
        {
            Assert.Contains("Valkey", value, StringComparison.OrdinalIgnoreCase);
        }
    }

    #endregion

    #region CONFIG GET/SET Multi-Parameter Tests

    [Fact]
    public async Task ConfigGetAsync_Standalone_MultiplePatterns()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        KeyValuePair<string, string>[] result = await client.ConfigGetAsync(
            [(ValkeyValue)"maxmemory", (ValkeyValue)"lfu-decay-time"]);

        Assert.NotNull(result);
        Assert.True(result.Length >= 2);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
        Assert.Contains(result, kvp => kvp.Key == "lfu-decay-time");
    }

    [Fact]
    public async Task ConfigSetAsync_Standalone_MultipleParameters()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

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

    [Fact]
    public async Task ConfigGetAsync_Cluster_MultiplePatterns()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        ClusterValue<KeyValuePair<string, string>[]> result = await client.ConfigGetAsync(
            [(ValkeyValue)"maxmemory", (ValkeyValue)"lfu-decay-time"]);

        Assert.NotNull(result);
        if (result.HasMultiData)
        {
            foreach (KeyValuePair<string, string>[] nodeResult in result.MultiValue.Values)
            {
                Assert.True(nodeResult.Length >= 2);
                Assert.Contains(nodeResult, kvp => kvp.Key == "maxmemory");
                Assert.Contains(nodeResult, kvp => kvp.Key == "lfu-decay-time");
            }
        }
        else
        {
            Assert.True(result.SingleValue.Length >= 2);
            Assert.Contains(result.SingleValue, kvp => kvp.Key == "maxmemory");
            Assert.Contains(result.SingleValue, kvp => kvp.Key == "lfu-decay-time");
        }
    }

    [Fact]
    public async Task ConfigSetAsync_Cluster_MultipleParameters()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

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

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_ClearsAllDatabases()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flushall-cluster-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync();

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_WithSyncMode()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flushall-cluster-sync-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(FlushMode.Sync);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_WithAsyncMode()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flushall-cluster-async-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(FlushMode.Async);

        // Wait briefly for async flush to complete
        await Task.Delay(500);
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Cluster_WithRoute()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flushall-cluster-route-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.ExistsAsync(key));

        await client.FlushAllDatabasesAsync(Route.AllPrimaries);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    #endregion

    #region WAITAOF Tests

    [Fact]
    public async Task WaitAofAsync_Standalone_ReturnsResults()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string key = $"waitaof-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");

        long[] result = await client.WaitAofAsync(0, 0, TimeSpan.FromSeconds(2));

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        // result[0] = local AOF count, result[1] = replica AOF count
        // With no replicas, we expect [0 or 1, 0]
        Assert.True(result[0] >= 0);
        Assert.True(result[1] >= 0);
    }

    [Fact]
    public async Task WaitAofAsync_Cluster_ReturnsResults()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"waitaof-cluster-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");

        long[] result = await client.WaitAofAsync(0, 0, TimeSpan.FromSeconds(2));

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.True(result[0] >= 0);
        Assert.True(result[1] >= 0);
    }

    #endregion

    #region LOLWUT Params-Only Tests

    [Fact]
    public async Task LolwutAsync_Standalone_WithParametersOnly()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string result = await client.LolwutAsync(new LolwutOptions { Parameters = [40, 20] });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LolwutAsync_Cluster_WithParametersOnly()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        Dictionary<string, string> result = await client.LolwutAsync(new LolwutOptions { Parameters = [40, 20] });

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        foreach (string value in result.Values)
        {
            Assert.Contains("Valkey", value, StringComparison.OrdinalIgnoreCase);
        }
    }

    #endregion
}

/// <summary>
/// Fixture class for server management command tests.
/// </summary>
public class ServerManagementFixture : IDisposable
{
    public StandaloneServer StandaloneServer = new();
    public ClusterServer ClusterServer = new();

    public void Dispose()
    {
        StandaloneServer.Dispose();
        ClusterServer.Dispose();
    }
}
