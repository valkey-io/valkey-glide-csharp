// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Adds or updates a member with its score in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd/">Valkey commands – ZADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member and score to add or update.</param>
    /// <returns><see langword="true"/> if the member was added; <see langword="false"/> if the member was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var added = await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// // added == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SortedSetAddAsync(ValkeyKey key, SortedSetEntry member);

    /// <summary>
    /// Adds or updates a member with its score in a sorted set,
    /// subject to the specified <paramref name="condition"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd/">Valkey commands – ZADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to add or update.</param>
    /// <param name="score">The score for the member.</param>
    /// <param name="condition">The condition under which to add or update the member.</param>
    /// <returns><see langword="true"/> if the member was added.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var added = await client.SortedSetAddAsync("myzset", "alice", 1.0, SortedSetAddCondition.OnlyIfNotExists);
    /// // added == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddCondition condition);

    /// <summary>
    /// Adds or updates a member with its score in a sorted set,
    /// subject to the specified <paramref name="options"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd/">Valkey commands – ZADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to add or update.</param>
    /// <param name="score">The score for the member.</param>
    /// <param name="options">Options for adding or updating the member.</param>
    /// <returns><see langword="true"/> if the member was added, or changed if <see cref="SortedSetAddOptions.Changed"/> is set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new SortedSetAddOptions { Changed = true };
    /// var changed = await client.SortedSetAddAsync("myzset", "alice", 2.0, options);
    /// // changed == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options);

    /// <summary>
    /// Adds or updates multiple members with their scores in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd/">Valkey commands – ZADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of members and their scores to add.</param>
    /// <returns>The number of members added to the sorted set, not including members already existing for which the score was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var members = new Dictionary&lt;ValkeyValue, double&gt; { ["alice"] = 1.0, ["bob"] = 2.0 };
    /// var count = await client.SortedSetAddAsync("myzset", members);
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members);

    /// <summary>
    /// Adds or updates multiple members with their scores in a sorted set,
    /// subject to the specified <paramref name="condition"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd/">Valkey commands – ZADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of members and their scores to add.</param>
    /// <param name="condition">The condition under which to add or update the members.</param>
    /// <returns>The number of members added to the sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var members = new Dictionary&lt;ValkeyValue, double&gt; { ["alice"] = 1.0, ["bob"] = 2.0 };
    /// var count = await client.SortedSetAddAsync("myzset", members, SortedSetAddCondition.OnlyIfNotExists);
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddCondition condition);

    /// <summary>
    /// Adds or updates multiple members with their scores in a sorted set,
    /// subject to the specified <paramref name="options"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd/">Valkey commands – ZADD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="members">A dictionary of members and their scores to add.</param>
    /// <param name="options">Options for adding or updating the members.</param>
    /// <returns>The number of members added to the sorted set, or added and updated if <see cref="SortedSetAddOptions.Changed"/> is set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var members = new Dictionary&lt;ValkeyValue, double&gt; { ["alice"] = 1.0, ["bob"] = 2.0 };
    /// var options = new SortedSetAddOptions { Changed = true };
    /// var count = await client.SortedSetAddAsync("myzset", members, options);
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options);

    /// <summary>
    /// Returns the number of members in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zcard/">Valkey commands – ZCARD</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The number of elements in the sorted set, or <c>0</c> if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// var count = await client.SortedSetCardAsync("myzset");
    /// // count == 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetCardAsync(ValkeyKey key);

    /// <summary>
    /// Returns the number of members in a sorted set within the given score range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zcount/">Valkey commands – ZCOUNT</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="range">The score range to count within.</param>
    /// <returns>The number of members in the score range.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var count = await client.SortedSetCountAsync("myzset", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(2.0)));
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetCountAsync(ValkeyKey key, ScoreRange range);

    /// <summary>
    /// Returns the number of members in a sorted set within the given lexicographic range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zlexcount/">Valkey commands – ZLEXCOUNT</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="range">The lexicographic range to count within.</param>
    /// <returns>The number of members in the lexicographic range.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 0));
    /// var count = await client.SortedSetLexCountAsync("myzset", LexRange.Between(LexBound.Inclusive("alice"), LexBound.Inclusive("bob")));
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetLexCountAsync(ValkeyKey key, LexRange range);

    /// <summary>
    /// Increments the score of a member in a sorted set.
    /// If <paramref name="member"/> does not exist, it is added with <paramref name="value"/> as its score.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zincrby/">Valkey commands – ZINCRBY</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member whose score to increment.</param>
    /// <param name="value">The score increment.</param>
    /// <returns>The new score of the member.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// var newScore = await client.SortedSetIncrementByAsync("myzset", "alice", 2.0);
    /// // newScore == 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value);

    /// <summary>
    /// Increments the score of a member in a sorted set,
    /// subject to the specified <paramref name="condition"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zincrby/">Valkey commands – ZINCRBY</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member whose score to increment.</param>
    /// <param name="value">The score increment.</param>
    /// <param name="condition">The condition under which to increment the member's score.</param>
    /// <returns>The new score of the member, or <see langword="null"/> if the operation was not performed due to conditions.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// var newScore = await client.SortedSetIncrementByAsync("myzset", "alice", 2.0, SortedSetAddCondition.OnlyIfExists);
    /// // newScore == 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition);

    /// <summary>
    /// Increments the score of a member in a sorted set,
    /// subject to the specified <paramref name="options"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zincrby/">Valkey commands – ZINCRBY</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member whose score to increment.</param>
    /// <param name="value">The score increment.</param>
    /// <param name="options">Options for adding or incrementing the member.</param>
    /// <returns>The new score of the member, or <see langword="null"/> if the operation was not performed due to conditions.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// var options = new SortedSetAddOptions { Condition = SortedSetAddCondition.OnlyIfExists };
    /// var newScore = await client.SortedSetIncrementByAsync("myzset", "alice", 2.0, options);
    /// // newScore == 3
    /// </code>
    /// </example>
    /// </remarks>
    Task<double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options);

    /// <summary>
    /// Returns the number of members in the intersection of the sorted sets specified by <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zintercard/">Valkey commands – ZINTERCARD</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <note>Since Valkey 7.0.0 and above.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="limit">If the intersection cardinality reaches this limit, the algorithm exits early. A value of <c>0</c> means no limit.</param>
    /// <returns>The number of elements in the resulting intersection.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("bob", 2.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var count = await client.SortedSetInterCardAsync(["zset1", "zset2"]);
    /// // count == 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <summary>
    /// Removes and returns the member with the lowest score from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zpopmin/">Valkey commands – ZPOPMIN</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The removed element, or <see langword="null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// var entry = await client.SortedSetPopMinAsync("myzset");
    /// // entry == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry?> SortedSetPopMinAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns the member with the highest score from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zpopmax/">Valkey commands – ZPOPMAX</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The removed element, or <see langword="null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// var entry = await client.SortedSetPopMaxAsync("myzset");
    /// // entry == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry?> SortedSetPopMaxAsync(ValkeyKey key);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> members with the lowest scores from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zpopmin/">Valkey commands – ZPOPMIN</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="count">The number of members to remove.</param>
    /// <returns>An array of removed elements, ordered from lowest to highest score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var entries = await client.SortedSetPopMinAsync("myzset", 2);
    /// // entries.Length == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetPopMinAsync(ValkeyKey key, long count);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> members with the highest scores from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zpopmax/">Valkey commands – ZPOPMAX</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="count">The number of members to remove.</param>
    /// <returns>An array of removed elements, ordered from highest to lowest score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var entries = await client.SortedSetPopMaxAsync("myzset", 2);
    /// // entries.Length == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetPopMaxAsync(ValkeyKey key, long count);

    /// <summary>
    /// Removes and returns the member with the lowest score from the first non-empty sorted set among the given <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop/">Valkey commands – ZMPOP</seealso>
    /// <seealso href="https://valkey.io/commands/bzmpop/">Valkey commands – BZMPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="timeout">Optional timeout for blocking. If <see langword="null"/>, uses non-blocking pop.</param>
    /// <returns>The removed element, or <see langword="null"/> when no element could be popped.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// var entry = await client.SortedSetPopMinAsync(["zset1", "zset2"]);
    /// // entry == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry?> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null);

    /// <summary>
    /// Removes and returns the member with the highest score from the first non-empty sorted set among the given <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop/">Valkey commands – ZMPOP</seealso>
    /// <seealso href="https://valkey.io/commands/bzmpop/">Valkey commands – BZMPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="timeout">Optional timeout for blocking. If <see langword="null"/>, uses non-blocking pop.</param>
    /// <returns>The removed element, or <see langword="null"/> when no element could be popped.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// var entry = await client.SortedSetPopMaxAsync(["zset1", "zset2"]);
    /// // entry == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry?> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> members with the lowest scores from the first non-empty sorted set among the given <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop/">Valkey commands – ZMPOP</seealso>
    /// <seealso href="https://valkey.io/commands/bzmpop/">Valkey commands – BZMPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop.</param>
    /// <param name="timeout">Optional timeout for blocking. If <see langword="null"/>, uses non-blocking pop.</param>
    /// <returns>A <see cref="SortedSetPopResult"/> containing the key and removed elements, or <see cref="SortedSetPopResult.Null"/> when no elements could be popped.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("bob", 2.0));
    /// var popResult = await client.SortedSetPopMinAsync(["zset1", "zset2"], 2);
    /// // popResult.Key == "zset1", popResult.Entries.Length == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetPopResult> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null);

    /// <summary>
    /// Removes and returns up to <paramref name="count"/> members with the highest scores from the first non-empty sorted set among the given <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zmpop/">Valkey commands – ZMPOP</seealso>
    /// <seealso href="https://valkey.io/commands/bzmpop/">Valkey commands – BZMPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop.</param>
    /// <param name="timeout">Optional timeout for blocking. If <see langword="null"/>, uses non-blocking pop.</param>
    /// <returns>A <see cref="SortedSetPopResult"/> containing the key and removed elements, or <see cref="SortedSetPopResult.Null"/> when no elements could be popped.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("bob", 2.0));
    /// var popResult = await client.SortedSetPopMaxAsync(["zset1", "zset2"], 2);
    /// // popResult.Key == "zset1", popResult.Entries.Length == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetPopResult> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null);

    /// <summary>
    /// Returns a random member with its score from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrandmember/">Valkey commands – ZRANDMEMBER</seealso>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <returns>The randomly selected element with its score, or <see langword="null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// var entry = await client.SortedSetRandomMemberWithScoreAsync("myzset");
    /// // entry == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry?> SortedSetRandomMemberWithScoreAsync(ValkeyKey key);

    /// <summary>
    /// Returns random members with their scores from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrandmember/">Valkey commands – ZRANDMEMBER</seealso>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <param name="count">The number of random members to return.</param>
    /// <returns>An array of randomly selected elements with their scores, or an empty array when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var entries = await client.SortedSetRandomMembersWithScoresAsync("myzset", 2);
    /// // entries.Length == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count);

    /// <summary>
    /// Computes the union of multiple sorted sets specified by <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zunion/">Valkey commands – ZUNION</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("bob", 2.0));
    /// var members = await client.SortedSetUnionAsync(["zset1", "zset2"]);
    /// // members == ["alice", "bob"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetUnionAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the union of multiple sorted sets with weights.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zunion/">Valkey commands – ZUNION</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("bob", 2.0));
    /// var keysAndWeights = new Dictionary&lt;ValkeyKey, double&gt; { ["zset1"] = 2.0, ["zset2"] = 1.0 };
    /// var members = await client.SortedSetUnionAsync(keysAndWeights);
    /// // members == ["bob", "alice"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetUnionAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the union of multiple sorted sets specified by <paramref name="keys"/>, returning members with scores.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zunion/">Valkey commands – ZUNION</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("bob", 2.0));
    /// var entries = await client.SortedSetUnionWithScoreAsync(["zset1", "zset2"]);
    /// // entries[0] == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetUnionWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the union of multiple sorted sets with weights, returning members with scores.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zunion/">Valkey commands – ZUNION</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("bob", 2.0));
    /// var keysAndWeights = new Dictionary&lt;ValkeyKey, double&gt; { ["zset1"] = 2.0, ["zset2"] = 1.0 };
    /// var entries = await client.SortedSetUnionWithScoreAsync(keysAndWeights);
    /// // entries[0] == { Element: "bob", Score: 2 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetUnionWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the intersection of multiple sorted sets specified by <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zinter/">Valkey commands – ZINTER</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("bob", 2.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var members = await client.SortedSetInterAsync(["zset1", "zset2"]);
    /// // members == ["alice"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetInterAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the intersection of multiple sorted sets with weights.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zinter/">Valkey commands – ZINTER</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var keysAndWeights = new Dictionary&lt;ValkeyKey, double&gt; { ["zset1"] = 2.0, ["zset2"] = 1.0 };
    /// var members = await client.SortedSetInterAsync(keysAndWeights);
    /// // members == ["alice"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetInterAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the intersection of multiple sorted sets specified by <paramref name="keys"/>, returning members with scores.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zinter/">Valkey commands – ZINTER</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var entries = await client.SortedSetInterWithScoreAsync(["zset1", "zset2"]);
    /// // entries[0] == { Element: "alice", Score: 4 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetInterWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the intersection of multiple sorted sets with weights, returning members with scores.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zinter/">Valkey commands – ZINTER</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var keysAndWeights = new Dictionary&lt;ValkeyKey, double&gt; { ["zset1"] = 2.0, ["zset2"] = 1.0 };
    /// var entries = await client.SortedSetInterWithScoreAsync(keysAndWeights);
    /// // entries[0] == { Element: "alice", Score: 5 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetInterWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the difference between the first sorted set and all successive sorted sets specified by <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zdiff/">Valkey commands – ZDIFF</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("bob", 2.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var members = await client.SortedSetDiffAsync(["zset1", "zset2"]);
    /// // members == ["bob"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetDiffAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Computes the difference between the first sorted set and all successive sorted sets specified by <paramref name="keys"/>, returning members with scores.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zdiff/">Valkey commands – ZDIFF</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("bob", 2.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var entries = await client.SortedSetDiffWithScoreAsync(["zset1", "zset2"]);
    /// // entries[0] == { Element: "bob", Score: 2 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetDiffWithScoreAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Computes the union of multiple sorted sets and stores the result in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zunionstore/">Valkey commands – ZUNIONSTORE</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("bob", 2.0));
    /// var count = await client.SortedSetUnionAndStoreAsync("dest", ["zset1", "zset2"]);
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the union of multiple sorted sets with weights and stores the result in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zunionstore/">Valkey commands – ZUNIONSTORE</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("bob", 2.0));
    /// var keysAndWeights = new Dictionary&lt;ValkeyKey, double&gt; { ["zset1"] = 2.0, ["zset2"] = 1.0 };
    /// var count = await client.SortedSetUnionAndStoreAsync("dest", keysAndWeights);
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the intersection of multiple sorted sets and stores the result in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zinterstore/">Valkey commands – ZINTERSTORE</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var count = await client.SortedSetInterAndStoreAsync("dest", ["zset1", "zset2"]);
    /// // count == 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetInterAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the intersection of multiple sorted sets with weights and stores the result in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zinterstore/">Valkey commands – ZINTERSTORE</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keysAndWeights">A dictionary of sorted set keys and their corresponding weights.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var keysAndWeights = new Dictionary&lt;ValkeyKey, double&gt; { ["zset1"] = 2.0, ["zset2"] = 1.0 };
    /// var count = await client.SortedSetInterAndStoreAsync("dest", keysAndWeights);
    /// // count == 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetInterAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <summary>
    /// Computes the difference between the first sorted set and all successive sorted sets and stores the result in <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zdiffstore/">Valkey commands – ZDIFFSTORE</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("bob", 2.0));
    /// await client.SortedSetAddAsync("zset2", new SortedSetEntry("alice", 3.0));
    /// var count = await client.SortedSetDiffAndStoreAsync("dest", ["zset1", "zset2"]);
    /// // count == 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetDiffAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Returns the rank of a member in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrank/">Valkey commands – ZRANK</seealso>
    /// <seealso href="https://valkey.io/commands/zrevrank/">Valkey commands – ZREVRANK</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to get the rank of.</param>
    /// <param name="order">The order to sort by (defaults to ascending).</param>
    /// <returns>The rank of the member, or <see langword="null"/> if <paramref name="member"/> or <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var rank = await client.SortedSetRankAsync("myzset", "bob");
    /// // rank == 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending);

    /// <summary>
    /// Returns the rank and score of a member in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrank/">Valkey commands – ZRANK</seealso>
    /// <seealso href="https://valkey.io/commands/zrevrank/">Valkey commands – ZREVRANK</seealso>
    /// <note>Since Valkey 7.2.0 and above.</note>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to get the rank and score of.</param>
    /// <param name="order">The order to sort by.</param>
    /// <returns>A tuple of the rank and score, or <see langword="null"/> if <paramref name="member"/> or <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var rankAndScore = await client.SortedSetRankWithScoreAsync("myzset", "bob");
    /// // rankAndScore == { Rank: 1, Score: 2 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<(long Rank, double Score)?> SortedSetRankWithScoreAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending);

    /// <summary>
    /// Returns all elements in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange/">Valkey commands – ZRANGE</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <returns>An array of elements in the sorted set, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var members = await client.SortedSetRangeAsync("myzset");
    /// // members == ["alice", "bob"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key);

    /// <summary>
    /// Returns all elements in a sorted set within the given range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange/">Valkey commands – ZRANGE</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="options">Options specifying the range, order, and limits.</param>
    /// <returns>An array of elements in the specified range.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var members = await client.SortedSetRangeAsync("myzset", new RangeOptions { Range = ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(2.0)) });
    /// // members == ["alice", "bob"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key, RangeOptions options);

    /// <summary>
    /// Returns all elements with their scores in a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange/">Valkey commands – ZRANGE</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <returns>An array of elements with their scores, ordered ascending by score.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var entries = await client.SortedSetRangeWithScoresAsync("myzset");
    /// // entries[0] == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetRangeWithScoresAsync(ValkeyKey key);

    /// <summary>
    /// Returns all elements with their scores in a sorted set within the given range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrange/">Valkey commands – ZRANGE</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="options">Options specifying the range, order, and limits.</param>
    /// <returns>An array of elements with their scores in the specified range.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var entries = await client.SortedSetRangeWithScoresAsync("myzset", new RangeOptions { Range = ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(2.0)) });
    /// // entries[0] == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry[]> SortedSetRangeWithScoresAsync(ValkeyKey key, RangeOptions options);

    /// <summary>
    /// Stores all elements from the sorted set at <paramref name="source"/> into <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrangestore/">Valkey commands – ZRANGESTORE</seealso>
    /// <param name="source">The key of the source sorted set.</param>
    /// <param name="destination">The key of the destination sorted set.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("src", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("src", new SortedSetEntry("bob", 2.0));
    /// var count = await client.SortedSetRangeAndStoreAsync("src", "dest");
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination);

    /// <summary>
    /// Stores elements from the sorted set at <paramref name="source"/> within the given range into <paramref name="destination"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zrangestore/">Valkey commands – ZRANGESTORE</seealso>
    /// <param name="source">The key of the source sorted set.</param>
    /// <param name="destination">The key of the destination sorted set.</param>
    /// <param name="options">Options specifying the range, order, and limits.</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("src", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("src", new SortedSetEntry("bob", 2.0));
    /// var count = await client.SortedSetRangeAndStoreAsync("src", "dest", new RangeOptions { Range = ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(2.0)) });
    /// // count == 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination, RangeOptions options);

    /// <summary>
    /// Removes all elements in a sorted set within the given range.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zremrangebyrank/">Valkey commands – ZREMRANGEBYRANK</seealso>
    /// <seealso href="https://valkey.io/commands/zremrangebyscore/">Valkey commands – ZREMRANGEBYSCORE</seealso>
    /// <seealso href="https://valkey.io/commands/zremrangebylex/">Valkey commands – ZREMRANGEBYLEX</seealso>
    /// <param name="key">The sorted set key.</param>
    /// <param name="range">The range specification.</param>
    /// <returns>The number of elements removed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("bob", 2.0));
    /// var removed = await client.SortedSetRemoveRangeAsync("myzset", ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(1.5)));
    /// // removed == 1
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> SortedSetRemoveRangeAsync(ValkeyKey key, Range range);

    /// <summary>
    /// Blocks the connection until it pops and returns a member-score pair from a sorted set.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bzpopmin/">Valkey commands – BZPOPMIN</seealso>
    /// <seealso href="https://valkey.io/commands/bzpopmax/">Valkey commands – BZPOPMAX</seealso>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout for the blocking operation.</param>
    /// <returns>A sorted set entry, or <see langword="null"/> if no element could be popped and the <paramref name="timeout"/> expired.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("myzset", new SortedSetEntry("alice", 1.0));
    /// var entry = await client.SortedSetBlockingPopAsync("myzset", Order.Ascending, TimeSpan.FromSeconds(5));
    /// // entry == { Element: "alice", Score: 1 }
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, TimeSpan timeout);

    /// <summary>
    /// Blocks the connection until it pops and returns up to <paramref name="count"/> entries from the first non-empty sorted set among the given <paramref name="keys"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/bzmpop/">Valkey commands – BZMPOP</seealso>
    /// <note>When in cluster mode, all keys must map to the same hash slot.</note>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop out of the sorted set.</param>
    /// <param name="order">The order to sort by when popping items out of the set.</param>
    /// <param name="timeout">The timeout for the blocking operation.</param>
    /// <returns>A <see cref="SortedSetPopResult"/> with the key and popped entries, or <see cref="SortedSetPopResult.Null"/> if no non-empty sorted sets are found or <paramref name="timeout"/> expired.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SortedSetAddAsync("zset1", new SortedSetEntry("alice", 1.0));
    /// var popResult = await client.SortedSetBlockingPopAsync(["zset1", "zset2"], 1, Order.Ascending, TimeSpan.FromSeconds(5));
    /// // popResult.Key == "zset1"
    /// </code>
    /// </example>
    /// </remarks>
    Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, TimeSpan timeout);

    /// <summary>
    /// Iterates over elements and their scores in a sorted set using a cursor.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zscan/">Valkey commands – ZSCAN</seealso>
    /// <param name="key">The key of the sorted set.</param>
    /// <param name="options">Optional scan options including pattern and count hint.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the sorted set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await foreach (var entry in client.SortedSetScanAsync("myzset"))
    /// {
    ///     Console.WriteLine($"{entry.Element}: {entry.Score}");
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var options = new ScanOptions { MatchPattern = "*pattern*" };
    /// await foreach (var entry in client.SortedSetScanAsync("myzset", options))
    /// {
    ///     Console.WriteLine($"{entry.Element}: {entry.Score}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ScanOptions? options = null);
}
