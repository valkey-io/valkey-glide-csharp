// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "List Commands" group for standalone and cluster clients.
/// <br/>
/// See more on <see href="https://valkey.io/commands#list">valkey.io</see>.
/// </summary>
public interface IListCommands
{
    /// <summary>
    /// Removes and returns the first elements of the list stored at <paramref name="key" />. The command pops a single element from the beginning
    /// of the list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpop"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The value of the first element.<br/>
    /// If <paramref name="key" /> does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ListLeftPopAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes and returns up to <paramref name="count" /> elements of the list stored at <paramref name="key" />, depending on the list's length.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpop"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="count">The count of the elements to pop from the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>An array of the popped elements will be returned depending on the list's length.<br/>
    /// If <paramref name="key" /> does not exist, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.ListLeftPopAsync(key, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> elements from the first non-empty list
    /// from the provided <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmpop"/>
    /// <param name="keys">An array of keys to lists.</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no list contains elements, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ListPopResult result = await client.ListLeftPopAsync(keys, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListLeftPopAsync(ValkeyKey[] keys, long count, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Inserts the specified value at the head of the list stored at <paramref name="key" />.
    /// If <paramref name="key" /> does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush"/>
    /// <seealso href="https://valkey.io/commands/lpushx"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the head of the list.</param>
    /// <param name="when">Use <see cref="When.Exists"/> for LPUSHX behavior (only push if key exists).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Regular LPUSH
    /// long result = await client.ListLeftPushAsync(key, value);
    /// // LPUSHX (only push if key exists)
    /// long result = await client.ListLeftPushAsync(key, value, When.Exists);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Inserts all the specified values at the head of the list stored at <paramref name="key" />. Elements are inserted one
    /// after the other to the head of the list, from the leftmost element to the rightmost element. If <paramref name="key" /> does not exist, it
    /// is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush"/>
    /// <seealso href="https://valkey.io/commands/lpushx"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the head of the list stored at <paramref name="key" />.</param>
    /// <param name="when">Use <see cref="When.Exists"/> for LPUSHX behavior (only push if key exists).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Regular LPUSH
    /// long result = await client.ListLeftPushAsync(key, values, When.Always);
    /// // LPUSHX (only push if key exists)
    /// long result = await client.ListLeftPushAsync(key, values, When.Exists);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Inserts all the specified values at the head of the list stored at <paramref name="key" />. Elements are inserted one
    /// after the other to the head of the list, from the leftmost element to the rightmost element. If key does not exist, it
    /// is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpush"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the head of the list stored at <paramref name="key" />.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListLeftPushAsync(key, values, CommandFlags.None);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue[] values, CommandFlags flags);

    /// <summary>
    /// Inserts the specified value at the tail of the list stored at <paramref name="key" />.
    /// If <paramref name="key" /> does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush"/>
    /// <seealso href="https://valkey.io/commands/rpushx"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to add to the tail of the list.</param>
    /// <param name="when">Use <see cref="When.Exists"/> for RPUSHX behavior (only push if key exists).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Regular RPUSH
    /// long result = await client.ListRightPushAsync(key, value);
    /// // RPUSHX (only push if key exists)
    /// long result = await client.ListRightPushAsync(key, value, When.Exists);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Inserts all the specified values at the tail of the list stored at <paramref name="key" />.
    /// Elements are inserted one after the other to the tail of the list, from the leftmost element to the rightmost element.
    /// If <paramref name="key" /> does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush"/>
    /// <seealso href="https://valkey.io/commands/rpushx"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the tail of the list stored at <paramref name="key" />.</param>
    /// <param name="when">Use <see cref="When.Exists"/> for RPUSHX behavior (only push if key exists).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Regular RPUSH
    /// long result = await client.ListRightPushAsync(key, values, When.Always);
    /// // RPUSHX (only push if key exists)
    /// long result = await client.ListRightPushAsync(key, values, When.Exists);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Inserts all the specified values at the tail of the list stored at key.
    /// elements are inserted one after the other to the tail of the list, from the leftmost element to the rightmost element.
    /// If key does not exist, it is created as an empty list before performing the push operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpush"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="values">The elements to insert at the tail of the list stored at <paramref name="key" />.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The length of the list after the push operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListRightPushAsync(key, values, CommandFlags.None);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue[] values, CommandFlags flags);

    /// <summary>
    /// Removes and returns the last elements of the list stored at <paramref name="key" />.
    /// The command pops a single element from the end of the list.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpop"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>The value of the last element.<br/>
    /// If <paramref name="key" /> does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ListRightPopAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes and returns up to <paramref name="count" /> elements from the list stored at <paramref name="key" />, depending on the list's length.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/rpop"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="count">The count of the elements to pop from the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>An array of the popped elements will be returned depending on the list's length.<br/>
    /// If <paramref name="key" /> does not exist, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.ListRightPopAsync(key, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> elements from the first non-empty list
    /// from the provided <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmpop"/>
    /// <param name="keys">An array of keys to lists.</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no list contains elements, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ListPopResult result = await client.ListRightPopAsync(keys, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListRightPopAsync(ValkeyKey[] keys, long count, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the length of the list stored at <paramref name="key" />.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/llen"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The length of the list at <paramref name="key" />.
    /// If <paramref name="key" /> does not exist, it is interpreted as an empty list and 0 is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListLengthAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the first <paramref name="count" /> occurrences of elements equal to <paramref name="value" /> from the list stored at <paramref name="key" />.
    /// The <paramref name="count" /> argument influences the operation in the following ways:
    /// <list type="bullet">
    ///     <item><paramref name="count" /> &gt; 0: Remove elements equal to <paramref name="value" /> moving from head to tail.</item>
    ///     <item><paramref name="count" /> &lt; 0: Remove elements equal to <paramref name="value" /> moving from tail to head.</item>
    ///     <item><paramref name="count" /> = 0: Remove all elements equal to <paramref name="value" />.</item>
    /// </list>
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lrem"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="value">The value to remove from the list.</param>
    /// <param name="count">The count of the occurrences of elements equal to <paramref name="value" /> to remove.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    ///	The number of the removed elements.
    ///	If <paramref name="key" /> does not exist, 0 is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListRemoveAsync(key, value, count);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Trims an existing list so that it will contain only the specified range of elements specified.
    /// The offsets <paramref name="start" /> and <paramref name="stop" /> are zero-based indexes, with 0 being the first element of the list, 1 being the next element
    /// and so on. These offsets can also be negative numbers indicating offsets starting at the end of the list, with -1 being
    /// the last element of the list, -2 being the penultimate, and so on.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/ltrim"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="start">The starting point of the range.</param>
    /// <param name="stop">The end of the range.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    ///	If <paramref name="start" /> exceeds the end of the list, or if <paramref name="start" /> is greater than <paramref name="stop" />, the list is emptied
    ///	and the key is removed.
    ///	If <paramref name="stop" /> exceeds the actual end of the list, it will be treated like the last element of the list.
    ///	If <paramref name="key" /> does not exist, the command will return without changes to the database.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// _ = await client.ListTrimAsync(key, start, stop);
    /// </code>
    /// </example>
    /// </remarks>
    Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified elements of the list stored at <paramref name="key" />.
    /// The offsets <paramref name="start" /> and <paramref name="stop" /> are zero-based indexes, with 0 being the first element of the list (the head of the list), 1 being the next element and so on.
    /// These offsets can also be negative numbers indicating offsets starting at the end of the list, with -1 being the last element of the list, -2 being the penultimate, and so on.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lrange"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="start">The starting point of the range.</param>
    /// <param name="stop">The end of the range.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    ///	Array of <see cref="ValkeyValue"/>s in the specified range.
    ///	If <paramref name="start" /> exceeds the end of the list, or if <paramref name="start" /> is greater than <paramref name="stop" />, an empty array will be returned.
    ///	If <paramref name="stop" /> exceeds the actual end of the list, the range will stop at the actual end of the list.
    ///	If <paramref name="key" /> does not exist an empty array will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.ListRangeAsync(key, start, stop);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None);

    // ===== BLOCKING OPERATIONS - NOT SUPPORTED BY STACKEXCHANGE.REDIS =====
    // The following blocking operations are NOT supported by StackExchange.Redis because they would
    // block the entire multiplexer, preventing other operations from executing.
    // These are GLIDE-only features:
    // - BLMOVE (blocking list move)
    // - BLMPOP (blocking list multi-pop)  
    // - BLPOP (blocking left pop)
    // - BRPOP (blocking right pop)
    // See: https://stackexchange.github.io/StackExchange.Redis/Basics.html

    /// <summary>
    /// Returns the element at index <paramref name="index"/> in the list stored at <paramref name="key"/>.
    /// The index is zero-based, so 0 means the first element, 1 the second element and so on.
    /// Negative indices can be used to designate elements starting at the tail of the list.
    /// Here, -1 means the last element, -2 means the penultimate and so forth.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lindex"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="index">The index of the element in the list to retrieve.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The element at <paramref name="index"/>.
    /// If <paramref name="index"/> is out of range or if <paramref name="key"/> does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ListGetByIndexAsync(key, 0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Inserts <paramref name="value"/> in the list stored at <paramref name="key"/> after the reference value <paramref name="pivot"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/linsert"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="pivot">The reference point in the list.</param>
    /// <param name="value">The new element to insert.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The length of the list after the insert operation.
    /// If the <paramref name="pivot"/> is not found, -1 is returned.
    /// If <paramref name="key"/> does not exist, 0 is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListInsertAfterAsync(key, "pivot", "new_element");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Inserts <paramref name="value"/> in the list stored at <paramref name="key"/> before the reference value <paramref name="pivot"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/linsert"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="pivot">The reference point in the list.</param>
    /// <param name="value">The new element to insert.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The length of the list after the insert operation.
    /// If the <paramref name="pivot"/> is not found, -1 is returned.
    /// If <paramref name="key"/> does not exist, 0 is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListInsertBeforeAsync(key, "pivot", "new_element");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Atomically returns and removes the first/last element of the list stored at <paramref name="sourceKey"/>
    /// and pushes the element at the first/last element of the list stored at <paramref name="destinationKey"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lmove"/>
    /// <param name="sourceKey">The key of the source list.</param>
    /// <param name="destinationKey">The key of the destination list.</param>
    /// <param name="sourceSide">The side of the source list to pop from (Left = head, Right = tail).</param>
    /// <param name="destinationSide">The side of the destination list to push to (Left = head, Right = tail).</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The element being popped and pushed.
    /// If <paramref name="sourceKey"/> does not exist, <see cref="ValkeyValue.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue result = await client.ListMoveAsync(sourceKey, destKey, ListSide.Left, ListSide.Right);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the index of the first occurrence of <paramref name="element"/> inside the list specified by <paramref name="key"/>.
    /// If no match is found, -1 is returned.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpos"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="element">The element to search for.</param>
    /// <param name="rank">The rank of the match to return (1-based). Negative values indicate searching from the end.</param>
    /// <param name="maxLength">Limit the search to this many elements. 0 means no limit.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// The index of the first occurrence of <paramref name="element"/>, or -1 if not found.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.ListPositionAsync(key, "element", 1, 0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the indices of matching elements inside the list specified by <paramref name="key"/>.
    /// If no matches are found, an empty array is returned.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lpos"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="element">The element to search for.</param>
    /// <param name="count">The maximum number of matches to return.</param>
    /// <param name="rank">The rank of the first match to return (1-based). Negative values indicate searching from the end.</param>
    /// <param name="maxLength">Limit the search to this many elements. 0 means no limit.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// An array of indices of matching elements.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long[] result = await client.ListPositionsAsync(key, "element", 10, 1, 0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets the list element at <paramref name="index"/> to <paramref name="value"/>.
    /// The index is zero-based, so 0 means the first element, 1 the second element and so on.
    /// Negative indices can be used to designate elements starting at the tail of the list.
    /// Here, -1 means the last element, -2 means the penultimate and so forth.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lset"/>
    /// <param name="key">The key of the list.</param>
    /// <param name="index">The index of the element in the list to set.</param>
    /// <param name="value">The new value.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// An error is returned for out of range indexes.
    /// <example>
    /// <code>
    /// await client.ListSetByIndexAsync(key, 0, "new_value");
    /// </code>
    /// </example>
    /// </remarks>
    Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Pops an element from the head of the first list that is non-empty, with the given <paramref name="keys"/> being checked in the order that
    /// they are given.
    /// Blocks the connection when there are no elements to pop from any of the given lists.
    /// <see cref="ListBlockingLeftPopAsync"/> is the blocking variant of <see cref="ListLeftPopAsync(ValkeyKey, CommandFlags)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>BLPOP is a client blocking command, see <see href="https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#blocking-commands">Blocking Commands</see> for more details and best practices.</note>
    /// <param name="keys">The keys of the lists to pop from.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// A two-element array containing the key from which the element was popped and the value of the popped element, formatted as [key, value].
    /// If no element could be popped and the timeout expired, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[]? result = await client.ListBlockingLeftPopAsync(new ValkeyKey[] { "list1", "list2" }, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingLeftPopAsync(ValkeyKey[] keys, TimeSpan timeout, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Pops an element from the tail of the first list that is non-empty, with the given <paramref name="keys"/> being checked in the order that
    /// they are given.
    /// Blocks the connection when there are no elements to pop from any of the given lists.
    /// <see cref="ListBlockingRightPopAsync"/> is the blocking variant of <see cref="ListRightPopAsync(ValkeyKey, CommandFlags)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/brpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>BRPOP is a client blocking command, see <see href="https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#blocking-commands">Blocking Commands</see> for more details and best practices.</note>
    /// <param name="keys">The keys of the lists to pop from.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// A two-element array containing the key from which the element was popped and the value of the popped element, formatted as [key, value].
    /// If no element could be popped and the timeout expired, <see langword="null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[]? result = await client.ListBlockingRightPopAsync(new ValkeyKey[] { "list1", "list2" }, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> ListBlockingRightPopAsync(ValkeyKey[] keys, TimeSpan timeout, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Blocks the connection until it atomically moves an element from the <paramref name="source"/> list to the <paramref name="destination"/> list.
    /// <see cref="ListBlockingMoveAsync"/> is the blocking variant of <see cref="ListMoveAsync"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmove"/>
    /// <note>When in cluster mode, <paramref name="source"/> and <paramref name="destination"/> must map to the same hash slot.</note>
    /// <note>BLMOVE is a client blocking command, see <see href="https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#blocking-commands">Blocking Commands</see> for more details and best practices.</note>
    /// <note>Since Valkey 6.2.0.</note>
    /// <param name="source">The key of the source list.</param>
    /// <param name="destination">The key of the destination list.</param>
    /// <param name="sourceSide">The side of the source list to pop from (Left = head, Right = tail).</param>
    /// <param name="destinationSide">The side of the destination list to push to (Left = head, Right = tail).</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
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
    Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Blocks the connection until it pops one element from the first non-empty list from the provided <paramref name="keys"/>.
    /// <see cref="ListBlockingPopAsync(ValkeyKey[], ListSide, TimeSpan, CommandFlags)"/> is the blocking variant of <see cref="ListLeftPopAsync(ValkeyKey[], long, CommandFlags)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>BLMPOP is a client blocking command, see <see href="https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#blocking-commands">Blocking Commands</see> for more details and best practices.</note>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="keys">An array of keys to lists.</param>
    /// <param name="side">The side of the list to pop from (Left = head, Right = tail).</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no element could be popped and the timeout expired, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ListPopResult result = await client.ListBlockingPopAsync(new ValkeyKey[] { "list1", "list2" }, ListSide.Left, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(ValkeyKey[] keys, ListSide side, TimeSpan timeout, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Blocks the connection until it pops up to <paramref name="count"/> elements from the first non-empty list from the provided <paramref name="keys"/>.
    /// <see cref="ListBlockingPopAsync(ValkeyKey[], ListSide, long, TimeSpan, CommandFlags)"/> is the blocking variant of <see cref="ListLeftPopAsync(ValkeyKey[], long, CommandFlags)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/blmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>BLMPOP is a client blocking command, see <see href="https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#blocking-commands">Blocking Commands</see> for more details and best practices.</note>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="keys">An array of keys to lists.</param>
    /// <param name="side">The side of the list to pop from (Left = head, Right = tail).</param>
    /// <param name="count">The maximum number of elements to pop.</param>
    /// <param name="timeout">The maximum time to wait for a blocking operation to complete. A value of TimeSpan.Zero will block indefinitely.</param>
    /// <param name="flags">Command flags are not supported by GLIDE.</param>
    /// <returns>
    /// A <see cref="ListPopResult"/> containing the key of the list that was popped from and the popped elements.
    /// If no element could be popped and the timeout expired, <see cref="ListPopResult.Null"/> will be returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ListPopResult result = await client.ListBlockingPopAsync(new ValkeyKey[] { "list1", "list2" }, ListSide.Left, 3, TimeSpan.FromSeconds(5));
    /// </code>
    /// </example>
    /// </remarks>
    Task<ListPopResult> ListBlockingPopAsync(ValkeyKey[] keys, ListSide side, long count, TimeSpan timeout, CommandFlags flags = CommandFlags.None);
}
