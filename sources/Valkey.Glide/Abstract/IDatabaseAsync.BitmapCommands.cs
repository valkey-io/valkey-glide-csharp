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
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringSetBitAsync(ValkeyKey, long, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitCountAsync(ValkeyKey, long, long, StringIndexType)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBitmapCommands.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBitmapCommands.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitFieldAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldSubCommand})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long[]> StringBitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands, CommandFlags flags);

    /// <inheritdoc cref="IBitmapCommands.StringBitFieldReadOnlyAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldReadOnlySubCommand})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long[]> StringBitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands, CommandFlags flags);
}
