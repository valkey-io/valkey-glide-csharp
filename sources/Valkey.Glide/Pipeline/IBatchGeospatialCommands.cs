// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchGeospatialCommands
{
    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, ValkeyValue, GeoPosition, GeoAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, ValkeyValue, GeoPosition, GeoAddOptions)" /></returns>
    IBatch GeoAdd(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options = default);

    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, IDictionary{ValkeyValue, GeoPosition}, GeoAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, IDictionary{ValkeyValue, GeoPosition}, GeoAddOptions)" /></returns>
    IBatch GeoAdd(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options = default);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit)" /></returns>
    IBatch GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoHashAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoHashAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch GeoHash(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoHashAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoHashAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch GeoHash(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoPositionAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoPositionAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch GeoPosition(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoPositionAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoPositionAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch GeoPosition(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" /></returns>
    IBatch GeoSearch(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" /></returns>
    IBatch GeoSearch(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default);


}
