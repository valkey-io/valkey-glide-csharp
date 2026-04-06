// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Sorted set commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="ISortedSetBaseCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, When when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetLengthAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetLengthAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetCardAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCardAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetCountAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCountAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min = default, ValkeyValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetCombineWithScoresAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetPopAsync(ValkeyKey, Order)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetPopAsync(ValkeyKey, long, Order)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetPopAsync(IEnumerable{ValkeyKey}, long, Order)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMemberAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMembersAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRangeAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long? take = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member, CommandFlags flags);

    /// <inheritdoc cref="ISortedSetBaseCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members, CommandFlags flags);
}
