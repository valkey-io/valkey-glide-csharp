// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

// Separate collection for flush tests to prevent interference with other tests
[Collection(typeof(FlushDatabaseTests))]
[CollectionDefinition(DisableParallelization = true)]
public class FlushDatabaseTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_ClearsDatabase(GlideClient client)
    {
        string key = $"flush-test-{Guid.NewGuid()}";
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.KeyExistsAsync(key));

        // Flush database
        await client.FlushDatabaseAsync();

        // Key should be gone
        Assert.False(await client.KeyExistsAsync(key));

        // Database size should be 0 or very small
        long size = await client.DatabaseSizeAsync();
        Assert.True(size <= 1); // Allow for some system keys
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_ClearsAllDatabases(GlideClient client)
    {
        string key = $"flushall-test-{Guid.NewGuid()}";

        // Add a key
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.KeyExistsAsync(key));

        // Flush all databases
        await client.FlushAllDatabasesAsync();

        // Key should be gone
        Assert.False(await client.KeyExistsAsync(key));

        // Database size should be 0 or very small
        long size = await client.DatabaseSizeAsync();
        Assert.True(size <= 1); // Allow for some system keys
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClusterClients), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_Cluster_ClearsAllDatabases(GlideClusterClient client)
    {
        string key = $"flushall-cluster-test-{Guid.NewGuid()}";

        // Add a key
        await client.StringSetAsync(key, "test-value");
        Assert.True(await client.KeyExistsAsync(key));

        // Flush all databases on cluster
        await client.FlushAllDatabasesAsync();

        // Key should be gone
        Assert.False(await client.KeyExistsAsync(key));
    }
}
