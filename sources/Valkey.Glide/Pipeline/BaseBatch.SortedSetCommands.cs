// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Sorted Set commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, ValkeyValue, double, SortedSetWhen)" />
    public T SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when = SortedSetWhen.Always) => AddCmd(SortedSetAddAsync(key, member, score, when));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetAdd(ValkeyKey, SortedSetEntry[], SortedSetWhen)" />
    public T SortedSetAdd(ValkeyKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always) => AddCmd(SortedSetAddAsync(key, values, when));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRemove(ValkeyKey, ValkeyValue)" />
    public T SortedSetRemove(ValkeyKey key, ValkeyValue member) => AddCmd(SortedSetRemoveAsync(key, member));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRemove(ValkeyKey, ValkeyValue[])" />
    public T SortedSetRemove(ValkeyKey key, ValkeyValue[] members) => AddCmd(SortedSetRemoveAsync(key, members));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetLength(ValkeyKey, double, double, Exclude)" />
    public T SortedSetLength(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None)
    {
        // If both min and max are infinity (default values), use ZCARD
        if (double.IsNegativeInfinity(min) && double.IsPositiveInfinity(max))
        {
            return SortedSetCard(key);
        }

        // Otherwise use ZCOUNT with the specified range
        return SortedSetCount(key, min, max, exclude);
    }

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCard(ValkeyKey)" />
    public T SortedSetCard(ValkeyKey key) => AddCmd(SortedSetCardAsync(key));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCount(ValkeyKey, double, double, Exclude)" />
    public T SortedSetCount(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None) => AddCmd(SortedSetCountAsync(key, min, max, exclude));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeByRank(ValkeyKey, long, long, Order)" />
    public T SortedSetRangeByRank(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending) => AddCmd(SortedSetRangeByRankAsync(key, start, stop, order));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeByRankWithScores(ValkeyKey, long, long, Order)" />
    public T SortedSetRangeByRankWithScores(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending) => AddCmd(SortedSetRangeByRankWithScoresAsync(key, start, stop, order));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeByScore(ValkeyKey, double, double, Exclude, Order, long, long)" />
    public T SortedSetRangeByScore(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) => AddCmd(SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeByScoreWithScores(ValkeyKey, double, double, Exclude, Order, long, long)" />
    public T SortedSetRangeByScoreWithScores(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) => AddCmd(SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeByValue(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long)" />
    public T SortedSetRangeByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, long skip = 0, long take = -1) => AddCmd(SortedSetRangeByValueAsync(key, min, max, exclude, skip, take));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetRangeByValue(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long)" />
    public T SortedSetRangeByValue(ValkeyKey key, ValkeyValue min = default, ValkeyValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) => AddCmd(SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCombine(SetOperation, ValkeyKey[], double[], Aggregate)" />
    public T SortedSetCombine(SetOperation operation, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetCombineAsync(operation, keys, weights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCombineWithScores(SetOperation, ValkeyKey[], double[], Aggregate)" />
    public T SortedSetCombineWithScores(SetOperation operation, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetCombineWithScoresAsync(operation, keys, weights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCombineAndStore(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate)" />
    public T SortedSetCombineAndStore(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetCombineAndStore(SetOperation, ValkeyKey, ValkeyKey[], double[], Aggregate)" />
    public T SortedSetCombineAndStore(SetOperation operation, ValkeyKey destination, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum) => AddCmd(SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetIncrement(ValkeyKey, ValkeyValue, double)" />
    public T SortedSetIncrement(ValkeyKey key, ValkeyValue member, double value) => AddCmd(SortedSetIncrementAsync(key, member, value));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetIntersectionLength(ValkeyKey[], long)" />
    public T SortedSetIntersectionLength(ValkeyKey[] keys, long limit = 0) => AddCmd(SortedSetIntersectionLengthAsync(keys, limit));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetLengthByValue(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)" />
    public T SortedSetLengthByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None) => AddCmd(SortedSetLengthByValueAsync(key, min, max, exclude));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetPop(ValkeyKey[], long, Order)" />
    public T SortedSetPop(ValkeyKey[] keys, long count, Order order = Order.Ascending) => AddCmd(SortedSetPopAsync(keys, count, order));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetScores(ValkeyKey, ValkeyValue[])" />
    public T SortedSetScores(ValkeyKey key, ValkeyValue[] members) => AddCmd(SortedSetScoresAsync(key, members));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetBlockingPop(ValkeyKey, Order, double)" />
    public T SortedSetBlockingPop(ValkeyKey key, Order order, double timeout) => AddCmd(SortedSetBlockingPopAsync(key, order, timeout));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetBlockingPop(ValkeyKey, long, Order, double)" />
    public T SortedSetBlockingPop(ValkeyKey key, long count, Order order, double timeout) => AddCmd(SortedSetBlockingPopAsync(key, count, order, timeout));

    /// <inheritdoc cref="IBatchSortedSetCommands.SortedSetBlockingPop(ValkeyKey[], long, Order, double)" />
    public T SortedSetBlockingPop(ValkeyKey[] keys, long count, Order order, double timeout) => AddCmd(SortedSetBlockingPopAsync(keys, count, order, timeout));

    // Explicit interface implementations for IBatchSortedSetCommands
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when) => SortedSetAdd(key, member, score, when);
    IBatch IBatchSortedSetCommands.SortedSetAdd(ValkeyKey key, SortedSetEntry[] values, SortedSetWhen when) => SortedSetAdd(key, values, when);
    IBatch IBatchSortedSetCommands.SortedSetRemove(ValkeyKey key, ValkeyValue member) => SortedSetRemove(key, member);
    IBatch IBatchSortedSetCommands.SortedSetRemove(ValkeyKey key, ValkeyValue[] members) => SortedSetRemove(key, members);
    IBatch IBatchSortedSetCommands.SortedSetLength(ValkeyKey key, double min, double max, Exclude exclude) => SortedSetLength(key, min, max, exclude);
    IBatch IBatchSortedSetCommands.SortedSetCard(ValkeyKey key) => SortedSetCard(key);
    IBatch IBatchSortedSetCommands.SortedSetCount(ValkeyKey key, double min, double max, Exclude exclude) => SortedSetCount(key, min, max, exclude);
    IBatch IBatchSortedSetCommands.SortedSetRangeByRank(ValkeyKey key, long start, long stop, Order order) => SortedSetRangeByRank(key, start, stop, order);
    IBatch IBatchSortedSetCommands.SortedSetRangeByRankWithScores(ValkeyKey key, long start, long stop, Order order) => SortedSetRangeByRankWithScores(key, start, stop, order);
    IBatch IBatchSortedSetCommands.SortedSetRangeByScore(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take) => SortedSetRangeByScore(key, start, stop, exclude, order, skip, take);
    IBatch IBatchSortedSetCommands.SortedSetRangeByScoreWithScores(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take) => SortedSetRangeByScoreWithScores(key, start, stop, exclude, order, skip, take);
    IBatch IBatchSortedSetCommands.SortedSetRangeByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take) => SortedSetRangeByValue(key, min, max, exclude, skip, take);
    IBatch IBatchSortedSetCommands.SortedSetRangeByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, Order order, long skip, long take) => SortedSetRangeByValue(key, min, max, exclude, order, skip, take);
    IBatch IBatchSortedSetCommands.SortedSetCombine(SetOperation operation, ValkeyKey[] keys, double[]? weights, Aggregate aggregate) => SortedSetCombine(operation, keys, weights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetCombineWithScores(SetOperation operation, ValkeyKey[] keys, double[]? weights, Aggregate aggregate) => SortedSetCombineWithScores(operation, keys, weights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetCombineAndStore(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate) => SortedSetCombineAndStore(operation, destination, first, second, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetCombineAndStore(SetOperation operation, ValkeyKey destination, ValkeyKey[] keys, double[]? weights, Aggregate aggregate) => SortedSetCombineAndStore(operation, destination, keys, weights, aggregate);
    IBatch IBatchSortedSetCommands.SortedSetIncrement(ValkeyKey key, ValkeyValue member, double value) => SortedSetIncrement(key, member, value);
    IBatch IBatchSortedSetCommands.SortedSetIntersectionLength(ValkeyKey[] keys, long limit) => SortedSetIntersectionLength(keys, limit);
    IBatch IBatchSortedSetCommands.SortedSetLengthByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude) => SortedSetLengthByValue(key, min, max, exclude);
    IBatch IBatchSortedSetCommands.SortedSetPop(ValkeyKey[] keys, long count, Order order) => SortedSetPop(keys, count, order);
    IBatch IBatchSortedSetCommands.SortedSetScores(ValkeyKey key, ValkeyValue[] members) => SortedSetScores(key, members);
    IBatch IBatchSortedSetCommands.SortedSetBlockingPop(ValkeyKey key, Order order, double timeout) => SortedSetBlockingPop(key, order, timeout);
    IBatch IBatchSortedSetCommands.SortedSetBlockingPop(ValkeyKey key, long count, Order order, double timeout) => SortedSetBlockingPop(key, count, order, timeout);
    IBatch IBatchSortedSetCommands.SortedSetBlockingPop(ValkeyKey[] keys, long count, Order order, double timeout) => SortedSetBlockingPop(keys, count, order, timeout);
}
