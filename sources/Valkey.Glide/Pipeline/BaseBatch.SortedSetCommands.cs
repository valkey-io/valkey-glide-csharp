// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Sorted Set commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, ValkeyValue, double)" />
    public T SortedSetAdd(ValkeyKey key, ValkeyValue member, double score) => AddCmd(SortedSetAddAsync(key, member, score));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, SortedSetEntry)" />
    public T SortedSetAdd(ValkeyKey key, SortedSetEntry member) => SortedSetAdd(key, member.Element, member.Score);

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, IEnumerable{SortedSetEntry})" />
    public T SortedSetAdd(ValkeyKey key, IEnumerable<SortedSetEntry> members) => AddCmd(SortedSetAddAsync(key, members));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" />
    public T SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddCondition condition) => SortedSetAdd(key, member, score, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)" />
    public T SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options) => AddCmd(SortedSetAddAsync(key, member, score, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, IDictionary{ValkeyValue, double})" />
    public T SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members) => SortedSetAdd(key, members.Select(kvp => new SortedSetEntry(kvp.Key, kvp.Value)));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddCondition)" />
    public T SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddCondition condition) => SortedSetAdd(key, members, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddOptions)" />
    public T SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options) => AddCmd(SortedSetAddAsync(key, members.Select(kvp => new SortedSetEntry(kvp.Key, kvp.Value)), options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRemove(ValkeyKey, ValkeyValue)" />
    public T SortedSetRemove(ValkeyKey key, ValkeyValue member) => AddCmd(SortedSetRemoveAsync(key, member));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRemove(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T SortedSetRemove(ValkeyKey key, IEnumerable<ValkeyValue> members) => AddCmd(SortedSetRemoveAsync(key, [.. members]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCard(ValkeyKey)" />
    public T SortedSetCard(ValkeyKey key) => AddCmd(SortedSetCardAsync(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCount(ValkeyKey, ScoreRange)" />
    public T SortedSetCount(ValkeyKey key, ScoreRange range) => AddCmd(SortedSetCountAsync(key, range));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetLexCount(ValkeyKey, LexRange)" />
    public T SortedSetLexCount(ValkeyKey key, LexRange range) => AddCmd(SortedSetLexCountAsync(key, range));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey, ValkeyValue, double)" />
    public T SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value) => AddCmd(SortedSetIncrementByAsync(key, member, value));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" />
    public T SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition) => SortedSetIncrementBy(key, member, value, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)" />
    public T SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options) => AddCmd(SortedSetIncrementByAsync(key, member, value, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterCard(IEnumerable{ValkeyKey}, long)" />
    public T SortedSetInterCard(IEnumerable<ValkeyKey> keys, long limit = 0) => AddCmd(SortedSetInterCardAsync([.. keys], limit));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMin(ValkeyKey)" />
    public T SortedSetPopMin(ValkeyKey key) => AddCmd(SortedSetPopMinAsync(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMax(ValkeyKey)" />
    public T SortedSetPopMax(ValkeyKey key) => AddCmd(SortedSetPopMaxAsync(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMin(ValkeyKey, long)" />
    public T SortedSetPopMin(ValkeyKey key, long count) => AddCmd(SortedSetPopMinAsync(key, count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMax(ValkeyKey, long)" />
    public T SortedSetPopMax(ValkeyKey key, long count) => AddCmd(SortedSetPopMaxAsync(key, count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMin(IEnumerable{ValkeyKey}, TimeSpan?)" />
    public T SortedSetPopMin(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null)
        => timeout.HasValue
            ? AddCmd(SortedSetPopMinAsync([.. keys], timeout.Value))
            : AddCmd(SortedSetPopMinAsync([.. keys]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMax(IEnumerable{ValkeyKey}, TimeSpan?)" />
    public T SortedSetPopMax(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null)
        => timeout.HasValue
            ? AddCmd(SortedSetPopMaxAsync([.. keys], timeout.Value))
            : AddCmd(SortedSetPopMaxAsync([.. keys]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMin(IEnumerable{ValkeyKey}, long, TimeSpan?)" />
    public T SortedSetPopMin(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null)
        => timeout.HasValue
            ? AddCmd(SortedSetPopMinAsync([.. keys], count, timeout.Value))
            : AddCmd(SortedSetPopMinAsync([.. keys], count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMax(IEnumerable{ValkeyKey}, long, TimeSpan?)" />
    public T SortedSetPopMax(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null)
        => timeout.HasValue
            ? AddCmd(SortedSetPopMaxAsync([.. keys], count, timeout.Value))
            : AddCmd(SortedSetPopMaxAsync([.. keys], count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRandomMember(ValkeyKey)" />
    public T SortedSetRandomMember(ValkeyKey key) => AddCmd(SortedSetRandomMemberAsync(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRandomMembers(ValkeyKey, long)" />
    public T SortedSetRandomMembers(ValkeyKey key, long count) => AddCmd(SortedSetRandomMembersAsync(key, count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRandomMemberWithScore(ValkeyKey)" />
    public T SortedSetRandomMemberWithScore(ValkeyKey key) => AddCmd(SortedSetRandomMemberWithScoreAsync(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRandomMembersWithScores(ValkeyKey, long)" />
    public T SortedSetRandomMembersWithScores(ValkeyKey key, long count) => AddCmd(SortedSetRandomMembersWithScoreAsync(key, count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetScore(ValkeyKey, ValkeyValue)" />
    public T SortedSetScore(ValkeyKey key, ValkeyValue member) => AddCmd(SortedSetScoreAsync(key, member));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetScores(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T SortedSetScores(ValkeyKey key, IEnumerable<ValkeyValue> members) => AddCmd(SortedSetScoresAsync(key, [.. members]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRank(ValkeyKey, ValkeyValue, Order)" />
    public T SortedSetRank(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending) => AddCmd(SortedSetRankAsync(key, member, order));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRankWithScore(ValkeyKey, ValkeyValue, Order)" />
    public T SortedSetRankWithScore(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending) => AddCmd(SortedSetRankWithScoreAsync(key, member, order));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRange(ValkeyKey, RangeOptions)" />
    public T SortedSetRange(ValkeyKey key) => SortedSetRange(key, new RangeOptions());

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRange(ValkeyKey, RangeOptions)" />
    public T SortedSetRange(ValkeyKey key, RangeOptions options) => AddCmd(SortedSetRangeAsync(key, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeWithScores(ValkeyKey, RangeOptions)" />
    public T SortedSetRangeWithScores(ValkeyKey key) => SortedSetRangeWithScores(key, new RangeOptions());

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeWithScores(ValkeyKey, RangeOptions)" />
    public T SortedSetRangeWithScores(ValkeyKey key, RangeOptions options) => AddCmd(SortedSetRangeWithScoresAsync(key, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeAndStore(ValkeyKey, ValkeyKey, RangeOptions)" />
    public T SortedSetRangeAndStore(ValkeyKey source, ValkeyKey destination) => SortedSetRangeAndStore(source, destination, new RangeOptions());

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeAndStore(ValkeyKey, ValkeyKey, RangeOptions)" />
    public T SortedSetRangeAndStore(ValkeyKey source, ValkeyKey destination, RangeOptions options) => AddCmd(SortedSetRangeAndStoreAsync(source, destination, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRemoveRange(ValkeyKey, Range)" />
    public T SortedSetRemoveRange(ValkeyKey key, Range range) => AddCmd(SortedSetRemoveRangeAsync(key, range));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnion(IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetUnion(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetUnionAsync([.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnion(IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetUnion(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetUnionAsync(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnionWithScore(IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetUnionWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetUnionWithScoreAsync([.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnionWithScore(IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetUnionWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetUnionWithScoreAsync(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInter(IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetInter(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetInterAsync([.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInter(IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetInter(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetInterAsync(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterWithScore(IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetInterWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetInterWithScoreAsync([.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterWithScore(IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetInterWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetInterWithScoreAsync(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetDiff(IEnumerable{ValkeyKey})" />
    public T SortedSetDiff(IEnumerable<ValkeyKey> keys) => AddCmd(SortedSetDiffAsync([.. keys]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetDiffWithScore(IEnumerable{ValkeyKey})" />
    public T SortedSetDiffWithScore(IEnumerable<ValkeyKey> keys) => AddCmd(SortedSetDiffWithScoreAsync([.. keys]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnionAndStore(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetUnionAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetUnionAndStoreAsync(destination, [.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnionAndStore(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetUnionAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetUnionAndStoreAsync(destination, keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterAndStore(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetInterAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetInterAndStoreAsync(destination, [.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterAndStore(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetInterAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetInterAndStoreAsync(destination, keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetDiffAndStore(ValkeyKey, IEnumerable{ValkeyKey})" />
    public T SortedSetDiffAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(SortedSetDiffAndStoreAsync(destination, [.. keys]));

    // TODO #287
    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetScan(ValkeyKey, ValkeyValue, int, long)" />
    public T SortedSetScan(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0) => AddCmd(SortedSetScanAsync(key, pattern, pageSize, cursor));

    // Explicit interface implementations for IBatchSortedSetCommands
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, ValkeyValue member, double score) => SortedSetAdd(key, member, score);
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, SortedSetEntry member) => SortedSetAdd(key, member);
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, IEnumerable<SortedSetEntry> members) => SortedSetAdd(key, members);
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddCondition condition) => SortedSetAdd(key, member, score, condition);
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options) => SortedSetAdd(key, member, score, options);
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members) => SortedSetAdd(key, members);
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddCondition condition) => SortedSetAdd(key, members, condition);
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options) => SortedSetAdd(key, members, options);
    IBatch IBatchSortedSetCommands.SortedSetRemove(ValkeyKey key, ValkeyValue member) => SortedSetRemove(key, member);
    IBatch IBatchSortedSetCommands.SortedSetRemove(ValkeyKey key, IEnumerable<ValkeyValue> members) => SortedSetRemove(key, members);
    IBatch IBatchSortedSetCommands.SortedSetCard(ValkeyKey key) => SortedSetCard(key);
    IBatch IBatchSortedSetCommands.SortedSetCount(ValkeyKey key, ScoreRange range) => SortedSetCount(key, range);
    IBatch IBatchSortedSetCommands.SortedSetLexCount(ValkeyKey key, LexRange range) => SortedSetLexCount(key, range);
    IBatch IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value) => SortedSetIncrementBy(key, member, value);
    IBatch IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition) => SortedSetIncrementBy(key, member, value, condition);
    IBatch IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options) => SortedSetIncrementBy(key, member, value, options);
    IBatch IBatchSortedSetCommands.SortedSetInterCard(IEnumerable<ValkeyKey> keys, long limit) => SortedSetInterCard(keys, limit);
    IBatch IBatchSortedSetCommands.SortedSetPopMin(ValkeyKey key) => SortedSetPopMin(key);
    IBatch IBatchSortedSetCommands.SortedSetPopMax(ValkeyKey key) => SortedSetPopMax(key);
    IBatch IBatchSortedSetCommands.SortedSetPopMin(ValkeyKey key, long count) => SortedSetPopMin(key, count);
    IBatch IBatchSortedSetCommands.SortedSetPopMax(ValkeyKey key, long count) => SortedSetPopMax(key, count);
    IBatch IBatchSortedSetCommands.SortedSetPopMin(IEnumerable<ValkeyKey> keys, TimeSpan? timeout) => SortedSetPopMin(keys, timeout);
    IBatch IBatchSortedSetCommands.SortedSetPopMax(IEnumerable<ValkeyKey> keys, TimeSpan? timeout) => SortedSetPopMax(keys, timeout);
    IBatch IBatchSortedSetCommands.SortedSetPopMin(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout) => SortedSetPopMin(keys, count, timeout);
    IBatch IBatchSortedSetCommands.SortedSetPopMax(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout) => SortedSetPopMax(keys, count, timeout);
    IBatch IBatchSortedSetCommands.SortedSetRandomMember(ValkeyKey key) => SortedSetRandomMember(key);
    IBatch IBatchSortedSetCommands.SortedSetRandomMembers(ValkeyKey key, long count) => SortedSetRandomMembers(key, count);
    IBatch IBatchSortedSetCommands.SortedSetRandomMemberWithScore(ValkeyKey key) => SortedSetRandomMemberWithScore(key);
    IBatch IBatchSortedSetCommands.SortedSetRandomMembersWithScores(ValkeyKey key, long count) => SortedSetRandomMembersWithScores(key, count);
    IBatch IBatchSortedSetCommands.SortedSetScore(ValkeyKey key, ValkeyValue member) => SortedSetScore(key, member);
    IBatch IBatchSortedSetCommands.SortedSetScores(ValkeyKey key, IEnumerable<ValkeyValue> members) => SortedSetScores(key, members);
    IBatch IBatchSortedSetCommands.SortedSetRank(ValkeyKey key, ValkeyValue member, Order order) => SortedSetRank(key, member, order);
    IBatch IBatchSortedSetCommands.SortedSetRankWithScore(ValkeyKey key, ValkeyValue member, Order order) => SortedSetRankWithScore(key, member, order);
    IBatch IBatchSortedSetCommands.SortedSetRange(ValkeyKey key) => SortedSetRange(key);
    IBatch IBatchSortedSetCommands.SortedSetRange(ValkeyKey key, RangeOptions options) => SortedSetRange(key, options);
    IBatch IBatchSortedSetCommands.SortedSetRangeWithScores(ValkeyKey key) => SortedSetRangeWithScores(key);
    IBatch IBatchSortedSetCommands.SortedSetRangeWithScores(ValkeyKey key, RangeOptions options) => SortedSetRangeWithScores(key, options);
    IBatch IBatchSortedSetCommands.SortedSetRangeAndStore(ValkeyKey source, ValkeyKey destination) => SortedSetRangeAndStore(source, destination);
    IBatch IBatchSortedSetCommands.SortedSetRangeAndStore(ValkeyKey source, ValkeyKey destination, RangeOptions options) => SortedSetRangeAndStore(source, destination, options);
    IBatch IBatchSortedSetCommands.SortedSetRemoveRange(ValkeyKey key, Range range) => SortedSetRemoveRange(key, range);
    IBatch IBatchSortedSetCommands.SortedSetUnion(IEnumerable<ValkeyKey> keys, Aggregate aggregate) => SortedSetUnion(keys, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetUnion(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate) => SortedSetUnion(keysAndWeights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetUnionWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate) => SortedSetUnionWithScore(keys, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetUnionWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate) => SortedSetUnionWithScore(keysAndWeights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetInter(IEnumerable<ValkeyKey> keys, Aggregate aggregate) => SortedSetInter(keys, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetInter(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate) => SortedSetInter(keysAndWeights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetInterWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate) => SortedSetInterWithScore(keys, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetInterWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate) => SortedSetInterWithScore(keysAndWeights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetDiff(IEnumerable<ValkeyKey> keys) => SortedSetDiff(keys);
    IBatch IBatchSortedSetCommands.SortedSetDiffWithScore(IEnumerable<ValkeyKey> keys) => SortedSetDiffWithScore(keys);
    IBatch IBatchSortedSetCommands.SortedSetUnionAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate) => SortedSetUnionAndStore(destination, keys, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetUnionAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate) => SortedSetUnionAndStore(destination, keysAndWeights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetInterAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate) => SortedSetInterAndStore(destination, keys, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetInterAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate) => SortedSetInterAndStore(destination, keysAndWeights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetDiffAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => SortedSetDiffAndStore(destination, keys);

    // TODO #287
    IBatch IBatchSortedSetCommands.SortedSetScan(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor) => SortedSetScan(key, pattern, pageSize, cursor);
}
