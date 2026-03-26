// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Sorted set commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="ISortedSetCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, When)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, When)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, When when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetLengthAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetLengthAsync(ValkeyKey key, double min, double max, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCardAsync(ValkeyKey)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCardAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCountAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCountAsync(ValkeyKey key, double min, double max, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start, long stop, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start, long stop, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start, double stop, Exclude exclude, Order order, long skip, long take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, Order order, long skip, long take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, Order, double)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, double timeout, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, long, Order, double)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetBlockingPopAsync(ValkeyKey key, long count, Order order, double timeout, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetBlockingPopAsync(IEnumerable{ValkeyKey}, long, Order, double)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, double timeout, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineWithScoresAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights, Aggregate aggregate, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(ValkeyKey, Order)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(ValkeyKey, long, Order)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetPopAsync(IEnumerable{ValkeyKey}, long, Order)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMemberAsync(ValkeyKey)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMembersAsync(ValkeyKey, long)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRangeAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder, Exclude exclude, Order order, long skip, long? take, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);
}
