// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "Stream Commands" group for batch requests.
/// </summary>
internal interface IBatchStreamCommands
{
    #region StreamAcknowledge

    /// <inheritdoc cref="IBaseClient.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamAcknowledge(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds);

    #endregion
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
    #region StreamAutoClaim

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamAutoClaimOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamAutoClaimOptions)" /></returns>
    IBatch StreamAutoClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, Commands.Options.StreamAutoClaimOptions options);

    #endregion
    #region StreamAutoClaimJustId

    /// <inheritdoc cref="IBaseClient.StreamAutoClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamAutoClaimOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamAutoClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamAutoClaimOptions)" /></returns>
    IBatch StreamAutoClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, Commands.Options.StreamAutoClaimOptions options);

    #endregion
    #region StreamClaim

    /// <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, Commands.Options.StreamClaimOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, Commands.Options.StreamClaimOptions)" /></returns>
    IBatch StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, Commands.Options.StreamClaimOptions options);

    /// <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, Commands.Options.StreamClaimOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, Commands.Options.StreamClaimOptions)" /></returns>
    IBatch StreamClaim(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, Commands.Options.StreamClaimOptions options);

    #endregion
    #region StreamClaimJustId

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, Commands.Options.StreamClaimOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue, Commands.Options.StreamClaimOptions)" /></returns>
    IBatch StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, ValkeyValue messageId, Commands.Options.StreamClaimOptions options);

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, Commands.Options.StreamClaimOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, IEnumerable{ValkeyValue}, Commands.Options.StreamClaimOptions)" /></returns>
    IBatch StreamClaimJustId(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, IEnumerable<ValkeyValue> messageIds, Commands.Options.StreamClaimOptions options);

    #endregion
    #region StreamDelete

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch StreamDelete(ValkeyKey key, IEnumerable<ValkeyValue> messageIds);

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamDeleteAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamDelete(ValkeyKey key, ValkeyValue messageId);

    #endregion
    #region StreamGroupCreate

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamGroupCreate(ValkeyKey key, ValkeyValue groupName, ValkeyValue position);

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamGroupCreateOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamGroupCreateAsync(ValkeyKey, ValkeyValue, ValkeyValue, Commands.Options.StreamGroupCreateOptions)" /></returns>
    IBatch StreamGroupCreate(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, Commands.Options.StreamGroupCreateOptions options);

    #endregion
    #region StreamGroupCreateConsumer

    /// <inheritdoc cref="IBaseClient.StreamGroupCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamGroupCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamGroupCreateConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    #endregion
    #region StreamGroupDeleteConsumer

    /// <inheritdoc cref="IBaseClient.StreamGroupDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamGroupDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamGroupDeleteConsumer(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName);

    #endregion
    #region StreamGroupDestroy

    /// <inheritdoc cref="IBaseClient.StreamGroupDestroyAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamGroupDestroyAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamGroupDestroy(ValkeyKey key, ValkeyValue groupName);

    #endregion
    #region StreamGroupSetId

    /// <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch StreamGroupSetId(ValkeyKey key, ValkeyValue groupName, ValkeyValue position);

    /// <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamGroupSetIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, long)" /></returns>
    IBatch StreamGroupSetId(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long entriesRead);

    #endregion
    #region StreamInfo

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamInfoAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamInfoAsync(ValkeyKey)" /></returns>
    IBatch StreamInfo(ValkeyKey key);

    #endregion
    #region StreamInfoConsumers

    /// <inheritdoc cref="IBaseClient.StreamInfoConsumersAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamInfoConsumersAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamInfoConsumers(ValkeyKey key, ValkeyValue groupName);

    #endregion
    #region StreamInfoFull

    /// <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey)" /></returns>
    IBatch StreamInfoFull(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey, int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey, int)" /></returns>
    IBatch StreamInfoFull(ValkeyKey key, int count);

    #endregion
    #region StreamInfoGroups

    /// <inheritdoc cref="IBaseClient.StreamInfoGroupsAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamInfoGroupsAsync(ValkeyKey)" /></returns>
    IBatch StreamInfoGroups(ValkeyKey key);

    #endregion
    #region StreamLength

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamLengthAsync(ValkeyKey)" /></returns>
    IBatch StreamLength(ValkeyKey key);

    #endregion
    #region StreamPending

    /// <inheritdoc cref="Commands.IStreamBaseCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStreamBaseCommands.StreamPendingAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StreamPending(ValkeyKey key, ValkeyValue groupName);

    /// <inheritdoc cref="IBaseClient.StreamPendingAsync(ValkeyKey, ValkeyValue, Commands.Options.StreamPendingOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamPendingAsync(ValkeyKey, ValkeyValue, Commands.Options.StreamPendingOptions)" /></returns>
    IBatch StreamPending(ValkeyKey key, ValkeyValue groupName, Commands.Options.StreamPendingOptions options);

    #endregion
    #region StreamRange

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey)" /></returns>
    IBatch StreamRange(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey, Commands.Options.StreamRangeOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey, Commands.Options.StreamRangeOptions)" /></returns>
    IBatch StreamRange(ValkeyKey key, Commands.Options.StreamRangeOptions options);

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
    #region StreamTrim

    /// <inheritdoc cref="IBaseClient.StreamTrimAsync(ValkeyKey, Commands.Options.StreamTrimOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.StreamTrimAsync(ValkeyKey, Commands.Options.StreamTrimOptions)" /></returns>
    IBatch StreamTrim(ValkeyKey key, Commands.Options.StreamTrimOptions options);

    #endregion
}
