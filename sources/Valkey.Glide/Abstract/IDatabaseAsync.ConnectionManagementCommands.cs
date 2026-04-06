// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Connection management commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IConnectionManagementCommands" />
/// <seealso cref="IConnectionManagementClusterCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IConnectionManagementCommands.ClientIdAsync()"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ClientIdAsync(CommandFlags flags);

    /// <inheritdoc cref="IConnectionManagementClusterCommands.ClientIdAsync(Route)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ClusterValue<long>> ClientIdAsync(Route route, CommandFlags flags);
}
