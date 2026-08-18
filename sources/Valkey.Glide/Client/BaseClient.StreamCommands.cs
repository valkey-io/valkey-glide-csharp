// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    #region StreamAcknowledgeAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task<bool> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId)
        => Command(Request.StreamAcknowledge(key, groupName, messageId));

    /// <inheritdoc cref="IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})"/>
    public Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds)
        => Command(Request.StreamAcknowledge(key, groupName, [.. messageIds]));

    #endregion
    #region StreamAddAsync

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue)
        => Command(Request.StreamAdd(key, [new NameValueEntry(streamField, streamValue)]));

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry})"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs)
        => Command(Request.StreamAdd(key, streamPairs));

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamAddOptions)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options)
        => Command(Request.StreamAdd(key, [new NameValueEntry(streamField, streamValue)], options));

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, StreamAddOptions)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options)
        => Command(Request.StreamAdd(key, streamPairs, options));

    #endregion
    #region StreamAutoClaimAsync

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamAutoClaimOptions)"/>
    public Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options)
        => Command(Request.StreamAutoClaim(key, consumerGroup, claimingConsumer, options));

    #endregion
    #region StreamAutoClaimJustIdAsync

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamAutoClaimOptions)"/>
    public Task<StreamAutoClaimJustIdResult> StreamAutoClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options)
        => Command(Request.StreamAutoClaimJustId(key, consumerGroup, claimingConsumer, options));

    #endregion
    #region StreamClaimAsync

    /// <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, StreamClaimOptions)"/>
    public Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options)
        => Command(Request.StreamClaim(key, consumerGroup, claimingConsumer, [messageId], options));

    /// <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, StreamClaimOptions)"/>
    public Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => Command(Request.StreamClaim(key, consumerGroup, claimingConsumer, messageIds, options));

    #endregion
    #region StreamClaimJustIdAsync

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, StreamClaimOptions)"/>
    public Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options)
        => Command(Request.StreamClaimJustIds(key, consumerGroup, claimingConsumer, [messageId], options));

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, StreamClaimOptions)"/>
    public Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => Command(Request.StreamClaimJustIds(key, consumerGroup, claimingConsumer, messageIds, options));

    #endregion
    #region StreamDeleteAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, ValkeyValue)"/>
    public Task<bool> StreamDeleteAsync(ValkeyKey key, ValkeyValue messageId)
        => Command(Request.StreamDelete(key, messageId));

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)
        => Command(Request.StreamDelete(key, [.. messageIds]));

    #endregion
    #region StreamGroupCreateAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task StreamGroupCreateAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position)
        => Command(Request.StreamGroupCreate(key, groupName, position));

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamGroupCreateOptions)"/>
    public Task StreamGroupCreateAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, StreamGroupCreateOptions options)
        => Command(Request.StreamGroupCreate(key, groupName, position, options));

    #endregion
    #region StreamGroupCreateConsumerAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task<bool> StreamGroupCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => Command(Request.StreamGroupCreateConsumer(key, groupName, consumerName));

    #endregion
    #region StreamGroupDeleteConsumerAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task<long> StreamGroupDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => Command(Request.StreamGroupDeleteConsumer(key, groupName, consumerName));

    #endregion
    #region StreamGroupDestroyAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupDestroyAsync(ValkeyKey, ValkeyValue)"/>
    public Task<bool> StreamGroupDestroyAsync(ValkeyKey key, ValkeyValue groupName)
        => Command(Request.StreamGroupDestroy(key, groupName));

    #endregion
    #region StreamGroupSetIdAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task StreamGroupSetIdAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position)
        => Command(Request.StreamGroupSetId(key, groupName, position));

    /// <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, long)"/>
    public Task StreamGroupSetIdAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long entriesRead)
        => Command(Request.StreamGroupSetId(key, groupName, position, entriesRead));

    #endregion
    #region StreamInfoAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamInfoAsync(ValkeyKey)"/>
    public Task<StreamInfo> StreamInfoAsync(ValkeyKey key)
        => Command(Request.StreamInfo(key));

    #endregion
    #region StreamInfoConsumersAsync

    /// <inheritdoc cref="IBaseClient.StreamInfoConsumersAsync(ValkeyKey, ValkeyValue)"/>
    public Task<StreamConsumerInfo[]> StreamInfoConsumersAsync(ValkeyKey key, ValkeyValue groupName)
        => Command(Request.StreamInfoConsumers(key, groupName));

    #endregion
    #region StreamInfoFullAsync

    /// <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey)"/>
    public Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key)
        => Command(Request.StreamInfoFull(key));

    /// <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey, int)"/>
    public Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key, int count)
        => Command(Request.StreamInfoFull(key, count));

    #endregion
    #region StreamInfoGroupsAsync

    /// <inheritdoc cref="IBaseClient.StreamInfoGroupsAsync(ValkeyKey)"/>
    public Task<StreamGroupInfo[]> StreamInfoGroupsAsync(ValkeyKey key)
        => Command(Request.StreamInfoGroups(key));

    #endregion
    #region StreamLengthAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamLengthAsync(ValkeyKey)"/>
    public Task<long> StreamLengthAsync(ValkeyKey key)
        => Command(Request.StreamLength(key));

    #endregion
    #region StreamPendingAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)"/>
    public Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName)
        => Command(Request.StreamPending(key, groupName));

    /// <inheritdoc cref="IBaseClient.StreamPendingAsync(ValkeyKey, ValkeyValue, StreamPendingOptions)"/>
    public Task<StreamPendingMessageInfo[]> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, StreamPendingOptions options)
        => Command(Request.StreamPending(key, groupName, options));

    #endregion
    #region StreamRangeAsync

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key)
        => Command(Request.StreamRange(key));

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey, StreamRangeOptions)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions options)
        => Command(Request.StreamRange(key, options));

    #endregion
    #region StreamReadAsync

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition)"/>
    public Task<StreamEntry[]> StreamReadAsync(StreamPosition position)
        => Command(Request.StreamRead(position));

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition})"/>
    public Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions)
        => Command(Request.StreamRead(streamPositions));

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition, StreamReadOptions)"/>
    public Task<StreamEntry[]> StreamReadAsync(StreamPosition position, StreamReadOptions options)
        => Command(Request.StreamRead(position, options));

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition}, StreamReadOptions)"/>
    public Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options)
        => Command(Request.StreamRead(streamPositions, options));

    #endregion
    #region StreamReadGroupAsync

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName)
        => Command(Request.StreamReadGroup(position, groupName, consumerName));

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName)
        => Command(Request.StreamReadGroup(positions, groupName, consumerName));

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue, StreamReadGroupOptions)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)
        => Command(Request.StreamReadGroup(position, groupName, consumerName, options));

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, StreamReadGroupOptions)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)
        => Command(Request.StreamReadGroup(positions, groupName, consumerName, options));

    #endregion
    #region StreamTrimAsync

    /// <inheritdoc cref="IBaseClient.StreamTrimAsync(ValkeyKey, StreamTrimOptions)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, StreamTrimOptions options)
        => Command(Request.StreamTrim(key, options));

    #endregion
}
