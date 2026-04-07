// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// <summary>
/// Bitmap commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <remarks>
/// <para>
/// This interface provides StackExchange.Redis-compatible method signatures with "String" prefix
/// and <see cref="StringIndexType"/>. For Valkey GLIDE-native methods without the "String" prefix
/// and using <see cref="BitmapIndexType"/>, use <see cref="IBaseClient"/>.
/// </para>
/// <para>
/// BITFIELD and BITFIELD_RO are not supported by StackExchange.Redis and are available
/// only through <see cref="IBaseClient"/>.
/// </para>
/// </remarks>
public partial interface IDatabaseAsync
{
    /// <summary>
    /// Returns the bit value at offset in the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getbit"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to get the bit at.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The bit value stored at offset. Returns false if the key does not exist or if the offset is beyond the string length.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets or clears the bit at offset in the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/setbit"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="offset">The offset in the string to set the bit at.</param>
    /// <param name="value">The bit value to set (true for 1, false for 0).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The original bit value stored at offset.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Count the number of set bits in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitcount"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The start offset. Default is 0.</param>
    /// <param name="end">The end offset. Default is -1 (end of string).</param>
    /// <param name="indexType">The index type (bit or byte). Default is <see cref="StringIndexType.Byte"/>.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of bits set to 1.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Return the position of the first bit set to 1 or 0 in a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitpos"/>
    /// <param name="key">The key of the string.</param>
    /// <param name="bit">The bit value to search for (true for 1, false for 0).</param>
    /// <param name="start">The start offset. Default is 0.</param>
    /// <param name="end">The end offset. Default is -1 (end of string).</param>
    /// <param name="indexType">The index type (bit or byte). Default is <see cref="StringIndexType.Byte"/>.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The position of the first bit with the specified value, or -1 if not found.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Perform a bitwise operation between two keys and store the result in the destination key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitop"/>
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="first">The first source key.</param>
    /// <param name="second">The second source key. If default, only <paramref name="first"/> is used.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The size of the string stored in the destination key.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second = default, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Perform a bitwise operation between multiple keys and store the result in the destination key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bitop"/>
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="keys">The source keys.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The size of the string stored in the destination key.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);
}
