// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Geospatial commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IGeospatialCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, double, double, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> GeoAddAsync(ValkeyKey key, double longitude, double latitude, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, longitude, latitude, member);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, value);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, values);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, GeoEntry, GeoAddOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, GeoAddOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, value, options);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoAddAsync(ValkeyKey, IEnumerable{GeoEntry}, GeoAddOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> GeoAddAsync(ValkeyKey key, IEnumerable<GeoEntry> values, GeoAddOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoAddAsync(key, values, options);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoDistanceAsync(ValkeyKey, ValkeyValue, ValkeyValue, GeoUnit)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoDistanceAsync(key, member1, member2, unit);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoHashAsync(key, member);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoHashAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<string?[]> GeoHashAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoHashAsync(key, members);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoPositionAsync(key, member);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoPositionAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoPositionAsync(key, members);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAsync(key, fromMember, shape, count, demandClosest, order, options);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAsync(ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, GeoRadiusOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, GeoRadiusOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAsync(key, fromPosition, shape, count, demandClosest, order, options);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, GeoSearchShape, long, bool, Order?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAndStoreAsync(sourceKey, destinationKey, fromMember, shape, count, demandClosest, order, storeDistances);
    }

    /// <inheritdoc cref="IGeospatialCommands.GeoSearchAndStoreAsync(ValkeyKey, ValkeyKey, GeoPosition, GeoSearchShape, long, bool, Order?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count, bool demandClosest, Order? order, bool storeDistances, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GeoSearchAndStoreAsync(sourceKey, destinationKey, fromPosition, shape, count, demandClosest, order, storeDistances);
    }
}
