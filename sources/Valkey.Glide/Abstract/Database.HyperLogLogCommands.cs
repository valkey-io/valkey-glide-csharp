// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// HyperLogLog commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IHyperLogLogCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogAddAsync(key, element);
    }

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogAddAsync(key, elements);
    }

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogLengthAsync(key);
    }

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogLengthAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HyperLogLogLengthAsync(keys);
    }

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await HyperLogLogMergeAsync(destination, first, second);
    }

    /// <inheritdoc cref="IHyperLogLogCommands.HyperLogLogMergeAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await HyperLogLogMergeAsync(destination, sourceKeys);
    }
}
