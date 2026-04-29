// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IBitmapBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IBaseClient.GetBitAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringGetBitAsync(ValkeyKey key, long offset, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SetBitAsync(ValkeyKey, long, bool)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.BitCountAsync(ValkeyKey, long, long, BitmapIndexType)" path="/*[not(self::param[@name='indexType'])]"/>
    /// <param name="indexType">The index type (bit or byte).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.BitPosAsync(ValkeyKey, bool, long, long, BitmapIndexType)" path="/*[not(self::param[@name='indexType'])]"/>
    /// <param name="indexType">The index type (bit or byte).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.BitOpAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})" path="/summary"/>
    /// <seealso href="https://valkey.io/commands/bitop/">Valkey commands – BITOP</seealso>
    /// <param name="operation">The bitwise operation to perform.</param>
    /// <param name="destination">The key to store the result.</param>
    /// <param name="first">The first source key.</param>
    /// <param name="second">The second source key. If default, only <paramref name="first"/> is used.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The size of the string stored in the destination key.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second = default, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.BitOpAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);
}
