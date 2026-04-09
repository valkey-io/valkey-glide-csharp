// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.GeoAddAsync(ValkeyKey, double, double, ValkeyValue, CommandFlags)"/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, double longitude, double latitude, ValkeyValue member, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, member, new GeoPosition(longitude, latitude));
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoAddAsync(ValkeyKey, GeoEntry, CommandFlags)"/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, value.Member, new GeoPosition(value.Longitude, value.Latitude));
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry}, CommandFlags)"/>
    public async Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var members = values.ToDictionary(e => e.Member, e => new GeoPosition(e.Longitude, e.Latitude));
        return await GeoAddAsync(key, members);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit, CommandFlags)"/>
    public async Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoDistanceAsync(key, member1, member2, unit);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoHashAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoHashAsync(key, member);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoHashAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoHashAsync(key, members);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoPositionAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoPositionAsync(key, member);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoPositionAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoPositionAsync(key, members);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions, CommandFlags)"/>
    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAsync(key, fromMember, shape, count, demandClosest, order, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions, CommandFlags)"/>
    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAsync(key, fromPosition, shape, count, demandClosest, order, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, bool, CommandFlags)"/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAndStoreAsync(sourceKey, destinationKey, fromMember, shape, count, demandClosest, order, storeDistances);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, bool, CommandFlags)"/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAndStoreAsync(sourceKey, destinationKey, fromPosition, shape, count, demandClosest, order, storeDistances);
    }
}
