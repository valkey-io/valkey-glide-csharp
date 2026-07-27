// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Globalization;

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    #region Command Builders

    public static Cmd<long, long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, params ValkeyValue[] messageIds)
    {
        List<GlideString> args = [key, groupName];
        foreach (var id in messageIds)
        {
            args.Add(id);
        }
        return new(RequestType.XAck, [.. args], false, response => response);
    }

    public static Cmd<GlideString, ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, StreamAddOptions options)
    {
        List<GlideString> args = [key];
        args.AddRange(options.ToArgs());

        foreach (var pair in streamPairs)
        {
            args.Add(pair.Name);
            args.Add(pair.Value);
        }

        return new(RequestType.XAdd, [.. args], true, response => (ValkeyValue)response);
    }

    public static Cmd<object[], StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, TimeSpan minIdleTime, ValkeyValue startId, int? count)
    {
        List<GlideString> args = [key, groupName, consumerName, ToMilliseconds(minIdleTime).ToGlideString(), startId];
        if (count.HasValue)
        {
            args.Add("COUNT");
            args.Add(count.Value.ToGlideString());
        }
        return new(RequestType.XAutoClaim, [.. args], false, ConvertAutoClaimResult);
    }

    public static Cmd<object[], StreamAutoClaimJustIdResult> StreamAutoClaimJustIdAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, TimeSpan minIdleTime, ValkeyValue startId, int? count)
    {
        List<GlideString> args = [key, groupName, consumerName, ToMilliseconds(minIdleTime).ToGlideString(), startId];
        if (count.HasValue)
        {
            args.Add("COUNT");
            args.Add(count.Value.ToGlideString());
        }
        args.Add("JUSTID");
        return new(RequestType.XAutoClaim, [.. args], false, ConvertAutoClaimIdsOnlyResult);
    }

    public static Cmd<object, StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, TimeSpan minIdleTime, ValkeyValue[] messageIds, StreamClaimOptions? options = null)
        => StreamClaimAsync<object, StreamEntry[]>(key, groupName, consumerName, minIdleTime, messageIds, options, false, ConvertStreamEntryMapResponse);

    public static Cmd<object[], ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, TimeSpan minIdleTime, ValkeyValue[] messageIds, StreamClaimOptions? options = null)
        => StreamClaimAsync<object[], ValkeyValue[]>(key, groupName, consumerName, minIdleTime, messageIds, options, true, ConvertClaimIdsOnly);

    public static Cmd<object[], StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName)
        => new(RequestType.XInfoConsumers, [key, groupName], false, ConvertStreamConsumerInfo);

    public static Cmd<string, bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead)
    {
        List<GlideString> args = [key, groupName, position];

        if (entriesRead.HasValue)
        {
            args.Add("ENTRIESREAD");
            args.Add(entriesRead.Value.ToGlideString());
        }

        return new(RequestType.XGroupSetId, [.. args], false, response => response == "OK");
    }

    public static Cmd<object, bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => new(RequestType.XGroupCreateConsumer, [key, groupName, consumerName], false, response
            => response is bool b ? b : (long)response == 1);

    public static Cmd<string, bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, bool createStream, long? entriesRead)
    {
        List<GlideString> args = [key, groupName, position.IsNull ? "$" : (GlideString)position];

        if (createStream)
        {
            args.Add("MKSTREAM");
        }

        if (entriesRead.HasValue)
        {
            args.Add("ENTRIESREAD");
            args.Add(entriesRead.Value.ToGlideString());
        }

        return new(RequestType.XGroupCreate, [.. args], false, response => response == "OK");
    }

    public static Cmd<long, bool> StreamDeleteAsync(ValkeyKey key, ValkeyValue messageId)
        => Boolean<long>(RequestType.XDel, [key, messageId]);

    public static Cmd<long, long> StreamDeleteAsync(ValkeyKey key, params ValkeyValue[] messageIds)
    {
        List<GlideString> args = [key];
        foreach (var id in messageIds)
        {
            args.Add(id);
        }
        return new(RequestType.XDel, [.. args], false, response => response);
    }

    public static Cmd<long, long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => new(RequestType.XGroupDelConsumer, [key, groupName, consumerName], false, response => response);

    public static Cmd<bool, bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName)
        => new(RequestType.XGroupDestroy, [key, groupName], false, response => response);

    public static Cmd<object[], StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key)
        => new(RequestType.XInfoGroups, [key], false, ConvertStreamGroupInfo);

    public static Cmd<object, StreamInfo> StreamInfoAsync(ValkeyKey key)
        => new(RequestType.XInfoStream, [key], false, ConvertStreamInfo);

    public static Cmd<long, long> StreamLengthAsync(ValkeyKey key)
        => new(RequestType.XLen, [key], false, response => response);

    public static Cmd<object[], StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName)
        => new(RequestType.XPending, [key, groupName], false, ConvertStreamPendingInfo);

    public static Cmd<object[], StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue minId, ValkeyValue maxId, int count, ValkeyValue consumerName, TimeSpan? minIdleTime)
    {
        List<GlideString> args = [key, groupName];

        if (minIdleTime.HasValue)
        {
            args.Add("IDLE");
            args.Add(ToMilliseconds(minIdleTime.Value).ToGlideString());
        }

        args.Add(minId);
        args.Add(maxId);
        args.Add(count.ToGlideString());

        if (!consumerName.IsNull)
        {
            args.Add(consumerName);
        }

        return new(RequestType.XPending, [.. args], false, ConvertStreamPendingMessages);
    }

    public static Cmd<object, StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions options)
    {
        var range = options.Range;
        var start = range.Start.Value;
        var end = range.End.Value;

        if (options.Order == Order.Descending)
        {
            // Order of start and end IDs is reversed for REVRANGE.
            return new(RequestType.XRevRange, [key, end, start, .. options.ToArgs()], false, ConvertStreamEntryMapResponse);
        }

        return new(RequestType.XRange, [key, start, end, .. options.ToArgs()], false, ConvertStreamEntryMapResponse);
    }

    public static Cmd<object, StreamEntry[]> StreamReadAsync(StreamPosition position, StreamReadOptions options)
        => new(RequestType.XRead, BuildStreamReadArgs([position], options), false, ConvertSingleStreamReadResponse, allowConverterToHandleNull: true);

    public static Cmd<object, ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> positions, StreamReadOptions options)
        => new(RequestType.XRead, BuildStreamReadArgs([.. positions], options), false, ConvertMultiStreamReadResponse, allowConverterToHandleNull: true);

    public static Cmd<object, StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue group, ValkeyValue consumer, StreamReadGroupOptions options)
        => new(RequestType.XReadGroup, BuildStreamReadGroupArgs([position], group, consumer, options), false, ConvertSingleStreamReadResponse, allowConverterToHandleNull: true);

    public static Cmd<object, ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue group, ValkeyValue consumer, StreamReadGroupOptions options)
        => new(RequestType.XReadGroup, BuildStreamReadGroupArgs([.. positions], group, consumer, options), false, ConvertMultiStreamReadResponse, allowConverterToHandleNull: true);

    // TODO SER only
    public static Cmd<long, long> StreamTrimAsync(ValkeyKey key, long? maxLength, ValkeyValue minId, bool useApproximateMaxLength, long? limit)
    {
        List<GlideString> args = [key];

        if (maxLength.HasValue)
        {
            args.Add("MAXLEN");

            if (useApproximateMaxLength)
            {
                args.Add("~");
            }

            args.Add(maxLength.Value.ToGlideString());
        }
        else if (!minId.IsNull)
        {
            args.Add("MINID");
            if (useApproximateMaxLength)
            {
                args.Add("~");
            }

            args.Add(minId);
        }

        if (limit.HasValue && useApproximateMaxLength)
        {
            args.Add("LIMIT");
            args.Add(limit.Value.ToGlideString());
        }

        return new(RequestType.XTrim, [.. args], false, response => response);
    }

    #endregion

    #region Response Converters

    private static StreamAutoClaimResult ConvertAutoClaimResult(object[] response)
    {
        var nextStartId = (ValkeyValue)(GlideString)response[0];
        var entries = ConvertStreamEntryMapResponse(response[1]);
        var deletedIds = response.Length > 2 && response[2] is object[] delArr ? delArr.Select(id => (ValkeyValue)(GlideString)id).ToArray() : [];
        return new StreamAutoClaimResult(nextStartId, entries, deletedIds);
    }

    private static StreamAutoClaimJustIdResult ConvertAutoClaimIdsOnlyResult(object[] response)
    {
        var nextStartId = (ValkeyValue)(GlideString)response[0];
        var claimedIds = ((object[])response[1]).Select(id => (ValkeyValue)(GlideString)id).ToArray();
        var deletedIds = response.Length > 2 && response[2] is object[] delArr ? delArr.Select(id => (ValkeyValue)(GlideString)id).ToArray() : [];
        return new StreamAutoClaimJustIdResult(nextStartId, claimedIds, deletedIds);
    }

    private static ValkeyValue[] ConvertClaimIdsOnly(object[] response)
    {
        var result = new ValkeyValue[response.Length];
        for (int i = 0; i < response.Length; i++)
        {
            result[i] = (ValkeyValue)(GlideString)response[i];
        }
        return result;
    }

    /// <summary>
    /// Converts a multi-stream read response (XREAD and XREADGROUP).
    /// </summary>
    internal static ValkeyStream[] ConvertMultiStreamReadResponse(object response)
    {
        // Null when BLOCK times out or no undelivered entries exist.
        if (response is null)
        {
            return [];
        }

        var result = new List<ValkeyStream>();
        foreach (var streamKvp in (Dictionary<GlideString, object>)response)
        {
            var streamKey = new ValkeyKey(streamKvp.Key);
            var entries = ConvertStreamEntryMapResponse((Dictionary<GlideString, object>)streamKvp.Value);
            result.Add(new ValkeyStream(streamKey, entries));
        }

        return [.. result];
    }

    /// <summary>
    /// Converts a single-stream read response (XREAD and XREADGROUP).
    /// </summary>
    internal static StreamEntry[] ConvertSingleStreamReadResponse(object response)
    {
        var streams = ConvertMultiStreamReadResponse(response);
        return streams.Length > 0 ? streams[0].Entries : [];
    }

    private static StreamConsumerInfo[] ConvertStreamConsumerInfo(object[] response)
    {
        var result = new StreamConsumerInfo[response.Length];
        for (int i = 0; i < response.Length; i++)
        {
            var name = "";
            var pending = 0;
            var idle = 0L;

            if (response[i] is Dictionary<GlideString, object> dict)
            {
                foreach (var kvp in dict)
                {
                    var key = kvp.Key.ToString();
                    var value = kvp.Value;
                    switch (key)
                    {
                        case "name": name = ((GlideString)value).ToString(); break;
                        case "pending": pending = value is GlideString gs ? int.Parse(gs.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                        case "idle": idle = value is GlideString gs2 ? long.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        default: break;
                    }
                }
            }
            else
            {
                var consumerData = (object[])response[i];
                for (int j = 0; j < consumerData.Length; j += 2)
                {
                    var key = ((GlideString)consumerData[j]).ToString();
                    var value = consumerData[j + 1];
                    switch (key)
                    {
                        case "name": name = ((GlideString)value).ToString(); break;
                        case "pending": pending = value is GlideString gs ? int.Parse(gs.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                        case "idle": idle = value is GlideString gs2 ? long.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        default: break;
                    }
                }
            }
            result[i] = new StreamConsumerInfo(name, pending, idle);
        }
        return result;
    }

    private static StreamEntry[] ConvertStreamEntries(object[] entries)
    {
        var result = new StreamEntry[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            var entry = (object[])entries[i];
            var id = (GlideString)entry[0];

            if (entry[1] is not object[] fields)
            {
                result[i] = new StreamEntry(id, []);
                continue;
            }

            var valuesList = new List<NameValueEntry>();

            // Check if this is the nested array format (each element is [field, value])
            if (fields.Length > 0 && fields[0] is object[] firstElement && firstElement.Length == 2)
            {
                // Handle nested array structure where each field-value pair is a separate array
                foreach (var field in fields)
                {
                    if (field is object[] fieldValuePair && fieldValuePair.Length == 2)
                    {
                        valuesList.Add(new NameValueEntry(
                            (GlideString)fieldValuePair[0],
                            (GlideString)fieldValuePair[1]
                        ));
                    }
                }
            }
            else
            {
                // Handle flattened array format (field1, value1, field2, value2, ...)
                for (int j = 0; j < fields.Length; j += 2)
                {
                    if (j + 1 < fields.Length)
                    {
                        valuesList.Add(new NameValueEntry(
                            (GlideString)fields[j],
                            (GlideString)fields[j + 1]
                        ));
                    }
                }
            }

            result[i] = new StreamEntry(id, [.. valuesList]);
        }

        return result;
    }

    /// <summary>
    /// Converts a stream entry map response (XREAD, XREADGROUP, XRANGE, XREVRANGE, XCLAIM, and XAUTOCLAIM).
    /// </summary>
    private static StreamEntry[] ConvertStreamEntryMapResponse(object response)
    {
        var entries = new List<StreamEntry>();
        foreach (var entryKvp in (Dictionary<GlideString, object>)response)
        {
            // Pending messages that have been acknowledged/deleted have nil field values.
            if (entryKvp.Value is not object[] outerArray || outerArray.Length == 0)
            {
                continue;
            }

            var entryId = entryKvp.Key;

            var values = new NameValueEntry[outerArray.Length];
            for (int i = 0; i < outerArray.Length; i++)
            {
                var fieldValues = (object[])outerArray[i];
                values[i] = new NameValueEntry(
                    (GlideString)fieldValues[0],
                    (GlideString)fieldValues[1]
                );
            }

            entries.Add(new StreamEntry(entryId, values));
        }

        return [.. entries];
    }

    private static StreamGroupInfo[] ConvertStreamGroupInfo(object[] response)
    {
        var result = new StreamGroupInfo[response.Length];
        for (int i = 0; i < response.Length; i++)
        {
            var name = "";
            var consumers = 0;
            var pending = 0;
            var lastDeliveredId = default(ValkeyValue);
            var entriesRead = (long?)null;
            var lag = (long?)null;

            if (response[i] is Dictionary<GlideString, object> dict)
            {
                foreach (var kvp in dict)
                {
                    var key = kvp.Key.ToString();
                    var value = kvp.Value;
                    switch (key)
                    {
                        case "name": name = ((GlideString)value).ToString(); break;
                        case "consumers": consumers = value is GlideString gs ? int.Parse(gs.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                        case "pending": pending = value is GlideString gs2 ? int.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                        case "last-delivered-id": lastDeliveredId = (ValkeyValue)(GlideString)value; break;
                        case "entries-read": entriesRead = value is null ? null : value is GlideString gs3 ? long.Parse(gs3.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        case "lag": lag = value is null ? null : value is GlideString gs4 ? long.Parse(gs4.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        default: break;
                    }
                }
            }
            else
            {
                var groupData = (object[])response[i];
                for (int j = 0; j < groupData.Length; j += 2)
                {
                    var key = ((GlideString)groupData[j]).ToString();
                    var value = groupData[j + 1];
                    switch (key)
                    {
                        case "name": name = ((GlideString)value).ToString(); break;
                        case "consumers": consumers = value is GlideString gs ? int.Parse(gs.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                        case "pending": pending = value is GlideString gs2 ? int.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                        case "last-delivered-id": lastDeliveredId = (ValkeyValue)(GlideString)value; break;
                        case "entries-read": entriesRead = value is null ? null : value is GlideString gs3 ? long.Parse(gs3.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        case "lag": lag = value is null ? null : value is GlideString gs4 ? long.Parse(gs4.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        default: break;
                    }
                }
            }
            result[i] = new StreamGroupInfo(name, consumers, pending, lastDeliveredId, entriesRead, lag);
        }
        return result;
    }

    private static StreamInfo ConvertStreamInfo(object response)
    {
        var length = 0;
        var radixTreeKeys = 0;
        var radixTreeNodes = 0;
        var groups = 0;
        var firstEntry = default(StreamEntry);
        var lastEntry = default(StreamEntry);
        var lastGeneratedId = default(ValkeyValue);

        if (response is Dictionary<GlideString, object> dict)
        {
            foreach (var kvp in dict)
            {
                var key = kvp.Key.ToString();
                var value = kvp.Value;
                switch (key)
                {
                    case "length": length = value is GlideString gs ? int.Parse(gs.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                    case "radix-tree-keys": radixTreeKeys = value is GlideString gs2 ? int.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                    case "radix-tree-nodes": radixTreeNodes = value is GlideString gs3 ? int.Parse(gs3.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                    case "groups": groups = value is GlideString gs4 ? int.Parse(gs4.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                    case "first-entry": firstEntry = value is object[] arr ? ConvertStreamEntries([arr])[0] : default; break;
                    case "last-entry": lastEntry = value is object[] arr2 ? ConvertStreamEntries([arr2])[0] : default; break;
                    case "last-generated-id": lastGeneratedId = (ValkeyValue)(GlideString)value; break;
                    default: break;
                }
            }
        }
        else
        {
            var infoArray = (object[])response;
            for (int i = 0; i < infoArray.Length; i += 2)
            {
                var key = ((GlideString)infoArray[i]).ToString();
                var value = infoArray[i + 1];
                switch (key)
                {
                    case "length": length = value is GlideString gs ? int.Parse(gs.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                    case "radix-tree-keys": radixTreeKeys = value is GlideString gs2 ? int.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                    case "radix-tree-nodes": radixTreeNodes = value is GlideString gs3 ? int.Parse(gs3.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                    case "groups": groups = value is GlideString gs4 ? int.Parse(gs4.ToString(), CultureInfo.InvariantCulture) : (int)(long)value; break;
                    case "first-entry": firstEntry = value is object[] arr ? ConvertStreamEntries([arr])[0] : default; break;
                    case "last-entry": lastEntry = value is object[] arr2 ? ConvertStreamEntries([arr2])[0] : default; break;
                    case "last-generated-id": lastGeneratedId = (ValkeyValue)(GlideString)value; break;
                    default: break;
                }
            }
        }

        return new StreamInfo(length, radixTreeKeys, radixTreeNodes, groups, firstEntry, lastEntry, lastGeneratedId);
    }

    private static StreamPendingInfo ConvertStreamPendingInfo(object[] response)
    {
        var pendingCount = response[0] is GlideString gs ? int.Parse(gs.ToString(), CultureInfo.InvariantCulture) : (int)(long)response[0];
        var lowestId = response[1] is null ? default : (ValkeyValue)(GlideString)response[1];
        var highestId = response[2] is null ? default : (ValkeyValue)(GlideString)response[2];
        var consumersArray = response[3] as object[];
        var consumers = consumersArray is null ? [] : new StreamConsumer[consumersArray.Length];
        if (consumersArray is not null)
        {
            for (int i = 0; i < consumersArray.Length; i++)
            {
                var consumerData = (object[])consumersArray[i];
                var count = consumerData[1] is GlideString gs2 ? int.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (int)(long)consumerData[1];
                consumers[i] = new StreamConsumer((ValkeyValue)(GlideString)consumerData[0], count);
            }
        }
        return new StreamPendingInfo(pendingCount, lowestId, highestId, consumers);
    }

    private static StreamPendingMessageInfo[] ConvertStreamPendingMessages(object[] response)
    {
        var result = new StreamPendingMessageInfo[response.Length];
        for (int i = 0; i < response.Length; i++)
        {
            var msgData = (object[])response[i];
            result[i] = new StreamPendingMessageInfo(
                (ValkeyValue)(GlideString)msgData[0],
                (ValkeyValue)(GlideString)msgData[1],
                (long)msgData[2],
                (int)(long)msgData[3]
            );
        }
        return result;
    }

    #endregion

    #region Argument Builders

    private static GlideString[] BuildStreamReadArgs(StreamPosition[] positions, StreamReadOptions options)
    {
        List<GlideString> args = [];

        if (options.Count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT);
            args.Add(options.Count.Value.ToGlideString());
        }

        if (options.Block.HasValue)
        {
            args.Add(ValkeyLiterals.BLOCK);
            args.Add(ToMilliseconds(options.Block.Value).ToGlideString());
        }

        args.Add(ValkeyLiterals.STREAMS);
        foreach (var sp in positions)
        {
            args.Add(sp.Key);
        }
        foreach (var sp in positions)
        {
            args.Add(sp.Position);
        }

        return [.. args];
    }

    private static GlideString[] BuildStreamReadGroupArgs(StreamPosition[] positions, ValkeyValue group, ValkeyValue consumer, StreamReadGroupOptions options)
    {
        List<GlideString> args = [ValkeyLiterals.GROUP, group, consumer];

        if (options.Count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT);
            args.Add(options.Count.Value.ToGlideString());
        }

        if (options.Block.HasValue)
        {
            args.Add(ValkeyLiterals.BLOCK);
            args.Add(ToMilliseconds(options.Block.Value).ToGlideString());
        }

        if (options.NoAck)
        {
            args.Add(ValkeyLiterals.NOACK);
        }

        args.Add(ValkeyLiterals.STREAMS);
        foreach (var sp in positions)
        {
            args.Add(sp.Key);
        }
        foreach (var sp in positions)
        {
            args.Add(sp.Position);
        }

        return [.. args];
    }

    private static Cmd<TResponse, TResult> StreamClaimAsync<TResponse, TResult>(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, TimeSpan minIdleTime, ValkeyValue[] messageIds, StreamClaimOptions? options, bool justId, Func<TResponse, TResult> converter)
    {
        List<GlideString> args = [key, groupName, consumerName, ToMilliseconds(minIdleTime).ToGlideString()];
        foreach (var id in messageIds)
        {
            args.Add(id);
        }

        if (options is not null)
        {
            if (options.Idle.HasValue)
            {
                args.Add(ValkeyLiterals.IDLE);
                args.Add(ToMilliseconds(options.Idle.Value).ToGlideString());
            }

            if (options.IdleUnix.HasValue)
            {
                args.Add(ValkeyLiterals.TIME);
                args.Add(options.IdleUnix.Value.ToUnixTimeMilliseconds().ToGlideString());
            }

            if (options.RetryCount.HasValue)
            {
                args.Add(ValkeyLiterals.RETRYCOUNT);
                args.Add(options.RetryCount.Value.ToGlideString());
            }

            if (options.Force)
            {
                args.Add(ValkeyLiterals.FORCE);
            }
        }

        if (justId)
        {
            args.Add("JUSTID");
        }
        return new(RequestType.XClaim, [.. args], false, converter);
    }

    #endregion
}
