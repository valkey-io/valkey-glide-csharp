// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Geospatial commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#geo">Valkey – Geospatial Commands</seealso>
public interface IGeospatialBaseCommands
{
    /// <summary>
    /// Returns the geohash string for a geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geohash/">Valkey commands – GEOHASH</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to get the geohash for.</param>
    /// <returns>The geohash string, or <see langword="null"/> if the member does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("mygeo", "Palermo", new GeoPosition(13.361389, 38.115556));
    /// var hash = await client.GeoHashAsync("mygeo", "Palermo");
    /// Console.WriteLine($"Geohash for member1: {hash}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member);

    /// <summary>
    /// Returns the geohash strings for geospatial members.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geohash/">Valkey commands – GEOHASH</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">The members to get the geohashes for.</param>
    /// <returns>An array with one geohash string per member, or <see langword="null"/> for members that do not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("mygeo", "Palermo", new GeoPosition(13.361389, 38.115556));
    /// await client.GeoAddAsync("mygeo", "Catania", new GeoPosition(15.087269, 37.502669));
    /// 
    /// var hashes = await client.GeoHashAsync("mygeo", ["Palermo", "Catania"]);
    /// Console.WriteLine($"Geohash for member2: {hashes[1]}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <summary>
    /// Returns the position of a geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geopos/">Valkey commands – GEOPOS</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to get the position for.</param>
    /// <returns>The position, or <see langword="null"/> if the member does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("mygeo", "Palermo", new GeoPosition(13.361389, 38.115556));
    /// var position = await client.GeoPositionAsync("mygeo", "Palermo");
    /// Console.WriteLine($"Palermo is at [{position!.Value.Latitude}, {position!.Value.Longitude}]");
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member);

    /// <summary>
    /// Returns the positions of geospatial members.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geopos/">Valkey commands – GEOPOS</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">The members to get the positions for.</param>
    /// <returns>An array with one position per member, or <see langword="null"/> for members that do not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("mygeo", "Palermo", new GeoPosition(12.3, 45.6));
    /// await client.GeoAddAsync("mygeo", "Catania", new GeoPosition(15.087269, 37.502669));
    /// 
    /// var positions = await client.GeoPositionAsync("mygeo", ["Palermo", "Catania"]);
    /// Console.WriteLine($"Catania is at [{positions[1]!.Value.Latitude}, {positions[1]!.Value.Longitude}]");
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);
}
