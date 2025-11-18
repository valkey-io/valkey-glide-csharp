// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class BaseClient : IStreamCommands
{
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            minId ?? default,
            useApproximateTrimming,
            [new NameValueEntry(streamField, streamValue)],
            limit,
            mode,
            noMakeStream));
    }

    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            minId ?? default,
            useApproximateTrimming,
            streamPairs,
            limit,
            mode,
            noMakeStream));
    }

    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null, int? block = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadAsync(key, position, count, block));
    }

    public async Task<ValkeyStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? count = null, int? block = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadAsync(streamPositions, count, block));
    }

    public async Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start = null, ValkeyValue? end = null, int? count = null, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamRangeAsync(key, start ?? "-", end ?? "+", count, order));
    }

    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamCreateConsumerGroupAsync(key, groupName, position ?? default, createStream));
    }

    public async Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamDeleteConsumerGroupAsync(key, groupName));
    }

    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamConsumerGroupSetPositionAsync(key, groupName, position));
    }

    public async Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamDeleteConsumerAsync(key, groupName, consumerName));
    }
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadGroupAsync(key, groupName, consumerName, position ?? default, count, noAck));
    }

    public async Task<ValkeyStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck));
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

    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, long? minIdleTimeInMs = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamPendingMessagesAsync(key, groupName, minId ?? "-", maxId ?? "+", count, consumerName, minIdleTimeInMs));
    }
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force));
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

    public async Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamTrimAsync(key, maxLength, minId ?? default, useApproximateTrimming, limit));
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
}
