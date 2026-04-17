// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

internal partial class Database
{
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

    /// <inheritdoc cref="IDatabaseAsync.LastSaveAsync(CommandFlags)"/>
    public Task<DateTime> LastSaveAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return LastSaveAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.LastSaveAsync(Route, CommandFlags)"/>
    public async Task<ClusterValue<DateTime>> LastSaveAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.LastSaveAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.TimeAsync(CommandFlags)"/>
    public Task<DateTime> TimeAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return TimeAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.TimeAsync(Route, CommandFlags)"/>
    public async Task<ClusterValue<DateTime>> TimeAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.TimeAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

    /// <inheritdoc cref="IDatabaseAsync.LolwutAsync(CommandFlags)"/>
    public Task<string> LolwutAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return LolwutAsync();
    }

    /// <inheritdoc cref="IDatabaseAsync.LolwutAsync(Route, CommandFlags)"/>
    public async Task<ClusterValue<string>> LolwutAsync(Route route, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.LolwutAsync().ToClusterValue(route is SingleNodeRoute), route);
    }

}
