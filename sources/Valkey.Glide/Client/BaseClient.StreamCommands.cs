// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<ValkeyValue> StreamAddAsync(
        ValkeyKey key,
        ValkeyValue streamField,
        ValkeyValue streamValue,
        ValkeyValue? messageId,
        int? maxLength,
        bool useApproximateMaxLength)
    {
        return await StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, null, false);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StreamAddAsync(
        ValkeyKey key,
        IEnumerable<NameValueEntry> streamPairs,
        ValkeyValue? messageId,
         int? maxLength,
         bool useApproximateMaxLength)
    {
        return await StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, null, false);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StreamAddAsync(
        ValkeyKey key,
        ValkeyValue streamField,
        ValkeyValue streamValue,
        ValkeyValue? messageId = null,
        long? maxLength = null,
        bool useApproximateMaxLength = false,
        long? limit = null,
        bool noMakeStream = false)
    {
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            default,
            useApproximateMaxLength,
            [new NameValueEntry(streamField, streamValue)],
            limit,
            noMakeStream));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StreamAddAsync(
        ValkeyKey key,
        IEnumerable<NameValueEntry> streamPairs,
        ValkeyValue? messageId = null,
        long? maxLength = null,
        bool useApproximateMaxLength = false,
        long? limit = null,
        bool noMakeStream = false)
    {
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            default,
            useApproximateMaxLength,
            [.. streamPairs],
            limit,
            noMakeStream));
    }

    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null)
    {
        return await Command(Request.StreamReadAsync(key, position, count));
    }

    /// <inheritdoc/>
    public async Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? count = null)
    {
        return await Command(Request.StreamReadAsync([.. streamPositions], count));
    }

    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start = null, ValkeyValue? end = null, int? count = null, Order order = Order.Ascending)
    {
        if (!start.HasValue)
        {
            if (order == Order.Ascending)
                start = StreamConstants.ReadMinValue;
            else
                start = StreamConstants.ReadMaxValue;
        }
        if (!end.HasValue)
        {
            if (order == Order.Ascending)
                end = StreamConstants.ReadMaxValue;
            else
                end = StreamConstants.ReadMinValue;
        }
        return await Command(Request.StreamRangeAsync(key, start ?? StreamConstants.ReadMinValue, end ?? StreamConstants.ReadMaxValue, count, order));
    }

    /// <inheritdoc/>
    public async Task StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position)
    {
        await StreamCreateConsumerGroupAsync(key, groupName, position, false, null);
    }

    /// <inheritdoc/>
    public async Task StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null)
    {
        _ = await Command(Request.StreamCreateConsumerGroupAsync(key, groupName, position ?? default, createStream, entriesRead));
    }

    /// <inheritdoc/>
    public async Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return await Command(Request.StreamDeleteConsumerGroupAsync(key, groupName));
    }

    /// <inheritdoc/>
    public async Task StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null)
    {
        _ = await Command(Request.StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead));
    }

    /// <inheritdoc/>
    public async Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
    {
        return await Command(Request.StreamCreateConsumerAsync(key, groupName, consumerName));
    }

    /// <inheritdoc/>
    public async Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
    {
        return await Command(Request.StreamDeleteConsumerAsync(key, groupName, consumerName));
    }

    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count)
    {
        return await StreamReadGroupAsync(key, groupName, consumerName, position, count, false);
    }

    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false)
    {
        return await Command(Request.StreamReadGroupAsync(key, groupName, consumerName, position ?? default, count, noAck));
    }

    /// <inheritdoc/>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream)
    {
        return await StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, false);
    }

    /// <inheritdoc/>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false)
    {
        return await Command(Request.StreamReadGroupAsync([.. streamPositions], groupName, consumerName, countPerStream, noAck));
    }

    /// <inheritdoc/>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId)
    {
        return await StreamAcknowledgeAsync(key, groupName, [messageId]);
    }

    /// <inheritdoc/>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds)
    {
        return await Command(Request.StreamAcknowledgeAsync(key, groupName, [.. messageIds]));
    }

    /// <inheritdoc/>
    public async Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return await Command(Request.StreamPendingAsync(key, groupName));
    }

    /// <inheritdoc/>
    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId)
    {
        return await StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, null);
    }

    /// <inheritdoc/>
    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, long? minIdleTimeInMs = null)
    {
        return await Command(Request.StreamPendingMessagesAsync(key, groupName, minId ?? StreamConstants.ReadMinValue, maxId ?? StreamConstants.ReadMaxValue, count, consumerName, minIdleTimeInMs));
    }
    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds)
    {
        return await StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, null, null, null, false);
    }

    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false)
    {
        return await Command(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, [.. messageIds], idleTimeInMs, timeUnixMs, retryCount, force));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds)
    {
        return await StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, null, null, null, false);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false)
    {
        return await Command(Request.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, [.. messageIds], idleTimeInMs, timeUnixMs, retryCount, force));
    }
    /// <inheritdoc/>
    public async Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null)
    {
        return await Command(Request.StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count));
    }

    /// <inheritdoc/>
    public async Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null)
    {
        return await Command(Request.StreamAutoClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count));
    }
    /// <inheritdoc/>
    public async Task<long> StreamLengthAsync(ValkeyKey key)
    {
        return await Command(Request.StreamLengthAsync(key));
    }

    /// <inheritdoc/>
    public async Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)
    {
        return await Command(Request.StreamDeleteAsync(key, [.. messageIds]));
    }

    /// <inheritdoc/>
    public async Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength)
    {
        return await StreamTrimAsync(key, maxLength, useApproximateMaxLength, null);
    }

    /// <inheritdoc/>
    public async Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null)
    {
        return await Command(Request.StreamTrimAsync(key, maxLength, default, useApproximateMaxLength, limit));
    }

    /// <inheritdoc/>
    public async Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null)
    {
        return await Command(Request.StreamTrimAsync(key, null, minId, useApproximateMaxLength, limit));
    }

    /// <inheritdoc/>
    public async Task<StreamInfo> StreamInfoAsync(ValkeyKey key)
    {
        return await Command(Request.StreamInfoAsync(key));
    }

    /// <inheritdoc/>
    public async Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key)
    {
        return await Command(Request.StreamGroupInfoAsync(key));
    }

    /// <inheritdoc/>
    public async Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return await Command(Request.StreamConsumerInfoAsync(key, groupName));
    }
}
