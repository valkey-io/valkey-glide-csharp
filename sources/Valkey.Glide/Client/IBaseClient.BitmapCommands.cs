// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Bitmap commands for Valkey GLIDE clients.
/// </summary>
/// <remarks>
/// These methods use Valkey GLIDE naming conventions. For StackExchange.Redis-compatible
/// methods with "String" prefix, use <see cref="IDatabaseAsync"/>.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#bitmap">Valkey – Bitmap Commands</seealso>
public partial interface IBaseClient
{
    /// <summary>
    /// Returns the bit value at offset in the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getbit"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to get the bit at.</param>
    /// <returns>The bit value stored at offset. Returns false if the key does not exist or if the offset is beyond the string length.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool bit = await client.GetBitAsync("mykey", 7);
    /// Console.WriteLine($"Bit at offset 7: {bit}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> GetBitAsync(ValkeyKey key, long offset);

    /// <summary>
    /// Sets or clears the bit at offset in the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/setbit"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to set the bit at.</param>
    /// <param name="value">The bit value to set (true for 1, false for 0).</param>
    /// <returns>The original bit value stored at offset.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool previousBit = await client.SetBitAsync("mykey", 7, true);
    /// Console.WriteLine($"Previous bit value: {previousBit}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetBitAsync(ValkeyKey key, long offset, bool value);

    /// <summary>
    /// Count the number of set bits in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitcount"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The start offset. Default is 0.</param>
    /// <param name="end">The end offset. Default is -1 (end of string).</param>
    /// <param name="indexType">The index type (bit or byte). Default is <see cref="BitmapIndexType.Byte"/>.</param>
    /// <returns>The number of bits set to 1.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Count all set bits in the string
    /// long count = await client.BitCountAsync("mykey");
    /// Console.WriteLine($"Total bits set: {count}");
    ///
    /// // Count set bits in byte range [0, 1]
    /// long rangeCount = await client.BitCountAsync("mykey", 0, 1, BitmapIndexType.Byte);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitCountAsync(ValkeyKey key, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte);

    /// <summary>
    /// Count the number of set bits in a string using offset options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitcount"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="options">The offset options specifying start, end, and index type. If null, defaults are used.</param>
    /// <returns>The number of bits set to 1.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long count = await client.BitCountAsync("mykey", BitOffsetOptions.InBitRange(0, 10));
    /// Console.WriteLine($"Bits set in range: {count}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitCountAsync(ValkeyKey key, BitOffsetOptions? options);

    /// <summary>
    /// Return the position of the first bit set to 1 or 0 in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitpos"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="bit">The bit value to search for (true for 1, false for 0).</param>
    /// <param name="start">The start offset. Default is 0.</param>
    /// <param name="end">The end offset. Default is -1 (end of string).</param>
    /// <param name="indexType">The index type (bit or byte). Default is <see cref="BitmapIndexType.Byte"/>.</param>
    /// <returns>The position of the first bit with the specified value, or -1 if not found.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Find the first set bit (1)
    /// long pos = await client.BitPosAsync("mykey", true);
    /// Console.WriteLine($"First set bit at position: {pos}");
    ///
    /// // Find the first clear bit (0) in byte range [1, 3]
    /// long clearPos = await client.BitPosAsync("mykey", false, 1, 3, BitmapIndexType.Byte);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitPosAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte);

    /// <summary>
    /// Return the position of the first bit set to 1 or 0 in a string using offset options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitpos"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="bit">The bit value to search for (true for 1, false for 0).</param>
    /// <param name="options">The offset options specifying start, end, and index type. If null, defaults are used.</param>
    /// <returns>The position of the first bit with the specified value, or -1 if not found.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long pos = await client.BitPosAsync("mykey", true, BitOffsetOptions.InBitRange(0, 100));
    /// Console.WriteLine($"First set bit in range: {pos}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitPosAsync(ValkeyKey key, bool bit, BitOffsetOptions? options);

    /// <summary>
    /// Perform a bitwise operation between multiple keys and store the result in the destination key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitop"/>
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="keys">The source keys. Must contain at least one key. For NOT operation, must contain exactly one key.</param>
    /// <returns>The size of the string stored in the destination key.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="keys"/> is empty.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// var size = await client.BitOpAsync(Bitwise.And, "result", ["key1", "key2"]);
    /// Console.WriteLine($"Result string size: {size} bytes");
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var size = await client.BitOpAsync(Bitwise.Not, "inverted", ["key1"]);
    /// Console.WriteLine($"Result string size: {size} bytes");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitOpAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Reads or modifies the array of bits representing the string stored at key based on the specified subcommands.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The subcommands to execute (GET, SET, INCRBY, OVERFLOW).</param>
    /// <returns>
    /// An array of results from the executed subcommands.
    /// Null values indicate overflow when using OVERFLOW FAIL.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var subCommands = new BitFieldOptions.IBitFieldSubCommand[]
    /// {
    ///     new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Signed(8), new BitFieldOptions.BitOffset(0)),
    ///     new BitFieldOptions.BitFieldSet(BitFieldOptions.Encoding.Unsigned(4), new BitFieldOptions.BitOffset(8), 15),
    ///     new BitFieldOptions.BitFieldIncrBy(BitFieldOptions.Encoding.Signed(8), new BitFieldOptions.BitOffset(0), 1)
    /// };
    /// long?[] results = await client.BitFieldAsync("mykey", subCommands);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?[]> BitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands);

    /// <summary>
    /// Reads or modifies the array of bits representing the string stored at key based on a single subcommand.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommand">The subcommand to execute (GET, SET, or INCRBY).</param>
    /// <returns>
    /// The result of the subcommand, or null if overflow occurred with OVERFLOW FAIL.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var encoding = BitFieldOptions.Encoding.Signed(8);
    /// var offset = new BitFieldOptions.BitOffset(0);
    /// var getCommand = new BitFieldOptions.BitFieldGet(encoding, offset);
    /// var fieldValue = await client.BitFieldAsync("mykey", getCommand);
    /// Console.WriteLine($"BitField value: {fieldValue}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?> BitFieldAsync(ValkeyKey key, BitFieldOptions.IBitFieldSubCommand subCommand);

    /// <summary>
    /// Reads the array of bits representing the string stored at key based on the specified GET subcommands.
    /// This is a read-only variant of BITFIELD that can be routed to replicas.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield_ro"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The GET subcommands to execute.</param>
    /// <returns>An array of results from the executed GET subcommands.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var subCommands = new BitFieldOptions.IBitFieldReadOnlySubCommand[]
    /// {
    ///     new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Signed(8), new BitFieldOptions.BitOffset(0)),
    ///     new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(16), new BitFieldOptions.BitOffset(8))
    /// };
    /// long[] results = await client.BitFieldReadOnlyAsync("mykey", subCommands);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> BitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands);

    /// <summary>
    /// Reads the bits representing the string stored at key based on a single GET subcommand.
    /// This is a read-only variant of BITFIELD that can be routed to replicas.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield_ro"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommand">The GET subcommand to execute.</param>
    /// <returns>The result of the GET subcommand.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long value = await client.BitFieldReadOnlyAsync("mykey", new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0)));
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitFieldReadOnlyAsync(ValkeyKey key, BitFieldOptions.IBitFieldReadOnlySubCommand subCommand);
}
