// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

// ATTENTION: Methods should only be added to this interface if they are implemented by Valkey GLIDE clients
// but NOT by StackExchange.Redis databases. Methods implemented by both should be added to the corresponding
// Commands interface instead.

public partial interface IBaseClient
{
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
