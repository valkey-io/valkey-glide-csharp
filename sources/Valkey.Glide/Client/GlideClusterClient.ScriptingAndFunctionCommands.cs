// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public sealed partial class GlideClusterClient
{
    // ===== Script Execution with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.ScriptInvokeAsync(Script, ClusterScriptOptions, CancellationToken)"/>
    // TODO #496: Change return type to Task<ValkeyResult>; remove AllPrimaries default.
    [Obsolete("Return type will change to Task<ValkeyResult>. See #496.")]
    public async Task<ClusterValue<ValkeyResult>> ScriptInvokeAsync(
        Script script,
        ClusterScriptOptions options,
        CancellationToken cancellationToken = default)
    {
        Route route = options.Route ?? Route.AllPrimaries;
        return await Command(Request.EvalSha(script.Hash, null, options.Args).ToClusterValue(route), route);
    }

    // ===== Script Management with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.ScriptExistsAsync(IEnumerable{string}, Route, CancellationToken)"/>
    public async Task<ClusterValue<bool[]>> ScriptExistsAsync(
        IEnumerable<string> sha1Hashes,
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.ScriptExists([.. sha1Hashes]).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.ScriptFlushAsync(Route, CancellationToken)"/>
    public async Task ScriptFlushAsync(
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.ScriptFlush(), route);

    /// <inheritdoc cref="IGlideClusterClient.ScriptFlushAsync(FlushMode, Route, CancellationToken)"/>
    public async Task ScriptFlushAsync(
        FlushMode mode,
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.ScriptFlush(mode), route);

    /// <inheritdoc cref="IGlideClusterClient.ScriptKillAsync(Route, CancellationToken)"/>
    public async Task ScriptKillAsync(
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.ScriptKill(), route);

    // ===== Function Execution with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.FCallAsync(string, Route, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FCall(function, null, null).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.FCallAsync(string, IEnumerable{string}, Route, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        IEnumerable<string> args,
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FCall(function, null, [.. args]).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.FCallReadOnlyAsync(string, Route, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FCallReadOnly(function, null, null).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.FCallReadOnlyAsync(string, IEnumerable{string}, Route, CancellationToken)"/>
    public async Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        IEnumerable<string> args,
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FCallReadOnly(function, null, [.. args]).ToClusterValue(route), route);

    // ===== Function Management with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.FunctionLoadAsync(string, Route, CancellationToken)"/>
    public async Task<ClusterValue<string>> FunctionLoadAsync(
        string libraryCode,
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FunctionLoad(libraryCode, false).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.FunctionLoadAsync(string, bool, Route, CancellationToken)"/>
    public async Task<ClusterValue<string>> FunctionLoadAsync(
        string libraryCode,
        bool replace,
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FunctionLoad(libraryCode, replace).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.FunctionDeleteAsync(string, Route, CancellationToken)"/>
    public async Task FunctionDeleteAsync(
        string libraryName,
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionDelete(libraryName), route);

    /// <inheritdoc cref="IGlideClusterClient.FunctionFlushAsync(Route, CancellationToken)"/>
    public async Task FunctionFlushAsync(
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionFlush(), route);

    /// <inheritdoc cref="IGlideClusterClient.FunctionFlushAsync(FlushMode, Route, CancellationToken)"/>
    public async Task FunctionFlushAsync(
        FlushMode mode,
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionFlush(mode), route);

    /// <inheritdoc cref="IBaseClient.FunctionKillAsync(CancellationToken)"/>
    public new async Task FunctionKillAsync(
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionKill());

    /// <inheritdoc cref="IGlideClusterClient.FunctionKillAsync(Route, CancellationToken)"/>
    public async Task FunctionKillAsync(
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionKill(), route);

    // ===== Function Inspection with Routing =====

    /// <inheritdoc cref="IGlideClusterClient.FunctionListAsync(FunctionListOptions?, CancellationToken)"/>
    // TODO #495: Remove method; consolidate single-value version into BaseClient.
    [Obsolete("Use FunctionListAsync(FunctionListOptions?, Route) instead. See #495.")]
    public async Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListOptions? options = null,
        CancellationToken cancellationToken = default)
            => await Command(Request.FunctionList(options).ToClusterValue(), Route.AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.FunctionListAsync(FunctionListOptions?, Route, CancellationToken)"/>
    public async Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListOptions? options,
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FunctionList(options).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.FunctionStatsAsync(Route, CancellationToken)"/>
    public async Task<ClusterValue<FunctionStatsResult>> FunctionStatsAsync(
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FunctionStats().ToClusterValue(route), route);

    // ===== Function Persistence with Routing =====

    /// <inheritdoc cref="IBaseClient.FunctionDumpAsync(CancellationToken)"/>
    public new async Task<byte[]> FunctionDumpAsync(
        CancellationToken cancellationToken = default)
            => await Command(Request.FunctionDump());

    /// <inheritdoc cref="IGlideClusterClient.FunctionDumpAsync(Route, CancellationToken)"/>
    public async Task<ClusterValue<byte[]>> FunctionDumpAsync(
        Route route,
        CancellationToken cancellationToken = default)
            => await Command(Request.FunctionDump().ToClusterValue(route), route);

    /// <inheritdoc cref="IBaseClient.FunctionRestoreAsync(byte[], CancellationToken)"/>
    public new async Task FunctionRestoreAsync(
        byte[] payload,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionRestore(payload, null));

    /// <inheritdoc cref="IGlideClusterClient.FunctionRestoreAsync(byte[], Route, CancellationToken)"/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionRestore(payload, null), route);

    /// <inheritdoc cref="IBaseClient.FunctionRestoreAsync(byte[], FunctionRestorePolicy, CancellationToken)"/>
    public new async Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionRestore(payload, policy));

    /// <inheritdoc cref="IGlideClusterClient.FunctionRestoreAsync(byte[], FunctionRestorePolicy, Route, CancellationToken)"/>
    public async Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        Route route,
        CancellationToken cancellationToken = default)
            => _ = await Command(Request.FunctionRestore(payload, policy), route);
}
