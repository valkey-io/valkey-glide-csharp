// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IStreamBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    #region StreamAddAsync

    /// <summary>
    /// Appends a new entry to a stream with a single field-value pair.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Appends a new entry to a stream with multiple field-value pairs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamReadAsync

    /// <summary>
    /// Reads entries from a single stream starting from a given position.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread"/>
    Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from multiple streams starting from given positions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread"/>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamRangeAsync

    /// <summary>
    /// Returns the entries in a stream within a specified range of IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xrange"/>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamReadGroupAsync

    /// <summary>
    /// Reads entries from a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from a stream for a consumer group with claimMinIdleTime support (not yet implemented).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, TimeSpan? claimMinIdleTime, CommandFlags flags);

    /// <summary>
    /// Reads entries from multiple streams for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamLengthAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags);

    #endregion
    #region StreamDeleteAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    #endregion
    #region StreamCreateConsumerGroupAsync

    /// <summary>
    /// Creates a consumer group for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-create"/>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Creates a consumer group for a stream with entriesRead support (Valkey 7.0+).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-create"/>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags);

    #endregion
    #region StreamDeleteConsumerGroupAsync

    /// <summary>
    /// Destroys a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-destroy"/>
    Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamCreateConsumerAsync

    /// <summary>
    /// Creates a consumer in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-createconsumer"/>
    Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamDeleteConsumerAsync

    /// <summary>
    /// Deletes a consumer from a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-delconsumer"/>
    Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamConsumerGroupSetPositionAsync

    /// <summary>
    /// Sets the position from which to read a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-setid"/>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets the position from which to read a stream for a consumer group with entriesRead support (Valkey 7.0+).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-setid"/>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead, CommandFlags flags);

    #endregion
    #region StreamAcknowledgeAsync

    /// <summary>
    /// Acknowledges a message in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xack"/>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Acknowledges messages in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xack"/>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamPendingAsync

    /// <summary>
    /// Returns pending messages summary for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending"/>
    Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamPendingMessagesAsync

    /// <summary>
    /// Returns detailed pending messages for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending"/>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamClaimAsync

    /// <summary>
    /// Claims pending messages for a consumer.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamClaimIdsOnlyAsync

    /// <summary>
    /// Claims pending messages for a consumer, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamAutoClaimAsync

    /// <summary>
    /// Automatically claims pending messages.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xautoclaim"/>
    Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamAutoClaimIdsOnlyAsync

    /// <summary>
    /// Automatically claims pending messages, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xautoclaim"/>
    Task<StreamAutoClaimJustIdResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamTrimAsync

    /// <summary>
    /// Trims the stream to a specified size.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Trims the stream to a specified size with extended options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamTrimByMinIdAsync

    /// <summary>
    /// Trims the stream by minimum ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamInfoAsync

    /// <summary>
    /// Returns stream information.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-stream"/>
    Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamGroupInfoAsync

    /// <summary>
    /// Returns consumer group information for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-groups"/>
    Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamConsumerInfoAsync

    /// <summary>
    /// Returns consumer information for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-consumers"/>
    Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None);

    #endregion
}
