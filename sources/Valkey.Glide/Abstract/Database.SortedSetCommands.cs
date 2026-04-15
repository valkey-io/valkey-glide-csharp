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

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen, CommandFlags)"/>
    public async Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score, ToSortedSetAddOptions(when));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, ValkeyValue, double, When, CommandFlags)"/>
    public Task<bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SortedSetAddAsync(key, member, score, SortedSetWhenExtensions.Parse(when), flags);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, CommandFlags)"/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, CommandFlags flags)
        => SortedSetAddAsync(key, values, SortedSetWhen.Always, flags);

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, When, CommandFlags)"/>
    public Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, When when, CommandFlags flags)
        => SortedSetAddAsync(key, values, SortedSetWhenExtensions.Parse(when), flags);

    /// <inheritdoc cref="IDatabaseAsync.SortedSetAddAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen, CommandFlags)"/>
    public async Task<long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, ToScoresDictionary(values), ToSortedSetAddOptions(when));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetUpdateAsync(ValkeyKey, ValkeyValue, double, SortedSetWhen, CommandFlags)"/>
    public async Task<bool> SortedSetUpdateAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, member, score, ToSortedSetAddOptions(when) with { Changed = true });
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetUpdateAsync(ValkeyKey, IEnumerable{SortedSetEntry}, SortedSetWhen, CommandFlags)"/>
    public async Task<long> SortedSetUpdateAsync(ValkeyKey key, IEnumerable<SortedSetEntry> values, SortedSetWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetAddAsync(key, ToScoresDictionary(values), ToSortedSetAddOptions(when) with { Changed = true });
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
        return double.IsNegativeInfinity(min) && double.IsPositiveInfinity(max)
            ? await SortedSetCardAsync(key)
            : await SortedSetCountAsync(key, ToScoreRange(min, max, exclude));
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
        return await SortedSetCountAsync(key, ToScoreRange(min, max, exclude));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByRankAsync(ValkeyKey, long, long, Order, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeAsync(key, new RangeOptions { Range = RankRange.Between(start, stop), Order = order });
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByRankWithScoresAsync(ValkeyKey, long, long, Order, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await SortedSetRangeWithScoresAsync(key, new RangeOptions { Range = RankRange.Between(start, stop), Order = order });
        return [.. results.Select(r => new SortedSetEntry(r.Member, r.Score))];
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByScoreAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeAsync(key, new RangeOptions { Range = ToScoreRange(start, stop, exclude), Order = order, Offset = skip, Count = take });
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByScoreWithScoresAsync(ValkeyKey, double, double, Exclude, Order, long, long, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await SortedSetRangeWithScoresAsync(key, new RangeOptions { Range = ToScoreRange(start, stop, exclude), Order = order, Offset = skip, Count = take });
        return [.. results.Select(r => new SortedSetEntry(r.Member, r.Score))];
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, long, long, CommandFlags)"/>
    public Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SortedSetRangeByValueAsync(key, min, max, exclude, Order.Ascending, skip, take);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, Order, long, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min = default, ValkeyValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRangeAsync(key, new RangeOptions { Range = ToLexRange(min, max, exclude), Order = order, Offset = skip, Count = take });
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        if (weights != null)
        {
            var kw = ToDictionary(keys, weights);
            return operation switch
            {
                SetOperation.Union => await SortedSetUnionAsync(kw, aggregate),
                SetOperation.Intersect => await SortedSetInterAsync(kw, aggregate),
                SetOperation.Difference => await SortedSetDiffAsync(keys),
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };
        }

        return operation switch
        {
            SetOperation.Union => await SortedSetUnionAsync(keys, aggregate),
            SetOperation.Intersect => await SortedSetInterAsync(keys, aggregate),
            SetOperation.Difference => await SortedSetDiffAsync(keys),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCombineWithScoresAsync(SetOperation, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        SortedSetScoreResult[] results;
        if (weights != null)
        {
            var kw = ToDictionary(keys, weights);
            results = operation switch
            {
                SetOperation.Union => await SortedSetUnionWithScoreAsync(kw, aggregate),
                SetOperation.Intersect => await SortedSetInterWithScoreAsync(kw, aggregate),
                SetOperation.Difference => await SortedSetDiffWithScoreAsync(keys),
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };
        }
        else
        {
            results = operation switch
            {
                SetOperation.Union => await SortedSetUnionWithScoreAsync(keys, aggregate),
                SetOperation.Intersect => await SortedSetInterWithScoreAsync(keys, aggregate),
                SetOperation.Difference => await SortedSetDiffWithScoreAsync(keys),
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };
        }

        return [.. results.Select(r => new SortedSetEntry(r.Member, r.Score))];
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, Aggregate, CommandFlags)"/>
    public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SortedSetCombineAndStoreAsync(operation, destination, (IEnumerable<ValkeyKey>)[first, second], null, aggregate);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, IEnumerable{double}?, Aggregate, CommandFlags)"/>
    public async Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, IEnumerable<double>? weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        if (weights != null)
        {
            var kw = ToDictionary(keys, weights);
            return operation switch
            {
                SetOperation.Union => await SortedSetUnionAndStoreAsync(destination, kw, aggregate),
                SetOperation.Intersect => await SortedSetInterAndStoreAsync(destination, kw, aggregate),
                SetOperation.Difference => await SortedSetDiffAndStoreAsync(destination, keys),
                _ => throw new ArgumentOutOfRangeException(nameof(operation)),
            };
        }

        return operation switch
        {
            SetOperation.Union => await SortedSetUnionAndStoreAsync(destination, keys, aggregate),
            SetOperation.Intersect => await SortedSetInterAndStoreAsync(destination, keys, aggregate),
            SetOperation.Difference => await SortedSetDiffAndStoreAsync(destination, keys),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)"/>
    public async Task<double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetIncrementByAsync(key, member, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetDecrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)"/>
    public Task<double> SortedSetDecrementAsync(ValkeyKey key, ValkeyValue member, double value, CommandFlags flags)
        => SortedSetIncrementAsync(key, member, -value, flags);

    /// <inheritdoc cref="IDatabaseAsync.SortedSetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public async Task<long> SortedSetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetInterCardAsync(keys, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetLengthByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, CommandFlags)"/>
    public async Task<long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetLexCountAsync(key, ToLexRange(min, max, exclude));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetPopAsync(ValkeyKey, Order, CommandFlags)"/>
    public async Task<SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var result = order == Order.Ascending ? await SortedSetPopMinAsync(key) : await SortedSetPopMaxAsync(key);
        return result.HasValue ? new SortedSetEntry(result.Value.Member, result.Value.Score) : null;
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetPopAsync(ValkeyKey, long, Order, CommandFlags)"/>
    public async Task<SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = order == Order.Ascending ? await SortedSetPopMinAsync(key, count) : await SortedSetPopMaxAsync(key, count);
        return [.. results.Select(r => new SortedSetEntry(r.Member, r.Score))];
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetPopAsync(IEnumerable{ValkeyKey}, long, Order, CommandFlags)"/>
    public Task<SortedSetPopResult> SortedSetPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return order == Order.Ascending ? SortedSetPopMinAsync(keys, count) : SortedSetPopMaxAsync(keys, count);
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
    public Task<SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.SortedSetRandomMembersWithScoresAsync(key, count));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRangeAndStoreAsync(ValkeyKey, ValkeyKey, ValkeyValue, ValkeyValue, SortedSetOrder, Exclude, Order, long, long?, CommandFlags)"/>
    public Task<long> SortedSetRangeAndStoreAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long? take = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SortedSetRangeAndStoreAsync(sourceKey, destinationKey, ToRangeOptions(start, stop, sortedSetOrder, exclude, order, skip, take));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRankAsync(ValkeyKey, ValkeyValue, Order, CommandFlags)"/>
    public async Task<long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortedSetRankAsync(key, member, order);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRemoveRangeByValueAsync(ValkeyKey, ValkeyValue, ValkeyValue, Exclude, CommandFlags)"/>
    public Task<long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SortedSetRemoveRangeAsync(key, ToLexRange(min, max, exclude));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRemoveRangeByRankAsync(ValkeyKey, long, long, CommandFlags)"/>
    public Task<long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SortedSetRemoveRangeAsync(key, RankRange.Between(start, stop));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetRemoveRangeByScoreAsync(ValkeyKey, double, double, Exclude, CommandFlags)"/>
    public Task<long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SortedSetRemoveRangeAsync(key, ToScoreRange(start, stop, exclude));
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)"/>
    public IAsyncEnumerable<SortedSetEntry> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return base.SortedSetScanAsync(key, pattern, pageSize, cursor, pageOffset);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortedSetBlockingPopAsync(ValkeyKey, Order, TimeSpan)"/>
    public Task<SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, TimeSpan timeout)
        => base.SortedSetBlockingPopAsync(key, order, timeout);

    /// <inheritdoc cref="IDatabaseAsync.SortedSetBlockingPopAsync(IEnumerable{ValkeyKey}, long, Order, TimeSpan)"/>
    public Task<SortedSetPopResult> SortedSetBlockingPopAsync(IEnumerable<ValkeyKey> keys, long count, Order order, TimeSpan timeout)
        => base.SortedSetBlockingPopAsync(keys, count, order, timeout);

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

    private static SortedSetAddOptions ToSortedSetAddOptions(SortedSetWhen when)
    {
        SortedSetAddCondition condition = when switch
        {
            SortedSetWhen.Always => SortedSetAddCondition.Always,
            SortedSetWhen.NotExists => SortedSetAddCondition.OnlyIfNotExists,
            SortedSetWhen.Exists => SortedSetAddCondition.OnlyIfExists,
            var w when w == (SortedSetWhen.Exists | SortedSetWhen.GreaterThan) => SortedSetAddCondition.OnlyIfGreaterThan,
            var w when w == (SortedSetWhen.Exists | SortedSetWhen.LessThan) => SortedSetAddCondition.OnlyIfLessThan,
            SortedSetWhen.GreaterThan => SortedSetAddCondition.OnlyIfNotExistsOrGreaterThan,
            SortedSetWhen.LessThan => SortedSetAddCondition.OnlyIfNotExistsOrLessThan,
            _ => SortedSetAddCondition.Always,
        };
        return new SortedSetAddOptions { Condition = condition };
    }

    private static IDictionary<ValkeyValue, double> ToScoresDictionary(IEnumerable<SortedSetEntry> entries)
        => entries.ToDictionary(e => e.Element, e => e.Score);

    private static IDictionary<ValkeyKey, double> ToDictionary(IEnumerable<ValkeyKey> keys, IEnumerable<double> weights)
    {
        Dictionary<ValkeyKey, double> result = [];
        using var keyEnum = keys.GetEnumerator();
        using var weightEnum = weights.GetEnumerator();
        while (true)
        {
            bool hasKey = keyEnum.MoveNext();
            bool hasWeight = weightEnum.MoveNext();
            if (hasKey != hasWeight) throw new ArgumentException("The number of weights must match the number of keys.", nameof(weights));
            if (!hasKey) return result;
            result[keyEnum.Current] = weightEnum.Current;
        }
    }

    private static ScoreRange ToScoreRange(double min, double max, Exclude exclude)
    {
        var minBound = double.IsNegativeInfinity(min) ? ScoreBound.Min
            : exclude.HasFlag(Exclude.Start) ? ScoreBound.Exclusive(min) : ScoreBound.Inclusive(min);
        var maxBound = double.IsPositiveInfinity(max) ? ScoreBound.Max
            : exclude.HasFlag(Exclude.Stop) ? ScoreBound.Exclusive(max) : ScoreBound.Inclusive(max);
        return ScoreRange.Between(minBound, maxBound);
    }

    private static LexRange ToLexRange(ValkeyValue min, ValkeyValue max, Exclude exclude)
    {
        var minBound = min.IsNull ? LexBound.Min
            : exclude.HasFlag(Exclude.Start) ? LexBound.Exclusive(min) : LexBound.Inclusive(min);
        var maxBound = max.IsNull ? LexBound.Max
            : exclude.HasFlag(Exclude.Stop) ? LexBound.Exclusive(max) : LexBound.Inclusive(max);
        return LexRange.Between(minBound, maxBound);
    }

    private static RangeOptions ToRangeOptions(ValkeyValue start, ValkeyValue stop, SortedSetOrder sortedSetOrder, Exclude exclude, Order order, long skip, long? take)
    {
        Range range = sortedSetOrder switch
        {
            SortedSetOrder.ByScore => ToScoreRange((double)start, (double)stop, exclude),
            SortedSetOrder.ByLex => ToLexRange(start, stop, exclude),
            _ => RankRange.Between((long)start, (long)stop),
        };
        return new RangeOptions { Range = range, Order = order, Offset = skip, Count = take ?? -1 };
    }
}
