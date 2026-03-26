// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

internal partial class Database
{
    #region Server Management Commands with CommandFlags (SER Compatibility)

    public async Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await EchoAsync(message);
    }

    public async Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.Echo(message).ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ConfigGetAsync(pattern);
    }

    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ConfigGetAsync(pattern).ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task ConfigResetStatisticsAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigResetStatisticsAsync();
    }

    public async Task ConfigResetStatisticsAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigResetStatisticsAsync(), route);
    }

    public async Task ConfigRewriteAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigRewriteAsync();
    }

    public async Task ConfigRewriteAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigRewriteAsync(), route);
    }

    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigSetAsync(setting, value);
    }

    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigSetAsync(setting, value), route);
    }

    public async Task<long> DatabaseSizeAsync(int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await DatabaseSizeAsync(database);
    }

    public async Task<ClusterValue<long>> DatabaseSizeAsync(Route route, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.DatabaseSizeAsync(database).ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task FlushAllDatabasesAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await FlushAllDatabasesAsync();
    }

    public async Task FlushAllDatabasesAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FlushAllDatabasesAsync(), route);
    }

    public async Task FlushDatabaseAsync(int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await FlushDatabaseAsync(database);
    }

    public async Task FlushDatabaseAsync(Route route, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FlushDatabaseAsync(database), route);
    }

    public async Task<DateTime> LastSaveAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await LastSaveAsync();
    }

    public async Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.LastSaveAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task<DateTime> TimeAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await TimeAsync();
    }

    public async Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.TimeAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task<string> LolwutAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await LolwutAsync();
    }

    public async Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.LolwutAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    public async Task SelectAsync(long index, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await SelectAsync(index);
    }

    #endregion
}
