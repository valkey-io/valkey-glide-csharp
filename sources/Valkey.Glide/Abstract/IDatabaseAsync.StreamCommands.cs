// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// Stream commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IStreamCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, long?, bool, long?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start, ValkeyValue? end, int? count, Order order, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, long?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, long? minIdleTimeInMs, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, long?, long?, int?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, long?, long?, int?, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, long? idleTimeInMs, long? timeUnixMs, int? retryCount, bool force, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamLengthAsync(ValkeyKey)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamTrimAsync(ValkeyKey, int, bool)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength, bool useApproximateMaxLength, long? limit, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamTrimByMinIdAsync(ValkeyKey, ValkeyValue, bool, long?)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength, long? limit, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamInfoAsync(ValkeyKey)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamGroupInfoAsync(ValkeyKey)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">>The flags to use for this operation. Only <see cref="CommandFlags.None"/> is supported by Valkey GLIDE.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);
}
