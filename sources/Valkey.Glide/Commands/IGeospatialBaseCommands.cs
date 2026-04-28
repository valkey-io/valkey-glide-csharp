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
    /// var hash = await client.GeoHashAsync("mygeo", "member1");  // "sqc8b49rny0"
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="GeoHashAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">The members to get the geohashes for.</param>
    /// <returns>An array with one geohash string per member, or <see langword="null"/> for members that do not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var hashes = await client.GeoHashAsync("mygeo", ["member1", "member2"]);
    /// // Output: ["sqc8b49rny0", "sqdtr74hyu0"]
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
    /// var position = await client.GeoPositionAsync("mygeo", "member1");
    /// // position.Value.Longitude == 13.361, position.Value.Latitude == 38.115
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="GeoPositionAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">The members to get the positions for.</param>
    /// <returns>An array with one position per member, or <see langword="null"/> for members that do not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var positions = await client.GeoPositionAsync("mygeo", ["member1", "member2"]);
    /// // Output: [{ Longitude: 13.361, Latitude: 38.115 }, { Longitude: 15.087, Latitude: 37.502 }]
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);
}
