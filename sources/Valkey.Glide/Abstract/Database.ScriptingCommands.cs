// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(string, IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, keys, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(byte[], IEnumerable{ValkeyKey}?, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(hash, keys, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(LuaScript, object?, CommandFlags)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, parameters);
    }

    /// <inheritdoc cref="IDatabaseAsync.ScriptEvaluateAsync(LoadedLuaScript, object?, CommandFlags)"/>
    public async Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, parameters);
    }
}
