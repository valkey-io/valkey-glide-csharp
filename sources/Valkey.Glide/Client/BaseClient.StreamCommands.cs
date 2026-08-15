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
        => Command(Request.StreamAcknowledgeAsync(key, groupName, messageId));

    /// <inheritdoc cref="IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})"/>
    public Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds)
        => Command(Request.StreamAcknowledgeAsync(key, groupName, [.. messageIds]));

    #endregion
    #region StreamAddAsync

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue)
        => StreamAddAsync(key, streamField, streamValue, new StreamAddOptions());

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry})"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs)
        => StreamAddAsync(key, streamPairs, new StreamAddOptions());

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamAddOptions)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options)
        => Command(Request.StreamAddAsync(key, [new NameValueEntry(streamField, streamValue)], options));

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, StreamAddOptions)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options)
        => Command(Request.StreamAddAsync(key, streamPairs, options));

    #endregion
    #region StreamAutoClaimAsync

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamAutoClaimOptions)"/>
    public Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options)
        => Command(Request.StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, options));

    #endregion
    #region StreamAutoClaimJustIdAsync

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamAutoClaimOptions)"/>
    public Task<StreamAutoClaimJustIdResult> StreamAutoClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options)
        => Command(Request.StreamAutoClaimJustIdAsync(key, consumerGroup, claimingConsumer, options));

    #endregion
    #region StreamClaimAsync

    /// <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, StreamClaimOptions)"/>
    public Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options)
        => StreamClaimAsync(key, consumerGroup, claimingConsumer, [messageId], options);

    /// <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, StreamClaimOptions)"/>
    public Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => Command(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, messageIds, options));

    #endregion
    #region StreamClaimJustIdAsync

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, StreamClaimOptions)"/>
    public Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options)
        => StreamClaimJustIdAsync(key, consumerGroup, claimingConsumer, [messageId], options);

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, StreamClaimOptions)"/>
    public Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
        => Command(Request.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, messageIds, options));

    #endregion
    #region StreamDeleteAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)
        => Command(Request.StreamDeleteAsync(key, [.. messageIds]));

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, ValkeyValue)"/>
    public Task<bool> StreamDeleteAsync(ValkeyKey key, ValkeyValue messageId)
        => Command(Request.StreamDeleteAsync(key, messageId));

    #endregion
    #region StreamGroupCreateAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task StreamGroupCreateAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position)
        => StreamGroupCreateAsync(key, groupName, position, new StreamGroupCreateOptions());

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamGroupCreateOptions)"/>
    public Task StreamGroupCreateAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, StreamGroupCreateOptions options)
        => Command(Request.StreamGroupCreateAsync(key, groupName, position, options));

    #endregion
    #region StreamGroupCreateConsumerAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task<bool> StreamGroupCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => Command(Request.StreamGroupCreateConsumerAsync(key, groupName, consumerName));

    #endregion
    #region StreamGroupDeleteConsumerAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task<long> StreamGroupDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
        => Command(Request.StreamGroupDeleteConsumerAsync(key, groupName, consumerName));

    #endregion
    #region StreamGroupDestroyAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupDestroyAsync(ValkeyKey, ValkeyValue)"/>
    public Task<bool> StreamGroupDestroyAsync(ValkeyKey key, ValkeyValue groupName)
        => Command(Request.StreamGroupDestroyAsync(key, groupName));

    #endregion
    #region StreamGroupSetIdAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task StreamGroupSetIdAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position)
        => Command(Request.StreamGroupSetIdAsync(key, groupName, position, null));

    /// <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, long)"/>
    public Task StreamGroupSetIdAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long entriesRead)
        => Command(Request.StreamGroupSetIdAsync(key, groupName, position, entriesRead));

    #endregion
    #region StreamInfoAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamInfoAsync(ValkeyKey)"/>
    public Task<StreamInfo> StreamInfoAsync(ValkeyKey key)
        => Command(Request.StreamInfoAsync(key));

    #endregion
    #region StreamInfoConsumersAsync

    /// <inheritdoc cref="IBaseClient.StreamInfoConsumersAsync(ValkeyKey, ValkeyValue)"/>
    public Task<StreamConsumerInfo[]> StreamInfoConsumersAsync(ValkeyKey key, ValkeyValue groupName)
        => Command(Request.StreamInfoConsumersAsync(key, groupName));

    #endregion
    #region StreamInfoFullAsync

    /// <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey)"/>
    public Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key)
        => Command(Request.StreamInfoFullAsync(key, null));

    /// <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey, int)"/>
    public Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key, int count)
        => Command(Request.StreamInfoFullAsync(key, count));

    #endregion
    #region StreamInfoGroupsAsync

    /// <inheritdoc cref="IBaseClient.StreamInfoGroupsAsync(ValkeyKey)"/>
    public Task<StreamGroupInfo[]> StreamInfoGroupsAsync(ValkeyKey key)
        => Command(Request.StreamInfoGroupsAsync(key));

    #endregion
    #region StreamLengthAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamLengthAsync(ValkeyKey)"/>
    public Task<long> StreamLengthAsync(ValkeyKey key)
        => Command(Request.StreamLengthAsync(key));

    #endregion
    #region StreamPendingAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)"/>
    public Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName)
        => Command(Request.StreamPendingAsync(key, groupName));

    /// <inheritdoc cref="IBaseClient.StreamPendingAsync(ValkeyKey, ValkeyValue, StreamPendingOptions)"/>
    public Task<StreamPendingMessageInfo[]> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, StreamPendingOptions options)
        => Command(Request.StreamPendingMessagesAsync(key, groupName, options.Start.Value, options.End.Value, options.Count, options.ConsumerName, options.MinIdleTime));

    #endregion
    #region StreamRangeAsync

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key)
        => StreamRangeAsync(key, new StreamRangeOptions());

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey, StreamRangeOptions)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions options)
        => Command(Request.StreamRangeAsync(key, options));

    #endregion
    #region StreamReadAsync

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition)"/>
    public Task<StreamEntry[]> StreamReadAsync(StreamPosition position)
        => StreamReadAsync(position, new StreamReadOptions());

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition})"/>
    public Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions)
        => StreamReadAsync(streamPositions, new StreamReadOptions());

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition, StreamReadOptions)"/>
    public Task<StreamEntry[]> StreamReadAsync(StreamPosition position, StreamReadOptions options)
        => Command(Request.StreamReadAsync(position, options));

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition}, StreamReadOptions)"/>
    public Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options)
        => Command(Request.StreamReadAsync(streamPositions, options));

    #endregion
    #region StreamReadGroupAsync

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName)
        => StreamReadGroupAsync(position, groupName, consumerName, new StreamReadGroupOptions());

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName)
        => StreamReadGroupAsync(positions, groupName, consumerName, new StreamReadGroupOptions());

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue, StreamReadGroupOptions)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)
        => Command(Request.StreamReadGroupAsync(position, groupName, consumerName, options));

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, StreamReadGroupOptions)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)
        => Command(Request.StreamReadGroupAsync(positions, groupName, consumerName, options));

    #endregion
    #region StreamTrimAsync

    /// <inheritdoc cref="IBaseClient.StreamTrimAsync(ValkeyKey, StreamTrimOptions)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, StreamTrimOptions options)
        => Command(Request.StreamTrimAsync(key, options));

    #endregion
}
