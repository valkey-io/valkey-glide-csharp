// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    #region Command Builders

    public static Cmd<long, bool> SortedSetAdd(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options = default)
        => new(RequestType.ZAdd, [key, .. options.ToArgs(), .. new SortedSetEntry(member, score).ToArgs()], false, response => response == 1);

    public static Cmd<long, long> SortedSetAdd(ValkeyKey key, IEnumerable<SortedSetEntry> members, SortedSetAddOptions options = default)
        => Simple<long>(RequestType.ZAdd, [key, .. options.ToArgs(), .. members.SelectMany(m => m.ToArgs())]);

    public static Cmd<long, long> SortedSetCard(ValkeyKey key)
        => Simple<long>(RequestType.ZCard, [key]);

    public static Cmd<long, long> SortedSetCount(ValkeyKey key, ScoreRange range)
        => Simple<long>(RequestType.ZCount, [key, .. range.ToArgs()]);

    public static Cmd<long, long> SortedSetDiffAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Simple<long>(RequestType.ZDiffStore, [destination, .. GetKeysArgs(keys)]);

    public static Cmd<object[], ValkeyValue[]> SortedSetDiff(IEnumerable<ValkeyKey> keys)
        => new(RequestType.ZDiff, [.. GetKeysArgs(keys)], false, ToValkeyValues);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetDiffWithScore(IEnumerable<ValkeyKey> keys)
        => new(RequestType.ZDiff, [.. GetKeysArgs(keys), ValkeyLiterals.WITHSCORES], false, ToScoreResults);

    public static Cmd<double, double> SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value)
        => Simple<double>(RequestType.ZIncrBy, [key, value.ToGlideString(), member]);

    public static Cmd<double?, double?> SortedSetIncrementBy(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options)
        => new(RequestType.ZAdd, [key, .. options.ToArgs(), ValkeyLiterals.INCR, .. new SortedSetEntry(member, value).ToArgs()], true, response => response);

    public static Cmd<long, long> SortedSetInterAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Simple<long>(RequestType.ZInterStore, [destination, .. GetKeysArgs(keys), .. aggregate.ToArgs()]);

    public static Cmd<long, long> SortedSetInterAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Simple<long>(RequestType.ZInterStore, [destination, .. GetKeysArgs(keysAndWeights.Keys), .. GetWeightsArgs(keysAndWeights.Values), .. aggregate.ToArgs()]);

    public static Cmd<object[], ValkeyValue[]> SortedSetInter(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => new(RequestType.ZInter, [.. GetKeysArgs(keys), .. aggregate.ToArgs()], false, ToValkeyValues);

    public static Cmd<object[], ValkeyValue[]> SortedSetInter(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => new(RequestType.ZInter, [.. GetKeysArgs(keysAndWeights.Keys), .. GetWeightsArgs(keysAndWeights.Values), .. aggregate.ToArgs()], false, ToValkeyValues);

    public static Cmd<long, long> SortedSetInterCard(IEnumerable<ValkeyKey> keys, long limit = 0)
    {
        List<GlideString> args = [.. GetKeysArgs(keys)];

        if (limit > 0)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add(limit.ToGlideString());
        }

        return Simple<long>(RequestType.ZInterCard, [.. args]);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetInterWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => new(RequestType.ZInter, [.. GetKeysArgs(keys), .. aggregate.ToArgs(), ValkeyLiterals.WITHSCORES], false, ToScoreResults);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetInterWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => new(RequestType.ZInter, [.. GetKeysArgs(keysAndWeights.Keys), .. GetWeightsArgs(keysAndWeights.Values), .. aggregate.ToArgs(), ValkeyLiterals.WITHSCORES], false, ToScoreResults);

    public static Cmd<long, long> SortedSetLexCount(ValkeyKey key, LexRange range)
        => Simple<long>(RequestType.ZLexCount, [key, .. range.ToArgs()]);

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMax(ValkeyKey key)
        => new(RequestType.ZPopMax, [key], true, ToSortedSetEntry, true);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetPopMax(ValkeyKey key, long count)
        => new(RequestType.ZPopMax, [key, count.ToGlideString()], false, ToScoreResults);

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMax(IEnumerable<ValkeyKey> keys)
        => new(RequestType.ZMPop, [.. GetKeysArgs(keys), ValkeyLiterals.MAX, ValkeyLiterals.COUNT, "1"], true, ToSortedSetEntryFromPopResult, true);

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMax(IEnumerable<ValkeyKey> keys, long count)
        => new(RequestType.ZMPop, [.. GetKeysArgs(keys), ValkeyLiterals.MAX, ValkeyLiterals.COUNT, count.ToGlideString()], true, HandleSortedSetPopResultResponse, true);

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMax(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
        => new(RequestType.BZMPop, [ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString(), keys.Count().ToGlideString(), .. keys, ValkeyLiterals.MAX, ValkeyLiterals.COUNT, "1"], true, ToSortedSetEntryFromPopResult, true);

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMax(IEnumerable<ValkeyKey> keys, long count, TimeSpan timeout)
        => new(RequestType.BZMPop, [ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString(), keys.Count().ToGlideString(), .. keys, ValkeyLiterals.MAX, ValkeyLiterals.COUNT, count.ToGlideString()], true, HandleSortedSetPopResultResponse, true);

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMin(ValkeyKey key)
        => new(RequestType.ZPopMin, [key], true, ToSortedSetEntry, true);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetPopMin(ValkeyKey key, long count)
        => new(RequestType.ZPopMin, [key, count.ToGlideString()], false, ToScoreResults);

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMin(IEnumerable<ValkeyKey> keys)
        => new(RequestType.ZMPop, [.. GetKeysArgs(keys), ValkeyLiterals.MIN, ValkeyLiterals.COUNT, "1"], true, ToSortedSetEntryFromPopResult, true);

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMin(IEnumerable<ValkeyKey> keys, long count)
        => new(RequestType.ZMPop, [.. GetKeysArgs(keys), ValkeyLiterals.MIN, ValkeyLiterals.COUNT, count.ToGlideString()], true, HandleSortedSetPopResultResponse, true);

    public static Cmd<object?, SortedSetEntry?> SortedSetPopMin(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
        => new(RequestType.BZMPop, [ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString(), keys.Count().ToGlideString(), .. keys, ValkeyLiterals.MIN, ValkeyLiterals.COUNT, "1"], true, ToSortedSetEntryFromPopResult, true);

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMin(IEnumerable<ValkeyKey> keys, long count, TimeSpan timeout)
        => new(RequestType.BZMPop, [ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString(), keys.Count().ToGlideString(), .. keys, ValkeyLiterals.MIN, ValkeyLiterals.COUNT, count.ToGlideString()], true, HandleSortedSetPopResultResponse, true);

    public static Cmd<GlideString?, ValkeyValue> SortedSetRandomMember(ValkeyKey key) =>
        new(RequestType.ZRandMember, [key], true, response =>
            response is null ? ValkeyValue.Null : (ValkeyValue)response, true);

    public static Cmd<object[], ValkeyValue[]> SortedSetRandomMembers(ValkeyKey key, long count)
        => new(RequestType.ZRandMember, [key, count.ToGlideString()], false, ToValkeyValues);

    public static Cmd<object[], SortedSetEntry[]> SortedSetRandomMembersWithScore(ValkeyKey key, long count)
        => new(RequestType.ZRandMember, [key, count.ToGlideString(), ValkeyLiterals.WITHSCORES], false, ToSortedSetEntriesFromPairArray);

    public static Cmd<object[], SortedSetEntry[]> SortedSetRandomMembersWithScores(ValkeyKey key, long count)
        => new(RequestType.ZRandMember, [key, count.ToGlideString(), ValkeyLiterals.WITHSCORES], false, ToSortedSetEntriesFromPairArray);

    public static Cmd<object[], SortedSetEntry?> SortedSetRandomMemberWithScore(ValkeyKey key)
        => new(RequestType.ZRandMember, [key, "1", ValkeyLiterals.WITHSCORES], false, ToSortedSetEntryFromPairArray);

    public static Cmd<long, long> SortedSetRangeAndStore(ValkeyKey source, ValkeyKey destination, RangeOptions options)
        => Simple<long>(RequestType.ZRangeStore, [destination, source, .. options.ToArgs()]);

    public static Cmd<object[], ValkeyValue[]> SortedSetRange(ValkeyKey key, RangeOptions options)
        => new(RequestType.ZRange, [key, .. options.ToArgs()], false, ToValkeyValues);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetRangeWithScores(ValkeyKey key, RangeOptions options)
        => new(RequestType.ZRange, [key, .. options.ToArgs(), ValkeyLiterals.WITHSCORES], false, ToScoreResults);

    public static Cmd<long?, long?> SortedSetRank(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
    {
        var requestType = order == Order.Ascending ? RequestType.ZRank : RequestType.ZRevRank;
        return new(requestType, [key, member], true, response => response);
    }

    public static Cmd<object[], (long Rank, double Score)?> SortedSetRankWithScore(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
    {
        var requestType = order == Order.Ascending ? RequestType.ZRank : RequestType.ZRevRank;
        return new(requestType, [key, member, ValkeyLiterals.WITHSCORE], true, ToRankAndScore);
    }

    public static Cmd<long, bool> SortedSetRemove(ValkeyKey key, ValkeyValue member)
        => Boolean<long>(RequestType.ZRem, [key, member]);

    public static Cmd<long, long> SortedSetRemove(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => Simple<long>(RequestType.ZRem, [key, .. members.ToGlideStrings()]);

    public static Cmd<long, long> SortedSetRemoveRange(ValkeyKey key, Range range)
    {
        var requestType = range switch
        {
            IndexRange => RequestType.ZRemRangeByRank,
            ScoreRange => RequestType.ZRemRangeByScore,
            LexRange => RequestType.ZRemRangeByLex,
            _ => throw new ArgumentException($"Unsupported range type: {range.GetType()}")
        };

        return Simple<long>(requestType, [key, .. range.ToArgs()]);
    }

    public static Cmd<object[], (long cursor, SortedSetEntry[] items)> SortedSetScan(ValkeyKey key, long cursor, ScanOptions? options = null)
        => new(RequestType.ZScan, [key, cursor.ToGlideString(), .. ToScanArgs(options)], false, ParseScanResponse);

    public static Cmd<double?, double?> SortedSetScore(ValkeyKey key, ValkeyValue member)
        => new(RequestType.ZScore, [key, member], true, response => response);

    public static Cmd<object[], double?[]> SortedSetScores(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => new(RequestType.ZMScore, [key, .. members], false, ToNullableDoubleArray);

    public static Cmd<long, long> SortedSetUnionAndStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => Simple<long>(RequestType.ZUnionStore, [destination, .. GetKeysArgs(keys), .. aggregate.ToArgs()]);

    public static Cmd<long, long> SortedSetUnionAndStore(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => Simple<long>(RequestType.ZUnionStore, [destination, .. GetKeysArgs(keysAndWeights.Keys), .. GetWeightsArgs(keysAndWeights.Values), .. aggregate.ToArgs()]);

    public static Cmd<object[], ValkeyValue[]> SortedSetUnion(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => new(RequestType.ZUnion, [.. GetKeysArgs(keys), .. aggregate.ToArgs()], false, ToValkeyValues);

    public static Cmd<object[], ValkeyValue[]> SortedSetUnion(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => new(RequestType.ZUnion, [.. GetKeysArgs(keysAndWeights.Keys), .. GetWeightsArgs(keysAndWeights.Values), .. aggregate.ToArgs()], false, ToValkeyValues);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetUnionWithScore(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
        => new(RequestType.ZUnion, [.. GetKeysArgs(keys), .. aggregate.ToArgs(), ValkeyLiterals.WITHSCORES], false, ToScoreResults);

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetUnionWithScore(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
        => new(RequestType.ZUnion, [.. GetKeysArgs(keysAndWeights.Keys), .. GetWeightsArgs(keysAndWeights.Values), .. aggregate.ToArgs(), ValkeyLiterals.WITHSCORES], false, ToScoreResults);

    #endregion Command Builders
    #region Response Converters

    private static readonly Func<object[], ValkeyValue[]> ToValkeyValues =
        array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)];

    private static readonly Func<Dictionary<GlideString, object>, SortedSetEntry[]> ToScoreResults =
        dict => [.. dict.Select(kvp => new SortedSetEntry((ValkeyValue)kvp.Key, (double)kvp.Value))];

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
    /// <param name="response">The pop response.</param>
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

    #endregion Response Converters
    #region Argument Builders

    private static IEnumerable<GlideString> GetKeysArgs(IEnumerable<ValkeyKey> keys)
        => [keys.Count().ToGlideString(), .. keys];

    private static IEnumerable<GlideString> GetWeightsArgs(IEnumerable<double> weights)
        => [ValkeyLiterals.WEIGHTS, .. weights.ToGlideStrings()];

    #endregion Argument Builders
}
