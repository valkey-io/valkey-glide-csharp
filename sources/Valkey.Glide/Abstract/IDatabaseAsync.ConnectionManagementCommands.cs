// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Connection management commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IConnectionManagementCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IConnectionManagementCommands.ClientGetNameAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> ClientGetNameAsync(CommandFlags flags);

    /// <inheritdoc cref="IConnectionManagementClusterCommands.ClientGetNameAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IConnectionManagementCommands.ClientIdAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> ClientIdAsync(CommandFlags flags);

    /// <inheritdoc cref="IConnectionManagementClusterCommands.ClientIdAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ClusterValue<long>> ClientIdAsync(Route route, CommandFlags flags);
}
