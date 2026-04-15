// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Sorted Set Commands" group for batch operations.
/// <br/>
/// See more on <see href="https://valkey.io/commands/?group=sorted-set#sorted-set">valkey.io</see>.
/// </summary>
internal interface IBatchSortedSetCommands
{
    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" /></returns>
    IBatch SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddCondition condition = SortedSetAddCondition.Always);

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)" /></returns>
    IBatch SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options);

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddCondition)" /></returns>
    IBatch SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddCondition condition = SortedSetAddCondition.Always);

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddOptions)" /></returns>
    IBatch SortedSetAdd(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SortedSetRemove(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SortedSetRemove(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <inheritdoc cref="IBaseClient.SortedSetCardAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetCardAsync(ValkeyKey)" /></returns>
    IBatch SortedSetCard(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.SortedSetCountAsync(ValkeyKey, ScoreRange)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetCountAsync(ValkeyKey, ScoreRange)" /></returns>
    IBatch SortedSetCount(ValkeyKey key, ScoreRange range);

    /// <inheritdoc cref="IBaseClient.SortedSetLexCountAsync(ValkeyKey, LexRange)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetLexCountAsync(ValkeyKey, LexRange)" /></returns>
    IBatch SortedSetLexCount(ValkeyKey key, LexRange range);

    /// <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double)" /></returns>
    IBatch SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value);

    /// <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)" /></returns>
    IBatch SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition);

    /// <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)" /></returns>
    IBatch SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options);

    /// <inheritdoc cref="IBaseClient.SortedSetInterCardAsync(IEnumerable{ValkeyKey}, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetInterCardAsync(IEnumerable{ValkeyKey}, long)" /></returns>
    IBatch SortedSetInterCard(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(ValkeyKey)" /></returns>
    IBatch SortedSetPopMin(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(ValkeyKey)" /></returns>
    IBatch SortedSetPopMax(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(ValkeyKey, long)" /></returns>
    IBatch SortedSetPopMin(ValkeyKey key, long count);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(ValkeyKey, long)" /></returns>
    IBatch SortedSetPopMax(ValkeyKey key, long count);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(IEnumerable{ValkeyKey}, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(IEnumerable{ValkeyKey}, TimeSpan?)" /></returns>
    IBatch SortedSetPopMin(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(IEnumerable{ValkeyKey}, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(IEnumerable{ValkeyKey}, TimeSpan?)" /></returns>
    IBatch SortedSetPopMax(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(IEnumerable{ValkeyKey}, long, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(IEnumerable{ValkeyKey}, long, TimeSpan?)" /></returns>
    IBatch SortedSetPopMin(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null);

    /// <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(IEnumerable{ValkeyKey}, long, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(IEnumerable{ValkeyKey}, long, TimeSpan?)" /></returns>
    IBatch SortedSetPopMax(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMemberAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMemberAsync(ValkeyKey)" /></returns>
    IBatch SortedSetRandomMember(ValkeyKey key);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMembersAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMembersAsync(ValkeyKey, long)" /></returns>
    IBatch SortedSetRandomMembers(ValkeyKey key, long count);

    /// <inheritdoc cref="IBaseClient.SortedSetRandomMemberWithScoreAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetRandomMemberWithScoreAsync(ValkeyKey)" /></returns>
    IBatch SortedSetRandomMemberWithScore(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)" /></returns>
    IBatch SortedSetRandomMembersWithScores(ValkeyKey key, long count);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SortedSetScore(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SortedSetScores(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <inheritdoc cref="IBaseClient.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)" /></returns>
    IBatch SortedSetRank(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending);

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAsync(ValkeyKey, RangeOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetRangeAsync(ValkeyKey, RangeOptions)" /></returns>
    IBatch SortedSetRange(ValkeyKey key, RangeOptions options);

    /// <inheritdoc cref="IBaseClient.SortedSetRangeWithScoresAsync(ValkeyKey, RangeOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetRangeWithScoresAsync(ValkeyKey, RangeOptions)" /></returns>
    IBatch SortedSetRangeWithScores(ValkeyKey key, RangeOptions options = default);

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, RangeOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, RangeOptions)" /></returns>
    IBatch SortedSetRangeAndStore(ValkeyKey source, ValkeyKey destination, RangeOptions options = default);

    /// <inheritdoc cref="IBaseClient.SortedSetRemoveRangeAsync(ValkeyKey, Range)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetRemoveRangeAsync(ValkeyKey, Range)" /></returns>
    IBatch SortedSetRemoveRange(ValkeyKey key, Range range);

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetUnionAsync(IEnumerable{ValkeyKey}, Aggregate)" /></returns>
    IBatch SortedSetUnion(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAsync(IDictionary{ValkeyKey, double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetUnionAsync(IDictionary{ValkeyKey, double}, Aggregate)" /></returns>
    IBatch SortedSetUnion(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetUnionWithScoreAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetUnionWithScoreAsync(IEnumerable{ValkeyKey}, Aggregate)" /></returns>
    IBatch SortedSetUnionWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetUnionWithScoreAsync(IDictionary{ValkeyKey, double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetUnionWithScoreAsync(IDictionary{ValkeyKey, double}, Aggregate)" /></returns>
    IBatch SortedSetUnionWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetInterAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetInterAsync(IEnumerable{ValkeyKey}, Aggregate)" /></returns>
    IBatch SortedSetInter(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetInterAsync(IDictionary{ValkeyKey, double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetInterAsync(IDictionary{ValkeyKey, double}, Aggregate)" /></returns>
    IBatch SortedSetInter(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetInterWithScoreAsync(IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetInterWithScoreAsync(IEnumerable{ValkeyKey}, Aggregate)" /></returns>
    IBatch SortedSetInterWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetInterWithScoreAsync(IDictionary{ValkeyKey, double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetInterWithScoreAsync(IDictionary{ValkeyKey, double}, Aggregate)" /></returns>
    IBatch SortedSetInterWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetDiffAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetDiffAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SortedSetDiff(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SortedSetDiffWithScoreAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetDiffWithScoreAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SortedSetDiffWithScore(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetUnionAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" /></returns>
    IBatch SortedSetUnionAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAndStoreAsync(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetUnionAndStoreAsync(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)" /></returns>
    IBatch SortedSetUnionAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetInterAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetInterAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)" /></returns>
    IBatch SortedSetInterAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetInterAndStoreAsync(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetInterAndStoreAsync(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)" /></returns>
    IBatch SortedSetInterAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="IBaseClient.SortedSetDiffAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetDiffAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SortedSetDiffAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    // TODO #287
    /// <inheritdoc cref="IBaseClient.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)" /></returns>
    IBatch SortedSetScan(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0);
}
