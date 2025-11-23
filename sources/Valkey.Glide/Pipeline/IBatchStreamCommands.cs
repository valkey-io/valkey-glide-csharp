// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Stream Commands" group for batch requests.
/// </summary>
internal interface IBatchStreamCommands
{
    /// <inheritdoc cref="Commands.IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool, ValkeyValue?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool, ValkeyValue?, CommandFlags)" /></returns>
    IBatch StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, bool noMakeStream = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamAddAsync(ValkeyKey, NameValueEntry[], ValkeyValue?, long?, bool, long?, bool, ValkeyValue?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAddAsync(ValkeyKey, NameValueEntry[], ValkeyValue?, long?, bool, long?, bool, ValkeyValue?, CommandFlags)" /></returns>
    IBatch StreamAdd(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, bool noMakeStream = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?, int?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?, int?, CommandFlags)" /></returns>
    IBatch StreamRead(ValkeyKey key, ValkeyValue position, int? count = null, int? block = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamReadAsync(StreamPosition[], int?, int?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamReadAsync(StreamPosition[], int?, int?, CommandFlags)" /></returns>
    IBatch StreamRead(StreamPosition[] streamPositions, int? count = null, int? block = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order, CommandFlags)" /></returns>
    IBatch StreamRange(ValkeyKey key, ValkeyValue? start = null, ValkeyValue? end = null, int? count = null, Order order = Order.Ascending);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamLengthAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamLengthAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch StreamLength(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamDeleteAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamDeleteAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch StreamDelete(ValkeyKey key, ValkeyValue[] messageIds);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?, ValkeyValue?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?, ValkeyValue?, CommandFlags)" /></returns>
    IBatch StreamTrim(ValkeyKey key, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?, CommandFlags)" /></returns>
    IBatch StreamCreateConsumerGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch StreamDeleteConsumerGroup(ValkeyKey key, ValkeyValue groupName);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)" /></returns>
    IBatch StreamDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?, CommandFlags)" /></returns>
    IBatch StreamConsumerGroupSetPosition(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)" /></returns>
    IBatch StreamReadGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamReadGroupAsync(StreamPosition[], ValkeyValue, ValkeyValue, int?, bool, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamReadGroupAsync(StreamPosition[], ValkeyValue, ValkeyValue, int?, bool, CommandFlags)" /></returns>
    IBatch StreamReadGroup(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue[], CommandFlags)" /></returns>
    IBatch StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, ValkeyValue[] messageIds);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamPendingAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamPendingAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch StreamPending(ValkeyKey key, ValkeyValue groupName);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, long?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, long?, CommandFlags)" /></returns>
    IBatch StreamPendingMessages(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, long? minIdleTimeInMs = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue[], long?, long?, int?, bool, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue[], long?, long?, int?, bool, CommandFlags)" /></returns>
    IBatch StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue[], long?, long?, int?, bool, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue[], long?, long?, int?, bool, CommandFlags)" /></returns>
    IBatch StreamClaimIdsOnly(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)" /></returns>
    IBatch StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)" /></returns>
    IBatch StreamAutoClaimIdsOnly(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamInfoAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamInfoAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch StreamInfo(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamInfoFullAsync(ValkeyKey, int?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamInfoFullAsync(ValkeyKey, int?, CommandFlags)" /></returns>
    IBatch StreamInfoFull(ValkeyKey key, int? count = null);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamGroupInfoAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamGroupInfoAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch StreamGroupInfo(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStreamCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch StreamConsumerInfo(ValkeyKey key, ValkeyValue groupName);
}
