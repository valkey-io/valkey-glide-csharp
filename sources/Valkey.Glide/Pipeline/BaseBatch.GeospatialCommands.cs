// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Geospatial commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, double, double, ValkeyValue)" />
    public T GeoAdd(ValkeyKey key, double longitude, double latitude, ValkeyValue member) => GeoAdd(key, new GeoEntry(longitude, latitude, member));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, GeoEntry)" />
    public T GeoAdd(ValkeyKey key, GeoEntry value) => AddCmd(GeoAddAsync(key, value));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, GeoEntry[])" />
    public T GeoAdd(ValkeyKey key, GeoEntry[] values) => AddCmd(GeoAddAsync(key, values));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoDistance(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit)" />
    public T GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters) => AddCmd(GeoDistanceAsync(key, member1, member2, unit));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoHash(ValkeyKey, ValkeyValue)" />
    public T GeoHash(ValkeyKey key, ValkeyValue member) => AddCmd(GeoHashAsync(key, member));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoHash(ValkeyKey, ValkeyValue[])" />
    public T GeoHash(ValkeyKey key, ValkeyValue[] members) => AddCmd(GeoHashAsync(key, members));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoPosition(ValkeyKey, ValkeyValue)" />
    public T GeoPosition(ValkeyKey key, ValkeyValue member) => AddCmd(GeoPositionAsync(key, member));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoPosition(ValkeyKey, ValkeyValue[])" />
    public T GeoPosition(ValkeyKey key, ValkeyValue[] members) => AddCmd(GeoPositionAsync(key, members));

    // Explicit interface implementations for IBatchGeospatialCommands
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, double longitude, double latitude, ValkeyValue member) => GeoAdd(key, longitude, latitude, member);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, GeoEntry value) => GeoAdd(key, value);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, GeoEntry[] values) => GeoAdd(key, values);
    IBatch IBatchGeospatialCommands.GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit) => GeoDistance(key, member1, member2, unit);
    IBatch IBatchGeospatialCommands.GeoHash(ValkeyKey key, ValkeyValue member) => GeoHash(key, member);
    IBatch IBatchGeospatialCommands.GeoHash(ValkeyKey key, ValkeyValue[] members) => GeoHash(key, members);
    IBatch IBatchGeospatialCommands.GeoPosition(ValkeyKey key, ValkeyValue member) => GeoPosition(key, member);
    IBatch IBatchGeospatialCommands.GeoPosition(ValkeyKey key, ValkeyValue[] members) => GeoPosition(key, members);
}