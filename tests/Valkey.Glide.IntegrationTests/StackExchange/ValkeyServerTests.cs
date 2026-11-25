// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="ValkeyServer" />.
/// </summary>
public class ValkeyServerTests(ValkeyServerFixture fixture) : IClassFixture<ValkeyServerFixture>
{
    #region Tests

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
    public async Task ScriptExistsAsync_WithByteArray_ReturnsCorrectStatus()
    {
        var server = fixture.Server;
        var db = fixture.Database;
        string script = "return 'hash exists test'";
        using var scriptObj = new Script(script);
        byte[] hash = Convert.FromHexString(scriptObj.Hash);

        // Script should not exist initially
        await server.ScriptFlushAsync();
        bool existsBefore = await server.ScriptExistsAsync(hash);
        Assert.False(existsBefore);

        // Load the script
        _ = await db.ScriptEvaluateAsync(script);

        // Script should exist now
        bool existsAfter = await server.ScriptExistsAsync(hash);
        Assert.True(existsAfter);
    }

    [Fact]
    public async Task ScriptExistsAsync_WithString_ReturnsCorrectStatus()
    {
        var server = fixture.Server;
        var db = fixture.Database;
        string script = "return 'exists test'";

        // Script should not exist initially
        await server.ScriptFlushAsync();
        bool existsBefore = await server.ScriptExistsAsync(script);
        Assert.False(existsBefore);

        // Load the script
        _ = await db.ScriptEvaluateAsync(script);

        // Script should exist now
        bool existsAfter = await server.ScriptExistsAsync(script);
        Assert.True(existsAfter);
    }

    [Fact]
    public async Task ScriptFlushAsync_RemovesAllScripts()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        // Load a script
        string script = "return 'flush test'";
        _ = await db.ScriptEvaluateAsync(script);

        // Verify it exists
        bool existsBefore = await server.ScriptExistsAsync(script);
        Assert.True(existsBefore);

        await server.ScriptFlushAsync();

        // Verify it no longer exists
        bool existsAfter = await server.ScriptExistsAsync(script);
        Assert.False(existsAfter);
    }

    [Fact]
    public async Task ScriptLoadAsync_WithLuaScript_ReturnsLoadedLuaScript()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        LoadedLuaScript loaded = await server.ScriptLoadAsync(LuaScript.Prepare("return redis.call('PING')"));

        Assert.NotNull(loaded);
        Assert.NotNull(loaded.Hash);
        Assert.NotEmpty(loaded.Hash);

        // Verify the script can be executed
        ValkeyResult result = await db.ScriptEvaluateAsync(loaded);
        Assert.Equal("PONG", result.ToString());
    }

    [Fact]
    public async Task ScriptLoadAsync_WithString_ReturnsHash()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        byte[] hash = await server.ScriptLoadAsync("return 'load test'");

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);

        // Verify the script can be executed with EVALSHA
        ValkeyResult result = await db.ScriptEvaluateAsync(hash);
        Assert.Equal("load test", result.ToString());
    }

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
