// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <inheritdoc cref="Commands.ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double)" path="/*[not(self::param[@name='member']) and not(self::param[@name='score'])]"/>
    /// <param name="member">The member and score to add or update.</param>
    Task<bool> SortedSetAddAsync(ValkeyKey key, SortedSetEntry member);

    /// <inheritdoc cref="Commands.ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double)" path="/*[not(self::returns)]"/>
    /// <param name="condition">The condition under which to add or update the member.</param>
    /// <returns><see langword="true"/> if the member was added.</returns>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddCondition condition);

    /// <inheritdoc cref="Commands.ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double)" path="/*[not(self::returns)]"/>
    /// <param name="options">Options for adding or updating the member.</param>
    /// <returns><see langword="true"/> if the member was added, or changed if <see cref="SortedSetAddOptions.Changed"/> is set.</returns>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options);

    /// <inheritdoc cref="Commands.ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry})" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of members and their scores to add.</param>
    /// <returns>The number of members added to the sorted set, not including members already existing for which the score was updated.</returns>
    Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members);

    /// <inheritdoc cref="SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double})" path="/*[not(self::returns)]"/>
    /// <param name="condition">The condition under which to add or update the members.</param>
    /// <returns>The number of members added to the sorted set.</returns>
    Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddCondition condition);

    /// <inheritdoc cref="SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddCondition)" path="/*[not(self::param[@name='condition']) and not(self::returns)]"/>
    /// <param name="options">Options for adding or updating the members.</param>
    /// <returns>The number of members added to the sorted set, or added and updated if <see cref="SortedSetAddOptions.Changed"/> is set.</returns>
    Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options);

    /// <summary>
    /// Returns the number of members in the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zcard"/>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The number of elements in the sorted set, or 0 if the key does not exist.</returns>
    Task<long> SortedSetCardAsync(ValkeyKey key);

    /// <summary>
    /// Returns the number of members in the sorted set within a score range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zcount"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="range">The score range to count within.</param>
    /// <returns>The number of members in the score range.</returns>
    Task<long> SortedSetCountAsync(ValkeyKey key, ScoreRange range);

    /// <summary>
    /// Returns the number of members in the sorted set within a lexicographic range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zlexcount"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="range">The lexicographic range to count within.</param>
    /// <returns>The number of members in the lexicographic range.</returns>
    Task<long> SortedSetLexCountAsync(ValkeyKey key, LexRange range);

    /// <summary>
    /// Adds a member to the sorted set or increments its score.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zincrby"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member whose score to increment.</param>
    /// <param name="value">The score increment.</param>
    /// <returns>The new score of the member.</returns>
    Task<double> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value);

    /// <inheritdoc cref="SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double)" path="/*[not(self::returns)]"/>
    /// <param name="condition">The condition under which to increment the member's score.</param>
    /// <returns>The new score of the member, or <see langword="null"/> if the operation was not performed due to conditions.</returns>
    Task<double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition);

    /// <inheritdoc cref="SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" path="/*[not(self::param[@name='condition'])]"/>
    /// <param name="options">Options for adding or incrementing the member.</param>
    Task<double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options);

    /// <summary>
    /// Returns the number of members in the intersection of the sorted sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zintercard"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>Since Valkey 7.0 and above.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="limit">If the intersection cardinality reaches this limit, the algorithm exits early. A value of 0 means no limit.</param>
    /// <returns>The number of elements in the resulting intersection.</returns>
    Task<long> SortedSetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <summary>
    /// Removes and returns the member with the lowest score.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zpopmin"/>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The removed element, or <see langword="null"/> when the key does not exist.</returns>
    Task<SortedSetEntry?> SortedSetPopMinAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns the member with the highest score.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zpopmax"/>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The removed element, or <see langword="null"/> when the key does not exist.</returns>
    Task<SortedSetEntry?> SortedSetPopMaxAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> members with the lowest scores.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zpopmin"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="count">The number of members to remove.</param>
    /// <returns>An array of removed elements.</returns>
    Task<SortedSetEntry[]> SortedSetPopMinAsync(ValkeyKey key, long count);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> members with the highest scores.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zpopmax"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="count">The number of members to remove.</param>
    /// <returns>An array of removed elements.</returns>
    Task<SortedSetEntry[]> SortedSetPopMaxAsync(ValkeyKey key, long count);

    /// <summary>
    /// Removes and returns the member with the lowest score from the first non-empty sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop"/>
    /// <seealso href="https://valkey.io/commands/bzmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="timeout">Optional timeout for blocking. If <see langword="null"/>, uses non-blocking pop.</param>
    /// <returns>The removed element, or <see langword="null"/> when no element could be popped.</returns>
    Task<SortedSetEntry?> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null);

    /// <summary>
    /// Removes and returns the member with the highest score from the first non-empty sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop"/>
    /// <seealso href="https://valkey.io/commands/bzmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="timeout">Optional timeout for blocking. If <see langword="null"/>, uses non-blocking pop.</param>
    /// <returns>The removed element, or <see langword="null"/> when no element could be popped.</returns>
    Task<SortedSetEntry?> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> members with the lowest scores from the first non-empty sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop"/>
    /// <seealso href="https://valkey.io/commands/bzmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop.</param>
    /// <param name="timeout">Optional timeout for blocking. If <see langword="null"/>, uses non-blocking pop.</param>
    /// <returns>A <see cref="SortedSetPopResult"/> containing the key and removed elements, or <see cref="SortedSetPopResult.Null"/> when no elements could be popped.</returns>
    Task<SortedSetPopResult> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> members with the highest scores from the first non-empty sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop"/>
    /// <seealso href="https://valkey.io/commands/bzmpop"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop.</param>
    /// <param name="timeout">Optional timeout for blocking. If <see langword="null"/>, uses non-blocking pop.</param>
    /// <returns>A <see cref="SortedSetPopResult"/> containing the key and removed elements, or <see cref="SortedSetPopResult.Null"/> when no elements could be popped.</returns>
    Task<SortedSetPopResult> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null);

    /// <summary>
    /// Returns a random member with its score from the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrandmember"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The randomly selected element with its score, or <see langword="null"/> when the key does not exist.</returns>
    Task<SortedSetEntry?> SortedSetRandomMemberWithScoreAsync(ValkeyKey key);

    /// <summary>
    /// Returns random members with their scores from the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrandmember"/>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <param name="count">The number of random members to return.</param>
    /// <returns>An array of randomly selected elements with their scores, or an empty array when the key does not exist.</returns>
    Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count);

    /// <summary>
    /// Computes the union of multiple sorted sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zunion"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    Task<ValkeyValue[]> SortedSetUnionAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="SortedSetUnionAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::param[@name='keys'])]"/>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    Task<ValkeyValue[]> SortedSetUnionAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="SortedSetUnionAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::returns)]"/>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    Task<SortedSetEntry[]> SortedSetUnionWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="SortedSetUnionWithScoreAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::param[@name='keys'])]"/>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    Task<SortedSetEntry[]> SortedSetUnionWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the intersection of multiple sorted sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zinter"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    Task<ValkeyValue[]> SortedSetInterAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="SortedSetInterAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::param[@name='keys'])]"/>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    Task<ValkeyValue[]> SortedSetInterAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="SortedSetInterAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::returns)]"/>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    Task<SortedSetEntry[]> SortedSetInterWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="SortedSetInterWithScoreAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::param[@name='keys'])]"/>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    Task<SortedSetEntry[]> SortedSetInterWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the difference between the first sorted set and all successive sorted sets.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zdiff"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    Task<ValkeyValue[]> SortedSetDiffAsync(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="SortedSetDiffAsync(IEnumerable{ValkeyKey})" path="/*[not(self::returns)]"/>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    Task<SortedSetEntry[]> SortedSetDiffWithScoreAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Computes the union of multiple sorted sets and stores the result in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zunionstore"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    Task<long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="SortedSetUnionAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::param[@name='keys'])]"/>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    Task<long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the intersection of multiple sorted sets and stores the result in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zinterstore"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    Task<long> SortedSetInterAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="SortedSetInterAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::param[@name='keys'])]"/>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    Task<long> SortedSetInterAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the difference between the first sorted set and all successive sorted sets and stores the result in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zdiffstore"/>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    Task<long> SortedSetDiffAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the rank of a member in the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrank"/>
    /// <seealso href="https://valkey.io/commands/zrevrank"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to get the rank of.</param>
    /// <param name="order">The order to sort by (defaults to ascending).</param>
    /// <returns>The rank of the member, or <see langword="null"/> if the member or key does not exist.</returns>
    Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending);

    /// <summary>
    /// Returns the rank and score of a member in the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrank"/>
    /// <seealso href="https://valkey.io/commands/zrevrank"/>
    /// <note>Since Valkey 7.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to get the rank and score of.</param>
    /// <param name="order">The order to sort by.</param>
    /// <returns>A tuple of the rank and score, or <see langword="null"/> if the member or key does not exist.</returns>
    Task<(long Rank, double Score)?> SortedSetRankWithScoreAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending);

    /// <summary>
    /// Returns all elements in the sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange/"/>
    /// <param name="key">The sorted set key.</param>
    /// <returns>An array of elements in the specified range.</returns>
    Task<ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key);

    /// <summary>
    /// Returns all elements in the sorted set within the given range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="options">Options specifying the range, order, and limits.</param>
    /// <returns>An array of elements in the specified range.</returns>
    Task<ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key, RangeOptions options);

    /// <inheritdoc cref="SortedSetRangeAsync(ValkeyKey)" path="/*[not(self::returns)]"/>
    /// <returns>An array of elements with their scores in the specified range.</returns>
    Task<SortedSetEntry[]> SortedSetRangeWithScoresAsync(ValkeyKey key);

    /// <inheritdoc cref="SortedSetRangeAsync(ValkeyKey, RangeOptions)" path="/*[not(self::returns)]"/>
    /// <param name="options">Options specifying the range, order, and limits.</param>
    /// <returns>An array of elements with their scores in the specified range.</returns>
    Task<SortedSetEntry[]> SortedSetRangeWithScoresAsync(ValkeyKey key, RangeOptions options);

    /// <summary>
    /// Stores all elements in the sorted set to <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrangestore/"/>
    /// <param name="source">The key of the source sorted set.</param>
    /// <param name="destination">The key of the destination sorted set.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    Task<long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination);

    /// <summary>
    /// Stores all elements in the sorted set within the given range to <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrangestore/"/>
    /// <param name="source">The key of the source sorted set.</param>
    /// <param name="destination">The key of the destination sorted set.</param>
    /// <param name="options">Options specifying the range, order, and limits.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    Task<long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination, RangeOptions options);

    /// <summary>
    /// Removes all elements in the sorted set within the given range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zremrangebyrank/"/>
    /// <seealso href="https://valkey.io/commands/zremrangebyscore/"/>
    /// <seealso href="https://valkey.io/commands/zremrangebylex/"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="range">The range specification.</param>
    /// <returns>The number of elements removed.</returns>
    Task<long> SortedSetRemoveRangeAsync(ValkeyKey key, Range range);

    /// <summary>
    /// Blocks the connection until it pops and returns a member-score pair from the sorted set stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bzpopmin"/>
    /// <seealso href="https://valkey.io/commands/bzpopmax"/>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout for the blocking operation.</param>
    /// <returns>A sorted set entry, or <see langword="null"/> if no element could be popped and the timeout expired.</returns>
    Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, TimeSpan timeout);

    /// <summary>
    /// Blocks the connection until it pops and returns up to <paramref name="count"/> entries from the first non-empty sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bzmpop"/>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop out of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout for the blocking operation.</param>
    /// <returns>A contiguous collection of sorted set entries with the key they were popped from, or <see cref="SortedSetPopResult.Null"/> if no non-empty sorted sets are found or timeout expired.</returns>
    Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, TimeSpan timeout);

    // TODO #287
    /// <summary>
    /// Iterates elements of the sorted set and their associated scores using a cursor.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zscan"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="pageSize">The page size to iterate by.</param>
    /// <param name="cursor">The cursor position to start at.</param>
    /// <param name="pageOffset">The page offset to start at.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the sorted set.</returns>
    IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0);
}
