// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchGeospatialCommands
{
    /// <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, double, double, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, double, double, ValkeyValue)" /></returns>
    IBatch GeoAdd(ValkeyKey key, double longitude, double latitude, ValkeyValue member);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, GeoEntry)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, GeoEntry)" /></returns>
    IBatch GeoAdd(ValkeyKey key, GeoEntry value);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry})" /></returns>
    IBatch GeoAdd(ValkeyKey key, IEnumerable<GeoEntry> values);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, GeoEntry, GeoAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, GeoEntry, GeoAddOptions)" /></returns>
    IBatch GeoAdd(ValkeyKey key, GeoEntry value, GeoAddOptions options);

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry}, GeoAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialBaseCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry}, GeoAddOptions)" /></returns>
    IBatch GeoAdd(ValkeyKey key, IEnumerable<GeoEntry> values, GeoAddOptions options);

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
