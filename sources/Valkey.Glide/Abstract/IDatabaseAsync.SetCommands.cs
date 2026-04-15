// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="ISetBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetMembersAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> SetPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetUnionAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetUnionAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetUnionAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetUnionAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetIntersectAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetIntersectAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetDifferenceAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetDifferenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetDifferenceAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetDifferenceAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetUnionStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetIntersectStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetIntersectStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetDifferenceStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SetDifferenceStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetContainsAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetRandomMemberAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetRandomMembersAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISetBaseCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value, CommandFlags flags);

    /// <summary>
    /// Iterates elements over a set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sscan"/>
    /// <param name="key">The key of the set.</param>
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="pageSize">The number of elements to return per iteration (hint to the server).</param>
    /// <param name="cursor">The cursor position to start at.</param>
    /// <param name="pageOffset">The number of elements to skip from the first page.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);
}
