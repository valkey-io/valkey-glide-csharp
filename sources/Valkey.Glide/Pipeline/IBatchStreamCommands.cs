// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Stream Commands" group for batch requests.
/// </summary>
internal interface IBatchStreamCommands
{
    /// <inheritdoc cref="Commands.IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool)" /></returns>
    IBatch StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, bool noMakeStream = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, long?, bool, long?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, long?, bool, long?, bool)" /></returns>
    IBatch StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, bool noMakeStream = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)" /></returns>
    IBatch StreamRead(ValkeyKey key, ValkeyValue position, int? count = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)" /></returns>
    IBatch StreamRead(IEnumerable<StreamPosition> streamPositions, int? count = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)" /></returns>
    IBatch StreamRange(ValkeyKey key, ValkeyValue? start = null, ValkeyValue? end = null, int? count = null, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamLengthAsync(ValkeyKey)" /></returns>
    IBatch StreamLength(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?)" /></returns>
    IBatch StreamTrim(ValkeyKey key, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)" /></returns>
    IBatch StreamCreateConsumerGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamDeleteConsumerGroup(ValkeyKey key, ValkeyValue groupName);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?)" /></returns>
    IBatch StreamConsumerGroupSetPosition(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" /></returns>
    IBatch StreamReadGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)" /></returns>
    IBatch StreamReadGroup(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamPending(ValkeyKey key, ValkeyValue groupName);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, TimeSpan?)" /></returns>
    IBatch StreamPendingMessages(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, TimeSpan? minIdleTime = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue}, TimeSpan?, DateTimeOffset?, int?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue}, TimeSpan?, DateTimeOffset?, int?, bool)" /></returns>
    IBatch StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, TimeSpan? idleTime = null, DateTimeOffset? timestamp = null, int? retryCount = null, bool force = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue}, TimeSpan?, DateTimeOffset?, int?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue}, TimeSpan?, DateTimeOffset?, int?, bool)" /></returns>
    IBatch StreamClaimIdsOnly(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, TimeSpan? idleTime = null, DateTimeOffset? timestamp = null, int? retryCount = null, bool force = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" /></returns>
    IBatch StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" /></returns>
    IBatch StreamAutoClaimIdsOnly(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamInfoAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamInfoAsync(ValkeyKey)" /></returns>
    IBatch StreamInfo(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamGroupInfoAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamGroupInfoAsync(ValkeyKey)" /></returns>
    IBatch StreamGroupInfo(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamConsumerInfo(ValkeyKey key, ValkeyValue groupName);
}
