// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER-compatible integration tests for server management commands.
/// Tests the IServer and IDatabaseAsync interfaces for CONFIG, LOLWUT, TIME, LASTSAVE,
/// FLUSHDB, and FLUSHALL operations.
/// </summary>
public class ServerManagementTests
{
    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion

    #region IServer Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigGetAsync_ReturnsResults(IServer server)
    {
        KeyValuePair<string, string>[] result = await server.ConfigGetAsync("maxmemory");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigGetAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.ConfigGetAsync(flags: UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigSetAsync_SetsValue(IServer server)
    {
        KeyValuePair<string, string>[] original = await server.ConfigGetAsync("lfu-decay-time");
        string originalValue = original.First(kvp => kvp.Key == "lfu-decay-time").Value;

        try
        {
            await server.ConfigSetAsync("lfu-decay-time", "5");

            KeyValuePair<string, string>[] result = await server.ConfigGetAsync("lfu-decay-time");
            Assert.Equal("5", result.First(kvp => kvp.Key == "lfu-decay-time").Value);
        }
        finally
        {
            await server.ConfigSetAsync("lfu-decay-time", originalValue);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigSetAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.ConfigSetAsync("lfu-decay-time", "1", UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigResetStatisticsAsync_Succeeds(IServer server)
    {
        await server.ConfigResetStatisticsAsync();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigResetStatisticsAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.ConfigResetStatisticsAsync(UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_LolwutAsync_ReturnsArt(IServer server)
    {
        string result = await server.LolwutAsync();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_LolwutAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.LolwutAsync(UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_TimeAsync_ReturnsValidTime(IServer server)
    {
        DateTime result = await server.TimeAsync();

        Assert.True(result > DateTime.UnixEpoch);
        Assert.True(result <= DateTime.UtcNow.AddMinutes(1));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_TimeAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.TimeAsync(UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_LastSaveAsync_ReturnsValidTime(IServer server)
    {
        DateTime result = await server.LastSaveAsync();

        Assert.True(result >= DateTime.UnixEpoch);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_LastSaveAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.LastSaveAsync(UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_FlushDatabaseAsync_Succeeds(IServer server)
    {
        await server.FlushDatabaseAsync();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_FlushDatabaseAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.FlushDatabaseAsync(UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_FlushAllDatabasesAsync_Succeeds(IServer server)
    {
        await server.FlushAllDatabasesAsync();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_FlushAllDatabasesAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.FlushAllDatabasesAsync(UnsupportedCommandFlag));

    #endregion

    #region IDatabaseAsync Server Management Tests

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_ConfigGetAsync_WithFlags(IDatabaseAsync db)
    {
        KeyValuePair<string, string>[] result = await db.ConfigGetAsync("maxmemory", CommandFlags.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, kvp => kvp.Key == "maxmemory");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_ConfigGetAsync_UnsupportedFlags_Throws(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ConfigGetAsync("maxmemory", UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_ConfigSetAsync_WithFlags(IDatabaseAsync db)
    {
        KeyValuePair<string, string>[] original = await db.ConfigGetAsync("lfu-decay-time", CommandFlags.None);
        string originalValue = original.First(kvp => kvp.Key == "lfu-decay-time").Value;

        try
        {
            await db.ConfigSetAsync("lfu-decay-time", "7", CommandFlags.None);

            KeyValuePair<string, string>[] result = await db.ConfigGetAsync("lfu-decay-time", CommandFlags.None);
            Assert.Equal("7", result.First(kvp => kvp.Key == "lfu-decay-time").Value);
        }
        finally
        {
            await db.ConfigSetAsync("lfu-decay-time", originalValue, CommandFlags.None);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_ConfigSetAsync_UnsupportedFlags_Throws(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ConfigSetAsync("lfu-decay-time", "1", UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_LolwutAsync_WithFlags(IDatabaseAsync db)
    {
        string result = await db.LolwutAsync(CommandFlags.None);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("Valkey", result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_LolwutAsync_UnsupportedFlags_Throws(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.LolwutAsync(UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_TimeAsync_WithFlags(IDatabaseAsync db)
    {
        DateTime result = await db.TimeAsync(CommandFlags.None);

        Assert.True(result > DateTime.UnixEpoch);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_TimeAsync_UnsupportedFlags_Throws(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.TimeAsync(UnsupportedCommandFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_LastSaveAsync_WithFlags(IDatabaseAsync db)
    {
        DateTime result = await db.LastSaveAsync(CommandFlags.None);

        Assert.True(result >= DateTime.UnixEpoch);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task IDatabaseAsync_LastSaveAsync_UnsupportedFlags_Throws(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.LastSaveAsync(UnsupportedCommandFlag));

    #endregion
}
