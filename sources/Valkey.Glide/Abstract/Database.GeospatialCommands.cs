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

        // Map values to a dictionary with last-wins semantics.
        var members = new Dictionary<ValkeyValue, GeoPosition>();
        foreach (var entry in values)
        {
            members[entry.Member] = new GeoPosition(entry.Longitude, entry.Latitude);
        }

        return await GeoAddAsync(key, members);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit, CommandFlags)"/>
    public async Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await base.GeoDistanceAsync(key, member1, member2, unit); // Use 'base' to ensure correct resolution due to defaults.
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

    /// <inheritdoc cref="IDatabaseAsync.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, int, bool, Order?, GeoRadiusOptions, CommandFlags)"/>
    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var searchOptions = ToSearchOptions(options, order, count, demandClosest);
        var results = await GeoSearchAsync(key, member, shape, searchOptions);
        return [.. results.Select(r => new GeoRadiusResult(r.Member, r.Distance, r.Hash, r.Position))];
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoSearchAsync(ValkeyKey, double, double, GeoSearchShape, int, bool, Order?, GeoRadiusOptions, CommandFlags)"/>
    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var searchOptions = ToSearchOptions(options, order, count, demandClosest);
        var results = await GeoSearchAsync(key, new GeoPosition(longitude, latitude), shape, searchOptions);
        return [.. results.Select(r => new GeoRadiusResult(r.Member, r.Distance, r.Hash, r.Position))];
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, GeoSearchShape, int, bool, Order?, bool, CommandFlags)"/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue member, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var searchStoreOptions = ToSearchStoreOptions(order, count, demandClosest, storeDistances);
        return await GeoSearchAndStoreAsync(sourceKey, destinationKey, member, shape, searchStoreOptions);
    }

    /// <inheritdoc cref="IDatabaseAsync.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, double, double, GeoSearchShape, int, bool, Order?, bool, CommandFlags)"/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, double longitude, double latitude, GeoSearchShape shape, int count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var searchStoreOptions = ToSearchStoreOptions(order, count, demandClosest, storeDistances);
        return await GeoSearchAndStoreAsync(sourceKey, destinationKey, new GeoPosition(longitude, latitude), shape, searchStoreOptions);
    }

    private static GeoSearchOptions ToSearchOptions(GeoRadiusOptions options, Order? order, int count, bool demandClosest) =>
        new()
        {
            Order = order,
            Count = count > 0 ? count : null,
            Any = count > 0 && !demandClosest,
            WithPosition = (options & GeoRadiusOptions.WithCoordinates) != 0,
            WithDistance = (options & GeoRadiusOptions.WithDistance) != 0,
            WithHash = (options & GeoRadiusOptions.WithGeoHash) != 0,
        };

    private static GeoSearchStoreOptions ToSearchStoreOptions(Order? order, int count, bool demandClosest, bool storeDistances) =>
        new()
        {
            Order = order,
            Count = count > 0 ? count : null,
            Any = count > 0 && !demandClosest,
            StoreDistances = storeDistances,
        };

}
