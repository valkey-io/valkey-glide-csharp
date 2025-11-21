// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports stream commands for standalone and cluster clients.
/// <br/>
/// See more on <see href="https://valkey.io/commands#stream">valkey.io</see>.
/// </summary>
public interface IStreamCommands
{
    /// <summary>
    /// Appends a new entry to a stream with a single field-value pair.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamField">The field name.</param>
    /// <param name="streamValue">The field value.</param>
    /// <param name="messageId">The message ID. Use null or "*" for auto-generation.</param>
    /// <param name="maxLength">The maximum length of the stream. If specified, old entries will be trimmed.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The ID of the added entry.</returns>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <summary>
    /// Appends a new entry to a stream with multiple field-value pairs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <param name="messageId">The message ID. Use null or "*" for auto-generation.</param>
    /// <param name="maxLength">The maximum length of the stream. If specified, old entries will be trimmed.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The ID of the added entry.</returns>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <summary>
    /// Appends a new entry to a stream with a single field-value pair.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamField">The field name.</param>
    /// <param name="streamValue">The field value.</param>
    /// <param name="messageId">The message ID. Use null or "*" for auto-generation, or "&lt;ms&gt;-*" to auto-generate sequence number for a specific timestamp.</param>
    /// <param name="maxLength">The maximum length of the stream. If specified, old entries will be trimmed. Mutually exclusive with minId.</param>
    /// <param name="useApproximateTrimming">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim. Only applicable when useApproximateTrimming is true.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="noMakeStream">If true, the stream will not be created if it doesn't exist. Returns null if stream doesn't exist.</param>
    /// <param name="minId">Trim entries with IDs lower than this. Mutually exclusive with maxLength.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The ID of the added entry, or null if noMakeStream is true and the stream doesn't exist.</returns>
    /// <exception cref="RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false, ValkeyValue? minId = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Appends a new entry to a stream with multiple field-value pairs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <param name="messageId">The message ID. Use null or "*" for auto-generation, or "&lt;ms&gt;-*" to auto-generate sequence number for a specific timestamp.</param>
    /// <param name="maxLength">The maximum length of the stream. If specified, old entries will be trimmed. Mutually exclusive with minId.</param>
    /// <param name="useApproximateTrimming">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim. Only applicable when useApproximateTrimming is true.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="noMakeStream">If true, the stream will not be created if it doesn't exist. Returns null if stream doesn't exist.</param>
    /// <param name="minId">Trim entries with IDs lower than this. Mutually exclusive with maxLength.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <param name="returns">The ID of the added entry, or null if noMakeStream is true and the stream doesn't exist.</param>
    /// <exception cref="RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false, ValkeyValue? minId = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from a single stream starting from a given position.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="position">The position from which to start reading. Use "0-0" to read from the beginning.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="block">The maximum time to block waiting for entries (in milliseconds). Use null for non-blocking.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of stream entries, or an empty array if no entries are available.</returns>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null, int? block = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from multiple streams starting from given positions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread"/>
    /// <param name="streamPositions">Array of stream keys and their starting positions.</param>
    /// <param name="count">The maximum number of entries to return per stream.</param>
    /// <param name="block">The maximum time to block waiting for entries (in milliseconds). Use null for non-blocking.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of streams with their entries, or an empty array if no entries are available.</returns>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<ValkeyStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? count = null, int? block = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the entries in a stream within a specified range of IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xrange"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="start">The minimum ID (inclusive). Use "-" for the smallest ID in the stream.</param>
    /// <param name="end">The maximum ID (inclusive). Use "+" for the largest ID in the stream.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="order">The order to return entries (Ascending uses XRANGE, Descending uses XREVRANGE).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of stream entries in the specified range.</returns>
    /// <exception cref="Errors.RequestException">Thrown if the command fails to execute on the server.</exception>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start = null, ValkeyValue? end = null, int? count = null, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Creates a consumer group for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-create"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The position from which to start reading. Use "$" for new messages, "0" for all messages.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>True if the group was created, false otherwise.</returns>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags);

    /// <summary>
    /// Creates a consumer group for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-create"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The position from which to start reading. Use "$" for new messages, "0" for all messages.</param>
    /// <param name="createStream">If true, creates the stream if it doesn't exist.</param>
    /// <param name="entriesRead">Valkey 7.0+: Sets the entries_read counter to an arbitrary value.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>True if the group was created, false otherwise.</returns>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Destroys a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-destroy"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>True if the group was destroyed, false otherwise.</returns>
    Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Deletes a consumer from a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-delconsumer"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of pending messages the consumer had before deletion.</returns>
    Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets the position from which to read a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-setid"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The position from which to read.</param>
    /// <param name="entriesRead">Valkey 7.0+: Sets the entries_read counter to an arbitrary value.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="position">The position from which to read. Use ">" for new messages only.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of stream entries.</returns>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags);

    /// <summary>
    /// Reads entries from a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="position">The position from which to read. Use ">" for new messages only.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="noAck">If true, messages are not added to the pending entries list.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of stream entries.</returns>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Reads entries from multiple streams for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="streamPositions">Array of stream keys and their starting positions.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="countPerStream">The maximum number of entries to return per stream.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of streams with their entries.</returns>
    Task<ValkeyStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags);

    /// <summary>
    /// Reads entries from multiple streams for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="streamPositions">Array of stream keys and their starting positions.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="countPerStream">The maximum number of entries to return per stream.</param>
    /// <param name="noAck">If true, messages are not added to the pending entries list.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of streams with their entries.</returns>
    Task<ValkeyStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Acknowledges a message in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xack"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="messageId">The message ID to acknowledge.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of messages acknowledged.</returns>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Acknowledges messages in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xack"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="messageIds">Array of message IDs to acknowledge.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of messages acknowledged.</returns>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue[] messageIds, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Acknowledges and optionally deletes a message from a stream.
    /// </summary>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="messageId">The message ID to acknowledge.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The result of the operation.</returns>
    /// <exception cref="NotSupportedException">This method is not yet supported.</exception>
    Task<StreamTrimResult> StreamAcknowledgeAndDeleteAsync(ValkeyKey key, ValkeyValue groupName, StreamTrimMode mode, ValkeyValue messageId, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Acknowledges and optionally deletes messages from a stream.
    /// </summary>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="messageIds">Array of message IDs to acknowledge.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>Array of results for each message.</returns>
    /// <exception cref="NotSupportedException">This method is not yet supported.</exception>
    Task<StreamTrimResult[]> StreamAcknowledgeAndDeleteAsync(ValkeyKey key, ValkeyValue groupName, StreamTrimMode mode, ValkeyValue[] messageIds, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns pending messages summary for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>Summary information about pending messages.</returns>
    Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns detailed pending messages for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="consumerName">Filter by consumer name.</param>
    /// <param name="minId">The minimum ID (inclusive). Use "-" for the smallest ID.</param>
    /// <param name="maxId">The maximum ID (inclusive). Use "+" for the largest ID.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of detailed information about each pending message.</returns>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId, CommandFlags flags);

    /// <summary>
    /// Returns detailed pending messages for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="consumerName">Filter by consumer name.</param>
    /// <param name="minId">The minimum ID (inclusive). Use "-" for the smallest ID.</param>
    /// <param name="maxId">The maximum ID (inclusive). Use "+" for the largest ID.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of detailed information about each pending message.</returns>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, long? minIdleTimeInMs = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Claims pending messages for a consumer.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="messageIds">Array of message IDs to claim.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of claimed stream entries.</returns>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, CommandFlags flags);

    /// <summary>
    /// Claims pending messages for a consumer.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="messageIds">Array of message IDs to claim.</param>
    /// <param name="idleTimeInMs">Set the idle time (last delivery time) of the message.</param>
    /// <param name="timeUnixMs">Set the idle time to a specific Unix time in milliseconds.</param>
    /// <param name="retryCount">Set the retry counter to the specified value.</param>
    /// <param name="force">Create PEL entry even if message not already assigned to a consumer.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of claimed stream entries.</returns>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Claims pending messages for a consumer, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="messageIds">Array of message IDs to claim.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of claimed message IDs.</returns>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, CommandFlags flags);

    /// <summary>
    /// Claims pending messages for a consumer, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="messageIds">Array of message IDs to claim.</param>
    /// <param name="idleTimeInMs">Set the idle time (last delivery time) of the message.</param>
    /// <param name="timeUnixMs">Set the idle time to a specific Unix time in milliseconds.</param>
    /// <param name="retryCount">Set the retry counter to the specified value.</param>
    /// <param name="force">Create PEL entry even if message not already assigned to a consumer.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of claimed message IDs.</returns>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue[] messageIds, long? idleTimeInMs = null, long? timeUnixMs = null, int? retryCount = null, bool force = false, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Automatically claims pending messages.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xautoclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="startAtId">The starting ID to scan for pending messages.</param>
    /// <param name="count">The maximum number of entries to claim.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>Result containing next start ID, claimed entries, and deleted IDs.</returns>
    Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Automatically claims pending messages, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xautoclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="startAtId">The starting ID to scan for pending messages.</param>
    /// <param name="count">The maximum number of entries to claim.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>Result containing next start ID, claimed IDs, and deleted IDs.</returns>
    Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of entries in a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xlen"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of entries in the stream.</returns>
    Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes one or more messages from a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xdel"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="messageIds">Array of message IDs to delete.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of messages deleted.</returns>
    Task<long> StreamDeleteAsync(ValkeyKey key, ValkeyValue[] messageIds, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes one or more messages from a stream with trim mode.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xdel"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="messageIds">Array of message IDs to delete.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>Array of results for each message.</returns>
    /// <exception cref="NotSupportedException">This method is not yet supported.</exception>
    Task<StreamTrimResult[]> StreamDeleteAsync(ValkeyKey key, ValkeyValue[] messageIds, StreamTrimMode mode, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Trims the stream to a specified size.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="maxLength">The maximum length of the stream.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of entries deleted.</returns>
    Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength, CommandFlags flags);

    /// <summary>
    /// Trims the stream to a specified size.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="maxLength">The maximum length of the stream. Mutually exclusive with minId.</param>
    /// <param name="useApproximateTrimming">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim. Only applicable when useApproximateTrimming is true.</param>
    /// <param name="minId">Trim entries with IDs lower than this. Mutually exclusive with maxLength.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of entries deleted.</returns>
    Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateTrimming = false, long? limit = null, ValkeyValue? minId = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Trims the stream by minimum ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="minId">Trim entries with IDs lower than this.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim.</param>
    /// <param name="mode">The trimming mode.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of entries deleted.</returns>
    Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns stream information.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-stream"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>Information about the stream.</returns>
    Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns detailed stream information including entries, groups, and consumers.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-stream"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="count">Maximum number of entries to return. If null, returns all entries.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>Dictionary containing detailed stream information with keys like "length", "entries", "groups", etc.</returns>
    Task<Dictionary<string, object>> StreamInfoFullAsync(ValkeyKey key, int? count = null, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns consumer group information for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-groups"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of information about each consumer group.</returns>
    Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns consumer information for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-consumers"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of information about each consumer in the group.</returns>
    Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None);
}
