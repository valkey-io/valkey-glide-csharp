// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IHyperLogLogCommands
{
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element)
        => await Command(Request.HyperLogLogAddAsync(key, element));

    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> elements)
        => await Command(Request.HyperLogLogAddAsync(key, [.. elements]));

    public async Task<long> HyperLogLogLengthAsync(ValkeyKey key)
        => await Command(Request.HyperLogLogLengthAsync(key));

    public async Task<long> HyperLogLogLengthAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.HyperLogLogLengthAsync([.. keys]));

    public async Task HyperLogLogMergeAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second)
        => _ = await Command(Request.HyperLogLogMergeAsync(destination, first, second));

    public async Task HyperLogLogMergeAsync(ValkeyKey destination, IEnumerable<ValkeyKey> sourceKeys)
        => _ = await Command(Request.HyperLogLogMergeAsync(destination, [.. sourceKeys]));
}
