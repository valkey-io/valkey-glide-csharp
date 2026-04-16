// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, keys, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(hash, keys, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(LuaScript, object?, CommandFlags)"/>
    public Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, parameters);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(LoadedLuaScript, object?, CommandFlags)"/>
    public Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, parameters);
    }
}
