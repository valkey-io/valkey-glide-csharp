// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.StringGetBitAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetBitAsync(key, offset);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetBitAsync(ValkeyKey, long, bool, CommandFlags)"/>
    public Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetBitAsync(key, offset, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitCountAsync(ValkeyKey, long, long, StringIndexType, CommandFlags)"/>
    public Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return BitCountAsync(key, start, end, indexType.ToBitmapIndexType());
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType, CommandFlags)"/>
    public Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return BitPosAsync(key, bit, start, end, indexType.ToBitmapIndexType());
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
    public Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return BitOpAsync(operation, destination, keys);
    }
}
