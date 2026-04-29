// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Scripting commands for clients (StackExchange.Redis compatibility).
/// </summary>
/// <seealso href="https://valkey.io/commands/#scripting">Valkey – Scripting and Function Commands</seealso>
public interface IScriptingAndFunctionBaseCommands
{
    // ===== StackExchange.Redis Compatibility Methods =====

    /// <summary>
    /// Evaluates a Lua script on the server.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/eval/">Valkey commands – EVAL</seealso>
    /// <param name="script">The Lua script to evaluate.</param>
    /// <param name="keys">The keys to pass to the script. Defaults to <see langword="null"/> (no keys).</param>
    /// <param name="values">The values to pass to the script. Defaults to <see langword="null"/> (no values).</param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// For better performance with repeated executions, consider using
    /// <see cref="LuaScript.Prepare(string)"/> or pre-loading scripts with
    /// <see cref="IServer.ScriptLoadAsync(string, CommandFlags)"/>.
    /// <example>
    /// <code>
    /// var scriptResult = await client.ScriptEvaluateAsync("return 'hello'");  // "hello"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null);

    /// <summary>
    /// Evaluates a pre-loaded Lua script on the server using its SHA1 hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/evalsha/">Valkey commands – EVALSHA</seealso>
    /// <param name="hash">The SHA1 hash of the script to evaluate.</param>
    /// <param name="keys">The keys to pass to the script. Defaults to <see langword="null"/> (no keys).</param>
    /// <param name="values">The values to pass to the script. Defaults to <see langword="null"/> (no values).</param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// If the script is not cached on the server, a <c>NOSCRIPT</c> error will be thrown.
    /// Use <see cref="IServer.ScriptLoadAsync(string, CommandFlags)"/> to pre-load scripts.
    /// <example>
    /// <code>
    /// byte[] scriptHash = await server.ScriptLoadAsync("return 'hello'");
    /// var scriptResult = await client.ScriptEvaluateAsync(scriptHash);  // "hello"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null);

    /// <summary>
    /// Evaluates a <see cref="LuaScript"/> with named parameter support.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/eval/">Valkey commands – EVAL</seealso>
    /// <param name="script">The <see cref="LuaScript"/> to evaluate.</param>
    /// <param name="parameters">
    /// An object whose properties/fields supply values for named parameters in
    /// <paramref name="script"/>. Defaults to <see langword="null"/> (no parameters).
    /// </param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// Parameters of type <see cref="ValkeyKey"/> are treated as keys, while other types are treated as arguments.
    /// <example>
    /// <code>
    /// var luaScript = LuaScript.Prepare("return @value");
    /// var scriptResult = await client.ScriptEvaluateAsync(luaScript, new { value = "hello" });  // "hello"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters = null);

    /// <summary>
    /// Evaluates a pre-loaded <see cref="LoadedLuaScript"/> using EVALSHA.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/evalsha/">Valkey commands – EVALSHA</seealso>
    /// <param name="script">The <see cref="LoadedLuaScript"/> to evaluate.</param>
    /// <param name="parameters">
    /// An object whose properties/fields supply values for named parameters in
    /// <paramref name="script"/>. Defaults to <see langword="null"/> (no parameters).
    /// </param>
    /// <returns>The result of the script execution.</returns>
    /// <remarks>
    /// If the script is not cached on the server, a <c>NOSCRIPT</c> error will be thrown.
    /// Obtain a <see cref="LoadedLuaScript"/> via <see cref="LuaScript.LoadAsync(IServer, CommandFlags)"/>.
    /// <example>
    /// <code>
    /// var luaScript = LuaScript.Prepare("return 'hello'");
    /// var loaded = await luaScript.LoadAsync(server);
    /// var scriptResult = await client.ScriptEvaluateAsync(loaded);  // "hello"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters = null);

    // Script management methods (ScriptInvokeAsync, ScriptExistsAsync, ScriptFlushAsync, ScriptShowAsync, ScriptKillAsync)
    // have been moved to IBaseClient as they are not in StackExchange.Redis.
    // Function methods (FCallAsync, FunctionLoadAsync, etc.) have also been moved to IBaseClient.
}
