// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Scripting and function commands specific to cluster clients with routing support.
/// </summary>
public interface IScriptingAndFunctionClusterCommands : IScriptingAndFunctionBaseCommands
{
    // ===== Script Execution with Routing =====

    /// <summary>
    /// Executes a Lua script with routing options for cluster execution.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="options">The options containing arguments and routing configuration.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing single or multi-node results depending on routing.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return 'Hello'");
    /// var options = new ClusterScriptOptions().WithRoute(Route.AllPrimaries);
    /// ClusterValue&lt;ValkeyResult&gt; result = await client.InvokeScriptAsync(script, options);
    /// if (result.HasMultiData)
    /// {
    ///     foreach (var (node, value) in result.MultiValue)
    ///     {
    ///         Console.WriteLine($"{node}: {value}");
    ///         ///     }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> InvokeScriptAsync(
        Script script,
        ClusterScriptOptions options,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    // ===== Script Management with Routing =====

    /// <summary>
    /// Checks if scripts exist in the server cache on specified nodes.
    /// </summary>
    /// <param name="sha1Hashes">The SHA1 hashes of scripts to check.</param>
    /// <param name="route">The routing configuration specifying which nodes to query.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing single or multi-node results.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;bool[]&gt; exists = await client.ScriptExistsAsync(
    ///     [script.Hash],
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<bool[]>> ScriptExistsAsync(
        string[] sha1Hashes,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the cache on specified nodes using default flush mode.
    /// </summary>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.ScriptFlushAsync(Route.AllNodes);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> ScriptFlushAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the cache on specified nodes with specified flush mode.
    /// </summary>
    /// <param name="mode">The flush mode (SYNC or ASYNC).</param>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.ScriptFlushAsync(
    ///     FlushMode.Async,
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> ScriptFlushAsync(
        FlushMode mode,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates currently executing scripts on specified nodes.
    /// </summary>
    /// <param name="route">The routing configuration specifying which nodes to target.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.ScriptKillAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> ScriptKillAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    // ===== Function Execution with Routing =====

    /// <summary>
    /// Executes a loaded function on specified nodes.
    /// </summary>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="route">The routing configuration specifying which nodes to execute on.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing single or multi-node results.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;ValkeyResult&gt; result = await client.FCallAsync(
    ///     "myfunction",
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function with arguments on specified nodes.
    /// </summary>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <param name="route">The routing configuration specifying which nodes to execute on.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing single or multi-node results.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;ValkeyResult&gt; result = await client.FCallAsync(
    ///     "myfunction",
    ///     ["arg1", "arg2"],
    ///     Route.RandomRoute);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> FCallAsync(
        string function,
        string[] args,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function in read-only mode on specified nodes.
    /// </summary>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="route">The routing configuration specifying which nodes to execute on.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing single or multi-node results.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;ValkeyResult&gt; result = await client.FCallReadOnlyAsync(
    ///     "myfunction",
    ///     Route.AllNodes);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function in read-only mode with arguments on specified nodes.
    /// </summary>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <param name="route">The routing configuration specifying which nodes to execute on.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing single or multi-node results.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;ValkeyResult&gt; result = await client.FCallReadOnlyAsync(
    ///     "myfunction",
    ///     ["arg1"],
    ///     Route.AllNodes);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<ValkeyResult>> FCallReadOnlyAsync(
        string function,
        string[] args,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    // ===== Function Management with Routing =====

    /// <summary>
    /// Loads a function library on specified nodes.
    /// </summary>
    /// <param name="libraryCode">The Lua code defining the function library.</param>
    /// <param name="replace">Whether to replace an existing library with the same name.</param>
    /// <param name="route">The routing configuration specifying which nodes to load on.</param>
    ///    /// <param name="flags">Tho use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing library names from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.FunctionLoadAsync(
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
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a function library from specified nodes.
    /// </summary>
    /// <param name="libraryName">The name of the library to delete.</param>
    /// <param name="route">The routing configuration specifying which nodes to delete from.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.FunctionDeleteAsync(
    ///  ///     "mylib",
    ///     imaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> FunctionDeleteAsync(
        string libraryName,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all loaded functions from specified nodes using default flush mode.
    /// </summary>
    ///     /// <param name="route">g configuration specifying which nodes to flush.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.FunctionFlushAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> FunctionFlushAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all loaded functions from specified nodes with specified flush mode.
    /// </summary>
    /// <param name="mode">The flush mode (SYNC or ASYNC).</param>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    ///   /// <code>
    /// ClusterValue&lt;string&gt; result = await client.FunctionFlushAsync(
    ///     FlushMode.Async,
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> FunctionFlushAsync(
        FlushMode mode,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates currently executing functions on specified nodes.
    /// </summary>
    /// <param name="route">The routing configuration specifying which nodes to target.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.FunctionKillAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> FunctionKillAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    // ===== Function Inspection with Routing =====

    /// <summary>
    /// Lists loaded function libraries from specified nodes.
    /// </summary>
    /// <param name="query">Optional query parameters to filter results.</param>
    /// <param name="route">The routing configuration specifying which nodes to query.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing library information from nodes.</returns>
    /// <remarks>
    ///  /// <example>
    /// <code>
    ///alue&lt;LibraryInfo[]&gt; result = await client.FunctionListAsync(
    ///     null,
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<LibraryInfo[]>> FunctionListAsync(
        FunctionListQuery? query,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns function statistics from specified nodes.
    /// </summary>
    /// <param name="route">The routing configuration specifying which nodes to query.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing per-node function statistics.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;FunctionStatsResult&gt; result = await client.FunctionStatsAsync(
    ///     Route.AllPrimaries);
    /// foreach (var (node, stats) in result.MultiValue)
    /// {
    ///     Console.WriteLine($"{node}: {stats.Engines.Count} engines");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<FunctionStatsResult>> FunctionStatsAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    // ===== Function Persistence with Routing =====

    /// <summary>
    /// Creates a binary backup of loaded functions from specified nodes.
    /// </summary>
    /// <param name="route">The routing configuration specifying which nodes to backup from.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing binary payloads from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;byte[]&gt; result = await client.FunctionDumpAsync(Route.RandomRoute);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<byte[]>> FunctionDumpAsync(
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup on specified nodes using default policy.
    /// </summary>
    /// <param name="payload">The binary payload from FunctionDump.</param>
    /// <param name="route">The routing configuration specifying which nodes to restore to.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.FunctionRestoreAsync(
    ///     backup,
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> FunctionRestoreAsync(
        byte[] payload,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup on specified nodes with specified policy.
    /// </summary>
    /// <param name="payload">The binary payload from FunctionDump.</param>
    /// <param name="policy">The restore policy (APPEND, FLUSH, or REPLACE).</param>
    /// <param name="route">The routing configuration specifying which nodes to restore to.</param>
    /// <param name="flags">The flags to use for this operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing "OK" responses from nodes.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ClusterValue&lt;string&gt; result = await client.FunctionRestoreAsync(
    ///     backup,
    ///     FunctionRestorePolicy.Replace,
    ///     Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ClusterValue<string>> FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        Route route,
        CommandFlags flags = CommandFlags.None,
        CancellationToken cancellationToken = default);
}
