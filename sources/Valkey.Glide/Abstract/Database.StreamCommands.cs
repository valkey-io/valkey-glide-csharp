// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Public Methods

    /// <inheritdoc cref="IDatabaseAsync.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var result = await ((IBaseClient)this).StreamAcknowledgeAsync(key, groupName, messageId);
        return result ? 1L : 0L;
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAcknowledgeAsync(ValkeyKey, ValkeyValue, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamAcknowledgeAsync(key, groupName, messageIds);
    }

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

    /// <inheritdoc cref="IDatabaseAsync.StreamAutoClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)"/>
    public Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        var options = StreamAutoClaimOptions.FromId(TimeSpan.FromMilliseconds(minIdleTimeInMs), startAtId);
        if (count.HasValue)
        {
            _ = options.WithCount(count.Value);
        }

        return StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamAutoClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, ValkeyValue, int?, CommandFlags)"/>
    public async Task<StreamAutoClaimIdsOnlyResult> StreamAutoClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, ValkeyValue startAtId, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        var options = StreamAutoClaimOptions.FromId(TimeSpan.FromMilliseconds(minIdleTimeInMs), startAtId);
        if (count.HasValue)
        {
            _ = options.WithCount(count.Value);
        }

        var result = await StreamAutoClaimJustIdAsync(key, consumerGroup, claimingConsumer, options);
        return new StreamAutoClaimIdsOnlyResult(result.NextStartId, result.ClaimedIds, result.DeletedIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = StreamClaimOptions.From(TimeSpan.FromMilliseconds(minIdleTimeInMs));
        return StreamClaimAsync(key, consumerGroup, claimingConsumer, messageIds, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamClaimIdsOnlyAsync(ValkeyKey, ValkeyValue, ValkeyValue, long, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<ValkeyValue[]> StreamClaimIdsOnlyAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, long minIdleTimeInMs, IEnumerable<ValkeyValue> messageIds, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = StreamClaimOptions.From(TimeSpan.FromMilliseconds(minIdleTimeInMs));
        return StreamClaimJustIdAsync(key, consumerGroup, claimingConsumer, messageIds, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await StreamGroupSetIdAsync(key, groupName, position);
        return true;
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerGroupSetPositionAsync(ValkeyKey, ValkeyValue, ValkeyValue, long?, CommandFlags)"/>
    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        if (entriesRead.HasValue)
        {
            await StreamGroupSetIdAsync(key, groupName, position, entriesRead.Value);
        }
        else
        {
            await StreamGroupSetIdAsync(key, groupName, position);
        }

        return true;
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamConsumerInfoAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamInfoConsumersAsync(key, groupName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamGroupCreateConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        var options = new StreamGroupCreateOptions { MakeStream = createStream };
        await StreamGroupCreateAsync(key, groupName, position ?? StreamPosition.NewMessages, options);

        return true;
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        var options = new StreamGroupCreateOptions { MakeStream = createStream, EntriesRead = entriesRead };
        await StreamGroupCreateAsync(key, groupName, position ?? StreamPosition.NewMessages, options);

        return true;
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, CommandFlags)"/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        var options = new StreamGroupCreateOptions { MakeStream = true };
        await StreamGroupCreateAsync(key, groupName, position ?? StreamPosition.NewMessages, options);

        return true;
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamDeleteAsync(key, messageIds);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamGroupDeleteConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamGroupDestroyAsync(key, groupName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamGroupInfoAsync(ValkeyKey, CommandFlags)"/>
    public Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamInfoGroupsAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamInfoAsync(ValkeyKey, CommandFlags)"/>
    public Task<StreamInfo> StreamInfoAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamInfoAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> StreamLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamPendingAsync(key, groupName);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamPendingMessagesAsync(ValkeyKey, ValkeyValue, int, ValkeyValue, ValkeyValue?, ValkeyValue?, CommandFlags)"/>
    public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId = null, ValkeyValue? maxId = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        var options = new StreamPendingOptions
        {
            Count = count,
            Start = minId.HasValue ? StreamIdBound.Inclusive(minId.Value) : StreamIdBound.Min,
            End = maxId.HasValue ? StreamIdBound.Inclusive(maxId.Value) : StreamIdBound.Max,
            ConsumerName = consumerName,
        };

        return StreamPendingAsync(key, groupName, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order, CommandFlags)"/>
    public Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        var range = StreamIdRange.Between(minId ?? StreamIdBound.Min, maxId ?? StreamIdBound.Max);
        var options = new StreamRangeOptions { Range = range, Count = count, Order = messageOrder };

        return StreamRangeAsync(key, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadAsync(ValkeyKey, ValkeyValue, int?, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadAsync(new StreamPosition(key, position), new StreamReadOptions { Count = count });
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadAsync(IEnumerable{StreamPosition}, int?, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StreamReadAsync(streamPositions, new StreamReadOptions { Count = countPerStream });
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, CommandFlags flags)
        => StreamReadGroupAsync(key, groupName, consumerName, position, count, noAck: false, claimMinIdleTime: null, flags: flags);

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count, bool noAck, CommandFlags flags)
        => StreamReadGroupAsync(key, groupName, consumerName, position, count, noAck, claimMinIdleTime: null, flags: flags);

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool, TimeSpan?, CommandFlags)"/>
    public Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false, TimeSpan? claimMinIdleTime = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        ThrowIfClaimMinIdleTime(claimMinIdleTime);

        var options = new StreamReadGroupOptions { Count = count, NoAck = noAck };
        var sp = new StreamPosition(key, position ?? StreamPosition.UndeliveredMessages);

        return StreamReadGroupAsync(sp, groupName, consumerName, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, CommandFlags flags)
        => StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck: false, claimMinIdleTime: null, flags: flags);

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream, bool noAck, CommandFlags flags)
        => StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, noAck, claimMinIdleTime: null, flags: flags);

    /// <inheritdoc cref="IDatabaseAsync.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, int?, bool, TimeSpan?, CommandFlags)"/>
    public Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false, TimeSpan? claimMinIdleTime = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        ThrowIfClaimMinIdleTime(claimMinIdleTime);

        var options = new StreamReadGroupOptions { Count = countPerStream, NoAck = noAck };
        return StreamReadGroupAsync(streamPositions, groupName, consumerName, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimAsync(ValkeyKey, int, bool, CommandFlags)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        var options = new StreamTrimOptions.MaxLen
        {
            MaxLength = maxLength,
            Exact = !useApproximateMaxLength,
        };

        return StreamTrimAsync(key, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimAsync(ValkeyKey, long?, bool, long?, StreamTrimMode, CommandFlags)"/>
    public Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GuardClauses.ThrowIfNotSupported(trimMode);

        // TODO #486: Remove once maxLength is no longer optional.
        ArgumentNullException.ThrowIfNull(maxLength, nameof(maxLength));

        var options = new StreamTrimOptions.MaxLen
        {
            MaxLength = maxLength.Value,
            Exact = !useApproximateMaxLength,
            Limit = limit
        };

        return StreamTrimAsync(key, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StreamTrimByMinIdAsync(ValkeyKey, ValkeyValue, bool, long?, StreamTrimMode, CommandFlags)"/>
    public Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode trimMode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GuardClauses.ThrowIfNotSupported(trimMode);

        var options = new StreamTrimOptions.MinId
        {
            MinEntryId = minId,
            Exact = !useApproximateMaxLength,
            Limit = limit
        };

        return StreamTrimAsync(key, options);
    }

    #endregion
    #region Private Methods

    // TODO #322: Support claimMinIdleTime (SER-specific XREADGROUP + XAUTOCLAIM combination).
    private static void ThrowIfClaimMinIdleTime(TimeSpan? claimMinIdleTime)
    {
        if (claimMinIdleTime is not null)
        {
            throw new NotImplementedException("claimMinIdleTime is a StackExchange.Redis-specific feature that combines XREADGROUP with auto-claiming. Use StreamAutoClaimAsync separately instead.");
        }
    }

    /// <summary>
    /// Converts the given arguments to a <see cref="StreamAddOptions"/> instance.
    /// </summary>
    /// <param name="messageId">The message ID to assign, or <see langword="null"/> to auto-generate one.</param>
    /// <param name="maxLength">The maximum number of entries to keep, or <see langword="null"/> for no trimming.</param>
    /// <param name="useApproximateMaxLength">Whether to trim approximately for better performance.</param>
    /// <param name="limit">The maximum number of entries to trim per operation.</param>
    private static StreamAddOptions ToStreamAddOptions(
        ValkeyValue? messageId,
        long? maxLength,
        bool useApproximateMaxLength,
        long? limit = null)
    => new()
    {
        Id = messageId ?? StreamAddOptions.AutoGenerateId,
        Trim = maxLength.HasValue
                ? new StreamTrimOptions.MaxLen { MaxLength = maxLength.Value, Exact = !useApproximateMaxLength, Limit = limit }
                : null
    };

    #endregion
}
