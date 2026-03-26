// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

/// <summary>
/// Server management commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IServerManagementCommands" />
/// <seealso cref="IServerManagementClusterCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IServerManagementCommands.EchoAsync(ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await EchoAsync(message);
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.EchoAsync(ValkeyValue, Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.Echo(message).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ConfigGetAsync(pattern);
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigGetAsync(ValkeyValue, Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ConfigGetAsync(pattern).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.ConfigResetStatisticsAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task ConfigResetStatisticsAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigResetStatisticsAsync();
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigResetStatisticsAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task ConfigResetStatisticsAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigResetStatisticsAsync(), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.ConfigRewriteAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task ConfigRewriteAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigRewriteAsync();
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigRewriteAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task ConfigRewriteAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigRewriteAsync(), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigSetAsync(setting, value);
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.ConfigSetAsync(ValkeyValue, ValkeyValue, Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigSetAsync(setting, value), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync(int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> DatabaseSizeAsync(int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await DatabaseSizeAsync(database);
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.DatabaseSizeAsync(Route, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ClusterValue<long>> DatabaseSizeAsync(Route route, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.DatabaseSizeAsync(database).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task FlushAllDatabasesAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await FlushAllDatabasesAsync();
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.FlushAllDatabasesAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task FlushAllDatabasesAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FlushAllDatabasesAsync(), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task FlushDatabaseAsync(int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await FlushDatabaseAsync(database);
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.FlushDatabaseAsync(Route, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task FlushDatabaseAsync(Route route, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FlushDatabaseAsync(database), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.LastSaveAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<DateTime> LastSaveAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await LastSaveAsync();
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.LastSaveAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.LastSaveAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.TimeAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<DateTime> TimeAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await TimeAsync();
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.TimeAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.TimeAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.LolwutAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<string> LolwutAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await LolwutAsync();
    }

    /// <inheritdoc cref="IServerManagementClusterCommands.LolwutAsync(Route)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.LolwutAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IServerManagementCommands.SelectAsync(long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task SelectAsync(long index, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await SelectAsync(index);
    }
}
