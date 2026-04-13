// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> geospatial commands.
/// </summary>
public class GeospatialCommandsTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Constants

    private const string PalermoName = "Palermo";
    private const string CataniaName = "Catania";
    private const string TrapaniName = "Trapani";
    private const string EnnaName = "Enna";

    private const double PalermoLng = 13.361389;
    private const double PalermoLat = 38.115556;
    private const double CataniaLng = 15.087269;
    private const double CataniaLat = 37.502669;
    private const double TrapaniLng = 12.758489;
    private const double TrapaniLat = 38.788135;
    private const double EnnaLng = 14.015482;
    private const double EnnaLat = 37.734741;

    private const double PalermoCataniaDistanceKm = 166.27;

    private static readonly GeoEntry Palermo = new(PalermoLng, PalermoLat, PalermoName);
    private static readonly GeoEntry Catania = new(CataniaLng, CataniaLat, CataniaName);
    private static readonly GeoEntry Trapani = new(TrapaniLng, TrapaniLat, TrapaniName);
    private static readonly GeoEntry Enna = new(EnnaLng, EnnaLat, EnnaName);

    // Tolerances for floating-point comparisons.
    private const double CoordinateTolerance = 0.001;
    private const double UnitConversionTolerance = 0.01;

    // Common search shapes.
    private static readonly GeoSearchCircle Circle200Km = new(200, GeoUnit.Kilometers);
    private static readonly GeoSearchBox Box400Km = new(400, 400, GeoUnit.Kilometers);

    // Common expected results.
    private static readonly GeoRadiusResult PalermoResult = new(PalermoName, null, null, null);
    private static readonly GeoRadiusResult CataniaResults = new(CataniaName, null, null, null);
    private static readonly GeoRadiusResult TrapaniResult = new(TrapaniName, null, null, null);

    private static readonly GeoRadiusResult[] PalermoCataniaRadiusResults = [PalermoResult, CataniaResults];

    #endregion
    #region GeoAddAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoAddAsync_SingleEntry_ReturnsTrue(IDatabaseAsync db)
        => Assert.True(await db.GeoAddAsync(Guid.NewGuid().ToString(), PalermoLng, PalermoLat, PalermoName));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoAddAsync_GeoEntry_ReturnsTrue(IDatabaseAsync db)
        => Assert.True(await db.GeoAddAsync(Guid.NewGuid().ToString(), Palermo));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoAddAsync_MultipleEntries_ReturnsCount(IDatabaseAsync db)
        => Assert.Equal(2, await db.GeoAddAsync(Guid.NewGuid().ToString(), [Palermo, Catania]));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoAddAsync_DuplicateEntry_ReturnsFalse(IDatabaseAsync db)
    {
        string key = $"ser-geoadd-dup-{Guid.NewGuid()}";
        Assert.True(await db.GeoAddAsync(key, PalermoLng, PalermoLat, PalermoName));

        Assert.False(await db.GeoAddAsync(key, PalermoLng, PalermoLat, PalermoName));
    }

    #endregion
    #region GeoDistanceAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistanceAsync_Kilometers_ReturnsCorrectDistance(IDatabaseAsync db)
    {
        string key = $"ser-geodist-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        double distance = Assert.NotNull(await db.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Kilometers));
        Assert.Equal(PalermoCataniaDistanceKm, distance, 1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistanceAsync_DefaultUnit_ReturnsMeters(IDatabaseAsync db)
    {
        string key = $"ser-geodist-def-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        Assert.Equal(
            await db.GeoDistanceAsync(key, PalermoName, CataniaName),
            await db.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Meters));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistanceAsync_AllUnits_ReturnsConsistentDistances(IDatabaseAsync db)
    {
        string key = $"ser-geodist-units-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        double meters = Assert.NotNull(await db.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Meters));
        double km = Assert.NotNull(await db.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Kilometers));
        double miles = Assert.NotNull(await db.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Miles));
        double feet = Assert.NotNull(await db.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Feet));

        Assert.True(Math.Abs((meters / 1000) - km) < UnitConversionTolerance);
        Assert.True(Math.Abs((meters / 1609.344) - miles) < UnitConversionTolerance);
        Assert.True(Math.Abs((meters * (1 / 0.3048)) - feet) < UnitConversionTolerance);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistanceAsync_NonExistentMember_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-geodist-noexist-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, PalermoLng, PalermoLat, PalermoName);

        Assert.Null(await db.GeoDistanceAsync(key, PalermoName, "NonExistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistanceAsync_NonExistentKey_ReturnsNull(IDatabaseAsync db)
        => Assert.Null(await db.GeoDistanceAsync(Guid.NewGuid().ToString(), "member1", "member2"));

    #endregion
    #region GeoHashAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoHashAsync_SingleMember_ReturnsHash(IDatabaseAsync db)
    {
        string key = $"ser-geohash-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, PalermoLng, PalermoLat, PalermoName);

        string? hash = await db.GeoHashAsync(key, PalermoName);
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoHashAsync_MultipleMembers_ReturnsHashes(IDatabaseAsync db)
    {
        string key = $"ser-geohash-multi-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        string?[] hashes = await db.GeoHashAsync(key, [PalermoName, CataniaName, "NonExistent"]);

        Assert.Equal(3, hashes.Length);
        Assert.NotNull(hashes[0]);
        Assert.NotNull(hashes[1]);
        Assert.Null(hashes[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoHashAsync_NonExistentMember_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-geohash-noexist-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, PalermoLng, PalermoLat, PalermoName);

        Assert.Null(await db.GeoHashAsync(key, "NonExistent"));
    }

    #endregion
    #region GeoPositionAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoPositionAsync_SingleMember_ReturnsPosition(IDatabaseAsync db)
    {
        string key = $"ser-geopos-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, PalermoLng, PalermoLat, PalermoName);

        GeoPosition position = Assert.NotNull(await db.GeoPositionAsync(key, PalermoName));
        Assert.True(Math.Abs(position.Longitude - PalermoLng) < CoordinateTolerance);
        Assert.True(Math.Abs(position.Latitude - PalermoLat) < CoordinateTolerance);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoPositionAsync_MultipleMembers_ReturnsPositions(IDatabaseAsync db)
    {
        string key = $"ser-geopos-multi-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        ValkeyValue[] members = [PalermoName, CataniaName, "NonExistent"];
        var positions = await db.GeoPositionAsync(key, members);

        Assert.Equal(3, positions.Length);

        var pos0 = Assert.NotNull(positions[0]);
        Assert.True(Math.Abs(pos0.Longitude - PalermoLng) < CoordinateTolerance);

        var pos1 = Assert.NotNull(positions[1]);
        Assert.True(Math.Abs(pos1.Longitude - CataniaLng) < CoordinateTolerance);

        Assert.Null(positions[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoPositionAsync_NonExistentMember_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-geopos-noexist-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, PalermoLng, PalermoLat, PalermoName);

        Assert.Null(await db.GeoPositionAsync(key, "NonExistent"));
    }

    #endregion
    #region GeoSearchAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_FromMember_ByRadius_ReturnsMembers(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-member-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania, Trapani]);

        var results = await db.GeoSearchAsync(key, PalermoName, Circle200Km);
        Assert.Equivalent(PalermoCataniaRadiusResults, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_FromPosition_ByRadius_ReturnsMembers(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-pos-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        var results = await db.GeoSearchAsync(key, PalermoName, Circle200Km);
        Assert.Equivalent(PalermoCataniaRadiusResults, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_FromMember_ByBox_ReturnsMembers(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-box-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        var results = await db.GeoSearchAsync(key, PalermoName, Box400Km);
        Assert.Equivalent(PalermoCataniaRadiusResults, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_WithCount_LimitsResults(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-count-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania, Trapani, Enna]);

        var results = await db.GeoSearchAsync(key, PalermoName, Circle200Km);
        Assert.Equivalent(PalermoCataniaRadiusResults, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_WithOrder_ReturnsOrderedResults(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-order-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania, Trapani]);

        var ascResults = await db.GeoSearchAsync(key, PalermoName, Circle200Km, order: Order.Ascending);
        Assert.Equal([PalermoResult, TrapaniResult, CataniaResults], ascResults);

        var descResults = await db.GeoSearchAsync(key, PalermoName, Circle200Km, order: Order.Descending);
        Assert.Equal([CataniaResults, TrapaniResult, PalermoResult], descResults);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_WithDistance_ReturnsDistances(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-dist-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        var results = await db.GeoSearchAsync(key, PalermoName, Circle200Km, options: GeoRadiusOptions.WithDistance);

        GeoRadiusResult[] expected =
        [
            new(PalermoName, distance: 0.0, hash: null, position: null),
            new(CataniaName, distance: PalermoCataniaDistanceKm, hash: null, position: null),
        ];
        Assert.Equivalent(expected, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_WithCoordinates_ReturnsPositions(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-coord-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, [Palermo, Catania]);

        var results = await db.GeoSearchAsync(key, PalermoName, Circle200Km, options: GeoRadiusOptions.WithCoordinates);

        GeoRadiusResult[] expected =
        [
            new(PalermoName, distance: null, hash: null, position: new GeoPosition(PalermoLng, PalermoLat)),
            new(CataniaName, distance: null, hash: null, position: new GeoPosition(CataniaLng, CataniaLat)),
        ];
        Assert.Equivalent(expected, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_WithDemandClosest_LimitsToClosest(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-closest-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key,
        [
            Palermo,
            Catania,
            new GeoEntry(13.5, 38.0, "Close"),
            Trapani,
            Enna,
        ]);

        var closestResults = await db.GeoSearchAsync(key, PalermoName, Circle200Km, count: 3, demandClosest: true);
        GeoRadiusResult[] expectedClosest = [PalermoResult, new("Close", null, null, null), TrapaniResult];
        Assert.Equivalent(expectedClosest, closestResults);

        var anyResults = await db.GeoSearchAsync(key, PalermoName, Circle200Km, count: 3, demandClosest: false, order: Order.Ascending);
        GeoRadiusResult[] expectedAny = [PalermoResult, new("Close", null, null, null), TrapaniResult];
        Assert.Equivalent(expectedAny, anyResults);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_NonExistentKey_ReturnsEmpty(IDatabaseAsync db)
    {
        var shape = new GeoSearchCircle(100, GeoUnit.Kilometers);
        Assert.Empty(await db.GeoSearchAsync(Guid.NewGuid().ToString(), PalermoLng, PalermoLat, shape));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_NoMembersInArea_ReturnsEmpty(IDatabaseAsync db)
    {
        string key = $"ser-geosearch-empty-{Guid.NewGuid()}";
        _ = await db.GeoAddAsync(key, PalermoLng, PalermoLat, PalermoName);

        var shape = new GeoSearchCircle(1, GeoUnit.Meters);
        Assert.Empty(await db.GeoSearchAsync(key, 0, 0, shape));
    }

    #endregion
    #region GeoSearchAndStoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStoreAsync_FromMember_StoresResults(IDatabaseAsync db)
    {
        string source = $"ser-geostore-src-{Guid.NewGuid()}";
        string dest = $"ser-geostore-dst-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(source, [Palermo, Catania, Trapani]);

        Assert.Equal(3, await db.GeoSearchAndStoreAsync(source, dest, PalermoName, Circle200Km));

        var stored = await db.GeoSearchAsync(dest, PalermoName, Circle200Km);
        GeoRadiusResult[] expected =
        [
            new(PalermoName, null, null, null),
            new(CataniaName, null, null, null),
            new(TrapaniName, null, null, null)
        ];
        Assert.Equivalent(expected, stored);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStoreAsync_FromPosition_StoresResults(IDatabaseAsync db)
    {
        string source = $"ser-geostore-pos-src-{Guid.NewGuid()}";
        string dest = $"ser-geostore-pos-dst-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(source, [Palermo, Catania]);

        Assert.Equal(1, await db.GeoSearchAndStoreAsync(source, dest, PalermoLng, PalermoLat, Circle200Km));

        var stored = await db.GeoSearchAsync(dest, PalermoLng, PalermoLat, Circle200Km);
        Assert.Contains(new GeoRadiusResult(PalermoName, null, null, null), stored);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStoreAsync_WithDistances_StoresDistances(IDatabaseAsync db)
    {
        string source = $"ser-geostore-dist-src-{Guid.NewGuid()}";
        string dest = $"ser-geostore-dist-dst-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(source, [Palermo, Catania]);

        long count = await db.GeoSearchAndStoreAsync(source, dest, PalermoName, Circle200Km, storeDistances: true);

        Assert.Equal(2, count);

        SortedSetEntry[] entries = await db.SortedSetRangeByRankWithScoresAsync(dest);
        Assert.Equal(2, entries.Length);

        var palermoEntry = entries.First(e => e.Element.ToString() == PalermoName);
        var cataniaEntry = entries.First(e => e.Element.ToString() == CataniaName);

        Assert.Equal(0.0, palermoEntry.Score, 0.1);
        Assert.Equal(PalermoCataniaDistanceKm, cataniaEntry.Score, 0.1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStoreAsync_WithCount_LimitsStoredResults(IDatabaseAsync db)
    {
        string source = $"ser-geostore-cnt-src-{Guid.NewGuid()}";
        string dest = $"ser-geostore-cnt-dst-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(source, [Palermo, Catania, Trapani, Enna]);

        Assert.Equal(2, await db.GeoSearchAndStoreAsync(source, dest, PalermoName, Circle200Km, count: 2));

        var stored = await db.GeoSearchAsync(dest, PalermoName, Circle200Km);
        Assert.Equal(2, stored.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStoreAsync_OverwritesDestination(IDatabaseAsync db)
    {
        string source = $"ser-geostore-ow-src-{Guid.NewGuid()}";
        string dest = $"ser-geostore-ow-dst-{Guid.NewGuid()}";

        _ = await db.GeoAddAsync(source, [Palermo, Catania]);

        // Pre-populate destination
        _ = await db.SortedSetAddAsync(dest, [new SortedSetEntry("OldMember", 100)]);
        _ = Assert.Single(await db.SortedSetRangeByRankAsync(dest));

        Assert.Equal(2, await db.GeoSearchAndStoreAsync(source, dest, PalermoName, Circle200Km));

        var stored = await db.GeoSearchAsync(dest, PalermoName, Circle200Km);
        Assert.Equivalent(PalermoCataniaRadiusResults, stored);
    }

    #endregion
}
