// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Geospatial commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, ValkeyValue, GeoPosition, GeoAddOptions)" />
    public T GeoAdd(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options = default) => AddCmd(GeoAddAsync(key, member, position, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, IDictionary{ValkeyValue, GeoPosition}, GeoAddOptions)" />
    public T GeoAdd(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options = default) => AddCmd(GeoAddAsync(key, members, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoDistance(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit)" />
    public T GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters) => AddCmd(GeoDistanceAsync(key, member1, member2, unit));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoHash(ValkeyKey, ValkeyValue)" />
    public T GeoHash(ValkeyKey key, ValkeyValue member) => AddCmd(GeoHashAsync(key, member));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoHash(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T GeoHash(ValkeyKey key, IEnumerable<ValkeyValue> members) => AddCmd(GeoHashAsync(key, [.. members]));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoPosition(ValkeyKey, ValkeyValue)" />
    public T GeoPosition(ValkeyKey key, ValkeyValue member) => AddCmd(GeoPositionAsync(key, member));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoPosition(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T GeoPosition(ValkeyKey key, IEnumerable<ValkeyValue> members) => AddCmd(GeoPositionAsync(key, [.. members]));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoSearch(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" />
    public T GeoSearch(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default) => AddCmd(GeoSearchAsync(key, fromMember, shape, count, demandClosest, order, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoSearch(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)" />
    public T GeoSearch(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default) => AddCmd(GeoSearchAsync(key, fromPosition, shape, count, demandClosest, order, options));



    // Explicit interface implementations for IBatchGeospatialCommands
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options) => GeoAdd(key, member, position, options);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options) => GeoAdd(key, members, options);
    IBatch IBatchGeospatialCommands.GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit) => GeoDistance(key, member1, member2, unit);
    IBatch IBatchGeospatialCommands.GeoHash(ValkeyKey key, ValkeyValue member) => GeoHash(key, member);
    IBatch IBatchGeospatialCommands.GeoHash(ValkeyKey key, IEnumerable<ValkeyValue> members) => GeoHash(key, members);
    IBatch IBatchGeospatialCommands.GeoPosition(ValkeyKey key, ValkeyValue member) => GeoPosition(key, member);
    IBatch IBatchGeospatialCommands.GeoPosition(ValkeyKey key, IEnumerable<ValkeyValue> members) => GeoPosition(key, members);
    IBatch IBatchGeospatialCommands.GeoSearch(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options) => GeoSearch(key, fromMember, shape, count, demandClosest, order, options);
    IBatch IBatchGeospatialCommands.GeoSearch(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options) => GeoSearch(key, fromPosition, shape, count, demandClosest, order, options);

}
