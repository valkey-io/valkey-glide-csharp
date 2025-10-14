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
    /// Adds the specified geospatial item to the specified key with options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="value">The geospatial item to add.</param>
    /// <param name="options">The options for the GEOADD command.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of elements added or changed, depending on options.</returns>
    Task<long> GeoAddAsync(ValkeyKey key, GeoEntry value, GeoAddOptions options, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Adds the specified geospatial items to the specified key with options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">The geospatial items to add.</param>
    /// <param name="options">The options for the GEOADD command.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of elements added or changed, depending on options.</returns>
    Task<long> GeoAddAsync(ValkeyKey key, GeoEntry[] values, GeoAddOptions options, CommandFlags flags = CommandFlags.None);

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

    /// <summary>
    /// Returns the geohash string for a single member in the geospatial index represented by the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geohash"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to get the geohash for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The geohash string for the member. Returns <see langword="null"/> if the member is missing.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string? hash = await client.GeoHashAsync("mygeo", "Palermo");
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the geohash strings for multiple members in the geospatial index represented by the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geohash"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to get the geohashes for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of geohash strings for the members. Returns <see langword="null"/> for missing members.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string?[] hashes = await client.GeoHashAsync("mygeo", new ValkeyValue[] { "Palermo", "Catania" });
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?[]> GeoHashAsync(ValkeyKey key, ValkeyValue[] members, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the longitude and latitude for a single member in the geospatial index represented by the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geopos"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to get the position for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The longitude and latitude for the member. Returns <see langword="null"/> if the member is missing.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// GeoPosition? position = await client.GeoPositionAsync("mygeo", "Palermo");
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the longitude and latitude for multiple members in the geospatial index represented by the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geopos"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to get the positions for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of longitude and latitude for the members. Returns <see langword="null"/> for missing members.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// GeoPosition?[] positions = await client.GeoPositionAsync("mygeo", new ValkeyValue[] { "Palermo", "Catania" });
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, ValkeyValue[] members, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the members of a geospatial index which are within the borders of the area specified by a given shape.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearch"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="fromMember">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return. Use -1 for no limit.</param>
    /// <param name="demandClosest">When true, returns the closest results. When false, allows any results.</param>
    /// <param name="order">The order in which to return results. Null for default ordering.</param>
    /// <param name="options">The options for the search result format.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of results within the specified area.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
    /// GeoRadiusResult[] results = await client.GeoSearchAsync("mygeo", "Palermo", shape);
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the members of a geospatial index which are within the borders of the area specified by a given shape.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearch"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="fromPosition">The position to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return. Use -1 for no limit.</param>
    /// <param name="demandClosest">When true, returns the closest results. When false, allows any results.</param>
    /// <param name="order">The order in which to return results. Null for default ordering.</param>
    /// <param name="options">The options for the search result format.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of results within the specified area.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new GeoPosition(15.087269, 37.502669);
    /// var shape = new GeoSearchCircle(200, GeoUnit.Kilometers);
    /// GeoRadiusResult[] results = await client.GeoSearchAsync("mygeo", position, shape);
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None);



    Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None);

    Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None);
}