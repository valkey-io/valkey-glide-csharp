// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogAddAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element)
        => await Command(Request.HyperLogLogAdd(key, element));

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogAddAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements)
        => await Command(Request.HyperLogLogAdd(key, [.. elements]));

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogLengthAsync(ValkeyKey)"/>
    public async Task<long> HyperLogLogLengthAsync(ValkeyKey key)
        => await Command(Request.HyperLogLogLength(key));

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogLengthAsync(IEnumerable{ValkeyKey})"/>
    public async Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.HyperLogLogLength([.. keys]));

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogMergeAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    public async Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second)
        => _ = await Command(Request.HyperLogLogMerge(destination, first, second));

    /// <inheritdoc cref="IHyperLogLogBaseCommands.HyperLogLogMergeAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    public async Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys)
        => _ = await Command(Request.HyperLogLogMerge(destination, [.. sourceKeys]));
}
