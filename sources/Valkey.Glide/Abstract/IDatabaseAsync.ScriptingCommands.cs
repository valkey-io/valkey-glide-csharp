// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Scripting commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IScriptingAndFunctionBaseCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LuaScript, object?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LoadedLuaScript, object?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters = null, CommandFlags flags = CommandFlags.None);
}
