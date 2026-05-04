// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Scripting and function commands for Valkey GLIDE cluster clients.
/// </summary>
/// <remarks>
/// These methods are GLIDE-specific and not available in StackExchange.Redis.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#scripting">Valkey – Scripting and Function Commands</seealso>
public partial interface IGlideClusterClient
{
    // ===== Script Execution with Routing =====

    /// <summary>
    /// Executes a Lua script with routing options for cluster execution.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/evalsha/">Valkey commands – EVALSHA</seealso>
    /// <seealso href="https://valkey.io/commands/eval/">Valkey commands – EVAL</seealso>
    /// <param name="script">The script to execute.</param>
    /// <param name="options">The options containing arguments and routing configuration.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing single- or multi-node results depending on routing.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return 'Hello'");
    /// var options = new ClusterScriptOptions().WithRoute(Route.AllPrimaries);
    /// var result = await clusterClient.ScriptInvokeAsync(script, options);
    /// if (result.HasMultiData)
    /// {
    ///     foreach (var (node, value) in result.MultiValue)
    ///     {
    ///         Console.WriteLine($"{node}: {value}");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> ScriptInvokeAsync(
        Script script,
        ClusterScriptOptions options,
        CancellationToken cancellationToken = default);

    // ===== Script Management with Routing =====

    /// <summary>
    /// Checks if scripts exist in the server cache on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-exists/">Valkey commands – SCRIPT EXISTS</seealso>
    /// <param name="sha1Hashes">The SHA1 hashes of scripts to check.</param>
    /// <param name="route">The routing configuration specifying which nodes to query.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing single- or multi-node results.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return 1");
    /// var exists = await clusterClient.ScriptExistsAsync(
    ///     [script.Hash],
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<bool[]>> ScriptExistsAsync(
        IEnumerable<string> sha1Hashes,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the cache on specified nodes using the server's default flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-flush/">Valkey commands – SCRIPT FLUSH</seealso>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <remarks>
    /// The flush behavior (sync or async) is determined by the server's <c>lazyfree-lazy-user-flush</c> configuration.
    /// Use the overload with <see cref="FlushMode"/> to explicitly specify the behavior.
    /// <example>
    /// <code>
    /// await clusterClient.ScriptFlushAsync(Route.AllNodes);
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptFlushAsync(
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the cache on specified nodes with the specified flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-flush/">Valkey commands – SCRIPT FLUSH</seealso>
    /// <param name="mode">The flush mode.</param>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ScriptFlushAsync(FlushMode.Async, Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptFlushAsync(
        FlushMode mode,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates currently executing scripts on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-kill/">Valkey commands – SCRIPT KILL</seealso>
    /// <param name="route">The routing configuration specifying which nodes to target.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if no script is running or if the script has written data on the targeted nodes.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.ScriptKillAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptKillAsync(
        Route route,
        CancellationToken cancellationToken = default);

    // ===== Function Execution with Routing =====

    /// <summary>
    /// Executes a loaded function on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/fcall/">Valkey commands – FCALL</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="route">The routing configuration specifying which nodes to execute on.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing single- or multi-node results.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await clusterClient.FCallAsync(
    ///     "myfunction",
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function with arguments on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/fcall/">Valkey commands – FCALL</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <param name="route">The routing configuration specifying which nodes to execute on.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing single- or multi-node results.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await clusterClient.FCallAsync(
    ///     "myfunction",
    ///     ["arg1", "arg2"],
    ///     Route.Random);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        IEnumerable<string> args,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function in read-only mode on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/fcall_ro/">Valkey commands – FCALL_RO</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="route">The routing configuration specifying which nodes to execute on.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing single- or multi-node results.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the function attempts to write data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await clusterClient.FCallReadOnlyAsync(
    ///     "myfunction",
    ///     Route.AllNodes);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function in read-only mode with arguments on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/fcall_ro/">Valkey commands – FCALL_RO</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <param name="route">The routing configuration specifying which nodes to execute on.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing single- or multi-node results.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the function attempts to write data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await clusterClient.FCallReadOnlyAsync(
    ///     "myfunction",
    ///     ["arg1"],
    ///     Route.AllNodes);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        IEnumerable<string> args,
        Route route,
        CancellationToken cancellationToken = default);

    // ===== Function Management with Routing =====

    /// <summary>
    /// Loads a function library on specified nodes without replacing an existing library.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-load/">Valkey commands – FUNCTION LOAD</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="libraryCode">The Lua code defining the function library.</param>
    /// <param name="route">The routing configuration specifying which nodes to load on.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing library names from the targeted nodes.</returns>
    /// <remarks>
    /// Use the overload with a <c>replace</c> parameter to overwrite existing libraries.
    /// <example>
    /// <code>
    /// string libraryCode = "#!lua name=mylib\nvalkey.register_function('myfunc', function(keys, args) return args[1] end)";
    /// var result = await clusterClient.FunctionLoadAsync(
    ///     libraryCode,
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> FunctionLoadAsync(
        string libraryCode,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a function library on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-load/">Valkey commands – FUNCTION LOAD</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="libraryCode">The Lua code defining the function library.</param>
    /// <param name="replace">Whether to replace an existing library with the same name.</param>
    /// <param name="route">The routing configuration specifying which nodes to load on.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing library names from the targeted nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string libraryCode = "#!lua name=mylib\nvalkey.register_function('myfunc', function(keys, args) return args[1] end)";
    /// var result = await clusterClient.FunctionLoadAsync(
    ///     libraryCode,
    ///     replace: true,
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> FunctionLoadAsync(
        string libraryCode,
        bool replace,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a function library from specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-delete/">Valkey commands – FUNCTION DELETE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="libraryName">The name of the library to delete.</param>
    /// <param name="route">The routing configuration specifying which nodes to delete from.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the library does not exist on a targeted node.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.FunctionDeleteAsync("mylib", Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionDeleteAsync(
        string libraryName,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all loaded functions from specified nodes using the server's default flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-flush/">Valkey commands – FUNCTION FLUSH</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <remarks>
    /// The flush behavior (sync or async) is determined by the server's <c>lazyfree-lazy-user-flush</c> configuration.
    /// Use the overload with <see cref="FlushMode"/> to explicitly specify the behavior.
    /// <example>
    /// <code>
    /// await clusterClient.FunctionFlushAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionFlushAsync(
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all loaded functions from specified nodes with the specified flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-flush/">Valkey commands – FUNCTION FLUSH</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="mode">The flush mode.</param>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.FunctionFlushAsync(FlushMode.Async, Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionFlushAsync(
        FlushMode mode,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates currently executing functions on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-kill/">Valkey commands – FUNCTION KILL</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="route">The routing configuration specifying which nodes to target.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if no function is running or if the function has written data on the targeted nodes.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await clusterClient.FunctionKillAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionKillAsync(
        Route route,
        CancellationToken cancellationToken = default);

    // ===== Function Inspection with Routing =====

    /// <summary>
    /// Lists loaded function libraries from all primary nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-list/">Valkey commands – FUNCTION LIST</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="options">Optional query parameters to filter results.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing library information from the targeted nodes.</returns>
    /// <remarks>
    /// Uses <see cref="Route.AllPrimaries"/> as the route. Use the overload with an explicit <c>route</c> parameter
    /// to target specific nodes.
    /// <example>
    /// <code>
    /// var result = await clusterClient.FunctionListAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists loaded function libraries from specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-list/">Valkey commands – FUNCTION LIST</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="options">Optional query parameters to filter results.</param>
    /// <param name="route">The routing configuration specifying which nodes to query.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing library information from the targeted nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await clusterClient.FunctionListAsync(null, Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListOptions? options,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns function statistics from specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-stats/">Valkey commands – FUNCTION STATS</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="route">The routing configuration specifying which nodes to query.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing per-node function statistics.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await clusterClient.FunctionStatsAsync(Route.AllPrimaries);
    /// foreach (var (node, stats) in result.MultiValue)
    /// {
    ///     Console.WriteLine($"{node}: {stats.Engines.Count} engines");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<FunctionStatsResult>> FunctionStatsAsync(
        Route route,
        CancellationToken cancellationToken = default);

    // ===== Function Persistence with Routing =====

    /// <summary>
    /// Creates a binary backup of loaded functions from specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-dump/">Valkey commands – FUNCTION DUMP</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="route">The routing configuration specifying which nodes to back up from.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A <see cref="ClusterValue{T}"/> containing binary payloads from the targeted nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await clusterClient.FunctionDumpAsync(Route.Random);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<byte[]>> FunctionDumpAsync(
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup on specified nodes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-restore/">Valkey commands – FUNCTION RESTORE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="payload">The binary payload from <see cref="FunctionDumpAsync(Route, CancellationToken)"/>.</param>
    /// <param name="route">The routing configuration specifying which nodes to restore to.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if restoration fails (e.g., library conflict with the default <see cref="FunctionRestorePolicy.Append"/> policy).</exception>
    /// <remarks>
    /// Uses the default <see cref="FunctionRestorePolicy.Append"/> policy. Use the overload with
    /// <see cref="FunctionRestorePolicy"/> to specify a different policy.
    /// <example>
    /// <code>
    /// var backup = new byte[] { 0x01, 0x02 };
    /// await clusterClient.FunctionRestoreAsync(backup, Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionRestoreAsync(
        byte[] payload,
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup on specified nodes with the specified policy.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-restore/">Valkey commands – FUNCTION RESTORE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="payload">The binary payload from <see cref="FunctionDumpAsync(Route, CancellationToken)"/>.</param>
    /// <param name="policy">The restore policy.</param>
    /// <param name="route">The routing configuration specifying which nodes to restore to.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if restoration fails.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var payload = new byte[] { 0x01, 0x02 };
    /// await clusterClient.FunctionRestoreAsync(payload, FunctionRestorePolicy.Replace, Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        Route route,
        CancellationToken cancellationToken = default);
}
