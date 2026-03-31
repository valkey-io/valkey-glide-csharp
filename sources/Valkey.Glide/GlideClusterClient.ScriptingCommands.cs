// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public sealed partial class GlideClusterClient : IScriptingAndFunctionClusterCommands
{
    // ===== Script Execution with Routing =====

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<ClusterValue<bool[]>> ScriptExistsAsync(
        IEnumerable<string> sha1Hashes,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.ScriptExistsAsync([.. sha1Hashes]).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task ScriptFlushAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptFlushAsync(), route);
    }

    /// <inheritdoc/>
    public async Task ScriptFlushAsync(
        FlushMode mode,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptFlushAsync(mode), route);
    }

    /// <inheritdoc/>
    public async Task ScriptKillAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.ScriptKillAsync(), route);
    }

    // ===== Function Execution with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallAsync(function, null, null).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        IEnumerable<string> args,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallAsync(function, null, [.. args]).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallReadOnlyAsync(function, null, null).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        IEnumerable<string> args,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FCallReadOnlyAsync(function, null, [.. args]).ToClusterValue(route), route);
    }

    // ===== Function Management with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> FunctionLoadAsync(
        string libraryCode,
        bool replace,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionLoadAsync(libraryCode, replace).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task FunctionDeleteAsync(
        string libraryName,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionDeleteAsync(libraryName), route);
    }

    /// <inheritdoc/>
    public async Task FunctionFlushAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionFlushAsync(), route);
    }

    /// <inheritdoc/>
    public async Task FunctionFlushAsync(
        FlushMode mode,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionFlushAsync(mode), route);
    }

    /// <inheritdoc/>
    public async Task FunctionKillAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionKillAsync(), route);
    }

    // ===== Function Inspection with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListQuery? query,
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionListAsync(query).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<FunctionStatsResult>> FunctionStatsAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionStatsAsync().ToClusterValue(route), route);
    }

    // ===== Function Persistence with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<byte[]>> FunctionDumpAsync(
        Route route,
        CancellationToken cancellationToken = default)
    {
        return await Command(Request.FunctionDumpAsync().ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, null), route);
    }

    /// <inheritdoc/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        Route route,
        CancellationToken cancellationToken = default)
    {
        _ = await Command(Request.FunctionRestoreAsync(payload, policy), route);
    }
}
