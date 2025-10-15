// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Bitmap Commands" group for standalone and cluster clients.
/// <br/>
/// See more on <see href="https://valkey.io/commands#bitmap">valkey.io</see>.
/// </summary>
public interface IBitmapCommands
{
    /// <summary>
    /// Returns the bit value at offset in the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getbit"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to get the bit at.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The bit value stored at offset. Returns 0 if the key does not exist or if the offset is beyond the string length.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.StringSetAsync("mykey", "A"); // ASCII 'A' is 01000001
    /// bool bit = await client.StringGetBitAsync("mykey", 1);
    /// Console.WriteLine(bit); // Output: true (bit 1 is set)
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets or clears the bit at offset in the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/setbit"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to set the bit at.</param>
    /// <param name="value">The bit value to set (true for 1, false for 0).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The original bit value stored at offset.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool oldBit = await client.StringSetBitAsync("mykey", 1, true);
    /// Console.WriteLine(oldBit); // Output: false (original bit value)
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Count the number of set bits in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitcount"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The start offset.</param>
    /// <param name="end">The end offset.</param>
    /// <param name="indexType">The index type (bit or byte).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of bits set to 1.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.StringSetAsync("mykey", "A"); // ASCII 'A' is 01000001
    /// long count = await client.StringBitCountAsync("mykey");
    /// Console.WriteLine(count); // Output: 2 (two bits set)
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None);
}