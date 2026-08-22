// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    #region Command Builders

    public static Cmd<object[], ValkeyValue[]?> ListBlockingLeftPop(ValkeyKey[] keys, TimeSpan timeout)
        => new(RequestType.BLPop, [.. keys, ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString()], true, array =>
            array is null ? null : [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<GlideString, ValkeyValue> ListBlockingMove(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout)
        => new(RequestType.BLMove, [source, destination, sourceSide.ToLiteral(), destinationSide.ToLiteral(), ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString()], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs, allowConverterToHandleNull: true);

    public static Cmd<Dictionary<GlideString, object>, ListPopResult> ListBlockingPop(ValkeyKey[] keys, ListSide side, TimeSpan timeout)
        => new(RequestType.BLMPop, [ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString(), keys.Length.ToGlideString(), .. keys.ToGlideStrings(), side.ToLiteral()], true, dict =>
            dict is null ? ListPopResult.Null : ConvertDictToListPopResult(dict), allowConverterToHandleNull: true);

    public static Cmd<Dictionary<GlideString, object>, ListPopResult> ListBlockingPop(ValkeyKey[] keys, ListSide side, long count, TimeSpan timeout)
        => new(RequestType.BLMPop, [ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString(), keys.Length.ToGlideString(), .. keys.ToGlideStrings(), side.ToLiteral(), ValkeyLiterals.COUNT, count.ToGlideString()], true, dict =>
            dict is null ? ListPopResult.Null : ConvertDictToListPopResult(dict), allowConverterToHandleNull: true);

    public static Cmd<object[], ValkeyValue[]?> ListBlockingRightPop(ValkeyKey[] keys, TimeSpan timeout)
        => new(RequestType.BRPop, [.. keys, ToNonNegativeDoubleSecs(timeout, nameof(timeout)).ToGlideString()], true, array =>
            array is null ? null : [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<GlideString, ValkeyValue> ListGetByIndex(ValkeyKey key, long index)
        => new(RequestType.LIndex, [key, index.ToGlideString()], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs, allowConverterToHandleNull: true);

    public static Cmd<long, long> ListInsertAfter(ValkeyKey key, ValkeyValue pivot, ValkeyValue value)
        => Simple<long>(RequestType.LInsert, [key, ValkeyLiterals.AFTER, pivot, value]);

    public static Cmd<long, long> ListInsertBefore(ValkeyKey key, ValkeyValue pivot, ValkeyValue value)
        => Simple<long>(RequestType.LInsert, [key, ValkeyLiterals.BEFORE, pivot, value]);

    public static Cmd<GlideString, ValkeyValue> ListLeftPop(ValkeyKey key)
        => new(RequestType.LPop, [key], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs, allowConverterToHandleNull: true);

    public static Cmd<object[], ValkeyValue[]?> ListLeftPop(ValkeyKey key, long count)
        => new(RequestType.LPop, [key, count.ToGlideString()], true, array =>
            array is null ? null : [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<Dictionary<GlideString, object>, ListPopResult> ListLeftPop(ValkeyKey[] keys, long count)
        => new(RequestType.LMPop, [keys.Length.ToGlideString(), .. keys.ToGlideStrings(), ValkeyLiterals.LEFT, ValkeyLiterals.COUNT, count.ToGlideString()], true, dict =>
            dict is null ? ListPopResult.Null : ConvertDictToListPopResult(dict), allowConverterToHandleNull: true);

    public static Cmd<long, long> ListLeftPush(ValkeyKey key, ValkeyValue value, When when = When.Always)
    {
        RequestType requestType = when == When.Exists ? RequestType.LPushX : RequestType.LPush;
        return Simple<long>(requestType, [key, value]);
    }

    public static Cmd<long, long> ListLeftPush(ValkeyKey key, ValkeyValue[] values, When when = When.Always)
    {
        RequestType requestType = when == When.Exists ? RequestType.LPushX : RequestType.LPush;
        return Simple<long>(requestType, [key, .. values.ToGlideStrings()]);
    }

    public static Cmd<long, long> ListLeftPush(ValkeyKey key, ValkeyValue[] values)
        => Simple<long>(RequestType.LPush, [key, .. values.ToGlideStrings()]);

    public static Cmd<long, long> ListLength(ValkeyKey key)
        => Simple<long>(RequestType.LLen, [key]);

    public static Cmd<GlideString, ValkeyValue> ListMove(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide)
        => new(RequestType.LMove, [sourceKey, destinationKey, sourceSide.ToLiteral(), destinationSide.ToLiteral()], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs, allowConverterToHandleNull: true);

    public static Cmd<long?, long> ListPosition(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0)
    {
        List<GlideString> args = [key, element];
        if (rank != 1)
        {
            args.AddRange([ValkeyLiterals.RANK, rank.ToGlideString()]);
        }
        if (maxLength != 0)
        {
            args.AddRange([ValkeyLiterals.MAXLEN, maxLength.ToGlideString()]);
        }
        // Convert null to -1L, similar to how other commands handle their null cases
        return new(RequestType.LPos, [.. args], true, response => response is null ? -1L : (long)response, allowConverterToHandleNull: true);
    }

    public static Cmd<object[], long[]> ListPositions(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0)
    {
        List<GlideString> args = [key, element, ValkeyLiterals.COUNT, count.ToGlideString()];
        if (rank != 1)
        {
            args.AddRange([ValkeyLiterals.RANK, rank.ToGlideString()]);
        }
        if (maxLength != 0)
        {
            args.AddRange([ValkeyLiterals.MAXLEN, maxLength.ToGlideString()]);
        }
        return new(RequestType.LPos, [.. args], false, array => [.. array.Cast<long>()]);
    }

    public static Cmd<object[], ValkeyValue[]> ListRange(ValkeyKey key, long start = 0, long stop = -1)
        => new(RequestType.LRange, [key, start.ToGlideString(), stop.ToGlideString()], false, array =>
            [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<long, long> ListRemove(ValkeyKey key, ValkeyValue value, long count = 0)
        => Simple<long>(RequestType.LRem, [key, count.ToGlideString(), value]);

    public static Cmd<GlideString, ValkeyValue> ListRightPop(ValkeyKey key)
        => new(RequestType.RPop, [key], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs, allowConverterToHandleNull: true);

    public static Cmd<object[], ValkeyValue[]?> ListRightPop(ValkeyKey key, long count)
        => new(RequestType.RPop, [key, count.ToGlideString()], true, array =>
            array is null ? null : [.. array.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<Dictionary<GlideString, object>, ListPopResult> ListRightPop(ValkeyKey[] keys, long count)
        => new(RequestType.LMPop, [keys.Length.ToGlideString(), .. keys.ToGlideStrings(), ValkeyLiterals.RIGHT, ValkeyLiterals.COUNT, count.ToGlideString()], true, dict =>
            dict is null ? ListPopResult.Null : ConvertDictToListPopResult(dict), allowConverterToHandleNull: true);

    public static Cmd<long, long> ListRightPush(ValkeyKey key, ValkeyValue value, When when = When.Always)
    {
        RequestType requestType = when == When.Exists ? RequestType.RPushX : RequestType.RPush;
        return Simple<long>(requestType, [key, value]);
    }

    public static Cmd<long, long> ListRightPush(ValkeyKey key, ValkeyValue[] values, When when = When.Always)
    {
        RequestType requestType = when == When.Exists ? RequestType.RPushX : RequestType.RPush;
        return Simple<long>(requestType, [key, .. values.ToGlideStrings()]);
    }

    public static Cmd<long, long> ListRightPush(ValkeyKey key, ValkeyValue[] values)
        => Simple<long>(RequestType.RPush, [key, .. values.ToGlideStrings()]);

    public static Cmd<string, ValkeyValue> ListSetByIndex(ValkeyKey key, long index, ValkeyValue value)
        => Ok(RequestType.LSet, [key, index.ToGlideString(), value]);

    public static Cmd<string, ValkeyValue> ListTrim(ValkeyKey key, long start, long stop)
        => Ok(RequestType.LTrim, [key, start.ToGlideString(), stop.ToGlideString()]);

    #endregion Command Builders

    #region Response Converters

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
        ValkeyKey key = kvp.Key.ToString();
        object[] valuesArray = (object[])kvp.Value;
        ValkeyValue[] values = valuesArray?.Cast<GlideString>().Select(gs => (ValkeyValue)gs).ToArray() ?? [];

        return new ListPopResult(key, values);
    }

    #endregion Response Converters
}
