// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Batch implementation for cluster <see cref="GlideClusterClient" />. Batches allow the execution of a group
/// of commands in a single step.
/// <para />
/// Batch Response: An <c>array</c> of command responses is returned by the client <see cref="GlideClusterClient.Exec(ClusterBatch, bool)" />
/// and <see cref="GlideClusterClient.Exec(ClusterBatch, bool, Options.ClusterBatchOptions)" /> API, in the order they were given.
/// Each element in the array represents a command given to the <c>Batch</c>. The response for each command depends on the
/// executed Valkey command. Specific response types are documented alongside each method.
/// <para />
/// See the <see href="https://valkey.io/topics/transactions/">Valkey Transactions (Atomic Batches)</see>.<br />
/// See the <see href="https://valkey.io/topics/pipelining/">Valkey Pipelines (Non-Atomic Batches)</see>.
/// </summary>
/// <remarks>
/// <inheritdoc cref="GlideClusterClient.Exec(ClusterBatch, bool)" path="/remarks/example" />
/// </remarks>
/// <param name="isAtomic">
/// <inheritdoc cref="BaseBatch{T}.BaseBatch(bool)" />
/// </param>
public sealed class ClusterBatch(bool isAtomic) : BaseBatch<ClusterBatch>(isAtomic), IBatch
{
    // Explicit interface implementations for IBatchStreamCommands
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
