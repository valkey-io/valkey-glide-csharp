// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Scripting and function commands for cluster clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#scripting">Valkey – Scripting and Function Commands</seealso>
public interface IScriptingAndFunctionClusterCommands : IScriptingAndFunctionBaseCommands
{
    // ===== Script Execution with Routing =====

    /// <summary>
    /// Executes a Lua script with routing options for cluster execution.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="options">The options containing arguments and routing configuration.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A ClusterValue containing single or multi-node results depending on routing.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return 'Hello'");
    /// var options = new ClusterScriptOptions().WithRoute(Route.AllPrimaries);
    /// ClusterValue&lt;ValkeyResult&gt; result = await client.ScriptInvokeAsync(script, options);
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
    /// <param name="sha1Hashes">The SHA1 hashes of scripts to check.</param>
    /// <param name="route">The routing configuration specifying which nodes to query.</param>
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
        IEnumerable<string> sha1Hashes,
        Route route,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Flushes all scripts from the cache on specified nodes using default flush mode.
    /// </summary>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ScriptFlushAsync(Route.AllNodes);
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptFlushAsync(
        Route route,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the cache on specified nodes with specified flush mode.
    /// </summary>
    /// <param name="mode">The flush mode (SYNC or ASYNC).</param>
    /// <param name="route">The routing configuration specifying which nodes to flush.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ScriptFlushAsync(FlushMode.Async, Route.AllPrimaries);
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
    /// <param name="route">The routing configuration specifying which nodes to target.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ScriptKillAsync(Route.AllPrimaries);
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptKillAsync(
        Route route,
        CancellationToken cancellationToken = default);

    // Function-related methods have been moved to IGlideClusterClient as they are not in StackExchange.Redis.
}
