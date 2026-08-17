// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by <see cref="IBaseClient"/> but NOT by <see cref="IDatabaseAsync"/>. Methods
/// implemented by both should be added to <see cref="IStreamBaseCommands"/> instead.

public partial interface IBaseClient
{
    #region StreamAddAsync

    /// <summary>
    /// Adds a new entry to a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd/">Valkey commands – XADD</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="streamField">The field name.</param>
    /// <param name="streamValue">The field value.</param>
    /// <returns>The ID of the added entry.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entryId = await client.StreamAddAsync("mystream", "sensor", "temperature");
    /// Console.WriteLine($"Added entry with ID: {entryId}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue);

    /// <summary>
    /// Adds a new entry to a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd/">Valkey commands – XADD</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <returns>The ID of the added entry.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var fields = new NameValueEntry[]
    /// {
    ///     new("sensor", "temperature"),
    ///     new("value", "23.5")
    /// };
    /// var entryId = await client.StreamAddAsync("mystream", fields);
    /// Console.WriteLine($"Added entry with ID: {entryId}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs);

    /// <inheritdoc cref="StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::returns) and not(self::remarks)]"/>
    /// <param name="options">The stream add options to apply.</param>
    /// <returns>
    /// The ID of the added entry, or <see cref="ValkeyValue.Null"/> if
    /// <see cref="StreamAddOptions.MakeStream"/> is <see langword="false"/> and the stream does not exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new StreamAddOptions { MakeStream = false };
    /// var entryId = await client.StreamAddAsync("mystream", "sensor", "temperature", options);
    /// Console.WriteLine($"Added entry with ID: {entryId}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options);

    /// <inheritdoc cref="StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry})" path="/*[not(self::returns) and not(self::remarks)]"/>
    /// <param name="options">The stream add options to apply.</param>
    /// <returns>The ID of the added entry, or <see cref="ValkeyValue.Null"/> if
    /// <see cref="StreamAddOptions.MakeStream"/> is <see langword="false"/> and the stream does not exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var fields = new NameValueEntry[]
    /// {
    ///     new("sensor", "temperature"),
    ///     new("value", "23.5")
    /// };
    /// var options = new StreamAddOptions { MakeStream = false };
    /// var entryId = await client.StreamAddAsync("mystream", fields, options);
    /// Console.WriteLine($"Added entry with ID: {entryId}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options);

    #endregion
    #region StreamAutoClaimAsync

    /// <summary>
    /// Transfers ownership of pending messages.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xautoclaim/">Valkey commands – XAUTOCLAIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the messages.</param>
    /// <param name="options">The stream auto-claim options to apply.</param>
    /// <returns>The claimed entries and the next scan cursor.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var claim = await client.StreamAutoClaimAsync("mystream", "mygroup", "consumer2", StreamAutoClaimOptions.FromStart(TimeSpan.Zero));
    /// Console.WriteLine($"Next cursor {claim.NextStartId}, claimed {claim.ClaimedEntries.Length}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options);

    #endregion
    #region StreamAutoClaimJustIdAsync

    /// <summary>
    /// Transfers ownership of pending messages, returning only the claimed IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xautoclaim/">Valkey commands – XAUTOCLAIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the messages.</param>
    /// <param name="options">The stream auto-claim options to apply.</param>
    /// <returns>The claimed entries and the next scan cursor.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var claim = await client.StreamAutoClaimJustIdAsync("mystream", "mygroup", "consumer2", StreamAutoClaimOptions.FromStart(TimeSpan.Zero));
    /// Console.WriteLine($"Next cursor {claim.NextStartId}, claimed {claim.ClaimedIds.Length}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamAutoClaimJustIdResult> StreamAutoClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, StreamAutoClaimOptions options);

    #endregion
    #region StreamClaimAsync

    /// <summary>
    /// Changes the ownership of one pending message to the given consumer.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim/">Valkey commands – XCLAIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the message.</param>
    /// <param name="messageId">The message ID to claim.</param>
    /// <param name="options">The stream claim options to apply.</param>
    /// <returns>The stream entries that were successfully claimed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var claimed = await client.StreamClaimAsync("mystream", "mygroup", "consumer2", "1526569495631-0", StreamClaimOptions.From(TimeSpan.Zero));
    /// Console.WriteLine($"Claimed {claimed.Length} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options);

    /// <summary>
    /// Changes the ownership of one or more pending messages to the given consumer.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim/">Valkey commands – XCLAIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the messages.</param>
    /// <param name="messageIds">The message IDs to claim.</param>
    /// <param name="options">The stream claim options to apply.</param>
    /// <returns>The stream entries that were successfully claimed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var claimed = await client.StreamClaimAsync("mystream", "mygroup", "consumer2", ["1526569495631-0", "1526569495632-0"], StreamClaimOptions.From(TimeSpan.Zero));
    /// Console.WriteLine($"Claimed {claimed.Length} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options);

    #endregion
    #region StreamClaimJustIdAsync

    /// <summary>
    /// Changes the ownership of one pending message, returning only the claimed message ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim/">Valkey commands – XCLAIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the message.</param>
    /// <param name="messageId">The message ID to claim.</param>
    /// <param name="options">The stream claim options to apply.</param>
    /// <returns>The IDs of the messages that were successfully claimed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var claimedIds = await client.StreamClaimJustIdAsync("mystream", "mygroup", "consumer2", "1526569495631-0", StreamClaimOptions.From(TimeSpan.Zero));
    /// Console.WriteLine($"Claimed {claimedIds.Length} id(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, StreamClaimOptions options);

    /// <summary>
    /// Changes the ownership of one or more pending messages, returning only the claimed message IDs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xclaim/">Valkey commands – XCLAIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the messages.</param>
    /// <param name="messageIds">The message IDs to claim.</param>
    /// <param name="options">The stream claim options to apply.</param>
    /// <returns>The IDs of the messages that were successfully claimed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var claimedIds = await client.StreamClaimJustIdAsync("mystream", "mygroup", "consumer2", ["1526569495631-0", "1526569495632-0"], StreamClaimOptions.From(TimeSpan.Zero));
    /// Console.WriteLine($"Claimed {claimedIds.Length} id(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options);

    #endregion
    #region StreamGroupCreateAsync

    /// <summary>
    /// Creates a new consumer group for a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-create/">Valkey commands – XGROUP CREATE</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The name of the consumer group to create.</param>
    /// <param name="position">The position from which the group starts reading.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.StreamGroupCreateAsync("mystream", "mygroup", StreamPosition.Beginning);
    /// </code>
    /// </example>
    /// </remarks>
    Task StreamGroupCreateAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position);

    /// <inheritdoc cref="StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks)]"/>
    /// <param name="options">The stream group create options to apply.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new StreamGroupCreateOptions { MakeStream = true };
    /// await client.StreamGroupCreateAsync("mystream", "mygroup", StreamPosition.Beginning, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task StreamGroupCreateAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, StreamGroupCreateOptions options);

    #endregion
    #region StreamGroupCreateConsumerAsync

    /// <summary>
    /// Creates a new consumer in a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-createconsumer/">Valkey commands – XGROUP CREATECONSUMER</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The name of the consumer to create.</param>
    /// <returns><see langword="true"/> if the consumer was created, or <see langword="false"/> if it already existed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var created = await client.StreamGroupCreateConsumerAsync("mystream", "mygroup", "myconsumer");
    /// Console.WriteLine($"Consumer created: {created}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> StreamGroupCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    #endregion
    #region StreamGroupDeleteConsumerAsync

    /// <summary>
    /// Deletes a consumer from a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-delconsumer/">Valkey commands – XGROUP DELCONSUMER</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The name of the consumer to delete.</param>
    /// <returns>The number of pending messages the consumer had before deletion.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var pending = await client.StreamGroupDeleteConsumerAsync("mystream", "mygroup", "myconsumer");
    /// Console.WriteLine($"Consumer had {pending} pending message(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> StreamGroupDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    #endregion
    #region StreamGroupDestroyAsync

    /// <summary>
    /// Destroys a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-destroy/">Valkey commands – XGROUP DESTROY</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name to destroy.</param>
    /// <returns><see langword="true"/> if the group was destroyed, or <see langword="false"/> if it did not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var destroyed = await client.StreamGroupDestroyAsync("mystream", "mygroup");
    /// Console.WriteLine($"Group destroyed: {destroyed}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> StreamGroupDestroyAsync(ValkeyKey key, ValkeyValue groupName);

    #endregion
    #region StreamGroupSetIdAsync

    /// <summary>
    /// Sets the last delivered ID for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xgroup-setid/">Valkey commands – XGROUP SETID</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The new position.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.StreamGroupSetIdAsync("mystream", "mygroup", StreamPosition.Beginning);
    /// </code>
    /// </example>
    /// </remarks>
    Task StreamGroupSetIdAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position);

    /// <inheritdoc cref="StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks)]"/>
    /// <param name="entriesRead">The value to set for the group's entries-read counter (ENTRIESREAD).</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.StreamGroupSetIdAsync("mystream", "mygroup", StreamPosition.Beginning, 10);
    /// </code>
    /// </example>
    /// </remarks>
    Task StreamGroupSetIdAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long entriesRead);

    #endregion
    #region StreamInfoConsumersAsync

    /// <summary>
    /// Returns information about the consumers of a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-consumers/">Valkey commands – XINFO CONSUMERS</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <returns>Information about the consumers.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var consumers = await client.StreamInfoConsumersAsync("mystream", "mygroup");
    /// Console.WriteLine($"Group has {consumers.Length} consumer(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamConsumerInfo[]> StreamInfoConsumersAsync(ValkeyKey key, ValkeyValue groupName);

    #endregion
    #region StreamInfoFullAsync

    /// <summary>
    /// Returns the full, detailed state of a stream, including its consumer groups, consumers,
    /// and pending entries lists.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-stream/">Valkey commands – XINFO STREAM FULL</seealso>
    /// <param name="key">The stream key.</param>
    /// <returns>Full information about the stream.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var info = await client.StreamInfoFullAsync("mystream");
    /// Console.WriteLine($"Length {info.Length}, groups {info.Groups.Length}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key);

    /// <inheritdoc cref="StreamInfoFullAsync(ValkeyKey)" path="/*[not(self::remarks)]"/>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// var info = await client.StreamInfoFullAsync("mystream", 10);
    /// Console.WriteLine($"Length {info.Length}, groups {info.Groups.Length}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key, int count);

    #endregion
    #region StreamInfoGroupsAsync

    /// <summary>
    /// Returns information about consumer groups.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xinfo-groups/">Valkey commands – XINFO GROUPS</seealso>
    /// <param name="key">The stream key.</param>
    /// <returns>Information about the consumer groups.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var groups = await client.StreamInfoGroupsAsync("mystream");
    /// Console.WriteLine($"Stream has {groups.Length} group(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamGroupInfo[]> StreamInfoGroupsAsync(ValkeyKey key);

    #endregion
    #region StreamPendingAsync

    /// <summary>
    /// Returns the detailed list of pending messages for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xpending/">Valkey commands – XPENDING</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="options">The stream pending options to apply.</param>
    /// <returns>Information about the pending messages.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var messages = await client.StreamPendingAsync("mystream", "mygroup", new StreamPendingOptions { Count = 10 });
    /// Console.WriteLine($"{messages.Length} pending message(s)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamPendingMessageInfo[]> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, StreamPendingOptions options);

    #endregion
    #region StreamRangeAsync

    /// <summary>
    /// Returns all entries in a stream in ascending order.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xrange/">Valkey commands – XRANGE</seealso>
    /// <param name="key">The stream key.</param>
    /// <returns>The stream entries.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entries = await client.StreamRangeAsync("mystream");
    /// Console.WriteLine($"{entries.Length} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key);

    /// <inheritdoc cref="StreamRangeAsync(ValkeyKey)" path="/*[not(self::returns) and not(self::remarks)]"/>
    /// <seealso href="https://valkey.io/commands/xrevrange/">Valkey commands – XREVRANGE</seealso>
    /// <param name="options">The stream range options to apply.</param>
    /// <returns>The stream entries in the specified range.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new StreamRangeOptions { Count = 10 };
    /// var entries = await client.StreamRangeAsync("mystream", options);
    /// Console.WriteLine($"{entries.Length} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions options);

    #endregion
    #region StreamReadAsync

    /// <summary>
    /// Reads entries from a single stream starting from a given position.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread/">Valkey commands – XREAD</seealso>
    /// <param name="position">The stream key and position from which to start reading.</param>
    /// <returns>The stream entries, or an empty array if no entries are available.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new StreamPosition("mystream", StreamPosition.Beginning);
    /// var entries = await client.StreamReadAsync(position);
    /// Console.WriteLine($"{entries.Length} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamReadAsync(StreamPosition position);

    /// <summary>
    /// Reads entries from multiple streams starting from given positions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread/">Valkey commands – XREAD</seealso>
    /// <param name="streamPositions">A collection of stream keys and their starting positions.</param>
    /// <returns>The stream entries, or an empty array if no entries are available.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// StreamPosition[] positions = [
    ///     new("stream1", StreamPosition.Beginning),
    ///     new("stream2", StreamPosition.Beginning)
    /// ];
    /// var streams = await client.StreamReadAsync(positions);
    /// foreach (var stream in streams)
    /// {
    ///     Console.WriteLine($"Stream {stream.Key}: {stream.Entries.Length} entry(ies)");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions);

    /// <inheritdoc cref="StreamReadAsync(StreamPosition)" path="/*[not(self::returns) and not(self::remarks)]"/>
    /// <param name="options">The stream read options to apply.</param>
    /// <returns>The stream entries, or an empty array if no entries are available.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new StreamPosition("mystream", StreamPosition.Beginning);
    /// var options = new StreamReadOptions { Count = 10 };
    /// var entries = await client.StreamReadAsync(position, options);
    /// Console.WriteLine($"{entries.Length} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamReadAsync(StreamPosition position, StreamReadOptions options);

    /// <inheritdoc cref="StreamReadAsync(IEnumerable{StreamPosition})" path="/*[not(self::returns) and not(self::remarks)]"/>
    /// <param name="options">The stream read options to apply.</param>
    /// <returns>The stream entries, or an empty array if no entries are available.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// StreamPosition[] positions = [
    ///     new("stream1", StreamPosition.Beginning),
    ///     new("stream2", StreamPosition.Beginning)
    /// ];
    /// var options = new StreamReadOptions { Count = 10 };
    /// var streams = await client.StreamReadAsync(positions, options);
    /// foreach (var stream in streams)
    /// {
    ///     Console.WriteLine($"Stream {stream.Key}: {stream.Entries.Length} entry(ies)");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options);

    #endregion
    #region StreamReadGroupAsync

    /// <summary>
    /// Reads entries from a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup/">Valkey commands – XREADGROUP</seealso>
    /// <param name="position">The stream key and position from which to read.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <returns>The stream entries read from the stream.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new StreamPosition("mystream", StreamPosition.UndeliveredMessages);
    /// var entries = await client.StreamReadGroupAsync(position, "mygroup", "myconsumer");
    /// Console.WriteLine($"{entries.Length} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName);

    /// <summary>
    /// Reads entries from multiple streams for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup/">Valkey commands – XREADGROUP</seealso>
    /// <param name="positions">A collection of stream keys and their starting positions.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <returns>The stream keys and their entries.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// StreamPosition[] positions = [
    ///     new("stream1", StreamPosition.UndeliveredMessages),
    ///     new("stream2", StreamPosition.UndeliveredMessages)
    /// ];
    /// var streams = await client.StreamReadGroupAsync(positions, "mygroup", "myconsumer");
    /// foreach (var stream in streams)
    /// {
    ///     Console.WriteLine($"Stream {stream.Key}: {stream.Entries.Length} entry(ies)");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue)" path="/*[not(self::returns) and not(self::remarks)]"/>
    /// <param name="options">The stream read group options to apply.</param>
    /// <returns>The stream entries read from the stream.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new StreamPosition("mystream", StreamPosition.UndeliveredMessages);
    /// var options = new StreamReadGroupOptions { Count = 10 };
    /// var entries = await client.StreamReadGroupAsync(position, "mygroup", "myconsumer", options);
    /// Console.WriteLine($"{entries.Length} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options);

    /// <inheritdoc cref="StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)" path="/*[not(self::returns) and not(self::remarks)]"/>
    /// <param name="options">The stream read group options to apply.</param>
    /// <returns>The stream keys and their entries.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// StreamPosition[] positions = [
    ///     new("stream1", StreamPosition.UndeliveredMessages),
    ///     new("stream2", StreamPosition.UndeliveredMessages)
    /// ];
    /// var options = new StreamReadGroupOptions { Count = 10 };
    /// var streams = await client.StreamReadGroupAsync(positions, "mygroup", "myconsumer", options);
    /// foreach (var stream in streams)
    /// {
    ///     Console.WriteLine($"Stream {stream.Key}: {stream.Entries.Length} entry(ies)");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options);

    #endregion
    #region StreamTrimAsync

    /// <summary>
    /// Trims a stream to a given size.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim/">Valkey commands – XTRIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="options">The stream trim options to apply.</param>
    /// <returns>The number of entries removed from the stream.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var trimmed = await client.StreamTrimAsync("mystream", new StreamTrimOptions.MaxLen { MaxLength = 1 });
    /// Console.WriteLine($"Trimmed {trimmed} entry(ies)");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> StreamTrimAsync(ValkeyKey key, StreamTrimOptions options);

    #endregion
}
