// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER-compatible integration tests for server management commands.
/// </summary>
[Collection(typeof(ServerManagementTests))]
[CollectionDefinition(DisableParallelization = true)]
public class ServerManagementTests(ServerManagementFixture fixture) : IClassFixture<ServerManagementFixture>
{
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
        => await fixture.Server.ConfigResetStatisticsAsync();

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

        Assert.True(result >= DateTime.UnixEpoch);
    }

    [Fact]
    public async Task IServer_FlushDatabaseAsync_Succeeds()
        => await fixture.Server.FlushDatabaseAsync();

    [Fact]
    public async Task IServer_FlushAllDatabasesAsync_Succeeds()
        => await fixture.Server.FlushAllDatabasesAsync();

    #endregion
}

/// <summary>
/// Fixture that provides an isolated Valkey server for SER server management tests.
/// </summary>
public class ServerManagementFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer = new();

    public IServer Server { get; }

    public ServerManagementFixture()
    {
        ConfigurationOptions config = new();
        (string host, ushort port) = _standaloneServer.Addresses.First();
        config.EndPoints.Add(host, port);
        ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(config);
        Server = conn.GetServers().First();
    }

    public void Dispose() => _standaloneServer.Dispose();
}
