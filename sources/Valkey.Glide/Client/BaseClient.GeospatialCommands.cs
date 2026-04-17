// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddCondition condition = GeoAddCondition.Always)
        => await GeoAddAsync(key, member, position, new GeoAddOptions { Condition = condition });

    /// <inheritdoc/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options)
        => await Command(Request.GeoAddAsync(key, member, position, options));

    /// <inheritdoc/>
    public async Task<long> GeoAddAsync(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddCondition condition = GeoAddCondition.Always)
        => await GeoAddAsync(key, members, new GeoAddOptions { Condition = condition });

    /// <inheritdoc/>
    public async Task<long> GeoAddAsync(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options)
        => await Command(Request.GeoAddAsync(key, members, options));

    /// <inheritdoc/>
    public async Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters)
        => await Command(Request.GeoDistanceAsync(key, member1, member2, unit));

    /// <inheritdoc/>
    public async Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member)
        => await Command(Request.GeoHashAsync(key, member));

    /// <inheritdoc/>
    public async Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => await Command(Request.GeoHashAsync(key, [.. members]));

    /// <inheritdoc/>
    public async Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member)
        => await Command(Request.GeoPositionAsync(key, member));

    /// <inheritdoc/>
    public async Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => await Command(Request.GeoPositionAsync(key, [.. members]));

    /// <inheritdoc/>
    public async Task<GeoSearchResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue from, GeoSearchShape shape, GeoSearchOptions options = default)
        => await Command(Request.GeoSearchAsync(key, from, shape, options));

    /// <inheritdoc/>
    public async Task<GeoSearchResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition from, GeoSearchShape shape, GeoSearchOptions options = default)
        => await Command(Request.GeoSearchAsync(key, from, shape, options));

    /// <inheritdoc/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue from, GeoSearchShape shape, GeoSearchStoreOptions options = default) =>
        await Command(Request.GeoSearchAndStoreAsync(source, destination, from, shape, options));

    /// <inheritdoc/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey source, ValkeyKey destination, GeoPosition from, GeoSearchShape shape, GeoSearchStoreOptions options = default) =>
        await Command(Request.GeoSearchAndStoreAsync(source, destination, from, shape, options));
}
