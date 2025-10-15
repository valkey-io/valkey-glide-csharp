// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IBitmapCommands
{
    /// <inheritdoc/>
    public async Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.GetBitAsync(key, offset));
    }

    /// <inheritdoc/>
    public async Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.SetBitAsync(key, offset, value));
    }

    /// <inheritdoc/>
    public async Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.BitCountAsync(key, start, end, indexType));
    }

    /// <inheritdoc/>
    public async Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.BitPositionAsync(key, bit, start, end, indexType));
    }

    /// <inheritdoc/>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.BitOperationAsync(operation, destination, first, second));
    }

    /// <inheritdoc/>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey[] keys, CommandFlags flags = CommandFlags.None)
    {
        Utils.Requires<NotImplementedException>(flags == CommandFlags.None, "Command flags are not supported by GLIDE");
        return await Command(Request.BitOperationAsync(operation, destination, keys));
    }
}