// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

/// <summary>
/// Connection management commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IConnectionManagementCommands" />
/// <seealso cref="IConnectionManagementClusterCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IConnectionManagementCommands.ClientGetNameAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ClientGetNameAsync();
    }

    /// <inheritdoc cref="IConnectionManagementClusterCommands.ClientGetNameAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ClientGetNameCluster(route), route);
    }

    /// <inheritdoc cref="IConnectionManagementCommands.ClientIdAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ClientIdAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ClientIdAsync();
    }

    /// <inheritdoc cref="IConnectionManagementClusterCommands.ClientIdAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ClusterValue<long>> ClientIdAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ClientId().ToClusterValue(route is SingleNodeRoute), route);
    }
}
