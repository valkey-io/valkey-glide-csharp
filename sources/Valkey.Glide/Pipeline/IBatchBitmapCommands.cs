// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Bitmap Commands" group for batch requests.
/// </summary>
/// <seealso href="https://valkey.io/commands/#bitmap">Valkey – Bitmap Commands</seealso>
internal interface IBatchBitmapCommands
{
    /// <inheritdoc cref="IBaseClient.GetBitAsync(ValkeyKey, long)"/>
    /// <returns>Command Response - The bit value stored at offset.</returns>
    IBatch GetBit(ValkeyKey key, long offset);

    /// <inheritdoc cref="IBaseClient.SetBitAsync(ValkeyKey, long, bool)"/>
    /// <returns>Command Response - The original bit value stored at offset.</returns>
    IBatch SetBit(ValkeyKey key, long offset, bool value);

    /// <inheritdoc cref="IBaseClient.BitCountAsync(ValkeyKey, long, long, BitmapIndexType)"/>
    /// <returns>Command Response - The number of bits set to 1.</returns>
    IBatch BitCount(ValkeyKey key, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte);

    /// <inheritdoc cref="IBaseClient.BitPosAsync(ValkeyKey, bool, long, long, BitmapIndexType)"/>
    /// <returns>Command Response - The position of the first bit with the specified value, or -1 if not found.</returns>
    IBatch BitPos(ValkeyKey key, bool bit, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte);

    /// <summary>
    /// Perform a bitwise operation between two keys and store the result in the destination key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitop"/>
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="first">The first source key.</param>
    /// <param name="second">The second source key.</param>
    /// <returns>Command Response - The size of the string stored in the destination key.</returns>
    IBatch BitOp(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IBaseClient.BitOpAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <returns>Command Response - The size of the string stored in the destination key.</returns>
    IBatch BitOp(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.BitFieldAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldSubCommand})"/>
    /// <returns>Command Response - An array of results from the executed subcommands.</returns>
    IBatch BitField(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands);

    /// <inheritdoc cref="IBaseClient.BitFieldReadOnlyAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldReadOnlySubCommand})"/>
    /// <returns>Command Response - An array of results from the executed GET subcommands.</returns>
    IBatch BitFieldReadOnly(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands);
}
