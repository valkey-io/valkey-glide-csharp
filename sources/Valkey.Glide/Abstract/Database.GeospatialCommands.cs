// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Geospatial Commands with CommandFlags (SER Compatibility)

    public async Task<bool> GeoAddAsync(ValkeyKey key, double longitude, double latitude, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, longitude, latitude, member);
    }

    public async Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, value);
    }

    public async Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, values);
    }

    public async Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, GeoAddOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, value, options);
    }

    public async Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, GeoAddOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, values, options);
    }

    public async Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoDistanceAsync(key, member1, member2, unit);
    }

    public async Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoHashAsync(key, member);
    }

    public async Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoHashAsync(key, members);
    }

    public async Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoPositionAsync(key, member);
    }

    public async Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoPositionAsync(key, members);
    }

    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAsync(key, fromMember, shape, count, demandClosest, order, options);
    }

    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAsync(key, fromPosition, shape, count, demandClosest, order, options);
    }

    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAndStoreAsync(sourceKey, destinationKey, fromMember, shape, count, demandClosest, order, storeDistances);
    }

    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAndStoreAsync(sourceKey, destinationKey, fromPosition, shape, count, demandClosest, order, storeDistances);
    }

    #endregion
}
