// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Commands.Constants.Constants;
using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<long, bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options = default)
    {
        List<GlideString> args = [key, .. options.ToArgs(), .. new SortedSetEntry(member, score).ToArgs()];
        return new(RequestType.ZAdd, [.. args], false, response => response == 1);
    }

    public static Cmd<long, long> SortedSetAddAsync(ValkeyKey key, IEnumerable<SortedSetEntry> members, SortedSetAddOptions options = default)
    {
        List<GlideString> args = [key, .. options.ToArgs()];

        foreach (SortedSetEntry entry in members)
        {
            args.AddRange(entry.ToArgs());
        }

        return Simple<long>(RequestType.ZAdd, [.. args]);
    }

    public static Cmd<double, double> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value)
        => Simple<double>(RequestType.ZIncrBy, [key, value.ToGlideString(), member]);

    public static Cmd<double?, double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options)
        => new(RequestType.ZAdd, [key, .. options.ToArgs(), ValkeyLiterals.INCR, .. new SortedSetEntry(member, value).ToArgs()], true, response => response);

    public static Cmd<long, bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member)
        => Boolean<long>(RequestType.ZRem, [key, member]);

    public static Cmd<long, long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => Simple<long>(RequestType.ZRem, [key, .. members.ToGlideStrings()]);

    public static Cmd<long, long> SortedSetCardAsync(ValkeyKey key)
        => Simple<long>(RequestType.ZCard, [key]);

    public static Cmd<long, long> SortedSetCountAsync(ValkeyKey key, ScoreRange range)
        => Simple<long>(RequestType.ZCount, [key, .. range.ToArgs()]);

    public static Cmd<object[], ValkeyValue[]> SortedSetUnionAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.AddRange(aggregate.ToArgs());
        return new(RequestType.ZUnion, [.. args], false, ToValkeyValues);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetUnionAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        args.AddRange(aggregate.ToArgs());
        return new(RequestType.ZUnion, [.. args], false, ToValkeyValues);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetUnionWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.AddRange(aggregate.ToArgs());
        args.Add(WithScoresKeyword);
        return new(RequestType.ZUnion, [.. args], false, ToScoreResults);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetUnionWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        args.AddRange(aggregate.ToArgs());
        args.Add(WithScoresKeyword);
        return new(RequestType.ZUnion, [.. args], false, ToScoreResults);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetInterAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.AddRange(aggregate.ToArgs());
        return new(RequestType.ZInter, [.. args], false, ToValkeyValues);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetInterAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        args.AddRange(aggregate.ToArgs());
        return new(RequestType.ZInter, [.. args], false, ToValkeyValues);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetInterWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.AddRange(aggregate.ToArgs());
        args.Add(WithScoresKeyword);
        return new(RequestType.ZInter, [.. args], false, ToScoreResults);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetInterWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        args.AddRange(aggregate.ToArgs());
        args.Add(WithScoresKeyword);
        return new(RequestType.ZInter, [.. args], false, ToScoreResults);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetDiffAsync(IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        return new(RequestType.ZDiff, [.. args], false, ToValkeyValues);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetDiffWithScoreAsync(IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(WithScoresKeyword);
        return new(RequestType.ZDiff, [.. args], false, ToScoreResults);
    }

    public static Cmd<long, long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination];
        AddKeys(args, keys);
        args.AddRange(aggregate.ToArgs());
        return Simple<long>(RequestType.ZUnionStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        args.AddRange(aggregate.ToArgs());
        return Simple<long>(RequestType.ZUnionStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetInterAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination];
        AddKeys(args, keys);
        args.AddRange(aggregate.ToArgs());
        return Simple<long>(RequestType.ZInterStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetInterAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        args.AddRange(aggregate.ToArgs());
        return Simple<long>(RequestType.ZInterStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetDiffAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [destination];
        AddKeys(args, keys);
        return Simple<long>(RequestType.ZDiffStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);

        if (limit > 0)
        {
            args.Add(LimitKeyword);
            args.Add(limit.ToGlideString());
        }

        return Simple<long>(RequestType.ZInterCard, [.. args]);
    }

    public static Cmd<long, long> SortedSetLexCountAsync(ValkeyKey key, LexRange range)
    {
        GlideString[] rangeArgs = range.ToArgs();
        List<GlideString> args = [key, .. rangeArgs];
        return Simple<long>(RequestType.ZLexCount, [.. args]);
    }

    public static Cmd<object[], double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
    {
        List<GlideString> args = [key];
        args.AddRange(members.Select(m => (GlideString)m));

        return new(RequestType.ZMScore, [.. args], false, ToNullableDoubleArray);
    }

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMinAsync(ValkeyKey key)
        => new(RequestType.ZPopMin, [key], true, ToSortedSetEntry, true);

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMaxAsync(ValkeyKey key)
        => new(RequestType.ZPopMax, [key], true, ToSortedSetEntry, true);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetPopMinAsync(ValkeyKey key, long count)
        => new(RequestType.ZPopMin, [key, count.ToGlideString()], false, ToScoreResults);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetPopMaxAsync(ValkeyKey key, long count)
        => new(RequestType.ZPopMax, [key, count.ToGlideString()], false, ToScoreResults);

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [];

        AddKeys(args, keys);
        args.Add(MinKeyword);
        args.Add(CountKeyword);
        args.Add(1.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, ToSortedSetEntryFromPopResult, true);
    }

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(MaxKeyword);
        args.Add(CountKeyword);
        args.Add(1.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, ToSortedSetEntryFromPopResult, true);
    }

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, long count)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(MinKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, HandleSortedSetPopResultResponse, true);
    }

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, long count)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(MaxKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, HandleSortedSetPopResultResponse, true);
    }

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
    {
        List<GlideString> args = [ToSeconds(timeout)];
        AddKeys(args, keys);
        args.Add(MinKeyword);
        args.Add(CountKeyword);
        args.Add(1.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, ToSortedSetEntryFromPopResult, true);
    }

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
    {
        List<GlideString> args = [ToSeconds(timeout)];
        AddKeys(args, keys);
        args.Add(MaxKeyword);
        args.Add(CountKeyword);
        args.Add(1.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, ToSortedSetEntryFromPopResult, true);
    }

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMinAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan timeout)
    {
        List<GlideString> args = [ToSeconds(timeout)];
        AddKeys(args, keys);
        args.Add(MinKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, HandleSortedSetPopResultResponse, true);
    }

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMaxAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan timeout)
    {
        List<GlideString> args = [ToSeconds(timeout)];
        AddKeys(args, keys);
        args.Add(MaxKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, HandleSortedSetPopResultResponse, true);
    }

    public static Cmd<object[], SortedSetEntry?> SortedSetRandomMemberWithScoreAsync(ValkeyKey key)
    {
        List<GlideString> args = [key, 1.ToGlideString(), WithScoresKeyword];

        return new(RequestType.ZRandMember, [.. args], false, ToSortedSetEntryFromPairArray);
    }

    public static Cmd<object[], SortedSetEntry[]> SortedSetRandomMembersWithScoreAsync(ValkeyKey key, long count)
    {
        List<GlideString> args = [key, count.ToGlideString(), WithScoresKeyword];

        return new(RequestType.ZRandMember, [.. args], false, ToSortedSetEntriesFromPairArray);
    }

    public static Cmd<GlideString?, ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key) =>
        new(RequestType.ZRandMember, [key], true, response =>
            response is null ? ValkeyValue.Null : (ValkeyValue)response, true);

    public static Cmd<object[], ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count)
        => new(RequestType.ZRandMember, [key, count.ToGlideString()], false, ToValkeyValues);

    public static Cmd<object[], SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count)
    {
        List<GlideString> args = [key, count.ToGlideString(), WithScoresKeyword];

        return new(RequestType.ZRandMember, [.. args], false, ToSortedSetEntriesFromPairArray);
    }

    public static Cmd<long?, long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
    {
        RequestType requestType = order == Order.Ascending ? RequestType.ZRank : RequestType.ZRevRank;
        return new(requestType, [key, member], true, response => response);
    }

    public static Cmd<object[], (long Rank, double Score)?> SortedSetRankWithScoreAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
    {
        RequestType requestType = order == Order.Ascending ? RequestType.ZRank : RequestType.ZRevRank;
        return new(requestType, [key, member, ValkeyLiterals.WITHSCORE], true, ToRankAndScore);
    }

    public static Cmd<object[], (long cursor, SortedSetEntry[] items)> SortedSetScanAsync(ValkeyKey key, long cursor, ScanOptions? options = null)
    {
        List<GlideString> args = [key, cursor.ToGlideString()];

        // TODO simplify?
        if (options != null)
        {
            args.AddRange(options.ToArgs());
        }

        return new(RequestType.ZScan, [.. args], false, ParseScanResponse);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key, RangeOptions options)
    {
        List<GlideString> args = [key];
        args.AddRange(options.ToArgs());

        return new(RequestType.ZRange, [.. args], false, ToValkeyValues);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetRangeWithScoresAsync(ValkeyKey key, RangeOptions options)
    {
        List<GlideString> args = [key];
        args.AddRange(options.ToArgs());
        args.Add(ValkeyLiterals.WITHSCORES);

        return new(RequestType.ZRange, [.. args], false, ToScoreResults);
    }

    public static Cmd<long, long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination, RangeOptions options)
    {
        List<GlideString> args = [destination, source];
        args.AddRange(options.ToArgs());

        return Simple<long>(RequestType.ZRangeStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetRemoveRangeAsync(ValkeyKey key, Range range)
    {
        List<GlideString> args = [key];
        args.AddRange(range.ToArgs());

        RequestType requestType = range switch
        {
            IndexRange => RequestType.ZRemRangeByRank,
            ScoreRange => RequestType.ZRemRangeByScore,
            LexRange => RequestType.ZRemRangeByLex,
            _ => throw new ArgumentException($"Unsupported range type: {range.GetType()}")
        };

        return Simple<long>(requestType, [.. args]);
    }

    public static Cmd<double?, double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member)
        => new(RequestType.ZScore, [key, member], true, response => response);

    #region Private Methods

    private static readonly Func<object[], ValkeyValue[]> ToValkeyValues = array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)];
    private static readonly Func<Dictionary<GlideString, object>, SortedSetEntry[]> ToScoreResults = dict => [.. dict.Select(kvp => new SortedSetEntry((ValkeyValue)kvp.Key, (double)kvp.Value))];

    private static (long Rank, double Score)? ToRankAndScore(object[] response)
        => response is { Length: 2 } ? ((long)response[0], (double)response[1]) : null;

    private static double?[] ToNullableDoubleArray(object[] response)
    {
        double?[] scores = new double?[response.Length];
        for (int i = 0; i < response.Length; i++)
        {
            scores[i] = response[i] == null ? null : (double)response[i];
        }
        return scores;
    }

    private static SortedSetEntry? ToSortedSetEntryFromPairArray(object[] response)
    {
        if (response.Length == 0)
        {
            return null;
        }

        object[] pair = (object[])response[0];
        return new SortedSetEntry((ValkeyValue)(GlideString)pair[0], (double)pair[1]);
    }

    private static SortedSetEntry[] ToSortedSetEntriesFromPairArray(object[] response)
    {
        SortedSetEntry[] entries = new SortedSetEntry[response.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            object[] pair = (object[])response[i];
            entries[i] = new SortedSetEntry((ValkeyValue)(GlideString)pair[0], (double)pair[1]);
        }
        return entries;
    }

    private static (long cursor, SortedSetEntry[] items) ParseScanResponse(object[] response)
    {
        long newCursor = long.Parse(response[0].ToString()!);
        object[] itemsArray = (object[])response[1];

        SortedSetEntry[] entries = new SortedSetEntry[itemsArray.Length / 2];
        for (int i = 0; i < entries.Length; i++)
        {
            ValkeyValue member = (ValkeyValue)(GlideString)itemsArray[i * 2];
            double score = double.Parse(((GlideString)itemsArray[(i * 2) + 1]).ToString());
            entries[i] = new SortedSetEntry(member, score);
        }

        return (newCursor, entries);
    }

    private static SortedSetEntry? ToSortedSetEntry(object? response)
    {
        if (response is not Dictionary<GlideString, object> dict || dict.Count == 0)
        {
            return null;
        }

        KeyValuePair<GlideString, object> first = dict.First();
        return new SortedSetEntry((ValkeyValue)first.Key, (double)first.Value);
    }

    private static SortedSetEntry? ToSortedSetEntryFromPopResult(object? response)
    {
        SortedSetPopResult popResult = HandleSortedSetPopResultResponse(response);
        return popResult.IsNull || popResult.Entries.Length == 0 ? null : popResult.Entries[0];
    }

    /// <summary>
    /// Shared response handler for sorted set pop operations (both blocking and non-blocking).
    /// Handles the standard response format: [key, Dictionary&lt;member, score&gt;] or null.
    /// </summary>
    private static SortedSetPopResult HandleSortedSetPopResultResponse(object? response)
    {
        if (response == null)
        {
            return SortedSetPopResult.Null;
        }

        if (response is not object[] responseArray || responseArray.Length != 2)
        {
            throw new InvalidOperationException($"Unexpected response format for sorted set pop operation");
        }

        ValkeyKey key = ((GlideString)responseArray[0]).ToString();

        if (responseArray[1] is not Dictionary<GlideString, object> membersAndScores)
        {
            throw new InvalidOperationException($"Expected dictionary for members and scores, got {responseArray[1]?.GetType()}");
        }

        if (membersAndScores.Count == 0)
        {
            return SortedSetPopResult.Null;
        }

        SortedSetEntry[] entries = [.. membersAndScores.Select(kvp => new SortedSetEntry((ValkeyValue)kvp.Key, (double)kvp.Value))];

        return new SortedSetPopResult(key, entries);
    }

    private static void AddKeys(List<GlideString> args, IEnumerable<ValkeyKey> keys)
    {
        args.Add(keys.Count().ToGlideString());
        args.AddRange(keys.ToGlideStrings());
    }

    private static void AddWeights(List<GlideString> args, IEnumerable<double> weights)
    {
        args.Add(WeightsKeyword);
        args.AddRange(weights.ToGlideStrings());
    }

    #endregion
}
