// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Geospatial commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, ValkeyValue, GeoPosition, GeoAddCondition)" />
    public T GeoAdd(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddCondition condition = GeoAddCondition.Always) => GeoAdd(key, member, position, new GeoAddOptions { Condition = condition });

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, ValkeyValue, GeoPosition, GeoAddOptions)" />
    public T GeoAdd(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options) => AddCmd(GeoAddAsync(key, member, position, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, IDictionary{ValkeyValue, GeoPosition}, GeoAddCondition)" />
    public T GeoAdd(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddCondition condition = GeoAddCondition.Always) => GeoAdd(key, members, new GeoAddOptions { Condition = condition });

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoAdd(ValkeyKey, IDictionary{ValkeyValue, GeoPosition}, GeoAddOptions)" />
    public T GeoAdd(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options) => AddCmd(GeoAddAsync(key, members, options));

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

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoSearch(ValkeyKey, ValkeyValue, GeoSearchShape, GeoSearchOptions)" />
    public T GeoSearch(ValkeyKey key, ValkeyValue from, GeoSearchShape shape, GeoSearchOptions options = default) => AddCmd(GeoSearchAsync(key, from, shape, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoSearch(ValkeyKey, GeoPosition, GeoSearchShape, GeoSearchOptions)" />
    public T GeoSearch(ValkeyKey key, GeoPosition from, GeoSearchShape shape, GeoSearchOptions options = default) => AddCmd(GeoSearchAsync(key, from, shape, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoSearchAndStore(ValkeyKey, ValkeyKey, ValkeyValue, GeoSearchShape, GeoSearchStoreOptions)" />
    public T GeoSearchAndStore(ValkeyKey source, ValkeyKey destination, ValkeyValue from, GeoSearchShape shape, GeoSearchStoreOptions options = default) => AddCmd(GeoSearchAndStoreAsync(source, destination, from, shape, options));

    /// <inheritdoc cref="IBatchGeospatialCommands.GeoSearchAndStore(ValkeyKey, ValkeyKey, GeoPosition, GeoSearchShape, GeoSearchStoreOptions)" />
    public T GeoSearchAndStore(ValkeyKey source, ValkeyKey destination, GeoPosition from, GeoSearchShape shape, GeoSearchStoreOptions options = default) => AddCmd(GeoSearchAndStoreAsync(source, destination, from, shape, options));

    // Explicit interface implementations for IBatchGeospatialCommands
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddCondition condition) => GeoAdd(key, member, position, condition);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options) => GeoAdd(key, member, position, options);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddCondition condition) => GeoAdd(key, members, condition);
    IBatch IBatchGeospatialCommands.GeoAdd(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options) => GeoAdd(key, members, options);
    IBatch IBatchGeospatialCommands.GeoDistance(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit) => GeoDistance(key, member1, member2, unit);
    IBatch IBatchGeospatialCommands.GeoHash(ValkeyKey key, ValkeyValue member) => GeoHash(key, member);
    IBatch IBatchGeospatialCommands.GeoHash(ValkeyKey key, IEnumerable<ValkeyValue> members) => GeoHash(key, members);
    IBatch IBatchGeospatialCommands.GeoPosition(ValkeyKey key, ValkeyValue member) => GeoPosition(key, member);
    IBatch IBatchGeospatialCommands.GeoPosition(ValkeyKey key, IEnumerable<ValkeyValue> members) => GeoPosition(key, members);
    IBatch IBatchGeospatialCommands.GeoSearch(ValkeyKey key, ValkeyValue from, GeoSearchShape shape, GeoSearchOptions options) => GeoSearch(key, from, shape, options);
    IBatch IBatchGeospatialCommands.GeoSearch(ValkeyKey key, GeoPosition from, GeoSearchShape shape, GeoSearchOptions options) => GeoSearch(key, from, shape, options);
    IBatch IBatchGeospatialCommands.GeoSearchAndStore(ValkeyKey source, ValkeyKey destination, ValkeyValue from, GeoSearchShape shape, GeoSearchStoreOptions options) => GeoSearchAndStore(source, destination, from, shape, options);
    IBatch IBatchGeospatialCommands.GeoSearchAndStore(ValkeyKey source, ValkeyKey destination, GeoPosition from, GeoSearchShape shape, GeoSearchStoreOptions options) => GeoSearchAndStore(source, destination, from, shape, options);

}
