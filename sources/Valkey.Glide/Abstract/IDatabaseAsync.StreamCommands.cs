// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IStreamBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    #region StreamAddAsync

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="streamField">The field name.</param>
    /// <param name="streamValue">The field value.</param>
    /// <param name="messageId">The explicit message ID, or <see langword="null"/> for auto-generation.</param>
    /// <param name="maxLength">The maximum number of entries in the stream.</param>
    /// <param name="useApproximateMaxLength">Whether to use approximate trimming.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The ID of the added entry.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry})" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="streamPairs">The field-value pairs to add.</param>
    /// <param name="messageId">The explicit message ID, or <see langword="null"/> for auto-generation.</param>
    /// <param name="maxLength">The maximum number of entries in the stream.</param>
    /// <param name="useApproximateMaxLength">Whether to use approximate trimming.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The ID of the added entry.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamReadAsync

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="position">The position from which to start reading.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="StreamEntry"/> values.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition})" path="/*[self::summary or self::seealso]"/>
    /// <param name="streamPositions">A collection of stream keys and their starting positions.</param>
    /// <param name="countPerStream">The maximum number of entries to return per stream.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="ValkeyStream"/> values.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamRangeAsync

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="minId">The minimum entry ID.</param>
    /// <param name="maxId">The maximum entry ID.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="messageOrder">The order of the returned entries.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="StreamEntry"/> values in the specified range.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamReadGroupAsync

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="position">The position from which to read.</param>
    /// <param name="count">The maximum number of entries to return.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="StreamEntry"/> values read from the stream.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags);

    /// <inheritdoc cref="StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <param name="noAck">Whether to skip adding entries to the PEL.</param>
    /// <returns>An array of <see cref="StreamEntry"/> values read from the stream.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, CommandFlags flags);

    /// <inheritdoc cref="StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <param name="claimMinIdleTime">The minimum idle time for claiming pending entries (StackExchange.Redis-specific; combines XREADGROUP with auto-claim). Not supported by GLIDE (see issue #322).</param>
    /// <returns>An array of <see cref="StreamEntry"/> values read from the stream.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>, or if <paramref name="claimMinIdleTime"/> is not <see langword="null"/>.</exception>
    Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, TimeSpan? claimMinIdleTime = null, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="streamPositions">A collection of stream keys and their starting positions.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="countPerStream">The maximum number of entries to return per stream.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="ValkeyStream"/> values.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags);

    /// <inheritdoc cref="StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <param name="noAck">Whether to skip adding entries to the PEL.</param>
    /// <returns>An array of <see cref="ValkeyStream"/> values.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck, CommandFlags flags);

    /// <inheritdoc cref="StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <param name="claimMinIdleTime">The minimum idle time for claiming pending entries (StackExchange.Redis-specific; combines XREADGROUP with auto-claim). Not supported by GLIDE (see issue #322).</param>
    /// <returns>An array of <see cref="ValkeyStream"/> values.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>, or if <paramref name="claimMinIdleTime"/> is not <see langword="null"/>.</exception>
    Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, TimeSpan? claimMinIdleTime = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamLengthAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags);

    #endregion
    #region StreamDeleteAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    #endregion
    #region StreamCreateConsumerGroupAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The position from which to start reading, or <see langword="null"/> for the latest.</param>
    /// <param name="createStream">Whether to create the stream if it does not exist.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the consumer group was created.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <param name="entriesRead">The number of entries read (Valkey 7.0+).</param>
    /// <returns><see langword="true"/> if the consumer group was created.</returns>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags);

    /// <inheritdoc cref="StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, CommandFlags)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The position from which to start reading, or <see langword="null"/> for the latest.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the consumer group was created.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>StackExchange.Redis compatibility overload; the stream is created if it does not already exist.</remarks>
    Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags);

    #endregion
    #region StreamDeleteConsumerGroupAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupDestroyAsync(ValkeyKey, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the consumer group was destroyed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamCreateConsumerAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the consumer was created.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamDeleteConsumerAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="consumerName">The consumer name.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of pending entries the consumer had before deletion.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamConsumerGroupSetPositionAsync

    /// <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="position">The new position.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the position was set.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)" path="/*[not(self::returns)]"/>
    /// <param name="entriesRead">The number of entries read (Valkey 7.0+).</param>
    /// <returns><see langword="true"/> if the position was set.</returns>
    Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamAcknowledgeAsync

    /// <inheritdoc cref="IBaseClient.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="messageId">The message ID to acknowledge.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of messages acknowledged.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="messageIds">The message IDs to acknowledge.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of messages acknowledged.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags);

    #endregion
    #region StreamPendingAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A <see cref="StreamPendingInfo"/> containing the pending messages summary.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags);

    #endregion
    #region StreamPendingMessagesAsync

    /// <inheritdoc cref="IBaseClient.StreamPendingAsync(ValkeyKey, ValkeyValue, Commands.Options.StreamPendingOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="count">The maximum number of pending messages to return.</param>
    /// <param name="consumerName">The consumer name to filter by.</param>
    /// <param name="minId">The minimum message ID.</param>
    /// <param name="maxId">The maximum message ID.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="StreamPendingMessageInfo"/> values.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamClaimAsync

    /// <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, Commands.Options.StreamClaimOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="messageIds">The message IDs to claim.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="StreamEntry"/> values for the claimed messages.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamClaimIdsOnlyAsync

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, Commands.Options.StreamClaimOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="messageIds">The message IDs to claim.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of claimed message IDs.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamAutoClaimAsync

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamAutoClaimOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="startAtId">The message ID to start scanning from.</param>
    /// <param name="count">The maximum number of messages to claim.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A <see cref="StreamAutoClaimResult"/> containing the claimed entries.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamAutoClaimIdsOnlyAsync

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamAutoClaimOptions)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="consumerGroup">The consumer group name.</param>
    /// <param name="claimingConsumer">The consumer that will take ownership of the messages.</param>
    /// <param name="minIdleTimeInMs">The minimum idle time in milliseconds.</param>
    /// <param name="startAtId">The message ID to start scanning from.</param>
    /// <param name="count">The maximum number of messages to claim.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A <see cref="StreamAutoClaimJustIdResult"/> containing the claimed message IDs.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamTrimAsync

    /// <summary>
    /// Trims a stream to a given size.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim/">Valkey commands – XTRIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="maxLength">The maximum number of entries to keep.</param>
    /// <param name="useApproximateMaxLength">Whether to use approximate trimming.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of entries removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// The default <paramref name="useApproximateMaxLength"/> and <paramref name="flags"/>
    /// values on this overload will be removed to match StackExchange.Redis.
    /// See <see href="https://github.com/valkey-io/valkey-glide-csharp/issues/486">#486</see>.
    /// </remarks>
    // TODO #486: Remove default parameter values to match StackExchange.Redis signature in 2.0.
    Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="StreamTrimAsync(ValkeyKey, int, bool, CommandFlags)" path="/*[not(self::param[@name='maxLength']) and not(self::returns)]"/>
    /// <param name="maxLength">The maximum number of entries to keep. Must not be <c>null</c>.</param>
    /// <param name="limit">The maximum number of entries to evict per call.</param>
    /// <param name="trimMode">The trim mode to use.</param>
    /// <returns>The number of entries removed.</returns>
    /// <remarks>
    /// The <paramref name="maxLength"/> parameter will change from <c>long?</c> to <c>long</c>
    /// match StackExchange.Redis. See <see href="https://github.com/valkey-io/valkey-glide-csharp/issues/486">#486</see>.
    /// </remarks>
    // TODO #486: Change `long? maxLength = null` to `long maxLength` to match StackExchange.Redis signature in 2.0.
    Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamTrimByMinIdAsync

    /// <summary>
    /// Trims the stream by minimum ID.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/xtrim/">Valkey commands – XTRIM</seealso>
    /// <param name="key">The stream key.</param>
    /// <param name="minId">The minimum entry ID to keep.</param>
    /// <param name="useApproximateMaxLength">Whether to use approximate trimming.</param>
    /// <param name="limit">The maximum number of entries to evict per call.</param>
    /// <param name="trimMode">The trim mode to use.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The number of entries removed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamInfoAsync

    /// <inheritdoc cref="IStreamBaseCommands.StreamInfoAsync(ValkeyKey)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A <see cref="StreamInfo"/> containing the stream information.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags);

    #endregion
    #region StreamGroupInfoAsync

    /// <inheritdoc cref="IBaseClient.StreamInfoGroupsAsync(ValkeyKey)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="StreamGroupInfo"/> values.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    #endregion
    #region StreamConsumerInfoAsync

    /// <inheritdoc cref="IBaseClient.StreamInfoConsumersAsync(ValkeyKey, ValkeyValue)" path="/*[self::summary or self::seealso]"/>
    /// <param name="key">The stream key.</param>
    /// <param name="groupName">The consumer group name.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>An array of <see cref="StreamConsumerInfo"/> values.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None);

    #endregion
}
