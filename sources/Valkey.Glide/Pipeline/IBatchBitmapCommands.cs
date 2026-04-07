// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Bitmap Commands" group for batch requests.
/// </summary>
internal interface IBatchBitmapCommands
{
    /// <summary>
    /// Returns the bit value at offset in the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getbit"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to get the bit at.</param>
    /// <returns>Command Response - The bit value stored at offset.</returns>
    IBatch StringGetBit(ValkeyKey key, long offset);

    /// <summary>
    /// Sets or clears the bit at offset in the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/setbit"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to set the bit at.</param>
    /// <param name="value">The bit value to set (true for 1, false for 0).</param>
    /// <returns>Command Response - The original bit value stored at offset.</returns>
    IBatch StringSetBit(ValkeyKey key, long offset, bool value);

    /// <summary>
    /// Count the number of set bits in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitcount"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The start offset. Default is 0.</param>
    /// <param name="end">The end offset. Default is -1 (end of string).</param>
    /// <param name="indexType">The index type (bit or byte). Default is <see cref="StringIndexType.Byte"/>.</param>
    /// <returns>Command Response - The number of bits set to 1.</returns>
    IBatch StringBitCount(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte);

    /// <summary>
    /// Return the position of the first bit set to 1 or 0 in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitpos"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="bit">The bit value to search for (true for 1, false for 0).</param>
    /// <param name="start">The start offset. Default is 0.</param>
    /// <param name="end">The end offset. Default is -1 (end of string).</param>
    /// <param name="indexType">The index type (bit or byte). Default is <see cref="StringIndexType.Byte"/>.</param>
    /// <returns>Command Response - The position of the first bit with the specified value, or -1 if not found.</returns>
    IBatch StringBitPosition(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte);

    /// <summary>
    /// Perform a bitwise operation between two keys and store the result in the destination key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitop"/>
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="first">The first source key.</param>
    /// <param name="second">The second source key.</param>
    /// <returns>Command Response - The size of the string stored in the destination key.</returns>
    IBatch StringBitOperation(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <summary>
    /// Perform a bitwise operation between multiple keys and store the result in the destination key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitop"/>
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="keys">The source keys.</param>
    /// <returns>Command Response - The size of the string stored in the destination key.</returns>
    IBatch StringBitOperation(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Reads or modifies the array of bits representing the string stored at key based on the specified subcommands.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The subcommands to execute (GET, SET, INCRBY, OVERFLOW).</param>
    /// <returns>Command Response - An array of results from the executed subcommands.</returns>
    IBatch StringBitField(ValkeyKey key, IEnumerable<Commands.Options.BitFieldOptions.IBitFieldSubCommand> subCommands);

    /// <summary>
    /// Reads the array of bits representing the string stored at key based on the specified GET subcommands.
    /// This is a read-only variant of BITFIELD that can be routed to replicas.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield_ro"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The GET subcommands to execute.</param>
    /// <returns>Command Response - An array of results from the executed GET subcommands.</returns>
    IBatch StringBitFieldReadOnly(ValkeyKey key, IEnumerable<Commands.Options.BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands);
}
