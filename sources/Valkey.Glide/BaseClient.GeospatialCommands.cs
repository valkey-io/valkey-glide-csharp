// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IGeospatialCommands
{
    /// <inheritdoc/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, double longitude, double latitude, ValkeyValue member, CommandFlags flags = CommandFlags.None)
    {
        return await GeoAddAsync(key, new GeoEntry(longitude, latitude, member), flags).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<bool> GeoAddAsync(ValkeyKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoAddAsync(key, value));
    }

    /// <inheritdoc/>
    public async Task<long> GeoAddAsync(ValkeyKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoAddAsync(key, values));
    }

    /// <inheritdoc/>
    public async Task<long> GeoAddAsync(ValkeyKey key, GeoEntry value, GeoAddOptions options, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoAddAsync(key, value, options));
    }

    /// <inheritdoc/>
    public async Task<long> GeoAddAsync(ValkeyKey key, GeoEntry[] values, GeoAddOptions options, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoAddAsync(key, values, options));
    }

    /// <inheritdoc/>
    public async Task<double?> GeoDistanceAsync(ValkeyKey key, ValkeyValue member1, ValkeyValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoDistanceAsync(key, member1, member2, unit));
    }

    /// <inheritdoc/>
    public async Task<string?> GeoHashAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoHashAsync(key, member));
    }

    /// <inheritdoc/>
    public async Task<string?[]> GeoHashAsync(ValkeyKey key, ValkeyValue[] members, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoHashAsync(key, members));
    }

    /// <inheritdoc/>
    public async Task<GeoPosition?> GeoPositionAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoPositionAsync(key, member));
    }

    /// <inheritdoc/>
    public async Task<GeoPosition?[]> GeoPositionAsync(ValkeyKey key, ValkeyValue[] members, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoPositionAsync(key, members));
    }

    /// <inheritdoc/>
    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoSearchAsync(key, fromMember, shape, count, demandClosest, order, options));
    }

    /// <inheritdoc/>
    public async Task<GeoRadiusResult[]> GeoSearchAsync(ValkeyKey key, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoSearchAsync(key, fromPosition, shape, count, demandClosest, order, options));
    }

    /// <inheritdoc/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue fromMember, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoSearchAndStoreAsync(sourceKey, destinationKey, fromMember, shape, count, demandClosest, order, storeDistances));
    }

    /// <inheritdoc/>
    public async Task<long> GeoSearchAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, GeoPosition fromPosition, GeoSearchShape shape, long count = -1, bool demandClosest = true, Order? order = null, bool storeDistances = false, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GeoSearchAndStoreAsync(sourceKey, destinationKey, fromPosition, shape, count, demandClosest, order, storeDistances));
    }
}