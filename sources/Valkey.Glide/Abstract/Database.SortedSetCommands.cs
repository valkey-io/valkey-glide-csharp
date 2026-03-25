// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Sorted Set Commands with CommandFlags (SER Compatibility)

    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score);
    }

    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score, when);
    }

    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score, when);
    }

    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values);
    }

    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values, when);
    }

    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values, when);
    }

    public async Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveAsync(key, member);
    }

    public async Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveAsync(key, members);
    }

    public async Task<long> SortedSetLengthAsync(ValkeyKey key, double min, double max, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetLengthAsync(key, min, max, exclude);
    }

    public async Task<long> SortedSetCardAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCardAsync(key);
    }

    public async Task<long> SortedSetCountAsync(ValkeyKey key, double min, double max, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCountAsync(key, min, max, exclude);
    }

    public async Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start, long stop, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByRankAsync(key, start, stop, order);
    }

    public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start, long stop, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByRankWithScoresAsync(key, start, stop, order);
    }

    public async Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take);
    }

    public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take);
    }

    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByValueAsync(key, min, max, exclude, skip, take);
    }

    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take);
    }

    public async Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(key, order, timeout);
    }

    public async Task<SortedSetEntry[]> SortedSetBlockingPopAsync(ValkeyKey key, long count, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(key, count, order, timeout);
    }

    public async Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(keys, count, order, timeout);
    }

    public async Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAsync(operation, keys, weights, aggregate);
    }

    public async Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineWithScoresAsync(operation, keys, weights, aggregate);
    }

    public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate);
    }

    public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate);
    }

    public async Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetIncrementAsync(key, member, value);
    }

    public async Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetIntersectionLengthAsync(keys, limit);
    }

    public async Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetLengthByValueAsync(key, min, max, exclude);
    }

    public async Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(key, order);
    }

    public async Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(key, count, order);
    }

    public async Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(keys, count, order);
    }

    public async Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMemberAsync(key);
    }

    public async Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMembersAsync(key, count);
    }

    public async Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMembersWithScoresAsync(key, count);
    }

    public async Task<long> SortedSetRangeAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder, Exclude exclude, Order order, long skip, long? take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeAndStoreAsync(sourceKey, destinationKey, start, stop, sortedSetOrder, exclude, order, skip, take);
    }

    public async Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRankAsync(key, member, order);
    }

    public async Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByValueAsync(key, min, max, exclude);
    }

    public async Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByRankAsync(key, start, stop);
    }

    public async Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude);
    }

    public async IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await foreach (SortedSetEntry entry in SortedSetScanAsync(key, pattern, pageSize, cursor, pageOffset))
        {
            yield return entry;
        }
    }

    public async Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetScoreAsync(key, member);
    }

    public async Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetScoresAsync(key, members);
    }

    #endregion
}
