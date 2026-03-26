// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Geospatial commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IGeospatialCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, double, double, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> GeoAddAsync(ValkeyKey key, double longitude, double latitude, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry, GeoAddOptions)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, GeoAddOptions options, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry}, GeoAddOptions)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, GeoAddOptions options, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances, CommandFlags flags);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances, CommandFlags flags);
}
