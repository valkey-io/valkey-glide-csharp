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
    /// <seealso href="https://valkey.io/commands/evalsha/">Valkey commands – EVALSHA</seealso>
    /// <seealso href="https://valkey.io/commands/eval/">Valkey commands – EVAL</seealso>
    /// <param name="script">The script to execute.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return 'Hello, World!'");
    /// var scriptResult = await client.ScriptInvokeAsync(script);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> ScriptInvokeAsync(
        Script script,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a Lua script with keys and arguments using EVALSHA with automatic fallback to EVAL on NOSCRIPT error.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/evalsha/">Valkey commands – EVALSHA</seealso>
    /// <seealso href="https://valkey.io/commands/eval/">Valkey commands – EVAL</seealso>
    /// <param name="script">The script to execute.</param>
    /// <param name="options">The options containing keys and arguments for the script.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return KEYS[1] .. ARGV[1]");
    /// var scriptOptions = new ScriptOptions().WithKeys("mykey").WithArgs("myvalue");
    /// var scriptResult = await client.ScriptInvokeAsync(script, scriptOptions);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> ScriptInvokeAsync(
        Script script,
        ScriptOptions options,
        CancellationToken cancellationToken = default);

    // ===== Script Management =====

    /// <summary>
    /// Checks if a script exists in the server cache by its SHA1 hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-exists/">Valkey commands – SCRIPT EXISTS</seealso>
    /// <param name="sha1Hash">The SHA1 hash of the script to check.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns><see langword="true"/> if the script exists in the cache, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return 1");
    /// var exists = await client.ScriptExistsAsync(script.Hash);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> ScriptExistsAsync(
        string sha1Hash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if scripts exist in the server cache by their SHA1 hashes.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-exists/">Valkey commands – SCRIPT EXISTS</seealso>
    /// <param name="sha1Hashes">The SHA1 hashes of scripts to check.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>An array of booleans indicating whether each script exists in the cache.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script1 = new Script("return 1");
    /// using var script2 = new Script("return 2");
    /// var existsResults = await client.ScriptExistsAsync([script1.Hash, script2.Hash]);  // [true, true]
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool[]> ScriptExistsAsync(
        IEnumerable<string> sha1Hashes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the server cache.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-flush/">Valkey commands – SCRIPT FLUSH</seealso>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <remarks>
    /// The flush behavior (sync or async) is determined by the server's <c>lazyfree-lazy-user-flush</c> configuration.
    /// Use the overload with <see cref="FlushMode"/> to explicitly specify the behavior.
    /// <example>
    /// <code>
    /// await client.ScriptFlushAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task ScriptFlushAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all scripts from the server cache with the specified flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-flush/">Valkey commands – SCRIPT FLUSH</seealso>
    /// <param name="mode">The flush mode (SYNC or ASYNC).</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
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
    /// <seealso href="https://valkey.io/commands/script-show/">Valkey commands – SCRIPT SHOW</seealso>
    /// <note>Since Valkey 8.0.0.</note>
    /// <param name="sha1Hash">The SHA1 hash of the script.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>The script source code, or <see langword="null"/> if the script is not in the cache.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// using var script = new Script("return 1");
    /// var source = await client.ScriptShowAsync(script.Hash);  // "return 1"
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> ScriptShowAsync(
        string sha1Hash,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates a currently executing script that has not written data.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/script-kill/">Valkey commands – SCRIPT KILL</seealso>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
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
    /// <seealso href="https://valkey.io/commands/fcall/">Valkey commands – FCALL</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>The result of the function execution.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var functionResult = await client.FCallAsync("myfunction");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> FCallAsync(
        string function,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function with keys and arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/fcall/">Valkey commands – FCALL</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="keys">The keys to pass to the function.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>The result of the function execution.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var functionResult = await client.FCallAsync("myfunction", ["key1"], ["arg1", "arg2"]);
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
    /// <seealso href="https://valkey.io/commands/fcall_ro/">Valkey commands – FCALL_RO</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>The result of the function execution.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the function attempts to write data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var readOnlyResult = await client.FCallReadOnlyAsync("myfunction");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> FCallReadOnlyAsync(
        string function,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a loaded function in read-only mode with keys and arguments.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/fcall_ro/">Valkey commands – FCALL_RO</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="function">The name of the function to execute.</param>
    /// <param name="keys">The keys to pass to the function.</param>
    /// <param name="args">The arguments to pass to the function.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>The result of the function execution.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the function attempts to write data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var readOnlyResult = await client.FCallReadOnlyAsync("myfunction", ["key1"], ["arg1"]);
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
    /// <seealso href="https://valkey.io/commands/function-load/">Valkey commands – FUNCTION LOAD</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="libraryCode">The Lua code defining the function library.</param>
    /// <param name="replace">Whether to replace an existing library with the same name. Defaults to <see langword="false"/>.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>The name of the loaded library.</returns>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the library code is invalid or if <paramref name="replace"/> is <see langword="false"/> and the library already exists.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var libraryName = await client.FunctionLoadAsync(
    ///     "#!lua name=mylib\nredis.register_function('myfunc', function(keys, args) return 'Hello' end)",
    ///     replace: true);  // "mylib"
    /// </code>
    /// </example>
    /// </remarks>
    Task<string> FunctionLoadAsync(
        string libraryCode,
        bool replace = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all loaded functions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-flush/">Valkey commands – FUNCTION FLUSH</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <remarks>
    /// The flush behavior (sync or async) is determined by the server's <c>lazyfree-lazy-user-flush</c> configuration.
    /// Use the overload with <see cref="FlushMode"/> to explicitly specify the behavior.
    /// <example>
    /// <code>
    /// await client.FunctionFlushAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionFlushAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Flushes all loaded functions with the specified flush mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-flush/">Valkey commands – FUNCTION FLUSH</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="mode">The flush mode (SYNC or ASYNC).</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
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

    /// <summary>
    /// Deletes a function library by name.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-delete/">Valkey commands – FUNCTION DELETE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="libraryName">The name of the library to delete.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if the library does not exist.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FunctionDeleteAsync("mylib");
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionDeleteAsync(
        string libraryName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates a currently executing function that has not written data.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-kill/">Valkey commands – FUNCTION KILL</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if no function is running or if the function has written data.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.FunctionKillAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionKillAsync(
        CancellationToken cancellationToken = default);

    // ===== Function Persistence =====

    /// <summary>
    /// Creates a binary backup of all loaded functions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-dump/">Valkey commands – FUNCTION DUMP</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <returns>A binary payload containing all loaded functions.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var backup = await client.FunctionDumpAsync();
    /// </code>
    /// </example>
    /// </remarks>
    Task<byte[]> FunctionDumpAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-restore/">Valkey commands – FUNCTION RESTORE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="payload">The binary payload from <see cref="FunctionDumpAsync"/>.</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if restoration fails (e.g., library conflict with default APPEND policy).</exception>
    /// <remarks>
    /// Uses the default APPEND policy. Use the overload with <see cref="FunctionRestorePolicy"/> to specify a different policy.
    /// <example>
    /// <code>
    /// var backup = await client.FunctionDumpAsync();
    /// await client.FunctionRestoreAsync(backup);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionRestoreAsync(
        byte[] payload,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores functions from a binary backup with the specified policy.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/function-restore/">Valkey commands – FUNCTION RESTORE</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="payload">The binary payload from <see cref="FunctionDumpAsync"/>.</param>
    /// <param name="policy">The restore policy (APPEND, FLUSH, or REPLACE).</param>
    /// <param name="cancellationToken">A token to cancel the async operation.</param>
    /// <exception cref="Errors.ValkeyServerException">Thrown if restoration fails.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var backup = await client.FunctionDumpAsync();
    /// await client.FunctionRestoreAsync(backup, FunctionRestorePolicy.Replace);
    /// </code>
    /// </example>
    /// </remarks>
    Task FunctionRestoreAsync(
        byte[] payload,
        FunctionRestorePolicy policy,
        CancellationToken cancellationToken = default);
}
