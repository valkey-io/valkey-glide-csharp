// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Globalization;

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    #region Command Builders

    public static Cmd<long, bool> StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId)
        => Boolean<long>(RequestType.XAck, [key, groupName, messageId]);

    public static Cmd<long, long> StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds)
        => Simple<long>(RequestType.XAck, [key, groupName, .. messageIds]);

    public static Cmd<GlideString, ValkeyValue> StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions? options = null)
        => ToValkeyValue(RequestType.XAdd, [key, .. (options ?? new StreamAddOptions()).ToArgs(), .. streamPairs.SelectMany(pair => pair.ToArgs())], isNullable: true);

    public static Cmd<object[], StreamAutoClaimResult> StreamAutoClaim(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, StreamAutoClaimOptions options)
        => new(RequestType.XAutoClaim, [key, groupName, consumerName, .. options.ToArgs()], false, ConvertStreamAutoClaimResponse);

    public static Cmd<object[], StreamAutoClaimJustIdResult> StreamAutoClaimJustId(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, StreamAutoClaimOptions options)
        => new(RequestType.XAutoClaim, [key, groupName, consumerName, .. options.ToArgs(), ValkeyLiterals.JUSTID], false, ConvertStreamAutoClaimJustIdResponse);

    public static Cmd<object, StreamEntry[]> StreamClaim(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => StreamClaimAsync<object, StreamEntry[]>(key, groupName, consumerName, messageIds, options, false, ConvertStreamClaimResponse);

    public static Cmd<object[], ValkeyValue[]> StreamClaimIdsOnly(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => StreamClaimAsync<object[], ValkeyValue[]>(key, groupName, consumerName, messageIds, options, true, ConvertStreamClaimIdsOnlyResponse);

    public static Cmd<long, bool> StreamDelete(ValkeyKey key, ValkeyValue messageId)
        => Boolean<long>(RequestType.XDel, [key, messageId]);

    public static Cmd<long, long> StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)
        => Simple<long>(RequestType.XDel, [key, .. messageIds]);

    public static Cmd<string, ValkeyValue> StreamGroupCreate(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, StreamGroupCreateOptions? options = null)
    {
        List<GlideString> args = [key, groupName, position];

        if (options != null)
        {
            args.AddRange(options.ToArgs());
        }

        return Ok(RequestType.XGroupCreate, [.. args]);
    }

    public static Cmd<object, bool> StreamGroupCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => Boolean<object>(RequestType.XGroupCreateConsumer, [key, groupName, consumerName]);

    public static Cmd<long, long> StreamGroupDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => Simple<long>(RequestType.XGroupDelConsumer, [key, groupName, consumerName]);

    public static Cmd<bool, bool> StreamGroupDestroy(ValkeyKey key, ValkeyValue groupName)
        => Simple<bool>(RequestType.XGroupDestroy, [key, groupName]);

    public static Cmd<string, ValkeyValue> StreamGroupSetId(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null)
    {
        List<GlideString> args = [key, groupName, position];

        if (entriesRead.HasValue)
        {
            args.Add(ValkeyLiterals.ENTRIESREAD);
            args.Add(entriesRead.Value.ToGlideString());
        }

        return Ok(RequestType.XGroupSetId, [.. args]);
    }

    public static Cmd<object, StreamInfo> StreamInfo(ValkeyKey key)
        => new(RequestType.XInfoStream, [key], false, ConvertStreamInfoResponse);

    public static Cmd<object[], StreamConsumerInfo[]> StreamInfoConsumers(ValkeyKey key, ValkeyValue groupName)
        => new(RequestType.XInfoConsumers, [key, groupName], false, ConvertStreamInfoConsumersResponse);

    public static Cmd<object, StreamInfoFull> StreamInfoFull(ValkeyKey key, int? count = null)
    {
        List<GlideString> args = [key, ValkeyLiterals.FULL];

        if (count.HasValue)
        {
            args.Add(ValkeyLiterals.COUNT);
            args.Add(count.Value.ToGlideString());
        }

        return new(RequestType.XInfoStream, [.. args], false, ConvertStreamInfoFullResponse);
    }

    public static Cmd<object[], StreamGroupInfo[]> StreamInfoGroups(ValkeyKey key)
        => new(RequestType.XInfoGroups, [key], false, ConvertStreamInfoGroupsResponse);

    public static Cmd<long, long> StreamLength(ValkeyKey key)
        => Simple<long>(RequestType.XLen, [key]);

    public static Cmd<object[], StreamPendingInfo> StreamPending(ValkeyKey key, ValkeyValue groupName)
        => new(RequestType.XPending, [key, groupName], false, ConvertStreamPendingResponse);

    public static Cmd<object[], StreamPendingMessageInfo[]> StreamPending(ValkeyKey key, ValkeyValue groupName, StreamPendingOptions options)
        => new(RequestType.XPending, [key, groupName, .. options.ToArgs()], false, ConvertStreamPendingMessagesResponse);

    public static Cmd<object, StreamEntry[]> StreamRange(ValkeyKey key, StreamRangeOptions? options = null)
    {
        options ??= new StreamRangeOptions();
        var requestType = options.Order == Order.Descending ? RequestType.XRevRange : RequestType.XRange;
        return new(requestType, [key, .. options.ToArgs()], false, ConvertStreamRangeResponse);
    }

    public static Cmd<object, StreamEntry[]> StreamRead(StreamPosition position, StreamReadOptions? options = null)
    {
        List<GlideString> args = [.. (options ?? new StreamReadOptions()).ToArgs(), .. ToArgs([position])];
        return new(RequestType.XRead, [.. args], false, ConvertStreamReadPosition, allowConverterToHandleNull: true);
    }

    public static Cmd<object, ValkeyStream[]> StreamRead(IEnumerable<StreamPosition> positions, StreamReadOptions? options = null)
    {
        List<GlideString> args = [.. (options ?? new StreamReadOptions()).ToArgs(), .. ToArgs(positions)];
        return new(RequestType.XRead, [.. args], false, ConvertStreamReadPositions, allowConverterToHandleNull: true);
    }

    public static Cmd<object, StreamEntry[]> StreamReadGroup(StreamPosition position, ValkeyValue group, ValkeyValue consumer, StreamReadGroupOptions? options = null)
    {
        List<GlideString> args = [ValkeyLiterals.GROUP, group, consumer, .. (options ?? new StreamReadGroupOptions()).ToArgs(), .. ToArgs([position])];
        return new(RequestType.XReadGroup, [.. args], false, ConvertStreamReadPosition, allowConverterToHandleNull: true);
    }

    public static Cmd<object, ValkeyStream[]> StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue group, ValkeyValue consumer, StreamReadGroupOptions? options = null)
    {
        List<GlideString> args = [ValkeyLiterals.GROUP, group, consumer, .. (options ?? new StreamReadGroupOptions()).ToArgs(), .. ToArgs(positions)];
        return new(RequestType.XReadGroup, [.. args], false, ConvertStreamReadPositions, allowConverterToHandleNull: true);
    }

    public static Cmd<long, long> StreamTrim(ValkeyKey key, StreamTrimOptions options)
        => Simple<long>(RequestType.XTrim, [key, .. options.ToArgs()]);

    #endregion
    #region Response Converters

    private static StreamAutoClaimJustIdResult ConvertStreamAutoClaimJustIdResponse(object[] response)
        => new(nextStartId: ToValkeyValue(response[0]),
            claimedIds: ToValkeyValueArray(response[1]),
            deletedIds: response.Length > 2 ? ToValkeyValueArray(response[2]) : []);

    private static StreamAutoClaimResult ConvertStreamAutoClaimResponse(object[] response)
        => new(nextStartId: ToValkeyValue(response[0]),
            claimedEntries: ToStreamEntries(response[1]),
            deletedIds: response.Length > 2 ? ToValkeyValueArray(response[2]) : []);

    private static ValkeyValue[] ConvertStreamClaimIdsOnlyResponse(object[] response)
        => ToValkeyValueArray(response);

    private static StreamEntry[] ConvertStreamClaimResponse(object response)
        => ToStreamEntries(response);

    // TODO
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

    // TODO
    private static StreamGroupInfoFull ConvertStreamGroupInfoFull(object groupResponse)
    {
        var map = ToFieldMap(groupResponse);

        var consumers = map.TryGetValue("consumers", out var consumersVal) && consumersVal is object[] consumersArr
            ? consumersArr.Select(ToStreamConsumerInfoFull).ToArray()
            : [];

        var pending = map.TryGetValue("pending", out var pendingVal) && pendingVal is object[] pendingArr
            ? pendingArr.Select(ToStreamPendingEntry).ToArray()
            : [];

        return new StreamGroupInfoFull(
            GetString(map, "name"),
            TryGetValkeyValue(map, "last-delivered-id"),
            TryGetLong(map, "entries-read"),
            TryGetLong(map, "lag"),
            GetLong(map, "pel-count"),
            pending,
            consumers);
    }

    // TODO
    private static StreamConsumerInfo[] ConvertStreamInfoConsumersResponse(object[] response)
    {
        var result = new StreamConsumerInfo[response.Length];
        for (int i = 0; i < response.Length; i++)
        {
            var name = "";
            var pending = 0;
            var idle = 0L;
            var inactive = (long?)null;

            if (response[i] is Dictionary<GlideString, object> dict)
            {
                foreach (var kvp in dict)
                {
                    var key = kvp.Key.ToString();
                    var value = kvp.Value;
                    switch (key)
                    {
                        case "name": name = ((GlideString)value).ToString(); break;
                        case "pending": pending = ParseInt(value); break;
                        case "idle": idle = value is GlideString gs2 ? long.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        case "inactive": inactive = value is GlideString gs3 ? long.Parse(gs3.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
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
                        case "pending": pending = ParseInt(value); break;
                        case "idle": idle = value is GlideString gs2 ? long.Parse(gs2.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        case "inactive": inactive = value is GlideString gs3 ? long.Parse(gs3.ToString(), CultureInfo.InvariantCulture) : (long)value; break;
                        default: break;
                    }
                }
            }
            result[i] = new StreamConsumerInfo(
                name,
                pending,
                TimeSpan.FromMilliseconds(idle),
                inactive is null ? null : TimeSpan.FromMilliseconds(inactive.Value));
        }
        return result;
    }

    // TODO
    private static StreamInfoFull ConvertStreamInfoFullResponse(object response)
    {
        var map = ToFieldMap(response);

        var entries = map.TryGetValue("entries", out var entriesVal) && entriesVal is object[] entriesArr
            ? ConvertStreamEntries(entriesArr)
            : [];

        var groups = map.TryGetValue("groups", out var groupsVal) && groupsVal is object[] groupsArr
            ? groupsArr.Select(ConvertStreamGroupInfoFull).ToArray()
            : [];

        return new StreamInfoFull(
            (int)GetLong(map, "length"),
            (int)GetLong(map, "radix-tree-keys"),
            (int)GetLong(map, "radix-tree-nodes"),
            TryGetValkeyValue(map, "last-generated-id"),
            TryGetValkeyValue(map, "max-deleted-entry-id"),
            TryGetLong(map, "entries-added") ?? -1L,
            TryGetValkeyValue(map, "recorded-first-entry-id"),
            entries,
            groups);
    }

    // TODO
    private static StreamGroupInfo[] ConvertStreamInfoGroupsResponse(object[] response)
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
                        case "consumers": consumers = ParseInt(value); break;
                        case "pending": pending = ParseInt(value); break;
                        case "last-delivered-id": lastDeliveredId = ToValkeyValue(value); break;
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
                        case "consumers": consumers = ParseInt(value); break;
                        case "pending": pending = ParseInt(value); break;
                        case "last-delivered-id": lastDeliveredId = ToValkeyValue(value); break;
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

    // TODO
    private static StreamInfo ConvertStreamInfoResponse(object response)
    {
        var length = 0;
        var radixTreeKeys = 0;
        var radixTreeNodes = 0;
        var groups = 0;
        var firstEntry = default(StreamEntry);
        var lastEntry = default(StreamEntry);
        var lastGeneratedId = ValkeyValue.Null;
        var maxDeletedEntryId = ValkeyValue.Null;
        var entriesAdded = -1L;
        var recordedFirstEntryId = ValkeyValue.Null;

        if (response is Dictionary<GlideString, object> dict)
        {
            foreach (var kvp in dict)
            {
                var key = kvp.Key.ToString();
                var value = kvp.Value;
                switch (key)
                {
                    case "length": length = ParseInt(value); break;
                    case "radix-tree-keys": radixTreeKeys = ParseInt(value); break;
                    case "radix-tree-nodes": radixTreeNodes = ParseInt(value); break;
                    case "groups": groups = ParseInt(value); break;
                    case "first-entry": firstEntry = value is object[] arr ? ConvertStreamEntries([arr])[0] : default; break;
                    case "last-entry": lastEntry = value is object[] arr2 ? ConvertStreamEntries([arr2])[0] : default; break;
                    case "last-generated-id": lastGeneratedId = ToValkeyValue(value); break;
                    case "max-deleted-entry-id": maxDeletedEntryId = ToValkeyValue(value); break;
                    case "entries-added": entriesAdded = ParseLong(value); break;
                    case "recorded-first-entry-id": recordedFirstEntryId = ToValkeyValue(value); break;
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
                    case "length": length = ParseInt(value); break;
                    case "radix-tree-keys": radixTreeKeys = ParseInt(value); break;
                    case "radix-tree-nodes": radixTreeNodes = ParseInt(value); break;
                    case "groups": groups = ParseInt(value); break;
                    case "first-entry": firstEntry = value is object[] arr ? ConvertStreamEntries([arr])[0] : default; break;
                    case "last-entry": lastEntry = value is object[] arr2 ? ConvertStreamEntries([arr2])[0] : default; break;
                    case "last-generated-id": lastGeneratedId = ToValkeyValue(value); break;
                    case "max-deleted-entry-id": maxDeletedEntryId = ToValkeyValue(value); break;
                    case "entries-added": entriesAdded = ParseLong(value); break;
                    case "recorded-first-entry-id": recordedFirstEntryId = ToValkeyValue(value); break;
                    default: break;
                }
            }
        }

        return new StreamInfo(
            length,
            radixTreeKeys,
            radixTreeNodes,
            lastGeneratedId,
            maxDeletedEntryId,
            entriesAdded,
            recordedFirstEntryId,
            groups,
            firstEntry,
            lastEntry);
    }

    private static StreamPendingMessageInfo[] ConvertStreamPendingMessagesResponse(object[] response)
    {
        var result = new StreamPendingMessageInfo[response.Length];
        for (int i = 0; i < response.Length; i++)
        {
            var msgData = (object[])response[i];
            result[i] = new StreamPendingMessageInfo(
                ToValkeyValue(msgData[0]),
                ToValkeyValue(msgData[1]),
                TimeSpan.FromMilliseconds((long)msgData[2]),
                ParseInt(msgData[3])
            );
        }
        return result;
    }

    private static StreamPendingInfo ConvertStreamPendingResponse(object[] response)
    {
        var pendingCount = ParseInt(response[0]);
        var lowestId = ToValkeyValue(response[1]);
        var highestId = ToValkeyValue(response[2]);
        var consumersArray = response[3] as object[];
        var consumers = consumersArray is null ? [] : new StreamConsumer[consumersArray.Length];
        if (consumersArray is not null)
        {
            for (int i = 0; i < consumersArray.Length; i++)
            {
                var consumerData = (object[])consumersArray[i];
                var count = ParseInt(consumerData[1]);
                consumers[i] = new StreamConsumer(ToValkeyValue(consumerData[0]), count);
            }
        }
        return new StreamPendingInfo(pendingCount, lowestId, highestId, consumers);
    }

    private static StreamEntry[] ConvertStreamRangeResponse(object response)
        => ToStreamEntries(response);

    private static StreamEntry[] ConvertStreamReadPosition(object response)
        => ToValkeyStream(response) is [var stream, ..] ? stream.Entries : [];

    private static ValkeyStream[] ConvertStreamReadPositions(object response)
        => ToValkeyStream(response);

    private static int ParseInt(object? value) => (int)ParseLong(value);

    private static long ParseLong(object? value) => value switch
    {
        null => 0L,
        long l => l,
        GlideString gs => long.Parse(gs.ToString(), CultureInfo.InvariantCulture),
        _ => (long)value,
    };

    private static DateTime ToDateTime(object? value) => DateTimeOffset.FromUnixTimeMilliseconds(ParseLong(value)).UtcDateTime;

    /// <summary>
    /// Normalizes a stream-info response element into a field lookup, accepting either a RESP3
    /// map (<see cref="Dictionary{GlideString, Object}"/>) or a RESP2 flat key-value array.
    /// </summary>
    private static Dictionary<GlideString, object> ToFieldMap(object response)
    {
        // RESP3 already returns a field map; use it directly.
        if (response is Dictionary<GlideString, object> dict)
        {
            return dict;
        }

        // RESP2 returns a flat key-value array; fold it into a map.
        var result = new Dictionary<GlideString, object>();
        if (response is object[] array)
        {
            for (int i = 0; i + 1 < array.Length; i += 2)
            {
                result[(GlideString)array[i]] = array[i + 1];
            }
        }

        return result;
    }

    private static DateTime? ToNullableDateTime(object? value) => value is null ? null : ToDateTime(value);

    private static StreamConsumerInfoFull ToStreamConsumerInfoFull(object consumerResponse)
    {
        var map = ToFieldMap(consumerResponse);
        var name = GetString(map, "name");

        // The consumer's PEL entries do not carry the consumer name on the wire; populate it from the owning consumer.
        var pending = map.TryGetValue("pending", out var pendingVal) && pendingVal is object[] pendingArr
            ? pendingArr.Select(entry => ToStreamPendingEntry(entry, name)).ToArray()
            : [];

        return new StreamConsumerInfoFull(
            name,
            ToDateTime(map.GetValueOrDefault("seen-time")),
            ToNullableDateTime(map.GetValueOrDefault("active-time")),
            GetLong(map, "pel-count"),
            pending);
    }

    /// <summary>
    /// Converts a stream entry map response (XREAD, XREADGROUP, XRANGE, XREVRANGE, XCLAIM, and XAUTOCLAIM).
    /// </summary>
    private static StreamEntry[] ToStreamEntries(object response)
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

    private static StreamPendingEntry ToStreamPendingEntry(object entry, string consumer)
    {
        var arr = (object[])entry;
        return new StreamPendingEntry(
            ToValkeyValue(arr[0]),
            consumer,
            ToDateTime(arr[1]),
            ParseInt(arr[2]));
    }

    private static StreamPendingEntry ToStreamPendingEntry(object entry)
    {
        var arr = (object[])entry;
        return new StreamPendingEntry(
            ToValkeyValue(arr[0]),
            ToValkeyValue(arr[1]).ToString(),
            ToDateTime(arr[2]),
            ParseInt(arr[3]));
    }

    internal static ValkeyStream[] ToValkeyStream(object response)
    {
        if (response is null)
        {
            return [];
        }

        var result = new List<ValkeyStream>();
        foreach (var kvp in (Dictionary<GlideString, object>)response)
        {
            var key = new ValkeyKey(kvp.Key);
            var entries = ToStreamEntries(kvp.Value);
            result.Add(new ValkeyStream(key, entries));
        }

        return [.. result];
    }

    #endregion

    #region Argument Builders

    private static Cmd<TResponse, TResult> StreamClaimAsync<TResponse, TResult>(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options, bool justId, Func<TResponse, TResult> converter)
    {
        List<GlideString> args = [key, groupName, consumerName, ToULongMs(options.MinIdleTime, nameof(options.MinIdleTime)).ToGlideString()];
        foreach (var id in messageIds)
        {
            args.Add(id);
        }

        args.AddRange(options.ToArgs());

        if (justId)
        {
            args.Add(ValkeyLiterals.JUSTID);
        }
        return new(RequestType.XClaim, [.. args], false, converter);
    }

    // Builds the STREAMS key... id... argument slice shared by XREAD and XREADGROUP.
    private static GlideString[] ToArgs(IEnumerable<StreamPosition> positions)
    {
        StreamPosition[] array = [.. positions];

        List<GlideString> args = [ValkeyLiterals.STREAMS];
        foreach (var sp in array)
        {
            args.Add(sp.Key);
        }
        foreach (var sp in array)
        {
            args.Add(sp.Position);
        }

        return [.. args];
    }

    #endregion
}
