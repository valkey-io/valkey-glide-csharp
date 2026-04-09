// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

public class GeospatialCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    /// <summary>
    /// Helper to add GeoEntry items using the new dictionary-based API.
    /// </summary>
    private static async Task<long> AddGeoEntriesAsync(BaseClient client, string key, GeoEntry[] entries)
    {
        var members = entries.ToDictionary(e => e.Member, e => new GeoPosition(e.Longitude, e.Latitude));
        return await client.GeoAddAsync(key, members);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_SingleEntry_ReturnsTrue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string member = "Palermo";
        var position = new GeoPosition(13.361389, 38.115556);

        bool result = await client.GeoAddAsync(key, member, position);
        Assert.True(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_GeoEntry_ReturnsTrue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        bool result = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));
        Assert.True(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_MultipleEntries_ReturnsCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            ["Palermo"] = new(13.361389, 38.115556),
            ["Catania"] = new(15.087269, 37.502669),
        };

        long result = await client.GeoAddAsync(key, members);
        Assert.Equal(2, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_DuplicateEntry_ReturnsFalse(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var position = new GeoPosition(13.361389, 38.115556);
        string member = "Palermo";

        bool firstResult = await client.GeoAddAsync(key, member, position);
        bool secondResult = await client.GeoAddAsync(key, member, position);

        Assert.True(firstResult);
        Assert.False(secondResult);
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
    {
        string key = Guid.NewGuid().ToString();
        var emptyMembers = new Dictionary<ValkeyValue, GeoPosition>();

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoAddAsync(key, emptyMembers));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");

        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556)));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_WithNX_OnlyAddsIfNotExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial entries
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            ["Palermo"] = new(13.361389, 38.115556),
            ["Catania"] = new(15.087269, 37.502669),
        };
        long result = await client.GeoAddAsync(key, members);
        Assert.Equal(2, result);

        // Try to add with NX option - should return 0 since members already exist
        var newMembers = new Dictionary<ValkeyValue, GeoPosition>
        {
            ["Palermo"] = new(13.361389, 39.0), // Different latitude
            ["Catania"] = new(15.087269, 38.0), // Different latitude
        };
        var nxOptions = new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists };
        long nxResult = await client.GeoAddAsync(key, newMembers, nxOptions);
        Assert.Equal(0, nxResult);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_WithXX_OnlyUpdatesIfExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial entries
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            ["Palermo"] = new(13.361389, 38.115556),
            ["Catania"] = new(15.087269, 37.502669),
        };
        long result = await client.GeoAddAsync(key, members);
        Assert.Equal(2, result);

        // Try to add new member with XX option - should return false since member doesn't exist
        var xxOptions = new GeoAddOptions { Condition = GeoAddCondition.OnlyIfExists };
        bool xxResult = await client.GeoAddAsync(key, "Tel-Aviv", new GeoPosition(32.0853, 34.7818), xxOptions);
        Assert.False(xxResult);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_WithCH_ReturnsChangedCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial entries
        var members = new Dictionary<ValkeyValue, GeoPosition>
        {
            ["Palermo"] = new(13.361389, 38.115556),
            ["Catania"] = new(15.087269, 37.502669),
        };
        long result = await client.GeoAddAsync(key, members);
        Assert.Equal(2, result);

        // Update existing member and add new member with CH option
        var updateMembers = new Dictionary<ValkeyValue, GeoPosition>
        {
            ["Catania"] = new(15.087269, 40.0), // Update existing
            ["Tel-Aviv"] = new(32.0853, 34.7818), // Add new
        };
        var chOptions = new GeoAddOptions { Changed = true };
        long chResult = await client.GeoAddAsync(key, updateMembers, chOptions);
        Assert.Equal(2, chResult); // Should return 2 (1 changed + 1 added)
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_AllUnits_ReturnsCorrectDistances(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        // Test all units with expected values (approximate distance between Palermo and Catania)
        double? distanceMeters = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Meters);
        double? distanceKilometers = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Kilometers);
        double? distanceMiles = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Miles);
        double? distanceFeet = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Feet);

        // Verify approximate expected values (distance between Palermo and Catania)
        Assert.True(Math.Abs(distanceMeters.Value - 166274) < 1000); // ~166274 meters
        Assert.True(Math.Abs(distanceKilometers.Value - 166.27) < 1); // ~166.27 km
        Assert.True(Math.Abs(distanceMiles.Value - 103.31) < 1); // ~103.31 miles
        Assert.True(Math.Abs(distanceFeet.Value - 545518) < 1000); // ~545,518 feet

        // Verify unit conversions are consistent
        double metersToKm = distanceMeters.Value / 1000;
        double metersToMiles = distanceMeters.Value / 1609.344;
        double metersToFeet = distanceMeters.Value * 3.28084;

        Assert.True(Math.Abs(metersToKm - distanceKilometers.Value) < 0.01);
        Assert.True(Math.Abs(metersToMiles - distanceMiles.Value) < 0.01);
        Assert.True(Math.Abs(metersToFeet - distanceFeet.Value) < 10);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_DefaultUnit_ReturnsMeters(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        // Test default unit (should be meters)
        double? distanceDefault = await client.GeoDistanceAsync(key, "Palermo", "Catania");
        double? distanceMeters = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Meters);

        Assert.True(Math.Abs(distanceMeters.Value - distanceDefault.Value) < 1e-9);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_AllUnits_ReturnsConsistentResults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries = [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
            new GeoEntry(12.758489, 38.788135, "Trapani")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var position = new GeoPosition(13.361389, 38.115556); // Palermo coordinates

        // Test search with different units - all should return same members but with different radius values
        var shapeMeters = new GeoSearchCircle(200000, GeoUnit.Meters); // 200km in meters
        var shapeKilometers = new GeoSearchCircle(200, GeoUnit.Kilometers); // 200km
        var shapeMiles = new GeoSearchCircle(124.27, GeoUnit.Miles); // ~200km in miles
        var shapeFeet = new GeoSearchCircle(656168, GeoUnit.Feet); // ~200km in feet

        GeoRadiusResult[] resultsMeters = await client.GeoSearchAsync(key, position, shapeMeters);
        GeoRadiusResult[] resultsKilometers = await client.GeoSearchAsync(key, position, shapeKilometers);
        GeoRadiusResult[] resultsMiles = await client.GeoSearchAsync(key, position, shapeMiles);
        GeoRadiusResult[] resultsFeet = await client.GeoSearchAsync(key, position, shapeFeet);

        // All searches should return the same members (Palermo and Catania should be within 200km)
        Assert.NotEmpty(resultsMeters);
        Assert.NotEmpty(resultsKilometers);
        Assert.NotEmpty(resultsMiles);
        Assert.NotEmpty(resultsFeet);

        // Should return same number of results
        Assert.Equal(resultsMeters.Length, resultsKilometers.Length);
        Assert.Equal(resultsMeters.Length, resultsMiles.Length);
        Assert.Equal(resultsMeters.Length, resultsFeet.Length);

        // Should contain Palermo and Catania in all unit searches
        Assert.Contains("Palermo", resultsMeters.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", resultsMeters.Select(r => r.Member.ToString()));
        Assert.Contains("Palermo", resultsKilometers.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", resultsKilometers.Select(r => r.Member.ToString()));
        Assert.Contains("Palermo", resultsMiles.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", resultsMiles.Select(r => r.Member.ToString()));
        Assert.Contains("Palermo", resultsFeet.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", resultsFeet.Select(r => r.Member.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_NonExistentMembers_ReturnsNulls(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries = [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        string?[] hashes = await client.GeoHashAsync(key, ["Palermo", "Catania", "NonExistent"]);
        Assert.NotNull(hashes[0]); // Palermo
        Assert.NotNull(hashes[1]); // Catania
        Assert.Null(hashes[2]); // NonExistent
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_NonExistentMembers_ReturnsNulls(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));

        GeoPosition?[] positions = await client.GeoPositionAsync(key, ["Palermo", "NonExistent"]);
        _ = Assert.NotNull(positions[0]); // Palermo
        Assert.Null(positions[1]); // NonExistent
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_NonExistentMember_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));

        double? distance = await client.GeoDistanceAsync(key, "Palermo", "NonExistent");
        Assert.Null(distance);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task Geo_NonExistentKey_ReturnsAppropriateDefaults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString(); // Non-existent key

        // GeoDistance should return null for non-existent key
        double? distance = await client.GeoDistanceAsync(key, "member1", "member2");
        Assert.Null(distance);

        // GeoHash should return null for non-existent key
        string? hash = await client.GeoHashAsync(key, "member");
        Assert.Null(hash);

        // GeoPosition should return null for non-existent key
        GeoPosition? position = await client.GeoPositionAsync(key, "member");
        Assert.Null(position);

        // GeoSearch should return empty array for non-existent key
        var searchPosition = new GeoPosition(13.361389, 38.115556);
        var shape = new GeoSearchCircle(100, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, searchPosition, shape);
        Assert.Empty(results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_ReturnsCorrectDistance(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];

        _ = await AddGeoEntriesAsync(client, key, entries);

        double? distance = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Kilometers);
        _ = Assert.NotNull(distance);
        Assert.Equal(166.27, distance.Value, 1); // Approximate distance between Palermo and Catania
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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_SingleMember_ReturnsHash(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));

        string? hash = await client.GeoHashAsync(key, "Palermo");
        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_MultipleMembers_ReturnsHashes(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        string?[] hashes = await client.GeoHashAsync(key, ["Palermo", "Catania"]);
        Assert.NotNull(hashes[0]);
        Assert.NotNull(hashes[1]);
        Assert.NotEmpty(hashes[0]!);
        Assert.NotEmpty(hashes[1]!);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_NonExistentMember_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));

        string? hash = await client.GeoHashAsync(key, "NonExistent");
        Assert.Null(hash);
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
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));

        string?[] hashes = await client.GeoHashAsync(key, []);
        Assert.Empty(hashes);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_SingleMember_ReturnsPosition(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        double longitude = 13.361389;
        double latitude = 38.115556;
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(longitude, latitude));

        GeoPosition? position = await client.GeoPositionAsync(key, "Palermo");
        _ = Assert.NotNull(position);
        Assert.True(Math.Abs(position.Value.Longitude - longitude) < 0.001);
        Assert.True(Math.Abs(position.Value.Latitude - latitude) < 0.001);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_MultipleMembers_ReturnsPositions(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        GeoPosition?[] positions = await client.GeoPositionAsync(key, ["Palermo", "Catania"]);
        _ = Assert.NotNull(positions[0]);
        _ = Assert.NotNull(positions[1]);
        Assert.True(Math.Abs(positions[0]!.Value.Longitude - 13.361389) < 0.001);
        Assert.True(Math.Abs(positions[0]!.Value.Latitude - 38.115556) < 0.001);
        Assert.True(Math.Abs(positions[1]!.Value.Longitude - 15.087269) < 0.001);
        Assert.True(Math.Abs(positions[1]!.Value.Latitude - 37.502669) < 0.001);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_NonExistentMember_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));

        GeoPosition? position = await client.GeoPositionAsync(key, "NonExistent");
        Assert.Null(position);
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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_FromMember_ByRadius_ReturnsMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
            new GeoEntry(12.758489, 38.788135, "edge")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, "Palermo", shape);

        Assert.NotEmpty(results);
        Assert.Contains("Palermo", results.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", results.Select(r => r.Member.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_FromPosition_ByRadius_ReturnsMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var position = new GeoPosition(13.361389, 38.115556); // Palermo coordinates
        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, position, shape);

        Assert.NotEmpty(results);
        Assert.Contains("Palermo", results.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", results.Select(r => r.Member.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_FromMember_ByBox_ReturnsMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
            new GeoEntry(12.758489, 38.788135, "edge")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var shape = new GeoSearchBox(400, 400, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, "Palermo", shape);

        Assert.NotEmpty(results);
        Assert.Contains("Palermo", results.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", results.Select(r => r.Member.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_FromPosition_ByBox_ReturnsMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var position = new GeoPosition(13.361389, 38.115556); // Palermo coordinates
        var shape = new GeoSearchBox(400, 400, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, position, shape);

        Assert.NotEmpty(results);
        Assert.Contains("Palermo", results.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", results.Select(r => r.Member.ToString()));
    }



    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithCount_LimitsResults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
            new GeoEntry(12.758489, 38.788135, "edge1"),
            new GeoEntry(14.015482, 37.734741, "edge2")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        GeoRadiusResult[] allResults = await client.GeoSearchAsync(key, "Palermo", shape);
        GeoRadiusResult[] limitedResults = await client.GeoSearchAsync(key, "Palermo", shape, 2);

        Assert.True(allResults.Length >= 2);
        Assert.Equal(2, limitedResults.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithDemandClosest_VerifiesParameterUsage(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
            new GeoEntry(12.758489, 38.788135, "edge1"),
            new GeoEntry(14.015482, 37.734741, "edge2"),
            new GeoEntry(13.5, 38.0, "close1")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);

        // Test that demandClosest=true works (should return closest results)
        GeoRadiusResult[] closestResults = await client.GeoSearchAsync(key, "Palermo", shape, 3, true);
        Assert.Equal(3, closestResults.Length);
        Assert.Contains("Palermo", closestResults.Select(r => r.Member.ToString()));
        Assert.Contains("close1", closestResults.Select(r => r.Member.ToString())); // close1 should be in closest results

        // Test that demandClosest=false works (should return any results, not necessarily closest)
        GeoRadiusResult[] anyResults = await client.GeoSearchAsync(key, "Palermo", shape, 3, false);
        Assert.Equal(3, anyResults.Length);
        Assert.Contains("Palermo", anyResults.Select(r => r.Member.ToString()));

        // Both should return valid results, verifying the parameter is accepted
        Assert.NotEmpty(closestResults);
        Assert.NotEmpty(anyResults);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithOrder_ReturnsOrderedResults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
            new GeoEntry(12.758489, 38.788135, "edge1")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);

        // Test ascending order
        GeoRadiusResult[] ascResults = await client.GeoSearchAsync(key, "Palermo", shape, order: Order.Ascending);
        Assert.NotEmpty(ascResults);

        // Test descending order
        GeoRadiusResult[] descResults = await client.GeoSearchAsync(key, "Palermo", shape, order: Order.Descending);
        Assert.NotEmpty(descResults);

        // Verify both return same count but potentially different order
        Assert.Equal(ascResults.Length, descResults.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithOptions_ReturnsEnrichedResults(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);

        // Test with distance option
        GeoRadiusResult[] distResults = await client.GeoSearchAsync(key, "Palermo", shape, options: GeoRadiusOptions.WithDistance);
        Assert.NotEmpty(distResults);

        var palermoDist = distResults.FirstOrDefault(r => r.Member.ToString() == "Palermo");
        Assert.Equal("Palermo", palermoDist.Member.ToString());
        _ = Assert.NotNull(palermoDist.Distance); // Should have distance
        Assert.Equal(0.0, palermoDist.Distance.Value, 1); // Distance from itself should be ~0
        Assert.Null(palermoDist.Position); // Should be null without WithCoordinates

        // Test with coordinates option
        GeoRadiusResult[] coordResults = await client.GeoSearchAsync(key, "Palermo", shape, options: GeoRadiusOptions.WithCoordinates);
        Assert.NotEmpty(coordResults);

        var palermoCoord = coordResults.FirstOrDefault(r => r.Member.ToString() == "Palermo");
        Assert.Equal("Palermo", palermoCoord.Member.ToString());
        Assert.True(palermoCoord.Position.HasValue); // Should have coordinates
        Assert.Null(palermoCoord.Distance); // Should be null without WithDistance
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WithDistance_ReturnsAccurateDistances(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, key, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, "Palermo", shape, options: GeoRadiusOptions.WithDistance);

        var palermoResult = results.FirstOrDefault(r => r.Member.ToString() == "Palermo");
        var cataniaResult = results.FirstOrDefault(r => r.Member.ToString() == "Catania");

        _ = Assert.NotNull(palermoResult.Distance);
        _ = Assert.NotNull(cataniaResult.Distance);

        Assert.Equal(0.0, palermoResult.Distance.Value, 1); // Distance from itself should be ~0
        Assert.Equal(166.27, cataniaResult.Distance.Value, 1); // ~166km between cities
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WithMember_StoresResults(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string sourceKey = keyPrefix + ":source";
        string destinationKey = keyPrefix + ":dest";

        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
            new GeoEntry(12.758489, 38.788135, "Trapani")
        ];
        _ = await AddGeoEntriesAsync(client, sourceKey, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, "Palermo", shape);

        Assert.Equal(3, count);

        ValkeyValue[] storedMembers = await client.SortedSetRangeByRankAsync(destinationKey, 0, -1);
        Assert.Equal(3, storedMembers.Length);
        Assert.Contains("Palermo", storedMembers.Select(r => r.ToString()));
        Assert.Contains("Catania", storedMembers.Select(r => r.ToString()));
        Assert.Contains("Trapani", storedMembers.Select(r => r.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WithPosition_StoresResults(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string sourceKey = keyPrefix + ":source";
        string destinationKey = keyPrefix + ":dest";

        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, sourceKey, entries);

        var position = new GeoPosition(13.361389, 38.115556);
        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, position, shape);

        Assert.True(count >= 1);
        Assert.Contains("Palermo", (await client.SortedSetRangeByRankAsync(destinationKey, 0, -1)).Select(r => r.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WithDistances_StoresDistances(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string sourceKey = keyPrefix + ":source";
        string destinationKey = keyPrefix + ":dest";

        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, sourceKey, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, "Palermo", shape, storeDistances: true);

        Assert.Equal(2, count);

        var results = await client.SortedSetRangeByRankWithScoresAsync(destinationKey, 0, -1);
        Assert.Equal(2, results.Length);

        var palermoResult = results.FirstOrDefault(r => r.Element.ToString() == "Palermo");
        var cataniaResult = results.FirstOrDefault(r => r.Element.ToString() == "Catania");

        Assert.Equal(0.0, palermoResult.Score, 0.1);
        Assert.Equal(166.2742, cataniaResult.Score, 0.1);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_NonExistentMember_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));

        var shape = new GeoSearchCircle(100, GeoUnit.Kilometers);
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoSearchAsync(key, "NonExistentMember", shape));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");

        var position = new GeoPosition(13.361389, 38.115556);
        var shape = new GeoSearchCircle(100, GeoUnit.Kilometers);
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoSearchAsync(key, position, shape));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_NoMembersInArea_ReturnsEmpty(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        _ = await client.GeoAddAsync(key, "Palermo", new GeoPosition(13.361389, 38.115556));

        var position = new GeoPosition(0.0, 0.0); // Far from Palermo
        var shape = new GeoSearchCircle(1, GeoUnit.Meters); // Very small radius
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, position, shape);

        Assert.Empty(results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_NonExistentMember_ThrowsException(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string sourceKey = keyPrefix + ":source";
        string destinationKey = keyPrefix + ":dest";

        _ = await client.GeoAddAsync(sourceKey, "Palermo", new GeoPosition(13.361389, 38.115556));

        var shape = new GeoSearchCircle(100, GeoUnit.Kilometers);
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, "NonExistentMember", shape));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WrongKeyType_ThrowsException(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string sourceKey = keyPrefix + ":source";
        string destinationKey = keyPrefix + ":dest";

        await client.StringSetAsync(sourceKey, "not_a_geo_key");

        var position = new GeoPosition(13.361389, 38.115556);
        var shape = new GeoSearchCircle(100, GeoUnit.Kilometers);
        _ = await Assert.ThrowsAsync<Errors.RequestException>(async () =>
            await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, position, shape));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_WithCount_LimitsStoredResults(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string sourceKey = keyPrefix + ":source";
        string destinationKey = keyPrefix + ":dest";

        GeoEntry[] entries = [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania"),
            new GeoEntry(12.758489, 38.788135, "Trapani"),
            new GeoEntry(14.015482, 37.734741, "Enna")
        ];
        _ = await AddGeoEntriesAsync(client, sourceKey, entries);

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, "Palermo", shape, count: 2);

        Assert.Equal(2, count);

        ValkeyValue[] storedMembers = await client.SortedSetRangeByRankAsync(destinationKey, 0, -1);
        Assert.Equal(2, storedMembers.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_OverwritesDestination(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string sourceKey = keyPrefix + ":source";
        string destinationKey = keyPrefix + ":dest";

        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        _ = await AddGeoEntriesAsync(client, sourceKey, entries);

        _ = await client.SortedSetAddAsync(destinationKey, [new SortedSetEntry("OldMember", 100)]);
        Assert.Equal(1, await client.SortedSetCardAsync(destinationKey));

        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, "Palermo", shape);

        Assert.Equal(2, count);

        ValkeyValue[] storedMembers = await client.SortedSetRangeByRankAsync(destinationKey, 0, -1);
        Assert.Equal(2, storedMembers.Length);
        Assert.DoesNotContain("OldMember", storedMembers.Select(m => m.ToString()));
        Assert.Contains("Palermo", storedMembers.Select(m => m.ToString()));
        Assert.Contains("Catania", storedMembers.Select(m => m.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStore_NoResults_CreatesEmptyDestination(BaseClient client)
    {
        string keyPrefix = "{" + Guid.NewGuid().ToString() + "}";
        string sourceKey = keyPrefix + ":source";
        string destinationKey = keyPrefix + ":dest";

        _ = await client.GeoAddAsync(sourceKey, "Palermo", new GeoPosition(13.361389, 38.115556));

        var position = new GeoPosition(0.0, 0.0);
        var shape = new GeoSearchCircle(1, GeoUnit.Meters);
        long count = await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, position, shape);

        Assert.Equal(0, count);
        Assert.Equal(0, await client.SortedSetCardAsync(destinationKey));
        // Verify destination key exists but is empty - TypeAsync not available in BaseClient
    }
}
