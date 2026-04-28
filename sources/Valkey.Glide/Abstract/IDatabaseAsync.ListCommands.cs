// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IListBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <summary>
    /// Inserts a value at the head of a list.
    /// If the key does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush/">Valkey commands – LPUSH</seealso>
    /// <seealso href="https://valkey.io/commands/lpushx/">Valkey commands – LPUSHX</seealso>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the head of the list.</param>
    /// <param name="when">Use <see cref="When.Exists"/> for LPUSHX behavior (only push if key exists).</param>
    /// <returns>The length of the list after the push operation.</returns>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when);

    /// <summary>
    /// Inserts all specified values at the head of a list. Elements are inserted one
    /// after the other to the head of the list, from the leftmost element to the rightmost element.
    /// If the key does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush/">Valkey commands – LPUSH</seealso>
    /// <seealso href="https://valkey.io/commands/lpushx/">Valkey commands – LPUSHX</seealso>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the head of the list.</param>
    /// <param name="when">Use <see cref="When.Exists"/> for LPUSHX behavior (only push if key exists).</param>
    /// <returns>The length of the list after the push operation.</returns>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when);

    /// <summary>
    /// Inserts a value at the tail of a list.
    /// If the key does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush/">Valkey commands – RPUSH</seealso>
    /// <seealso href="https://valkey.io/commands/rpushx/">Valkey commands – RPUSHX</seealso>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the tail of the list.</param>
    /// <param name="when">Use <see cref="When.Exists"/> for RPUSHX behavior (only push if key exists).</param>
    /// <returns>The length of the list after the push operation.</returns>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when);

    /// <summary>
    /// Inserts all specified values at the tail of a list.
    /// Elements are inserted one after the other to the tail of the list, from the leftmost element to the rightmost element.
    /// If the key does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush/">Valkey commands – RPUSH</seealso>
    /// <seealso href="https://valkey.io/commands/rpushx/">Valkey commands – RPUSHX</seealso>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the tail of the list.</param>
    /// <param name="when">Use <see cref="When.Exists"/> for RPUSHX behavior (only push if key exists).</param>
    /// <returns>The length of the list after the push operation.</returns>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags);

    /// <inheritdoc cref="ListLeftPushAsync(ValkeyKey, ValkeyValue, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags);

    /// <inheritdoc cref="ListRightPushAsync(ValkeyKey, ValkeyValue, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListRightPushAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListTrimAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListRangeAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags);

    /// <summary>
    /// Atomically pops an element from the tail of the source list and pushes it to the head of the destination list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpoplpush/">Valkey commands – RPOPLPUSH</seealso>
    /// <param name="source">The key of the source list.</param>
    /// <param name="destination">The key of the destination list.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The element being popped and pushed, or <see cref="ValkeyValue.Null"/> if the source list is empty.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListRightPopLeftPushAsync(ValkeyKey source, ValkeyKey destination, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IListBaseCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank, long maxLength, CommandFlags flags);

    /// <inheritdoc cref="IListBaseCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank, long maxLength, CommandFlags flags);

    /// <summary>
    /// Returns the element at a given index in a list.
    /// The index is zero-based, so <c>0</c> means the first element, <c>1</c> the second element and so on.
    /// Negative indices can be used to designate elements starting at the tail of the list.
    /// Here, <c>-1</c> means the last element, <c>-2</c> means the penultimate and so forth.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lindex/">Valkey commands – LINDEX</seealso>
    /// <param name="key">The key of the list.</param>
    /// <param name="index">The index of the element in the list to retrieve.</param>
    /// <returns>
    /// The element at the given index.
    /// If the index is out of range or if the key does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index);

    /// <inheritdoc cref="ListGetByIndexAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags);

    /// <summary>
    /// Sets the list element at a given index to a new value.
    /// The index is zero-based, so <c>0</c> means the first element, <c>1</c> the second element and so on.
    /// Negative indices can be used to designate elements starting at the tail of the list.
    /// Here, <c>-1</c> means the last element, <c>-2</c> means the penultimate and so forth.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lset/">Valkey commands – LSET</seealso>
    /// <param name="key">The key of the list.</param>
    /// <param name="index">The index of the element in the list to set.</param>
    /// <param name="value">The new value.</param>
    Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value);

    /// <inheritdoc cref="ListSetByIndexAsync(ValkeyKey, long, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags);
}
