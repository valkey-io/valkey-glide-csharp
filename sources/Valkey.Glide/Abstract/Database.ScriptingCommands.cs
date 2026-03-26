// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Scripting commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IScriptingAndFunctionBaseCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public ValkeyResult ScriptEvaluate(
        string script,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, keys, values).GetAwaiter().GetResult();
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public ValkeyResult ScriptEvaluate(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(hash, keys, values).GetAwaiter().GetResult();
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LuaScript, object?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public ValkeyResult ScriptEvaluate(LuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, parameters).GetAwaiter().GetResult();
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LoadedLuaScript, object?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public ValkeyResult ScriptEvaluate(LoadedLuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, parameters).GetAwaiter().GetResult();
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, keys, values);
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(hash, keys, values);
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LuaScript, object?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, parameters);
    }

    /// <inheritdoc cref="IScriptingAndFunctionBaseCommands.ScriptEvaluateAsync(LoadedLuaScript, object?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, parameters);
    }
}
