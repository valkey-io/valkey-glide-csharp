// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.ComponentModel;

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Sorted Set Commands" group for standalone and cluster clients.
/// <br />
/// See more on <see href="https://valkey.io/commands/#sorted-set">valkey.io</see>.
/// </summary>
public interface ISortedSetCommands
{
    /// <inheritdoc cref="SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen, CommandFlags)" />
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, CommandFlags flags);

    /// <inheritdoc cref="SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen, CommandFlags)" />
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Adds members with their scores to the sorted set stored at key.
    /// If a member is already a part of the sorted set, its score is updated.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to add to the sorted set.</param>
    /// <param name="score">The score of the member.</param>
    /// <param name="when">Indicates when this operation should be performed.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns><see langword="true"/> if the member was added. <see langword="false"/> if the member already existed and the score was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.SortedSetAddAsync(key, "member1", 10.5);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetAddAsync(ValkeyKey, SortedSetEntry[], SortedSetWhen, CommandFlags)" />
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Task<long> SortedSetAddAsync(ValkeyKey key, SortedSetEntry[] values, CommandFlags flags);

    /// <inheritdoc cref="SortedSetAddAsync(ValkeyKey, SortedSetEntry[], SortedSetWhen, CommandFlags)" />
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    Task<long> SortedSetAddAsync(ValkeyKey key, SortedSetEntry[] values, When when, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Adds members with their scores to the sorted set stored at key.
    /// If a member is already a part of the sorted set, its score is updated.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="values">An array of <see cref="SortedSetEntry"/> objects representing the members and their scores to add.</param>
    /// <param name="when">Indicates when this operation should be performed.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of elements added to the sorted set, not including elements already existing for which the score was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entries = new SortedSetEntry[]
    /// {
    ///     new SortedSetEntry("member1", 10.5),
    ///     new SortedSetEntry("member2", 8.2)
    /// };
    /// long result = await client.SortedSetAddAsync(key, entries);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetAddAsync(ValkeyKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified member from the sorted set stored at key.
    /// Non existing members are ignored.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrem"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to remove from the sorted set.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns><see langword="true"/> if the member was removed. <see langword="false"/> if the member was not a member of the sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool result = await client.SortedSetRemoveAsync(key, "member1");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified members from the sorted set stored at key.
    /// Non existing members are ignored.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrem"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">An array of members to remove from the sorted set.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of members that were removed from the sorted set, not including non existing members.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.SortedSetRemoveAsync(key, ["member1", "member2"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue[] members, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the sorted set cardinality (number of elements) of the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zcard"/>
    /// <seealso href="https://valkey.io/commands/zcount"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The min score to filter by (defaults to negative infinity).</param>
    /// <param name="max">The max score to filter by (defaults to positive infinity).</param>
    /// <param name="exclude">Whether to exclude <paramref name="min"/> and <paramref name="max"/> from the range check (defaults to both inclusive).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The cardinality (number of elements) of the sorted set, or 0 if key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// // Get total cardinality
    /// long totalCount = await client.SortedSetLengthAsync(key);
    /// 
    /// // Count elements in score range
    /// long rangeCount = await client.SortedSetLengthAsync(key, 1.0, 10.0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetLengthAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the cardinality (number of elements) of the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zcard"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    ///	The number of elements in the sorted set.
    ///	If key does not exist, it is treated as an empty sorted set, and this command returns 0.
    ///	If key holds a value that is not a sorted set, an error is returned.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.SortedSetCardAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetCardAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of members in the sorted set stored at key with scores between min and max score.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zcount"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The minimum score to count from (defaults to negative infinity).</param>
    /// <param name="max">The maximum score to count up to (defaults to positive infinity).</param>
    /// <param name="exclude">Whether to exclude min and max from the range check (defaults to both inclusive).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of members in the specified score range.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.SortedSetCountAsync(key, 1.0, 10.0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetCountAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key by their index (rank).
    /// By default the elements are considered to be ordered from the lowest to the highest score.
    /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on.<br/>
    /// To get the elements with their scores, <see cref="SortedSetRangeByRankWithScoresAsync" />.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The start index to get.</param>
    /// <param name="stop">The stop index to get.</param>
    /// <param name="order">The order to sort by (defaults to ascending).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    ///	An array of elements within the specified range.
    ///	If key does not exist, it is treated as an empty sorted set, and the command returns an empty array.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.SortedSetRangeByRankAsync(key, 0, 10);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified range of elements with their scores in the sorted set stored at key by their index (rank).
    /// By default the elements are considered to be ordered from the lowest to the highest score.
    /// Both start and stop are zero-based indexes, where 0 is the first element, 1 is the next element and so on.<br/>
    /// To get the elements without their scores, <see cref="SortedSetRangeByRankAsync" />.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange"/>
    /// <seealso href="https://redis.io/commands/zrevrange"/>.
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The start index to get.</param>
    /// <param name="stop">The stop index to get.</param>
    /// <param name="order">The order to sort by (defaults to ascending).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    ///	An array of elements and their scores within the specified range.
    ///	If key does not exist, it is treated as an empty sorted set, and the command returns an empty array.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// SortedSetEntry[] result = await client.SortedSetRangeByRankWithScoresAsync(key, 0, 10);
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key by their score.
    /// By default the elements are considered to be ordered from the lowest to the highest score.
    /// Start and stop are used to specify the min and max range for score values.
    /// To get the elements with their scores, <see cref="SortedSetRangeByScoreWithScoresAsync" />.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange"/>
    /// <seealso href="https://redis.io/commands/zrevrange"/>.
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The minimum score to filter by.</param>
    /// <param name="stop">The maximum score to filter by.</param>
    /// <param name="exclude">Which of start and stop to exclude (defaults to both inclusive).</param>
    /// <param name="order">The order to sort by (defaults to ascending).</param>
    /// <param name="skip">How many items to skip.</param>
    /// <param name="take">How many items to take.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    ///	An array of elements within the specified range.
    ///	If key does not exist, it is treated as an empty sorted set, and the command returns an empty array.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.SortedSetRangeByScoreAsync(key, 1.0, 10.0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetRangeByScoreAsync(
        ValkeyKey key,
        double start = double.NegativeInfinity,
        double stop = double.PositiveInfinity,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0,
        long take = -1,
        CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key with their scores by their score.
    /// By default the elements are considered to be ordered from the lowest to the highest score.
    /// Start and stop are used to specify the min and max range for score values 
    /// To get the elements without their scores, <see cref="SortedSetRangeByScoreAsync" />.
    /// </summary>.
    /// <seealso href="https://valkey.io/commands/zrange"/>
    /// <seealso href="https://redis.io/commands/zrevrange"/>.
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="start">The minimum score to filter by.</param>
    /// <param name="stop">The maximum score to filter by.</param>
    /// <param name="exclude">Which of start and stop to exclude (defaults to both inclusive).</param>
    /// <param name="order">The order to sort by (defaults to ascending).</param>
    /// <param name="skip">How many items to skip.</param>
    /// <param name="take">How many items to take.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    ///	An array of elements and their scores within the specified range.
    ///	If key does not exist, it is treated as an empty sorted set, and the command returns an empty array.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// SortedSetEntry[] result = await client.SortedSetRangeByScoreWithScoresAsync(key, 1.0, 10.0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(
        ValkeyKey key,
        double start = double.NegativeInfinity,
        double stop = double.PositiveInfinity,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0,
        long take = -1,
        CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key by their lexicographical order.
    /// This command returns all the elements in the sorted set at key with a value between min and max.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange"/>
    /// <seealso href="https://redis.io/commands/zrevrange"/>.
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The min value to filter by.</param>
    /// <param name="max">The max value to filter by.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="skip">How many items to skip.</param>
    /// <param name="take">How many items to take.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    ///	An array of elements within the specified range.
    ///	If key does not exist, it is treated as an empty sorted set, and the command returns an empty array.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.SortedSetRangeByValueAsync(key, "a", "z", Exclude.None, 0, -1);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(
        ValkeyKey key,
        ValkeyValue min,
        ValkeyValue max,
        Exclude exclude,
        long skip,
        long take = -1,
        CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the specified range of elements in the sorted set stored at key by their lexicographical order.
    /// This command returns all the elements in the sorted set at key with a value between min and max.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange"/>
    /// <seealso href="https://redis.io/commands/zrevrange"/>.
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The min value to filter by.</param>
    /// <param name="max">The max value to filter by.</param>
    /// <param name="exclude">Which of min and max to exclude (defaults to both inclusive).</param>
    /// <param name="order">The order to sort by (defaults to ascending).</param>
    /// <param name="skip">How many items to skip.</param>
    /// <param name="take">How many items to take.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    ///	An array of elements within the specified range.
    ///	If key does not exist, it is treated as an empty sorted set, and the command returns an empty array.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.SortedSetRangeByValueAsync(key, "a", "z", order: Order.Descending);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(
        ValkeyKey key,
        ValkeyValue min = default,
        ValkeyValue max = default,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0,
        long take = -1,
        CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Blocks the connection until it pops and returns a member-score pair from the sorted set stored at key.
    /// This is the blocking variant of . TODO: Put ref here once it is implemented in Sorted Set Commands Batch 2 
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bzpopmin"/>
    /// <seealso href="https://valkey.io/commands/bzpopmax"/>
    /// <note>This is a client blocking command. See <see href="https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#blocking-commands"/> for more details and best practices.</note>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout in seconds. A timeout of zero can be used to block indefinitely.</param>
    /// <param name="flags">The flags to use for the operation. Currently flags are ignored.</param>
    /// <returns>A sorted set entry, or <see langword="null"/> if no element could be popped and the timeout expired.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// SortedSetEntry? result = await client.SortedSetBlockingPopAsync(key, Order.Ascending, 5.0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, double timeout, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Blocks the connection until it pops and returns the specified number of elements from the sorted set stored at key.
    /// This is the blocking variant of TODO: Put ref here once it is implemented in Sorted Set Commands Batch 2 .
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bzpopmin"/>
    /// <seealso href="https://valkey.io/commands/bzpopmax"/>
    /// <note>This is a client blocking command. See <see href="https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#blocking-commands"/> for more details and best practices.</note>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="count">The number of elements to return.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout in seconds. A timeout of zero can be used to block indefinitely.</param>
    /// <param name="flags">The flags to use for the operation. Currently flags are ignored.</param>
    /// <returns>An array of elements, or an empty array when key does not exist or timeout expired.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// SortedSetEntry[] result = await client.SortedSetBlockingPopAsync(key, 2, Order.Ascending, 5.0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetBlockingPopAsync(ValkeyKey key, long count, Order order, double timeout, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Blocks the connection until it pops and returns up to <paramref name="count"/> entries from the first non-empty sorted set.
    /// The given keys are checked in the order they are provided.
    /// This is the blocking variant of <see cref="SortedSetPopAsync(ValkeyKey[], long, Order, CommandFlags)"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bzmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>This is a client blocking command. See <see href="https://github.com/valkey-io/valkey-glide/wiki/General-Concepts#blocking-commands"/> for more details and best practices.</note>
    /// <note>Since Valkey 7.0 and above.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop out of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout in seconds. A timeout of zero can be used to block indefinitely.</param>
    /// <param name="flags">The flags to use for the operation. Currently flags are ignored.</param>
    /// <returns>A contiguous collection of sorted set entries with the key they were popped from, or <see cref="SortedSetPopResult.Null"/> if no non-empty sorted sets are found or timeout expired.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// SortedSetPopResult result = await client.SortedSetBlockingPopAsync(new[] { key1, key2, key3 }, 2, Order.Ascending, 5.0);
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetPopResult> SortedSetBlockingPopAsync(ValkeyKey[] keys, long count, Order order, double timeout, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Three functions:
    /// - Returns the union of members from sorted sets specified by the given keys.
    /// - Returns the intersection of members from sorted sets specified by the given keys.
    /// - Returns the difference between the first sorted set and all the successive sorted sets.
    /// 
    /// Computes a set operation for multiple sorted sets (optionally using per-set weights),
    /// optionally performing a specific aggregation (defaults to Sum).
    /// Difference operation cannot be used with weights or aggregation.
    /// </summary>
    /// <remarks>
    /// See
    /// TODO: add zunion after it has been implemented
    /// <seealso href="https://redis.io/commands/zinter"/>,
    /// <seealso href="https://redis.io/commands/zdiff"/>.
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// </remarks>
    /// <param name="operation">The operation to perform.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="weights">The optional weights per set that correspond to keys.</param>
    /// <param name="aggregate">The aggregation method (defaults to Sum).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The resulting sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] result = await client.SortedSetCombineAsync(SetOperation.Difference, new[] { key1, key2 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Three functions:
    /// - Returns the union of members and their scores from sorted sets specified by the given keys.
    /// - Returns the intersection of members and their scores from sorted sets specified by the given keys.
    /// - Returns the difference between the first sorted set and all the successive sorted sets.
    /// 
    /// Computes a set operation for multiple sorted sets (optionally using per-set weights),
    /// optionally performing a specific aggregation (defaults to Sum) and returns the result with scores.
    /// Difference operation cannot be used with weights or aggregation.
    /// </summary>
    /// <remarks>
    /// See
    /// TODO: add zunion after it has been implemented
    /// <seealso href="https://redis.io/commands/zinter"/>,
    /// <seealso href="https://redis.io/commands/zdiff"/>.
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// </remarks>
    /// <param name="operation">The operation to perform.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="weights">The optional weights per set that correspond to keys.</param>
    /// <param name="aggregate">The aggregation method (defaults to Sum).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The resulting sorted set with scores.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// SortedSetEntry[] result = await client.SortedSetCombineWithScoresAsync(SetOperation.Difference, new[] { key1, key2 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Three functions:
    /// - Computes the intersection of sorted sets given by the specified keys and stores the result in destination. If destination already exists, it is overwritten.
    ///   Otherwise, a new sorted set will be created.
    /// - Calculates the difference between the first sorted set and all the successive sorted sets at keys and stores the difference as a sorted set to destination,
    ///   overwriting it if it already exists. Non-existent keys are treated as empty sets.
    /// - Computes the union of sorted sets given by the specified keys, and stores the result in destination. If destination already exists, it
    ///   is overwritten. Otherwise, a new sorted set will be created.
    /// 
    /// Computes a set operation over two sorted sets, and stores the result in destination, optionally performing
    /// a specific aggregation (defaults to sum).
    /// Difference operation cannot be used with aggregation.
    /// </summary>
    /// <remarks>
    /// See
    /// TODO: add zunionstore after it has been implemented
    /// <seealso href="https://redis.io/commands/zinterstore"/>,
    /// <seealso href="https://redis.io/commands/zdiffstore"/>.
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// </remarks>
    /// <param name="operation">The operation to perform.</param>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="first">The key of the first sorted set.</param>
    /// <param name="second">The key of the second sorted set.</param>
    /// <param name="aggregate">The aggregation method (defaults to sum).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of elements in the resulting sorted set at destination.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.SortedSetCombineAndStoreAsync(SetOperation.Difference, destKey, key1, key2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Three functions:
    /// - Computes the intersection of sorted sets given by the specified keys and stores the result in destination. If destination already exists, it is overwritten.
    ///   Otherwise, a new sorted set will be created.
    /// - Calculates the difference between the first sorted set and all the successive sorted sets at keys and stores the difference as a sorted set to destination,
    ///   overwriting it if it already exists. Non-existent keys are treated as empty sets.
    /// - Computes the union of sorted sets given by the specified keys, and stores the result in destination. If destination already exists, it
    ///   is overwritten. Otherwise, a new sorted set will be created.
    ///
    /// Computes a set operation over multiple sorted sets (optionally using per-set weights), and stores the result in destination, optionally performing
    /// a specific aggregation (defaults to sum).
    /// Difference operation cannot be used with aggregation.
    /// </summary>
    /// <remarks>
    /// See
    /// TODO: add zunionstore after it has been implemented
    /// <seealso href="https://redis.io/commands/zinterstore"/>,
    /// <seealso href="https://redis.io/commands/zdiffstore"/>.
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// </remarks>
    /// <param name="operation">The operation to perform.</param>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="weights">The optional weights per set that correspond to keys.</param>
    /// <param name="aggregate">The aggregation method (defaults to sum).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of elements in the resulting sorted set at destination.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long result = await client.SortedSetCombineAndStoreAsync(SetOperation.Difference, destKey, new[] { key1, key2, key3 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Increments the score of member in the sorted set stored at key by increment.
    /// If member does not exist in the sorted set, it is added with increment as its score. (as if its previous score was 0.0).
    /// If key does not exist, a new sorted set with the specified member as its sole member is created.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zincrby"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="member">The member to increment.</param>
    /// <param name="value">The amount to increment by.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The new score of member.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// double newScore = await client.SortedSetIncrementAsync(key, "member1", 2.5);
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the cardinality of the intersection of the sorted sets at keys.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zintercard"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>Since Valkey 7.0 and above.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="limit">If the intersection cardinality reaches limit partway through the computation, the algorithm will exit and yield limit as the cardinality (defaults to 0 meaning unlimited).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of elements in the resulting intersection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long intersectionCount = await client.SortedSetIntersectionLengthAsync(new[] { key1, key2, key3 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetIntersectionLengthAsync(ValkeyKey[] keys, long limit = 0, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of elements in the sorted set at key with a value between min and max.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zlexcount"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="min">The min value to filter by.</param>
    /// <param name="max">The max value to filter by.</param>
    /// <param name="exclude">Whether to exclude min and max from the range check (defaults to both inclusive).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of elements in the sorted set at key with a value between min and max.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long count = await client.SortedSetLengthByValueAsync(key, "a", "z");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes and returns up to count entries from the first non-empty sorted set in keys.
    /// Returns <see cref="SortedSetPopResult.Null"/> if none of the sets exist or contain any elements.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys to check.</param>
    /// <param name="count">The maximum number of records to pop out of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="flags">The flags to use for the operation. Currently flags are ignored.</param>
    /// <returns>A contiguous collection of sorted set entries with the key they were popped from, or <see cref="SortedSetPopResult.Null"/> if no non-empty sorted sets are found.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// SortedSetPopResult result = await client.SortedSetPopAsync(new[] { key1, key2, key3 }, 2);
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetPopResult> SortedSetPopAsync(ValkeyKey[] keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the scores associated with the specified members in the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmscore"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="members">The members to get the scores for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of scores corresponding to members.
    /// If a member does not exist in the sorted set, the corresponding value in the list will be <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// double?[] scores = await client.SortedSetScoresAsync(key, new[] { "member1", "member2", "member3" });
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?[]> SortedSetScoresAsync(ValkeyKey key, ValkeyValue[] members, CommandFlags flags = CommandFlags.None);
}
