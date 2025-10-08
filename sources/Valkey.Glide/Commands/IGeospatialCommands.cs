// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Geospatial Commands" group for standalone and cluster clients.
/// <br/>
/// See more on <see href="https://valkey.io/commands#geo">valkey.io</see>.
/// </summary>
public interface IGeospatialCommands
{
    /// <summary>
    /// Adds the specified geospatial items (longitude, latitude, name) to the specified key.
    /// If a member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="longitude">The longitude coordinate according to WGS84.</param>
    /// <param name="latitude">The latitude coordinate according to WGS84.</param>
    /// <param name="member">The name of the member to add.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns><see langword="true"/> if the member was added. <see langword="false"/> if the member was already a member of the sorted set and the score was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool wasAdded = await client.GeoAddAsync("mygeo", 13.361389, 38.115556, "Palermo");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> GeoAddAsync(ValkeyKey key, double longitude, double latitude, ValkeyValue member, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Adds the specified geospatial item to the specified key.
    /// If a member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="value">The geospatial item to add.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns><see langword="true"/> if the member was added. <see langword="false"/> if the member was already a member of the sorted set and the score was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entry = new GeoEntry(13.361389, 38.115556, "Palermo");
    /// bool wasAdded = await client.GeoAddAsync("mygeo", entry);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Adds the specified geospatial items to the specified key.
    /// If a member is already a member of the sorted set, the score is updated and the element reinserted at the right position to ensure the correct ordering.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">The geospatial items to add.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of elements added to the sorted set, not including elements already existing for which the score was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entries = new GeoEntry[]
    /// {
    ///     new GeoEntry(13.361389, 38.115556, "Palermo"),
    ///     new GeoEntry(15.087269, 37.502669, "Catania")
    /// };
    /// long addedCount = await client.GeoAddAsync("mygeo", entries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> GeoAddAsync(ValkeyKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the distance between two members in the geospatial index represented by the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geodist"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member1">The first member.</param>
    /// <param name="member2">The second member.</param>
    /// <param name="unit">The unit of distance (defaults to meters).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The distance between the two members in the specified unit. Returns <see langword="null"/> if one or both members are missing.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// double? distance = await client.GeoDistanceAsync("mygeo", "Palermo", "Catania", GeoUnit.Kilometers);
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None);
}