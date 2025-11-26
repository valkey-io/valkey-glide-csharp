// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class BaseClient : IStreamCommands
{
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, null, false, flags);
    }

    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, null, false, flags);
    }

    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, bool noMakeStream = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
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

    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, bool noMakeStream = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            default,
            useApproximateMaxLength,
            streamPairs,
            limit,
            noMakeStream));
    }

    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadAsync(key, position, count));
    }

    public async Task<ValkeyStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadAsync(streamPositions, count));
    }

    public async Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start = null, ValkeyValue? end = null, int? count = null, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
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

    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position, false, null, flags);
    }

    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamCreateConsumerGroupAsync(key, groupName, position ?? default, createStream, entriesRead));
    }

    public async Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamDeleteConsumerGroupAsync(key, groupName));
    }

    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead));
    }

    public async Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamDeleteConsumerAsync(key, groupName, consumerName));
    }

    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(key, groupName, consumerName, position, count, false, flags);
    }

    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadGroupAsync(key, groupName, consumerName, position ?? default, count, noAck));
    }

    public async Task<ValkeyStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, false, flags);
    }

    public async Task<ValkeyStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck));
    }

    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAcknowledgeAsync(key, groupName, [messageId], flags);
    }

    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue[] messageIds, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAcknowledgeAsync(key, groupName, messageIds));
    }

    public async Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamPendingAsync(key, groupName));
    }

    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, null, flags);
    }

    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, long? minIdleTimeInMs = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamPendingMessagesAsync(key, groupName, minId ?? StreamConstants.ReadMinValue, maxId ?? StreamConstants.ReadMaxValue, count, consumerName, minIdleTimeInMs));
    }
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, null, null, null, false, flags);
    }

    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force));
    }

    public async Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, null, null, null, false, flags);
    }

    public async Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force));
    }
    public async Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count));
    }

    public async Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAutoClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count));
    }
    public async Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamLengthAsync(key));
    }

    public async Task<long> StreamDeleteAsync(ValkeyKey key, ValkeyValue[] messageIds, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamDeleteAsync(key, messageIds));
    }

    public async Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamTrimAsync(key, maxLength, useApproximateMaxLength, null, flags);
    }

    public async Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamTrimAsync(key, maxLength, default, useApproximateMaxLength, limit));
    }

    public async Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamTrimAsync(key, null, minId, useApproximateMaxLength, limit));
    }

    public async Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamInfoAsync(key));
    }

    public async Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamGroupInfoAsync(key));
    }

    public async Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamConsumerInfoAsync(key, groupName));
    }

    public Task<long> StreamAcknowledgeAndDeleteAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue[] messageIds, CommandFlags flags = CommandFlags.None)
    {
        throw new NotSupportedException("This method is not implemented. Use StreamAcknowledgeAsync followed by StreamDeleteAsync instead.");
    }

    public Task<long> StreamDeleteAsync(ValkeyKey key, ValkeyValue[] messageIds, object mode, CommandFlags flags = CommandFlags.None)
    {
        throw new NotSupportedException("This method is not implemented. Use StreamDeleteAsync without the mode parameter instead.");
    }

    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId, long? maxLength, bool useApproximateTrimming, long? limit, bool noMakeStream, ValkeyValue? minId, object mode, CommandFlags flags = CommandFlags.None)
    {
        throw new NotSupportedException("This method is not implemented. Use StreamAddAsync without the mode parameter instead.");
    }

    public Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength, bool useApproximateTrimming, long? limit, ValkeyValue? minId, object mode, CommandFlags flags = CommandFlags.None)
    {
        throw new NotSupportedException("This method is not implemented. Use StreamTrimAsync without the mode parameter instead.");
    }
}
