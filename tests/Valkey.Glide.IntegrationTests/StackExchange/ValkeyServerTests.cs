// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.TestUtils.Data;

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
        var server = fixture.Server;
        var db = fixture.Database;

        var size = await server.DatabaseSizeAsync();

        var key = $"server-dbsize-test-{Guid.NewGuid()}";
        _ = await db.StringSetAsync(key, "test-value");
        Assert.Equal(size + 1, await server.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushAllDatabasesAsync_ClearsAllDatabases()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        var key = $"server-flushall-test-{Guid.NewGuid()}";
        _ = await db.StringSetAsync(key, "test-value");
        Assert.True(await server.DatabaseSizeAsync() > 0);

        await server.FlushAllDatabasesAsync();
        Assert.Equal(0, await server.DatabaseSizeAsync());
    }

    [Fact]
    public async Task FlushDatabaseAsync_ClearsDatabase()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        var key = $"server-flush-test-{Guid.NewGuid()}";
        _ = await db.StringSetAsync(key, "test-value");
        Assert.True(await server.DatabaseSizeAsync() > 0);

        await server.FlushDatabaseAsync();
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

        // Verify script can be loaded.
        var script = LuaScript.Prepare("return redis.call('SET', @key, @value)");
        var loaded = await server.ScriptLoadAsync(script);
        Assert.NotEmpty(loaded.Hash);

        // Verify the script can be executed.
        var parameters = new { key = BuildKey(), value = BuildValue() };
        Assert.Equal("OK", (ValkeyValue)await db.ScriptEvaluateAsync(loaded, parameters));
        Assert.Equal(parameters.value, await db.StringGetAsync(parameters.key));
    }

    [Fact]
    public async Task ScriptLoadAsync_WithString_ReturnsHash()
    {
        var server = fixture.Server;
        var db = fixture.Database;

        // Verify script can be loaded.
        var hash = await server.ScriptLoadAsync("return redis.call('SET', KEYS[1], ARGV[1])");
        Assert.NotEmpty(hash);

        // Verify the script can be executed.
        var key = BuildKey();
        var value = BuildValue();
        Assert.Equal("OK", (ValkeyValue)await db.ScriptEvaluateAsync(hash, [key], [value]));
        Assert.Equal(value, await db.StringGetAsync(key));
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
