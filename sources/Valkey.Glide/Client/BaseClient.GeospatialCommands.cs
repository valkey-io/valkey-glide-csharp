// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, ValkeyValue member, GeoPosition position, GeoAddOptions options = default)
        => await Command(Request.GeoAddAsync(key, member, position, options));

    /// <inheritdoc/>
    public async Task<long> GeoAddAsync(ValkeyKey key, IDictionary<ValkeyValue, GeoPosition> members, GeoAddOptions options = default)
        => await Command(Request.GeoAddAsync(key, members, options));

    /// <inheritdoc/>
    public async Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters)
    {
        return await Command(Request.GeoDistanceAsync(key, member1, member2, unit));
    }

    /// <inheritdoc/>
    public async Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member)
    {
        return await Command(Request.GeoHashAsync(key, member));
    }

    /// <inheritdoc/>
    public async Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
    {
        return await Command(Request.GeoHashAsync(key, [.. members]));
    }

    /// <inheritdoc/>
    public async Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member)
    {
        return await Command(Request.GeoPositionAsync(key, member));
    }

    /// <inheritdoc/>
    public async Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
    {
        return await Command(Request.GeoPositionAsync(key, [.. members]));
    }

    /// <inheritdoc/>
    public async Task<GeoSearchResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue from, GeoSearchShape shape, GeoSearchOptions options = default)
        => await Command(Request.GeoSearchAsync(key, from, shape, options));

    /// <inheritdoc/>
    public async Task<GeoSearchResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition from, GeoSearchShape shape, GeoSearchOptions options = default)
        => await Command(Request.GeoSearchAsync(key, from, shape, options));

    /// <inheritdoc/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false)
    {
        return await Command(Request.GeoSearchAndStoreAsync(sourceKey, destinationKey, fromMember, shape, count, demandClosest, order, storeDistances));
    }

    /// <inheritdoc/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false)
    {
        return await Command(Request.GeoSearchAndStoreAsync(sourceKey, destinationKey, fromPosition, shape, count, demandClosest, order, storeDistances));
    }
}
