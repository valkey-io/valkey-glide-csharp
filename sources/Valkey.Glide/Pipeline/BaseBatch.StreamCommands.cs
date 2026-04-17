// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    #region StreamAdd

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue) =>
        StreamAdd(key, streamField, streamValue, new StreamAddOptions());

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, IEnumerable{NameValueEntry})" />
    public T StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs) =>
        StreamAdd(key, streamPairs, new StreamAddOptions());

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, ValkeyValue, ValkeyValue, StreamAddOptions)" />
    public T StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options) =>
        AddCmd(Request.StreamAddAsync(key, [new NameValueEntry(streamField, streamValue)], options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamAdd(ValkeyKey, IEnumerable{NameValueEntry}, StreamAddOptions)" />
    public T StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options) =>
        AddCmd(Request.StreamAddAsync(key, [.. streamPairs], options));

    #endregion
    #region StreamRead

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(StreamPosition)" />
    public T StreamRead(StreamPosition position) =>
        StreamRead(position, new StreamReadOptions());

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(IEnumerable{StreamPosition})" />
    public T StreamRead(IEnumerable<StreamPosition> streamPositions) =>
        StreamRead(streamPositions, new StreamReadOptions());

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(StreamPosition, StreamReadOptions)" />
    public T StreamRead(StreamPosition position, StreamReadOptions options) =>
        AddCmd(Request.StreamReadAsync(position, options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamRead(IEnumerable{StreamPosition}, StreamReadOptions)" />
    public T StreamRead(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options) =>
        AddCmd(Request.StreamReadAsync(streamPositions, options));

    #endregion
    #region StreamLength

    /// <inheritdoc cref="IBatchStreamCommands.StreamLength(ValkeyKey)" />
    public T StreamLength(ValkeyKey key) => AddCmd(Request.StreamLengthAsync(key));

    #endregion
    #region StreamDelete

    /// <inheritdoc cref="IBatchStreamCommands.StreamDelete(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds) => AddCmd(Request.StreamDeleteAsync(key, [.. messageIds]));

    /// <inheritdoc cref="IBatchStreamCommands.StreamDelete(ValkeyKey, ValkeyValue)" />
    public T StreamDelete(ValkeyKey key, ValkeyValue messageId) => AddCmd(Request.StreamDeleteAsync(key, messageId));

    #endregion
    #region StreamRange

    /// <inheritdoc cref="IBatchStreamCommands.StreamRange(ValkeyKey)" />
    public T StreamRange(ValkeyKey key) =>
        StreamRange(key, new StreamRangeOptions());

    /// <inheritdoc cref="IBatchStreamCommands.StreamRange(ValkeyKey, StreamRangeOptions)" />
    public T StreamRange(ValkeyKey key, StreamRangeOptions options) =>
        AddCmd(Request.StreamRangeAsync(key, options));

    #endregion
    #region StreamReadGroup

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(StreamPosition, ValkeyValue, ValkeyValue)" />
    public T StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName) =>
        StreamReadGroup(position, groupName, consumerName, new StreamReadGroupOptions());

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)" />
    public T StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName) =>
        StreamReadGroup(positions, groupName, consumerName, new StreamReadGroupOptions());

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(StreamPosition, ValkeyValue, ValkeyValue, StreamReadGroupOptions)" />
    public T StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options) =>
        AddCmd(Request.StreamReadGroupSingleAsync(position, groupName, consumerName, options));

    /// <inheritdoc cref="IBatchStreamCommands.StreamReadGroup(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, StreamReadGroupOptions)" />
    public T StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options) =>
        AddCmd(Request.StreamReadGroupMultiAsync(positions, groupName, consumerName, options));

    #endregion
    #region Explicit interface implementations

    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue) => StreamAdd(key, streamField, streamValue);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs) => StreamAdd(key, streamPairs);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options) => StreamAdd(key, streamField, streamValue, options);
    IBatch IBatchStreamCommands.StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options) => StreamAdd(key, streamPairs, options);
    IBatch IBatchStreamCommands.StreamRead(StreamPosition position) => StreamRead(position);
    IBatch IBatchStreamCommands.StreamRead(IEnumerable<StreamPosition> streamPositions) => StreamRead(streamPositions);
    IBatch IBatchStreamCommands.StreamRead(StreamPosition position, StreamReadOptions options) => StreamRead(position, options);
    IBatch IBatchStreamCommands.StreamRead(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options) => StreamRead(streamPositions, options);
    IBatch IBatchStreamCommands.StreamLength(ValkeyKey key) => StreamLength(key);
    IBatch IBatchStreamCommands.StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds) => StreamDelete(key, messageIds);
    IBatch IBatchStreamCommands.StreamDelete(ValkeyKey key, ValkeyValue messageId) => StreamDelete(key, messageId);
    IBatch IBatchStreamCommands.StreamRange(ValkeyKey key) => StreamRange(key);
    IBatch IBatchStreamCommands.StreamRange(ValkeyKey key, StreamRangeOptions options) => StreamRange(key, options);
    IBatch IBatchStreamCommands.StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName) => StreamReadGroup(position, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName) => StreamReadGroup(positions, groupName, consumerName);
    IBatch IBatchStreamCommands.StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options) => StreamReadGroup(position, groupName, consumerName, options);
    IBatch IBatchStreamCommands.StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options) => StreamReadGroup(positions, groupName, consumerName, options);

    #endregion
}
