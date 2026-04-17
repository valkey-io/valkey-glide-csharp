// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER-compatible integration tests for server management commands.
/// </summary>
public class ServerManagementTests(SERServerManagementFixture fixture) : IClassFixture<SERServerManagementFixture>
{
    #region Non-Destructive IServer Tests (shared server)

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

    #endregion

    #region Destructive IServer Tests (isolated fixture)

    [Fact]
    public async Task IServer_FlushDatabaseAsync_Succeeds()
    {
        await fixture.Server.FlushDatabaseAsync();
    }

    [Fact]
    public async Task IServer_FlushAllDatabasesAsync_Succeeds()
    {
        await fixture.Server.FlushAllDatabasesAsync();
    }

    #endregion
}

/// <summary>
/// Fixture that provides an isolated Valkey server for SER server management flush tests.
/// </summary>
public class SERServerManagementFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer = new();

    public IServer Server { get; }

    public SERServerManagementFixture()
    {
        ConfigurationOptions config = new();
        config.EndPoints.Add(_standaloneServer.Addresses.First().Host, _standaloneServer.Addresses.First().Port);
        ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(config);
        Server = conn.GetServers().First();
    }

    public void Dispose()
    {
        _standaloneServer.Dispose();
    }
}
