// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IHyperLogLogCommands
{
    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue element, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.HyperLogLogAddAsync(key, element));
    }

    public async Task<bool> HyperLogLogAddAsync(ValkeyKey key, ValkeyValue[] elements, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.HyperLogLogAddAsync(key, elements));
    }

    public async Task<long> HyperLogLogLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.HyperLogLogLengthAsync(key));
    }

    public async Task<long> HyperLogLogLengthAsync(ValkeyKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.HyperLogLogLengthAsync(keys));
    }
}