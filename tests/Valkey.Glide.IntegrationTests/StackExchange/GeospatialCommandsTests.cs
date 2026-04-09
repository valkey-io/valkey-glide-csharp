// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.TestUtils;

using static Valkey.Glide.ConnectionConfiguration;

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER compatibility tests for geospatial commands.
/// Tests verify that GeoEntry-based and GeoRadiusResult-based methods delegate correctly
/// to the GLIDE layer and that CommandFlags validation works correctly.
/// </summary>
public class GeospatialCommandsTests(GeospatialCommandsFixture fixture) : IClassFixture<GeospatialCommandsFixture>
{
    #region Constants

    private const CommandFlags UnsupportedCommandFlag = CommandFlags.DemandMaster;

    #endregion

    #region GeoAddAsync Tests

    [Fact]
    public async Task GeoAddAsync_SingleEntry_WithLonLatMember_ReturnsTrue()
    {
        var db = fixture.Database;
        string key = $"ser-geoadd-{Guid.NewGuid()}";

        bool result = await db.GeoAddAsync(key, 13.361389, 38.115556, "Palermo");
        Assert.True(result);
    }

    [Fact]
    public async Task GeoAddAsync_SingleGeoEntry_ReturnsTrue()
    {
        var db = fixture.Database;
        string key = $"ser-geoadd-{Guid.NewGuid()}";

        bool result = await db.GeoAddAsync(key, new GeoEntry(13.361389, 38.115556, "Palermo"));
        Assert.True(result);
    }

    [Fact]
    public async Task GeoAddAsync_MultipleGeoEntries_ReturnsCount()
    {
        var db = fixture.Database;
        string key = $"ser-geoadd-{Guid.NewGuid()}";

        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
        ];
        long result = await db.GeoAddAsync(key, entries);
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GeoAddAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
    {
        var db = fixture.Database;

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoAddAsync("key", 13.361389, 38.115556, "Palermo", UnsupportedCommandFlag));

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoAddAsync("key", new GeoEntry(13.361389, 38.115556, "Palermo"), UnsupportedCommandFlag));

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoAddAsync("key", [new GeoEntry(13.361389, 38.115556, "Palermo")], UnsupportedCommandFlag));
    }

    #endregion

    #region GeoSearchAsync Tests

    [Fact]
    public async Task GeoSearchAsync_FromMember_ReturnsResults()
    {
        var db = fixture.Database;
        string key = $"ser-geosearch-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(key, [new GeoEntry(13.361389, 38.115556, "Palermo"), new GeoEntry(15.087269, 37.502669, "Catania")]);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await db.GeoSearchAsync(key, "Palermo", shape);

        Assert.NotEmpty(results);
        Assert.Contains("Palermo", results.Select(r => r.Member.ToString()));
    }

    [Fact]
    public async Task GeoSearchAsync_FromPosition_ReturnsResults()
    {
        var db = fixture.Database;
        string key = $"ser-geosearch-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(key, [new GeoEntry(13.361389, 38.115556, "Palermo"), new GeoEntry(15.087269, 37.502669, "Catania")]);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await db.GeoSearchAsync(key, 13.361389, 38.115556, shape);

        Assert.NotEmpty(results);
        Assert.Contains("Palermo", results.Select(r => r.Member.ToString()));
    }

    [Fact]
    public async Task GeoSearchAsync_WithOptions_ReturnsEnrichedResults()
    {
        var db = fixture.Database;
        string key = $"ser-geosearch-opts-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(key, [new GeoEntry(13.361389, 38.115556, "Palermo"), new GeoEntry(15.087269, 37.502669, "Catania")]);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await db.GeoSearchAsync(key, "Palermo", shape, options: GeoRadiusOptions.WithDistance | GeoRadiusOptions.WithCoordinates);

        Assert.NotEmpty(results);
        var palermo = results.First(r => r.Member.ToString() == "Palermo");
        _ = Assert.NotNull(palermo.Distance);
        Assert.True(palermo.Position.HasValue);
    }

    [Fact]
    public async Task GeoSearchAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
    {
        var db = fixture.Database;
        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoSearchAsync("key", "member", shape, flags: UnsupportedCommandFlag));

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoSearchAsync("key", 0.0, 0.0, shape, flags: UnsupportedCommandFlag));
    }

    #endregion

    #region GeoSearchAndStoreAsync Tests

    [Fact]
    public async Task GeoSearchAndStoreAsync_FromMember_StoresResults()
    {
        var db = fixture.Database;
        string key = $"ser-geostore-{Guid.NewGuid()}";
        string dest = $"ser-geostore-dest-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(key, [new GeoEntry(13.361389, 38.115556, "Palermo"), new GeoEntry(15.087269, 37.502669, "Catania")]);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await db.GeoSearchAndStoreAsync(key, dest, "Palermo", shape);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GeoSearchAndStoreAsync_FromPosition_StoresResults()
    {
        var db = fixture.Database;
        string key = $"ser-geostore-{Guid.NewGuid()}";
        string dest = $"ser-geostore-dest-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(key, [new GeoEntry(13.361389, 38.115556, "Palermo"), new GeoEntry(15.087269, 37.502669, "Catania")]);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await db.GeoSearchAndStoreAsync(key, dest, 13.361389, 38.115556, shape);

        Assert.True(count >= 1);
    }

    [Fact]
    public async Task GeoSearchAndStoreAsync_WithNonNoneCommandFlags_ThrowsNotImplementedException()
    {
        var db = fixture.Database;
        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoSearchAndStoreAsync("key", "dest", "member", shape, flags: UnsupportedCommandFlag));

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoSearchAndStoreAsync("key", "dest", 0.0, 0.0, shape, flags: UnsupportedCommandFlag));
    }

    #endregion
}

/// <summary>
/// Fixture class for <see cref="GeospatialCommandsTests" />.
/// </summary>
public class GeospatialCommandsFixture : IDisposable
{
    private readonly StandaloneServer _standaloneServer;
    private readonly ConnectionMultiplexer _connection;
    private readonly GlideClient _client;

    public IDatabase Database { get; }
    public GlideClient Client => _client;

    public GeospatialCommandsFixture()
    {
        _standaloneServer = new();
        var (host, port) = _standaloneServer.Addresses.First();

        ConfigurationOptions config = new();
        config.EndPoints.Add(host, port);
        _connection = ConnectionMultiplexer.Connect(config);

        Database = _connection.GetDatabase();

        var glideConfig = new StandaloneClientConfigurationBuilder()
            .WithAddress(host, port)
            .Build();
        _client = GlideClient.CreateClient(glideConfig).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _client.Dispose();
        _connection.Dispose();
        _standaloneServer.Dispose();
    }
}
