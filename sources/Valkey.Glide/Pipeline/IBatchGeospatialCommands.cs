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

    /// <inheritdoc cref="IGeospatialCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGeospatialCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit, CommandFlags)" /></returns>
    IBatch GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters);
}