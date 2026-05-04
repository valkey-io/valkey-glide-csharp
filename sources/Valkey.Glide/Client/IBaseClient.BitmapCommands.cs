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
    /// Returns the bit value at a given offset in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getbit/">Valkey commands – GETBIT</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The bit offset in the string value.</param>
    /// <returns>
    /// The bit value stored at <paramref name="offset"/>. Returns <see langword="false"/> if
    /// <paramref name="key"/> does not exist or if <paramref name="offset"/> is beyond the string length.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var bit = await client.GetBitAsync("mykey", 7);  // false
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> GetBitAsync(ValkeyKey key, long offset);

    /// <summary>
    /// Sets or clears the bit at a given offset in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/setbit/">Valkey commands – SETBIT</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The bit offset in the string value.</param>
    /// <param name="value">The bit value to set (<see langword="true"/> for 1, <see langword="false"/> for 0).</param>
    /// <returns>The original bit value stored at <paramref name="offset"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var previousBit = await client.SetBitAsync("mykey", 7, true);  // false
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetBitAsync(ValkeyKey key, long offset, bool value);

    /// <summary>
    /// Counts the number of set bits (population counting) in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitcount/">Valkey commands – BITCOUNT</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The start offset.</param>
    /// <param name="end">The end offset</param>
    /// <param name="indexType">The index type (bit or byte).</param>
    /// <returns>The number of bits set to 1.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("mykey", "foobar");
    /// var totalCount = await client.BitCountAsync("mykey");  // 26
    /// var rangeCount = await client.BitCountAsync("mykey", 0, 1, BitmapIndexType.Byte);  // 10
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitCountAsync(ValkeyKey key, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte);

    /// <summary>
    /// Counts the number of set bits (population counting) in a string
    /// using offset options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitcount/">Valkey commands – BITCOUNT</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="options">The offset options specifying start, end, and index type.
    /// If <see langword="null"/>, defaults are used.</param>
    /// <returns>The number of bits set to 1.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("mykey", "foobar");
    /// var count = await client.BitCountAsync("mykey", BitOffsetOptions.InBitRange(0, 10));  // 6
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitCountAsync(ValkeyKey key, BitOffsetOptions? options);

    /// <summary>
    /// Returns the position of the first bit set to 1 or 0 in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitpos/">Valkey commands – BITPOS</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="bit">The bit value to search for (<see langword="true"/> for 1, <see langword="false"/> for 0).</param>
    /// <param name="start">The start offset.</param>
    /// <param name="end">The end offset</param>
    /// <param name="indexType">The index type (bit or byte).</param>
    /// <returns>The position of the first bit matching <paramref name="bit"/>, or <c>-1</c> if not found.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("mykey", "foobar");
    /// var firstSet = await client.BitPosAsync("mykey", true);  // 1
    /// var firstClear = await client.BitPosAsync("mykey", false, 1, 3, BitmapIndexType.Byte);  // 8
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitPosAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte);

    /// <summary>
    /// Returns the position of the first bit set to 1 or 0 in a string
    /// using offset options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitpos/">Valkey commands – BITPOS</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="bit">The bit value to search for (<see langword="true"/> for 1, <see langword="false"/> for 0).</param>
    /// <param name="options">The offset options specifying start, end, and index type.
    /// If <see langword="null"/>, defaults are used.</param>
    /// <returns>The position of the first bit matching <paramref name="bit"/>, or <c>-1</c> if not found.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("mykey", "foobar");
    /// var position = await client.BitPosAsync("mykey", true, BitOffsetOptions.InBitRange(0, 47));  // 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitPosAsync(ValkeyKey key, bool bit, BitOffsetOptions? options);

    /// <summary>
    /// Performs a bitwise operation between multiple keys and stores the result in
    /// <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitop/">Valkey commands – BITOP</seealso>
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="keys">The source keys. Must contain at least one key.
    /// For <see cref="Bitwise.Not"/>, must contain exactly one key.</param>
    /// <returns>The size of the string stored in <paramref name="destination"/>, in bytes.</returns>
    /// <exception cref="Errors.RequestException">Thrown when <paramref name="keys"/> is empty.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key1", "abc");
    /// await client.SetAsync("key2", "abd");
    /// var size = await client.BitOpAsync(Bitwise.And, "result", ["key1", "key2"]);  // 3
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// await client.SetAsync("key1", "abc");
    /// var size = await client.BitOpAsync(Bitwise.Not, "inverted", ["key1"]);  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitOpAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Reads or modifies the array of bits representing a string
    /// based on the specified subcommands.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield/">Valkey commands – BITFIELD</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommands">The subcommands to execute (GET, SET, INCRBY, OVERFLOW).</param>
    /// <returns>
    /// An array of results from the executed subcommands.<br/>
    /// <see langword="null"/> values indicate overflow when using OVERFLOW FAIL.
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
    /// var results = await client.BitFieldAsync("mykey", subCommands);  // [0, 0, 1]
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?[]> BitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands);

    /// <summary>
    /// Reads or modifies the array of bits representing a string
    /// based on a single subcommand.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield/">Valkey commands – BITFIELD</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommand">The subcommand to execute (GET, SET, or INCRBY).</param>
    /// <returns>
    /// The result of the subcommand, or <see langword="null"/> if overflow occurred with OVERFLOW FAIL.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var encoding = BitFieldOptions.Encoding.Signed(8);
    /// var offset = new BitFieldOptions.BitOffset(0);
    /// var getCommand = new BitFieldOptions.BitFieldGet(encoding, offset);
    /// var fieldValue = await client.BitFieldAsync("mykey", getCommand);  // 0
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?> BitFieldAsync(ValkeyKey key, BitFieldOptions.IBitFieldSubCommand subCommand);

    /// <summary>
    /// Reads the array of bits representing a string based on the
    /// specified GET subcommands. This is a read-only variant of BITFIELD that can be routed to replicas.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield_ro/">Valkey commands – BITFIELD_RO</seealso>
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
    /// var results = await client.BitFieldReadOnlyAsync("mykey", subCommands);  // [0, 0]
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> BitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands);

    /// <summary>
    /// Reads the bits representing a string based on a single GET
    /// subcommand. This is a read-only variant of BITFIELD that can be routed to replicas.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitfield_ro/">Valkey commands – BITFIELD_RO</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="subCommand">The GET subcommand to execute.</param>
    /// <returns>The result of the GET subcommand.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var value = await client.BitFieldReadOnlyAsync(
    ///     "mykey",
    ///     new BitFieldOptions.BitFieldGet(BitFieldOptions.Encoding.Unsigned(8), new BitFieldOptions.BitOffset(0))
    /// );  // 0
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitFieldReadOnlyAsync(ValkeyKey key, BitFieldOptions.IBitFieldReadOnlySubCommand subCommand);
}
