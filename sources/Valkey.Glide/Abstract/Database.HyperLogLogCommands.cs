// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region HyperLogLog Commands with CommandFlags (SER Compatibility)

    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogAddAsync(key, element);
    }

    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogAddAsync(key, elements);
    }

    public async Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogLengthAsync(key);
    }

    public async Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogLengthAsync(keys);
    }

    public async Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await HyperLogLogMergeAsync(destination, first, second);
    }

    public async Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await HyperLogLogMergeAsync(destination, sourceKeys);
    }

    #endregion
}
