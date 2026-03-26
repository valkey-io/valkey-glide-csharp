// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Bitmap commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IBitmapCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IBitmapCommands.StringGetBitAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringSetBitAsync(ValkeyKey, long, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitCountAsync(ValkeyKey, long, long, StringIndexType)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringBitCountAsync(ValkeyKey key, long start, long end, StringIndexType indexType, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start, long end, StringIndexType indexType, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitFieldAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldSubCommand})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> StringBitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitFieldReadOnlyAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldReadOnlySubCommand})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long[]> StringBitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands, CommandFlags flags);
}
