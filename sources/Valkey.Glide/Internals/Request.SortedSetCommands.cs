// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Commands.Constants.Constants;
using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    private static void AddSortedSetCombineOptions(List<GlideString> args, ValkeyKey[] keys, double[]? weights, Aggregate aggregate, SetOperation operation)
    {
        // Add number of keys
        args.Add(keys.Length.ToGlideString());

        // Add keys
        args.AddRange(keys.Select(key => key.ToGlideString()));

        // Add weights if provided (not allowed for difference)
        if (weights != null && operation != SetOperation.Difference)
        {
            args.Add(WeightsKeyword);
            args.AddRange(weights.Select(w => w.ToGlideString()));
        }

        // Add aggregate if not default (not allowed for difference)
        if (aggregate != Aggregate.Sum && operation != SetOperation.Difference)
        {
            args.Add(AggregateKeyword);
            args.Add(aggregate.ToString().ToUpper());
        }
    }

    private static RequestType GetSortedSetCombineRequestType(SetOperation operation, bool isStore = false) => operation switch
    {
        SetOperation.Union => isStore ? RequestType.ZUnionStore : RequestType.ZUnion,
        SetOperation.Intersect => isStore ? RequestType.ZInterStore : RequestType.ZInter,
        SetOperation.Difference => isStore ? RequestType.ZDiffStore : RequestType.ZDiff,
        _ => throw new ArgumentException($"Unsupported operation: {operation}")
    };

    private static void AddSortedSetWhenOptions(List<GlideString> args, SortedSetWhen when)
    {
        // Add conditional options
        if (when.HasFlag(SortedSetWhen.Exists))
        {
            args.Add(ExistsKeyword);
        }
        else if (when.HasFlag(SortedSetWhen.NotExists))
        {
            args.Add(NotExistsKeyword);
        }

        if (when.HasFlag(SortedSetWhen.GreaterThan))
        {
            args.Add(GreaterThanKeyword);
        }
        else if (when.HasFlag(SortedSetWhen.LessThan))
        {
            args.Add(LessThanKeyword);
        }
    }

    public static Cmd<long, bool> SortedSetAddAsync(ValkeyKey key, ValkeyValue member, double score, SortedSetWhen when = SortedSetWhen.Always)
    {
        List<GlideString> args = [key.ToGlideString()];
        AddSortedSetWhenOptions(args, when);

        // Add score and member
        args.Add(score.ToGlideString());
        args.Add(member.ToGlideString());

        return new(RequestType.ZAdd, [.. args], false, response => response == 1);
    }

    public static Cmd<long, long> SortedSetAddAsync(ValkeyKey key, SortedSetEntry[] values, SortedSetWhen when = SortedSetWhen.Always)
    {
        List<GlideString> args = [key.ToGlideString()];
        AddSortedSetWhenOptions(args, when);

        // Add score-member pairs
        foreach (SortedSetEntry entry in values)
        {
            args.Add(entry.Score.ToGlideString());
            args.Add(entry.Element.ToGlideString());
        }

        return Simple<long>(RequestType.ZAdd, [.. args]);
    }

    public static Cmd<long, bool> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue member)
        => Boolean<long>(RequestType.ZRem, [key.ToGlideString(), member.ToGlideString()]);

    public static Cmd<long, long> SortedSetRemoveAsync(ValkeyKey key, ValkeyValue[] members)
    {
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(members.Select(member => member.ToGlideString()));

        return Simple<long>(RequestType.ZRem, [.. args]);
    }

    public static Cmd<long, long> SortedSetCardAsync(ValkeyKey key)
        => Simple<long>(RequestType.ZCard, [key.ToGlideString()]);

    public static Cmd<long, long> SortedSetCountAsync(ValkeyKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None)
    {
        // Create score boundaries based on exclude flags
        ScoreBoundary minBoundary = exclude.HasFlag(Exclude.Start)
            ? ScoreBoundary.Exclusive(min)
            : ScoreBoundary.Inclusive(min);

        ScoreBoundary maxBoundary = exclude.HasFlag(Exclude.Stop)
            ? ScoreBoundary.Exclusive(max)
            : ScoreBoundary.Inclusive(max);

        ZCountRange range = new(minBoundary, maxBoundary);
        string[] rangeArgs = range.ToArgs();

        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(rangeArgs.Select(arg => arg.ToGlideString()));

        return Simple<long>(RequestType.ZCount, [.. args]);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetRangeByRankAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending)
    {
        RangeByIndex query = new(start, stop);
        if (order == Order.Descending)
        {
            _ = query.SetReverse();
        }

        string[] queryArgs = query.ToArgs();
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(queryArgs.Select(arg => arg.ToGlideString()));

        return new(RequestType.ZRange, [.. args], false, array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(ValkeyKey key, long start = 0, long stop = -1, Order order = Order.Ascending)
    {
        RangeByIndex query = new(start, stop);
        if (order == Order.Descending)
        {
            _ = query.SetReverse();
        }

        string[] queryArgs = query.ToArgs();
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(queryArgs.Select(arg => arg.ToGlideString()));
        args.Add(WithScoresKeyword);

        return new(RequestType.ZRange, [.. args], false, dict =>
        {
            IEnumerable<SortedSetEntry> entries = dict.Select(kvp => new SortedSetEntry((ValkeyValue)kvp.Key, (double)kvp.Value));

            // Sort by score, then by element for consistent ordering
            IOrderedEnumerable<SortedSetEntry> sortedEntries = order == Order.Ascending
                ? entries.OrderBy(e => e.Score).ThenBy(e => e.Element.ToString())
                : entries.OrderByDescending(e => e.Score).ThenByDescending(e => e.Element.ToString());
            return [.. sortedEntries];
        });
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetRangeByScoreAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
    {
        // Create score boundaries based on exclude flags
        ScoreBoundary startBoundary = exclude.HasFlag(Exclude.Start)
            ? ScoreBoundary.Exclusive(start)
            : ScoreBoundary.Inclusive(start);

        ScoreBoundary stopBoundary = exclude.HasFlag(Exclude.Stop)
            ? ScoreBoundary.Exclusive(stop)
            : ScoreBoundary.Inclusive(stop);

        RangeByScore query = new(startBoundary, stopBoundary);
        if (order == Order.Descending)
        {
            _ = query.SetReverse();
        }

        if (take != -1)
        {
            _ = query.SetLimit(skip, take);
        }

        string[] queryArgs = query.ToArgs();
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(queryArgs.Select(arg => arg.ToGlideString()));

        return new(RequestType.ZRange, [.. args], false, array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(ValkeyKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
    {
        // Create score boundaries based on exclude flags
        ScoreBoundary startBoundary = exclude.HasFlag(Exclude.Start)
            ? ScoreBoundary.Exclusive(start)
            : ScoreBoundary.Inclusive(start);

        ScoreBoundary stopBoundary = exclude.HasFlag(Exclude.Stop)
            ? ScoreBoundary.Exclusive(stop)
            : ScoreBoundary.Inclusive(stop);

        RangeByScore query = new(startBoundary, stopBoundary);
        if (order == Order.Descending)
        {
            _ = query.SetReverse();
        }

        if (take != -1)
        {
            _ = query.SetLimit(skip, take);
        }

        string[] queryArgs = query.ToArgs();
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(queryArgs.Select(arg => arg.ToGlideString()));
        args.Add(WithScoresKeyword);

        return new(RequestType.ZRange, [.. args], false, dict =>
        {
            IEnumerable<SortedSetEntry> entries = dict.Select(kvp => new SortedSetEntry((ValkeyValue)kvp.Key, (double)kvp.Value));

            // Sort by score, then by element for consistent ordering
            IOrderedEnumerable<SortedSetEntry> sortedEntries = order == Order.Ascending
                ? entries.OrderBy(e => e.Score).ThenBy(e => e.Element.ToString())
                : entries.OrderByDescending(e => e.Score).ThenByDescending(e => e.Element.ToString());
            return [.. sortedEntries];
        });
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None, long skip = 0, long take = -1)
    {
        // Create lexicographical boundaries based on exclude flags
        LexBoundary minBoundary = exclude.HasFlag(Exclude.Start)
            ? LexBoundary.Exclusive(min)
            : LexBoundary.Inclusive(min);

        LexBoundary maxBoundary = exclude.HasFlag(Exclude.Stop)
            ? LexBoundary.Exclusive(max)
            : LexBoundary.Inclusive(max);

        RangeByLex query = new(minBoundary, maxBoundary);

        if (take != -1)
        {
            _ = query.SetLimit(skip, take);
        }

        string[] queryArgs = query.ToArgs();
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(queryArgs.Select(arg => arg.ToGlideString()));

        return new(RequestType.ZRange, [.. args], false, array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetRangeByValueAsync(ValkeyKey key, ValkeyValue min = default, ValkeyValue max = default, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1)
    {
        // Handle default values for min and max
        ValkeyValue actualMin = min.IsNull ? NegativeInfinity : min;
        ValkeyValue actualMax = max.IsNull ? PositiveInfinity : max;

        // Create lexicographical boundaries based on exclude flags
        // Handle double infinity values and default infinity symbols by converting them to lexicographical infinity symbols
        string minStr = actualMin.ToString();
        LexBoundary minBoundary = minStr switch
        {
            NegativeInfinityScore or NegativeInfinity => LexBoundary.NegativeInfinity(),
            PositiveInfinityScore or PositiveInfinity => LexBoundary.PositiveInfinity(),
            _ => exclude.HasFlag(Exclude.Start) ? LexBoundary.Exclusive(actualMin) : LexBoundary.Inclusive(actualMin)
        };

        string maxStr = actualMax.ToString();
        LexBoundary maxBoundary = maxStr switch
        {
            NegativeInfinityScore or NegativeInfinity => LexBoundary.NegativeInfinity(),
            PositiveInfinityScore or PositiveInfinity => LexBoundary.PositiveInfinity(),
            _ => exclude.HasFlag(Exclude.Stop) ? LexBoundary.Exclusive(actualMax) : LexBoundary.Inclusive(actualMax)
        };

        RangeByLex query = new(minBoundary, maxBoundary);
        if (order == Order.Descending)
        {
            _ = query.SetReverse();
        }

        if (take != -1)
        {
            _ = query.SetLimit(skip, take);
        }

        string[] queryArgs = query.ToArgs();
        List<GlideString> args = [key.ToGlideString()];
        args.AddRange(queryArgs.Select(arg => arg.ToGlideString()));

        return new(RequestType.ZRange, [.. args], false, array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);
    }

    public static Cmd<object[], ValkeyValue[]> SortedSetCombineAsync(SetOperation operation, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddSortedSetCombineOptions(args, keys, weights, aggregate, operation);

        RequestType requestType = GetSortedSetCombineRequestType(operation);

        return new(requestType, [.. args], false, array => [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);
    }

    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetCombineWithScoresAsync(SetOperation operation, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [];
        AddSortedSetCombineOptions(args, keys, weights, aggregate, operation);

        // Add WITHSCORES
        args.Add(WithScoresKeyword);

        RequestType requestType = GetSortedSetCombineRequestType(operation);

        return new(requestType, [.. args], false, dict =>
        {
            IEnumerable<SortedSetEntry> entries = dict.Select(kvp => new SortedSetEntry((ValkeyValue)kvp.Key, (double)kvp.Value));
            return [.. entries.OrderBy(e => e.Score).ThenBy(e => e.Element.ToString())];
        });
    }

    public static Cmd<long, long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, Aggregate aggregate = Aggregate.Sum)
        => SortedSetCombineAndStoreAsync(operation, destination, [first, second], null, aggregate);

    public static Cmd<long, long> SortedSetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey[] keys, double[]? weights = null, Aggregate aggregate = Aggregate.Sum)
    {
        List<GlideString> args = [destination.ToGlideString()];
        AddSortedSetCombineOptions(args, keys, weights, aggregate, operation);

        RequestType requestType = GetSortedSetCombineRequestType(operation, isStore: true);

        return Simple<long>(requestType, [.. args]);
    }

    public static Cmd<double, double> SortedSetIncrementAsync(ValkeyKey key, ValkeyValue member, double value)
    {
        List<GlideString> args = [key.ToGlideString(), value.ToGlideString(), member.ToGlideString()];
        return Simple<double>(RequestType.ZIncrBy, [.. args], false);
    }

    public static Cmd<long, long> SortedSetIntersectionLengthAsync(ValkeyKey[] keys, long limit = 0)
    {
        List<GlideString> args = [keys.Length.ToGlideString()];
        args.AddRange(keys.Select(key => key.ToGlideString()));

        if (limit > 0)
        {
            args.Add(LimitKeyword);
            args.Add(limit.ToGlideString());
        }

        return Simple<long>(RequestType.ZInterCard, [.. args]);
    }

    public static Cmd<long, long> SortedSetLengthByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None)
    {
        // Create lexicographical boundaries based on exclude flags
        LexBoundary minBoundary = exclude.HasFlag(Exclude.Start)
            ? LexBoundary.Exclusive(min)
            : LexBoundary.Inclusive(min);

        LexBoundary maxBoundary = exclude.HasFlag(Exclude.Stop)
            ? LexBoundary.Exclusive(max)
            : LexBoundary.Inclusive(max);

        List<GlideString> args = [key.ToGlideString(), ((string)minBoundary).ToGlideString(), ((string)maxBoundary).ToGlideString()];
        return Simple<long>(RequestType.ZLexCount, [.. args]);
    }

    public static Cmd<object[], double?[]> SortedSetScoresAsync(ValkeyKey key, ValkeyValue[] members)
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

    public static Cmd<object?, SortedSetEntry?> SortedSetBlockingPopAsync(ValkeyKey key, Order order, double timeout)
    {
        List<GlideString> args = [key.ToGlideString(), timeout.ToGlideString()];

        RequestType requestType = order == Order.Ascending ? RequestType.BZPopMin : RequestType.BZPopMax;

        return new(requestType, [.. args], true, response =>
        {
            if (response == null)
            {
                return null;
            }

            Object[] responseArray = (Object[])response;

            ValkeyValue member = (ValkeyValue)(GlideString)responseArray[1];
            double score = (double)responseArray[2];
            return new SortedSetEntry(member, score);
        }, allowConverterToHandleNull: true);
    }

    // Note: We keep count for the future TODO but disable the warning for now.
#pragma warning disable IDE0060 // Remove unused parameter
    public static Cmd<object?, SortedSetEntry[]> SortedSetBlockingPopAsync(ValkeyKey key, long count, Order order, double timeout)
    {
        // FUTURE TODO: support count > 1 requests
        List<GlideString> args = [key.ToGlideString(), timeout.ToGlideString()];
        RequestType requestType = order == Order.Ascending ? RequestType.BZPopMin : RequestType.BZPopMax;

        return new(requestType, [.. args], true, response =>
        {
            if (response == null)
            {
                return [];
            }

            Object[] responseArray = (Object[])response;

            // BZPOPMIN/BZPOPMAX returns [key, member, score] - only one element
            ValkeyValue member = (ValkeyValue)(GlideString)responseArray[1];
            double score = (double)responseArray[2];
            return [new SortedSetEntry(member, score)];
        }, allowConverterToHandleNull: true);
    }
#pragma warning restore IDE0060 // Remove unused parameter

    public static Cmd<object?, SortedSetPopResult> SortedSetBlockingPopAsync(ValkeyKey[] keys, long count, Order order, double timeout)
    {
        List<GlideString> args = [timeout.ToGlideString(), keys.Length.ToGlideString()];
        args.AddRange(keys.Select(key => key.ToGlideString()));

        args.Add(order == Order.Ascending ? MinKeyword : MaxKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.BZMPop, [.. args], true, HandleSortedSetPopResultResponse, allowConverterToHandleNull: true);
    }

    public static Cmd<object?, SortedSetEntry?> SortedSetPopAsync(ValkeyKey key, Order order = Order.Ascending)
    {
        RequestType requestType = order == Order.Ascending ? RequestType.ZPopMin : RequestType.ZPopMax;

        return new(requestType, [key.ToGlideString()], true, response =>
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
            ValkeyValue member = (ValkeyValue)firstEntry.Key;
            double score = (double)firstEntry.Value;
            return new SortedSetEntry(member, score);
        }, allowConverterToHandleNull: true);
    }

    // Note: We keep count for the future TODO but disable the warning for now.
#pragma warning disable IDE0060 // Remove unused parameter
    public static Cmd<Dictionary<GlideString, object>, SortedSetEntry[]> SortedSetPopAsync(ValkeyKey key, long count, Order order = Order.Ascending)
    {
        List<GlideString> args = [key.ToGlideString(), count.ToGlideString()];
        RequestType requestType = order == Order.Ascending ? RequestType.ZPopMin : RequestType.ZPopMax;

        return new(requestType, [.. args], false, response =>
        {
            SortedSetEntry[] entries = new SortedSetEntry[response.Count];
            int i = 0;
            foreach (KeyValuePair<GlideString, object> kvp in response)
            {
                ValkeyValue member = (ValkeyValue)kvp.Key;
                double score = (double)kvp.Value;
                entries[i] = new SortedSetEntry(member, score);
                i++;
            }

            return entries;
        });
    }
#pragma warning restore IDE0060 // Remove unused parameter

    public static Cmd<object?, SortedSetPopResult> SortedSetPopAsync(ValkeyKey[] keys, long count, Order order = Order.Ascending)
    {
        List<GlideString> args = [keys.Length.ToGlideString()];
        args.AddRange(keys.Select(key => key.ToGlideString()));

        args.Add(order == Order.Ascending ? MinKeyword : MaxKeyword);
        args.Add(CountKeyword);
        args.Add(count.ToGlideString());

        return new(RequestType.ZMPop, [.. args], true, HandleSortedSetPopResultResponse);
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

    public static Cmd<long, long> SortedSetRangeAndStoreAsync(
        ValkeyKey sourceKey,
        ValkeyKey destinationKey,
        ValkeyValue start,
        ValkeyValue stop,
        SortedSetOrder sortedSetOrder = SortedSetOrder.ByRank,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0,
        long? take = null)
    {
        List<GlideString> args = [destinationKey.ToGlideString(), sourceKey.ToGlideString()];
        string[] queryArgs;

        switch (sortedSetOrder)
        {
            case SortedSetOrder.ByRank:
                {
                    // Convert ValkeyValue to long for rank-based queries
                    long startIndex = start.HasValue ? (long)start : 0;
                    long stopIndex = stop.HasValue ? (long)stop : -1;

                    // For ByRank with skip/take, we need to adjust the start/stop indices
                    // since ZRANGESTORE with ByRank doesn't support LIMIT clause
                    if (skip > 0)
                    {
                        startIndex += skip;
                    }
                    if (take.HasValue)
                    {
                        stopIndex = startIndex + take.Value - 1;
                    }

                    RangeByIndex query = new(startIndex, stopIndex);
                    if (order == Order.Descending)
                    {
                        _ = query.SetReverse();
                    }
                    queryArgs = query.ToArgs();
                    break;
                }
            case SortedSetOrder.ByScore:
                {
                    // Convert ValkeyValue to double for score-based queries
                    double startScore = start.HasValue ? (double)start : double.NegativeInfinity;
                    double stopScore = stop.HasValue ? (double)stop : double.PositiveInfinity;

                    // Create score boundaries based on exclude flags
                    ScoreBoundary startBoundary = exclude.HasFlag(Exclude.Start)
                        ? ScoreBoundary.Exclusive(startScore)
                        : ScoreBoundary.Inclusive(startScore);

                    ScoreBoundary stopBoundary = exclude.HasFlag(Exclude.Stop)
                        ? ScoreBoundary.Exclusive(stopScore)
                        : ScoreBoundary.Inclusive(stopScore);

                    RangeByScore query = new(startBoundary, stopBoundary);
                    if (order == Order.Descending)
                    {
                        _ = query.SetReverse();
                    }
                    if (skip > 0 || take.HasValue)
                    {
                        long count = take ?? -1;
                        _ = query.SetLimit(skip, count);
                    }
                    queryArgs = query.ToArgs();
                    break;
                }
            case SortedSetOrder.ByLex:
                {
                    // Create lexicographical boundaries based on exclude flags
                    LexBoundary startBoundary = exclude.HasFlag(Exclude.Start)
                        ? LexBoundary.Exclusive(start)
                        : LexBoundary.Inclusive(start);

                    LexBoundary stopBoundary = exclude.HasFlag(Exclude.Stop)
                        ? LexBoundary.Exclusive(stop)
                        : LexBoundary.Inclusive(stop);

                    RangeByLex query = new(startBoundary, stopBoundary);
                    if (order == Order.Descending)
                    {
                        _ = query.SetReverse();
                    }
                    if (skip > 0 || take.HasValue)
                    {
                        long count = take ?? -1;
                        _ = query.SetLimit(skip, count);
                    }
                    queryArgs = query.ToArgs();
                    break;
                }
            default:
                throw new ArgumentException($"Unsupported SortedSetOrder: {sortedSetOrder}");
        }

        args.AddRange(queryArgs.Select(arg => arg.ToGlideString()));
        return Simple<long>(RequestType.ZRangeStore, [.. args]);
    }

    public static Cmd<long?, long?> SortedSetRankAsync(ValkeyKey key, ValkeyValue member, Order order = Order.Ascending)
    {
        RequestType requestType = order == Order.Ascending ? RequestType.ZRank : RequestType.ZRevRank;
        return new(requestType, [key.ToGlideString(), member.ToGlideString()], true, response => response);
    }

    public static Cmd<long, long> SortedSetRemoveRangeByValueAsync(ValkeyKey key, ValkeyValue min, ValkeyValue max, Exclude exclude = Exclude.None)
    {
        // Create lexicographical boundaries based on exclude flags
        LexBoundary minBoundary = exclude.HasFlag(Exclude.Start)
            ? LexBoundary.Exclusive(min)
            : LexBoundary.Inclusive(min);

        LexBoundary maxBoundary = exclude.HasFlag(Exclude.Stop)
            ? LexBoundary.Exclusive(max)
            : LexBoundary.Inclusive(max);

        List<GlideString> args = [key.ToGlideString(), ((string)minBoundary).ToGlideString(), ((string)maxBoundary).ToGlideString()];
        return Simple<long>(RequestType.ZRemRangeByLex, [.. args]);
    }

    public static Cmd<long, long> SortedSetRemoveRangeByRankAsync(ValkeyKey key, long start, long stop)
    {
        List<GlideString> args = [key.ToGlideString(), start.ToGlideString(), stop.ToGlideString()];
        return Simple<long>(RequestType.ZRemRangeByRank, [.. args]);
    }

    public static Cmd<long, long> SortedSetRemoveRangeByScoreAsync(ValkeyKey key, double start, double stop, Exclude exclude = Exclude.None)
    {
        // Create score boundaries based on exclude flags
        ScoreBoundary minBoundary = exclude.HasFlag(Exclude.Start)
            ? ScoreBoundary.Exclusive(start)
            : ScoreBoundary.Inclusive(start);

        ScoreBoundary maxBoundary = exclude.HasFlag(Exclude.Stop)
            ? ScoreBoundary.Exclusive(stop)
            : ScoreBoundary.Inclusive(stop);

        List<GlideString> args = [key.ToGlideString(), ((string)minBoundary).ToGlideString(), ((string)maxBoundary).ToGlideString()];
        return Simple<long>(RequestType.ZRemRangeByScore, [.. args]);
    }

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
}
