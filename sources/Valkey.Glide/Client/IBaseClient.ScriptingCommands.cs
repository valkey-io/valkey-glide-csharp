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
