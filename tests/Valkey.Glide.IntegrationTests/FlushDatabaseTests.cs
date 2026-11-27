// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for flush database commands.
/// </summary>
public class FlushDatabaseTests(ServerFixture fixture) : IClassFixture<ServerFixture>
{
    [Fact]
    public async Task FlushDatabaseAsync_Standalone_ClearsDatabase()
    {
        await using var client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        var key = $"flush-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.DatabaseSizeAsync() > 0);

        await client.FlushDatabaseAsync();
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_Standalone_ClearsAllDatabases()
    {
        await using var client = await fixture.StandaloneServer.CreateStandaloneClientAsync();

        var key = $"flushall-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.DatabaseSizeAsync() > 0);

        await client.FlushAllDatabasesAsync();
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_ClearsDatabase()
    {
        await using var client = await fixture.ClusterServer.CreateClusterClientAsync();

        var key = $"flush-cluster-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.DatabaseSizeAsync() > 0);

        await client.FlushDatabaseAsync(FlushMode.Sync);
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_Cluster_WithRoute_ClearsDatabase()
    {
        await using var client = await fixture.ClusterServer.CreateClusterClientAsync();

        var key = $"flush-cluster-test-{Guid.NewGuid()}";
        await client.SetAsync(key, "test-value");
        Assert.True(await client.DatabaseSizeAsync() > 0);

        await client.FlushDatabaseAsync(FlushMode.Sync, Route.AllPrimaries);
        Assert.Equal(0, await client.DatabaseSizeAsync());
    }
}
