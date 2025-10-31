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

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, GeoEntry, GeoAddOptions)" />
    public T GeoAdd(ValkeyKey key, GeoEntry value, GeoAddOptions options) => AddCmd(GeoAddAsync(key, value, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, GeoEntry[], GeoAddOptions)" />
    public T GeoAdd(ValkeyKey key, GeoEntry[] values, GeoAddOptions options) => AddCmd(GeoAddAsync(key, values, options));

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

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoSearch(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" />
    public T GeoSearch(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default) => AddCmd(GeoSearchAsync(key, fromMember, shape, count, demandClosest, order, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoSearch(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" />
    public T GeoSearch(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default) => AddCmd(GeoSearchAsync(key, fromPosition, shape, count, demandClosest, order, options));



    // Explicit interface implementations for IBatchGeospatialCommands
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, double longitude, double latitude, ValkeyValue member) => GeoAdd(key, longitude, latitude, member);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, GeoEntry value) => GeoAdd(key, value);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, GeoEntry[] values) => GeoAdd(key, values);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, GeoEntry value, GeoAddOptions options) => GeoAdd(key, value, options);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, GeoEntry[] values, GeoAddOptions options) => GeoAdd(key, values, options);
    IBatch IBatchGeospatialCommands.GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit) => GeoDistance(key, member1, member2, unit);
    IBatch IBatchGeospatialCommands.GeoHash(ValkeyKey key, ValkeyValue member) => GeoHash(key, member);
    IBatch IBatchGeospatialCommands.GeoHash(ValkeyKey key, ValkeyValue[] members) => GeoHash(key, members);
    IBatch IBatchGeospatialCommands.GeoPosition(ValkeyKey key, ValkeyValue member) => GeoPosition(key, member);
    IBatch IBatchGeospatialCommands.GeoPosition(ValkeyKey key, ValkeyValue[] members) => GeoPosition(key, members);
    IBatch IBatchGeospatialCommands.GeoSearch(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options) => GeoSearch(key, fromMember, shape, count, demandClosest, order, options);
    IBatch IBatchGeospatialCommands.GeoSearch(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options) => GeoSearch(key, fromPosition, shape, count, demandClosest, order, options);

}
