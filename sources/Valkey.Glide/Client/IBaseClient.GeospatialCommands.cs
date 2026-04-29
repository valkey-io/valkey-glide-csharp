// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Adds or updates a geospatial member in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/">Valkey commands – GEOADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member name to add or update.</param>
    /// <param name="position">The geographic position of the member.</param>
    /// <param name="condition">The condition under which to add or update the member.</param>
    /// <returns><see langword="true"/> if the member was added, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new GeoPosition(13.361389, 38.115556);
    /// var added = await client.GeoAddAsync("locations", "Palermo", position);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> GeoAddAsync(
        ValkeyKey key,
        ValkeyValue member,
        GeoPosition position,
        GeoAddCondition condition = GeoAddCondition.Always);

    /// <summary>
    /// Adds or updates a geospatial member in a sorted set with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/">Valkey commands – GEOADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member name to add or update.</param>
    /// <param name="position">The geographic position of the member.</param>
    /// <param name="options">The options for adding or updating the member.</param>
    /// <returns><see langword="true"/> if the member was added (or changed, if
    /// <see cref="GeoAddOptions.Changed"/> is set), <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new GeoPosition(13.361389, 38.115556);
    /// var options = new GeoAddOptions { Condition = GeoAddCondition.OnlyIfNotExists };
    /// var added = await client.GeoAddAsync("locations", "Palermo", position, options);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> GeoAddAsync(
        ValkeyKey key,
        ValkeyValue member,
        GeoPosition position,
        GeoAddOptions options);

    /// <summary>
    /// Adds or updates multiple geospatial members in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/">Valkey commands – GEOADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of member names and their geographic positions.</param>
    /// <param name="condition">The condition under which to add or update members.</param>
    /// <returns>The number of members added.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var members = new Dictionary&lt;ValkeyValue, GeoPosition&gt;
    /// {
    ///     ["Palermo"] = new GeoPosition(13.361389, 38.115556),
    ///     ["Catania"] = new GeoPosition(15.087269, 37.502669)
    /// };
    /// var count = await client.GeoAddAsync("locations", members);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> GeoAddAsync(
        ValkeyKey key,
        IDictionary<ValkeyValue, GeoPosition> members,
        GeoAddCondition condition = GeoAddCondition.Always);

    /// <summary>
    /// Adds or updates multiple geospatial members in a sorted set with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/">Valkey commands – GEOADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of member names and their geographic positions.</param>
    /// <param name="options">The options for adding or updating the members.</param>
    /// <returns>The number of members added (or changed, if
    /// <see cref="GeoAddOptions.Changed"/> is set).</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var members = new Dictionary&lt;ValkeyValue, GeoPosition&gt;
    /// {
    ///     ["Palermo"] = new GeoPosition(13.361389, 38.115556),
    ///     ["Catania"] = new GeoPosition(15.087269, 37.502669)
    /// };
    /// var options = new GeoAddOptions { Changed = true };
    /// var count = await client.GeoAddAsync("locations", members, options);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> GeoAddAsync(
        ValkeyKey key,
        IDictionary<ValkeyValue, GeoPosition> members,
        GeoAddOptions options);

    /// <summary>
    /// Returns the distance between two geospatial members.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geodist/">Valkey commands – GEODIST</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member1">The first member.</param>
    /// <param name="member2">The second member.</param>
    /// <param name="unit">The unit of distance measurement.</param>
    /// <returns>The distance between the two members in the specified unit,
    /// or <see langword="null"/> if one or both members do not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("locations", "Palermo", new GeoPosition(13.361389, 38.115556));
    /// await client.GeoAddAsync("locations", "Catania", new GeoPosition(15.087269, 37.502669));
    /// var distance = await client.GeoDistanceAsync("locations", "Palermo", "Catania", GeoUnit.Kilometers);  // ~166.27 km
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?> GeoDistanceAsync(
        ValkeyKey key,
        ValkeyValue member1,
        ValkeyValue member2,
        GeoUnit unit = GeoUnit.Meters);

    /// <summary>
    /// Searches for geospatial members within an area centered on another geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearch/">Valkey commands – GEOSEARCH</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="from">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="options">Optional search parameters.</param>
    /// <returns>An array of <see cref="GeoSearchResult"/> for matching members.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("locations", "Palermo", new GeoPosition(13.361389, 38.115556));
    /// await client.GeoAddAsync("locations", "Catania", new GeoPosition(15.087269, 37.502669));
    /// var results = await client.GeoSearchAsync("locations", "Palermo", new GeoSearchCircle(200, GeoUnit.Kilometers));  // 2 results
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoSearchResult[]> GeoSearchAsync(
        ValkeyKey key,
        ValkeyValue from,
        GeoSearchShape shape,
        GeoSearchOptions options = default);

    /// <summary>
    /// Searches for geospatial members within an area centered on a geographic position.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearch/">Valkey commands – GEOSEARCH</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="from">The position to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="options">Optional search parameters.</param>
    /// <returns>An array of <see cref="GeoSearchResult"/> for matching members.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("locations", "Palermo", new GeoPosition(13.361389, 38.115556));
    /// await client.GeoAddAsync("locations", "Catania", new GeoPosition(15.087269, 37.502669));
    ///
    /// var origin = new GeoPosition(15.0, 37.0);
    /// var circle = new GeoSearchCircle(200, GeoUnit.Kilometers);
    /// var results = await client.GeoSearchAsync("locations", origin, circle);  // 2 results
    /// </code>
    /// </example>
    /// </remarks>
    Task<GeoSearchResult[]> GeoSearchAsync(
        ValkeyKey key,
        GeoPosition from,
        GeoSearchShape shape,
        GeoSearchOptions options = default);

    /// <summary>
    /// Searches for geospatial members within an area centered on another
    /// geospatial member and stores the results in a destination sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearchstore/">Valkey commands – GEOSEARCHSTORE</seealso>
    /// <param name="source">The source sorted set key.</param>
    /// <param name="destination">The destination sorted set key.</param>
    /// <param name="from">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="options">Optional search and store parameters.</param>
    /// <returns>The number of elements stored in <paramref name="destination"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("locations", "Palermo", new GeoPosition(13.361389, 38.115556));
    /// await client.GeoAddAsync("locations", "Catania", new GeoPosition(15.087269, 37.502669));
    ///
    /// var circle = new GeoSearchCircle(200, GeoUnit.Kilometers);
    /// var stored = await client.GeoSearchAndStoreAsync("locations", "nearby", "Palermo", circle);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> GeoSearchAndStoreAsync(
        ValkeyKey source,
        ValkeyKey destination,
        ValkeyValue from,
        GeoSearchShape shape,
        GeoSearchStoreOptions options = default);

    /// <summary>
    /// Searches for geospatial members within an area centered on a geographic
    /// position and stores the results in a destination sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearchstore/">Valkey commands – GEOSEARCHSTORE</seealso>
    /// <param name="source">The source sorted set key.</param>
    /// <param name="destination">The destination sorted set key.</param>
    /// <param name="from">The position to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="options">Optional search and store parameters.</param>
    /// <returns>The number of elements stored in <paramref name="destination"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.GeoAddAsync("locations", "Palermo", new GeoPosition(13.361389, 38.115556));
    /// await client.GeoAddAsync("locations", "Catania", new GeoPosition(15.087269, 37.502669));
    ///
    /// var origin = new GeoPosition(15.0, 37.0);
    /// var circle = new GeoSearchCircle(200, GeoUnit.Kilometers);
    /// var stored = await client.GeoSearchAndStoreAsync("locations", "nearby", origin, circle);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> GeoSearchAndStoreAsync(
        ValkeyKey source,
        ValkeyKey destination,
        GeoPosition from,
        GeoSearchShape shape,
        GeoSearchStoreOptions options = default);
}
