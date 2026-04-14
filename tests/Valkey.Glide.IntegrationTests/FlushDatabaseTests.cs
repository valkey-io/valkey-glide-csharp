// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for flush database commands.
/// </summary>
public class FlushDatabaseTests(FlushDatabaseFixture fixture) : IClassFixture<FlushDatabaseFixture>
{
    [Fact]
    public async Task FlushDatabaseAsync_Standalone_ClearsDatabase()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string key = $"flush-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");

        Assert.True(await client.ExistsAsync(key));
        Assert.Equal(1, await client.DatabaseSizeAsync());

        await client.FlushDatabaseAsync();

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Standalone_ClearsAllDatabases()
    {
        using GlideClient client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        string key = $"flushall-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");

        Assert.True(await client.ExistsAsync(key));
        Assert.Equal(1, await client.DatabaseSizeAsync());

        await client.FlushAllDatabasesAsync();

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_ClearsDatabase()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flush-cluster-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");

        Assert.True(await client.ExistsAsync(key));
        Assert.Equal(1, await client.DatabaseSizeAsync());

        await client.FlushDatabaseAsync();

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithRoute_ClearsDatabase()
    {
        using GlideClusterClient client = await fixture.ClusterServer.CreateClusterClientAsync();

        string key = $"flush-cluster-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");

        Assert.True(await client.ExistsAsync(key));
        Assert.Equal(1, await client.DatabaseSizeAsync());

        await client.FlushDatabaseAsync(Route.AllPrimaries);

        Assert.False(await client.ExistsAsync(key));
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }
}

/// <summary>
/// Fixture class for flush database tests.
/// </summary>
public class FlushDatabaseFixture : IDisposable
{
    public StandaloneServer StandaloneServer = new();
    public ClusterServer ClusterServer = new();

    public void Dispose()
    {
        StandaloneServer.Dispose();
        ClusterServer.Dispose();
    }
}
