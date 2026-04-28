// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by <see cref="IBaseClient"/> but NOT by <see cref="IDatabaseAsync"/>. Methods
/// implemented by both should be added to <see cref="IStreamBaseCommands"/> instead.

public partial interface IBaseClient
{
    #region StreamReadGroupAsync

    /// <summary>
    /// Reads entries from a stream for a consumer group.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xreadgroup/">Valkey commands – XREADGROUP</seealso>
    /// <param name="position">The stream key and position from which to read.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <returns>An array of <see cref="StreamEntry"/> values read from the stream.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new StreamPosition("mystream", StreamPosition.UndeliveredMessages);
    /// var entries = await client.StreamReadGroupAsync(position, "mygroup", "myconsumer");
    /// foreach (var entry in entries)
    /// {
    ///     Console.WriteLine($"{entry.Id}: {entry.Values[0].Name} = {entry.Values[0].Value}");
    /// }
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
    /// <returns>An array of <see cref="ValkeyStream"/> values, each containing the stream key and its entries.</returns>
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
    ///     Console.WriteLine($"Stream: {stream.Key}, Entries: {stream.Entries.Length}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue)" path="/*[not(self::returns)]"/>
    /// <param name="options">Options including count, block timeout, and noAck.</param>
    /// <returns>An array of <see cref="StreamEntry"/> values read from the stream.</returns>
    Task<StreamEntry[]> StreamReadGroupAsync(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options);

    /// <inheritdoc cref="StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)" path="/*[not(self::returns)]"/>
    /// <param name="options">Options including count, block timeout, and noAck.</param>
    /// <returns>An array of <see cref="ValkeyStream"/> values, each containing the stream key and its entries.</returns>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options);

    #endregion
    #region StreamAddAsync

    /// <summary>
    /// Appends a new entry to a stream with a single field-value pair.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd/">Valkey commands – XADD</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="streamField">The field name.</param>
    /// <param name="streamValue">The field value.</param>
    /// <returns>The auto-generated ID of the added entry.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entryId = await client.StreamAddAsync("mystream", "sensor", "temperature");  // "1526919030474-0"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue);

    /// <summary>
    /// Appends a new entry to a stream with multiple field-value pairs.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xadd/">Valkey commands – XADD</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <returns>The auto-generated ID of the added entry.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// NameValueEntry[] fields = [
    ///     new("sensor", "temperature"),
    ///     new("value", "23.5")
    /// ];
    /// var entryId = await client.StreamAddAsync("mystream", fields);  // "1526919030474-0"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs);

    /// <inheritdoc cref="StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::returns)]"/>
    /// <param name="options">Options for the XADD command.</param>
    /// <returns>
    /// The ID of the added entry, or <see cref="ValkeyValue.Null"/> if
    /// <see cref="StreamAddOptions.MakeStream"/> is <see langword="false"/> and the stream does not exist.
    /// </returns>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions options);

    /// <inheritdoc cref="StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry})" path="/*[not(self::returns)]"/>
    /// <param name="options">Options for the XADD command.</param>
    /// <returns>
    /// The ID of the added entry, or <see cref="ValkeyValue.Null"/> if
    /// <see cref="StreamAddOptions.MakeStream"/> is <see langword="false"/> and the stream does not exist.
    /// </returns>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions options);

    #endregion
    #region StreamReadAsync

    /// <summary>
    /// Reads entries from a single stream starting from a given position.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread/">Valkey commands – XREAD</seealso>
    /// <param name="position">The stream key and position from which to start reading.</param>
    /// <returns>An array of <see cref="StreamEntry"/> values, or an empty array if no entries are available.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var position = new StreamPosition("mystream", StreamPosition.Beginning);
    /// var entries = await client.StreamReadAsync(position);
    /// foreach (var entry in entries)
    /// {
    ///     Console.WriteLine($"{entry.Id}: {entry.Values[0].Name} = {entry.Values[0].Value}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamReadAsync(StreamPosition position);

    /// <summary>
    /// Reads entries from multiple streams starting from given positions.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xread/">Valkey commands – XREAD</seealso>
    /// <param name="streamPositions">A collection of stream keys and their starting positions.</param>
    /// <returns>An array of <see cref="ValkeyStream"/> values, or an empty array if no entries are available.</returns>
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
    ///     Console.WriteLine($"Stream: {stream.Key}, Entries: {stream.Entries.Length}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions);

    /// <inheritdoc cref="StreamReadAsync(StreamPosition)" path="/*[not(self::returns)]"/>
    /// <param name="options">Options including count and block timeout.</param>
    /// <returns>An array of <see cref="StreamEntry"/> values, or an empty array if no entries are available.</returns>
    Task<StreamEntry[]> StreamReadAsync(StreamPosition position, StreamReadOptions options);

    /// <inheritdoc cref="StreamReadAsync(IEnumerable{StreamPosition})" path="/*[not(self::returns)]"/>
    /// <param name="options">Options including count and block timeout.</param>
    /// <returns>An array of <see cref="ValkeyStream"/> values, or an empty array if no entries are available.</returns>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options);

    #endregion
    #region StreamRangeAsync

    /// <summary>
    /// Returns all entries in a stream in ascending order.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xrange/">Valkey commands – XRANGE</seealso>
    /// <param name="key">The stream key.</param>
    /// <returns>An array of <see cref="StreamEntry"/> values.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entries = await client.StreamRangeAsync("mystream");
    /// foreach (var entry in entries)
    /// {
    ///     Console.WriteLine($"{entry.Id}: {entry.Values[0].Name} = {entry.Values[0].Value}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key);

    /// <inheritdoc cref="StreamRangeAsync(ValkeyKey)" path="/*[not(self::returns)]"/>
    /// <seealso href="https://valkey.io/commands/xrevrange/">Valkey commands – XREVRANGE</seealso>
    /// <param name="options">Options including range bounds, count, and order.</param>
    /// <returns>An array of <see cref="StreamEntry"/> values in the specified range.</returns>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions options);

    #endregion
    #region StreamDeleteAsync

    /// <summary>
    /// Removes a message from a stream.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xdel/">Valkey commands – XDEL</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="messageId">The message ID to delete.</param>
    /// <returns><see langword="true"/> if the message was deleted, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entryId = await client.StreamAddAsync("mystream", "field", "value");
    /// var deleted = await client.StreamDeleteAsync("mystream", entryId);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> StreamDeleteAsync(ValkeyKey key, ValkeyValue messageId);

    #endregion

    // ──────────────────────────────────────────────────────────────────────
    // GLIDE-only methods below are temporarily commented out pending cleanup.
    // The underlying Request methods are kept intact.
    // ──────────────────────────────────────────────────────────────────────

    // #region XGROUP CREATE
    // Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null);
    // #endregion

    // #region XGROUP SETID
    // Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null);
    // #endregion

    // #region XTRIM
    // Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null);
    // Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null);
    // #endregion

    // #region XCLAIM
    // Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options);
    // Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options);
    // Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds);
    // #endregion

    // #region XAUTOCLAIM
    // Task<StreamAutoClaimJustIdResult> StreamAutoClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);
    // #endregion

    // #region XINFO STREAM FULL
    // Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key, int? count = null);
    // #endregion
}
