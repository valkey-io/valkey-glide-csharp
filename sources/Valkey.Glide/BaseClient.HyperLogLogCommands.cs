// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IHyperLogLogCommands
{
    /// <inheritdoc/>
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HyperLogLogAddAsync(key, element));
    }

    /// <inheritdoc/>
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HyperLogLogAddAsync(key, [.. elements]));
    }

    /// <inheritdoc/>
    public async Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HyperLogLogLengthAsync(key));
    }

    /// <inheritdoc/>
    public async Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HyperLogLogLengthAsync([.. keys]));
    }

    /// <inheritdoc/>
    public async Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.HyperLogLogMergeAsync(destination, first, second));
    }

    /// <inheritdoc/>
    public async Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await Command(Request.HyperLogLogMergeAsync(destination, [.. sourceKeys]));
    }
}
