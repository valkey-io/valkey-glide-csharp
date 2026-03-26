// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.StringGetBitAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetBitAsync(key, offset);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetBitAsync(ValkeyKey, long, bool, CommandFlags)"/>
    public async Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetBitAsync(key, offset, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitCountAsync(ValkeyKey, long, long, StringIndexType, CommandFlags)"/>
    public async Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitCountAsync(key, start, end, indexType);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType, CommandFlags)"/>
    public async Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitPositionAsync(key, bit, start, end, indexType);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitOperationAsync(operation, destination, first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitOperationAsync(operation, destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitFieldAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldSubCommand}, CommandFlags)"/>
    public async Task<long[]> StringBitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitFieldAsync(key, subCommands);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringBitFieldReadOnlyAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldReadOnlySubCommand}, CommandFlags)"/>
    public async Task<long[]> StringBitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitFieldReadOnlyAsync(key, subCommands);
    }
}
