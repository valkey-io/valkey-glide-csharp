// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    #region Public Methods

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double)"/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score)
        => Command(Request.SortedSetAdd(key, member, score));

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, SortedSetEntry)"/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, SortedSetEntry member)
        => SortedSetAddAsync(key, member.Element, member.Score);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry})"/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> members)
        => Command(Request.SortedSetAdd(key, members));

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)"/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddCondition condition)
        => SortedSetAddAsync(key, member, score, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)"/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options)
        => Command(Request.SortedSetAdd(key, member, score, options));

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double})"/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members)
        => SortedSetAddAsync(key, members.Select(kvp => new SortedSetEntry(kvp.Key, kvp.Value)));

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddCondition)"/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddCondition condition)
        => SortedSetAddAsync(key, members, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBaseClient.SortedSetAddAsync(ValkeyKey, IDictionary{ValkeyValue, double}, SortedSetAddOptions)"/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options)
        => Command(Request.SortedSetAdd(key, members.Select(kvp => new SortedSetEntry(kvp.Key, kvp.Value)), options));

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    public Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member)
        => Command(Request.SortedSetRemove(key, member));

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => Command(Request.SortedSetRemove(key, members));

    /// <inheritdoc cref="IBaseClient.SortedSetCardAsync(ValkeyKey)"/>
    public Task<long> SortedSetCardAsync(ValkeyKey key)
        => Command(Request.SortedSetCard(key));

    /// <inheritdoc cref="IBaseClient.SortedSetCountAsync(ValkeyKey, ScoreRange)"/>
    public Task<long> SortedSetCountAsync(ValkeyKey key, ScoreRange range)
        => Command(Request.SortedSetCount(key, range));

    /// <inheritdoc cref="IBaseClient.SortedSetLexCountAsync(ValkeyKey, LexRange)"/>
    public Task<long> SortedSetLexCountAsync(ValkeyKey key, LexRange range)
        => Command(Request.SortedSetLexCount(key, range));

    /// <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double)"/>
    public Task<double> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value)
        => Command(Request.SortedSetIncrementBy(key, member, value));

    /// <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double, SortedSetAddCondition)"/>
    public Task<double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition)
        => SortedSetIncrementByAsync(key, member, value, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc cref="IBaseClient.SortedSetIncrementByAsync(ValkeyKey, ValkeyValue, double, SortedSetAddOptions)"/>
    public Task<double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options)
        => Command(Request.SortedSetIncrementBy(key, member, value, options));

    /// <inheritdoc cref="IBaseClient.SortedSetInterCardAsync(IEnumerable{ValkeyKey}, long)"/>
    public Task<long> SortedSetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
        => Command(Request.SortedSetInterCard(keys, limit));

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(ValkeyKey)"/>
    public Task<SortedSetEntry?> SortedSetPopMinAsync(ValkeyKey key)
        => Command(Request.SortedSetPopMin(key));

    /// <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(ValkeyKey)"/>
    public Task<SortedSetEntry?> SortedSetPopMaxAsync(ValkeyKey key)
        => Command(Request.SortedSetPopMax(key));

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(ValkeyKey, long)"/>
    public Task<SortedSetEntry[]> SortedSetPopMinAsync(ValkeyKey key, long count)
        => Command(Request.SortedSetPopMin(key, count));

    /// <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(ValkeyKey, long)"/>
    public Task<SortedSetEntry[]> SortedSetPopMaxAsync(ValkeyKey key, long count)
        => Command(Request.SortedSetPopMax(key, count));

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(IEnumerable{ValkeyKey}, TimeSpan?)"/>
    public Task<SortedSetEntry?> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null)
        => timeout.HasValue
            ? Command(Request.SortedSetPopMin(keys, timeout.Value))
            : Command(Request.SortedSetPopMin(keys));

    /// <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(IEnumerable{ValkeyKey}, TimeSpan?)"/>
    public Task<SortedSetEntry?> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null)
        => timeout.HasValue
            ? Command(Request.SortedSetPopMax(keys, timeout.Value))
            : Command(Request.SortedSetPopMax(keys));

    /// <inheritdoc cref="IBaseClient.SortedSetPopMinAsync(IEnumerable{ValkeyKey}, long, TimeSpan?)"/>
    public Task<SortedSetPopResult> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null)
        => timeout.HasValue
            ? Command(Request.SortedSetPopMin(keys, count, timeout.Value))
            : Command(Request.SortedSetPopMin(keys, count));

    /// <inheritdoc cref="IBaseClient.SortedSetPopMaxAsync(IEnumerable{ValkeyKey}, long, TimeSpan?)"/>
    public Task<SortedSetPopResult> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null)
        => timeout.HasValue
            ? Command(Request.SortedSetPopMax(keys, count, timeout.Value))
            : Command(Request.SortedSetPopMax(keys, count));

    /// <inheritdoc cref="IBaseClient.SortedSetRandomMemberWithScoreAsync(ValkeyKey)"/>
    public Task<SortedSetEntry?> SortedSetRandomMemberWithScoreAsync(ValkeyKey key)
        => Command(Request.SortedSetRandomMemberWithScore(key));

    /// <inheritdoc cref="IBaseClient.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)"/>
    public Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count)
        => Command(Request.SortedSetRandomMembersWithScore(key, count));

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAsync(IEnumerable{ValkeyKey}, Aggregate)"/>
    public Task<ValkeyValue[]> SortedSetUnionAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnion(keys, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAsync(IDictionary{ValkeyKey, double}, Aggregate)"/>
    public Task<ValkeyValue[]> SortedSetUnionAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnion(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetUnionWithScoreAsync(IEnumerable{ValkeyKey}, Aggregate)"/>
    public Task<SortedSetEntry[]> SortedSetUnionWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionWithScore(keys, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetUnionWithScoreAsync(IDictionary{ValkeyKey, double}, Aggregate)"/>
    public Task<SortedSetEntry[]> SortedSetUnionWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionWithScore(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetInterAsync(IEnumerable{ValkeyKey}, Aggregate)"/>
    public Task<ValkeyValue[]> SortedSetInterAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInter(keys, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetInterAsync(IDictionary{ValkeyKey, double}, Aggregate)"/>
    public Task<ValkeyValue[]> SortedSetInterAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInter(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetInterWithScoreAsync(IEnumerable{ValkeyKey}, Aggregate)"/>
    public Task<SortedSetEntry[]> SortedSetInterWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterWithScore(keys, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetInterWithScoreAsync(IDictionary{ValkeyKey, double}, Aggregate)"/>
    public Task<SortedSetEntry[]> SortedSetInterWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterWithScore(keysAndWeights, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetDiffAsync(IEnumerable{ValkeyKey})"/>
    public Task<ValkeyValue[]> SortedSetDiffAsync(IEnumerable<ValkeyKey> keys)
        => Command(Request.SortedSetDiff(keys));

    /// <inheritdoc cref="IBaseClient.SortedSetDiffWithScoreAsync(IEnumerable{ValkeyKey})"/>
    public Task<SortedSetEntry[]> SortedSetDiffWithScoreAsync(IEnumerable<ValkeyKey> keys)
        => Command(Request.SortedSetDiffWithScore(keys));

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)"/>
    public Task<long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionAndStore(destination, keys, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetUnionAndStoreAsync(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)"/>
    public Task<long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionAndStore(destination, keysAndWeights, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetInterAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, Aggregate)"/>
    public Task<long> SortedSetInterAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterAndStore(destination, keys, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetInterAndStoreAsync(ValkeyKey, IDictionary{ValkeyKey, double}, Aggregate)"/>
    public Task<long> SortedSetInterAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterAndStore(destination, keysAndWeights, aggregate));

    /// <inheritdoc cref="IBaseClient.SortedSetDiffAndStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    public Task<long> SortedSetDiffAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Command(Request.SortedSetDiffAndStore(destination, keys));

    /// <inheritdoc cref="IBaseClient.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)"/>
    public Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
        => Command(Request.SortedSetRank(key, member, order));

    /// <inheritdoc cref="IBaseClient.SortedSetRankWithScoreAsync(ValkeyKey, ValkeyValue, Order)"/>
    public Task<(long Rank, double Score)?> SortedSetRankWithScoreAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
        => Command(Request.SortedSetRankWithScore(key, member, order));

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAsync(ValkeyKey)"/>
    public Task<ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key)
        => SortedSetRangeAsync(key, new RangeOptions());

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAsync(ValkeyKey, RangeOptions)"/>
    public Task<ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key, RangeOptions options)
        => Command(Request.SortedSetRange(key, options));

    /// <inheritdoc cref="IBaseClient.SortedSetRangeWithScoresAsync(ValkeyKey)"/>
    public Task<SortedSetEntry[]> SortedSetRangeWithScoresAsync(ValkeyKey key)
        => SortedSetRangeWithScoresAsync(key, new RangeOptions());

    /// <inheritdoc cref="IBaseClient.SortedSetRangeWithScoresAsync(ValkeyKey, RangeOptions)"/>
    public Task<SortedSetEntry[]> SortedSetRangeWithScoresAsync(ValkeyKey key, RangeOptions options)
        => Command(Request.SortedSetRangeWithScores(key, options));

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey)"/>
    public Task<long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination)
        => SortedSetRangeAndStoreAsync(source, destination, new RangeOptions());

    /// <inheritdoc cref="IBaseClient.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, RangeOptions)"/>
    public Task<long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination, RangeOptions options)
        => Command(Request.SortedSetRangeAndStore(source, destination, options));

    /// <inheritdoc cref="IBaseClient.SortedSetRemoveRangeAsync(ValkeyKey, Range)"/>
    public Task<long> SortedSetRemoveRangeAsync(ValkeyKey key, Range range)
        => Command(Request.SortedSetRemoveRange(key, range));

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)"/>
    public Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member)
        => Command(Request.SortedSetScore(key, member));

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => Command(Request.SortedSetScores(key, members));

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMemberAsync(ValkeyKey)"/>
    public Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key)
        => Command(Request.SortedSetRandomMember(key));

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMembersAsync(ValkeyKey, long)"/>
    public Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count)
        => Command(Request.SortedSetRandomMembers(key, count));

    /// <inheritdoc cref="IBaseClient.SortedSetBlockingPopAsync(ValkeyKey, Order, TimeSpan)"/>
    public async Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, TimeSpan timeout)
        => order == Order.Ascending
            ? await SortedSetPopMinAsync([key], timeout)
            : await SortedSetPopMaxAsync([key], timeout);

    /// <inheritdoc cref="IBaseClient.SortedSetBlockingPopAsync(IEnumerable{ValkeyKey}, long, Order, TimeSpan)"/>
    public async Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, TimeSpan timeout)
        => order == Order.Ascending
            ? await SortedSetPopMinAsync(keys, count, timeout)
            : await SortedSetPopMaxAsync(keys, count, timeout);

    /// <inheritdoc cref="IBaseClient.SortedSetScanAsync(ValkeyKey, ScanOptions?)"/>
    public IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ScanOptions? options = null)
        => SortedSetScanAsync(key, 0, options);

    #endregion
    #region Protected Methods

    /// <inheritdoc cref="IBaseClient.SortedSetScanAsync(ValkeyKey, ScanOptions?)"/>
    private protected async IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(
        ValkeyKey key,
        long cursor,
        ScanOptions? options)
    {
        do
        {
            (cursor, var elements) = await Command(Request.SortedSetScan(key, cursor, options));
            foreach (var element in elements)
            {
                yield return element;
            }
        } while (cursor != 0);
    }

    #endregion
}
