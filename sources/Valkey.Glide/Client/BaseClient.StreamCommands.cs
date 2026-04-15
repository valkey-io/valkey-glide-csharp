// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class BaseClient
{
    /// <summary>Appends a new entry to a stream (legacy SER-compatible overload).</summary>
    public async Task<ValkeyValue> StreamAddAsync(
        ValkeyKey key,
        ValkeyValue streamField,
        ValkeyValue streamValue,
        ValkeyValue? messageId,
        int? maxLength,
        bool useApproximateMaxLength)
    {
        var options = new StreamAddOptions { Id = messageId };
        if (maxLength.HasValue)
            options = options with { Trim = new StreamTrimOptions.MaxLen { MaxLength = maxLength.Value, Exact = !useApproximateMaxLength } };
        return await StreamAddAsync(key, streamField, streamValue, options);
    }

    /// <summary>Appends a new entry to a stream with multiple field-value pairs (legacy SER-compatible overload).</summary>
    public async Task<ValkeyValue> StreamAddAsync(
        ValkeyKey key,
        IEnumerable<NameValueEntry> streamPairs,
        ValkeyValue? messageId,
        int? maxLength,
        bool useApproximateMaxLength)
    {
        var options = new StreamAddOptions { Id = messageId };
        if (maxLength.HasValue)
            options = options with { Trim = new StreamTrimOptions.MaxLen { MaxLength = maxLength.Value, Exact = !useApproximateMaxLength } };
        return await StreamAddAsync(key, streamPairs, options);
    }

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, StreamAddOptions?)"/>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, StreamAddOptions? options = null)
    {
        return await Command(Request.StreamAddAsync(key, [new NameValueEntry(streamField, streamValue)], options));
    }

    /// <inheritdoc cref="IBaseClient.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, StreamAddOptions?)"/>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, StreamAddOptions? options = null)
    {
        return await Command(Request.StreamAddAsync(key, [.. streamPairs], options));
    }

    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null)
    {
        return await Command(Request.StreamReadAsync(key, position, count));
    }

    /// <inheritdoc/>
    public async Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? count = null)
    {
        return await Command(Request.StreamReadAsync([.. streamPositions], count));
    }

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(ValkeyKey, ValkeyValue, StreamReadOptions)"/>
    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, StreamReadOptions options)
    {
        return await Command(Request.StreamReadAsync(key, position, options));
    }

    /// <inheritdoc cref="IBaseClient.StreamReadAsync(IEnumerable{StreamPosition}, StreamReadOptions)"/>
    public async Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, StreamReadOptions options)
    {
        return await Command(Request.StreamReadAsync([.. streamPositions], options));
    }

    /// <summary>Queries a stream range (legacy SER-compatible overload).</summary>
    public async Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? minId = null, ValkeyValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending)
    {
        return await Command(Request.StreamRangeAsync(key, minId ?? StreamConstants.ReadMinValue, maxId ?? StreamConstants.ReadMaxValue, count, messageOrder));
    }

    /// <inheritdoc cref="IBaseClient.StreamRangeAsync(ValkeyKey, StreamRangeOptions?)"/>
    public async Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, StreamRangeOptions? options = null)
    {
        var opts = options ?? new StreamRangeOptions();
        return await Command(Request.StreamRangeAsync(
            key,
            opts.MinId ?? StreamConstants.ReadMinValue,
            opts.MaxId ?? StreamConstants.ReadMaxValue,
            opts.Count,
            opts.MessageOrder));
    }

    /// <inheritdoc/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position)
    {
        return await StreamCreateConsumerGroupAsync(key, groupName, position, false, null);
    }

    /// <inheritdoc/>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position = null, bool createStream = true, long? entriesRead = null)
    {
        return await Command(Request.StreamCreateConsumerGroupAsync(key, groupName, position ?? default, createStream, entriesRead));
    }

    /// <inheritdoc/>
    public async Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return await Command(Request.StreamDeleteConsumerGroupAsync(key, groupName));
    }

    /// <inheritdoc/>
    public async Task<bool> StreamConsumerGroupSetPositionAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue position, long? entriesRead = null)
    {
        return await Command(Request.StreamConsumerGroupSetPositionAsync(key, groupName, position, entriesRead));
    }

    /// <inheritdoc/>
    public async Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
    {
        return await Command(Request.StreamCreateConsumerAsync(key, groupName, consumerName));
    }

    /// <inheritdoc/>
    public async Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName)
    {
        return await Command(Request.StreamDeleteConsumerAsync(key, groupName, consumerName));
    }

    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, int? count)
    {
        return await StreamReadGroupAsync(key, groupName, consumerName, position, count, false);
    }

    /// <inheritdoc/>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position = null, int? count = null, bool noAck = false)
    {
        return await Command(Request.StreamReadGroupAsync(key, groupName, consumerName, position ?? default, count, noAck));
    }

    /// <inheritdoc/>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream)
    {
        return await StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, false);
    }

    /// <inheritdoc/>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, int? countPerStream = null, bool noAck = false)
    {
        return await Command(Request.StreamReadGroupAsync([.. streamPositions], groupName, consumerName, countPerStream, noAck));
    }

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, StreamReadGroupOptions)"/>
    public async Task<StreamEntry[]> StreamReadGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, ValkeyValue? position, StreamReadGroupOptions options)
    {
        return await Command(Request.StreamReadGroupAsync(key, groupName, consumerName, position ?? default, options));
    }

    /// <inheritdoc cref="IBaseClient.StreamReadGroupAsync(IEnumerable{StreamPosition}, ValkeyValue, ValkeyValue, StreamReadGroupOptions)"/>
    public async Task<ValkeyStream[]> StreamReadGroupAsync(IEnumerable<StreamPosition> streamPositions, ValkeyValue groupName, ValkeyValue consumerName, StreamReadGroupOptions options)
    {
        return await Command(Request.StreamReadGroupAsync([.. streamPositions], groupName, consumerName, options));
    }

    /// <inheritdoc/>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue messageId)
    {
        return await StreamAcknowledgeAsync(key, groupName, [messageId]);
    }

    /// <inheritdoc/>
    public async Task<long> StreamAcknowledgeAsync(ValkeyKey key, ValkeyValue groupName, IEnumerable<ValkeyValue> messageIds)
    {
        return await Command(Request.StreamAcknowledgeAsync(key, groupName, [.. messageIds]));
    }

    /// <inheritdoc/>
    public async Task<StreamPendingInfo> StreamPendingAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return await Command(Request.StreamPendingAsync(key, groupName));
    }

    /// <inheritdoc/>
    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName, ValkeyValue? minId, ValkeyValue? maxId)
    {
        return await StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, null);
    }

    /// <inheritdoc/>
    public async Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(ValkeyKey key, ValkeyValue groupName, int count, ValkeyValue consumerName = default, ValkeyValue? minId = null, ValkeyValue? maxId = null, TimeSpan? minIdleTime = null)
    {
        return await Command(Request.StreamPendingMessagesAsync(key, groupName, minId ?? StreamConstants.ReadMinValue, maxId ?? StreamConstants.ReadMaxValue, count, consumerName, minIdleTime));
    }
    /// <summary>Claims pending messages for a consumer.</summary>
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds)
    {
        return await Command(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds], null, null, null, false));
    }

    /// <inheritdoc cref="IBaseClient.StreamClaimAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue}, StreamClaimOptions)"/>
    public async Task<StreamEntry[]> StreamClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
    {
        return await Command(Request.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds], options.Idle, options.IdleUnix, options.RetryCount, options.Force));
    }

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue})"/>
    public async Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds)
    {
        return await Command(Request.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds], null, null, null, false));
    }

    /// <inheritdoc cref="IBaseClient.StreamClaimJustIdAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan, IEnumerable{ValkeyValue}, StreamClaimOptions)"/>
    public async Task<ValkeyValue[]> StreamClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, IEnumerable<ValkeyValue> messageIds, StreamClaimOptions options)
    {
        return await Command(Request.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTime, [.. messageIds], options.Idle, options.IdleUnix, options.RetryCount, options.Force));
    }
    /// <inheritdoc/>
    public async Task<StreamAutoClaimResult> StreamAutoClaimAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null)
    {
        return await Command(Request.StreamAutoClaimAsync(key, consumerGroup, claimingConsumer, minIdleTime, startAtId, count));
    }

    /// <inheritdoc/>
    public async Task<StreamAutoClaimJustIdResult> StreamAutoClaimJustIdAsync(ValkeyKey key, ValkeyValue consumerGroup, ValkeyValue claimingConsumer, TimeSpan minIdleTime, ValkeyValue startAtId, int? count = null)
    {
        return await Command(Request.StreamAutoClaimJustIdAsync(key, consumerGroup, claimingConsumer, minIdleTime, startAtId, count));
    }
    /// <inheritdoc/>
    public async Task<long> StreamLengthAsync(ValkeyKey key)
    {
        return await Command(Request.StreamLengthAsync(key));
    }

    /// <inheritdoc/>
    public async Task<long> StreamDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> messageIds)
    {
        return await Command(Request.StreamDeleteAsync(key, [.. messageIds]));
    }

    /// <inheritdoc/>
    public async Task<long> StreamTrimAsync(ValkeyKey key, int maxLength, bool useApproximateMaxLength)
    {
        return await StreamTrimAsync(key, maxLength, useApproximateMaxLength, null);
    }

    /// <inheritdoc/>
    public async Task<long> StreamTrimAsync(ValkeyKey key, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null)
    {
        return await Command(Request.StreamTrimAsync(key, maxLength, default, useApproximateMaxLength, limit));
    }

    /// <inheritdoc/>
    public async Task<long> StreamTrimByMinIdAsync(ValkeyKey key, ValkeyValue minId, bool useApproximateMaxLength = false, long? limit = null)
    {
        return await Command(Request.StreamTrimAsync(key, null, minId, useApproximateMaxLength, limit));
    }

    /// <inheritdoc/>
    public async Task<StreamInfo> StreamInfoAsync(ValkeyKey key)
    {
        return await Command(Request.StreamInfoAsync(key));
    }

    /// <inheritdoc/>
    public async Task<StreamGroupInfo[]> StreamGroupInfoAsync(ValkeyKey key)
    {
        return await Command(Request.StreamGroupInfoAsync(key));
    }

    /// <inheritdoc/>
    public async Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(ValkeyKey key, ValkeyValue groupName)
    {
        return await Command(Request.StreamConsumerInfoAsync(key, groupName));
    }

    /// <inheritdoc cref="IBaseClient.StreamInfoFullAsync(ValkeyKey, int?)"/>
    public async Task<StreamInfoFull> StreamInfoFullAsync(ValkeyKey key, int? count = null)
    {
        return await Command(Request.StreamInfoFullAsync(key, count));
    }
}
