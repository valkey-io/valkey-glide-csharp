// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>Stream commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.</summary>
/// <seealso cref="IStreamCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, limit, noMakeStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, long?, bool, long?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, limit, noMakeStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadAsync(key, position, countPerStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadAsync(streamPositions, countPerStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start, ValkeyValue? end, int? count, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamRangeAsync(key, start, end, count, order);
    }

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position);
    }

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position, createStream, entriesRead);
    }

    /// <inheritdoc cref="IStreamCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamDeleteConsumerGroupAsync(key, groupName);
    }

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IStreamCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamDeleteConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IStreamCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead);
    }

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(key, groupName, consumerName, position, count);
    }

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(key, groupName, consumerName, position, count, noAck);
    }

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAcknowledgeAsync(key, groupName, messageId);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAcknowledgeAsync(key, groupName, messageIds);
    }

    /// <inheritdoc cref="IStreamCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamPendingAsync(key, groupName);
    }

    /// <inheritdoc cref="IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId);
    }

    /// <inheritdoc cref="IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, long?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, long? minIdleTimeInMs, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, minIdleTimeInMs);
    }

    /// <inheritdoc cref="IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds);
    }

    /// <inheritdoc cref="IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, long?, long?, int?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force);
    }

    /// <inheritdoc cref="IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds);
    }

    /// <inheritdoc cref="IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, long?, long?, int?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, idleTimeInMs, timeUnixMs, retryCount, force);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAutoClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, startAtId, count);
    }

    /// <inheritdoc cref="IStreamCommands.StreamLengthAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamLengthAsync(key);
    }

    /// <inheritdoc cref="IStreamCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamDeleteAsync(key, messageIds);
    }

    /// <inheritdoc cref="IStreamCommands.StreamTrimAsync(ValkeyKey, int, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamTrimAsync(key, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IStreamCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength, bool useApproximateMaxLength, long? limit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamTrimAsync(key, maxLength, useApproximateMaxLength, limit);
    }

    /// <inheritdoc cref="IStreamCommands.StreamTrimByMinIdAsync(ValkeyKey, ValkeyValue, bool, long?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength, long? limit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamTrimByMinIdAsync(key, minId, useApproximateMaxLength, limit);
    }

    /// <inheritdoc cref="IStreamCommands.StreamInfoAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamInfoAsync(key);
    }

    /// <inheritdoc cref="IStreamCommands.StreamGroupInfoAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamGroupInfoAsync(key);
    }

    /// <inheritdoc cref="IStreamCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamConsumerInfoAsync(key, groupName);
    }
}
