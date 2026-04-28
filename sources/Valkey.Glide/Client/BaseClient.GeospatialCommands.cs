// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, ValkeyValue, GeoPosition, GeoAddCondition)"/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddCondition condition = GeoAddCondition.Always)
        => await GeoAddAsync(key, member, position, new GeoAddOptions { Condition = condition });

    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, ValkeyValue, GeoPosition, GeoAddOptions)"/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options)
        => await Command(Request.GeoAddAsync(key, member, position, options));

    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, IDictionary{ValkeyValue, GeoPosition}, GeoAddCondition)"/>
    public async Task<long> GeoAddAsync(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddCondition condition = GeoAddCondition.Always)
        => await GeoAddAsync(key, members, new GeoAddOptions { Condition = condition });

    /// <inheritdoc cref="IBaseClient.GeoAddAsync(ValkeyKey, IDictionary{ValkeyValue, GeoPosition}, GeoAddOptions)"/>
    public async Task<long> GeoAddAsync(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options)
        => await Command(Request.GeoAddAsync(key, members, options));

    /// <inheritdoc/>
    public async Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters)
        => await Command(Request.GeoDistanceAsync(key, member1, member2, unit));

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoHashAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member)
        => await Command(Request.GeoHashAsync(key, member));

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoHashAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => await Command(Request.GeoHashAsync(key, [.. members]));

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoPositionAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member)
        => await Command(Request.GeoPositionAsync(key, member));

    /// <inheritdoc cref="IGeospatialBaseCommands.GeoPositionAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => await Command(Request.GeoPositionAsync(key, [.. members]));

    /// <inheritdoc cref="IBaseClient.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, GeoSearchOptions)"/>
    public async Task<GeoSearchResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue from, GeoSearchShape shape, GeoSearchOptions options = default)
        => await Command(Request.GeoSearchAsync(key, from, shape, options));

    /// <inheritdoc cref="IBaseClient.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, GeoSearchOptions)"/>
    public async Task<GeoSearchResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition from, GeoSearchShape shape, GeoSearchOptions options = default)
        => await Command(Request.GeoSearchAsync(key, from, shape, options));

    /// <inheritdoc cref="IBaseClient.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, GeoSearchShape, GeoSearchStoreOptions)"/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue from, GeoSearchShape shape, GeoSearchStoreOptions options = default) =>
        await Command(Request.GeoSearchAndStoreAsync(source, destination, from, shape, options));

    /// <inheritdoc cref="IBaseClient.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, GeoPosition, GeoSearchShape, GeoSearchStoreOptions)"/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey source, ValkeyKey destination, GeoPosition from, GeoSearchShape shape, GeoSearchStoreOptions options = default) =>
        await Command(Request.GeoSearchAndStoreAsync(source, destination, from, shape, options));
}
