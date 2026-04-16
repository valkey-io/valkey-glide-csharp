// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score)
        => await SortedSetAddAsync(key, member, score, SortedSetWhen.Always);

    /// <inheritdoc/>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when)
        => await SortedSetAddAsync(key, member, score, SortedSetWhenExtensions.Parse(when));

    /// <inheritdoc/>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when = SortedSetWhen.Always)
    {
        return await Command(Request.SortedSetAddAsync(key, member, score, when));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values)
        => await SortedSetAddAsync(key, values, SortedSetWhen.Always);

    /// <inheritdoc/>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, When when)
        => await SortedSetAddAsync(key, values, SortedSetWhenExtensions.Parse(when));

    /// <inheritdoc/>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when = SortedSetWhen.Always)
    {
        return await Command(Request.SortedSetAddAsync(key, [.. values], when));
    }

    /// <inheritdoc/>
    public async Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member)
    {
        return await Command(Request.SortedSetRemoveAsync(key, member));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
    {
        return await Command(Request.SortedSetRemoveAsync(key, [.. members]));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetLengthAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None)
    {
        // If both min and max are infinity (default values), use ZCARD
        if (double.IsNegativeInfinity(min) && double.IsPositiveInfinity(max))
        {
            return await SortedSetCardAsync(key);
        }

        // Otherwise use ZCOUNT with the specified range
        return await SortedSetCountAsync(key, min, max, exclude);
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetCardAsync(ValkeyKey key)
    {
        return await Command(Request.SortedSetCardAsync(key));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetCountAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None)
    {
        return await Command(Request.SortedSetCountAsync(key, min, max, exclude));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending)
    {
        return await Command(Request.SortedSetRangeByRankAsync(key, start, stop, order));
    }

    /// <inheritdoc/>
    public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending)
    {
        return await Command(Request.SortedSetRangeByRankWithScoresAsync(key, start, stop, order));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
    {
        return await Command(Request.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take));
    }

    /// <inheritdoc/>
    public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
    {
        return await Command(Request.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take)
        => await SortedSetRangeByValueAsync(key, min, max, exclude, Order.Ascending, skip, take);

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(
        ValkeyKey key,
        ValkeyValue min = default,
        ValkeyValue max = default,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0,
        long take = -1)
    {
        return await Command(Request.SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SortedSetCombineAsync(
        SetOperation operation,
        IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null,
        Aggregate aggregate = Aggregate.Sum)
    {
        return await Command(Request.SortedSetCombineAsync(operation, [.. keys], weights?.ToArray(), aggregate));
    }

    /// <inheritdoc/>
    public async Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(
        SetOperation operation,
        IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null,
        Aggregate aggregate = Aggregate.Sum)
    {
        return await Command(Request.SortedSetCombineWithScoresAsync(operation, [.. keys], weights?.ToArray(), aggregate));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum)
    {
        return await Command(Request.SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetCombineAndStoreAsync(
        SetOperation operation,
        ValkeyKey destination,
        IEnumerable<ValkeyKey> keys,
        IEnumerable<double>? weights = null,
        Aggregate aggregate = Aggregate.Sum)
    {
        return await Command(Request.SortedSetCombineAndStoreAsync(operation, destination, [.. keys], weights?.ToArray(), aggregate));
    }

    /// <inheritdoc/>
    public async Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value)
    {
        return await Command(Request.SortedSetIncrementAsync(key, member, value));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
    {
        return await Command(Request.SortedSetIntersectionLengthAsync([.. keys], limit));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None)
    {
        return await Command(Request.SortedSetLengthByValueAsync(key, min, max, exclude));
    }

    /// <inheritdoc/>
    public async Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order = Order.Ascending)
    {
        return await Command(Request.SortedSetPopAsync([.. keys], count, order));
    }

    /// <inheritdoc/>
    public async Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
    {
        return await Command(Request.SortedSetScoresAsync(key, [.. members]));
    }

    /// <inheritdoc/>
    public async Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, TimeSpan timeout)
    {
        return await Command(Request.SortedSetBlockingPopAsync(key, order, timeout));
    }

    /// <inheritdoc/>
    public async Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, TimeSpan timeout)
    {
        return await Command(Request.SortedSetBlockingPopAsync([.. keys], count, order, timeout));
    }

    /// <inheritdoc/>
    public async Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order = Order.Ascending)
    {
        return await Command(Request.SortedSetPopAsync(key, order));
    }

    /// <inheritdoc/>
    public async Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order = Order.Ascending)
    {
        Utils.Requires<ArgumentException>(count == 1, "GLIDE does not currently support multipop ZPOPMIN or ZPOPMAX"); // TODO for the future
        return await Command(Request.SortedSetPopAsync(key, count, order));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key)
    {
        return await Command(Request.SortedSetRandomMemberAsync(key));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count)
    {
        return await Command(Request.SortedSetRandomMembersAsync(key, count));
    }

    /// <inheritdoc/>
    public async Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count)
    {
        return await Command(Request.SortedSetRandomMembersWithScoresAsync(key, count));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetRangeAndStoreAsync(
        ValkeyKey sourceKey,
        ValkeyKey destinationKey,
        ValkeyValue start,
        ValkeyValue stop,
        SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0,
        long? take = null)
    {
        return await Command(Request.SortedSetRangeAndStoreAsync(sourceKey, destinationKey, start, stop, sortedSetOrder, exclude, order, skip, take));
    }

    /// <inheritdoc/>
    public async Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
    {
        return await Command(Request.SortedSetRankAsync(key, member, order));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None)
    {
        return await Command(Request.SortedSetRemoveRangeByValueAsync(key, min, max, exclude));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop)
    {
        return await Command(Request.SortedSetRemoveRangeByRankAsync(key, start, stop));
    }

    /// <inheritdoc/>
    public async Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double min, double max, Exclude exclude = Exclude.None)
    {
        return await Command(Request.SortedSetRemoveRangeByScoreAsync(key, min, max, exclude));
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ScanOptions? options = null)
    {
        long currentCursor = 0;

        do
        {
            (long nextCursor, SortedSetEntry[] elements) = await Command(Request.SortedSetScanAsync(key, currentCursor, options));

            foreach (SortedSetEntry element in elements)
            {
                yield return element;
            }

            currentCursor = nextCursor;
        } while (currentCursor != 0);
    }

    /// <inheritdoc/>
    public async Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member)
    {
        return await Command(Request.SortedSetScoreAsync(key, member));
    }
}
