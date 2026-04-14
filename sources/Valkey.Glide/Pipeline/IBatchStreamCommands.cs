// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Stream Commands" group for batch requests.
/// </summary>
internal interface IBatchStreamCommands
{
    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" /></returns>
    IBatch StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, Commands.Options.StreamAddOptions? options = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool)" /></returns>
    IBatch StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, Commands.Options.StreamAddOptions? options = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)" /></returns>
    IBatch StreamRead(ValkeyKey key, ValkeyValue position, int? count = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)" /></returns>
    IBatch StreamRead(IEnumerable<StreamPosition> streamPositions, int? count = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)" /></returns>
    IBatch StreamRange(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamLengthAsync(ValkeyKey)" /></returns>
    IBatch StreamLength(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?)" /></returns>
    IBatch StreamTrim(ValkeyKey key, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)" /></returns>
    IBatch StreamCreateConsumerGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamDeleteConsumerGroup(ValkeyKey key, ValkeyValue groupName);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?)" /></returns>
    IBatch StreamConsumerGroupSetPosition(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)" /></returns>
    IBatch StreamReadGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)" /></returns>
    IBatch StreamReadGroup(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamPending(ValkeyKey key, ValkeyValue groupName);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, TimeSpan?)" /></returns>
    IBatch StreamPendingMessages(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName = default, ValkeyValue? minId = null, ValkeyValue? maxId = null, TimeSpan? minIdleTime = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, Commands.Options.StreamClaimOptions? options = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, Commands.Options.StreamClaimOptions? options = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" /></returns>
    IBatch StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamAutoClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamAutoClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" /></returns>
    IBatch StreamAutoClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamInfoAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamInfoAsync(ValkeyKey)" /></returns>
    IBatch StreamInfo(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamGroupInfoAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamGroupInfoAsync(ValkeyKey)" /></returns>
    IBatch StreamGroupInfo(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamConsumerInfo(ValkeyKey key, ValkeyValue groupName);

    /// <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey, int?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey, int?)" /></returns>
    IBatch StreamInfoFull(ValkeyKey key, int? count = null);
}
