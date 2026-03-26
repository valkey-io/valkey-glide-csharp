// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <inheritdoc cref="IDatabaseAsync" path="//*[not(self::seealso)]"/>
/// <seealso cref="IHyperLogLogCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogAddAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogAddAsync(key, element);
    }

    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogAddAsync(key, elements);
    }

    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogLengthAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HyperLogLogLengthAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogLengthAsync(keys);
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
