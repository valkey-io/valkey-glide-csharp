// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.StackExchange;

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

        var prefix = $"ser-keys-{Guid.NewGuid()}";
        foreach (var element in new[] { "a", "b", "c", "d", "e" })
        {
            _ = await db.StringSetAsync($"{prefix}:{element}", "value");
        }

        // Test scanning all keys
        var results = new List<ValkeyKey>();
        await foreach (var key in server.KeysAsync(pattern: $"{prefix}:*"))
        {
            results.Add(key);
        }

        Assert.Equal(5, results.Count);
        results.Clear();

        // Test scanning with pageSize
        await foreach (var key in server.KeysAsync(pattern: $"{prefix}:*", pageSize: 1))
        {
            results.Add(key);
        }

        Assert.Equal(5, results.Count);
        results.Clear();

        // Test scanning with pageOffset
        await foreach (var key in server.KeysAsync(pattern: $"{prefix}:*", pageOffset: 3))
        {
            results.Add(key);
        }

        Assert.Equal(2, results.Count);
        results.Clear();

        // Test scanning with pageOffset > pageSize
        await foreach (var key in server.KeysAsync(pattern: $"{prefix}:*", pageSize: 2, pageOffset: 3))
        {
            results.Add(key);
        }

        Assert.Equal(2, results.Count);
        results.Clear();

        // Test scanning non-existent pattern
        await foreach (var key in server.KeysAsync(pattern: "nonexistent:*"))
        {
            results.Add(key);
        }

        Assert.Empty(results);

        await server.FlushDatabaseAsync();
    }

    [Fact]
    public async Task DatabaseSizeAsync_ReturnsSize()
    {
        string key = $"server-dbsize-test-{Guid.NewGuid()}";

        var server = fixture.Server;
        var db = fixture.Database;

        Assert.Equal(0, await server.DatabaseSizeAsync());

        _ = await db.StringSetAsync(key, "test-value");
        Assert.Equal(1, await server.DatabaseSizeAsync());

        _ = await db.KeyDeleteAsync(key);
        Assert.Equal(0, await server.DatabaseSizeAsync());
    }

    [Fact]
    public async Task DatabaseSizeAsync_CommandFlags_Throws()
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Server.DatabaseSizeAsync(flags: UnsupportedCommandFlag));

    [Fact]
    public async Task FlushDatabaseAsync_ClearsDatabase()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        string key = $"server-flush-test-{Guid.NewGuid()}";
        _ = await db.StringSetAsync(key, "test-value");

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
    public async Task FlushAllDatabasesAsync_ClearsAllDatabases()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        string key = $"server-flushall-test-{Guid.NewGuid()}";
        _ = await db.StringSetAsync(key, "test-value");

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
public class ValkeyServerFixture : IAsyncLifetime
{
    private StandaloneServer _standaloneServer = null!;
    private ConnectionMultiplexer _connection = null!;

    public IServer Server { get; private set; } = null!;
    public IDatabase Database { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _standaloneServer = new();
        var (host, port) = _standaloneServer.Address;

        ConfigurationOptions config = new();
        config.EndPoints.Add(host, port);
        _connection = await ConnectionMultiplexer.ConnectAsync(config);

        Server = _connection.GetServer(host, port);
        Database = _connection.GetDatabase();
    }

    public ValueTask DisposeAsync()
    {
        _connection.Dispose();
        _standaloneServer.Dispose();
        return ValueTask.CompletedTask;
    }
}
