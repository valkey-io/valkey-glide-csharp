// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// Describes functionality that is common to both standalone and cluster servers.<br />
/// See also <see cref="GlideClient" /> and <see cref="GlideClusterClient" />.
/// </summary>
public interface IDatabase : IDatabaseAsync
{
    /// <inheritdoc cref="IDatabaseAsync.ExecuteAsync(string, object[])"/>
    ValkeyResult Execute(string command, params object[] args);

    /// <inheritdoc cref="IDatabaseAsync.ExecuteAsync(string, ICollection{object}, CommandFlags)"/>
    ValkeyResult Execute(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Allows creation of a group of operations that will be sent to the server as a single unit,
    /// but which may or may not be processed on the server contiguously.
    /// </summary>
    /// <param name="asyncState">The async state is not supported by GLIDE.</param>
    /// <returns>The created batch.</returns>
    IBatch CreateBatch(object? asyncState = null);

    /// <summary>
    /// Allows creation of a group of operations that will be sent to the server as a single unit,
    /// and processed on the server as a single unit.
    /// </summary>
    /// <param name="asyncState">The async state is not supported by GLIDE.</param>
    /// <returns>The created transaction.</returns>
    ITransaction CreateTransaction(object? asyncState = null);

    // ===== StackExchange.Redis Compatibility Methods (Synchronous) =====

    /// <summary>
    /// Evaluates a Lua script on the server (StackExchange.Redis compatibility).
    /// </summary>
    /// <param name="script">The Lua script to evaluate.</param>
    /// <param name="keys">The keys to pass to the script (KEYS array).</param>
    /// <param name="values">The values to pass to the script (ARGV array).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The result of the script execution.</returns>
    ValkeyResult ScriptEvaluate(string script, ValkeyKey[]? keys = null, ValkeyValue[]? values = null,
        CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Evaluates a pre-loaded Lua script on the server using its SHA1 hash (StackExchange.Redis compatibility).
    /// </summary>
    /// <param name="hash">The SHA1 hash of the script to evaluate.</param>
    /// <param name="keys">The keys to pass to the script (KEYS array).</param>
    /// <param name="values">The values to pass to the script (ARGV array).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The result of the script execution.</returns>
    ValkeyResult ScriptEvaluate(byte[] hash, ValkeyKey[]? keys = null, ValkeyValue[]? values = null,
        CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Evaluates a LuaScript with named parameter support (StackExchange.Redis compatibility).
    /// </summary>
    /// <param name="script">The LuaScript to evaluate.</param>
    /// <param name="parameters">An object containing parameter values.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The result of the script execution.</returns>
    ValkeyResult ScriptEvaluate(LuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Evaluates a pre-loaded LuaScript using EVALSHA (StackExchange.Redis compatibility).
    /// </summary>
    /// <param name="script">The LoadedLuaScript to evaluate.</param>
    /// <param name="parameters">An object containing parameter values.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The result of the script execution.</returns>
    ValkeyResult ScriptEvaluate(LoadedLuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None);
}
