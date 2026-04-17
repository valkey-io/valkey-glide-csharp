// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER-compatible integration tests for server management commands.
/// Tests the IServer interface for CONFIG, LOLWUT, TIME, LASTSAVE,
/// FLUSHDB, and FLUSHALL operations.
/// </summary>
public class ServerManagementTests
{
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
    public async Task IServer_ConfigResetStatisticsAsync_Succeeds(IServer server)
    {
        await server.ConfigResetStatisticsAsync();
    }

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
    public async Task IServer_TimeAsync_ReturnsValidTime(IServer server)
    {
        DateTime result = await server.TimeAsync();

        Assert.True(result > DateTime.UnixEpoch);
        Assert.True(result <= DateTime.UtcNow.AddMinutes(1));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_LastSaveAsync_ReturnsValidTime(IServer server)
    {
        DateTime result = await server.LastSaveAsync();

        Assert.True(result >= DateTime.UnixEpoch);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_FlushDatabaseAsync_Succeeds(IServer server)
    {
        await server.FlushDatabaseAsync();
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_FlushAllDatabasesAsync_Succeeds(IServer server)
    {
        await server.FlushAllDatabasesAsync();
    }

    #endregion
}
