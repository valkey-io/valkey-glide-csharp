// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests;

/// <summary>
/// Tests for <see cref="ValkeyServer" />.
/// </summary>
public class ValkeyServerTests(ValkeyServerFixture fixture) : IClassFixture<ValkeyServerFixture>
{
    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion
    #region Tests

    [Fact]
    public async Task KeysAsync_ReturnsMatchingKeys()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        string prefix = Guid.NewGuid().ToString();
        string key1 = $"{prefix}:key1";
        string key2 = $"{prefix}:key2";
        string key3 = $"{prefix}:key3";
        string otherKey = "other:key";

        await db.StringSetAsync(key1, "value1");
        await db.StringSetAsync(key2, "value2");
        await db.StringSetAsync(key3, "value3");
        await db.StringSetAsync(otherKey, "other");

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

        // Clear database.
        await db.FlushDatabaseAsync();
    }

    [Fact]
    public async Task KeysAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.KeysAsync(flags: UnsupportedCommandFlag)
                .GetAsyncEnumerator(TestContext.Current.CancellationToken)
                .MoveNextAsync().AsTask());

    [Fact]
    public async Task DatabaseSizeAsync_ReturnsSize()
    {
        string key = $"server-dbsize-test-{Guid.NewGuid()}";

        var server = fixture.Server;
        var db = fixture.Database;

        Assert.Equal(0, await server.DatabaseSizeAsync());

        await db.StringSetAsync(key, "test-value");
        Assert.Equal(1, await server.DatabaseSizeAsync());

        _ = await db.KeyDeleteAsync(key);
        Assert.Equal(0, await server.DatabaseSizeAsync());
    }

    [Fact]
    public async Task DatabaseSizeAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.DatabaseSizeAsync(flags: UnsupportedCommandFlag));

    [Fact]
    public async Task DatabaseSizeAsync_NonDefaultDatabase_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.DatabaseSizeAsync(database: 0));

    [Fact]
    public async Task FlushDatabaseAsync_ClearsDatabase()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        string key = $"server-flush-test-{Guid.NewGuid()}";
        await db.StringSetAsync(key, "test-value");

        Assert.True(await db.KeyExistsAsync(key));
        Assert.Equal(1, await server.DatabaseSizeAsync());

        await server.FlushDatabaseAsync();

        Assert.False(await db.KeyExistsAsync(key));
        Assert.Equal(0, await server.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.FlushDatabaseAsync(flags: UnsupportedCommandFlag));

    [Fact]
    public async Task FlushDatabaseAsync_NonDefaultDatabase_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.FlushDatabaseAsync(database: 0));

    [Fact]
    public async Task FlushAllDatabasesAsync_ClearsAllDatabases()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        string key = $"server-flushall-test-{Guid.NewGuid()}";
        await db.StringSetAsync(key, "test-value");

        Assert.True(await db.KeyExistsAsync(key));
        Assert.Equal(1, await server.DatabaseSizeAsync());

        await server.FlushAllDatabasesAsync();

        Assert.False(await db.KeyExistsAsync(key));
        Assert.Equal(0, await server.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.FlushAllDatabasesAsync(flags: UnsupportedCommandFlag));

    #endregion
}

/// <summary>
/// Fixture class for <see cref="ValkeyServerTests" />.
/// </summary>
public class ValkeyServerFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer;
    private readonly ConnectionMultiplexer _connection;

    public IServer Server { get; }
    public IDatabase Database { get; }

    public ValkeyServerFixture()
    {
        _standaloneServer = new();
        var (host, port) = _standaloneServer.Addresses.First();

        ConfigurationOptions config = new();
        config.EndPoints.Add(host, port);
        _connection = ConnectionMultiplexer.Connect(config);

        Server = _connection.GetServer(host, port);
        Database = _connection.GetDatabase();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _standaloneServer.Dispose();
    }
}
