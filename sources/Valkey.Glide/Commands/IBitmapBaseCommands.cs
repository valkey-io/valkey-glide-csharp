// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Commands;

/// <summary>
/// Bitmap commands shared between Valkey GLIDE clients and StackExchange.Redis-compatible interfaces.
/// </summary>
/// <seealso href="https://valkey.io/commands/#bitmap">Valkey – Bitmap Commands</seealso>
/// <remarks>
/// <para>
/// This interface uses Valkey GLIDE naming conventions (no "String" prefix) and <see cref="BitmapIndexType"/>.
/// For StackExchange.Redis compatibility with "String" prefixed methods and <see cref="StringIndexType"/>,
/// use <see cref="IDatabaseAsync"/>.
/// </para>
/// <para>
/// BITFIELD and BITFIELD_RO are not included here as they are not supported by StackExchange.Redis.
/// They are available in <see cref="IBaseClient"/>.
/// </para>
/// </remarks>
public interface IBitmapBaseCommands
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
    /// await client.SetAsync("mykey", "A"); // ASCII 'A' is 01000001
    /// bool bit = await client.GetBitAsync("mykey", 1);
    /// Console.WriteLine(bit); // Output: true (bit 1 is set)
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
    /// <returns>The original bit value stored at offset. Returns false if the key does not exist or if the offset is beyond the string length.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool oldBit = await client.SetBitAsync("mykey", 1, true);
    /// Console.WriteLine(oldBit); // Output: false (original bit value)
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
    /// await client.SetAsync("mykey", "A"); // ASCII 'A' is 01000001
    /// long count = await client.BitCountAsync("mykey");
    /// Console.WriteLine(count); // Output: 2 (two bits set)
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
    /// await client.SetAsync("mykey", "A"); // ASCII 'A' is 01000001
    /// var options = new BitOffsetOptions { Start = 0, End = 7, IndexType = BitmapIndexType.Bit };
    /// long count = await client.BitCountAsync("mykey", options);
    /// Console.WriteLine(count); // Output: 2 (two bits set in first byte)
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
    /// await client.SetAsync("mykey", "A"); // ASCII 'A' is 01000001
    /// long pos = await client.BitPosAsync("mykey", true);
    /// Console.WriteLine(pos); // Output: 1 (first set bit at position 1)
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
    /// await client.SetAsync("mykey", "A"); // ASCII 'A' is 01000001
    /// var options = new BitOffsetOptions { Start = 0, End = 7, IndexType = BitmapIndexType.Bit };
    /// long pos = await client.BitPosAsync("mykey", true, options);
    /// Console.WriteLine(pos); // Output: 1 (first set bit at position 1)
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
    /// await client.SetAsync("key1", "A");
    /// await client.SetAsync("key2", "B");
    /// long size = await client.BitOpAsync(Bitwise.And, "result", ["key1", "key2"]);
    /// Console.WriteLine(size); // Output: 1 (size of result)
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> BitOpAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys);
}
