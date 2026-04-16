// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogAddAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HyperLogLogAddAsync(key, element);
    }

    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HyperLogLogAddAsync(key, elements);
    }

    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HyperLogLogLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogLengthAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HyperLogLogLengthAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await HyperLogLogMergeAsync(destination, first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogMergeAsync(ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await HyperLogLogMergeAsync(destination, sourceKeys);
    }
}
