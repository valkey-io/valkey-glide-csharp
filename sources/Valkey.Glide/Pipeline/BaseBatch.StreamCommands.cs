// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, ValkeyValue, ValkeyValue, StreamAddOptions?)" />
    public T StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions? options = null) =>
        AddCmd(Request.StreamAddAsync(key, [new NameValueEntry(streamField, streamValue)], options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, IEnumerable{NameValueEntry}, StreamAddOptions?)" />
    public T StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions? options = null) =>
        AddCmd(Request.StreamAddAsync(key, [.. streamPairs], options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(ValkeyKey, ValkeyValue, int?)" />
    public T StreamRead(ValkeyKey key, ValkeyValue position, int? count = null) =>
        AddCmd(Request.StreamReadAsync(key, position, count));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(IEnumerable{StreamPosition}, int?)" />
    public T StreamRead(IEnumerable<StreamPosition> streamPositions, int? count = null) =>
        AddCmd(Request.StreamReadAsync([.. streamPositions], count));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRange(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)" />
    public T StreamRange(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending) =>
        AddCmd(Request.StreamRangeAsync(key, minId ?? "-", maxId ?? "+", count, messageOrder));

    /// <inheritdoc cref="IBatchStreamCommands.StreamLength(ValkeyKey)" />
    public T StreamLength(ValkeyKey key) => AddCmd(Request.StreamLengthAsync(key));

    /// <inheritdoc cref="IBatchStreamCommands.StreamDelete(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds) => AddCmd(Request.StreamDeleteAsync(key, [.. messageIds]));

    /// <inheritdoc cref="IBatchStreamCommands.StreamTrim(ValkeyKey, long?, ValkeyValue?, bool, long?)" />
    public T StreamTrim(ValkeyKey key, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null) =>
        AddCmd(Request.StreamTrimAsync(key, maxLength, minId ?? default, useApproximateTrimming, limit));

    /// <inheritdoc cref="IBatchStreamCommands.StreamCreateConsumerGroup(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)" />
    public T StreamCreateConsumerGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null) =>
        AddCmd(Request.StreamCreateConsumerGroupAsync(key, groupName, position ?? default, createStream, entriesRead));

    /// <inheritdoc cref="IBatchStreamCommands.StreamDeleteConsumerGroup(ValkeyKey, ValkeyValue)" />
    public T StreamDeleteConsumerGroup(ValkeyKey key, ValkeyValue groupName) => AddCmd(Request.StreamDeleteConsumerGroupAsync(key, groupName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamCreateConsumer(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName) => AddCmd(Request.StreamCreateConsumerAsync(key, groupName, consumerName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamDeleteConsumer(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName) => AddCmd(Request.StreamDeleteConsumerAsync(key, groupName, consumerName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamConsumerGroupSetPosition(ValkeyKey, ValkeyValue, ValkeyValue, long?)" />
    public T StreamConsumerGroupSetPosition(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null) => AddCmd(Request.StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead));

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" />
    public T StreamReadGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false) =>
        AddCmd(Request.StreamReadGroupAsync(key, groupName, consumerName, position ?? default, count, noAck));

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)" />
    public T StreamReadGroup(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false) =>
        AddCmd(Request.StreamReadGroupAsync([.. streamPositions], groupName, consumerName, countPerStream, noAck));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAcknowledge(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" />
    public T StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds) => AddCmd(Request.StreamAcknowledgeAsync(key, groupName, [.. messageIds]));

    /// <inheritdoc cref="IBatchStreamCommands.StreamPending(ValkeyKey, ValkeyValue)" />
    public T StreamPending(ValkeyKey key, ValkeyValue groupName) => AddCmd(Request.StreamPendingAsync(key, groupName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamPendingMessages(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, TimeSpan?)" />
    public T StreamPendingMessages(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName = default, ValkeyValue? minId = null, ValkeyValue? maxId = null, TimeSpan? minIdleTime = null) =>
        AddCmd(Request.StreamPendingMessagesAsync(key, groupName, minId ?? "-", maxId ?? "+", count, consumerName, minIdleTime));

    /// <inheritdoc cref="IBatchStreamCommands.StreamClaim(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue}, StreamClaimOptions?)" />
    public T StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions? options = null) =>
        AddCmd(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds], options?.Idle, options?.IdleUnix, options?.RetryCount, options?.Force ?? false));

    /// <inheritdoc cref="IBatchStreamCommands.StreamClaimJustId(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue}, StreamClaimOptions?)" />
    public T StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions? options = null) =>
        AddCmd(Request.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds], options?.Idle, options?.IdleUnix, options?.RetryCount, options?.Force ?? false));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAutoClaim(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" />
    public T StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null) =>
        AddCmd(Request.StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, minIdleTime, startAtId, count));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAutoClaimJustId(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" />
    public T StreamAutoClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null) =>
        AddCmd(Request.StreamAutoClaimJustIdAsync(key, consumerGroup, claimingConsumer, minIdleTime, startAtId, count));

    /// <inheritdoc cref="IBatchStreamCommands.StreamInfo(ValkeyKey)" />
    public T StreamInfo(ValkeyKey key) => AddCmd(Request.StreamInfoAsync(key));

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupInfo(ValkeyKey)" />
    public T StreamGroupInfo(ValkeyKey key) => AddCmd(Request.StreamGroupInfoAsync(key));

    /// <inheritdoc cref="IBatchStreamCommands.StreamConsumerInfo(ValkeyKey, ValkeyValue)" />
    public T StreamConsumerInfo(ValkeyKey key, ValkeyValue groupName) => AddCmd(Request.StreamConsumerInfoAsync(key, groupName));

    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions? options) => StreamAdd(key, streamField, streamValue, options);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions? options) => StreamAdd(key, streamPairs, options);
    IBatch IBatchStreamCommands.StreamRead(ValkeyKey key, ValkeyValue position, int? count) => StreamRead(key, position, count);
    IBatch IBatchStreamCommands.StreamRead(IEnumerable<StreamPosition> streamPositions, int? count) => StreamRead(streamPositions, count);
    IBatch IBatchStreamCommands.StreamRange(ValkeyKey key, ValkeyValue? minId, ValkeyValue? maxId, int? count, Order messageOrder) => StreamRange(key, minId, maxId, count, messageOrder);
    IBatch IBatchStreamCommands.StreamLength(ValkeyKey key) => StreamLength(key);
    IBatch IBatchStreamCommands.StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds) => StreamDelete(key, messageIds);
    IBatch IBatchStreamCommands.StreamTrim(ValkeyKey key, long? maxLength, ValkeyValue? minId, bool useApproximateTrimming, long? limit) => StreamTrim(key, maxLength, minId, useApproximateTrimming, limit);
    IBatch IBatchStreamCommands.StreamCreateConsumerGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead) => StreamCreateConsumerGroup(key, groupName, position, createStream, entriesRead);
    IBatch IBatchStreamCommands.StreamDeleteConsumerGroup(ValkeyKey key, ValkeyValue groupName) => StreamDeleteConsumerGroup(key, groupName);
    IBatch IBatchStreamCommands.StreamCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName) => StreamCreateConsumer(key, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName) => StreamDeleteConsumer(key, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamConsumerGroupSetPosition(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead) => StreamConsumerGroupSetPosition(key, groupName, position, entriesRead);
    IBatch IBatchStreamCommands.StreamReadGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck) => StreamReadGroup(key, groupName, consumerName, position, count, noAck);
    IBatch IBatchStreamCommands.StreamReadGroup(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck) => StreamReadGroup(streamPositions, groupName, consumerName, countPerStream, noAck);
    IBatch IBatchStreamCommands.StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds) => StreamAcknowledge(key, groupName, messageIds);
    IBatch IBatchStreamCommands.StreamPending(ValkeyKey key, ValkeyValue groupName) => StreamPending(key, groupName);
    IBatch IBatchStreamCommands.StreamPendingMessages(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, TimeSpan? minIdleTime) => StreamPendingMessages(key, groupName, count, consumerName, minId, maxId, minIdleTime);
    IBatch IBatchStreamCommands.StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions? options) => StreamClaim(key, consumerGroup, claimingConsumer, minIdleTime, messageIds, options);
    IBatch IBatchStreamCommands.StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions? options) => StreamClaimJustId(key, consumerGroup, claimingConsumer, minIdleTime, messageIds, options);
    IBatch IBatchStreamCommands.StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count) => StreamAutoClaim(key, consumerGroup, claimingConsumer, minIdleTime, startAtId, count);
    IBatch IBatchStreamCommands.StreamAutoClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count) => StreamAutoClaimJustId(key, consumerGroup, claimingConsumer, minIdleTime, startAtId, count);
    IBatch IBatchStreamCommands.StreamInfo(ValkeyKey key) => StreamInfo(key);
    IBatch IBatchStreamCommands.StreamGroupInfo(ValkeyKey key) => StreamGroupInfo(key);
    IBatch IBatchStreamCommands.StreamConsumerInfo(ValkeyKey key, ValkeyValue groupName) => StreamConsumerInfo(key, groupName);
}
