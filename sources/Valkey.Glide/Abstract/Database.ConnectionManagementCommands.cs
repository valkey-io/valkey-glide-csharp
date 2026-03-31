// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.ClientGetNameAsync(CommandFlags)"/>
    public async Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ClientGetNameAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.ClientGetNameAsync(Route, CommandFlags)"/>
    public async Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ClientGetNameCluster(route), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.ClientIdAsync(CommandFlags)"/>
    public async Task<long> ClientIdAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ClientIdAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.ClientIdAsync(Route, CommandFlags)"/>
    public async Task<ClusterValue<long>> ClientIdAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ClientId().ToClusterValue(route is SingleNodeRoute), route);
    }
}
