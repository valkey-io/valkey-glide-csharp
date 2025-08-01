// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Constants;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, ValkeyValue> ListLeftPopAsync(ValkeyKey key)
        => new(RequestType.LPop, [key], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs);

    public static Cmd<object[], ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count)
        => new(RequestType.LPop, [key, count.ToGlideString()], true, array =>
            array is null ? null : [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<long, long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always)
    {
        RequestType requestType = when == When.Exists ? RequestType.LPushX : RequestType.LPush;
        return Simple<long>(requestType, [key, value]);
    }

    public static Cmd<long, long> ListLeftPushAsync(ValkeyKey key, ValkeyValue[] values, When when = When.Always)
    {
        RequestType requestType = when == When.Exists ? RequestType.LPushX : RequestType.LPush;
        return Simple<long>(requestType, [key.ToGlideString(), .. values.ToGlideStrings()]);
    }

    public static Cmd<long, long> ListLeftPushAsync(ValkeyKey key, ValkeyValue[] values)
        => Simple<long>(RequestType.LPush, [key.ToGlideString(), .. values.ToGlideStrings()]);

    public static Cmd<GlideString, ValkeyValue> ListRightPopAsync(ValkeyKey key)
        => new(RequestType.RPop, [key], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs);

    public static Cmd<object[], ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count)
        => new(RequestType.RPop, [key, count.ToGlideString()], true, array =>
            array is null ? null : [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<long, long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always)
    {
        RequestType requestType = when == When.Exists ? RequestType.RPushX : RequestType.RPush;
        return Simple<long>(requestType, [key, value]);
    }

    public static Cmd<long, long> ListRightPushAsync(ValkeyKey key, ValkeyValue[] values, When when = When.Always)
    {
        RequestType requestType = when == When.Exists ? RequestType.RPushX : RequestType.RPush;
        return Simple<long>(requestType, [key.ToGlideString(), .. values.ToGlideStrings()]);
    }

    public static Cmd<long, long> ListRightPushAsync(ValkeyKey key, ValkeyValue[] values)
        => Simple<long>(RequestType.RPush, [key.ToGlideString(), .. values.ToGlideStrings()]);

    public static Cmd<long, long> ListLengthAsync(ValkeyKey key)
        => Simple<long>(RequestType.LLen, [key]);

    public static Cmd<long, long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0)
        => Simple<long>(RequestType.LRem, [key, count.ToGlideString(), value]);

    public static Cmd<string, string> ListTrimAsync(ValkeyKey key, long start, long stop)
        => Simple<string>(RequestType.LTrim, [key, start.ToGlideString(), stop.ToGlideString()]);

    public static Cmd<object[], ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start = 0, long stop = -1)
        => new(RequestType.LRange, [key, start.ToGlideString(), stop.ToGlideString()], false, array =>
            [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<Dictionary<GlideString, object>, ListPopResult> ListLeftPopAsync(ValkeyKey[] keys, long count)
        => new(RequestType.LMPop, [keys.Length.ToGlideString(), .. keys.ToGlideStrings(), Constants.LeftKeyword, Constants.CountKeyword, count.ToGlideString()], true, dict =>
            dict is null ? ListPopResult.Null : ConvertDictToListPopResult(dict));

    public static Cmd<Dictionary<GlideString, object>, ListPopResult> ListRightPopAsync(ValkeyKey[] keys, long count)
        => new(RequestType.LMPop, [keys.Length.ToGlideString(), .. keys.ToGlideStrings(), Constants.RightKeyword, Constants.CountKeyword, count.ToGlideString()], true, dict =>
            dict is null ? ListPopResult.Null : ConvertDictToListPopResult(dict));

    public static Cmd<GlideString, ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index)
        => new(RequestType.LIndex, [key, index.ToGlideString()], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs);

    public static Cmd<long, long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value)
        => Simple<long>(RequestType.LInsert, [key, Constants.BeforeKeyword, pivot, value]);

    public static Cmd<long, long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value)
        => Simple<long>(RequestType.LInsert, [key, Constants.AfterKeyword, pivot, value]);

    public static Cmd<GlideString, ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide)
        => new(RequestType.LMove, [sourceKey, destinationKey, sourceSide.ToLiteral(), destinationSide.ToLiteral()], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs);

    public static Cmd<long?, long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0)
    {
        List<GlideString> args = new List<GlideString> { key, element };
        if (rank != 1)
        {
            args.AddRange([Constants.RankKeyword, rank.ToGlideString()]);
        }
        if (maxLength != 0)
        {
            args.AddRange([Constants.MaxLenKeyword, maxLength.ToGlideString()]);
        }
        // Use custom null handling to convert null to -1L
        return new(RequestType.LPos, [.. args], false, response => response is null ? -1L : (long)response, allowConverterToHandleNull: true);
    }

    public static Cmd<object[], long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0)
    {
        List<GlideString> args = new List<GlideString> { key, element, Constants.CountKeyword, count.ToGlideString() };
        if (rank != 1)
        {
            args.AddRange([Constants.RankKeyword, rank.ToGlideString()]);
        }
        if (maxLength != 0)
        {
            args.AddRange([Constants.MaxLenKeyword, maxLength.ToGlideString()]);
        }
        return new(RequestType.LPos, [.. args], false, array => [.. array.Cast<long>()]);
    }

    public static Cmd<string, string> ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value)
        => Simple<string>(RequestType.LSet, [key, index.ToGlideString(), value]);

    public static Cmd<object[], ValkeyValue[]?> ListBlockingLeftPopAsync(ValkeyKey[] keys, TimeSpan timeout)
        => new(RequestType.BLPop, [.. keys.ToGlideStrings(), timeout.TotalSeconds.ToGlideString()], true, array =>
            array is null ? null : [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<object[], ValkeyValue[]?> ListBlockingRightPopAsync(ValkeyKey[] keys, TimeSpan timeout)
        => new(RequestType.BRPop, [.. keys.ToGlideStrings(), timeout.TotalSeconds.ToGlideString()], true, array =>
            array is null ? null : [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<GlideString, ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout)
        => new(RequestType.BLMove, [source, destination, sourceSide.ToLiteral(), destinationSide.ToLiteral(), timeout.TotalSeconds.ToGlideString()], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs);

    public static Cmd<Dictionary<GlideString, object>, ListPopResult> ListBlockingPopAsync(ValkeyKey[] keys, ListSide side, TimeSpan timeout)
        => new(RequestType.BLMPop, [timeout.TotalSeconds.ToGlideString(), keys.Length.ToGlideString(), .. keys.ToGlideStrings(), side.ToLiteral()], true, dict =>
            dict is null ? ListPopResult.Null : ConvertDictToListPopResult(dict));

    public static Cmd<Dictionary<GlideString, object>, ListPopResult> ListBlockingPopAsync(ValkeyKey[] keys, ListSide side, long count, TimeSpan timeout)
        => new(RequestType.BLMPop, [timeout.TotalSeconds.ToGlideString(), keys.Length.ToGlideString(), .. keys.ToGlideStrings(), side.ToLiteral(), Constants.CountKeyword, count.ToGlideString()], true, dict =>
            dict is null ? ListPopResult.Null : ConvertDictToListPopResult(dict));

    private static ListPopResult ConvertDictToListPopResult(Dictionary<GlideString, object> dict)
    {
        if (dict == null || dict.Count == 0)
        {
            return ListPopResult.Null;
        }

        // LMPOP returns a dictionary with one key-value pair where:
        // - key is the list name that was popped from
        // - value is an array of the popped elements
        KeyValuePair<GlideString, object> kvp = dict.First();
        ValkeyKey key = new ValkeyKey(kvp.Key.ToString());
        object[] valuesArray = (object[])kvp.Value;
        ValkeyValue[] values = valuesArray?.Cast<GlideString>().Select(gs => (ValkeyValue)gs).ToArray() ?? [];

        return new ListPopResult(key, values);
    }
}
