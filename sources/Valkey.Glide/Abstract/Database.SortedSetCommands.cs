// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Sorted set commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="ISortedSetCommands" />
internal partial class Database
{
    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score, when);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score, when);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values, when);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, values, when);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveAsync(key, member);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveAsync(key, members);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetLengthAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetLengthAsync(ValkeyKey key, double min, double max, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetLengthAsync(key, min, max, exclude);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCardAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetCardAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCardAsync(key);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCountAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetCountAsync(ValkeyKey key, double min, double max, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCountAsync(key, min, max, exclude);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start, long stop, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByRankAsync(key, start, stop, order);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start, long stop, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByRankWithScoresAsync(key, start, stop, order);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByValueAsync(key, min, max, exclude, skip, take);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, Order order, long skip, long take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, Order, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(key, order, timeout);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, long, Order, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetEntry[]> SortedSetBlockingPopAsync(ValkeyKey key, long count, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(key, count, order, timeout);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(IEnumerable{ValkeyKey}, long, Order, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, double timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetBlockingPopAsync(keys, count, order, timeout);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAsync(operation, keys, weights, aggregate);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineWithScoresAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineWithScoresAsync(operation, keys, weights, aggregate);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetIncrementAsync(key, member, value);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetIntersectionLengthAsync(keys, limit);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetLengthByValueAsync(key, min, max, exclude);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(ValkeyKey, Order)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(key, order);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(ValkeyKey, long, Order)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(key, count, order);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(IEnumerable{ValkeyKey}, long, Order)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetPopAsync(keys, count, order);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMemberAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMemberAsync(key);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMembersAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMembersAsync(key, count);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRandomMembersWithScoresAsync(key, count);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetRangeAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder, Exclude exclude, Order order, long skip, long? take, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeAndStoreAsync(sourceKey, destinationKey, start, stop, sortedSetOrder, exclude, order, skip, take);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRankAsync(key, member, order);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByValueAsync(key, min, max, exclude);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByRankAsync(key, start, stop);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await foreach (SortedSetEntry entry in SortedSetScanAsync(key, pattern, pageSize, cursor, pageOffset))
        {
            yield return entry;
        }
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetScoreAsync(key, member);
    }

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetScoresAsync(key, members);
    }
}
