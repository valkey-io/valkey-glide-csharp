// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IStreamBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IStreamBaseCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)"/>
    /// <param name="trimMode">Determines how stream trimming should be performed (currently only <see cref="StreamTrimMode.KeepReferences"/> is supported).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool)"/>
    /// <param name="trimMode">Determines how stream trimming should be performed (currently only <see cref="StreamTrimMode.KeepReferences"/> is supported).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from a stream for a consumer group, with optional auto-claiming of idle messages.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="position">The position from which to read.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="noAck">If true, messages are not added to the pending entries list.</param>
    /// <param name="claimMinIdleTime">Auto-claim messages that have been idle for at least this long.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, TimeSpan? claimMinIdleTime, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue})" path="//*[not(self::param[@name='minIdleTime'])]"/>
    /// <param name="minIdleTimeInMs">The minimum message idle time in milliseconds.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue})" path="//*[not(self::param[@name='minIdleTime'])]"/>
    /// <param name="minIdleTimeInMs">The minimum message idle time in milliseconds.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" path="//*[not(self::param[@name='minIdleTime'])]"/>
    /// <param name="minIdleTimeInMs">The minimum message idle time in milliseconds.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, ValkeyValue, int?)" path="//*[not(self::param[@name='minIdleTime'])]"/>
    /// <param name="minIdleTimeInMs">The minimum message idle time in milliseconds.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamAutoClaimJustIdResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamTrimAsync(ValkeyKey, int, bool)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamTrimAsync(ValkeyKey, long?, bool, long?)"/>
    /// <param name="trimMode">Determines how stream trimming should be performed (currently only <see cref="StreamTrimMode.KeepReferences"/> is supported).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamTrimByMinIdAsync(ValkeyKey, ValkeyValue, bool, long?)"/>
    /// <param name="trimMode">Determines how stream trimming should be performed (currently only <see cref="StreamTrimMode.KeepReferences"/> is supported).</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamInfoAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamGroupInfoAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStreamBaseCommands.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);
}
