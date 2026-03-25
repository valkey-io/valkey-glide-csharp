// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Bitmap Commands with CommandFlags (SER Compatibility)

    public async Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetBitAsync(key, offset);
    }

    public async Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetBitAsync(key, offset, value);
    }

    public async Task<long> StringBitCountAsync(ValkeyKey key, long start, long end, StringIndexType indexType, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitCountAsync(key, start, end, indexType);
    }

    public async Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start, long end, StringIndexType indexType, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitPositionAsync(key, bit, start, end, indexType);
    }

    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitOperationAsync(operation, destination, first, second);
    }

    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitOperationAsync(operation, destination, keys);
    }

    public async Task<long[]> StringBitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitFieldAsync(key, subCommands);
    }

    public async Task<long[]> StringBitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitFieldReadOnlyAsync(key, subCommands);
    }

    #endregion
}
