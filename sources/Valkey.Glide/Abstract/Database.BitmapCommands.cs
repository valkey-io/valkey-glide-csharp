// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Bitmap commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IBitmapCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IBitmapCommands.StringGetBitAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetBitAsync(key, offset);
    }

    /// <inheritdoc cref="IBitmapCommands.StringSetBitAsync(ValkeyKey, long, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetBitAsync(key, offset, value);
    }

    /// <inheritdoc cref="IBitmapCommands.StringBitCountAsync(ValkeyKey, long, long, StringIndexType)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringBitCountAsync(ValkeyKey key, long start, long end, StringIndexType indexType, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitCountAsync(key, start, end, indexType);
    }

    /// <inheritdoc cref="IBitmapCommands.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start, long end, StringIndexType indexType, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitPositionAsync(key, bit, start, end, indexType);
    }

    /// <inheritdoc cref="IBitmapCommands.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitOperationAsync(operation, destination, first, second);
    }

    /// <inheritdoc cref="IBitmapCommands.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitOperationAsync(operation, destination, keys);
    }

    /// <inheritdoc cref="IBitmapCommands.StringBitFieldAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldSubCommand})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> StringBitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitFieldAsync(key, subCommands);
    }

    /// <inheritdoc cref="IBitmapCommands.StringBitFieldReadOnlyAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldReadOnlySubCommand})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> StringBitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringBitFieldReadOnlyAsync(key, subCommands);
    }
}
