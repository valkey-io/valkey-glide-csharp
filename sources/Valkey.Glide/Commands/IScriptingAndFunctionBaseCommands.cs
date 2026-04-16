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
    /// Evaluates a Lua script on the server (StackExchange.Redis compatibility).
    /// </summary>
    /// <param name="script">The Lua script to evaluate.</param>
    /// <param name="keys">The keys to pass to the script.</param>
    /// <param name="values">The values to pass to the script.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the script execution.</returns>
    /// <remarks>
    /// This method uses EVAL to execute the script. For better performance with repeated executions,
    /// consider using LuaScript.Prepare() or pre-loading scripts with IServer.ScriptLoadAsync().
    /// </remarks>
    Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null);

    /// <summary>
    /// Evaluates a pre-loaded Lua script on the server using its SHA1 hash (StackExchange.Redis compatibility).
    /// </summary>
    /// <param name="hash">The SHA1 hash of the script to evaluate.</param>
    /// <param name="keys">The keys to pass to the script.</param>
    /// <param name="values">The values to pass to the script.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the script execution.</returns>
    /// <remarks>
    /// This method uses EVALSHA to execute the script by its hash. If the script is not cached on the server,
    /// a NOSCRIPT error will be thrown. Use IServer.ScriptLoadAsync() to pre-load scripts.
    /// </remarks>
    Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null);

    /// <summary>
    /// Evaluates a LuaScript with named parameter support (StackExchange.Redis compatibility).
    /// </summary>
    /// <param name="script">The LuaScript to evaluate.</param>
    /// <param name="parameters">An object containing parameter values. Properties/fields should match parameter names.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the script execution.</returns>
    /// <remarks>
    /// This method extracts parameter values from the provided object and passes them to the script.
    /// Parameters of type ValkeyKey are treated as keys, while other types are treated
    /// as arguments.
    /// </remarks>
    Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters = null);

    /// <summary>
    /// Evaluates a pre-loaded LuaScript using EVALSHA (StackExchange.Redis compatibility).
    /// </summary>
    /// <param name="script">The LoadedLuaScript to evaluate.</param>
    /// <param name="parameters">An object containing parameter values. Properties/fields should match parameter names.</param>
    /// <returns>A task representing the asynchronous operation, containing the result of the script execution.</returns>
    /// <remarks>
    /// This method uses EVALSHA to execute the script by its hash. If the script is not cached on the server,
    /// a NOSCRIPT error will be thrown.
    /// </remarks>
    Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters = null);

    // Script management methods (ScriptInvokeAsync, ScriptExistsAsync, ScriptFlushAsync, ScriptShowAsync, ScriptKillAsync)
    // have been moved to IBaseClient as they are not in StackExchange.Redis.
    // Function methods (FCallAsync, FunctionLoadAsync, etc.) have also been moved to IBaseClient.
}
