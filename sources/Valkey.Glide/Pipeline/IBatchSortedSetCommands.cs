// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Sorted Set Commands" group for batch operations.
/// <br/>
/// See more on <see href="https://valkey.io/commands/?group=sorted-set#sorted-set">valkey.io</see>.
/// </summary>
internal interface IBatchSortedSetCommands
{
    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen, CommandFlags)" /></returns>
    IBatch SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when = SortedSetWhen.Always);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetAddAsync(ValkeyKey, SortedSetEntry[], SortedSetWhen, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetAddAsync(ValkeyKey, SortedSetEntry[], SortedSetWhen, CommandFlags)" /></returns>
    IBatch SortedSetAdd(ValkeyKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch SortedSetRemove(ValkeyKey key, ValkeyValue member);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch SortedSetRemove(ValkeyKey key, ValkeyValue[] members);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetLengthAsync(ValkeyKey, double, double, Exclude, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetLengthAsync(ValkeyKey, double, double, Exclude, CommandFlags)" /></returns>
    IBatch SortedSetLength(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCardAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCardAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch SortedSetCard(ValkeyKey key);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCountAsync(ValkeyKey, double, double, Exclude, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCountAsync(ValkeyKey, double, double, Exclude, CommandFlags)" /></returns>
    IBatch SortedSetCount(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order, CommandFlags)" /></returns>
    IBatch SortedSetRangeByRank(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order, CommandFlags)" /></returns>
    IBatch SortedSetRangeByRankWithScores(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)" /></returns>
    IBatch SortedSetRangeByScore(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)" /></returns>
    IBatch SortedSetRangeByScoreWithScores(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long, CommandFlags)" /></returns>
    IBatch SortedSetRangeByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, long skip = 0, long take = -1);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long, CommandFlags)" /></returns>
    IBatch SortedSetRangeByValue(ValkeyKey key, ValkeyValue min = default, ValkeyValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAsync(SetOperation, ValkeyKey[], double[], Aggregate, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAsync(SetOperation, ValkeyKey[], double[], Aggregate, CommandFlags)" /></returns>
    IBatch SortedSetCombine(SetOperation operation, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineWithScoresAsync(SetOperation, ValkeyKey[], double[], Aggregate, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineWithScoresAsync(SetOperation, ValkeyKey[], double[], Aggregate, CommandFlags)" /></returns>
    IBatch SortedSetCombineWithScores(SetOperation operation, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate, CommandFlags)" /></returns>
    IBatch SortedSetCombineAndStore(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey[], double[], Aggregate, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey[], double[], Aggregate, CommandFlags)" /></returns>
    IBatch SortedSetCombineAndStore(SetOperation operation, ValkeyKey destination, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)" /></returns>
    IBatch SortedSetIncrement(ValkeyKey key, ValkeyValue member, double value);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetIntersectionLengthAsync(ValkeyKey[], long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetIntersectionLengthAsync(ValkeyKey[], long, CommandFlags)" /></returns>
    IBatch SortedSetIntersectionLength(ValkeyKey[] keys, long limit = 0);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, CommandFlags)" /></returns>
    IBatch SortedSetLengthByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey[], long, Order, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey[], long, Order, CommandFlags)" /></returns>
    IBatch SortedSetPop(ValkeyKey[] keys, long count, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScoresAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScoresAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch SortedSetScores(ValkeyKey key, ValkeyValue[] members);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, Order, double, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, Order, double, CommandFlags)" /></returns>
    IBatch SortedSetBlockingPop(ValkeyKey key, Order order, double timeout);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, long, Order, double, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey, long, Order, double, CommandFlags)" /></returns>
    IBatch SortedSetBlockingPop(ValkeyKey key, long count, Order order, double timeout);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey[], long, Order, double, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetBlockingPopAsync(ValkeyKey[], long, Order, double, CommandFlags)" /></returns>
    IBatch SortedSetBlockingPop(ValkeyKey[] keys, long count, Order order, double timeout);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey, Order, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey, Order, CommandFlags)" /></returns>
    IBatch SortedSetPop(ValkeyKey key, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey, long, Order, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetPopAsync(ValkeyKey, long, Order, CommandFlags)" /></returns>
    IBatch SortedSetPop(ValkeyKey key, long count, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMemberAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMemberAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch SortedSetRandomMember(ValkeyKey key);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMembersAsync(ValkeyKey, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMembersAsync(ValkeyKey, long, CommandFlags)" /></returns>
    IBatch SortedSetRandomMembers(ValkeyKey key, long count);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRandomMembersWithScoresAsync(ValkeyKey, long, CommandFlags)" /></returns>
    IBatch SortedSetRandomMembersWithScores(ValkeyKey key, long count);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?, CommandFlags)" /></returns>
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

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order, CommandFlags)" /></returns>
    IBatch SortedSetRank(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, CommandFlags)" /></returns>
    IBatch SortedSetRemoveRangeByValue(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long, CommandFlags)" /></returns>
    IBatch SortedSetRemoveRangeByRank(ValkeyKey key, long start, long stop);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude, CommandFlags)" /></returns>
    IBatch SortedSetRemoveRangeByScore(ValkeyKey key, double min, double max, Exclude exclude = Exclude.None);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)" /></returns>
    IBatch SortedSetScan(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0);

    /// <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.ISortedSetCommands.SortedSetScoreAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch SortedSetScore(ValkeyKey key, ValkeyValue member);
}
