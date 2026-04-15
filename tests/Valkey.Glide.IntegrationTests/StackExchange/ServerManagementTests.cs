// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER-compatible integration tests for server management commands.
/// Tests the IServer and IDatabaseAsync interfaces for FlushMode, LOLWUT, and CONFIG operations.
/// </summary>
public class ServerManagementTests(ServerManagementSERFixture fixture) : IClassFixture<ServerManagementSERFixture>
{
    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion

    #region IServer Tests

    [Fact]
    public async Task IServer_ConfigGetAsync_ReturnsResults()
    {
        KeyValuePair<string, string>[] result = await fixture.Server.ConfigGetAsync("maxmemory");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
    }

    [Fact]
    public async Task IServer_ConfigSetAsync_SetsValue()
    {
        // Get original value
        KeyValuePair<string, string>[] original = await fixture.Server.ConfigGetAsync("lfu-decay-time");
        string originalValue = original.First(kvp => kvp.Key == "lfu-decay-time").Value;

        try
        {
            await fixture.Server.ConfigSetAsync("lfu-decay-time", "5");

            KeyValuePair<string, string>[] result = await fixture.Server.ConfigGetAsync("lfu-decay-time");
            Assert.Equal("5", result.First(kvp => kvp.Key == "lfu-decay-time").Value);
        }
        finally
        {
            await fixture.Server.ConfigSetAsync("lfu-decay-time", originalValue);
        }
    }

    [Fact]
    public async Task IServer_ConfigResetStatisticsAsync_Succeeds()
    {
        await fixture.Server.ConfigResetStatisticsAsync();
        // No exception means success
    }

    [Fact]
    public async Task IServer_LolwutAsync_ReturnsArt()
    {
        string result = await fixture.Server.LolwutAsync();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IServer_TimeAsync_ReturnsValidTime()
    {
        DateTime result = await fixture.Server.TimeAsync();

        Assert.True(result > DateTime.UnixEpoch);
        Assert.True(result <= DateTime.UtcNow.AddMinutes(1));
    }

    [Fact]
    public async Task IServer_LastSaveAsync_ReturnsValidTime()
    {
        DateTime result = await fixture.Server.LastSaveAsync();

        // LastSave returns the Unix timestamp of the last save; it should be a valid date
        Assert.True(result >= DateTime.UnixEpoch);
    }

    [Fact]
    public async Task IServer_FlushDatabaseAsync_ClearsDatabase()
    {
        string key = $"ser-server-flush-{Guid.NewGuid()}";
        await fixture.Database.StringSetAsync(key, "test-value");
        Assert.True(await fixture.Database.KeyExistsAsync(key));

        await fixture.Server.FlushDatabaseAsync();

        Assert.False(await fixture.Database.KeyExistsAsync(key));
    }

    [Fact]
    public async Task IServer_FlushAllDatabasesAsync_ClearsAllDatabases()
    {
        string key = $"ser-server-flushall-{Guid.NewGuid()}";
        await fixture.Database.StringSetAsync(key, "test-value");
        Assert.True(await fixture.Database.KeyExistsAsync(key));

        await fixture.Server.FlushAllDatabasesAsync();

        Assert.False(await fixture.Database.KeyExistsAsync(key));
    }

    #endregion

    #region IDatabaseAsync Server Management Tests

    [Fact]
    public async Task IDatabaseAsync_ConfigGetAsync_WithFlags()
    {
        KeyValuePair<string, string>[] result = await fixture.Database.ConfigGetAsync("maxmemory", CommandFlags.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
    }

    [Fact]
    public async Task IDatabaseAsync_ConfigGetAsync_UnsupportedFlags_Throws()
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.ConfigGetAsync("maxmemory", UnsupportedCommandFlag));
    }

    [Fact]
    public async Task IDatabaseAsync_ConfigSetAsync_WithFlags()
    {
        // Get original value
        KeyValuePair<string, string>[] original = await fixture.Database.ConfigGetAsync("lfu-decay-time", CommandFlags.None);
        string originalValue = original.First(kvp => kvp.Key == "lfu-decay-time").Value;

        try
        {
            await fixture.Database.ConfigSetAsync("lfu-decay-time", "7", CommandFlags.None);

            KeyValuePair<string, string>[] result = await fixture.Database.ConfigGetAsync("lfu-decay-time", CommandFlags.None);
            Assert.Equal("7", result.First(kvp => kvp.Key == "lfu-decay-time").Value);
        }
        finally
        {
            await fixture.Database.ConfigSetAsync("lfu-decay-time", originalValue, CommandFlags.None);
        }
    }

    [Fact]
    public async Task IDatabaseAsync_ConfigSetAsync_UnsupportedFlags_Throws()
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.ConfigSetAsync("lfu-decay-time", "1", UnsupportedCommandFlag));
    }

    [Fact]
    public async Task IDatabaseAsync_LolwutAsync_WithFlags()
    {
        string result = await fixture.Database.LolwutAsync(CommandFlags.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IDatabaseAsync_LolwutAsync_UnsupportedFlags_Throws()
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.LolwutAsync(UnsupportedCommandFlag));
    }

    [Fact]
    public async Task IDatabaseAsync_TimeAsync_WithFlags()
    {
        DateTime result = await fixture.Database.TimeAsync(CommandFlags.None);

        Assert.True(result > DateTime.UnixEpoch);
    }

    [Fact]
    public async Task IDatabaseAsync_TimeAsync_UnsupportedFlags_Throws()
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.TimeAsync(UnsupportedCommandFlag));
    }

    [Fact]
    public async Task IDatabaseAsync_LastSaveAsync_WithFlags()
    {
        DateTime result = await fixture.Database.LastSaveAsync(CommandFlags.None);

        Assert.True(result >= DateTime.UnixEpoch);
    }

    [Fact]
    public async Task IDatabaseAsync_LastSaveAsync_UnsupportedFlags_Throws()
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => fixture.Database.LastSaveAsync(UnsupportedCommandFlag));
    }

    #endregion
}

/// <summary>
/// Fixture class for SER-compatible server management tests.
/// </summary>
public class ServerManagementSERFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer;
    private readonly ConnectionMultiplexer _connection;

    public IServer Server { get; }
    public IDatabaseAsync Database { get; }

    public ServerManagementSERFixture()
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
