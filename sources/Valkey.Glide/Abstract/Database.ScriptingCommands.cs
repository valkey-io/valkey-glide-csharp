// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Scripting Commands with CommandFlags (SER Compatibility)

    public ValkeyResult ScriptEvaluate(
        string script,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, keys, values).GetAwaiter().GetResult();
    }

    public ValkeyResult ScriptEvaluate(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys = null,
        IEnumerable<ValkeyValue>? values = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(hash, keys, values).GetAwaiter().GetResult();
    }

    public ValkeyResult ScriptEvaluate(LuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, parameters).GetAwaiter().GetResult();
    }

    public ValkeyResult ScriptEvaluate(LoadedLuaScript script, object? parameters = null,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ScriptEvaluateAsync(script, parameters).GetAwaiter().GetResult();
    }

    public async Task<ValkeyResult> ScriptEvaluateAsync(
        string script,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, keys, values);
    }

    public async Task<ValkeyResult> ScriptEvaluateAsync(
        byte[] hash,
        IEnumerable<ValkeyKey>? keys,
        IEnumerable<ValkeyValue>? values,
        CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(hash, keys, values);
    }

    public async Task<ValkeyResult> ScriptEvaluateAsync(LuaScript script, object? parameters, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, parameters);
    }

    public async Task<ValkeyResult> ScriptEvaluateAsync(LoadedLuaScript script, object? parameters, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ScriptEvaluateAsync(script, parameters);
    }

    #endregion
}
