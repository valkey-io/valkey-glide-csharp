// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Adds or changes a geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member name to add or change.</param>
    /// <param name="position">The position of the member.</param>
    /// <param name="condition">The condition under which to add or update the member.</param>
    /// <returns><see langword="true"/> if the member was added, <see langword="false"/> otherwise.</returns>
    Task<bool> GeoAddAsync(
        ValkeyKey key,
        ValkeyValue member,
        GeoPosition position,
        GeoAddCondition condition = GeoAddCondition.Always);

    /// <summary>
    /// Adds or changes a geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member name to add or change.</param>
    /// <param name="position">The position of the member.</param>
    /// <param name="options">The options for adding or changing the member.</param>
    /// <returns><see langword="true"/> if the member was added (or changed, if
    /// <see cref="GeoAddOptions.Changed"/> is set), <see langword="false"/> otherwise.</returns>
    Task<bool> GeoAddAsync(
        ValkeyKey key,
        ValkeyValue member,
        GeoPosition position,
        GeoAddOptions options);

    /// <summary>
    /// Adds or changes geospatial members.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of member names and positions.</param>
    /// <param name="condition">The condition under which to add or update members.</param>
    /// <returns>The number of members added.</returns>
    Task<long> GeoAddAsync(
        ValkeyKey key,
        IDictionary<ValkeyValue, GeoPosition> members,
        GeoAddCondition condition = GeoAddCondition.Always);

    /// <summary>
    /// Adds or changes geospatial members.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geoadd/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of member names and positions.</param>
    /// <param name="options">The options for adding or changing the members.</param>
    /// <returns>The number of members added (or changed, if
    /// <see cref="GeoAddOptions.Changed"/> is set).</returns>
    Task<long> GeoAddAsync(
        ValkeyKey key,
        IDictionary<ValkeyValue, GeoPosition> members,
        GeoAddOptions options);

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
    /// Searches for geospatial members within an area centered on another geospatial member.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearch/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="from">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="options">Optional search parameters.</param>
    /// <returns>An array of <see cref="GeoSearchResult"/> for matching members.</returns>
    Task<GeoSearchResult[]> GeoSearchAsync(
        ValkeyKey key,
        ValkeyValue from,
        GeoSearchShape shape,
        GeoSearchOptions options = default);

    /// <summary>
    /// Searches for geospatial members within an area centered on a geospatial position.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearch/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="from">The position to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="options">Optional search parameters.</param>
    /// <returns>An array of <see cref="GeoSearchResult"/> for matching members.</returns>
    Task<GeoSearchResult[]> GeoSearchAsync(
        ValkeyKey key,
        GeoPosition from,
        GeoSearchShape shape,
        GeoSearchOptions options = default);

    /// <summary>
    /// Searches for geospatial members within an area centered on another
    /// geospatial member and stores the results to a destination sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearchstore/"/>
    /// <param name="source">The source sorted set key.</param>
    /// <param name="destination">The destination sorted set key.</param>
    /// <param name="from">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="options">Optional search and store parameters.</param>
    /// <returns>The number of elements stored.</returns>
    Task<long> GeoSearchAndStoreAsync(
        ValkeyKey source,
        ValkeyKey destination,
        ValkeyValue from,
        GeoSearchShape shape,
        GeoSearchStoreOptions options = default);

    /// <summary>
    /// Searches for geospatial members within an area centered on a geospatial
    /// position and stores the results to a destination sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/geosearchstore/"/>
    /// <param name="source">The source sorted set key.</param>
    /// <param name="destination">The destination sorted set key.</param>
    /// <param name="from">The position to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="options">Optional search and store parameters.</param>
    /// <returns>The number of elements stored.</returns>
    Task<long> GeoSearchAndStoreAsync(
        ValkeyKey source,
        ValkeyKey destination,
        GeoPosition from,
        GeoSearchShape shape,
        GeoSearchStoreOptions options = default);
}
