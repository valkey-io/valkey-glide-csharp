// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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

    public static Cmd<Dictionary<GlideString, object>, StreamEntry[]> StreamClaim(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => new(RequestType.XClaim, [key, groupName, consumerName, ToULongMs(options.MinIdleTime, nameof(options.MinIdleTime)).ToGlideString(), .. messageIds, .. options.ToArgs()], false, ConvertStreamEntriesResponse);

    public static Cmd<object[], ValkeyValue[]> StreamClaimIdsOnly(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => new(RequestType.XClaim, [key, groupName, consumerName, ToULongMs(options.MinIdleTime, nameof(options.MinIdleTime)).ToGlideString(), .. messageIds, .. options.ToArgs(), ValkeyLiterals.JUSTID], false, ToValkeyValueArray);

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
        => new(RequestType.XInfoConsumers, [key, groupName], false, ConvertStreamConsumerInfoResponses);

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
        => new(RequestType.XInfoGroups, [key], false, ConvertStreamGroupInfoResponses);

    public static Cmd<long, long> StreamLength(ValkeyKey key)
        => Simple<long>(RequestType.XLen, [key]);

    public static Cmd<object[], StreamPendingInfo> StreamPending(ValkeyKey key, ValkeyValue groupName)
        => new(RequestType.XPending, [key, groupName], false, ConvertStreamPendingInfoResponse);

    public static Cmd<object[], StreamPendingMessageInfo[]> StreamPending(ValkeyKey key, ValkeyValue groupName, StreamPendingOptions options)
        => new(RequestType.XPending, [key, groupName, .. options.ToArgs()], false, ConvertStreamPendingMessageInfoResponses);

    public static Cmd<Dictionary<GlideString, object>, StreamEntry[]> StreamRange(ValkeyKey key, StreamRangeOptions? options = null)
    {
        options ??= new StreamRangeOptions();
        var requestType = options.Order == Order.Descending ? RequestType.XRevRange : RequestType.XRange;
        return new(requestType, [key, .. options.ToArgs()], false, ConvertStreamEntriesResponse);
    }

    public static Cmd<Dictionary<GlideString, object>, StreamEntry[]> StreamRead(StreamPosition position, StreamReadOptions? options = null)
    {
        List<GlideString> args = [.. (options ?? new StreamReadOptions()).ToArgs(), .. ToArgs([position])];
        return new(RequestType.XRead, [.. args], false, ConvertStreamReadPositionResponse, allowConverterToHandleNull: true);
    }

    public static Cmd<Dictionary<GlideString, object>, ValkeyStream[]> StreamRead(IEnumerable<StreamPosition> positions, StreamReadOptions? options = null)
    {
        List<GlideString> args = [.. (options ?? new StreamReadOptions()).ToArgs(), .. ToArgs(positions)];
        return new(RequestType.XRead, [.. args], false, ConvertValkeyStreamResponse, allowConverterToHandleNull: true);
    }

    public static Cmd<Dictionary<GlideString, object>, StreamEntry[]> StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions? options = null)
    {
        List<GlideString> args = [ValkeyLiterals.GROUP, groupName, consumerName, .. (options ?? new StreamReadGroupOptions()).ToArgs(), .. ToArgs([position])];
        return new(RequestType.XReadGroup, [.. args], false, ConvertStreamReadPositionResponse, allowConverterToHandleNull: true);
    }

    public static Cmd<Dictionary<GlideString, object>, ValkeyStream[]> StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions? options = null)
    {
        List<GlideString> args = [ValkeyLiterals.GROUP, groupName, consumerName, .. (options ?? new StreamReadGroupOptions()).ToArgs(), .. ToArgs(positions)];
        return new(RequestType.XReadGroup, [.. args], false, ConvertValkeyStreamResponse, allowConverterToHandleNull: true);
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
            claimedEntries: ConvertStreamEntriesResponse((Dictionary<GlideString, object>)response[1]),
            deletedIds: response.Length > 2 ? ToValkeyValueArray(response[2]) : []);

    private static StreamEntry ConvertStreamEntryResponse(object response)
    {
        var array = (object[])response;
        var fields = (object[])array[1];

        var values = new NameValueEntry[fields.Length / 2];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = new NameValueEntry(
                name: (GlideString)fields[2 * i],
                value: (GlideString)fields[(2 * i) + 1]);
        }

        return new StreamEntry(
            id: (GlideString)array[0],
            values: values);
    }

    private static StreamGroupInfoFull ConvertStreamGroupInfoFullResponse(object response)
    {
        var map = ToFieldMap(response);

        var consumers = (map.GetValueOrDefault("consumers") as object[] ?? []).Select(ConvertStreamConsumerInfoFullResponse).ToArray();
        var pending = (map.GetValueOrDefault("pending") as object[] ?? []).Select(ConvertStreamPendingEntryResponse).ToArray();

        return new StreamGroupInfoFull(
            GetString(map, "name"),
            TryGetValkeyValue(map, "last-delivered-id"),
            TryGetLong(map, "entries-read"),
            TryGetLong(map, "lag"),
            GetLong(map, "pel-count"),
            pending,
            consumers);
    }

    private static StreamConsumerInfo[] ConvertStreamConsumerInfoResponses(object[] responses)
        => [.. responses.Select(response =>
        {
            var map = ToFieldMap(response);
            return new StreamConsumerInfo(
                GetString(map, "name"),
                GetInt(map, "pending"),
                ToTimeSpan(map["idle"]),
                map.TryGetValue("inactive", out var inactive) ? ToTimeSpan(inactive) : null);
        })];

    private static StreamInfoFull ConvertStreamInfoFullResponse(object response)
    {
        var map = ToFieldMap(response);

        var entries = (map.GetValueOrDefault("entries") as object[] ?? []).Select(ConvertStreamEntryResponse).ToArray();
        var groups = (map.GetValueOrDefault("groups") as object[] ?? []).Select(ConvertStreamGroupInfoFullResponse).ToArray();

        return new StreamInfoFull(
            length: GetInt(map, "length"),
            radixTreeKeys: GetInt(map, "radix-tree-keys"),
            radixTreeNodes: GetInt(map, "radix-tree-nodes"),
            lastGeneratedId: TryGetValkeyValue(map, "last-generated-id"),
            maxDeletedEntryId: TryGetValkeyValue(map, "max-deleted-entry-id"),
            entriesAdded: TryGetLong(map, "entries-added") ?? -1L,
            recordedFirstEntryId: TryGetValkeyValue(map, "recorded-first-entry-id"),
            entries: entries,
            groups: groups);
    }

    private static StreamGroupInfo[] ConvertStreamGroupInfoResponses(object[] responses)
        => [.. responses.Select(response =>
        {
            var map = ToFieldMap(response);

            return new StreamGroupInfo(
                name: GetString(map, "name"),
                consumerCount: GetInt(map, "consumers"),
                pendingMessageCount: GetInt(map, "pending"),
                lastDeliveredId: TryGetValkeyValue(map, "last-delivered-id"),
                entriesRead: TryGetLong(map, "entries-read"),
                lag: TryGetLong(map, "lag"));
        })];

    private static StreamInfo ConvertStreamInfoResponse(object response)
    {
        var map = ToFieldMap(response);

        var firstEntry = map.GetValueOrDefault("first-entry") is object[] first ? ConvertStreamEntryResponse(first) : StreamEntry.Null;
        var lastEntry = map.GetValueOrDefault("last-entry") is object[] last ? ConvertStreamEntryResponse(last) : StreamEntry.Null;

        return new StreamInfo(
            GetInt(map, "length"),
            GetInt(map, "radix-tree-keys"),
            GetInt(map, "radix-tree-nodes"),
            TryGetValkeyValue(map, "last-generated-id"),
            TryGetValkeyValue(map, "max-deleted-entry-id"),
            TryGetLong(map, "entries-added") ?? -1L,
            TryGetValkeyValue(map, "recorded-first-entry-id"),
            GetInt(map, "groups"),
            firstEntry,
            lastEntry);
    }

    private static StreamPendingMessageInfo[] ConvertStreamPendingMessageInfoResponses(object[] responses)
    {
        var result = new StreamPendingMessageInfo[responses.Length];
        for (int i = 0; i < responses.Length; i++)
        {
            var msgData = (object[])responses[i];

            result[i] = new StreamPendingMessageInfo(
                messageId: ToValkeyValue(msgData[0]),
                consumerName: ToValkeyValue(msgData[1]),
                idle: ToTimeSpan(msgData[2]),
                deliveryCount: ToInt(msgData[3])
            );
        }
        return result;
    }

    private static StreamPendingInfo ConvertStreamPendingInfoResponse(object[] response)
    {
        var consumers = (response[3] as object[] ?? []).Select(ConvertStreamConsumerResponse).ToArray();

        return new StreamPendingInfo(
            pendingMessageCount: ToInt(response[0]),
            lowestId: ToValkeyValue(response[1]),
            highestId: ToValkeyValue(response[2]),
            consumers: consumers);
    }

    private static StreamConsumer ConvertStreamConsumerResponse(object response)
    {
        var data = (object[])response;

        return new StreamConsumer(
            name: ToValkeyValue(data[0]),
            pendingMessageCount: ToInt(data[1]));
    }

    private static StreamEntry[] ConvertStreamReadPositionResponse(Dictionary<GlideString, object>? response)
    {
        if (response is null)
        {
            return [];
        }

        return ConvertStreamEntriesResponse((Dictionary<GlideString, object>)response.Values.First());
    }

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

    private static NameValueEntry ConvertNameValueEntryResponse(object response)
    {
        var pair = (object[])response;

        return new NameValueEntry(
            name: (GlideString)pair[0],
            value: (GlideString)pair[1]);
    }

    private static StreamConsumerInfoFull ConvertStreamConsumerInfoFullResponse(object response)
    {
        var map = ToFieldMap(response);
        var name = GetString(map, "name");

        // The consumer's PEL entries do not carry the consumer name on the wire; populate it from the owning consumer.
        var pending = (map.GetValueOrDefault("pending") as object[] ?? []).Select(entry => ConvertStreamPendingEntryResponse(entry, name)).ToArray();

        return new StreamConsumerInfoFull(
            name,
            ToDateTimeOffset(map["seen-time"]),
            map.TryGetValue("active-time", out var activeTime) ? ToDateTimeOffset(activeTime) : null,
            GetLong(map, "pel-count"),
            pending);
    }

    private static StreamEntry[] ConvertStreamEntriesResponse(Dictionary<GlideString, object> response)
    {
        var entries = new List<StreamEntry>();
        foreach (var entry in response)
        {
            // Pending messages that have been acknowledged/deleted have nil field values.
            if (entry.Value is not object[] outerArray || outerArray.Length == 0)
            {
                continue;
            }

            entries.Add(new StreamEntry(
                id: entry.Key,
                values: [.. outerArray.Select(ConvertNameValueEntryResponse)]));
        }

        return [.. entries];
    }

    private static StreamPendingEntry ConvertStreamPendingEntryResponse(object response, string consumer)
    {
        var arr = (object[])response;
        return new StreamPendingEntry(
            ToValkeyValue(arr[0]),
            consumer,
            ToDateTimeOffset(arr[1]),
            ToInt(arr[2]));
    }

    private static StreamPendingEntry ConvertStreamPendingEntryResponse(object response)
    {
        var arr = (object[])response;
        return new StreamPendingEntry(
            ToValkeyValue(arr[0]),
            ToValkeyValue(arr[1]).ToString(),
            ToDateTimeOffset(arr[2]),
            ToInt(arr[3]));
    }

    internal static ValkeyStream[] ConvertValkeyStreamResponse(Dictionary<GlideString, object>? response)
    {
        if (response is null)
        {
            return [];
        }

        var result = new List<ValkeyStream>();
        foreach (var kvp in response)
        {
            var key = new ValkeyKey(kvp.Key);
            var entries = ConvertStreamEntriesResponse((Dictionary<GlideString, object>)kvp.Value);
            result.Add(new ValkeyStream(key, entries));
        }

        return [.. result];
    }

    #endregion

    #region Argument Builders

    private static GlideString[] ToArgs(IEnumerable<StreamPosition> positions)
    {
        List<GlideString> args = [ValkeyLiterals.STREAMS];

        var array = positions.ToArray();
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
