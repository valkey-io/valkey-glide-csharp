// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for <see cref="ValkeyServer" /> (IServer) methods.
/// </summary>
public class ValkeyServerTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion
    #region Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task KeysAsync_ReturnsMatchingKeys(ConnectionMultiplexer conn)
    {
        string prefix = Guid.NewGuid().ToString();
        string key1 = $"{prefix}:key1";
        string key2 = $"{prefix}:key2";
        string key3 = $"{prefix}:key3";
        string otherKey = "other:key";

        var server = GetServer(conn);
        var db = conn.GetDatabase();

        // Set up test keys
        _ = await db.StringSetAsync(key1, "value1");
        _ = await db.StringSetAsync(key2, "value2");
        _ = await db.StringSetAsync(key3, "value3");
        _ = await db.StringSetAsync(otherKey, "other");

        // Test scanning all keys with pattern
        List<ValkeyKey> keys = [];
        await foreach (ValkeyKey key in server.KeysAsync(pattern: $"{prefix}:*"))
        {
            keys.Add(key);
        }

        Assert.Equal(3, keys.Count);
        Assert.Contains(key1, keys.Select(k => k.ToString()));
        Assert.Contains(key2, keys.Select(k => k.ToString()));
        Assert.Contains(key3, keys.Select(k => k.ToString()));
        Assert.DoesNotContain(otherKey, keys.Select(k => k.ToString()));

        // Test scanning with pageSize
        keys.Clear();
        await foreach (ValkeyKey key in server.KeysAsync(pattern: $"{prefix}:*", pageSize: 1))
        {
            keys.Add(key);
        }
        Assert.Equal(3, keys.Count);

        // Test scanning with pageOffset
        keys.Clear();
        await foreach (ValkeyKey key in server.KeysAsync(pattern: $"{prefix}:*", pageOffset: 1))
        {
            keys.Add(key);
        }
        Assert.True(keys.Count >= 2);

        // Test scanning non-existent pattern
        keys.Clear();
        await foreach (ValkeyKey key in server.KeysAsync(pattern: "nonexistent:*"))
        {
            keys.Add(key);
        }
        Assert.Empty(keys);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task KeysAsync_CommandFlags_Throws(ConnectionMultiplexer conn)
        => await Assert.ThrowsAsync<NotImplementedException>(
            () => GetServer(conn).KeysAsync(flags: UnsupportedCommandFlag).GetAsyncEnumerator(TestContext.Current.CancellationToken).MoveNextAsync().AsTask());

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task DatabaseSizeAsync_ReturnsSize(ConnectionMultiplexer conn)
    {
        string key = $"server-dbsize-test-{Guid.NewGuid()}";
        var server = GetServer(conn);
        var db = conn.GetDatabase();

        try
        {
            long initialSize = await server.DatabaseSizeAsync();
            Assert.True(initialSize >= 0);

            _ = await db.StringSetAsync(key, "test-value");

            long newSize = await server.DatabaseSizeAsync();
            Assert.True(newSize > initialSize);
        }
        finally
        {
            _ = await db.KeyDeleteAsync(key);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task DatabaseSizeAsync_CommandFlags_Throws(ConnectionMultiplexer conn)
        => await Assert.ThrowsAsync<NotImplementedException>(
            () => GetServer(conn).DatabaseSizeAsync(flags: UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task DatabaseSizeAsync_NonDefaultDatabase_Throws(ConnectionMultiplexer conn)
        => await Assert.ThrowsAsync<ArgumentException>(
            () => GetServer(conn).DatabaseSizeAsync(database: 0));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_ClearsDatabase(ConnectionMultiplexer conn)
    {
        string key = $"server-flush-test-{Guid.NewGuid()}";
        var server = GetServer(conn);
        var db = conn.GetDatabase();

        _ = await db.StringSetAsync(key, "test-value");
        Assert.True(await db.KeyExistsAsync(key));

        await server.FlushDatabaseAsync();

        Assert.False(await db.KeyExistsAsync(key));

        long size = await server.DatabaseSizeAsync();
        Assert.True(size <= 1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_CommandFlags_Throws(ConnectionMultiplexer conn)
        => await Assert.ThrowsAsync<NotImplementedException>(
            () => GetServer(conn).FlushDatabaseAsync(flags: UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task FlushDatabaseAsync_NonDefaultDatabase_Throws(ConnectionMultiplexer conn)
        => await Assert.ThrowsAsync<ArgumentException>(
            () => GetServer(conn).FlushDatabaseAsync(database: 0));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_ClearsAllDatabases(ConnectionMultiplexer conn)
    {
        string key = $"server-flushall-test-{Guid.NewGuid()}";
        var server = GetServer(conn);
        var db = conn.GetDatabase();

        _ = await db.StringSetAsync(key, "test-value");
        Assert.True(await db.KeyExistsAsync(key));

        await server.FlushAllDatabasesAsync();

        Assert.False(await db.KeyExistsAsync(key));

        long size = await server.DatabaseSizeAsync();
        Assert.True(size <= 1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestStandaloneConnections), MemberType = typeof(TestConfiguration))]
    public async Task FlushAllDatabasesAsync_CommandFlags_Throws(ConnectionMultiplexer conn)
        => await Assert.ThrowsAsync<NotImplementedException>(
            () => GetServer(conn).FlushAllDatabasesAsync(flags: UnsupportedCommandFlag));

    #endregion
    #region Helpers

    private static IServer GetServer(ConnectionMultiplexer conn)
        => conn.GetServer(conn.GetEndPoints(true)[0]);

    #endregion
}
