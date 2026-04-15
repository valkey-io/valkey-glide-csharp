// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Sorted set commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="ISortedSetBaseCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to add.</param>
    /// <param name="score">The score for the member.</param>
    /// <param name="when">Condition under which to add (e.g. <see cref="SortedSetWhen.NotExists"/>).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the member was added.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen, CommandFlags)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="values">The members and their scores.</param>
    /// <param name="when">Condition under which to add (e.g. <see cref="SortedSetWhen.NotExists"/>).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of members added to the sorted set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Updates the score of a member in a sorted set. Returns <see langword="true"/> if the score changed.
    /// Uses the ZADD command with the CH (changed) flag.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member to update.</param>
    /// <param name="score">The new score.</param>
    /// <param name="when">Condition for the update.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetUpdateAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Updates the scores of multiple members in a sorted set. Returns the number of members whose scores changed.
    /// Uses the ZADD command with the CH (changed) flag.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/zadd"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="values">The members and their new scores.</param>
    /// <param name="when">Condition for the update.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetUpdateAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when = SortedSetWhen.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.SortedSetCardAsync(ValkeyKey)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="min">The minimum score to filter by.</param>
    /// <param name="max">The maximum score to filter by.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of elements in the sorted set, or within the score range when specified.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetLengthAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAsync(ValkeyKey, RangeOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="start">The start rank.</param>
    /// <param name="stop">The stop rank.</param>
    /// <param name="order">The sort order.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of elements in the specified rank range.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetRangeByRankAsync(ValkeyKey, long, long, Order, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <returns>An array of elements with their scores in the specified rank range.</returns>
    Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAsync(ValkeyKey, RangeOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="start">The minimum score.</param>
    /// <param name="stop">The maximum score.</param>
    /// <param name="exclude">Which of start and stop to exclude.</param>
    /// <param name="order">The sort order.</param>
    /// <param name="skip">The number of elements to skip.</param>
    /// <param name="take">The number of elements to take (-1 for all).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of elements in the specified score range.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <returns>An array of elements with their scores in the specified score range.</returns>
    Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAsync(ValkeyKey, RangeOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="min">The minimum lexicographic value.</param>
    /// <param name="max">The maximum lexicographic value.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="skip">The number of elements to skip.</param>
    /// <param name="take">The number of elements to take (-1 for all).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of elements in the specified lexicographic range.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long, CommandFlags)" path="/*[not(self::param[@name='skip']) and not(self::returns)]"/>
    /// <param name="order">The sort order.</param>
    /// <param name="skip">The number of elements to skip.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of elements in the specified lexicographic range.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min = default, ValkeyValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[self::summary or self::seealso]"/>
    /// <param name="operation">The set operation to perform (Union, Intersect, or Difference).</param>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="weights">Optional weights for each sorted set.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The resulting members, ordered ascending by score.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <returns>The resulting members with scores, ordered ascending by score.</returns>
    Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" path="/*[self::summary or self::seealso]"/>
    /// <param name="operation">The set operation to perform (Union, Intersect, or Difference).</param>
    /// <param name="destination">The key to store the results in.</param>
    /// <param name="first">The first sorted set key.</param>
    /// <param name="second">The second sorted set key.</param>
    /// <param name="aggregate">The aggregation method.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate, CommandFlags)" path="/*[not(self::param[@name='first']) and not(self::param[@name='second'])]"/>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="weights">Optional weights for each sorted set.</param>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member whose score to increment.</param>
    /// <param name="value">The score increment.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The new score of the member.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="member">The member whose score to decrement.</param>
    /// <param name="value">The amount to decrement by.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The new score of the member.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> SortedSetDecrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetInterCardAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetLexCountAsync(ValkeyKey, LexRange)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="min">The minimum lexicographic value.</param>
    /// <param name="max">The maximum lexicographic value.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of members in the lexicographic range.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(ValkeyKey)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="order">The order to pop by (ascending pops min, descending pops max).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A sorted set entry, or <see langword="null"/> when the key does not exist.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="SortedSetPopAsync(ValkeyKey, Order, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <param name="count">The number of members to pop.</param>
    /// <returns>An array of sorted set entries.</returns>
    Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(IEnumerable{ValkeyKey}, long, TimeSpan?)" path="/*[self::summary or self::seealso]"/>
    /// <param name="keys">The keys of the sorted sets.</param>
    /// <param name="count">The maximum number of records to pop.</param>
    /// <param name="order">The order to pop by (ascending pops min, descending pops max).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A <see cref="SortedSetPopResult"/> containing the key and removed elements.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMemberAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMembersAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="count">The number of random members to return.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of randomly selected entries with their scores.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, RangeOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="sourceKey">The source sorted set key.</param>
    /// <param name="destinationKey">The destination sorted set key.</param>
    /// <param name="start">The start of the range.</param>
    /// <param name="stop">The stop of the range.</param>
    /// <param name="sortedSetOrder">The type of range (by rank, score, or lex).</param>
    /// <param name="exclude">Which of start and stop to exclude.</param>
    /// <param name="order">The sort order.</param>
    /// <param name="skip">The number of elements to skip.</param>
    /// <param name="take">The number of elements to take.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of elements in the resulting sorted set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRangeAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long? take = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetRemoveRangeAsync(ValkeyKey, Range)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="min">The minimum lexicographic value.</param>
    /// <param name="max">The maximum lexicographic value.</param>
    /// <param name="exclude">Which of min and max to exclude.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of elements removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetRemoveRangeAsync(ValkeyKey, Range)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="start">The start rank.</param>
    /// <param name="stop">The stop rank.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of elements removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetRemoveRangeAsync(ValkeyKey, Range)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The sorted set key.</param>
    /// <param name="start">The minimum score.</param>
    /// <param name="stop">The maximum score.</param>
    /// <param name="exclude">Which of start and stop to exclude.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of elements removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);
}
