// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.StringGetBitAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetBitAsync(key, offset);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetBitAsync(ValkeyKey, long, bool, CommandFlags)"/>
    public async Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetBitAsync(key, offset, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitCountAsync(ValkeyKey, long, long, StringIndexType, CommandFlags)"/>
    public async Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await BitCountAsync(key, start, end, indexType.ToBitmapIndexType());
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType, CommandFlags)"/>
    public async Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await BitPosAsync(key, bit, start, end, indexType.ToBitmapIndexType());
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second = default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        // If second is default, only use first key
        if (second.Equals(default(ValkeyKey)))
        {
            return await BitOpAsync(operation, destination, [first]);
        }
        return await BitOpAsync(operation, destination, [first, second]);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await BitOpAsync(operation, destination, keys);
    }
}
