// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

public partial interface IBaseClient
{
    /// <summary>
    /// Creates a consumer group for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-create"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The position from which to start reading. Use <see cref="StreamConstants.NewMessages"/> for new messages, <see cref="StreamConstants.AllMessages"/> for all messages. Defaults to <see cref="StreamConstants.NewMessages"/> if null.</param>
    /// <param name="createStream">If true, creates the stream if it doesn't exist.</param>
    /// <param name="entriesRead">Valkey 7.0+: Sets the entries_read counter to an arbitrary value.</param>
    /// <returns><c>true</c> if the consumer group was created successfully.</returns>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null);

    /// <summary>
    /// Sets the position from which to read a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-setid"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The position from which to read.</param>
    /// <param name="entriesRead">Valkey 7.0+: Sets the entries_read counter to an arbitrary value.</param>
    /// <returns><c>true</c> if the position was set successfully.</returns>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null);

    /// <summary>
    /// Reads entries from a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="position">The position from which to read. Use <see cref="StreamConstants.UndeliveredMessages"/> for new messages only. Defaults to <see cref="StreamConstants.UndeliveredMessages"/> if null.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="noAck">If true, messages are not added to the pending entries list.</param>
    /// <returns>An array of stream entries.</returns>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false);

    /// <summary>
    /// Reads entries from multiple streams for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="streamPositions">A collection of stream keys and their starting positions.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="countPerStream">The maximum number of entries to return per stream.</param>
    /// <param name="noAck">If true, messages are not added to the pending entries list.</param>
    /// <returns>An array of streams with their entries.</returns>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false);

    /// <summary>
    /// Trims the stream to a specified size.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="maxLength">The maximum length of the stream.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim. Only applicable when useApproximateMaxLength is true.</param>
    /// <returns>The number of entries deleted.</returns>
    Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null);

    /// <summary>
    /// Trims the stream by minimum ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="minId">Trim entries with IDs lower than this.</param>
    /// <param name="useApproximateMaxLength">If true, uses approximate trimming (~) for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim.</param>
    /// <returns>The number of entries deleted.</returns>
    Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null);

    /// <summary>
    /// Appends a new entry to a stream with a single field-value pair.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamField">The field name.</param>
    /// <param name="streamValue">The field value.</param>
    /// <param name="options">Optional arguments for the XADD command.</param>
    /// <returns>The ID of the added entry, or null if <see cref="StreamAddOptions.MakeStream"/> is <c>false</c> and the stream doesn't exist.</returns>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions? options = null);

    /// <summary>
    /// Appends a new entry to a stream with multiple field-value pairs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <param name="options">Optional arguments for the XADD command.</param>
    /// <returns>The ID of the added entry, or null if <see cref="StreamAddOptions.MakeStream"/> is <c>false</c> and the stream doesn't exist.</returns>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions? options = null);

    /// <summary>
    /// Claims pending messages for a consumer with additional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTime">The minimum idle time for the message to be claimed.</param>
    /// <param name="messageIds">A collection of message IDs to claim.</param>
    /// <param name="options">Optional arguments for the XCLAIM command.</param>
    /// <returns>An array of claimed stream entries.</returns>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options);

    /// <summary>
    /// Claims pending messages for a consumer with additional options, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTime">The minimum idle time for the message to be claimed.</param>
    /// <param name="messageIds">A collection of message IDs to claim.</param>
    /// <param name="options">Optional arguments for the XCLAIM command.</param>
    /// <returns>An array of claimed message IDs.</returns>
    Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options);

    /// <summary>
    /// Claims pending messages for a consumer, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTime">The minimum idle time for the message to be claimed.</param>
    /// <param name="messageIds">A collection of message IDs to claim.</param>
    /// <returns>An array of claimed message IDs.</returns>
    Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds);

    /// <summary>
    /// Automatically claims pending messages, returning only IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xautoclaim"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer claiming the messages.</param>
    /// <param name="minIdleTime">The minimum idle time for the message to be claimed.</param>
    /// <param name="startAtId">The starting ID to scan for pending messages.</param>
    /// <param name="count">The maximum number of entries to claim. Defaults to 100 if null.</param>
    /// <returns>Result containing next start ID, claimed IDs, and deleted IDs.</returns>
    Task<StreamAutoClaimJustIdResult> StreamAutoClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);

    /// <summary>
    /// Reads entries from a single stream with options including BLOCK support.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="position">The position from which to start reading.</param>
    /// <param name="options">Optional arguments including count and block timeout.</param>
    /// <returns>An array of stream entries, or an empty array if no entries are available.</returns>
    Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, StreamReadOptions options);

    /// <summary>
    /// Reads entries from multiple streams with options including BLOCK support.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread"/>
    /// <param name="streamPositions">A collection of stream keys and their starting positions.</param>
    /// <param name="options">Optional arguments including count and block timeout.</param>
    /// <returns>An array of streams with their entries, or an empty array if no entries are available.</returns>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options);

    /// <summary>
    /// Reads entries from a stream for a consumer group with options including BLOCK support.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="position">The position from which to read.</param>
    /// <param name="options">Optional arguments including count, block timeout, and noAck.</param>
    /// <returns>An array of stream entries.</returns>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, StreamReadGroupOptions options);

    /// <summary>
    /// Reads entries from multiple streams for a consumer group with options including BLOCK support.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup"/>
    /// <param name="streamPositions">A collection of stream keys and their starting positions.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="options">Optional arguments including count, block timeout, and noAck.</param>
    /// <returns>An array of streams with their entries.</returns>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options);

    /// <summary>
    /// Returns detailed stream information including consumer group and PEL details.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-stream/"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="count">Maximum number of PEL entries per consumer to return. Defaults to 10 if null.</param>
    /// <returns>Detailed information about the stream.</returns>
    Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key, int? count = null);

    /// <summary>
    /// Returns the entries in a stream within a specified range of IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xrange"/>
    /// <seealso href="https://valkey.io/commands/xrevrange"/>
    /// <param name="key">The key of the stream.</param>
    /// <param name="options">Optional arguments including range bounds, count, and order.</param>
    /// <returns>An array of stream entries in the specified range.</returns>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions? options = null);
}
