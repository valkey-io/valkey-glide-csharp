// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

public class GeospatialCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_SingleEntry_ReturnsTrue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        double longitude = 13.361389;
        double latitude = 38.115556;
        string member = "Palermo";

        bool result = await client.GeoAddAsync(key, longitude, latitude, member);
        Assert.True(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_GeoEntry_ReturnsTrue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        var entry = new GeoEntry(13.361389, 38.115556, "Palermo");

        bool result = await client.GeoAddAsync(key, entry);
        Assert.True(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_MultipleEntries_ReturnsCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];

        long result = await client.GeoAddAsync(key, entries);
        Assert.Equal(2, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_DuplicateEntry_ReturnsFalse(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        double longitude = 13.361389;
        double latitude = 38.115556;
        string member = "Palermo";

        bool firstResult = await client.GeoAddAsync(key, longitude, latitude, member);
        bool secondResult = await client.GeoAddAsync(key, longitude, latitude, member);

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
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoAddAsync(key, -181, 0, member));

        // Test longitude too high (181)
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoAddAsync(key, 181, 0, member));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_InvalidLatitude_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string member = "InvalidPlace";

        // Test latitude too high (86)
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoAddAsync(key, 0, 86, member));

        // Test latitude too low (-86)
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoAddAsync(key, 0, -86, member));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_EmptyEntries_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        GeoEntry[] emptyEntries = [];

        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoAddAsync(key, emptyEntries));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoAdd_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");

        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoAddAsync(key, 13.361389, 38.115556, "Palermo"));
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

        await client.GeoAddAsync(key, entries);
        
        double? distance = await client.GeoDistanceAsync(key, "Palermo", "Catania", GeoUnit.Kilometers);
        Assert.NotNull(distance);
        Assert.True(distance > 160 && distance < 170); // Approximate distance between Palermo and Catania
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_NonExistentMember_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.GeoAddAsync(key, 13.361389, 38.115556, "Palermo");
        
        double? distance = await client.GeoDistanceAsync(key, "Palermo", "NonExistent");
        Assert.Null(distance);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistance_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");
        
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoDistanceAsync(key, "member1", "member2"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_SingleMember_ReturnsHash(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.GeoAddAsync(key, 13.361389, 38.115556, "Palermo");
        
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
        await client.GeoAddAsync(key, entries);
        
        string?[] hashes = await client.GeoHashAsync(key, new ValkeyValue[] { "Palermo", "Catania" });
        Assert.Equal(2, hashes.Length);
        Assert.NotNull(hashes[0]);
        Assert.NotNull(hashes[1]);
        Assert.NotEmpty(hashes[0]);
        Assert.NotEmpty(hashes[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_NonExistentMember_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.GeoAddAsync(key, 13.361389, 38.115556, "Palermo");
        
        string? hash = await client.GeoHashAsync(key, "NonExistent");
        Assert.Null(hash);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");
        
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoHashAsync(key, "member"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoHash_EmptyMembers_ReturnsEmptyArray(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.GeoAddAsync(key, 13.361389, 38.115556, "Palermo");
        
        string?[] hashes = await client.GeoHashAsync(key, new ValkeyValue[] { });
        Assert.Empty(hashes);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_SingleMember_ReturnsPosition(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        double longitude = 13.361389;
        double latitude = 38.115556;
        await client.GeoAddAsync(key, longitude, latitude, "Palermo");
        
        GeoPosition? position = await client.GeoPositionAsync(key, "Palermo");
        Assert.NotNull(position);
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
        await client.GeoAddAsync(key, entries);
        
        GeoPosition?[] positions = await client.GeoPositionAsync(key, new ValkeyValue[] { "Palermo", "Catania" });
        Assert.Equal(2, positions.Length);
        Assert.NotNull(positions[0]);
        Assert.NotNull(positions[1]);
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
        await client.GeoAddAsync(key, 13.361389, 38.115556, "Palermo");
        
        GeoPosition? position = await client.GeoPositionAsync(key, "NonExistent");
        Assert.Null(position);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoPosition_WrongKeyType_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.StringSetAsync(key, "not_a_geo_key");
        
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
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
        await client.GeoAddAsync(key, entries);
        
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
        await client.GeoAddAsync(key, entries);
        
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
        await client.GeoAddAsync(key, entries);
        
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
        await client.GeoAddAsync(key, entries);
        
        var position = new GeoPosition(13.361389, 38.115556); // Palermo coordinates
        var shape = new GeoSearchBox(400, 400, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, position, shape);
        
        Assert.NotEmpty(results);
        Assert.Contains("Palermo", results.Select(r => r.Member.ToString()));
        Assert.Contains("Catania", results.Select(r => r.Member.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_ByPolygon_ReturnsMembers(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version(9, 0, 0),
            "BYPOLYGON is only supported in Valkey 9.0.0+"
        );
        
        string key = Guid.NewGuid().ToString();
        GeoEntry[] entries =
        [
            new GeoEntry(13.361389, 38.115556, "Palermo"),
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        await client.GeoAddAsync(key, entries);
        
        var vertices = new GeoPosition[]
        {
            new GeoPosition(12.0, 37.0),
            new GeoPosition(16.0, 37.0),
            new GeoPosition(16.0, 39.0),
            new GeoPosition(12.0, 39.0)
        };
        var polygon = new GeoSearchPolygon(vertices);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, polygon);
        
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
        await client.GeoAddAsync(key, entries);
        
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
        await client.GeoAddAsync(key, entries);
        
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
        await client.GeoAddAsync(key, entries);
        
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
        await client.GeoAddAsync(key, entries);
        
        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        
        // Test with distance option
        GeoRadiusResult[] distResults = await client.GeoSearchAsync(key, "Palermo", shape, options: GeoRadiusOptions.WithDistance);
        Assert.NotEmpty(distResults);
        
        var palermoDist = distResults.FirstOrDefault(r => r.Member.ToString() == "Palermo");
        Assert.NotNull(palermoDist);
        Assert.Equal("Palermo", palermoDist.Member.ToString());
        Assert.NotNull(palermoDist.Distance); // Should have distance
        Assert.Equal(0.0, palermoDist.Distance.Value, 1); // Distance from itself should be ~0
        Assert.Null(palermoDist.Position); // Should be null without WithCoordinates
        
        // Test with coordinates option
        GeoRadiusResult[] coordResults = await client.GeoSearchAsync(key, "Palermo", shape, options: GeoRadiusOptions.WithCoordinates);
        Assert.NotEmpty(coordResults);
        
        var palermoCoord = coordResults.FirstOrDefault(r => r.Member.ToString() == "Palermo");
        Assert.NotNull(palermoCoord);
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
        await client.GeoAddAsync(key, entries);
        
        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        GeoRadiusResult[] results = await client.GeoSearchAsync(key, "Palermo", shape, options: GeoRadiusOptions.WithDistance);
        
        var palermoResult = results.FirstOrDefault(r => r.Member.ToString() == "Palermo");
        var cataniaResult = results.FirstOrDefault(r => r.Member.ToString() == "Catania");
        
        Assert.NotNull(palermoResult);
        Assert.NotNull(cataniaResult);
        Assert.NotNull(palermoResult.Distance);
        Assert.NotNull(cataniaResult.Distance);
        
        Assert.Equal(0.0, palermoResult.Distance.Value, 1); // Distance from itself should be ~0
        Assert.True(cataniaResult.Distance.Value > 160 && cataniaResult.Distance.Value < 170); // ~166km between cities
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
            new GeoEntry(15.087269, 37.502669, "Catania")
        ];
        await client.GeoAddAsync(sourceKey, entries);
        
        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, "Palermo", shape);
        
        Assert.True(count >= 1);
        Assert.Contains("Palermo", (await client.SortedSetRangeByRankAsync(destinationKey, 0, -1)).Select(r => r.ToString()));
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
        await client.GeoAddAsync(sourceKey, entries);
        
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
        await client.GeoAddAsync(sourceKey, entries);
        
        var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
        long count = await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, "Palermo", shape, storeDistances: true);
        
        Assert.True(count >= 1);
        var results = await client.SortedSetRangeByRankWithScoresAsync(destinationKey, 0, -1);
        Assert.NotEmpty(results);
        Assert.True(results.Any(r => r.Score >= 0)); // Distances should be non-negative
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_NonExistentMember_ThrowsException(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.GeoAddAsync(key, 13.361389, 38.115556, "Palermo");
        
        var shape = new GeoSearchCircle(100, GeoUnit.Kilometers);
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
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
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoSearchAsync(key, position, shape));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearch_NoMembersInArea_ReturnsEmpty(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.GeoAddAsync(key, 13.361389, 38.115556, "Palermo");
        
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
        
        await client.GeoAddAsync(sourceKey, 13.361389, 38.115556, "Palermo");
        
        var shape = new GeoSearchCircle(100, GeoUnit.Kilometers);
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
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
        await Assert.ThrowsAsync<Errors.RequestException>(async () => 
            await client.GeoSearchAndStoreAsync(sourceKey, destinationKey, position, shape));
    }
}