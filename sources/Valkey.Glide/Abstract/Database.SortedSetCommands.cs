// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, CommandFlags)"/>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, When, CommandFlags)"/>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen, CommandFlags)"/>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, CommandFlags)"/>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, When, CommandFlags)"/>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen, CommandFlags)"/>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRemoveAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveAsync(key, member);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveAsync(key, members);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetLengthAsync(ValkeyKey, double, double, Exclude, CommandFlags)"/>
    public async Task<long> SortedSetLengthAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetLengthAsync(key, min, max, exclude);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCardAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> SortedSetCardAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCardAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCountAsync(ValkeyKey, double, double, Exclude, CommandFlags)"/>
    public async Task<long> SortedSetCountAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCountAsync(key, min, max, exclude);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByRankAsync(key, start, stop, order);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByRankWithScoresAsync(key, start, stop, order);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByValueAsync(key, min, max, exclude, skip, take);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min = default, ValkeyValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetBlockingPopAsync(ValkeyKey, Order, double, CommandFlags)"/>
    public async Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(key, order, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetBlockingPopAsync(ValkeyKey, long, Order, double, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetBlockingPopAsync(ValkeyKey key, long count, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(key, count, order, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetBlockingPopAsync(IEnumerable{ValkeyKey}, long, Order, double, CommandFlags)"/>
    public async Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(keys, count, order, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAsync(operation, keys, weights, aggregate);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCombineWithScoresAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineWithScoresAsync(operation, keys, weights, aggregate);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate, CommandFlags)"/>
    public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate, CommandFlags)"/>
    public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)"/>
    public async Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetIncrementAsync(key, member, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public async Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetIntersectionLengthAsync(keys, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, CommandFlags)"/>
    public async Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetLengthByValueAsync(key, min, max, exclude);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetPopAsync(ValkeyKey, Order, CommandFlags)"/>
    public async Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(key, order);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetPopAsync(ValkeyKey, long, Order, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(key, count, order);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetPopAsync(IEnumerable{ValkeyKey}, long, Order, CommandFlags)"/>
    public async Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(keys, count, order);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRandomMemberAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMemberAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRandomMembersAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMembersAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMembersWithScoresAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?, CommandFlags)"/>
    public async Task<long> SortedSetRangeAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long? take = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeAndStoreAsync(sourceKey, destinationKey, start, stop, sortedSetOrder, exclude, order, skip, take);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order, CommandFlags)"/>
    public async Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRankAsync(key, member, order);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, CommandFlags)"/>
    public async Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByValueAsync(key, min, max, exclude);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long, CommandFlags)"/>
    public async Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByRankAsync(key, start, stop);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude, CommandFlags)"/>
    public async Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)"/>
    public IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SortedSetScanAsync(key, pattern, pageSize, cursor, pageOffset);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetScoreAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetScoreAsync(key, member);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetScoresAsync(key, members);
    }
}
