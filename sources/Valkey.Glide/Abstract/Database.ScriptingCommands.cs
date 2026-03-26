// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <inheritdoc cref="IDatabaseAsync" path="//*[not(self::seealso)]"/>
/// <seealso cref="IScriptingAndFunctionBaseCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public ValkeyResult ScriptEvaluate(
        string script,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, keys, values).GetAwaiter().GetResult();
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public ValkeyResult ScriptEvaluate(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(hash, keys, values).GetAwaiter().GetResult();
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(LuaScript, object?, CommandFlags)"/>
    public ValkeyResult ScriptEvaluate(LuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, parameters).GetAwaiter().GetResult();
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(LoadedLuaScript, object?, CommandFlags)"/>
    public ValkeyResult ScriptEvaluate(LoadedLuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, parameters).GetAwaiter().GetResult();
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, keys, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(hash, keys, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(LuaScript, object?, CommandFlags)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, parameters);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(LoadedLuaScript, object?, CommandFlags)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, parameters);
    }
}
