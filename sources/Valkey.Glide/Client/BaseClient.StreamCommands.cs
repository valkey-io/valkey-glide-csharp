// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    #region StreamAddAsync

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue)
        => StreamAddAsync(key, streamField, streamValue, new StreamAddOptions());

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry})"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs)
        => StreamAddAsync(key, streamPairs, new StreamAddOptions());

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamAddOptions)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options)
        => Command(Request.StreamAdd(key, [new NameValueEntry(streamField, streamValue)], options));

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, StreamAddOptions)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options)
        => Command(Request.StreamAdd(key, [.. streamPairs], options));

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
        => Command(Request.StreamRead(position, options));

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition}, StreamReadOptions)"/>
    public Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options)
        => Command(Request.StreamRead(streamPositions, options));

    #endregion
    #region StreamRangeAsync

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key)
        => StreamRangeAsync(key, new StreamRangeOptions());

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey, StreamRangeOptions)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions options)
        => Command(Request.StreamRange(key, options));

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
        => Command(Request.StreamReadGroup(position, groupName, consumerName, options));

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, StreamReadGroupOptions)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)
        => Command(Request.StreamReadGroup(positions, groupName, consumerName, options));

    #endregion
    #region StreamLengthAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamLengthAsync(ValkeyKey)"/>
    public Task<long> StreamLengthAsync(ValkeyKey key)
        => Command(Request.StreamLength(key));

    #endregion
    #region StreamDeleteAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)
        => Command(Request.StreamDelete(key, [.. messageIds]));

    /// <inheritdoc cref="IBaseClient.StreamDeleteAsync(ValkeyKey, ValkeyValue)"/>
    public Task<bool> StreamDeleteAsync(ValkeyKey key, ValkeyValue messageId)
        => Command(Request.StreamDelete(key, messageId));

    #endregion

    // TODO #326: Redesign and expose these methods

    // ──────────────────────────────────────────────────────────────────────
    // Methods below are temporarily commented out pending cleanup.
    // The underlying Request methods are kept intact.
    // ──────────────────────────────────────────────────────────────────────

    // #region XGROUP CREATE
    // public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position)
    //     => await StreamCreateConsumerGroupAsync(key, groupName, position, false, null);
    //
    // public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null)
    //     => await Command(Request.StreamCreateConsumerGroup(key, groupName, position ?? default, createStream, entriesRead));
    // #endregion

    // #region XGROUP DESTROY / CREATECONSUMER / DELCONSUMER
    // public async Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName)
    //     => await Command(Request.StreamDeleteConsumerGroup(key, groupName));
    //
    // public async Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
    //     => await Command(Request.StreamCreateConsumer(key, groupName, consumerName));
    //
    // public async Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
    //     => await Command(Request.StreamDeleteConsumer(key, groupName, consumerName));
    // #endregion

    // #region XGROUP SETID
    // public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null)
    //     => await Command(Request.StreamConsumerGroupSetPosition(key, groupName, position, entriesRead));
    // #endregion

    // #region XACK
    // public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId)
    //     => await StreamAcknowledgeAsync(key, groupName, [messageId]);
    //
    // public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds)
    //     => await Command(Request.StreamAcknowledge(key, groupName, [.. messageIds]));
    // #endregion

    // #region XPENDING
    // public async Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName)
    //     => await Command(Request.StreamPending(key, groupName));
    //
    // public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId)
    //     => await StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, null);
    //
    // public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName = default, ValkeyValue? minId = null, ValkeyValue? maxId = null, TimeSpan? minIdleTime = null)
    //     => await Command(Request.StreamPendingMessages(key, groupName, minId ?? "-", maxId ?? "+", count, consumerName, minIdleTime));
    // #endregion

    // #region XCLAIM
    // public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds)
    //     => await Command(Request.StreamClaim(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds]));
    //
    // public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
    //     => await Command(Request.StreamClaim(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds], options));
    //
    // public async Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds)
    //     => await Command(Request.StreamClaimIdsOnly(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds]));
    //
    // public async Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
    //     => await Command(Request.StreamClaimIdsOnly(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds], options));
    // #endregion

    // #region XAUTOCLAIM
    // public async Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null)
    //     => await Command(Request.StreamAutoClaim(key, consumerGroup, claimingConsumer, minIdleTime, startAtId, count));
    //
    // public async Task<StreamAutoClaimJustIdResult> StreamAutoClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null)
    //     => await Command(Request.StreamAutoClaimJustId(key, consumerGroup, claimingConsumer, minIdleTime, startAtId, count));
    // #endregion

    #region StreamTrimAsync

    /// <inheritdoc cref="IBaseClient.StreamTrimAsync(ValkeyKey, StreamTrimOptions)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, StreamTrimOptions options)
        => Command(Request.StreamTrim(key, options));

    #endregion

    // #region XINFO
    // public async Task<StreamInfo> StreamInfoAsync(ValkeyKey key)
    //     => await Command(Request.StreamInfo(key));
    //
    // public async Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key)
    //     => await Command(Request.StreamGroupInfo(key));
    //
    // public async Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName)
    //     => await Command(Request.StreamConsumerInfo(key, groupName));
    //
    // public async Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key, int? count = null)
    //     => await Command(Request.StreamInfoFull(key, count));
    // #endregion
}
