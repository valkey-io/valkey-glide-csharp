// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool, CommandFlags)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, StreamTrimMode, CommandFlags)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GuardClauses.ThrowIfUnsupportedTrimMode(trimMode);
        var options = new Commands.Options.StreamAddOptions { Id = messageId };
        if (maxLength.HasValue)
            options = options with { Trim = new Commands.Options.StreamTrimOptions.MaxLen { MaxLength = maxLength.Value, Exact = !useApproximateMaxLength, Limit = limit } };
        return StreamAddAsync(key, streamField, streamValue, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, long?, bool, long?, StreamTrimMode, CommandFlags)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GuardClauses.ThrowIfUnsupportedTrimMode(trimMode);
        var options = new Commands.Options.StreamAddOptions { Id = messageId };
        if (maxLength.HasValue)
            options = options with { Trim = new Commands.Options.StreamTrimOptions.MaxLen { MaxLength = maxLength.Value, Exact = !useApproximateMaxLength, Limit = limit } };
        return StreamAddAsync(key, streamPairs, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadAsync(ValkeyKey, ValkeyValue, int?, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadAsync(key, position, countPerStream);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadAsync(IEnumerable{StreamPosition}, int?, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadAsync(streamPositions, countPerStream);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order, CommandFlags)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamRangeAsync(key, minId, maxId, count, messageOrder);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position, createStream, entriesRead);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamDeleteConsumerGroupAsync(key, groupName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamCreateConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamDeleteConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?, CommandFlags)"/>
    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadGroupAsync(key, groupName, consumerName, position, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadGroupAsync(key, groupName, consumerName, position, count, noAck);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAcknowledgeAsync(key, groupName, messageId);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAcknowledgeAsync(key, groupName, messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamPendingAsync(key, groupName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, CommandFlags)"/>
    public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamClaimAsync(key, consumerGroup, claimingConsumer, TimeSpan.FromMilliseconds(minIdleTimeInMs), messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamClaimJustIdAsync(key, consumerGroup, claimingConsumer, TimeSpan.FromMilliseconds(minIdleTimeInMs), messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)"/>
    public Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, TimeSpan.FromMilliseconds(minIdleTimeInMs), startAtId, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)"/>
    public Task<StreamAutoClaimJustIdResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAutoClaimJustIdAsync(key, consumerGroup, claimingConsumer, TimeSpan.FromMilliseconds(minIdleTimeInMs), startAtId, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamDeleteAsync(key, messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimAsync(ValkeyKey, int, bool, CommandFlags)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamTrimAsync(key, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimAsync(ValkeyKey, long?, bool, long?, StreamTrimMode, CommandFlags)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GuardClauses.ThrowIfUnsupportedTrimMode(trimMode);
        return StreamTrimAsync(key, maxLength, useApproximateMaxLength, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimByMinIdAsync(ValkeyKey, ValkeyValue, bool, long?, StreamTrimMode, CommandFlags)"/>
    public Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GuardClauses.ThrowIfUnsupportedTrimMode(trimMode);
        return StreamTrimByMinIdAsync(key, minId, useApproximateMaxLength, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamInfoAsync(ValkeyKey, CommandFlags)"/>
    public Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamInfoAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamGroupInfoAsync(ValkeyKey, CommandFlags)"/>
    public Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamGroupInfoAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamConsumerInfoAsync(key, groupName);
    }
}
