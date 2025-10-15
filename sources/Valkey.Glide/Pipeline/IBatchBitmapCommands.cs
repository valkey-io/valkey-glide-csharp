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
}