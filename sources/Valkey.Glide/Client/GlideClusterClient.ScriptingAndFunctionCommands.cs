// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public sealed partial class GlideClusterClient
{
    // ===== Script Execution with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.ScriptInvokeAsync(Script, ClusterScriptOptions, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> ScriptInvokeAsync(
        Script script,
        ClusterScriptOptions options,
        CancellationToken cancellationToken = default)
    {
        // Determine the route - use provided route or default to AllPrimaries
        Route route = options.Route ?? Route.AllPrimaries;

        // Determine if this is a single-node route
        bool isSingleNode = route is Route.SingleNodeRoute;

        // Create the EVALSHA command with cluster value support
        var cmd = Request.EvalShaAsync(script.Hash, null, options.Args).ToClusterValue(isSingleNode);

        return await Command(cmd, route);
    }

    // ===== Script Management with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.ScriptExistsAsync(IEnumerable{string}, Route, CancellationToken)"/>
    public async Task<ClusterValue<bool[]>> ScriptExistsAsync(
        IEnumerable<string> sha1Hashes,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.ScriptExistsAsync([.. sha1Hashes]).ToClusterValue(route), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.ScriptFlushAsync(Route, CancellationToken)"/>
    public async Task ScriptFlushAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptFlushAsync(), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.ScriptFlushAsync(FlushMode, Route, CancellationToken)"/>
    public async Task ScriptFlushAsync(
        FlushMode mode,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptFlushAsync(mode), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.ScriptKillAsync(Route, CancellationToken)"/>
    public async Task ScriptKillAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptKillAsync(), route);
    }

    // ===== Function Execution with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.FCallAsync(string, Route, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallAsync(function, null, null).ToClusterValue(route), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.FCallAsync(string, IEnumerable{string}, Route, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        IEnumerable<string> args,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallAsync(function, null, [.. args]).ToClusterValue(route), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.FCallReadOnlyAsync(string, Route, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallReadOnlyAsync(function, null, null).ToClusterValue(route), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.FCallReadOnlyAsync(string, IEnumerable{string}, Route, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        IEnumerable<string> args,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallReadOnlyAsync(function, null, [.. args]).ToClusterValue(route), route);
    }

    // ===== Function Management with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.FunctionLoadAsync(string, Route, CancellationToken)"/>
    public async Task<ClusterValue<string>> FunctionLoadAsync(
        string libraryCode,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionLoadAsync(libraryCode, false).ToClusterValue(route), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionLoadAsync(string, bool, Route, CancellationToken)"/>
    public async Task<ClusterValue<string>> FunctionLoadAsync(
        string libraryCode,
        bool replace,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionLoadAsync(libraryCode, replace).ToClusterValue(route), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionDeleteAsync(string, Route, CancellationToken)"/>
    public async Task FunctionDeleteAsync(
        string libraryName,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionDeleteAsync(libraryName), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionFlushAsync(Route, CancellationToken)"/>
    public async Task FunctionFlushAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionFlushAsync(), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionFlushAsync(FlushMode, Route, CancellationToken)"/>
    public async Task FunctionFlushAsync(
        FlushMode mode,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionFlushAsync(mode), route);
    }

    /// <inheritdoc cref="IBaseClient.FunctionKillAsync(CancellationToken)"/>
    public new async Task FunctionKillAsync(
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionKillAsync(), Route.AllPrimaries);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionKillAsync(Route, CancellationToken)"/>
    public async Task FunctionKillAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionKillAsync(), route);
    }

    // ===== Function Inspection with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.FunctionListAsync(FunctionListOptions?, CancellationToken)"/>
    public async Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionListAsync(options).ToClusterValue(Route.AllPrimaries), Route.AllPrimaries);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionListAsync(FunctionListOptions?, Route, CancellationToken)"/>
    public async Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListOptions? options,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionListAsync(options).ToClusterValue(route), route);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionStatsAsync(Route, CancellationToken)"/>
    public async Task<ClusterValue<FunctionStatsResult>> FunctionStatsAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionStatsAsync().ToClusterValue(route), route);
    }

    // ===== Function Persistence with Routing =====

    /// <inheritdoc cref="IBaseClient.FunctionDumpAsync(CancellationToken)"/>
    public new async Task<byte[]> FunctionDumpAsync(
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionDumpAsync());
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionDumpAsync(Route, CancellationToken)"/>
    public async Task<ClusterValue<byte[]>> FunctionDumpAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionDumpAsync().ToClusterValue(route), route);
    }

    /// <inheritdoc cref="IBaseClient.FunctionRestoreAsync(byte[], CancellationToken)"/>
    public new async Task FunctionRestoreAsync(
        byte[] payload,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, null), Route.AllPrimaries);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionRestoreAsync(byte[], Route, CancellationToken)"/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, null), route);
    }

    /// <inheritdoc cref="IBaseClient.FunctionRestoreAsync(byte[], FunctionRestorePolicy, CancellationToken)"/>
    public new async Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, policy), Route.AllPrimaries);
    }

    /// <inheritdoc cref="IGlideClusterClient.FunctionRestoreAsync(byte[], FunctionRestorePolicy, Route, CancellationToken)"/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, policy), route);
    }
}
