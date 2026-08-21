// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Sorted Set commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, ValkeyValue, double)" />
    public T SortedSetAdd(ValkeyKey key, ValkeyValue member, double score) => AddCmd(Request.SortedSetAdd(key, member, score));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, SortedSetEntry)" />
    public T SortedSetAdd(ValkeyKey key, SortedSetEntry member) => SortedSetAdd(key, member.Element, member.Score);

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, IEnumerable{SortedSetEntry})" />
    public T SortedSetAdd(ValkeyKey key, IEnumerable<SortedSetEntry> members) => AddCmd(Request.SortedSetAdd(key, members));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" />
    public T SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddCondition condition) => SortedSetAdd(key, member, score, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)" />
    public T SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options) => AddCmd(Request.SortedSetAdd(key, member, score, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, IDictionary{ValkeyValue, double})" />
    public T SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members) => SortedSetAdd(key, members.Select(kvp => new SortedSetEntry(kvp.Key, kvp.Value)));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddCondition)" />
    public T SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddCondition condition) => SortedSetAdd(key, members, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddOptions)" />
    public T SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options) => AddCmd(Request.SortedSetAdd(key, members.Select(kvp => new SortedSetEntry(kvp.Key, kvp.Value)), options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRemove(ValkeyKey, ValkeyValue)" />
    public T SortedSetRemove(ValkeyKey key, ValkeyValue member) => AddCmd(Request.SortedSetRemove(key, member));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRemove(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T SortedSetRemove(ValkeyKey key, IEnumerable<ValkeyValue> members) => AddCmd(Request.SortedSetRemove(key, [.. members]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCard(ValkeyKey)" />
    public T SortedSetCard(ValkeyKey key) => AddCmd(Request.SortedSetCard(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCount(ValkeyKey, ScoreRange)" />
    public T SortedSetCount(ValkeyKey key, ScoreRange range) => AddCmd(Request.SortedSetCount(key, range));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetLexCount(ValkeyKey, LexRange)" />
    public T SortedSetLexCount(ValkeyKey key, LexRange range) => AddCmd(Request.SortedSetLexCount(key, range));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey, ValkeyValue, double)" />
    public T SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value) => AddCmd(Request.SortedSetIncrementBy(key, member, value));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" />
    public T SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition) => SortedSetIncrementBy(key, member, value, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetIncrementBy(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)" />
    public T SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options) => AddCmd(Request.SortedSetIncrementBy(key, member, value, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterCard(IEnumerable{ValkeyKey}, long)" />
    public T SortedSetInterCard(IEnumerable<ValkeyKey> keys, long limit = 0) => AddCmd(Request.SortedSetInterCard([.. keys], limit));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMin(ValkeyKey)" />
    public T SortedSetPopMin(ValkeyKey key) => AddCmd(Request.SortedSetPopMin(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMax(ValkeyKey)" />
    public T SortedSetPopMax(ValkeyKey key) => AddCmd(Request.SortedSetPopMax(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMin(ValkeyKey, long)" />
    public T SortedSetPopMin(ValkeyKey key, long count) => AddCmd(Request.SortedSetPopMin(key, count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMax(ValkeyKey, long)" />
    public T SortedSetPopMax(ValkeyKey key, long count) => AddCmd(Request.SortedSetPopMax(key, count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMin(IEnumerable{ValkeyKey}, TimeSpan?)" />
    public T SortedSetPopMin(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null)
        => timeout.HasValue
            ? AddCmd(Request.SortedSetPopMin([.. keys], timeout.Value))
            : AddCmd(Request.SortedSetPopMin([.. keys]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMax(IEnumerable{ValkeyKey}, TimeSpan?)" />
    public T SortedSetPopMax(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null)
        => timeout.HasValue
            ? AddCmd(Request.SortedSetPopMax([.. keys], timeout.Value))
            : AddCmd(Request.SortedSetPopMax([.. keys]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMin(IEnumerable{ValkeyKey}, long, TimeSpan?)" />
    public T SortedSetPopMin(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null)
        => timeout.HasValue
            ? AddCmd(Request.SortedSetPopMin([.. keys], count, timeout.Value))
            : AddCmd(Request.SortedSetPopMin([.. keys], count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPopMax(IEnumerable{ValkeyKey}, long, TimeSpan?)" />
    public T SortedSetPopMax(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null)
        => timeout.HasValue
            ? AddCmd(Request.SortedSetPopMax([.. keys], count, timeout.Value))
            : AddCmd(Request.SortedSetPopMax([.. keys], count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRandomMember(ValkeyKey)" />
    public T SortedSetRandomMember(ValkeyKey key) => AddCmd(Request.SortedSetRandomMember(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRandomMembers(ValkeyKey, long)" />
    public T SortedSetRandomMembers(ValkeyKey key, long count) => AddCmd(Request.SortedSetRandomMembers(key, count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRandomMemberWithScore(ValkeyKey)" />
    public T SortedSetRandomMemberWithScore(ValkeyKey key) => AddCmd(Request.SortedSetRandomMemberWithScore(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRandomMembersWithScores(ValkeyKey, long)" />
    public T SortedSetRandomMembersWithScores(ValkeyKey key, long count) => AddCmd(Request.SortedSetRandomMembersWithScore(key, count));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetScan(ValkeyKey, long, ScanOptions)" />
    public T SortedSetScan(ValkeyKey key, long cursor = 0, ScanOptions? options = null) => AddCmd(Request.SortedSetScan(key, cursor, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetScore(ValkeyKey, ValkeyValue)" />
    public T SortedSetScore(ValkeyKey key, ValkeyValue member) => AddCmd(Request.SortedSetScore(key, member));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetScores(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T SortedSetScores(ValkeyKey key, IEnumerable<ValkeyValue> members) => AddCmd(Request.SortedSetScores(key, [.. members]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRank(ValkeyKey, ValkeyValue, Order)" />
    public T SortedSetRank(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending) => AddCmd(Request.SortedSetRank(key, member, order));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRankWithScore(ValkeyKey, ValkeyValue, Order)" />
    public T SortedSetRankWithScore(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending) => AddCmd(Request.SortedSetRankWithScore(key, member, order));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRange(ValkeyKey, RangeOptions)" />
    public T SortedSetRange(ValkeyKey key) => SortedSetRange(key, new RangeOptions());

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRange(ValkeyKey, RangeOptions)" />
    public T SortedSetRange(ValkeyKey key, RangeOptions options) => AddCmd(Request.SortedSetRange(key, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeWithScores(ValkeyKey, RangeOptions)" />
    public T SortedSetRangeWithScores(ValkeyKey key) => SortedSetRangeWithScores(key, new RangeOptions());

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeWithScores(ValkeyKey, RangeOptions)" />
    public T SortedSetRangeWithScores(ValkeyKey key, RangeOptions options) => AddCmd(Request.SortedSetRangeWithScores(key, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeAndStore(ValkeyKey, ValkeyKey, RangeOptions)" />
    public T SortedSetRangeAndStore(ValkeyKey source, ValkeyKey destination) => SortedSetRangeAndStore(source, destination, new RangeOptions());

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeAndStore(ValkeyKey, ValkeyKey, RangeOptions)" />
    public T SortedSetRangeAndStore(ValkeyKey source, ValkeyKey destination, RangeOptions options) => AddCmd(Request.SortedSetRangeAndStore(source, destination, options));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRemoveRange(ValkeyKey, Range)" />
    public T SortedSetRemoveRange(ValkeyKey key, Range range) => AddCmd(Request.SortedSetRemoveRange(key, range));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnion(IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetUnion(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetUnion([.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnion(IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetUnion(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetUnion(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnionWithScore(IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetUnionWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetUnionWithScore([.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnionWithScore(IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetUnionWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetUnionWithScore(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInter(IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetInter(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetInter([.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInter(IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetInter(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetInter(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterWithScore(IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetInterWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetInterWithScore([.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterWithScore(IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetInterWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetInterWithScore(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetDiff(IEnumerable{ValkeyKey})" />
    public T SortedSetDiff(IEnumerable<ValkeyKey> keys) => AddCmd(Request.SortedSetDiff([.. keys]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetDiffWithScore(IEnumerable{ValkeyKey})" />
    public T SortedSetDiffWithScore(IEnumerable<ValkeyKey> keys) => AddCmd(Request.SortedSetDiffWithScore([.. keys]));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnionAndStore(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetUnionAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetUnionAndStore(destination, [.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetUnionAndStore(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetUnionAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetUnionAndStore(destination, keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterAndStore(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" />
    public T SortedSetInterAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetInterAndStore(destination, [.. keys], aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetInterAndStore(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)" />
    public T SortedSetInterAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum) => AddCmd(Request.SortedSetInterAndStore(destination, keysAndWeights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetDiffAndStore(ValkeyKey, IEnumerable{ValkeyKey})" />
    public T SortedSetDiffAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(Request.SortedSetDiffAndStore(destination, [.. keys]));

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
    IBatch IBatchSortedSetCommands.SortedSetScan(ValkeyKey key, long cursor, ScanOptions? options) => SortedSetScan(key, cursor, options);
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
}
