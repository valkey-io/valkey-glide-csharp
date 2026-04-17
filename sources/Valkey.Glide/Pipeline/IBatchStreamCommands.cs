// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Stream Commands" group for batch requests.
/// </summary>
internal interface IBatchStreamCommands
{
    #region StreamAdd

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue);

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry})" /></returns>
    IBatch StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs);

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamAddOptions)" /></returns>
    IBatch StreamAdd(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, Commands.Options.StreamAddOptions options);

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, Commands.Options.StreamAddOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, Commands.Options.StreamAddOptions)" /></returns>
    IBatch StreamAdd(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, Commands.Options.StreamAddOptions options);

    #endregion
    #region StreamRead

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition)" /></returns>
    IBatch StreamRead(StreamPosition position);

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition})" /></returns>
    IBatch StreamRead(IEnumerable<StreamPosition> streamPositions);

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition, Commands.Options.StreamReadOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamReadAsync(StreamPosition, Commands.Options.StreamReadOptions)" /></returns>
    IBatch StreamRead(StreamPosition position, Commands.Options.StreamReadOptions options);

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition}, Commands.Options.StreamReadOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition}, Commands.Options.StreamReadOptions)" /></returns>
    IBatch StreamRead(IEnumerable<StreamPosition> streamPositions, Commands.Options.StreamReadOptions options);

    #endregion
    #region StreamLength

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamLengthAsync(ValkeyKey)" /></returns>
    IBatch StreamLength(ValkeyKey key);

    #endregion
    #region StreamDelete

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds);

    /// <inheritdoc cref="IBaseClient.StreamDeleteAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamDeleteAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamDelete(ValkeyKey key, ValkeyValue messageId);

    #endregion
    #region StreamRange

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey)" /></returns>
    IBatch StreamRange(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey, Commands.Options.StreamRangeOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey, Commands.Options.StreamRangeOptions)" /></returns>
    IBatch StreamRange(ValkeyKey key, Commands.Options.StreamRangeOptions options);

    #endregion
    #region StreamReadGroup

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName);

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue, Commands.Options.StreamReadGroupOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamReadGroupAsync(StreamPosition, ValkeyValue, ValkeyValue, Commands.Options.StreamReadGroupOptions)" /></returns>
    IBatch StreamReadGroup(StreamPosition position, ValkeyValue groupName, ValkeyValue consumerName, Commands.Options.StreamReadGroupOptions options);

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, Commands.Options.StreamReadGroupOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, Commands.Options.StreamReadGroupOptions)" /></returns>
    IBatch StreamReadGroup(IEnumerable<StreamPosition> positions, ValkeyValue groupName, ValkeyValue consumerName, Commands.Options.StreamReadGroupOptions options);

    #endregion

    // ──────────────────────────────────────────────────────────────────────
    // Batch methods below are temporarily commented out pending cleanup.
    // ──────────────────────────────────────────────────────────────────────

    // IBatch StreamTrim(ValkeyKey key, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null);
    // IBatch StreamCreateConsumerGroup(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null);
    // IBatch StreamDeleteConsumerGroup(ValkeyKey key, ValkeyValue groupName);
    // IBatch StreamCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);
    // IBatch StreamDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);
    // IBatch StreamConsumerGroupSetPosition(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null);
    // IBatch StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds);
    // IBatch StreamPending(ValkeyKey key, ValkeyValue groupName);
    // IBatch StreamPendingMessages(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName = default, ValkeyValue? minId = null, ValkeyValue? maxId = null, TimeSpan? minIdleTime = null);
    // IBatch StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, Commands.Options.StreamClaimOptions? options = null);
    // IBatch StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, Commands.Options.StreamClaimOptions? options = null);
    // IBatch StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);
    // IBatch StreamAutoClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null);
    // IBatch StreamInfo(ValkeyKey key);
    // IBatch StreamGroupInfo(ValkeyKey key);
    // IBatch StreamConsumerInfo(ValkeyKey key, ValkeyValue groupName);
    // IBatch StreamInfoFull(ValkeyKey key, int? count = null);
}
