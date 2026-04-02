// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Sorted Set Commands" group for batch operations.
/// <br/>
/// See more on <see href="https://valkey.io/commands/?group=sorted-set#sorted-set">valkey.io</see>.
/// </summary>
internal interface IBatchSortedSetCommands
{
    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen)" /></returns>
    IBatch SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when = SortedSetWhen.Always);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen)" /></returns>
    IBatch SortedSetAdd(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when = SortedSetWhen.Always);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SortedSetRemove(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SortedSetRemove(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetLengthAsync(ValkeyKey, double, double, Exclude)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetLengthAsync(ValkeyKey, double, double, Exclude)" /></returns>
    IBatch SortedSetLength(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCardAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCardAsync(ValkeyKey)" /></returns>
    IBatch SortedSetCard(ValkeyKey key);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCountAsync(ValkeyKey, double, double, Exclude)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCountAsync(ValkeyKey, double, double, Exclude)" /></returns>
    IBatch SortedSetCount(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order)" /></returns>
    IBatch SortedSetRangeByRank(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order)" /></returns>
    IBatch SortedSetRangeByRankWithScores(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long)" /></returns>
    IBatch SortedSetRangeByScore(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long)" /></returns>
    IBatch SortedSetRangeByScoreWithScores(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long)" /></returns>
    IBatch SortedSetRangeByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, long skip = 0, long take = -1);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long)" /></returns>
    IBatch SortedSetRangeByValue(ValkeyKey key, ValkeyValue min = default, ValkeyValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)" /></returns>
    IBatch SortedSetCombine(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineWithScoresAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineWithScoresAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)" /></returns>
    IBatch SortedSetCombineWithScores(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate)" /></returns>
    IBatch SortedSetCombineAndStore(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, IEnumerable{double}, Aggregate)" /></returns>
    IBatch SortedSetCombineAndStore(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double)" /></returns>
    IBatch SortedSetIncrement(ValkeyKey key, ValkeyValue member, double value);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)" /></returns>
    IBatch SortedSetIntersectionLength(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)" /></returns>
    IBatch SortedSetLengthByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(IEnumerable{ValkeyKey}, long, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(IEnumerable{ValkeyKey}, long, Order)" /></returns>
    IBatch SortedSetPop(IEnumerable<ValkeyKey> keys, long count, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScoresAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SortedSetScores(ValkeyKey key, IEnumerable<ValkeyValue> members);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, Order, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, Order, TimeSpan)" /></returns>
    IBatch SortedSetBlockingPop(ValkeyKey key, Order order, TimeSpan timeout);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(IEnumerable{ValkeyKey}, long, Order, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(IEnumerable{ValkeyKey}, long, Order, TimeSpan)" /></returns>
    IBatch SortedSetBlockingPop(IEnumerable<ValkeyKey> keys, long count, Order order, TimeSpan timeout);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey, Order)" /></returns>
    IBatch SortedSetPop(ValkeyKey key, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey, long, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey, long, Order)" /></returns>
    IBatch SortedSetPop(ValkeyKey key, long count, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMemberAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMemberAsync(ValkeyKey)" /></returns>
    IBatch SortedSetRandomMember(ValkeyKey key);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMembersAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMembersAsync(ValkeyKey, long)" /></returns>
    IBatch SortedSetRandomMembers(ValkeyKey key, long count);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long)" /></returns>
    IBatch SortedSetRandomMembersWithScores(ValkeyKey key, long count);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?)" /></returns>
    IBatch SortedSetRangeAndStore(
        ValkeyKey sourceKey,
        ValkeyKey destinationKey,
        ValkeyValue start,
        ValkeyValue stop,
        SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0,
        long? take = null);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order)" /></returns>
    IBatch SortedSetRank(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude)" /></returns>
    IBatch SortedSetRemoveRangeByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long)" /></returns>
    IBatch SortedSetRemoveRangeByRank(ValkeyKey key, long start, long stop);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude)" /></returns>
    IBatch SortedSetRemoveRangeByScore(ValkeyKey key, double min, double max, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int)" /></returns>
    IBatch SortedSetScan(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SortedSetScore(ValkeyKey key, ValkeyValue member);
}
