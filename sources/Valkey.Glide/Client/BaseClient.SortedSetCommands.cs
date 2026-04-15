// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score)
        => Command(Request.SortedSetAddAsync(key, member, score));

    /// <inheritdoc/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, SortedSetEntry member)
        => SortedSetAddAsync(key, member.Element, member.Score);

    /// <inheritdoc/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> members)
        => Command(Request.SortedSetAddAsync(key, members));

    /// <inheritdoc/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddCondition condition)
        => SortedSetAddAsync(key, member, score, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options)
        => Command(Request.SortedSetAddAsync(key, member, score, options));

    /// <inheritdoc/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members)
        => SortedSetAddAsync(key, members.Select(kvp => new SortedSetEntry(kvp.Key, kvp.Value)));

    /// <inheritdoc/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddCondition condition)
        => SortedSetAddAsync(key, members, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options)
        => Command(Request.SortedSetAddAsync(key, members.Select(kvp => new SortedSetEntry(kvp.Key, kvp.Value)), options));

    /// <inheritdoc/>
    public Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member)
        => Command(Request.SortedSetRemoveAsync(key, member));

    /// <inheritdoc/>
    public Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => Command(Request.SortedSetRemoveAsync(key, members));

    /// <inheritdoc/>
    public Task<long> SortedSetCardAsync(ValkeyKey key)
        => Command(Request.SortedSetCardAsync(key));

    /// <inheritdoc/>
    public Task<long> SortedSetCountAsync(ValkeyKey key, ScoreRange range)
        => Command(Request.SortedSetCountAsync(key, range));

    /// <inheritdoc/>
    public Task<long> SortedSetLexCountAsync(ValkeyKey key, LexRange range)
        => Command(Request.SortedSetLexCountAsync(key, range));

    /// <inheritdoc/>
    public Task<double> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value)
        => Command(Request.SortedSetIncrementByAsync(key, member, value));

    /// <inheritdoc/>
    public Task<double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition)
        => SortedSetIncrementByAsync(key, member, value, new SortedSetAddOptions { Condition = condition });

    /// <inheritdoc/>
    public Task<double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options)
        => Command(Request.SortedSetIncrementByAsync(key, member, value, options));

    /// <inheritdoc/>
    public Task<long> SortedSetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
        => Command(Request.SortedSetInterCardAsync(keys, limit));

    /// <inheritdoc/>
    public Task<SortedSetEntry?> SortedSetPopMinAsync(ValkeyKey key)
        => Command(Request.SortedSetPopMinAsync(key));

    /// <inheritdoc/>
    public Task<SortedSetEntry?> SortedSetPopMaxAsync(ValkeyKey key)
        => Command(Request.SortedSetPopMaxAsync(key));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetPopMinAsync(ValkeyKey key, long count)
        => Command(Request.SortedSetPopMinAsync(key, count));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetPopMaxAsync(ValkeyKey key, long count)
        => Command(Request.SortedSetPopMaxAsync(key, count));

    /// <inheritdoc/>
    public Task<SortedSetEntry?> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null)
        => timeout.HasValue
            ? Command(Request.SortedSetPopMinAsync(keys, timeout.Value))
            : Command(Request.SortedSetPopMinAsync(keys));

    /// <inheritdoc/>
    public Task<SortedSetEntry?> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, TimeSpan? timeout = null)
        => timeout.HasValue
            ? Command(Request.SortedSetPopMaxAsync(keys, timeout.Value))
            : Command(Request.SortedSetPopMaxAsync(keys));

    /// <inheritdoc/>
    public Task<SortedSetPopResult> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null)
        => timeout.HasValue
            ? Command(Request.SortedSetPopMinAsync(keys, count, timeout.Value))
            : Command(Request.SortedSetPopMinAsync(keys, count));

    /// <inheritdoc/>
    public Task<SortedSetPopResult> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan? timeout = null)
        => timeout.HasValue
            ? Command(Request.SortedSetPopMaxAsync(keys, count, timeout.Value))
            : Command(Request.SortedSetPopMaxAsync(keys, count));

    /// <inheritdoc/>
    public Task<SortedSetEntry?> SortedSetRandomMemberWithScoreAsync(ValkeyKey key)
        => Command(Request.SortedSetRandomMemberWithScoreAsync(key));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count)
        => Command(Request.SortedSetRandomMembersWithScoreAsync(key, count));

    /// <inheritdoc/>
    public Task<ValkeyValue[]> SortedSetUnionAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionAsync(keys, aggregate));

    /// <inheritdoc/>
    public Task<ValkeyValue[]> SortedSetUnionAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionAsync(keysAndWeights, aggregate));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetUnionWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionWithScoreAsync(keys, aggregate));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetUnionWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionWithScoreAsync(keysAndWeights, aggregate));

    /// <inheritdoc/>
    public Task<ValkeyValue[]> SortedSetInterAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterAsync(keys, aggregate));

    /// <inheritdoc/>
    public Task<ValkeyValue[]> SortedSetInterAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterAsync(keysAndWeights, aggregate));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetInterWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterWithScoreAsync(keys, aggregate));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetInterWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterWithScoreAsync(keysAndWeights, aggregate));

    /// <inheritdoc/>
    public Task<ValkeyValue[]> SortedSetDiffAsync(IEnumerable<ValkeyKey> keys)
        => Command(Request.SortedSetDiffAsync(keys));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetDiffWithScoreAsync(IEnumerable<ValkeyKey> keys)
        => Command(Request.SortedSetDiffWithScoreAsync(keys));

    /// <inheritdoc/>
    public Task<long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionAndStoreAsync(destination, keys, aggregate));

    /// <inheritdoc/>
    public Task<long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetUnionAndStoreAsync(destination, keysAndWeights, aggregate));

    /// <inheritdoc/>
    public Task<long> SortedSetInterAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterAndStoreAsync(destination, keys, aggregate));

    /// <inheritdoc/>
    public Task<long> SortedSetInterAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Command(Request.SortedSetInterAndStoreAsync(destination, keysAndWeights, aggregate));

    /// <inheritdoc/>
    public Task<long> SortedSetDiffAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Command(Request.SortedSetDiffAndStoreAsync(destination, keys));

    /// <inheritdoc/>
    public Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
        => Command(Request.SortedSetRankAsync(key, member, order));

    /// <inheritdoc/>
    public Task<(long Rank, double Score)?> SortedSetRankWithScoreAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
        => Command(Request.SortedSetRankWithScoreAsync(key, member, order));

    /// <inheritdoc/>
    public Task<ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key, RangeOptions options = default)
        => Command(Request.SortedSetRangeAsync(key, options));

    /// <inheritdoc/>
    public Task<SortedSetEntry[]> SortedSetRangeWithScoresAsync(ValkeyKey key, RangeOptions options = default)
        => Command(Request.SortedSetRangeWithScoresAsync(key, options));

    /// <inheritdoc/>
    public Task<long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination, RangeOptions options = default)
        => Command(Request.SortedSetRangeAndStoreAsync(source, destination, options));

    /// <inheritdoc/>
    public Task<long> SortedSetRemoveRangeAsync(ValkeyKey key, Range range)
        => Command(Request.SortedSetRemoveRangeAsync(key, range));

    /// <inheritdoc/>
    public Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member)
        => Command(Request.SortedSetScoreAsync(key, member));

    /// <inheritdoc/>
    public Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => Command(Request.SortedSetScoresAsync(key, members));

    /// <inheritdoc/>
    public Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key)
        => Command(Request.SortedSetRandomMemberAsync(key));

    /// <inheritdoc/>
    public Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count)
        => Command(Request.SortedSetRandomMembersAsync(key, count));

    /// <inheritdoc/>
    public async Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, TimeSpan timeout)
        => order == Order.Ascending
            ? await SortedSetPopMinAsync([key], timeout)
            : await SortedSetPopMaxAsync([key], timeout);

    /// <inheritdoc/>
    public async Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, TimeSpan timeout)
        => order == Order.Ascending
            ? await SortedSetPopMinAsync(keys, count, timeout)
            : await SortedSetPopMaxAsync(keys, count, timeout);

    // TODO #287
    /// <inheritdoc/>
    public async IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0)
    {
        long currentCursor = cursor;
        int currentOffset = pageOffset;

        do
        {
            (long nextCursor, SortedSetEntry[] elements) = await Command(Request.SortedSetScanAsync(key, pattern, pageSize, currentCursor));

            IEnumerable<SortedSetEntry> elementsToYield = currentOffset > 0 ? elements.Skip(currentOffset) : elements;

            foreach (SortedSetEntry element in elementsToYield)
            {
                yield return element;
            }

            currentCursor = nextCursor;
            currentOffset = 0;
        } while (currentCursor != 0);
    }
}
