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
}