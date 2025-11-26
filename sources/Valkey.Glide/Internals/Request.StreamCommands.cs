// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue messageId, long? maxLength, ValkeyValue minId, bool useApproximateMaxLength, NameValueEntry[] streamPairs, long? limit, bool noMakeStream)
    {
        // Validate mutually exclusive parameters
        if (maxLength.HasValue && !minId.IsNull)
        {
            throw new ArgumentException("maxLength and minId are mutually exclusive. Only one can be specified.");
        }

        List<GlideString> args = [key.ToGlideString()];

        // Add NOMKSTREAM if specified
        if (noMakeStream)
        {
            args.Add("NOMKSTREAM".ToGlideString());
        }

        // Add trimming options (MAXLEN or MINID)
        if (maxLength.HasValue)
        {
            args.Add("MAXLEN".ToGlideString());
            if (useApproximateMaxLength)
            {
                args.Add("~".ToGlideString());
            }
            args.Add(maxLength.Value.ToGlideString());

            // Add LIMIT if specified and approximate trimming is used
            if (limit.HasValue && useApproximateMaxLength)
            {
                args.Add("LIMIT".ToGlideString());
                args.Add(limit.Value.ToGlideString());
            }
        }
        else if (!minId.IsNull)
        {
            args.Add("MINID".ToGlideString());
            if (useApproximateMaxLength)
            {
                args.Add("~".ToGlideString());
            }
            args.Add(minId.ToGlideString());

            // Add LIMIT if specified and approximate trimming is used
            if (limit.HasValue && useApproximateMaxLength)
            {
                args.Add("LIMIT".ToGlideString());
                args.Add(limit.Value.ToGlideString());
            }
        }

        // Add message ID
        args.Add(messageId.IsNull ? "*".ToGlideString() : messageId.ToGlideString());

        // Add field-value pairs
        foreach (var pair in streamPairs)
        {
            args.Add(pair.Name.ToGlideString());
            args.Add(pair.Value.ToGlideString());
        }

        return new(RequestType.XAdd, [.. args], true, response => (ValkeyValue)response);
    }

    public static Cmd<object, StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count)
    {
        return StreamReadAsync([new StreamPosition(key, position)], count, ConvertSingleStreamRead);
    }

    public static Cmd<object, ValkeyStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? count)
    {
        return StreamReadAsync(streamPositions, count, ConvertMultiStreamRead);
    }

    private static Cmd<object, TResult> StreamReadAsync<TResult>(StreamPosition[] streamPositions, int? count, Func<object, TResult> converter)
    {
        List<GlideString> args = [];

        if (count.HasValue)
        {
            args.Add("COUNT".ToGlideString());
            args.Add(count.Value.ToGlideString());
        }

        args.Add("STREAMS".ToGlideString());
        foreach (var sp in streamPositions)
        {
            args.Add(sp.Key.ToGlideString());
        }
        foreach (var sp in streamPositions)
        {
            args.Add(sp.Position.ToGlideString());
        }

        return new(RequestType.XRead, [.. args], false, converter, allowConverterToHandleNull: true);
    }

    public static Cmd<object, StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue start, ValkeyValue end, int? count, Order order)
    {
        List<GlideString> args = [key.ToGlideString(), start.ToGlideString(), end.ToGlideString()];

        if (count.HasValue)
        {
            args.Add("COUNT".ToGlideString());
            args.Add(count.Value.ToGlideString());
        }

        var requestType = order == Order.Descending ? RequestType.XRevRange : RequestType.XRange;
        return new(requestType, [.. args], false, ConvertXRangeResponse);
    }

    private static StreamEntry[] ConvertXRangeResponse(object response)
    {
        // Handle RESP3 dictionary format
        if (response is Dictionary<GlideString, object> dict)
        {
            var entries = new List<StreamEntry>();
            foreach (var kvp in dict)
            {
                var entryId = kvp.Key;
                var outerArray = kvp.Value as object[];
                if (outerArray is null || outerArray.Length == 0) continue;

                // Check if this is the nested format for duplicate fields
                if (outerArray.Length >= 2 && outerArray[0] is object[] && outerArray[1] is object[])
                {
                    var valuesList = new List<NameValueEntry>();
                    for (int i = 0; i < outerArray.Length; i++)
                    {
                        if (outerArray[i] is object[] fieldValuePair && fieldValuePair.Length == 2)
                        {
                            valuesList.Add(new NameValueEntry(
                                (GlideString)fieldValuePair[0],
                                (GlideString)fieldValuePair[1]
                            ));
                        }
                    }
                    entries.Add(new StreamEntry(entryId, [.. valuesList]));
                }
                else
                {
                    var fieldValues = outerArray[0] as object[];
                    if (fieldValues is null || fieldValues.Length == 0) continue;

                    var values = new NameValueEntry[fieldValues.Length / 2];
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = new NameValueEntry(
                            (GlideString)fieldValues[i * 2],
                            (GlideString)fieldValues[i * 2 + 1]
                        );
                    }
                    entries.Add(new StreamEntry(entryId, values));
                }
            }
            return [.. entries];
        }

        // Handle RESP2 array format
        return ConvertStreamEntries((object[])response);
    }

    private static StreamEntry[] ConvertSingleStreamRead(object response)
    {
        if (response is null) return [];

        // Handle dictionary response (RESP3 format)
        // Structure: {stream_key => {entry_id => [field_value_pairs]}}
        if (response is Dictionary<GlideString, object> dict)
        {
            var allEntries = new List<StreamEntry>();

            foreach (var streamKvp in dict)
            {
                // streamKvp.Value should be a nested dictionary of {entry_id => field_values}
                if (streamKvp.Value is Dictionary<GlideString, object> entriesDict)
                {
                    foreach (var entryKvp in entriesDict)
                    {
                        var entryId = entryKvp.Key; // This is the entry ID like "1763338493936-0"
                        var outerArray = entryKvp.Value as object[];

                        if (outerArray is null || outerArray.Length == 0) continue;

                        // The value is an array containing a single element which is the field-value array
                        var fieldValues = outerArray[0] as object[];
                        if (fieldValues is null || fieldValues.Length == 0) continue;

                        // Convert field-value array to NameValueEntry array
                        var values = new NameValueEntry[fieldValues.Length / 2];
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = new NameValueEntry(
                                (GlideString)fieldValues[i * 2],
                                (GlideString)fieldValues[i * 2 + 1]
                            );
                        }

                        allEntries.Add(new StreamEntry(entryId, values));
                    }
                }
            }

            return [.. allEntries];
        }

        // Handle standalone response (array)
        var streams = (object[])response;
        if (streams.Length == 0) return [];

        var streamData = (object[])streams[0];
        if (streamData.Length < 2) return [];

        var entries = (object[])streamData[1];
        if (entries.Length == 0) return [];

        return ConvertStreamEntries(entries);
    }

    private static ValkeyStream[] ConvertMultiStreamRead(object response)
    {
        if (response is null) return [];

        // Handle dictionary response (RESP3 format)
        // Structure: {stream_key => {entry_id => [field_value_pairs]}}
        if (response is Dictionary<GlideString, object> dict)
        {
            var result = new List<ValkeyStream>();

            foreach (var streamKvp in dict)
            {
                var streamKey = new ValkeyKey(streamKvp.Key);
                var entries = new List<StreamEntry>();

                // streamKvp.Value should be a nested dictionary of {entry_id => field_values}
                if (streamKvp.Value is Dictionary<GlideString, object> entriesDict)
                {
                    foreach (var entryKvp in entriesDict)
                    {
                        var entryId = entryKvp.Key;
                        var outerArray = entryKvp.Value as object[];

                        if (outerArray is null || outerArray.Length == 0) continue;

                        // The value is an array containing a single element which is the field-value array
                        var fieldValues = outerArray[0] as object[];
                        if (fieldValues is null || fieldValues.Length == 0) continue;

                        // Convert field-value array to NameValueEntry array
                        var values = new NameValueEntry[fieldValues.Length / 2];
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = new NameValueEntry(
                                (GlideString)fieldValues[i * 2],
                                (GlideString)fieldValues[i * 2 + 1]
                            );
                        }

                        entries.Add(new StreamEntry(entryId, values));
                    }
                }

                result.Add(new ValkeyStream(streamKey, [.. entries]));
            }

            return [.. result];
        }

        // Handle standalone response (array)
        var streams = (object[])response;
        var resultArray = new ValkeyStream[streams.Length];

        for (int i = 0; i < streams.Length; i++)
        {
            var streamData = (object[])streams[i];
            var key = new ValkeyKey((GlideString)streamData[0]);
            var entries = streamData[1] as object[];

            resultArray[i] = new ValkeyStream(key, entries is null || entries.Length == 0 ? [] : ConvertStreamEntries(entries));
        }

        return resultArray;
    }

    private static StreamEntry[] ConvertStreamEntries(object[] entries)
    {
        var result = new StreamEntry[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {
            var entry = (object[])entries[i];
            var id = (GlideString)entry[0];
            var fields = entry[1] as object[];

            if (fields is null)
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

    public static Cmd<string, bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, bool createStream, long? entriesRead)
    {
        List<GlideString> args = [key.ToGlideString(), groupName.ToGlideString(), position.IsNull ? "$".ToGlideString() : position.ToGlideString()];

        if (createStream)
        {
            args.Add("MKSTREAM".ToGlideString());
        }

        if (entriesRead.HasValue)
        {
            args.Add("ENTRIESREAD".ToGlideString());
            args.Add(entriesRead.Value.ToGlideString());
        }

        return new(RequestType.XGroupCreate, [.. args], false, response => response == "OK");
    }

    public static Cmd<bool, bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return new(RequestType.XGroupDestroy, [key.ToGlideString(), groupName.ToGlideString()], false, response => response);
    }

    public static Cmd<string, bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead)
    {
        List<GlideString> args = [key.ToGlideString(), groupName.ToGlideString(), position.ToGlideString()];

        if (entriesRead.HasValue)
        {
            args.Add("ENTRIESREAD".ToGlideString());
            args.Add(entriesRead.Value.ToGlideString());
        }

        return new(RequestType.XGroupSetId, [.. args], false, response => response == "OK");
    }

    public static Cmd<object, bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
    {
        return new(RequestType.XGroupCreateConsumer, [key.ToGlideString(), groupName.ToGlideString(), consumerName.ToGlideString()], false, response => response is bool b ? b : (long)response == 1);
    }

    public static Cmd<long, long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
    {
        return new(RequestType.XGroupDelConsumer, [key.ToGlideString(), groupName.ToGlideString(), consumerName.ToGlideString()], false, response => response);
    }

    public static Cmd<object, StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue position, int? count, bool noAck)
    {
        return StreamReadGroupAsync([new StreamPosition(key, position)], groupName, consumerName, count, noAck, ConvertSingleStreamRead);
    }

    public static Cmd<object, ValkeyStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck)
    {
        return StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck, ConvertMultiStreamRead);
    }

    private static Cmd<object, TResult> StreamReadGroupAsync<TResult>(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? count, bool noAck, Func<object, TResult> converter)
    {
        List<GlideString> args = ["GROUP".ToGlideString(), groupName.ToGlideString(), consumerName.ToGlideString()];

        if (count.HasValue)
        {
            args.Add("COUNT".ToGlideString());
            args.Add(count.Value.ToGlideString());
        }

        if (noAck)
        {
            args.Add("NOACK".ToGlideString());
        }

        args.Add("STREAMS".ToGlideString());
        foreach (var sp in streamPositions)
        {
            args.Add(sp.Key.ToGlideString());
        }
        foreach (var sp in streamPositions)
        {
            args.Add(sp.Position.IsNull ? ">".ToGlideString() : sp.Position.ToGlideString());
        }

        return new(RequestType.XReadGroup, [.. args], false, converter, allowConverterToHandleNull: true);
    }

    public static Cmd<long, long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, params ValkeyValue[] messageIds)
    {
        List<GlideString> args = [key.ToGlideString(), groupName.ToGlideString()];
        foreach (var id in messageIds)
        {
            args.Add(id.ToGlideString());
        }
        return new(RequestType.XAck, [.. args], false, response => response);
    }

    public static Cmd<object[], StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return new(RequestType.XPending, [key.ToGlideString(), groupName.ToGlideString()], false, ConvertStreamPendingInfo);
    }

    public static Cmd<object[], StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue minId, ValkeyValue maxId, int count, ValkeyValue consumerName, long? minIdleTime)
    {
        List<GlideString> args = [key.ToGlideString(), groupName.ToGlideString()];
        if (minIdleTime.HasValue)
        {
            args.Add("IDLE".ToGlideString());
            args.Add(minIdleTime.Value.ToGlideString());
        }
        args.Add(minId.ToGlideString());
        args.Add(maxId.ToGlideString());
        args.Add(count.ToGlideString());
        if (!consumerName.IsNull)
        {
            args.Add(consumerName.ToGlideString());
        }
        return new(RequestType.XPending, [.. args], false, ConvertStreamPendingMessages);
    }

    private static StreamPendingInfo ConvertStreamPendingInfo(object[] response)
    {
        var pendingCount = response[0] is GlideString gs ? int.Parse(gs.ToString()) : (int)(long)response[0];
        var lowestId = response[1] is null ? default : (ValkeyValue)(GlideString)response[1];
        var highestId = response[2] is null ? default : (ValkeyValue)(GlideString)response[2];
        var consumersArray = response[3] as object[];
        var consumers = consumersArray is null ? [] : new StreamConsumer[consumersArray.Length];
        if (consumersArray is not null)
        {
            for (int i = 0; i < consumersArray.Length; i++)
            {
                var consumerData = (object[])consumersArray[i];
                var count = consumerData[1] is GlideString gs2 ? int.Parse(gs2.ToString()) : (int)(long)consumerData[1];
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

    public static Cmd<object, StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, long minIdleTime, ValkeyValue[] messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force)
    {
        return StreamClaimAsync<object, StreamEntry[]>(key, groupName, consumerName, minIdleTime, messageIds, idleTimeInMs, timeUnixMs, retryCount, force, false, ConvertXRangeResponse);
    }

    public static Cmd<object[], ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, long minIdleTime, ValkeyValue[] messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force)
    {
        return StreamClaimAsync<object[], ValkeyValue[]>(key, groupName, consumerName, minIdleTime, messageIds, idleTimeInMs, timeUnixMs, retryCount, force, true, ConvertClaimIdsOnly);
    }

    private static Cmd<TResponse, TResult> StreamClaimAsync<TResponse, TResult>(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, long minIdleTime, ValkeyValue[] messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, bool justId, Func<TResponse, TResult> converter)
    {
        List<GlideString> args = [key.ToGlideString(), groupName.ToGlideString(), consumerName.ToGlideString(), minIdleTime.ToGlideString()];
        foreach (var id in messageIds)
        {
            args.Add(id.ToGlideString());
        }
        if (idleTimeInMs.HasValue)
        {
            args.Add("IDLE".ToGlideString());
            args.Add(idleTimeInMs.Value.ToGlideString());
        }
        if (timeUnixMs.HasValue)
        {
            args.Add("TIME".ToGlideString());
            args.Add(timeUnixMs.Value.ToGlideString());
        }
        if (retryCount.HasValue)
        {
            args.Add("RETRYCOUNT".ToGlideString());
            args.Add(retryCount.Value.ToGlideString());
        }
        if (force)
        {
            args.Add("FORCE".ToGlideString());
        }
        if (justId)
        {
            args.Add("JUSTID".ToGlideString());
        }
        return new(RequestType.XClaim, [.. args], false, converter);
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

    public static Cmd<object[], StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, long minIdleTime, ValkeyValue startId, int? count)
    {
        List<GlideString> args = [key.ToGlideString(), groupName.ToGlideString(), consumerName.ToGlideString(), minIdleTime.ToGlideString(), startId.ToGlideString()];
        if (count.HasValue)
        {
            args.Add("COUNT".ToGlideString());
            args.Add(count.Value.ToGlideString());
        }
        return new(RequestType.XAutoClaim, [.. args], false, ConvertAutoClaimResult);
    }

    public static Cmd<object[], StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, long minIdleTime, ValkeyValue startId, int? count)
    {
        List<GlideString> args = [key.ToGlideString(), groupName.ToGlideString(), consumerName.ToGlideString(), minIdleTime.ToGlideString(), startId.ToGlideString()];
        if (count.HasValue)
        {
            args.Add("COUNT".ToGlideString());
            args.Add(count.Value.ToGlideString());
        }
        args.Add("JUSTID".ToGlideString());
        return new(RequestType.XAutoClaim, [.. args], false, ConvertAutoClaimIdsOnlyResult);
    }

    private static StreamAutoClaimResult ConvertAutoClaimResult(object[] response)
    {
        var nextStartId = (ValkeyValue)(GlideString)response[0];
        var entries = response[1] is Dictionary<GlideString, object> dict ? ConvertXRangeResponse(dict) : ConvertStreamEntries((object[])response[1]);
        var deletedIds = response.Length > 2 && response[2] is object[] delArr ? delArr.Select(id => (ValkeyValue)(GlideString)id).ToArray() : [];
        return new StreamAutoClaimResult(nextStartId, entries, deletedIds);
    }

    private static StreamAutoClaimIdsOnlyResult ConvertAutoClaimIdsOnlyResult(object[] response)
    {
        var nextStartId = (ValkeyValue)(GlideString)response[0];
        var claimedIds = ((object[])response[1]).Select(id => (ValkeyValue)(GlideString)id).ToArray();
        var deletedIds = response.Length > 2 && response[2] is object[] delArr ? delArr.Select(id => (ValkeyValue)(GlideString)id).ToArray() : [];
        return new StreamAutoClaimIdsOnlyResult(nextStartId, claimedIds, deletedIds);
    }

    public static Cmd<object[], StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key)
    {
        return new(RequestType.XInfoGroups, [key.ToGlideString()], false, ConvertStreamGroupInfo);
    }

    public static Cmd<object[], StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return new(RequestType.XInfoConsumers, [key.ToGlideString(), groupName.ToGlideString()], false, ConvertStreamConsumerInfo);
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
                        case "consumers": consumers = value is GlideString gs ? int.Parse(gs.ToString()) : (int)(long)value; break;
                        case "pending": pending = value is GlideString gs2 ? int.Parse(gs2.ToString()) : (int)(long)value; break;
                        case "last-delivered-id": lastDeliveredId = (ValkeyValue)(GlideString)value; break;
                        case "entries-read": entriesRead = value is null ? null : value is GlideString gs3 ? long.Parse(gs3.ToString()) : (long)value; break;
                        case "lag": lag = value is null ? null : value is GlideString gs4 ? long.Parse(gs4.ToString()) : (long)value; break;
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
                        case "consumers": consumers = value is GlideString gs ? int.Parse(gs.ToString()) : (int)(long)value; break;
                        case "pending": pending = value is GlideString gs2 ? int.Parse(gs2.ToString()) : (int)(long)value; break;
                        case "last-delivered-id": lastDeliveredId = (ValkeyValue)(GlideString)value; break;
                        case "entries-read": entriesRead = value is null ? null : value is GlideString gs3 ? long.Parse(gs3.ToString()) : (long)value; break;
                        case "lag": lag = value is null ? null : value is GlideString gs4 ? long.Parse(gs4.ToString()) : (long)value; break;
                        default: break;
                    }
                }
            }
            result[i] = new StreamGroupInfo(name, consumers, pending, lastDeliveredId, entriesRead, lag);
        }
        return result;
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
                        case "pending": pending = value is GlideString gs ? int.Parse(gs.ToString()) : (int)(long)value; break;
                        case "idle": idle = value is GlideString gs2 ? long.Parse(gs2.ToString()) : (long)value; break;
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
                        case "pending": pending = value is GlideString gs ? int.Parse(gs.ToString()) : (int)(long)value; break;
                        case "idle": idle = value is GlideString gs2 ? long.Parse(gs2.ToString()) : (long)value; break;
                        default: break;
                    }
                }
            }
            result[i] = new StreamConsumerInfo(name, pending, idle);
        }
        return result;
    }

    public static Cmd<long, long> StreamLengthAsync(ValkeyKey key)
    {
        return new(RequestType.XLen, [key.ToGlideString()], false, response => response);
    }

    public static Cmd<long, long> StreamDeleteAsync(ValkeyKey key, params ValkeyValue[] messageIds)
    {
        List<GlideString> args = [key.ToGlideString()];
        foreach (var id in messageIds)
        {
            args.Add(id.ToGlideString());
        }
        return new(RequestType.XDel, [.. args], false, response => response);
    }

    public static Cmd<long, long> StreamTrimAsync(ValkeyKey key, long? maxLength, ValkeyValue minId, bool useApproximateMaxLength, long? limit)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (maxLength.HasValue)
        {
            args.Add("MAXLEN".ToGlideString());
            if (useApproximateMaxLength) args.Add("~".ToGlideString());
            args.Add(maxLength.Value.ToGlideString());
        }
        else if (!minId.IsNull)
        {
            args.Add("MINID".ToGlideString());
            if (useApproximateMaxLength) args.Add("~".ToGlideString());
            args.Add(minId.ToGlideString());
        }

        if (limit.HasValue && useApproximateMaxLength)
        {
            args.Add("LIMIT".ToGlideString());
            args.Add(limit.Value.ToGlideString());
        }

        return new(RequestType.XTrim, [.. args], false, response => response);
    }

    public static Cmd<object, StreamInfo> StreamInfoAsync(ValkeyKey key)
    {
        return new(RequestType.XInfoStream, [key.ToGlideString()], false, ConvertStreamInfo);
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
                    case "length": length = value is GlideString gs ? int.Parse(gs.ToString()) : (int)(long)value; break;
                    case "radix-tree-keys": radixTreeKeys = value is GlideString gs2 ? int.Parse(gs2.ToString()) : (int)(long)value; break;
                    case "radix-tree-nodes": radixTreeNodes = value is GlideString gs3 ? int.Parse(gs3.ToString()) : (int)(long)value; break;
                    case "groups": groups = value is GlideString gs4 ? int.Parse(gs4.ToString()) : (int)(long)value; break;
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
                    case "length": length = value is GlideString gs ? int.Parse(gs.ToString()) : (int)(long)value; break;
                    case "radix-tree-keys": radixTreeKeys = value is GlideString gs2 ? int.Parse(gs2.ToString()) : (int)(long)value; break;
                    case "radix-tree-nodes": radixTreeNodes = value is GlideString gs3 ? int.Parse(gs3.ToString()) : (int)(long)value; break;
                    case "groups": groups = value is GlideString gs4 ? int.Parse(gs4.ToString()) : (int)(long)value; break;
                    case "first-entry": firstEntry = value is object[] arr ? ConvertStreamEntries([arr])[0] : default; break;
                    case "last-entry": lastEntry = value is object[] arr2 ? ConvertStreamEntries([arr2])[0] : default; break;
                    case "last-generated-id": lastGeneratedId = (ValkeyValue)(GlideString)value; break;
                    default: break;
                }
            }
        }

        return new StreamInfo(length, radixTreeKeys, radixTreeNodes, groups, firstEntry, lastEntry, lastGeneratedId);
    }
}
