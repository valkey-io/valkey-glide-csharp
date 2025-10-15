// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Bitmap Commands" group for batch requests.
/// </summary>
internal interface IBatchBitmapCommands
{
    /// <inheritdoc cref="Commands.IBitmapCommands.StringGetBitAsync(ValkeyKey, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapCommands.StringGetBitAsync(ValkeyKey, long, CommandFlags)" /></returns>
    IBatch StringGetBit(ValkeyKey key, long offset);

    /// <inheritdoc cref="Commands.IBitmapCommands.StringSetBitAsync(ValkeyKey, long, bool, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapCommands.StringSetBitAsync(ValkeyKey, long, bool, CommandFlags)" /></returns>
    IBatch StringSetBit(ValkeyKey key, long offset, bool value);

    /// <inheritdoc cref="Commands.IBitmapCommands.StringBitCountAsync(ValkeyKey, long, long, StringIndexType, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapCommands.StringBitCountAsync(ValkeyKey, long, long, StringIndexType, CommandFlags)" /></returns>
    IBatch StringBitCount(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte);
}