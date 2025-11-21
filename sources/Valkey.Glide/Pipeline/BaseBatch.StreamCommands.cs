// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, ValkeyValue?, bool, long?, StreamTrimMode, bool)" />
    public T StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false) =>
        AddCmd(Request.StreamAddAsync(key, messageId ?? default, maxLength, minId ?? default, useApproximateTrimming, [new NameValueEntry(streamField, streamValue)], limit, mode, noMakeStream));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, NameValueEntry[], ValkeyValue?, long?, ValkeyValue?, bool, long?, StreamTrimMode, bool)" />
    public T StreamAdd(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false) =>
        AddCmd(Request.StreamAddAsync(key, messageId ?? default, maxLength, minId ?? default, useApproximateTrimming, streamPairs, limit, mode, noMakeStream));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(ValkeyKey, ValkeyValue, int?, int?)" />
    public T StreamRead(ValkeyKey key, ValkeyValue position, int? count = null, int? block = null) =>
        AddCmd(Request.StreamReadAsync(key, position, count, block));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(StreamPosition[], int?, int?)" />
    public T StreamRead(StreamPosition[] streamPositions, int? count = null, int? block = null) =>
        AddCmd(Request.StreamReadAsync(streamPositions, count, block));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRange(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)" />
    public T StreamRange(ValkeyKey key, ValkeyValue? start = null, ValkeyValue? end = null, int? count = null, Order order = Order.Ascending) =>
        AddCmd(Request.StreamRangeAsync(key, start ?? "-", end ?? "+", count, order));

    /// <inheritdoc cref="IBatchStreamCommands.StreamLength(ValkeyKey)" />
    public T StreamLength(ValkeyKey key) => AddCmd(Request.StreamLengthAsync(key));

    /// <inheritdoc cref="IBatchStreamCommands.StreamDelete(ValkeyKey, ValkeyValue[])" />
    public T StreamDelete(ValkeyKey key, ValkeyValue[] messageIds) => AddCmd(Request.StreamDeleteAsync(key, messageIds));

    /// <inheritdoc cref="IBatchStreamCommands.StreamTrim(ValkeyKey, long?, ValkeyValue?, bool, long?)" />
    public T StreamTrim(ValkeyKey key, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null) =>
        AddCmd(Request.StreamTrimAsync(key, maxLength, minId ?? default, useApproximateTrimming, limit, StreamTrimMode.KeepReferences));

    /// <inheritdoc cref="IBatchStreamCommands.StreamCreateConsumerGroup(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)" />
    public T StreamCreateConsumerGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null) =>
        AddCmd(Request.StreamCreateConsumerGroupAsync(key, groupName, position ?? default, createStream, entriesRead));

    /// <inheritdoc cref="IBatchStreamCommands.StreamDeleteConsumerGroup(ValkeyKey, ValkeyValue)" />
    public T StreamDeleteConsumerGroup(ValkeyKey key, ValkeyValue groupName) => AddCmd(Request.StreamDeleteConsumerGroupAsync(key, groupName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamDeleteConsumer(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName) => AddCmd(Request.StreamDeleteConsumerAsync(key, groupName, consumerName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamConsumerGroupSetPosition(ValkeyKey, ValkeyValue, ValkeyValue, long?)" />
    public T StreamConsumerGroupSetPosition(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null) => AddCmd(Request.StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead));

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" />
    public T StreamReadGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false) =>
        AddCmd(Request.StreamReadGroupAsync(key, groupName, consumerName, position ?? default, count, noAck));

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(StreamPosition[], ValkeyValue, ValkeyValue, int?, bool)" />
    public T StreamReadGroup(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false) =>
        AddCmd(Request.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAcknowledge(ValkeyKey, ValkeyValue, ValkeyValue[])" />
    public T StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, ValkeyValue[] messageIds) => AddCmd(Request.StreamAcknowledgeAsync(key, groupName, messageIds));

    /// <inheritdoc cref="IBatchStreamCommands.StreamPending(ValkeyKey, ValkeyValue)" />
    public T StreamPending(ValkeyKey key, ValkeyValue groupName) => AddCmd(Request.StreamPendingAsync(key, groupName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamPendingMessages(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, long?)" />
    public T StreamPendingMessages(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, long? minIdleTimeInMs = null) =>
        AddCmd(Request.StreamPendingMessagesAsync(key, groupName, minId ?? "-", maxId ?? "+", count, consumerName, minIdleTimeInMs));

    /// <inheritdoc cref="IBatchStreamCommands.StreamClaim(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue[], long?, long?, int?, bool)" />
    public T StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false) =>
        AddCmd(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force));

    /// <inheritdoc cref="IBatchStreamCommands.StreamClaimIdsOnly(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue[], long?, long?, int?, bool)" />
    public T StreamClaimIdsOnly(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false) =>
        AddCmd(Request.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAutoClaim(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?)" />
    public T StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null) =>
        AddCmd(Request.StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAutoClaimIdsOnly(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?)" />
    public T StreamAutoClaimIdsOnly(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null) =>
        AddCmd(Request.StreamAutoClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count));

    /// <inheritdoc cref="IBatchStreamCommands.StreamInfo(ValkeyKey)" />
    public T StreamInfo(ValkeyKey key) => AddCmd(Request.StreamInfoAsync(key));

    /// <inheritdoc cref="IBatchStreamCommands.StreamInfoFull(ValkeyKey, int?)" />
    public T StreamInfoFull(ValkeyKey key, int? count = null) => AddCmd(Request.StreamInfoFullAsync(key, count));

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupInfo(ValkeyKey)" />
    public T StreamGroupInfo(ValkeyKey key) => AddCmd(Request.StreamGroupInfoAsync(key));

    /// <inheritdoc cref="IBatchStreamCommands.StreamConsumerInfo(ValkeyKey, ValkeyValue)" />
    public T StreamConsumerInfo(ValkeyKey key, ValkeyValue groupName) => AddCmd(Request.StreamConsumerInfoAsync(key, groupName));

    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, long? maxLength, ValkeyValue? minId, bool useApproximateTrimming, long? limit, StreamTrimMode mode, bool noMakeStream) => StreamAdd(key, streamField, streamValue, messageId, maxLength, minId, useApproximateTrimming, limit, mode, noMakeStream);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId, long? maxLength, ValkeyValue? minId, bool useApproximateTrimming, long? limit, StreamTrimMode mode, bool noMakeStream) => StreamAdd(key, streamPairs, messageId, maxLength, minId, useApproximateTrimming, limit, mode, noMakeStream);
    IBatch IBatchStreamCommands.StreamRead(ValkeyKey key, ValkeyValue position, int? count, int? block) => StreamRead(key, position, count, block);
    IBatch IBatchStreamCommands.StreamRead(StreamPosition[] streamPositions, int? count, int? block) => StreamRead(streamPositions, count, block);
    IBatch IBatchStreamCommands.StreamRange(ValkeyKey key, ValkeyValue? start, ValkeyValue? end, int? count, Order order) => StreamRange(key, start, end, count, order);
    IBatch IBatchStreamCommands.StreamLength(ValkeyKey key) => StreamLength(key);
    IBatch IBatchStreamCommands.StreamDelete(ValkeyKey key, ValkeyValue[] messageIds) => StreamDelete(key, messageIds);
    IBatch IBatchStreamCommands.StreamTrim(ValkeyKey key, long? maxLength, ValkeyValue? minId, bool useApproximateTrimming, long? limit) => StreamTrim(key, maxLength, minId, useApproximateTrimming, limit);
    IBatch IBatchStreamCommands.StreamCreateConsumerGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead) => StreamCreateConsumerGroup(key, groupName, position, createStream, entriesRead);
    IBatch IBatchStreamCommands.StreamDeleteConsumerGroup(ValkeyKey key, ValkeyValue groupName) => StreamDeleteConsumerGroup(key, groupName);
    IBatch IBatchStreamCommands.StreamDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName) => StreamDeleteConsumer(key, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamConsumerGroupSetPosition(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead) => StreamConsumerGroupSetPosition(key, groupName, position, entriesRead);
    IBatch IBatchStreamCommands.StreamReadGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck) => StreamReadGroup(key, groupName, consumerName, position, count, noAck);
    IBatch IBatchStreamCommands.StreamReadGroup(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck) => StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, noAck);
    IBatch IBatchStreamCommands.StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, ValkeyValue[] messageIds) => StreamAcknowledge(key, groupName, messageIds);
    IBatch IBatchStreamCommands.StreamPending(ValkeyKey key, ValkeyValue groupName) => StreamPending(key, groupName);
    IBatch IBatchStreamCommands.StreamPendingMessages(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, long? minIdleTimeInMs) => StreamPendingMessages(key, groupName, count, consumerName, minId, maxId, minIdleTimeInMs);
    IBatch IBatchStreamCommands.StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force) => StreamClaim(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force);
    IBatch IBatchStreamCommands.StreamClaimIdsOnly(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force) => StreamClaimIdsOnly(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force);
    IBatch IBatchStreamCommands.StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count) => StreamAutoClaim(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count);
    IBatch IBatchStreamCommands.StreamAutoClaimIdsOnly(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count) => StreamAutoClaimIdsOnly(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count);
    IBatch IBatchStreamCommands.StreamInfo(ValkeyKey key) => StreamInfo(key);
    IBatch IBatchStreamCommands.StreamInfoFull(ValkeyKey key, int? count) => StreamInfoFull(key, count);
    IBatch IBatchStreamCommands.StreamGroupInfo(ValkeyKey key) => StreamGroupInfo(key);
    IBatch IBatchStreamCommands.StreamConsumerInfo(ValkeyKey key, ValkeyValue groupName) => StreamConsumerInfo(key, groupName);
}
