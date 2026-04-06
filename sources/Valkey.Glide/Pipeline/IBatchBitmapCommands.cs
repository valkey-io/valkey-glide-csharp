// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Bitmap Commands" group for batch requests.
/// </summary>
internal interface IBatchBitmapCommands
{
    /// <inheritdoc cref="Commands.IBitmapBaseCommands.StringGetBitAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapBaseCommands.StringGetBitAsync(ValkeyKey, long)" /></returns>
    IBatch StringGetBit(ValkeyKey key, long offset);

    /// <inheritdoc cref="Commands.IBitmapBaseCommands.StringSetBitAsync(ValkeyKey, long, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapBaseCommands.StringSetBitAsync(ValkeyKey, long, bool)" /></returns>
    IBatch StringSetBit(ValkeyKey key, long offset, bool value);

    /// <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitCountAsync(ValkeyKey, long, long, StringIndexType)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitCountAsync(ValkeyKey, long, long, StringIndexType)" /></returns>
    IBatch StringBitCount(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte);

    /// <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitPositionAsync(ValkeyKey, bool, long, long, StringIndexType)" /></returns>
    IBatch StringBitPosition(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte);

    /// <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitOperationAsync(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey)" /></returns>
    IBatch StringBitOperation(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitOperationAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch StringBitOperation(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitFieldAsync(ValkeyKey, IEnumerable{Commands.Options.BitFieldOptions.IBitFieldSubCommand})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitFieldAsync(ValkeyKey, IEnumerable{Commands.Options.BitFieldOptions.IBitFieldSubCommand})" /></returns>
    IBatch StringBitField(ValkeyKey key, IEnumerable<Commands.Options.BitFieldOptions.IBitFieldSubCommand> subCommands);

    /// <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitFieldReadOnlyAsync(ValkeyKey, IEnumerable{Commands.Options.BitFieldOptions.IBitFieldReadOnlySubCommand})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IBitmapBaseCommands.StringBitFieldReadOnlyAsync(ValkeyKey, IEnumerable{Commands.Options.BitFieldOptions.IBitFieldReadOnlySubCommand})" /></returns>
    IBatch StringBitFieldReadOnly(ValkeyKey key, IEnumerable<Commands.Options.BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands);
}
