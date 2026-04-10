// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Geospatial commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#geo">Valkey – Geospatial Commands</seealso>
public interface IGeospatialBaseCommands
{
    /// <summary>
    /// Returns the distance between two geospatial members.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geodist/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member1">The first member.</param>
    /// <param name="member2">The second member.</param>
    /// <param name="unit">The unit of distance measurement.</param>
    /// <returns>The distance between the two members in the specified unit,
    /// or <see langword="null"/> if one or both members does not exist.</returns>
    Task<double?> GeoDistanceAsync(
        ValkeyKey key,
        ValkeyValue member1,
        ValkeyValue member2,
        GeoUnit unit = GeoUnit.Meters);

    /// <summary>
    /// Returns the geohash string for a geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geohash/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to get the geohash for.</param>
    /// <returns>The geohash string, or <see langword="null"/> if the member does not exist.</returns>
    Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="GeoHashAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">The members to get the geohashes for.</param>
    /// <returns>An array with one geohash string per member, or <see langword="null"/> if the member does not exist.</returns>
    Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <summary>
    /// Returns the position of a geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geopos/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to get the position for.</param>
    /// <returns>The position, or <see langword="null"/> if the member does not exist.</returns>
    Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="GeoPositionAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">The members to get the positions for.</param>
    /// <returns>An array with one position per member, or <see langword="null"/> if the member does not exist.</returns>
    Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members);
}
