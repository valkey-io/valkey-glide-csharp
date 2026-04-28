// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Lists commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#list">Valkey – List Commands</seealso>
public interface IListBaseCommands
{
    /// <summary>
    /// Removes and returns the first element of a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpop/">Valkey commands – LPOP</seealso>
    /// <param name="key">The list key.</param>
    /// <returns>The value of the first element, or <see cref="ValkeyValue.Null"/> if the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var value = await client.ListLeftPopAsync("mylist");  // "a"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns up to <paramref name="count" /> elements from the head of a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpop/">Valkey commands – LPOP</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="count">The count of the elements to pop from the list.</param>
    /// <returns>An array of the popped elements, or <see langword="null"/> if the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var values = await client.ListLeftPopAsync("mylist", 2);  // ["a", "b"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> elements from the first non-empty list
    /// among the provided keys.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmpop/">Valkey commands – LMPOP</seealso>
    /// <param name="keys">A collection of list keys.</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no list contains elements, <see cref="ListPopResult.Null"/> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("list1", ["a", "b"]);
    /// var popResult = await client.ListLeftPopAsync(["list1", "list2"], 2);
    /// // popResult.Key == "list1", popResult.Values == ["a", "b"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count);

    /// <summary>
    /// Inserts a value at the head of a list. Creates the list if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush/">Valkey commands – LPUSH</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="value">The value to add to the head of the list.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var length = await client.ListLeftPushAsync("mylist", "value");  // 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Inserts all the specified values at the head of a list. Creates the list if it does not exist.
    /// Elements are inserted one after the other from left to right.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush/">Valkey commands – LPUSH</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="values">The elements to insert at the head of the list.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var length = await client.ListLeftPushAsync("mylist", ["a", "b", "c"]);  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Inserts a value at the tail of a list. Creates the list if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush/">Valkey commands – RPUSH</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="value">The value to add to the tail of the list.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var length = await client.ListRightPushAsync("mylist", "value");  // 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Inserts all the specified values at the tail of a list. Creates the list if it does not exist.
    /// Elements are inserted one after the other from left to right.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush/">Valkey commands – RPUSH</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="values">The elements to insert at the tail of the list.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var length = await client.ListRightPushAsync("mylist", ["a", "b", "c"]);  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Removes and returns the last element of a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpop/">Valkey commands – RPOP</seealso>
    /// <param name="key">The list key.</param>
    /// <returns>The value of the last element, or <see cref="ValkeyValue.Null"/> if the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var value = await client.ListRightPopAsync("mylist");  // "c"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListRightPopAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns up to <paramref name="count" /> elements from the tail of a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpop/">Valkey commands – RPOP</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="count">The count of the elements to pop from the list.</param>
    /// <returns>An array of the popped elements, or <see langword="null"/> if the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var values = await client.ListRightPopAsync("mylist", 2);  // ["c", "b"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> elements from the last non-empty list
    /// among the provided keys.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmpop/">Valkey commands – LMPOP</seealso>
    /// <param name="keys">A collection of list keys.</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no list contains elements, <see cref="ListPopResult.Null"/> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("list1", ["a", "b"]);
    /// var popResult = await client.ListRightPopAsync(["list1", "list2"], 2);
    /// // popResult.Key == "list1", popResult.Values == ["b", "a"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count);

    /// <summary>
    /// Returns the length of a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/llen/">Valkey commands – LLEN</seealso>
    /// <param name="key">The list key.</param>
    /// <returns>
    /// The length of the list.
    /// If the key does not exist, it is interpreted as an empty list and <c>0</c> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var length = await client.ListLengthAsync("mylist");  // 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLengthAsync(ValkeyKey key);

    /// <summary>
    /// Removes the first <paramref name="count" /> occurrences of elements equal to <paramref name="value" /> from a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lrem/">Valkey commands – LREM</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="value">The value to remove from the list.</param>
    /// <param name="count">The count of the occurrences of elements equal to <paramref name="value" /> to remove.
    /// <list type="bullet">
    ///     <item><paramref name="count" /> &gt; <c>0</c>: Remove elements moving from head to tail.</item>
    ///     <item><paramref name="count" /> &lt; <c>0</c>: Remove elements moving from tail to head.</item>
    ///     <item><paramref name="count" /> = <c>0</c>: Remove all elements equal to <paramref name="value" />.</item>
    /// </list>
    /// </param>
    /// <returns>
    /// The number of removed elements.
    /// If the key does not exist, <c>0</c> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "a", "c", "a"]);
    /// var removedCount = await client.ListRemoveAsync("mylist", "a", 2);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0);

    /// <summary>
    /// Trims a list to the specified range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ltrim/">Valkey commands – LTRIM</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="start">The starting point of the range (zero-based, inclusive).</param>
    /// <param name="stop">The end of the range (zero-based, inclusive). Negative values indicate offsets from the end.</param>
    /// <remarks>
    /// If <paramref name="start" /> exceeds the end of the list, or if <paramref name="start" /> is greater than <paramref name="stop" />, the list is emptied.
    /// If <paramref name="stop" /> exceeds the actual end of the list, it is treated as the last element.
    /// If the key does not exist, the command returns without changes.
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c", "d"]);
    /// await client.ListTrimAsync("mylist", 0, 1);
    /// var remaining = await client.ListRangeAsync("mylist");  // ["a", "b"]
    /// </code>
    /// </example>
    /// </remarks>
    Task ListTrimAsync(ValkeyKey key, long start, long stop);

    /// <summary>
    /// Returns the specified range of elements from a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lrange/">Valkey commands – LRANGE</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="start">The starting point of the range (zero-based, inclusive).</param>
    /// <param name="stop">The end of the range (zero-based, inclusive). Negative values indicate offsets from the end.</param>
    /// <returns>
    /// An array of elements in the specified range.
    /// If the key does not exist, an empty array is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var elements = await client.ListRangeAsync("mylist", 0, -1);  // ["a", "b", "c"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start = 0, long stop = -1);

    /// <summary>
    /// Inserts a value after the reference value <paramref name="pivot"/> in a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/linsert/">Valkey commands – LINSERT</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="pivot">The reference point in the list.</param>
    /// <param name="value">The new element to insert.</param>
    /// <returns>
    /// The length of the list after the insert operation.
    /// If the <paramref name="pivot"/> is not found, <c>-1</c> is returned.
    /// If the key does not exist, <c>0</c> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var length = await client.ListInsertAfterAsync("mylist", "b", "x");  // 4
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value);

    /// <summary>
    /// Inserts a value before the reference value <paramref name="pivot"/> in a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/linsert/">Valkey commands – LINSERT</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="pivot">The reference point in the list.</param>
    /// <param name="value">The new element to insert.</param>
    /// <returns>
    /// The length of the list after the insert operation.
    /// If the <paramref name="pivot"/> is not found, <c>-1</c> is returned.
    /// If the key does not exist, <c>0</c> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c"]);
    /// var length = await client.ListInsertBeforeAsync("mylist", "b", "x");  // 4
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value);

    /// <summary>
    /// Atomically pops an element from a source list and pushes it to a destination list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmove/">Valkey commands – LMOVE</seealso>
    /// <param name="sourceKey">The source list key.</param>
    /// <param name="destinationKey">The destination list key.</param>
    /// <param name="sourceSide">The side of the source list to pop from (Left = head, Right = tail).</param>
    /// <param name="destinationSide">The side of the destination list to push to (Left = head, Right = tail).</param>
    /// <returns>
    /// The element being popped and pushed, or <see cref="ValkeyValue.Null"/> if the source list does not exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("source", ["a", "b", "c"]);
    /// var moved = await client.ListMoveAsync("source", "dest", ListSide.Left, ListSide.Right);  // "a"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide);

    /// <summary>
    /// Returns the index of the first occurrence of an element in a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpos/">Valkey commands – LPOS</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="element">The element to search for.</param>
    /// <param name="rank">The rank of the match to return (<c>1</c>-based). Negative values indicate searching from the end.</param>
    /// <param name="maxLength">Limit the search to this many elements. <c>0</c> means no limit.</param>
    /// <returns>
    /// The index of the first occurrence of <paramref name="element"/>, or <c>-1</c> if not found.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c", "b"]);
    /// var index = await client.ListPositionAsync("mylist", "b");  // 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0);

    /// <summary>
    /// Returns the indices of matching elements in a list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpos/">Valkey commands – LPOS</seealso>
    /// <param name="key">The list key.</param>
    /// <param name="element">The element to search for.</param>
    /// <param name="count">The maximum number of matches to return.</param>
    /// <param name="rank">The rank of the first match to return (<c>1</c>-based). Negative values indicate searching from the end.</param>
    /// <param name="maxLength">Limit the search to this many elements. <c>0</c> means no limit.</param>
    /// <returns>
    /// An array of indices of matching elements, or an empty array if no matches are found.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.ListRightPushAsync("mylist", ["a", "b", "c", "b"]);
    /// var indices = await client.ListPositionsAsync("mylist", "b", 10);  // [1, 3]
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0);

}
