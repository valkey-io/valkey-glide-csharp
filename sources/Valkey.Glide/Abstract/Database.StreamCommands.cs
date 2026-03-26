// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)"/>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool, CommandFlags)"/>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool, CommandFlags)"/>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, limit, noMakeStream);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, long?, bool, long?, bool, CommandFlags)"/>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, limit, noMakeStream);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadAsync(ValkeyKey, ValkeyValue, int?, CommandFlags)"/>
    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadAsync(key, position, countPerStream);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadAsync(IEnumerable{StreamPosition}, int?, CommandFlags)"/>
    public async Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadAsync(streamPositions, countPerStream);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order, CommandFlags)"/>
    public async Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start, ValkeyValue? end, int? count, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamRangeAsync(key, start, end, count, order);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position, createStream, entriesRead);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamDeleteConsumerGroupAsync(key, groupName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamDeleteConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?, CommandFlags)"/>
    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, CommandFlags)"/>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(key, groupName, consumerName, position, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)"/>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(key, groupName, consumerName, position, count, noAck);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, CommandFlags)"/>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool, CommandFlags)"/>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAcknowledgeAsync(key, groupName, messageId);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAcknowledgeAsync(key, groupName, messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamPendingAsync(key, groupName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, CommandFlags)"/>
    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, long?, CommandFlags)"/>
    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, long? minIdleTimeInMs, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, minIdleTimeInMs);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, long?, long?, int?, bool, CommandFlags)"/>
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, long?, long?, int?, bool, CommandFlags)"/>
    public async Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)"/>
    public async Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)"/>
    public async Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAutoClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamLengthAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamDeleteAsync(key, messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimAsync(ValkeyKey, int, bool, CommandFlags)"/>
    public async Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamTrimAsync(key, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimAsync(ValkeyKey, long?, bool, long?, CommandFlags)"/>
    public async Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength, bool useApproximateMaxLength, long? limit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamTrimAsync(key, maxLength, useApproximateMaxLength, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimByMinIdAsync(ValkeyKey, ValkeyValue, bool, long?, CommandFlags)"/>
    public async Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength, long? limit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamTrimByMinIdAsync(key, minId, useApproximateMaxLength, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamInfoAsync(ValkeyKey, CommandFlags)"/>
    public async Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamInfoAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamGroupInfoAsync(ValkeyKey, CommandFlags)"/>
    public async Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamGroupInfoAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamConsumerInfoAsync(key, groupName);
    }
}
