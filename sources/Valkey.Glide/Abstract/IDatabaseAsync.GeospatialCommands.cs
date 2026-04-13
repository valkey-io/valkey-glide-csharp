// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Geospatial commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IGeospatialBaseCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, ValkeyValue, GeoPosition, GeoAddOptions)" path="/*[self::summary or self::seealso or self::returns]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="longitude">The longitude coordinate.</param>
    /// <param name="latitude">The latitude coordinate.</param>
    /// <param name="member">The name of the member to add.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> GeoAddAsync(ValkeyKey key, double longitude, double latitude, ValkeyValue member, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, ValkeyValue, GeoPosition, GeoAddOptions)" path="/*[self::summary or self::seealso or self::returns]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="value">The geospatial entry to add.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, IDictionary{ValkeyValue, GeoPosition}, GeoAddOptions)" path="/*[self::summary or self::seealso or self::returns]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="values">The geospatial entries to add.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoHashAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoHashAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoPositionAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoPositionAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, GeoSearchOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return. Use -1 for no limit.</param>
    /// <param name="demandClosest">When <see langword="true"/>, returns the closest results.</param>
    /// <param name="order">The sort order for results, or <see langword="null"/> for server default.</param>
    /// <param name="options">The result fields to include.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="GeoRadiusResult"/> for matching members.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, GeoSearchOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="longitude">The longitude of the search origin.</param>
    /// <param name="latitude">The latitude of the search origin.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to return. Use -1 for no limit.</param>
    /// <param name="demandClosest">When <see langword="true"/>, returns the closest results.</param>
    /// <param name="order">The sort order for results, or <see langword="null"/> for server default.</param>
    /// <param name="options">The result fields to include.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="GeoRadiusResult"/> for matching members.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, GeoSearchShape, GeoSearchStoreOptions)" path="/*[self::summary or self::seealso or self::returns]"/>
    /// <param name="sourceKey">The source sorted set key.</param>
    /// <param name="destinationKey">The destination sorted set key.</param>
    /// <param name="member">The member to search from.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to store. Use -1 for no limit.</param>
    /// <param name="demandClosest">When <see langword="true"/>, stores the closest results.</param>
    /// <param name="order">The sort order for results, or <see langword="null"/> for server default.</param>
    /// <param name="storeDistances">When <see langword="true"/>, stores distances as scores instead of geohash values.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, GeoPosition, GeoSearchShape, GeoSearchStoreOptions)" path="/*[self::summary or self::seealso or self::returns]"/>
    /// <param name="sourceKey">The source sorted set key.</param>
    /// <param name="destinationKey">The destination sorted set key.</param>
    /// <param name="longitude">The longitude of the search origin.</param>
    /// <param name="latitude">The latitude of the search origin.</param>
    /// <param name="shape">The search area shape.</param>
    /// <param name="count">The maximum number of results to store. Use -1 for no limit.</param>
    /// <param name="demandClosest">When <see langword="true"/>, stores the closest results.</param>
    /// <param name="order">The sort order for results, or <see langword="null"/> for server default.</param>
    /// <param name="storeDistances">When <see langword="true"/>, stores distances as scores instead of geohash values.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None);
}
