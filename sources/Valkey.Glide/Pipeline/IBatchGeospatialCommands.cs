// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchGeospatialCommands
{
    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, double, double, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, double, double, ValkeyValue, CommandFlags)" /></returns>
    IBatch GeoAdd(ValkeyKey key, double longitude, double latitude, ValkeyValue member);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry, CommandFlags)" /></returns>
    IBatch GeoAdd(ValkeyKey key, GeoEntry value);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry[], CommandFlags)" /></returns>
    IBatch GeoAdd(ValkeyKey key, GeoEntry[] values);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry, GeoAddOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry, GeoAddOptions, CommandFlags)" /></returns>
    IBatch GeoAdd(ValkeyKey key, GeoEntry value, GeoAddOptions options);

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry[], GeoAddOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry[], GeoAddOptions, CommandFlags)" /></returns>
    IBatch GeoAdd(ValkeyKey key, GeoEntry[] values, GeoAddOptions options);

    /// <inheritdoc cref="IGeospatialCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit, CommandFlags)" /></returns>
    IBatch GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters);

    /// <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch GeoHash(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch GeoHash(ValkeyKey key, ValkeyValue[] members);

    /// <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch GeoPosition(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch GeoPosition(ValkeyKey key, ValkeyValue[] members);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions, CommandFlags)" /></returns>
    IBatch GeoSearch(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default);

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions, CommandFlags)" /></returns>
    IBatch GeoSearch(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default);


}
