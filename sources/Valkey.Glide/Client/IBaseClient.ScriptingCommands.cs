// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Scripting and function commands for Valkey GLIDE clients.
/// </summary>
/// <remarks>
/// These methods are GLIDE-specific and not available in StackExchange.Redis.
/// For StackExchange.Redis-compatible scripting methods, use <see cref="IDatabaseAsync"/>.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#scripting">Valkey – Scripting and Function Commands</seealso>
public partial interface IBaseClient
{
    // ===== Script Execution =====

    /// <summary>
    /// Executes a Lua script using EVALSHA with automatic fallback to EVAL on NOSCRIPT error.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return 'Hello, World!'");
    /// ValkeyResult result = await client.ScriptInvokeAsync(script);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> ScriptInvokeAsync(
        Script script,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a Lua script with keys and arguments using EVALSHA with automatic fallback to EVAL on NOSCRIPT error.
    /// </summary>
    /// <param name="script">The script to execute.</param>
    /// <param name="options">The options containing keys and arguments for the script.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return KEYS[1] .. ARGV[1]");
    /// var options = new ScriptOptions().WithKeys("mykey").WithArgs("myvalue");
    /// ValkeyResult result = await client.ScriptInvokeAsync(script, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> ScriptInvokeAsync(
        Script script,
        ScriptOptions options,
        CancellationToken cancellationToken = default);

    // ===== Script Management =====


    /// <summary>
    /// Checks if scripts exist in the server cache by their SHA1 hashes.
    /// </summary>
    /// <param name="sha1Hashes">The SHA1 hashes of scripts to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An array of booleans indicating whether each script exists in the cache.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool[] exists = await client.ScriptExistsAsync([script1.Hash, script2.Hash]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool[]> ScriptExistsAsync(
        IEnumerable<string> sha1Hashes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the server cache using default flush mode (SYNC).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ScriptFlushAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptFlushAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the server cache with specified flush mode.
    /// </summary>
    /// <param name="mode">The flush mode (SYNC or ASYNC).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ScriptFlushAsync(FlushMode.Async);
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptFlushAsync(
        FlushMode mode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the source code of a cached script by its SHA1 hash.
    /// </summary>
    /// <param name="sha1Hash">The SHA1 hash of the script.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The script source code, or null if the script is not in the cache.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// string? source = await client.ScriptShowAsync(script.Hash);
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> ScriptShowAsync(
        string sha1Hash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates a currently executing script that has not written data.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if no script is running or if the script has written data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ScriptKillAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptKillAsync(
        CancellationToken cancellationToken = default);

    // ===== Function Execution =====

    /// <summary>
    /// Executes a loaded function by name.
    /// </summary>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the function execution.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyResult result = await client.FCallAsync("myfunction");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> FCallAsync(
        string function,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Executes a loaded function with keys and arguments.
    /// </summary>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="keys">The keys to pass to the function.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the function execution.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyResult result = await client.FCallAsync("myfunction", ["key1"], ["arg1", "arg2"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> FCallAsync(
        string function,
        IEnumerable<string> keys,
        IEnumerable<string> args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function in read-only mode.
    /// </summary>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the function execution.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the function attempts to write data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyResult result = await client.FCallReadOnlyAsync("myfunction");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> FCallReadOnlyAsync(
        string function,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function in read-only mode with keys and arguments.
    /// </summary>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="keys">The keys to pass to the function.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the function execution.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the function attempts to write data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyResult result = await client.FCallReadOnlyAsync("myfunction", ["key1"], ["arg1"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> FCallReadOnlyAsync(
        string function,
        IEnumerable<string> keys,
        IEnumerable<string> args,
        CancellationToken cancellationToken = default);

    // ===== Function Management =====

    /// <summary>
    /// Loads a function library from Lua code.
    /// </summary>
    /// <param name="libraryCode">The Lua code defining the function library.</param>
    /// <param name="replace">Whether to replace an existing library with the same name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The name of the loaded library.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the library code is invalid or if replace is false and the library already exists.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// string libraryName = await client.FunctionLoadAsync(
    ///     "#!lua name=mylib\nredis.register_function('myfunc', function(keys, args) return 'Hello' end)",
    ///     replace: true);
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> FunctionLoadAsync(
        string libraryCode,
        bool replace = false,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Flushes all loaded functions using default flush mode (SYNC).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FunctionFlushAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionFlushAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all loaded functions with specified flush mode.
    /// </summary>
    /// <param name="mode">The flush mode (SYNC or ASYNC).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FunctionFlushAsync(FlushMode.Async);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionFlushAsync(
        FlushMode mode,
        CancellationToken cancellationToken = default);
}
