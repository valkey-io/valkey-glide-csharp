// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Stream commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#stream">Valkey – Stream Commands</seealso>
public interface IStreamBaseCommands
{
    /// <summary>
    /// Creates a consumer group for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-create"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The position from which to start reading. Use <see cref="StreamConstants.NewMessages"/>  for new messages, <see cref="StreamConstants.AllMessages"/>  for all messages. Defaults to <see cref="StreamConstants.NewMessages"/>  if null.</param>
    /// <returns><c>true</c> if the consumer group was created successfully.</returns>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position);

    /// <summary>
    /// Destroys a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-destroy"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <returns>True if the group was destroyed, false otherwise.</returns>
    Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName);

    /// <summary>
    /// Creates a consumer in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-createconsumer"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name to create.</param>
    /// <returns>True if the consumer was created, false if it already existed.</returns>
    Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    /// <summary>
    /// Deletes a consumer from a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-delconsumer"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <returns>The number of pending messages the consumer had before deletion.</returns>
    Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    /// <summary>
    /// Reads entries from a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="position">The position from which to read. Use <see cref="StreamConstants.UndeliveredMessages"/>  for new messages only. Defaults to <see cref="StreamConstants.UndeliveredMessages"/>  if null.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <returns>An array of stream entries.</returns>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count);

    /// <summary>
    /// Reads entries from multiple streams for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="streamPositions">A collection of stream keys and their starting positions.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="countPerStream">The maximum number of entries to return per stream.</param>
    /// <returns>An array of streams with their entries.</returns>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream);

    /// <summary>
    /// Acknowledges a message in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xack"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="messageId">The message ID to acknowledge.</param>
    /// <returns>The number of messages acknowledged.</returns>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId);

    /// <summary>
    /// Acknowledges messages in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xack"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="messageIds">A collection of message IDs to acknowledge.</param>
    /// <returns>The number of messages acknowledged.</returns>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds);

    /// <summary>
    /// Returns pending messages summary for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <returns>Summary information about pending messages.</returns>
    Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName);

    /// <summary>
    /// Returns detailed pending messages for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="consumerName">Filter by consumer name. Pass <see cref="ValkeyValue.Null"/> or default to include all consumers.</param>
    /// <param name="minId">The minimum ID (inclusive). Defaults to <see cref="StreamConstants.ReadMinValue"/>  (smallest ID) if null.</param>
    /// <param name="maxId">The maximum ID (inclusive). Defaults to <see cref="StreamConstants.ReadMaxValue"/>  (largest ID) if null.</param>
    /// <returns>An array of detailed information about each pending message.</returns>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId);

    /// <summary>
    /// Returns detailed pending messages for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="consumerName">Filter by consumer name. Pass <see cref="ValkeyValue.Null"/> or default to include all consumers.</param>
    /// <param name="minId">The minimum ID (inclusive). Defaults to <see cref="StreamConstants.ReadMinValue"/>  (smallest ID) if null.</param>
    /// <param name="maxId">The maximum ID (inclusive). Defaults to <see cref="StreamConstants.ReadMaxValue"/>  (largest ID) if null.</param>
    /// <param name="minIdleTime">The minimum idle time for pending messages</param>
    /// <returns>An array of detailed information about each pending message.</returns>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName = default, ValkeyValue? minId = null, ValkeyValue? maxId = null, TimeSpan? minIdleTime = null);

    /// <summary>
    /// Claims pending messages for a consumer.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTime">The minimum idle time for the message to be claimed</param>
    /// <param name="messageIds">A collection of message IDs to claim.</param>
    /// <returns>An array of claimed stream entries.</returns>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds);

    /// <summary>
    /// Automatically claims pending messages.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xautoclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTime">The minimum idle time for the message to be claimed</param>
    /// <param name="startAtId">The starting ID to scan for pending messages.</param>
    /// <param name="count">The maximum number of entries to claim. Defaults to 100 if null.</param>
    /// <returns>Result containing next start ID, claimed entries, and deleted IDs.</returns>
    Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);

    /// <summary>
    /// Returns the number of entries in a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xlen"/>
    /// <param name="key">The key of the stream.</param>
    /// <returns>The number of entries in the stream.</returns>
    Task<long> StreamLengthAsync(ValkeyKey key);

    /// <summary>
    /// Removes one or more messages from a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xdel"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="messageIds">A collection of message IDs to delete.</param>
    /// <returns>The number of messages deleted.</returns>
    Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds);

    /// <summary>
    /// Trims the stream to a specified size.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="maxLength">The maximum length of the stream.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <returns>The number of entries deleted.</returns>
    Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength);

    /// <summary>
    /// Returns stream information.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-stream"/>
    /// <param name="key">The key of the stream.</param>
    /// <returns>Information about the stream.</returns>
    Task<StreamInfo> StreamInfoAsync(ValkeyKey key);

    /// <summary>
    /// Returns consumer group information for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-groups"/>
    /// <param name="key">The key of the stream.</param>
    /// <returns>An array of information about each consumer group.</returns>
    Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key);

    /// <summary>
    /// Returns consumer information for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-consumers"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <returns>An array of information about each consumer in the group.</returns>
    Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName);
}
