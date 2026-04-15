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
