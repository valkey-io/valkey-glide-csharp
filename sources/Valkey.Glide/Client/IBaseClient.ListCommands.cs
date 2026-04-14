// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

/// <summary>
/// List commands for Valkey GLIDE clients.
/// </summary>
/// <remarks>
/// These methods use Valkey GLIDE naming conventions. For StackExchange.Redis-compatible
/// methods with "List" prefix, use <see cref="IDatabaseAsync"/>.
/// </remarks>
/// <seealso href="https://valkey.io/commands/#list">Valkey – List Commands</seealso>
public partial interface IBaseClient
{
    /// <summary>
    /// Pops an element from the head of the first list that is non-empty, with the given <paramref name="keys"/> being checked in the order that
    /// they are given.
    /// Blocks the connection when there are no elements to pop from any of the given lists.
    /// <see cref="ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan)"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(ValkeyKey)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the lists to pop from.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// A two-element array containing the key from which the element was popped and the value of the popped element, formatted as [key, value].
    /// If no element could be popped and the timeout expired, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[]? result = await client.ListBlockingLeftPopAsync(["list1", "list2"], TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout);

    /// <summary>
    /// Pops an element from the head of the list stored at <paramref name="key"/>.
    /// Blocks the connection when there are no elements to pop from the list.
    /// <see cref="ListBlockingLeftPopAsync(ValkeyKey, TimeSpan)"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(ValkeyKey)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blpop"/>
    /// <param name="key">The key of the list to pop from.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// A two-element array containing the key from which the element was popped and the value of the popped element, formatted as [key, value].
    /// If no element could be popped and the timeout expired, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[]? result = await client.ListBlockingLeftPopAsync("mylist", TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingLeftPopAsync(ValkeyKey key, TimeSpan timeout);

    /// <summary>
    /// Pops an element from the tail of the first list that is non-empty, with the given <paramref name="keys"/> being checked in the order that
    /// they are given.
    /// Blocks the connection when there are no elements to pop from any of the given lists.
    /// <see cref="ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan)"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListRightPopAsync(ValkeyKey)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/brpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the lists to pop from.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// A two-element array containing the key from which the element was popped and the value of the popped element, formatted as [key, value].
    /// If no element could be popped and the timeout expired, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[]? result = await client.ListBlockingRightPopAsync(["list1", "list2"], TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout);

    /// <summary>
    /// Pops an element from the tail of the list stored at <paramref name="key"/>.
    /// Blocks the connection when there are no elements to pop from the list.
    /// <see cref="ListBlockingRightPopAsync(ValkeyKey, TimeSpan)"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListRightPopAsync(ValkeyKey)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/brpop"/>
    /// <param name="key">The key of the list to pop from.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// A two-element array containing the key from which the element was popped and the value of the popped element, formatted as [key, value].
    /// If no element could be popped and the timeout expired, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[]? result = await client.ListBlockingRightPopAsync("mylist", TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingRightPopAsync(ValkeyKey key, TimeSpan timeout);

    /// <summary>
    /// Blocks the connection until it atomically moves an element from the <paramref name="source"/> list to the <paramref name="destination"/> list.
    /// <see cref="ListBlockingMoveAsync"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListMoveAsync"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmove"/>
    /// <note>When in cluster mode, <paramref name="source"/> and <paramref name="destination"/> must map to the same hash slot.</note>
    /// <note>Since Valkey 6.2.0.</note>
    /// <param name="source">The key of the source list.</param>
    /// <param name="destination">The key of the destination list.</param>
    /// <param name="sourceSide">The side of the source list to pop from (Left = head, Right = tail).</param>
    /// <param name="destinationSide">The side of the destination list to push to (Left = head, Right = tail).</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// The element being popped and pushed.
    /// If <paramref name="source"/> does not exist or if the operation timed-out, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ListBlockingMoveAsync("sourceList", "destList", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout);

    /// <summary>
    /// Blocks the connection until it pops one element from the first non-empty list from the provided <paramref name="keys"/>.
    /// <see cref="ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan)"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="keys">A collection of keys to lists.</param>
    /// <param name="side">The side of the list to pop from (Left = head, Right = tail).</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no element could be popped and the timeout expired, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ListPopResult result = await client.ListBlockingPopAsync(["list1", "list2"], ListSide.Left, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout);

    /// <summary>
    /// Blocks the connection until it pops one element from the list stored at <paramref name="key"/>.
    /// <see cref="ListBlockingPopAsync(ValkeyKey, ListSide, TimeSpan)"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(ValkeyKey)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop"/>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="key">The key of the list to pop from.</param>
    /// <param name="side">The side of the list to pop from (Left = head, Right = tail).</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no element could be popped and the timeout expired, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ListPopResult result = await client.ListBlockingPopAsync("mylist", ListSide.Left, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(ValkeyKey key, ListSide side, TimeSpan timeout);

    /// <summary>
    /// Blocks the connection until it pops up to <paramref name="count"/> elements from the first non-empty list from the provided <paramref name="keys"/>.
    /// <see cref="ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan)"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="keys">A collection of keys to lists.</param>
    /// <param name="side">The side of the list to pop from (Left = head, Right = tail).</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no element could be popped and the timeout expired, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ListPopResult result = await client.ListBlockingPopAsync(["list1", "list2"], ListSide.Left, 3, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout);

    /// <summary>
    /// Blocks the connection until it pops up to <paramref name="count"/> elements from the list stored at <paramref name="key"/>.
    /// <see cref="ListBlockingPopAsync(ValkeyKey, ListSide, long, TimeSpan)"/> is the blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(ValkeyKey, long)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop"/>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="key">The key of the list to pop from.</param>
    /// <param name="side">The side of the list to pop from (Left = head, Right = tail).</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no element could be popped and the timeout expired, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ListPopResult result = await client.ListBlockingPopAsync("mylist", ListSide.Left, 3, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(ValkeyKey key, ListSide side, long count, TimeSpan timeout);

    // ===== LPUSHX / RPUSHX - Explicit Methods =====

    /// <summary>
    /// Inserts the specified value at the head of the list stored at <paramref name="key"/>, only if <paramref name="key"/> already exists and holds a list.
    /// Unlike <see cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)"/>, no operation will be performed when <paramref name="key"/> does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpushx"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the head of the list.</param>
    /// <returns>The length of the list after the push operation. Returns 0 if the key does not exist.</returns>
    /// <remarks>
    /// This is the GLIDE-style explicit method for LPUSHX. For SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)"/> with <c>When.Exists</c>.
    /// <example>
    /// <code>
    /// long result = await client.ListLeftPushIfExistsAsync(key, value);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushIfExistsAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Inserts all the specified values at the head of the list stored at <paramref name="key"/>, only if <paramref name="key"/> already exists and holds a list.
    /// Unlike <see cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>, no operation will be performed when <paramref name="key"/> does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpushx"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The values to add to the head of the list.</param>
    /// <returns>The length of the list after the push operation. Returns 0 if the key does not exist.</returns>
    /// <remarks>
    /// This is the GLIDE-style explicit method for LPUSHX. For SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/> with <c>When.Exists</c>.
    /// <example>
    /// <code>
    /// long result = await client.ListLeftPushIfExistsAsync(key, values);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushIfExistsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Inserts the specified value at the tail of the list stored at <paramref name="key"/>, only if <paramref name="key"/> already exists and holds a list.
    /// Unlike <see cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, When)"/>, no operation will be performed when <paramref name="key"/> does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpushx"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the tail of the list.</param>
    /// <returns>The length of the list after the push operation. Returns 0 if the key does not exist.</returns>
    /// <remarks>
    /// This is the GLIDE-style explicit method for RPUSHX. For SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, When)"/> with <c>When.Exists</c>.
    /// <example>
    /// <code>
    /// long result = await client.ListRightPushIfExistsAsync(key, value);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushIfExistsAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Inserts all the specified values at the tail of the list stored at <paramref name="key"/>, only if <paramref name="key"/> already exists and holds a list.
    /// Unlike <see cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>, no operation will be performed when <paramref name="key"/> does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpushx"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The values to add to the tail of the list.</param>
    /// <returns>The length of the list after the push operation. Returns 0 if the key does not exist.</returns>
    /// <remarks>
    /// This is the GLIDE-style explicit method for RPUSHX. For SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/> with <c>When.Exists</c>.
    /// <example>
    /// <code>
    /// long result = await client.ListRightPushIfExistsAsync(key, values);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushIfExistsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    // ===== LINDEX / LSET - GLIDE-style naming =====

    /// <summary>
    /// Returns the element at <paramref name="index"/> in the list stored at <paramref name="key"/>.
    /// The index is zero-based, so <c>0</c> means the first element, <c>1</c> the second element and so on.
    /// Negative indices can be used to designate elements starting at the tail of the list.
    /// Here, <c>-1</c> means the last element, <c>-2</c> means the penultimate and so forth.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lindex"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="index">The index of the element in the list to retrieve.</param>
    /// <returns>
    /// The element at <paramref name="index"/>.
    /// If <paramref name="index"/> is out of range or if <paramref name="key"/> does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// This is the GLIDE-style method for LINDEX. For SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListGetByIndexAsync(ValkeyKey, long, CommandFlags)"/>.
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ListIndexAsync(key, 0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListIndexAsync(ValkeyKey key, long index);

    /// <summary>
    /// Sets the list element at <paramref name="index"/> to <paramref name="value"/>.
    /// The index is zero-based, so <c>0</c> means the first element, <c>1</c> the second element and so on.
    /// Negative indices can be used to designate elements starting at the tail of the list.
    /// Here, <c>-1</c> means the last element, <c>-2</c> means the penultimate and so forth.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lset"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="index">The index of the element in the list to set.</param>
    /// <param name="value">The new value.</param>
    /// <remarks>
    /// An error is returned for out of range indexes.
    /// This is the GLIDE-style method for LSET. For SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>.
    /// <example>
    /// <code>
    /// await client.ListSetAsync(key, 0, "new_value");
    /// </code>
    /// </example>
    /// </remarks>
    Task ListSetAsync(ValkeyKey key, long index, ValkeyValue value);
}
