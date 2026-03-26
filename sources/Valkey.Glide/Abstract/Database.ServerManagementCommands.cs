// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

/// <inheritdoc cref="IDatabaseAsync" />
internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.EchoAsync(ValkeyValue, CommandFlags)"/>
    public async Task<ValkeyValue> EchoAsync(ValkeyValue message, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await EchoAsync(message);
    }

    /// <inheritdoc cref="IDatabaseAsync.EchoAsync(ValkeyValue, Route, CommandFlags)"/>
    public async Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.Echo(message).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.ConfigGetAsync(ValkeyValue, CommandFlags)"/>
    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ConfigGetAsync(pattern);
    }

    /// <inheritdoc cref="IDatabaseAsync.ConfigGetAsync(ValkeyValue, Route, CommandFlags)"/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ConfigGetAsync(pattern).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.ConfigResetStatisticsAsync(CommandFlags)"/>
    public async Task ConfigResetStatisticsAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigResetStatisticsAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.ConfigResetStatisticsAsync(Route, CommandFlags)"/>
    public async Task ConfigResetStatisticsAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigResetStatisticsAsync(), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.ConfigRewriteAsync(CommandFlags)"/>
    public async Task ConfigRewriteAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigRewriteAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.ConfigRewriteAsync(Route, CommandFlags)"/>
    public async Task ConfigRewriteAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigRewriteAsync(), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.ConfigSetAsync(ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ConfigSetAsync(setting, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ConfigSetAsync(ValkeyValue, ValkeyValue, Route, CommandFlags)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.ConfigSetAsync(setting, value), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.DatabaseSizeAsync(int, CommandFlags)"/>
    public async Task<long> DatabaseSizeAsync(int database = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await DatabaseSizeAsync(database);
    }

    /// <inheritdoc cref="IDatabaseAsync.DatabaseSizeAsync(Route, int, CommandFlags)"/>
    public async Task<ClusterValue<long>> DatabaseSizeAsync(Route route, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.DatabaseSizeAsync(database).ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.FlushAllDatabasesAsync(CommandFlags)"/>
    public async Task FlushAllDatabasesAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await FlushAllDatabasesAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.FlushAllDatabasesAsync(Route, CommandFlags)"/>
    public async Task FlushAllDatabasesAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FlushAllDatabasesAsync(), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.FlushDatabaseAsync(int, CommandFlags)"/>
    public async Task FlushDatabaseAsync(int database = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await FlushDatabaseAsync(database);
    }

    /// <inheritdoc cref="IDatabaseAsync.FlushDatabaseAsync(Route, int, CommandFlags)"/>
    public async Task FlushDatabaseAsync(Route route, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.FlushDatabaseAsync(database), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.LastSaveAsync(CommandFlags)"/>
    public async Task<DateTime> LastSaveAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await LastSaveAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.LastSaveAsync(Route, CommandFlags)"/>
    public async Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.LastSaveAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.TimeAsync(CommandFlags)"/>
    public async Task<DateTime> TimeAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await TimeAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.TimeAsync(Route, CommandFlags)"/>
    public async Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.TimeAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.LolwutAsync(CommandFlags)"/>
    public async Task<string> LolwutAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await LolwutAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.LolwutAsync(Route, CommandFlags)"/>
    public async Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.LolwutAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.SelectAsync(long, CommandFlags)"/>
    public async Task SelectAsync(long index, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await SelectAsync(index);
    }
}
