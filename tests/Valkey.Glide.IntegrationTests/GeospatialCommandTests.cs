// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Microsoft.Testing.Platform.Extensions.TestFramework;

namespace Valkey.Glide.IntegrationTests;

public class GeospatialCommandTests(TestConfiguration config)
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

    private static readonly GeoPosition PalermoPos = new(PalermoLng, PalermoLat);
    private static readonly GeoPosition CataniaPos = new(CataniaLng, CataniaLat);
    private static readonly GeoPosition TrapaniPos = new(TrapaniLng, TrapaniLat);
    private static readonly GeoPosition EnnaPos = new(EnnaLng, EnnaLat);

    private static readonly Dictionary<ValkeyValue, GeoPosition> PalermoCatania = new()
    {
        [PalermoName] = PalermoPos,
        [CataniaName] = CataniaPos,
    };

    // Tolerances for floating-point comparisons.
    private const double CoordinateTolerance = 0.001;
    private const double UnitConversionTolerance = 0.01;

    // Common search shapes.
    private static readonly GeoSearchCircle Circle200Km = new(200, GeoUnit.Kilometers);
    private static readonly GeoSearchCircle Circle100Km = new(100, GeoUnit.Kilometers);
    private static readonly GeoSearchBox Box400Km = new(400, 400, GeoUnit.Kilometers);

    // Common expected results.
    private static readonly GeoSearchResult[] PalermoCataniaSearchResults =
    [
        new(PalermoName),
        new(CataniaName),
    ];

    #endregion
    #region GeoAddAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_SingleMember_ReturnsTrue(BaseClient client)
        => Assert.True(await client.GeoAddAsync(Guid.NewGuid().ToString(), PalermoName, PalermoPos));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_MultipleEntries_ReturnsCount(BaseClient client)
        => Assert.Equal(2, await client.GeoAddAsync(Guid.NewGuid().ToString(), PalermoCatania));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_DuplicateEntry_ReturnsFalse(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        Assert.True(await client.GeoAddAsync(key, PalermoName, PalermoPos));
        Assert.False(await client.GeoAddAsync(key, PalermoName, PalermoPos));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_InvalidLongitude_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string member = "InvalidPlace";

        // Test longitude too low (-181)
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoAddAsync(key, member, new GeoPosition(-181, 0)));

        // Test longitude too high (181)
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoAddAsync(key, member, new GeoPosition(181, 0)));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_InvalidLatitude_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string member = "InvalidPlace";

        // Test latitude too high (86)
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoAddAsync(key, member, new GeoPosition(0, 86)));

        // Test latitude too low (-86)
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoAddAsync(key, member, new GeoPosition(0, -86)));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_EmptyEntries_ThrowsException(BaseClient client)
        => _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoAddAsync(Guid.NewGuid().ToString(), new Dictionary<ValkeyValue, GeoPosition>()));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoAddAsync(key, PalermoName, PalermoPos));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_OnlyAddsIfNotExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        var onlyIfNotExists = new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists };
        Assert.Equal(0, await client.GeoAddAsync(key, PalermoCatania, onlyIfNotExists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_OnlyUpdatesIfExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        var onlyIfExists = new GeoAddOptions { Condition = GeoAddCondition.OnlyIfExists };
        Assert.False(await client.GeoAddAsync(key, TrapaniName, TrapaniPos, onlyIfExists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_WithCH_ReturnsChangedCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        var updatedLat = 40.0;
        var updateMembers = new Dictionary<ValkeyValue, GeoPosition>
        {
            [CataniaName] = new(CataniaLng, updatedLat),
            [TrapaniName] = TrapaniPos,
        };

        var changed = new GeoAddOptions { Changed = true };
        Assert.Equal(2, await client.GeoAddAsync(key, updateMembers, changed));

        GeoPosition pos = Assert.NotNull(await client.GeoPositionAsync(key, CataniaName));
        Assert.Equal(updatedLat, pos.Latitude);
    }

    #endregion
    #region GeoDistanceAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_AllUnits_ReturnsCorrectDistances(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        double metres = Assert.NotNull(await client.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Meters));
        double kilometres = Assert.NotNull(await client.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Kilometers));
        double miles = Assert.NotNull(await client.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Miles));
        double feet = Assert.NotNull(await client.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Feet));

        // Verify approximate expected values.
        Assert.True(Math.Abs(metres - 166274) < 1000);
        Assert.True(Math.Abs(kilometres - PalermoCataniaDistanceKm) < 1);
        Assert.True(Math.Abs(miles - 103.31) < 1);
        Assert.True(Math.Abs(feet - 545518) < 1000);

        // Verify unit conversions are consistent
        double metersToKm = metres / 1000;
        double metersToMiles = metres / 1609.344;
        double metersToFeet = metres * 3.28084;

        Assert.True(Math.Abs(metersToKm - kilometres) < UnitConversionTolerance);
        Assert.True(Math.Abs(metersToMiles - miles) < UnitConversionTolerance);
        Assert.True(Math.Abs(metersToFeet - feet) < 10);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_DefaultUnit_ReturnsMeters(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        // Test default unit (should be meters)
        double distanceDefault = Assert.NotNull(await client.GeoDistanceAsync(key, PalermoName, CataniaName));
        double distanceMeters = Assert.NotNull(await client.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Meters));

        Assert.Equal(distanceMeters, distanceDefault);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_NonExistentMember_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        Assert.Null(await client.GeoDistanceAsync(key, PalermoName, "NonExistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_ReturnsCorrectDistance(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        Assert.Equal(2L, await client.GeoAddAsync(key, PalermoCatania));

        double distance = Assert.NotNull(await client.GeoDistanceAsync(key, PalermoName, CataniaName, GeoUnit.Kilometers));
        Assert.Equal(PalermoCataniaDistanceKm, distance, 1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoDistanceAsync(key, "member1", "member2"));
    }

    #endregion
    #region GeoHashAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_SingleMember_ReturnsHash(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        string? hash = await client.GeoHashAsync(key, PalermoName);
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_MultipleMembers_ReturnsHashes(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        Assert.Equal(2L, await client.GeoAddAsync(key, PalermoCatania));

        string?[] hashes = await client.GeoHashAsync(key, [PalermoName, CataniaName]);
        Assert.NotNull(hashes[0]);
        Assert.NotNull(hashes[1]);
        Assert.NotEmpty(hashes[0]!);
        Assert.NotEmpty(hashes[1]!);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_NonExistentMembers_ReturnsNulls(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        string?[] hashes = await client.GeoHashAsync(key, [PalermoName, CataniaName, "NonExistent"]);
        Assert.NotNull(hashes[0]);
        Assert.NotNull(hashes[1]);
        Assert.Null(hashes[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_NonExistentMember_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        Assert.Null(await client.GeoHashAsync(key, "NonExistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoHashAsync(key, "member"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_EmptyMembers_ReturnsEmptyArray(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        Assert.Empty(await client.GeoHashAsync(key, []));
    }

    #endregion
    #region GeoPositionAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_SingleMember_ReturnsPosition(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        GeoPosition position = Assert.NotNull(await client.GeoPositionAsync(key, PalermoName));
        Assert.True(Math.Abs(position.Longitude - PalermoLng) < CoordinateTolerance);
        Assert.True(Math.Abs(position.Latitude - PalermoLat) < CoordinateTolerance);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_MultipleMembers_ReturnsPositions(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        GeoPosition?[] positions = await client.GeoPositionAsync(key, [PalermoName, CataniaName]);

        GeoPosition pos0 = Assert.NotNull(positions[0]);
        Assert.True(Math.Abs(pos0.Longitude - PalermoLng) < CoordinateTolerance);
        Assert.True(Math.Abs(pos0.Latitude - PalermoLat) < CoordinateTolerance);

        GeoPosition pos1 = Assert.NotNull(positions[1]);
        Assert.True(Math.Abs(pos1.Longitude - CataniaLng) < CoordinateTolerance);
        Assert.True(Math.Abs(pos1.Latitude - CataniaLat) < CoordinateTolerance);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_NonExistentMembers_ReturnsNulls(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        GeoPosition?[] positions = await client.GeoPositionAsync(key, [PalermoName, "NonExistent"]);
        _ = Assert.NotNull(positions[0]);
        Assert.Null(positions[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_NonExistentMember_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        Assert.Null(await client.GeoPositionAsync(key, "NonExistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoPositionAsync(key, "member"));
    }

    #endregion
    #region GeoSearchAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_AllUnits_ReturnsConsistentResults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            [PalermoName] = PalermoPos,
            [CataniaName] = CataniaPos,
            [TrapaniName] = TrapaniPos,
        };
        _ = await client.GeoAddAsync(key, members);

        // Test search with different units - all with approx 200 km radius.
        var shapeMeters = new GeoSearchCircle(200000, GeoUnit.Meters);
        var shapeKilometers = new GeoSearchCircle(200, GeoUnit.Kilometers);
        var shapeMiles = new GeoSearchCircle(124.27, GeoUnit.Miles);
        var shapeFeet = new GeoSearchCircle(656168, GeoUnit.Feet);

        var resultsMeters = await client.GeoSearchAsync(key, PalermoPos, shapeMeters);
        var resultsKilometers = await client.GeoSearchAsync(key, PalermoPos, shapeKilometers);
        var resultsMiles = await client.GeoSearchAsync(key, PalermoPos, shapeMiles);
        var resultsFeet = await client.GeoSearchAsync(key, PalermoPos, shapeFeet);

        Assert.Equivalent(PalermoCataniaSearchResults, resultsMeters);
        Assert.Equivalent(PalermoCataniaSearchResults, resultsKilometers);
        Assert.Equivalent(PalermoCataniaSearchResults, resultsMiles);
        Assert.Equivalent(PalermoCataniaSearchResults, resultsFeet);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Geo_NonExistentKey_ReturnsAppropriateDefaults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Return null for non-existent key
        Assert.Null(await client.GeoDistanceAsync(key, "member1", "member2"));
        Assert.Null(await client.GeoHashAsync(key, "member"));
        Assert.Null(await client.GeoPositionAsync(key, "member"));

        // Return empty for non-existent key
        Assert.Empty(await client.GeoSearchAsync(key, PalermoPos, Circle100Km));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_FromMember_ByRadius_ReturnsMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            [PalermoName] = PalermoPos,
            [CataniaName] = CataniaPos,
            ["edge"] = TrapaniPos,
        };
        _ = await client.GeoAddAsync(key, members);

        var results = await client.GeoSearchAsync(key, PalermoName, Circle200Km);
        Assert.Equivalent(PalermoCataniaSearchResults, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_FromPosition_ByRadius_ReturnsMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        var results = await client.GeoSearchAsync(key, PalermoPos, Circle200Km);
        Assert.Equivalent(PalermoCataniaSearchResults, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_FromMember_ByBox_ReturnsMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            [PalermoName] = PalermoPos,
            [CataniaName] = CataniaPos,
            ["edge"] = TrapaniPos,
        };
        _ = await client.GeoAddAsync(key, members);

        var results = await client.GeoSearchAsync(key, PalermoName, Box400Km);
        Assert.Equivalent(PalermoCataniaSearchResults, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_FromPosition_ByBox_ReturnsMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        var results = await client.GeoSearchAsync(key, PalermoPos, Box400Km);
        Assert.Equivalent(PalermoCataniaSearchResults, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithCount_LimitsResults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            [PalermoName] = PalermoPos,
            [CataniaName] = CataniaPos,
            ["edge1"] = TrapaniPos,
            ["edge2"] = EnnaPos,
        };

        Assert.Equal(4L, await client.GeoAddAsync(key, members));

        var allResults = await client.GeoSearchAsync(key, PalermoName, Circle200Km);
        var limitedResults = await client.GeoSearchAsync(key, PalermoName, Circle200Km, new GeoSearchOptions { Count = 2 });

        Assert.True(allResults.Length >= 2);
        Assert.Equal(2, limitedResults.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithDemandClosest_VerifiesParameterUsage(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            [PalermoName] = PalermoPos,
            [CataniaName] = CataniaPos,
            ["edge1"] = TrapaniPos,
            ["edge2"] = EnnaPos,
            ["close1"] = new(13.5, 38.0),
        };
        _ = await client.GeoAddAsync(key, members);

        var closestResults = await client.GeoSearchAsync(key, PalermoName, Circle200Km, new GeoSearchOptions { Count = 3 });
        var closestExpected = new GeoSearchResult[] { new(PalermoName), new(CataniaName), new("close1") };
        Assert.Equivalent(closestExpected, closestResults);

        // Test that demandClosest=false works (should return any results, not necessarily closest)
        var anyResults = await client.GeoSearchAsync(key, PalermoName, Circle200Km, new GeoSearchOptions { Count = 3, Any = true });
        Assert.Equal(3, anyResults.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithOrder_ReturnsOrderedResults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            [PalermoName] = PalermoPos,
            [CataniaName] = CataniaPos,
            ["edge1"] = TrapaniPos,
        };
        _ = await client.GeoAddAsync(key, members);

        var ascResults = await client.GeoSearchAsync(key, PalermoName, Circle200Km, new GeoSearchOptions { Order = Order.Ascending });
        Assert.Equivalent(new GeoSearchResult[] { new(PalermoName), new(CataniaName), new("close1") }, ascResults);

        var descResults = await client.GeoSearchAsync(key, PalermoName, Circle200Km, new GeoSearchOptions { Order = Order.Descending });
        Assert.Equivalent(new GeoSearchResult[] { new("close1"), new(CataniaName), new(PalermoName) }, descResults);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithOptions_ReturnsEnrichedResults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        var distResults = await client.GeoSearchAsync(key, PalermoName, Circle200Km, new GeoSearchOptions { WithDistance = true });
        Assert.Equivalent(new GeoSearchResult[] { new(PalermoName, distance: 0) }, distResults);

        // Test with coordinates option
        var coordResults = await client.GeoSearchAsync(key, PalermoName, Circle200Km, new GeoSearchOptions { WithPosition = true });
        Assert.Equivalent(new GeoSearchResult[] { new(PalermoName, position: PalermoPos) }, coordResults);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithDistance_ReturnsAccurateDistances(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoCatania);

        var options = new GeoSearchOptions { WithDistance = true, Order = Order.Ascending };
        var results = await client.GeoSearchAsync(key, PalermoName, Circle200Km, options);

        Assert.Equal(2, results.Length);
        Assert.Equal(0.0, results[0].Distance!.Value, 1);
        Assert.Equal(PalermoCataniaDistanceKm, results[1].Distance!.Value, 1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_NonExistentMember_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoSearchAsync(key, "NonExistentMember", Circle100Km));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoSearchAsync(key, PalermoPos, Circle100Km));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_NoMembersInArea_ReturnsEmpty(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, PalermoName, PalermoPos);

        var position = new GeoPosition(0.0, 0.0); // Far from Palermo
        var shape = new GeoSearchCircle(1, GeoUnit.Meters); // Very small radius

        Assert.Empty(await client.GeoSearchAsync(key, position, shape));
    }

    #endregion
    #region GeoSearchAndStoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WithMember_StoresResults(BaseClient client)
    {
        string prefix = "{" + Guid.NewGuid().ToString() + "}";
        string source = prefix + ":source";
        string dest = prefix + ":dest";

        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            [PalermoName] = PalermoPos,
            [CataniaName] = CataniaPos,
            [TrapaniName] = TrapaniPos,
        };
        _ = await client.GeoAddAsync(source, members);

        Assert.Equal(3, await client.GeoSearchAndStoreAsync(source, dest, PalermoName, Circle200Km));

        var stored = await client.GeoSearchAsync(dest, PalermoPos, Circle200Km, new GeoSearchOptions { Order = Order.Ascending });
        Assert.Equivalent(new GeoSearchResult[] { new(CataniaName), new(PalermoName), new(TrapaniName) }, stored);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WithPosition_StoresResults(BaseClient client)
    {
        string prefix = "{" + Guid.NewGuid().ToString() + "}";
        string source = prefix + ":source";
        string dest = prefix + ":dest";

        _ = await client.GeoAddAsync(source, PalermoCatania);

        Assert.Equal(2, await client.GeoSearchAndStoreAsync(source, dest, PalermoPos, Circle200Km));

        var stored = await client.GeoSearchAsync(dest, PalermoPos, Circle200Km, new GeoSearchOptions { Order = Order.Ascending });
        Assert.Equivalent(new GeoSearchResult[] { new(CataniaName), new(PalermoName) }, stored);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WithDistances_StoresDistances(BaseClient client)
    {
        string prefix = "{" + Guid.NewGuid().ToString() + "}";
        string source = prefix + ":source";
        string dest = prefix + ":dest";

        _ = await client.GeoAddAsync(source, PalermoCatania);

        Assert.Equal(2, await client.GeoSearchAndStoreAsync(source, dest, PalermoName, Circle200Km, new GeoSearchStoreOptions { StoreDistances = true }));

        var results = await client.SortedSetRangeByRankWithScoresAsync(dest, 0, -1);
        Assert.Equal(2, results.Length);

        var palermoResult = results.FirstOrDefault(r => r.Element.ToString() == PalermoName);
        var cataniaResult = results.FirstOrDefault(r => r.Element.ToString() == CataniaName);

        Assert.Equal(0.0, palermoResult.Score, 0.1);
        Assert.Equal(166.2742, cataniaResult.Score, 0.1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_NonExistentMember_ThrowsException(BaseClient client)
    {
        string prefix = "{" + Guid.NewGuid().ToString() + "}";
        string source = prefix + ":source";
        string dest = prefix + ":dest";

        _ = await client.GeoAddAsync(source, PalermoName, PalermoPos);

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoSearchAndStoreAsync(source, dest, "NonExistentMember", Circle100Km));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WrongKeyType_ThrowsException(BaseClient client)
    {
        string prefix = "{" + Guid.NewGuid().ToString() + "}";
        string source = prefix + ":source";
        string dest = prefix + ":dest";

        await client.StringSetAsync(source, "not_a_geo_key");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoSearchAndStoreAsync(source, dest, PalermoPos, Circle100Km));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WithCount_LimitsStoredResults(BaseClient client)
    {
        string prefix = "{" + Guid.NewGuid().ToString() + "}";
        string source = prefix + ":source";
        string dest = prefix + ":dest";

        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            [PalermoName] = PalermoPos,
            [CataniaName] = CataniaPos,
            [TrapaniName] = TrapaniPos,
            [EnnaName] = EnnaPos,
        };
        _ = await client.GeoAddAsync(source, members);

        Assert.Equal(2, await client.GeoSearchAndStoreAsync(source, dest, PalermoName, Circle200Km, new GeoSearchStoreOptions { Count = 2 }));

        var storedMembers = await client.SortedSetRangeByRankAsync(dest, 0, -1);
        Assert.Equal(2, storedMembers.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_OverwritesDestination(BaseClient client)
    {
        string prefix = "{" + Guid.NewGuid().ToString() + "}";
        string source = prefix + ":source";
        string dest = prefix + ":dest";

        _ = await client.GeoAddAsync(source, PalermoCatania);

        _ = await client.SortedSetAddAsync(dest, [new SortedSetEntry("OldMember", 100)]);
        Assert.Equal(1, await client.SortedSetCardAsync(dest));

        Assert.Equal(2, await client.GeoSearchAndStoreAsync(source, dest, PalermoName, Circle200Km));

        var storedMembers = await client.SortedSetRangeByRankAsync(dest, 0, -1);
        Assert.Equivalent(new[] { PalermoName, CataniaName }, storedMembers.Select(m => m.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_NoResults_CreatesEmptyDestination(BaseClient client)
    {
        string prefix = "{" + Guid.NewGuid().ToString() + "}";
        string source = prefix + ":source";
        string dest = prefix + ":dest";

        _ = await client.GeoAddAsync(source, PalermoName, PalermoPos);

        var position = new GeoPosition(0.0, 0.0);
        var shape = new GeoSearchCircle(1, GeoUnit.Meters);
        long count = await client.GeoSearchAndStoreAsync(source, dest, position, shape);

        Assert.Equal(0, count);
        Assert.Equal(0, await client.SortedSetCardAsync(dest));
    }

    #endregion
}
