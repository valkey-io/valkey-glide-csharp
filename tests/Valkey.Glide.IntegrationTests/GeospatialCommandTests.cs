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
}