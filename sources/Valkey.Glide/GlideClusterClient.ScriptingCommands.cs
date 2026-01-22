// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public sealed partial class GlideClusterClient : IScriptingAndFunctionClusterCommands
{
    // ===== Script Execution with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> InvokeScriptAsync(
        Script script,
        ClusterScriptOptions options,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

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
        string[] sha1Hashes,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ScriptExistsAsync(sha1Hashes).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> ScriptFlushAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ScriptFlushAsync().ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> ScriptFlushAsync(
        FlushMode mode,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ScriptFlushAsync(mode).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> ScriptKillAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ScriptKillAsync().ToClusterValue(route), route);
    }

    // ===== Function Execution with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FCallAsync(function, null, null).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        string[] args,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FCallAsync(function, null, args).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FCallReadOnlyAsync(function, null, null).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        string[] args,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FCallReadOnlyAsync(function, null, args).ToClusterValue(route), route);
    }

    // ===== Function Management with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> FunctionLoadAsync(
        string libraryCode,
        bool replace,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionLoadAsync(libraryCode, replace).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> FunctionDeleteAsync(
        string libraryName,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionDeleteAsync(libraryName).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> FunctionFlushAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionFlushAsync().ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> FunctionFlushAsync(
        FlushMode mode,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionFlushAsync(mode).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> FunctionKillAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionKillAsync().ToClusterValue(route), route);
    }

    // ===== Function Inspection with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListQuery? query,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionListAsync(query).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<FunctionStatsResult>> FunctionStatsAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionStatsAsync().ToClusterValue(route), route);
    }

    // ===== Function Persistence with Routing =====

    /// <inheritdoc/>
    public async Task<ClusterValue<byte[]>> FunctionDumpAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionDumpAsync().ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> FunctionRestoreAsync(
        byte[] payload,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionRestoreAsync(payload, null).ToClusterValue(route), route);
    }

    /// <inheritdoc/>
    public async Task<ClusterValue<string>> FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.FunctionRestoreAsync(payload, policy).ToClusterValue(route), route);
    }
}
