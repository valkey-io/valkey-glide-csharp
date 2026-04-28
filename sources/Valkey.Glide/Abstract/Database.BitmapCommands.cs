// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc/>
    public Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetBitAsync(key, offset);
    }

    /// <inheritdoc/>
    public Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetBitAsync(key, offset, value);
    }

    /// <inheritdoc/>
    public Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return BitCountAsync(key, start, end, indexType.ToBitmapIndexType());
    }

    /// <inheritdoc/>
    public Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return BitPosAsync(key, bit, start, end, indexType.ToBitmapIndexType());
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second = default, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        // If second is default, only use first key
        return second.Equals(default)
            ? BitOpAsync(operation, destination, [first])
            : BitOpAsync(operation, destination, [first, second]);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return BitOpAsync(operation, destination, keys);
    }
}
