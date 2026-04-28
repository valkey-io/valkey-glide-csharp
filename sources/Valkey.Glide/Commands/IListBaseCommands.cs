// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// List commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#list">Valkey – List Commands</seealso>
public interface IListBaseCommands
{
    /// <summary>
    /// Removes and returns the first elements of the list stored at <paramref name="key" />. The command pops a single element from the beginning
    /// of the list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpop"/>
    /// <param name="key">The key of the list.</param>
    /// <returns>The value of the first element.<br/>
    /// If <paramref name="key" /> does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListLeftPopAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns up to <paramref name="count" /> elements of the list stored at <paramref name="key" />, depending on the list's length.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpop"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="count">The count of the elements to pop from the list.</param>
    /// <returns>An array of the popped elements will be returned depending on the list's length.<br/>
    /// If <paramref name="key" /> does not exist, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListLeftPopAsync("list", 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> elements from the first non-empty list
    /// from the provided <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmpop"/>
    /// <param name="keys">A collection of keys to lists.</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no list contains elements, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListLeftPopAsync(["list1", "list2"], 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count);

    /// <summary>
    /// Inserts the specified value at the head of the list stored at <paramref name="key" />.
    /// If <paramref name="key" /> does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the head of the list.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListLeftPushAsync("list", "value");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Inserts all the specified values at the head of the list stored at <paramref name="key" />. Elements are inserted one
    /// after the other to the head of the list, from the leftmost element to the rightmost element. If <paramref name="key" /> does not exist, it
    /// is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the head of the list stored at <paramref name="key" />.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListLeftPushAsync("list", ["value1", "value2"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Inserts the specified value at the tail of the list stored at <paramref name="key" />.
    /// If <paramref name="key" /> does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the tail of the list.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListRightPushAsync("list", "value");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Inserts all the specified values at the tail of the list stored at <paramref name="key" />.
    /// Elements are inserted one after the other to the tail of the list, from the leftmost element to the rightmost element.
    /// If <paramref name="key" /> does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the tail of the list stored at <paramref name="key" />.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListRightPushAsync("list", ["value1", "value2"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <summary>
    /// Removes and returns the last elements of the list stored at <paramref name="key" />.
    /// The command pops a single element from the end of the list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpop"/>
    /// <param name="key">The key of the list.</param>
    /// <returns>The value of the last element.<br/>
    /// If <paramref name="key" /> does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListRightPopAsync("list");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListRightPopAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns up to <paramref name="count" /> elements from the list stored at <paramref name="key" />, depending on the list's length.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpop"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="count">The count of the elements to pop from the list.</param>
    /// <returns>An array of the popped elements will be returned depending on the list's length.<br/>
    /// If <paramref name="key" /> does not exist, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListRightPopAsync("list", 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> elements from the first non-empty list
    /// from the provided <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmpop"/>
    /// <param name="keys">A collection of keys to lists.</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no list contains elements, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListRightPopAsync(["list1", "list2"], 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count);

    /// <summary>
    /// Returns the length of the list stored at <paramref name="key" />.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/llen"/>
    /// <param name="key">The key of the list.</param>
    /// <returns>
    /// The length of the list at <paramref name="key" />.
    /// If <paramref name="key" /> does not exist, it is interpreted as an empty list and <c>0</c> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListLengthAsync("list");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLengthAsync(ValkeyKey key);

    /// <summary>
    /// Removes the first <paramref name="count" /> occurrences of elements equal to <paramref name="value" /> from the list stored at <paramref name="key" />.
    /// The <paramref name="count" /> argument influences the operation in the following ways:
    /// <list type="bullet">
    ///     <item><paramref name="count" /> &gt; <c>0</c>: Remove elements equal to <paramref name="value" /> moving from head to tail.</item>
    ///     <item><paramref name="count" /> &lt; <c>0</c>: Remove elements equal to <paramref name="value" /> moving from tail to head.</item>
    ///     <item><paramref name="count" /> = <c>0</c>: Remove all elements equal to <paramref name="value" />.</item>
    /// </list>
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lrem"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to remove from the list.</param>
    /// <param name="count">The count of the occurrences of elements equal to <paramref name="value" /> to remove.</param>
    /// <returns>
    ///	The number of the removed elements.
    ///	If <paramref name="key" /> does not exist, <c>0</c> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.ListRemoveAsync("list", "value", 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0);

    /// <summary>
    /// Trims an existing list so that it will contain only the specified range of elements specified.
    /// The offsets <paramref name="start" /> and <paramref name="stop" /> are zero-based indexes, with <c>0</c> being the first element of the list, <c>1</c> being the next element
    /// and so on. These offsets can also be negative numbers indicating offsets starting at the end of the list, with <c>-1</c> being
    /// the last element of the list, <c>-2</c> being the penultimate, and so on.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ltrim"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="start">The starting point of the range.</param>
    /// <param name="stop">The end of the range.</param>
    /// <remarks>
    ///	If <paramref name="start" /> exceeds the end of the list, or if <paramref name="start" /> is greater than <paramref name="stop" />, the list is emptied
    ///	and the key is removed.
    ///	If <paramref name="stop" /> exceeds the actual end of the list, it will be treated like the last element of the list.
    ///	If <paramref name="key" /> does not exist, the command will return without changes to the database.
    /// <example>
    /// <code>
    /// await client.ListTrimAsync("list", 0, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task ListTrimAsync(ValkeyKey key, long start, long stop);

    /// <summary>
    /// Returns the specified elements of the list stored at <paramref name="key" />.
    /// The offsets <paramref name="start" /> and <paramref name="stop" /> are zero-based indexes, with <c>0</c> being the first element of the list (the head of the list), <c>1</c> being the next element and so on.
    /// These offsets can also be negative numbers indicating offsets starting at the end of the list, with <c>-1</c> being the last element of the list, <c>-2</c> being the penultimate, and so on.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lrange"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="start">The starting point of the range.</param>
    /// <param name="stop">The end of the range.</param>
    /// <returns>
    ///	Array of <see cref="ValkeyValue"/>s in the specified range.
    ///	If <paramref name="start" /> exceeds the end of the list, or if <paramref name="start" /> is greater than <paramref name="stop" />, an empty array will be returned.
    ///	If <paramref name="stop" /> exceeds the actual end of the list, the range will stop at the actual end of the list.
    ///	If <paramref name="key" /> does not exist an empty array will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.ListRangeAsync("list", 0, -1);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start = 0, long stop = -1);

    /// <summary>
    /// Inserts <paramref name="value"/> in the list stored at <paramref name="key"/> after the reference value <paramref name="pivot"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/linsert"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="pivot">The reference point in the list.</param>
    /// <param name="value">The new element to insert.</param>
    /// <returns>
    /// The length of the list after the insert operation.
    /// If the <paramref name="pivot"/> is not found, <c>-1</c> is returned.
    /// If <paramref name="key"/> does not exist, <c>0</c> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListInsertAfterAsync("list", "pivot", "new_element");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value);

    /// <summary>
    /// Inserts <paramref name="value"/> in the list stored at <paramref name="key"/> before the reference value <paramref name="pivot"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/linsert"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="pivot">The reference point in the list.</param>
    /// <param name="value">The new element to insert.</param>
    /// <returns>
    /// The length of the list after the insert operation.
    /// If the <paramref name="pivot"/> is not found, <c>-1</c> is returned.
    /// If <paramref name="key"/> does not exist, <c>0</c> is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListInsertBeforeAsync("list", "pivot", "new_element");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value);

    /// <summary>
    /// Atomically returns and removes the first/last element of the list stored at <paramref name="sourceKey"/>
    /// and pushes the element at the first/last element of the list stored at <paramref name="destinationKey"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmove"/>
    /// <param name="sourceKey">The key of the source list.</param>
    /// <param name="destinationKey">The key of the destination list.</param>
    /// <param name="sourceSide">The side of the source list to pop from (Left = head, Right = tail).</param>
    /// <param name="destinationSide">The side of the destination list to push to (Left = head, Right = tail).</param>
    /// <returns>
    /// The element being popped and pushed.
    /// If <paramref name="sourceKey"/> does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ListMoveAsync("source_list", "dest_list", ListSide.Left, ListSide.Right);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide);

    /// <summary>
    /// Returns the index of the first occurrence of <paramref name="element"/> inside the list specified by <paramref name="key"/>.
    /// If no match is found, <c>-1</c> is returned.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpos"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="element">The element to search for.</param>
    /// <param name="rank">The rank of the match to return (<c>1</c>-based). Negative values indicate searching from the end.</param>
    /// <param name="maxLength">Limit the search to this many elements. <c>0</c> means no limit.</param>
    /// <returns>
    /// The index of the first occurrence of <paramref name="element"/>, or <c>-1</c> if not found.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListPositionAsync("list", "element", 1, 0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0);

    /// <summary>
    /// Returns the indices of matching elements inside the list specified by <paramref name="key"/>.
    /// If no matches are found, an empty array is returned.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpos"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="element">The element to search for.</param>
    /// <param name="count">The maximum number of matches to return.</param>
    /// <param name="rank">The rank of the first match to return (<c>1</c>-based). Negative values indicate searching from the end.</param>
    /// <param name="maxLength">Limit the search to this many elements. <c>0</c> means no limit.</param>
    /// <returns>
    /// An array of indices of matching elements.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long[] result = await client.ListPositionsAsync("list", "element", 10, 1, 0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0);

}
