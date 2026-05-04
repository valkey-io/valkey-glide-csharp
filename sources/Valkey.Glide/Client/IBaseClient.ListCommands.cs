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
    /// Pops an element from the head of the first non-empty list among the given <paramref name="keys"/>,
    /// blocking the connection when there are no elements to pop.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blpop/">Valkey commands – BLPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The list keys, checked in order.</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// A two-element array <c>[key, value]</c> containing the source key and the popped element,
    /// or <see langword="null"/> if the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(ValkeyKey)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("list1", ["a", "b"]);
    /// var popped = await client.ListBlockingLeftPopAsync(["list1", "list2"], TimeSpan.FromSeconds(5));  // ["list1", "a"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout);

    /// <summary>
    /// Pops an element from the head of a list,
    /// blocking the connection when there are no elements to pop.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blpop/">Valkey commands – BLPOP</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// A two-element array <c>[key, value]</c> containing the source key and the popped element,
    /// or <see langword="null"/> if the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(ValkeyKey)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b"]);
    /// var popped = await client.ListBlockingLeftPopAsync("mylist", TimeSpan.FromSeconds(5));  // ["mylist", "a"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingLeftPopAsync(ValkeyKey key, TimeSpan timeout);

    /// <summary>
    /// Pops an element from the tail of the first non-empty list among the given <paramref name="keys"/>,
    /// blocking the connection when there are no elements to pop.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/brpop/">Valkey commands – BRPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The list keys, checked in order.</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// A two-element array <c>[key, value]</c> containing the source key and the popped element,
    /// or <see langword="null"/> if the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListRightPopAsync(ValkeyKey)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("list1", ["a", "b"]);
    /// var popped = await client.ListBlockingRightPopAsync(["list1", "list2"], TimeSpan.FromSeconds(5));  // ["list1", "b"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout);

    /// <summary>
    /// Pops an element from the tail of a list,
    /// blocking the connection when there are no elements to pop.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/brpop/">Valkey commands – BRPOP</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// A two-element array <c>[key, value]</c> containing the source key and the popped element,
    /// or <see langword="null"/> if the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListRightPopAsync(ValkeyKey)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b"]);
    /// var popped = await client.ListBlockingRightPopAsync("mylist", TimeSpan.FromSeconds(5));  // popped == ["mylist", "b"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingRightPopAsync(ValkeyKey key, TimeSpan timeout);

    /// <summary>
    /// Atomically moves an element from the <paramref name="source"/> list to the
    /// <paramref name="destination"/> list, blocking until an element is available.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmove/">Valkey commands – BLMOVE</seealso>
    /// <note>When in cluster mode, <paramref name="source"/> and <paramref name="destination"/> must map to the same hash slot.</note>
    /// <param name="source">The source list key.</param>
    /// <param name="destination">The destination list key.</param>
    /// <param name="sourceSide">The side to pop from (<see cref="ListSide.Left"/> = head, <see cref="ListSide.Right"/> = tail).</param>
    /// <param name="destinationSide">The side to push to (<see cref="ListSide.Left"/> = head, <see cref="ListSide.Right"/> = tail).</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// The moved element, or <see cref="ValkeyValue.Null"/> if <paramref name="source"/> does not exist
    /// or the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListMoveAsync"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("src", ["a", "b", "c"]);
    /// var moved = await client.ListBlockingMoveAsync("src", "dst", ListSide.Left, ListSide.Right, TimeSpan.FromSeconds(5));
    /// // moved == "a"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout);

    /// <summary>
    /// Pops one element from the first non-empty list among the given <paramref name="keys"/>,
    /// blocking until an element is available.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop/">Valkey commands – BLMPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="keys">The list keys, checked in order.</param>
    /// <param name="side">The side to pop from (<see cref="ListSide.Left"/> = head, <see cref="ListSide.Right"/> = tail).</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> with the source key and popped elements,
    /// or <see cref="ListPopResult.Null"/> if the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("list1", ["a", "b"]);
    /// var popResult = await client.ListBlockingPopAsync(["list1", "list2"], ListSide.Left, TimeSpan.FromSeconds(5));
    /// // popResult.Key == "list1", popResult.Values[0] == "a"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout);

    /// <summary>
    /// Pops one element from a list,
    /// blocking until an element is available.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop/">Valkey commands – BLMPOP</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="key">The list key.</param>
    /// <param name="side">The side to pop from (<see cref="ListSide.Left"/> = head, <see cref="ListSide.Right"/> = tail).</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> with the source key and popped elements,
    /// or <see cref="ListPopResult.Null"/> if the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(ValkeyKey)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b"]);
    /// var popResult = await client.ListBlockingPopAsync("mylist", ListSide.Left, TimeSpan.FromSeconds(5));
    /// // popResult.Key == "mylist", popResult.Values[0] == "a"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(ValkeyKey key, ListSide side, TimeSpan timeout);

    /// <summary>
    /// Pops up to <paramref name="count"/> elements from the first non-empty list among the given
    /// <paramref name="keys"/>, blocking until an element is available.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop/">Valkey commands – BLMPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="keys">The list keys, checked in order.</param>
    /// <param name="side">The side to pop from (<see cref="ListSide.Left"/> = head, <see cref="ListSide.Right"/> = tail).</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> with the source key and popped elements,
    /// or <see cref="ListPopResult.Null"/> if the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("list1", ["a", "b", "c"]);
    /// var popResult = await client.ListBlockingPopAsync(["list1", "list2"], ListSide.Left, 2, TimeSpan.FromSeconds(5));
    /// // popResult.Key == "list1", popResult.Values[0] == "a"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout);

    /// <summary>
    /// Pops up to <paramref name="count"/> elements from a list,
    /// blocking until an element is available.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop/">Valkey commands – BLMPOP</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="key">The list key.</param>
    /// <param name="side">The side to pop from (<see cref="ListSide.Left"/> = head, <see cref="ListSide.Right"/> = tail).</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="timeout">The maximum time to wait. <see cref="TimeSpan.Zero"/> blocks indefinitely.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> with the source key and popped elements,
    /// or <see cref="ListPopResult.Null"/> if the <paramref name="timeout"/> expired.
    /// </returns>
    /// <remarks>
    /// Blocking variant of <see cref="Commands.IListBaseCommands.ListLeftPopAsync(ValkeyKey, long)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var popResult = await client.ListBlockingPopAsync("mylist", ListSide.Left, 2, TimeSpan.FromSeconds(5));
    /// // popResult.Key == "mylist", popResult.Values[0] == "a"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(ValkeyKey key, ListSide side, long count, TimeSpan timeout);

    // ===== LPUSHX / RPUSHX - Explicit Methods =====

    /// <summary>
    /// Inserts a value at the head of a list, only if
    /// the key already exists and holds a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpushx/">Valkey commands – LPUSHX</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="value">The value to prepend.</param>
    /// <returns>The length of the list after the push, or <c>0</c> if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// For the SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)"/> with <c>When.Exists</c>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("key", ["x"]);
    /// var length = await client.ListLeftPushIfExistsAsync("key", "value");
    /// // length == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushIfExistsAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Inserts all specified values at the head of a list, only if
    /// the key already exists and holds a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpushx/">Valkey commands – LPUSHX</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="values">The values to prepend.</param>
    /// <returns>The length of the list after the push, or <c>0</c> if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// For the SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/> with <c>When.Exists</c>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("key", ["x"]);
    /// var length = await client.ListLeftPushIfExistsAsync("key", ["a", "b", "c"]);
    /// // length == 4
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushIfExistsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Inserts a value at the tail of a list, only if
    /// the key already exists and holds a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpushx/">Valkey commands – RPUSHX</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="value">The value to append.</param>
    /// <returns>The length of the list after the push, or <c>0</c> if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// For the SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, When)"/> with <c>When.Exists</c>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("key", ["x"]);
    /// var length = await client.ListRightPushIfExistsAsync("key", "value");
    /// // length == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushIfExistsAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Inserts all specified values at the tail of a list, only if
    /// the key already exists and holds a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpushx/">Valkey commands – RPUSHX</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="values">The values to append.</param>
    /// <returns>The length of the list after the push, or <c>0</c> if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// For the SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/> with <c>When.Exists</c>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("key", ["x"]);
    /// var length = await client.ListRightPushIfExistsAsync("key", ["a", "b", "c"]);
    /// // length == 4
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushIfExistsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    // ===== LINDEX / LSET - GLIDE-style naming =====

    /// <summary>
    /// Returns the element at a given index in a list.
    /// The index is zero-based; negative indices count from the tail (<c>-1</c> is the last element).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lindex/">Valkey commands – LINDEX</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <returns>
    /// The element at <paramref name="index"/>, or <see cref="ValkeyValue.Null"/> if
    /// <paramref name="index"/> is out of range or <paramref name="key"/> does not exist.
    /// </returns>
    /// <remarks>
    /// For the SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListGetByIndexAsync(ValkeyKey, long, CommandFlags)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("key", ["a", "b", "c"]);
    /// var element = await client.ListIndexAsync("key", 0);
    /// // element == "a"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListIndexAsync(ValkeyKey key, long index);

    /// <summary>
    /// Sets the list element at <paramref name="index"/> to <paramref name="value"/>.
    /// The index is zero-based; negative indices count from the tail (<c>-1</c> is the last element).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lset/">Valkey commands – LSET</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="index">The zero-based index of the element to set.</param>
    /// <param name="value">The new value.</param>
    /// <remarks>
    /// An error is returned for out-of-range indexes. For the SER-compatible API, use
    /// <see cref="IDatabaseAsync.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("key", ["a", "b", "c"]);
    /// await client.ListSetAsync("key", 0, "new_value");
    /// var element = await client.ListIndexAsync("key", 0);
    /// // element == "new_value"
    /// </code>
    /// </example>
    /// </remarks>
    Task ListSetAsync(ValkeyKey key, long index, ValkeyValue value);
}
