// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region StreamAddAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAddAsync(key, streamField, streamValue, ToStreamAddOptions(messageId, maxLength, useApproximateMaxLength));
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool, CommandFlags)"/>
    public Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAddAsync(key, streamPairs, ToStreamAddOptions(messageId, maxLength, useApproximateMaxLength));
    }

    #endregion
    #region StreamReadAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamReadAsync(ValkeyKey, ValkeyValue, int?, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadAsync(new StreamPosition(key, position), new StreamReadOptions { Count = countPerStream });
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadAsync(IEnumerable{StreamPosition}, int?, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadAsync(streamPositions, new StreamReadOptions { Count = countPerStream });
    }

    #endregion
    #region StreamRangeAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order, CommandFlags)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var range = StreamIdRange.Between(minId ?? StreamIdBound.Min, maxId ?? StreamIdBound.Max);
        var options = new StreamRangeOptions { Range = range, Count = count, Order = messageOrder };
        return StreamRangeAsync(key, options);
    }

    #endregion
    #region StreamReadGroupAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = new StreamReadGroupOptions { Count = count, NoAck = noAck };
        var sp = new StreamPosition(key, position ?? StreamPosition.UndeliveredMessages);
        return StreamReadGroupAsync(sp, groupName, consumerName, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, TimeSpan?, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, TimeSpan? claimMinIdleTime, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        // TODO #322: Support claimMinIdleTime (SER-specific XREADGROUP + XAUTOCLAIM combination).
        if (claimMinIdleTime is not null)
        {
            throw new NotImplementedException("claimMinIdleTime is a StackExchange.Redis-specific feature that combines XREADGROUP with auto-claiming. Use StreamAutoClaimAsync separately instead.");
        }

        var options = new StreamReadGroupOptions { Count = count, NoAck = noAck };
        var sp = new StreamPosition(key, position ?? StreamPosition.UndeliveredMessages);
        return StreamReadGroupAsync(sp, groupName, consumerName, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = new StreamReadGroupOptions { Count = countPerStream, NoAck = noAck };
        return StreamReadGroupAsync(streamPositions, groupName, consumerName, options);
    }

    #endregion
    #region StreamLengthAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamLengthAsync(key);
    }

    #endregion
    #region StreamDeleteAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamDeleteAsync(key, messageIds);
    }

    #endregion
    #region StreamCreateConsumerGroupAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamCreateConsumerGroupAsync(key, groupName, position ?? default, createStream, null));
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamCreateConsumerGroupAsync(key, groupName, position ?? default, createStream, entriesRead));
    }

    #endregion
    #region StreamDeleteConsumerGroupAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamDeleteConsumerGroupAsync(key, groupName));
    }

    #endregion
    #region StreamCreateConsumerAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamCreateConsumerAsync(key, groupName, consumerName));
    }

    #endregion
    #region StreamDeleteConsumerAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamDeleteConsumerAsync(key, groupName, consumerName));
    }

    #endregion
    #region StreamConsumerGroupSetPositionAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamConsumerGroupSetPositionAsync(key, groupName, position, null));
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?, CommandFlags)"/>
    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead));
    }

    #endregion
    #region StreamAcknowledgeAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamAcknowledgeAsync(key, groupName, messageId));
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamAcknowledgeAsync(key, groupName, [.. messageIds]));
    }

    #endregion
    #region StreamPendingAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamPendingAsync(key, groupName));
    }

    #endregion
    #region StreamPendingMessagesAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, CommandFlags)"/>
    public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamPendingMessagesAsync(key, groupName, minId ?? "-", maxId ?? "+", count, consumerName, null));
    }

    #endregion
    #region StreamClaimAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, TimeSpan.FromMilliseconds(minIdleTimeInMs), [.. messageIds]));
    }

    #endregion
    #region StreamClaimIdsOnlyAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, TimeSpan.FromMilliseconds(minIdleTimeInMs), [.. messageIds]));
    }

    #endregion
    #region StreamAutoClaimAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)"/>
    public Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, TimeSpan.FromMilliseconds(minIdleTimeInMs), startAtId, count));
    }

    #endregion
    #region StreamAutoClaimIdsOnlyAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)"/>
    public Task<StreamAutoClaimJustIdResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamAutoClaimJustIdAsync(key, consumerGroup, claimingConsumer, TimeSpan.FromMilliseconds(minIdleTimeInMs), startAtId, count));
    }

    #endregion
    #region StreamTrimAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimAsync(ValkeyKey, int, bool, CommandFlags)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamTrimAsync(key, maxLength, default, useApproximateMaxLength, null));
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimAsync(ValkeyKey, long?, bool, long?, StreamTrimMode, CommandFlags)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        ThrowIfUnsupportedTrimMode(trimMode);
        return Command(Request.StreamTrimAsync(key, maxLength, default, useApproximateMaxLength, limit));
    }

    #endregion
    #region StreamTrimByMinIdAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimByMinIdAsync(ValkeyKey, ValkeyValue, bool, long?, StreamTrimMode, CommandFlags)"/>
    public Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        ThrowIfUnsupportedTrimMode(trimMode);
        return Command(Request.StreamTrimAsync(key, null, minId, useApproximateMaxLength, limit));
    }

    #endregion
    #region StreamInfoAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamInfoAsync(ValkeyKey, CommandFlags)"/>
    public Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamInfoAsync(key));
    }

    #endregion
    #region StreamGroupInfoAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamGroupInfoAsync(ValkeyKey, CommandFlags)"/>
    public Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamGroupInfoAsync(key));
    }

    #endregion
    #region StreamConsumerInfoAsync

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.StreamConsumerInfoAsync(key, groupName));
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Converts the given arguments to a <see cref="StreamAddOptions"/> instance.
    /// </summary>
    private static StreamAddOptions ToStreamAddOptions(ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit = null) => new()
    {
        Id = messageId ?? StreamAddOptions.AutoGenerateId,
        Trim = maxLength.HasValue
            ? new StreamTrimOptions.MaxLen { MaxLength = maxLength.Value, Exact = !useApproximateMaxLength, Limit = limit }
            : null
    };

    /// <summary>
    /// Throws a <see cref="NotImplementedException"/> if the stream trim mode is not supported.
    /// </summary>
    /// <param name="trimMode">The stream trim mode to validate.</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="trimMode"/> is not <see cref="StreamTrimMode.KeepReferences"/>.</exception>
    private static void ThrowIfUnsupportedTrimMode(StreamTrimMode trimMode)
    {
        if (trimMode != StreamTrimMode.KeepReferences)
        {
            throw new NotImplementedException($"Stream trim mode {trimMode} is not supported by Valkey GLIDE");
        }
    }

    #endregion
}
