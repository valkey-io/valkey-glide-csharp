// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Server management commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IServerManagementCommands" />
/// <seealso cref="IServerManagementClusterCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IServerManagementCommands.EchoAsync(ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.EchoAsync(ValkeyValue, Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigGetAsync(ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigResetStatisticsAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task ConfigResetStatisticsAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigResetStatisticsAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task ConfigResetStatisticsAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigRewriteAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task ConfigRewriteAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigRewriteAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task ConfigRewriteAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigSetAsync(ValkeyValue, ValkeyValue, Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync(int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IServerManagementClusterCommands.DatabaseSizeAsync(Route, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ClusterValue<long>> DatabaseSizeAsync(Route route, int database, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task FlushAllDatabasesAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.FlushAllDatabasesAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task FlushAllDatabasesAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IServerManagementClusterCommands.FlushDatabaseAsync(Route, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task FlushDatabaseAsync(Route route, int database, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.LastSaveAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<DateTime> LastSaveAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.LastSaveAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.TimeAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<DateTime> TimeAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.TimeAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.LolwutAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<string> LolwutAsync(CommandFlags flags);

    /// <inheritdoc cref="IServerManagementClusterCommands.LolwutAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags);

    /// <inheritdoc cref="IServerManagementCommands.SelectAsync(long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task SelectAsync(long index, CommandFlags flags);
}
