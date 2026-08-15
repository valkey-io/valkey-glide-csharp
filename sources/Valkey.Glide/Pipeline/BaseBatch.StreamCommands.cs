// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    #region StreamAcknowledge

    /// <inheritdoc cref="IBatchStreamCommands.StreamAcknowledge(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId)
        => AddCmd(Request.StreamAcknowledge(key, groupName, messageId));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAcknowledge(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" />
    public T StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds)
        => AddCmd(Request.StreamAcknowledge(key, groupName, [.. messageIds]));

    #endregion
    #region StreamAdd

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue)
        => AddCmd(Request.StreamAdd(key, [new NameValueEntry(streamField, streamValue)]));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, IEnumerable{NameValueEntry})" />
    public T StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs)
        => AddCmd(Request.StreamAdd(key, streamPairs));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, ValkeyValue, ValkeyValue, StreamAddOptions)" />
    public T StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options)
        => AddCmd(Request.StreamAdd(key, [new NameValueEntry(streamField, streamValue)], options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, IEnumerable{NameValueEntry}, StreamAddOptions)" />
    public T StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options)
        => AddCmd(Request.StreamAdd(key, streamPairs, options));

    #endregion
    #region StreamAutoClaim

    /// <inheritdoc cref="IBatchStreamCommands.StreamAutoClaim(ValkeyKey, ValkeyValue, ValkeyValue, StreamAutoClaimOptions)" />
    public T StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options)
        => AddCmd(Request.StreamAutoClaim(key, consumerGroup, claimingConsumer, options));

    #endregion
    #region StreamAutoClaimJustId

    /// <inheritdoc cref="IBatchStreamCommands.StreamAutoClaimJustId(ValkeyKey, ValkeyValue, ValkeyValue, StreamAutoClaimOptions)" />
    public T StreamAutoClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options)
        => AddCmd(Request.StreamAutoClaimJustId(key, consumerGroup, claimingConsumer, options));

    #endregion
    #region StreamClaim

    /// <inheritdoc cref="IBatchStreamCommands.StreamClaim(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, StreamClaimOptions)" />
    public T StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options)
        => AddCmd(Request.StreamClaim(key, consumerGroup, claimingConsumer, [messageId], options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamClaim(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, StreamClaimOptions)" />
    public T StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => AddCmd(Request.StreamClaim(key, consumerGroup, claimingConsumer, messageIds, options));

    #endregion
    #region StreamClaimJustId

    /// <inheritdoc cref="IBatchStreamCommands.StreamClaimJustId(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, StreamClaimOptions)" />
    public T StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options)
        => AddCmd(Request.StreamClaimIdsOnly(key, consumerGroup, claimingConsumer, [messageId], options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamClaimJustId(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, StreamClaimOptions)" />
    public T StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => AddCmd(Request.StreamClaimIdsOnly(key, consumerGroup, claimingConsumer, messageIds, options));

    #endregion
    #region StreamDelete

    /// <inheritdoc cref="IBatchStreamCommands.StreamDelete(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)
        => AddCmd(Request.StreamDelete(key, [.. messageIds]));

    /// <inheritdoc cref="IBatchStreamCommands.StreamDelete(ValkeyKey, ValkeyValue)" />
    public T StreamDelete(ValkeyKey key, ValkeyValue messageId)
        => AddCmd(Request.StreamDelete(key, messageId));

    #endregion
    #region StreamGroupCreate

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupCreate(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamGroupCreate(ValkeyKey key, ValkeyValue groupName, ValkeyValue position)
        => AddCmd(Request.StreamGroupCreate(key, groupName, position));

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupCreate(ValkeyKey, ValkeyValue, ValkeyValue, StreamGroupCreateOptions)" />
    public T StreamGroupCreate(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, StreamGroupCreateOptions options)
        => AddCmd(Request.StreamGroupCreate(key, groupName, position, options));

    #endregion
    #region StreamGroupCreateConsumer

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupCreateConsumer(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamGroupCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => AddCmd(Request.StreamGroupCreateConsumer(key, groupName, consumerName));

    #endregion
    #region StreamGroupDeleteConsumer

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupDeleteConsumer(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamGroupDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => AddCmd(Request.StreamGroupDeleteConsumer(key, groupName, consumerName));

    #endregion
    #region StreamGroupDestroy

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupDestroy(ValkeyKey, ValkeyValue)" />
    public T StreamGroupDestroy(ValkeyKey key, ValkeyValue groupName)
        => AddCmd(Request.StreamGroupDestroy(key, groupName));

    #endregion
    #region StreamGroupSetId

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupSetId(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamGroupSetId(ValkeyKey key, ValkeyValue groupName, ValkeyValue position)
        => AddCmd(Request.StreamGroupSetId(key, groupName, position));

    /// <inheritdoc cref="IBatchStreamCommands.StreamGroupSetId(ValkeyKey, ValkeyValue, ValkeyValue, long)" />
    public T StreamGroupSetId(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long entriesRead)
        => AddCmd(Request.StreamGroupSetId(key, groupName, position, entriesRead));

    #endregion
    #region StreamInfo

    /// <inheritdoc cref="IBatchStreamCommands.StreamInfo(ValkeyKey)" />
    public T StreamInfo(ValkeyKey key)
        => AddCmd(Request.StreamInfo(key));

    #endregion
    #region StreamInfoConsumers

    /// <inheritdoc cref="IBatchStreamCommands.StreamInfoConsumers(ValkeyKey, ValkeyValue)" />
    public T StreamInfoConsumers(ValkeyKey key, ValkeyValue groupName)
        => AddCmd(Request.StreamInfoConsumers(key, groupName));

    #endregion
    #region StreamInfoFull

    /// <inheritdoc cref="IBatchStreamCommands.StreamInfoFull(ValkeyKey)" />
    public T StreamInfoFull(ValkeyKey key)
        => AddCmd(Request.StreamInfoFull(key));

    /// <inheritdoc cref="IBatchStreamCommands.StreamInfoFull(ValkeyKey, int)" />
    public T StreamInfoFull(ValkeyKey key, int count)
        => AddCmd(Request.StreamInfoFull(key, count));

    #endregion
    #region StreamInfoGroups

    /// <inheritdoc cref="IBatchStreamCommands.StreamInfoGroups(ValkeyKey)" />
    public T StreamInfoGroups(ValkeyKey key)
        => AddCmd(Request.StreamInfoGroups(key));

    #endregion
    #region StreamLength

    /// <inheritdoc cref="IBatchStreamCommands.StreamLength(ValkeyKey)" />
    public T StreamLength(ValkeyKey key)
        => AddCmd(Request.StreamLength(key));

    #endregion
    #region StreamPending

    /// <inheritdoc cref="IBatchStreamCommands.StreamPending(ValkeyKey, ValkeyValue)" />
    public T StreamPending(ValkeyKey key, ValkeyValue groupName)
        => AddCmd(Request.StreamPending(key, groupName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamPending(ValkeyKey, ValkeyValue, StreamPendingOptions)" />
    public T StreamPending(ValkeyKey key, ValkeyValue groupName, StreamPendingOptions options)
        => AddCmd(Request.StreamPending(key, groupName, options));

    #endregion
    #region StreamRange

    /// <inheritdoc cref="IBatchStreamCommands.StreamRange(ValkeyKey)" />
    public T StreamRange(ValkeyKey key)
        => AddCmd(Request.StreamRange(key));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRange(ValkeyKey, StreamRangeOptions)" />
    public T StreamRange(ValkeyKey key, StreamRangeOptions options)
        => AddCmd(Request.StreamRange(key, options));

    #endregion
    #region StreamRead

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(StreamPosition)" />
    public T StreamRead(StreamPosition position)
        => AddCmd(Request.StreamRead(position));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(IEnumerable{StreamPosition})" />
    public T StreamRead(IEnumerable<StreamPosition> streamPositions)
        => AddCmd(Request.StreamRead(streamPositions));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(StreamPosition, StreamReadOptions)" />
    public T StreamRead(StreamPosition position, StreamReadOptions options)
        => AddCmd(Request.StreamRead(position, options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(IEnumerable{StreamPosition}, StreamReadOptions)" />
    public T StreamRead(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options)
        => AddCmd(Request.StreamRead(streamPositions, options));

    #endregion
    #region StreamReadGroup

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(StreamPosition, ValkeyValue, ValkeyValue)" />
    public T StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName)
        => AddCmd(Request.StreamReadGroup(position, groupName, consumerName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)" />
    public T StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName)
        => AddCmd(Request.StreamReadGroup(positions, groupName, consumerName));

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(StreamPosition, ValkeyValue, ValkeyValue, StreamReadGroupOptions)" />
    public T StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)
        => AddCmd(Request.StreamReadGroup(position, groupName, consumerName, options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, StreamReadGroupOptions)" />
    public T StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)
        => AddCmd(Request.StreamReadGroup(positions, groupName, consumerName, options));

    #endregion
    #region StreamTrim

    /// <inheritdoc cref="IBatchStreamCommands.StreamTrim(ValkeyKey, StreamTrimOptions)" />
    public T StreamTrim(ValkeyKey key, StreamTrimOptions options)
        => AddCmd(Request.StreamTrim(key, options));

    #endregion
    #region Explicit interface implementations

    IBatch IBatchStreamCommands.StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds) => StreamAcknowledge(key, groupName, messageIds);
    IBatch IBatchStreamCommands.StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId) => StreamAcknowledge(key, groupName, messageId);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs) => StreamAdd(key, streamPairs);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options) => StreamAdd(key, streamPairs, options);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue) => StreamAdd(key, streamField, streamValue);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options) => StreamAdd(key, streamField, streamValue, options);
    IBatch IBatchStreamCommands.StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options) => StreamAutoClaim(key, consumerGroup, claimingConsumer, options);
    IBatch IBatchStreamCommands.StreamAutoClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options) => StreamAutoClaimJustId(key, consumerGroup, claimingConsumer, options);
    IBatch IBatchStreamCommands.StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options) => StreamClaim(key, consumerGroup, claimingConsumer, messageIds, options);
    IBatch IBatchStreamCommands.StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options) => StreamClaim(key, consumerGroup, claimingConsumer, messageId, options);
    IBatch IBatchStreamCommands.StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options) => StreamClaimJustId(key, consumerGroup, claimingConsumer, messageIds, options);
    IBatch IBatchStreamCommands.StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options) => StreamClaimJustId(key, consumerGroup, claimingConsumer, messageId, options);
    IBatch IBatchStreamCommands.StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds) => StreamDelete(key, messageIds);
    IBatch IBatchStreamCommands.StreamDelete(ValkeyKey key, ValkeyValue messageId) => StreamDelete(key, messageId);
    IBatch IBatchStreamCommands.StreamGroupCreate(ValkeyKey key, ValkeyValue groupName, ValkeyValue position) => StreamGroupCreate(key, groupName, position);
    IBatch IBatchStreamCommands.StreamGroupCreate(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, StreamGroupCreateOptions options) => StreamGroupCreate(key, groupName, position, options);
    IBatch IBatchStreamCommands.StreamGroupCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName) => StreamGroupCreateConsumer(key, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamGroupDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName) => StreamGroupDeleteConsumer(key, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamGroupDestroy(ValkeyKey key, ValkeyValue groupName) => StreamGroupDestroy(key, groupName);
    IBatch IBatchStreamCommands.StreamGroupSetId(ValkeyKey key, ValkeyValue groupName, ValkeyValue position) => StreamGroupSetId(key, groupName, position);
    IBatch IBatchStreamCommands.StreamGroupSetId(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long entriesRead) => StreamGroupSetId(key, groupName, position, entriesRead);
    IBatch IBatchStreamCommands.StreamInfo(ValkeyKey key) => StreamInfo(key);
    IBatch IBatchStreamCommands.StreamInfoConsumers(ValkeyKey key, ValkeyValue groupName) => StreamInfoConsumers(key, groupName);
    IBatch IBatchStreamCommands.StreamInfoFull(ValkeyKey key) => StreamInfoFull(key);
    IBatch IBatchStreamCommands.StreamInfoFull(ValkeyKey key, int count) => StreamInfoFull(key, count);
    IBatch IBatchStreamCommands.StreamInfoGroups(ValkeyKey key) => StreamInfoGroups(key);
    IBatch IBatchStreamCommands.StreamLength(ValkeyKey key) => StreamLength(key);
    IBatch IBatchStreamCommands.StreamPending(ValkeyKey key, ValkeyValue groupName) => StreamPending(key, groupName);
    IBatch IBatchStreamCommands.StreamPending(ValkeyKey key, ValkeyValue groupName, StreamPendingOptions options) => StreamPending(key, groupName, options);
    IBatch IBatchStreamCommands.StreamRange(ValkeyKey key) => StreamRange(key);
    IBatch IBatchStreamCommands.StreamRange(ValkeyKey key, StreamRangeOptions options) => StreamRange(key, options);
    IBatch IBatchStreamCommands.StreamRead(IEnumerable<StreamPosition> streamPositions) => StreamRead(streamPositions);
    IBatch IBatchStreamCommands.StreamRead(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options) => StreamRead(streamPositions, options);
    IBatch IBatchStreamCommands.StreamRead(StreamPosition position) => StreamRead(position);
    IBatch IBatchStreamCommands.StreamRead(StreamPosition position, StreamReadOptions options) => StreamRead(position, options);
    IBatch IBatchStreamCommands.StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName) => StreamReadGroup(positions, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options) => StreamReadGroup(positions, groupName, consumerName, options);
    IBatch IBatchStreamCommands.StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName) => StreamReadGroup(position, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options) => StreamReadGroup(position, groupName, consumerName, options);
    IBatch IBatchStreamCommands.StreamTrim(ValkeyKey key, StreamTrimOptions options) => StreamTrim(key, options);

    #endregion
}
