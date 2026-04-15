// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Commands.Constants.Constants;
using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<long, bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetAddOptions options = default)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(options.ToArgs());

        // Add score-member pair
        args.Add(score.ToGlideString());
        args.Add(member.ToGlideString());

        return new(RequestType.ZAdd, [.. args], false, response => response == 1);
    }

    public static Cmd<long, long> SortedSetAddAsync(ValkeyKey key, IDictionary<ValkeyValue, double> members, SortedSetAddOptions options = default)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(options.ToArgs());

        // Add score-member pairs
        foreach (var kvp in members)
        {
            args.Add(kvp.Value.ToGlideString());
            args.Add(kvp.Key.ToGlideString());
        }

        return Simple<long>(RequestType.ZAdd, [.. args]);
    }

    public static Cmd<double, double> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value)
        => Simple<double>(RequestType.ZIncrBy, [key.ToGlideString(), value.ToGlideString(), member.ToGlideString()]);

    public static Cmd<double?, double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddCondition condition)
        => SortedSetIncrementByAsync(key, member, value, new SortedSetAddOptions { Condition = condition });

    public static Cmd<double?, double?> SortedSetIncrementByAsync(ValkeyKey key, ValkeyValue member, double value, SortedSetAddOptions options)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(options.ToArgs());
        args.Add(ValkeyLiterals.INCR);

        // Add score-member pair
        args.Add(value.ToGlideString());
        args.Add(member.ToGlideString());

        return new(RequestType.ZAdd, [.. args], true, response => response);
    }

    public static Cmd<long, bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member)
        => Boolean<long>(RequestType.ZRem, [key.ToGlideString(), member.ToGlideString()]);

    public static Cmd<long, long> SortedSetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
        => Simple<long>(RequestType.ZRem, [key.ToGlideString(), .. members.ToGlideStrings()]);

    public static Cmd<long, long> SortedSetCardAsync(ValkeyKey key)
        => Simple<long>(RequestType.ZCard, [key.ToGlideString()]);

    public static Cmd<long, long> SortedSetCountAsync(ValkeyKey key, ScoreRange range)
    {
        /// For unbounded ranges, use more efficient <see cref="SortedSetCardAsync"/>.
        if (range.IsUnbounded())
        {
            return SortedSetCardAsync(key);
        }

        return Simple<long>(RequestType.ZCount, [key.ToGlideString(), .. range.ToArgs()]);
    }

    private static readonly Func<object[], ValkeyValue[]> ToValkeyValues = array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)];
    private static readonly Func<Dictionary<GlideString, object>, SortedSetScoreResult[]> ToScoreResults = dict => [.. dict.Select(kvp => new SortedSetScoreResult((ValkeyValue)kvp.Key, (double)kvp.Value))];

    public static Cmd<object[], ValkeyValue[]> SortedSetUnionAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        AddAggregate(args, aggregate);
        return new(RequestType.ZUnion, [.. args], false, ToValkeyValues);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetUnionAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        AddAggregate(args, aggregate);
        return new(RequestType.ZUnion, [.. args], false, ToValkeyValues);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetScoreResult[]> SortedSetUnionWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        AddAggregate(args, aggregate);
        args.Add(WithScoresKeyword);
        return new(RequestType.ZUnion, [.. args], false, ToScoreResults);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetScoreResult[]> SortedSetUnionWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        AddAggregate(args, aggregate);
        args.Add(WithScoresKeyword);
        return new(RequestType.ZUnion, [.. args], false, ToScoreResults);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetInterAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        AddAggregate(args, aggregate);
        return new(RequestType.ZInter, [.. args], false, ToValkeyValues);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetInterAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        AddAggregate(args, aggregate);
        return new(RequestType.ZInter, [.. args], false, ToValkeyValues);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetScoreResult[]> SortedSetInterWithScoreAsync(IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        AddAggregate(args, aggregate);
        args.Add(WithScoresKeyword);
        return new(RequestType.ZInter, [.. args], false, ToScoreResults);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetScoreResult[]> SortedSetInterWithScoreAsync(IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        AddAggregate(args, aggregate);
        args.Add(WithScoresKeyword);
        return new(RequestType.ZInter, [.. args], false, ToScoreResults);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetDiffAsync(IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        return new(RequestType.ZDiff, [.. args], false, ToValkeyValues);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetScoreResult[]> SortedSetDiffWithScoreAsync(IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(WithScoresKeyword);
        return new(RequestType.ZDiff, [.. args], false, ToScoreResults);
    }

    public static Cmd<long, long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination.ToGlideString()];
        AddKeys(args, keys);
        AddAggregate(args, aggregate);
        return Simple<long>(RequestType.ZUnionStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetUnionAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination.ToGlideString()];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        AddAggregate(args, aggregate);
        return Simple<long>(RequestType.ZUnionStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetInterAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination.ToGlideString()];
        AddKeys(args, keys);
        AddAggregate(args, aggregate);
        return Simple<long>(RequestType.ZInterStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetInterAndStoreAsync(ValkeyKey destination, IDictionary<ValkeyKey, double> keysAndWeights, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination.ToGlideString()];
        AddKeys(args, keysAndWeights.Keys);
        AddWeights(args, keysAndWeights.Values);
        AddAggregate(args, aggregate);
        return Simple<long>(RequestType.ZInterStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetDiffAndStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [destination.ToGlideString()];
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
        List<GlideString> args = [key.ToGlideString(), .. rangeArgs];
        return Simple<long>(RequestType.ZLexCount, [.. args]);
    }

    public static Cmd<object[], double?[]> SortedSetScoresAsync(ValkeyKey key, IEnumerable<ValkeyValue> members)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(members.Select(member => member.ToGlideString()));

        return new(RequestType.ZMScore, [.. args], false, response =>
        {
            double?[] scores = new double?[response.Length];

            for (int i = 0; i < response.Length; i++)
            {
                scores[i] = response[i] == null ? null : (double)response[i];
            }

            return scores;
        });
    }

    public static Cmd<object?, SortedSetScoreResult?> SortedSetPopMinAsync(ValkeyKey key)
        => new(RequestType.ZPopMin, [key.ToGlideString()], true, response =>
        {
            if (response == null)
            {
                return null;
            }

            Dictionary<GlideString, object> responseDict = (Dictionary<GlideString, object>)response;
            if (responseDict.Count == 0)
            {
                return null;
            }

            KeyValuePair<GlideString, object> firstEntry = responseDict.First();
            return new SortedSetScoreResult((ValkeyValue)firstEntry.Key, (double)firstEntry.Value);
        }, allowConverterToHandleNull: true);

    public static Cmd<object?, SortedSetScoreResult?> SortedSetPopMaxAsync(ValkeyKey key)
        => new(RequestType.ZPopMax, [key.ToGlideString()], true, response =>
        {
            if (response == null)
            {
                return null;
            }

            Dictionary<GlideString, object> responseDict = (Dictionary<GlideString, object>)response;
            if (responseDict.Count == 0)
            {
                return null;
            }

            KeyValuePair<GlideString, object> firstEntry = responseDict.First();
            return new SortedSetScoreResult((ValkeyValue)firstEntry.Key, (double)firstEntry.Value);
        }, allowConverterToHandleNull: true);

    public static Cmd<Dictionary<GlideString, object>, SortedSetScoreResult[]> SortedSetPopMinAsync(ValkeyKey key, long count)
    {
        List<GlideString> args = [key.ToGlideString(), count.ToGlideString()];

        return new(RequestType.ZPopMin, [.. args], false, response =>
        {
            SortedSetScoreResult[] entries = new SortedSetScoreResult[response.Count];
            int i = 0;
            foreach (KeyValuePair<GlideString, object> kvp in response)
            {
                entries[i] = new SortedSetScoreResult((ValkeyValue)kvp.Key, (double)kvp.Value);
                i++;
            }

            return entries;
        });
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetScoreResult[]> SortedSetPopMaxAsync(ValkeyKey key, long count)
    {
        List<GlideString> args = [key.ToGlideString(), count.ToGlideString()];

        return new(RequestType.ZPopMax, [.. args], false, response =>
        {
            SortedSetScoreResult[] entries = new SortedSetScoreResult[response.Count];
            int i = 0;
            foreach (KeyValuePair<GlideString, object> kvp in response)
            {
                entries[i] = new SortedSetScoreResult((ValkeyValue)kvp.Key, (double)kvp.Value);
                i++;
            }

            return entries;
        });
    }

    public static Cmd<object?, SortedSetScoreResult?> SortedSetPopMinMultiKeyAsync(IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(MinKeyword);
        args.Add(CountKeyword);
        args.Add(1.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, response =>
        {
            SortedSetPopResult popResult = HandleSortedSetPopResultResponse(response);
            if (popResult.IsNull || popResult.Entries.Length == 0)
            {
                return null;
            }

            SortedSetEntry entry = popResult.Entries[0];
            return new SortedSetScoreResult(entry.Element, entry.Score);
        }, allowConverterToHandleNull: true);
    }

    public static Cmd<object?, SortedSetScoreResult?> SortedSetPopMaxMultiKeyAsync(IEnumerable<ValkeyKey> keys)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(MaxKeyword);
        args.Add(CountKeyword);
        args.Add(1.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, response =>
        {
            SortedSetPopResult popResult = HandleSortedSetPopResultResponse(response);
            if (popResult.IsNull || popResult.Entries.Length == 0)
            {
                return null;
            }

            SortedSetEntry entry = popResult.Entries[0];
            return new SortedSetScoreResult(entry.Element, entry.Score);
        }, allowConverterToHandleNull: true);
    }

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMinMultiKeyAsync(IEnumerable<ValkeyKey> keys, long count)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(MinKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, HandleSortedSetPopResultResponse, allowConverterToHandleNull: true);
    }

    public static Cmd<object?, SortedSetPopResult> SortedSetPopMaxMultiKeyAsync(IEnumerable<ValkeyKey> keys, long count)
    {
        List<GlideString> args = [];
        AddKeys(args, keys);
        args.Add(MaxKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, HandleSortedSetPopResultResponse, allowConverterToHandleNull: true);
    }

    public static Cmd<object?, SortedSetScoreResult?> SortedSetBlockingPopMinMultiKeyAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
    {
        List<GlideString> args = [ToSeconds(timeout).ToGlideString()];
        AddKeys(args, keys);
        args.Add(MinKeyword);
        args.Add(CountKeyword);
        args.Add(1.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, response =>
        {
            SortedSetPopResult popResult = HandleSortedSetPopResultResponse(response);
            if (popResult.IsNull || popResult.Entries.Length == 0)
            {
                return null;
            }

            SortedSetEntry entry = popResult.Entries[0];
            return new SortedSetScoreResult(entry.Element, entry.Score);
        }, allowConverterToHandleNull: true);
    }

    public static Cmd<object?, SortedSetScoreResult?> SortedSetBlockingPopMaxMultiKeyAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
    {
        List<GlideString> args = [ToSeconds(timeout).ToGlideString()];
        AddKeys(args, keys);
        args.Add(MaxKeyword);
        args.Add(CountKeyword);
        args.Add(1.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, response =>
        {
            SortedSetPopResult popResult = HandleSortedSetPopResultResponse(response);
            if (popResult.IsNull || popResult.Entries.Length == 0)
            {
                return null;
            }

            SortedSetEntry entry = popResult.Entries[0];
            return new SortedSetScoreResult(entry.Element, entry.Score);
        }, allowConverterToHandleNull: true);
    }

    public static Cmd<object?, SortedSetPopResult> SortedSetBlockingPopMinMultiKeyAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan timeout)
    {
        List<GlideString> args = [ToSeconds(timeout).ToGlideString()];
        AddKeys(args, keys);
        args.Add(MinKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, HandleSortedSetPopResultResponse, allowConverterToHandleNull: true);
    }

    public static Cmd<object?, SortedSetPopResult> SortedSetBlockingPopMaxMultiKeyAsync(IEnumerable<ValkeyKey> keys, long count, TimeSpan timeout)
    {
        List<GlideString> args = [ToSeconds(timeout).ToGlideString()];
        AddKeys(args, keys);
        args.Add(MaxKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, HandleSortedSetPopResultResponse, allowConverterToHandleNull: true);
    }

    public static Cmd<object[], SortedSetScoreResult?> SortedSetRandomMemberWithScoreAsync(ValkeyKey key)
    {
        List<GlideString> args = [key.ToGlideString(), 1.ToGlideString(), WithScoresKeyword];

        return new(RequestType.ZRandMember, [.. args], false, response =>
        {
            if (response.Length == 0)
            {
                return null;
            }

            object[] pair = (object[])response[0];
            ValkeyValue member = (ValkeyValue)(GlideString)pair[0];
            double score = (double)pair[1];
            return new SortedSetScoreResult(member, score);
        });
    }

    public static Cmd<object[], SortedSetScoreResult[]> SortedSetRandomMembersWithScoreAsync(ValkeyKey key, long count)
    {
        List<GlideString> args = [key.ToGlideString(), count.ToGlideString(), WithScoresKeyword];

        return new(RequestType.ZRandMember, [.. args], false, response =>
        {
            SortedSetScoreResult[] results = new SortedSetScoreResult[response.Length];
            for (int i = 0; i < results.Length; i++)
            {
                object[] pair = (object[])response[i];
                ValkeyValue member = (ValkeyValue)(GlideString)pair[0];
                double score = (double)pair[1];
                results[i] = new SortedSetScoreResult(member, score);
            }
            return results;
        });
    }

    public static Cmd<GlideString?, ValkeyValue> SortedSetRandomMemberAsync(ValkeyKey key) =>
        new(RequestType.ZRandMember, [key.ToGlideString()], true, response =>
            response is null ? ValkeyValue.Null : (ValkeyValue)response, allowConverterToHandleNull: true);

    public static Cmd<object[], ValkeyValue[]> SortedSetRandomMembersAsync(ValkeyKey key, long count)
    {
        List<GlideString> args = [key.ToGlideString(), count.ToGlideString()];

        return new(RequestType.ZRandMember, [.. args], false, response =>
            [.. response.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);
    }

    public static Cmd<object[], SortedSetEntry[]> SortedSetRandomMembersWithScoresAsync(ValkeyKey key, long count)
    {
        List<GlideString> args = [key.ToGlideString(), count.ToGlideString(), WithScoresKeyword];

        return new(RequestType.ZRandMember, [.. args], false, response =>
        {
            SortedSetEntry[] entries = new SortedSetEntry[response.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                object[] pair = (object[])response[i];
                ValkeyValue member = (ValkeyValue)(GlideString)pair[0];
                double score = (double)pair[1];
                entries[i] = new SortedSetEntry(member, score);
            }
            return entries;
        });
    }

    public static Cmd<long?, long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
    {
        RequestType requestType = order == Order.Ascending ? RequestType.ZRank : RequestType.ZRevRank;
        return new(requestType, [key.ToGlideString(), member.ToGlideString()], true, response => response);
    }

    // TODO #287: SortedSetScanAsync returns SER-only SortedSetEntry; should return GLIDE-native type.
    public static Cmd<object[], (long cursor, SortedSetEntry[] items)> SortedSetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0)
    {
        List<GlideString> args = [key.ToGlideString(), cursor.ToGlideString()];

        if (!pattern.IsNull)
        {
            args.Add(MatchKeyword);
            args.Add(pattern.ToGlideString());
        }

        if (pageSize != 250)
        {
            args.Add(CountKeyword);
            args.Add(pageSize.ToGlideString());
        }

        return new(RequestType.ZScan, [.. args], false, response =>
        {
            object[] responseArray = response;
            long newCursor = long.Parse(responseArray[0].ToString()!);
            object[] itemsArray = (object[])responseArray[1];

            SortedSetEntry[] entries = new SortedSetEntry[itemsArray.Length / 2];
            for (int i = 0; i < entries.Length; i++)
            {
                ValkeyValue member = (ValkeyValue)(GlideString)itemsArray[i * 2];
                double score = double.Parse(((GlideString)itemsArray[(i * 2) + 1]).ToString());
                entries[i] = new SortedSetEntry(member, score);
            }

            return (newCursor, entries);
        });
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetRangeAsync(ValkeyKey key, RangeOptions options = default)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(options.ToArgs());

        return new(RequestType.ZRange, [.. args], false, array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetScoreResult[]> SortedSetRangeWithScoresAsync(ValkeyKey key, RangeOptions options = default)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(options.ToArgs());
        args.Add(ValkeyLiterals.WITHSCORES);

        return new(RequestType.ZRange, [.. args], false, dict =>
            [.. dict.Select(kvp => new SortedSetScoreResult((ValkeyValue)kvp.Key, (double)kvp.Value))]);
    }

    public static Cmd<long, long> SortedSetRangeAndStoreAsync(ValkeyKey source, ValkeyKey destination, RangeOptions options = default)
    {
        List<GlideString> args = [destination.ToGlideString(), source.ToGlideString()];
        args.AddRange(options.ToArgs());

        return Simple<long>(RequestType.ZRangeStore, [.. args]);
    }

    public static Cmd<long, long> SortedSetRemoveRangeAsync(ValkeyKey key, Range range)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(range.ToArgs());

        RequestType requestType = range switch
        {
            RankRange => RequestType.ZRemRangeByRank,
            ScoreRange => RequestType.ZRemRangeByScore,
            LexRange => RequestType.ZRemRangeByLex,
            _ => throw new ArgumentException($"Unsupported range type: {range.GetType()}")
        };

        return Simple<long>(requestType, [.. args]);
    }

    public static Cmd<double?, double?> SortedSetScoreAsync(ValkeyKey key, ValkeyValue member)
        => new(RequestType.ZScore, [key.ToGlideString(), member.ToGlideString()], true, response => response);

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

    #region Private Methods

    private static void AddKeys(List<GlideString> args, IEnumerable<ValkeyKey> keys)
    {
        args.Add(keys.Count().ToGlideString());
        args.AddRange(keys.Select(key => key.ToGlideString()));
    }

    private static void AddWeights(List<GlideString> args, IEnumerable<double> weights)
    {
        args.Add(WeightsKeyword);
        args.AddRange(weights.Select(key => key.ToGlideString()));
    }

    private static void AddAggregate(List<GlideString> args, Aggregate aggregate)
    {
        if (aggregate != Aggregate.Sum)
        {
            args.Add(AggregateKeyword);
            args.Add(aggregate.ToString().ToUpper());
        }
    }

    #endregion
}
