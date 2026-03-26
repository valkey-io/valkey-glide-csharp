// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>Stream commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.</summary>
/// <seealso cref="IStreamCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, int?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, int? maxLength, bool useApproximateMaxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, ValkeyValue, ValkeyValue, ValkeyValue?, long?, bool, long?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, limit, noMakeStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamAddAsync(ValkeyKey, IEnumerable{NameValueEntry}, ValkeyValue?, long?, bool, long?, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, IEnumerable<NameValueEntry> streamPairs, ValkeyValue? messageId, long? maxLength, bool useApproximateMaxLength, long? limit, bool noMakeStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, limit, noMakeStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamReadAsync(ValkeyKey, ValkeyValue, int?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadAsync(key, position, countPerStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamReadAsync(IEnumerable{StreamPosition}, int?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyStream[]> StreamReadAsync(IEnumerable<StreamPosition> streamPositions, int? countPerStream, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamReadAsync(streamPositions, countPerStream);
    }

    /// <inheritdoc cref="IStreamCommands.StreamRangeAsync(ValkeyKey, ValkeyValue?, ValkeyValue?, int?, Order)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<StreamEntry[]> StreamRangeAsync(ValkeyKey key, ValkeyValue? start, ValkeyValue? end, int? count, Order order, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamRangeAsync(key, start, end, count, order);
    }

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position);
    }

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerGroupAsync(ValkeyKey, ValkeyValue, ValkeyValue?, bool, long?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamCreateConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue? position, bool createStream, long? entriesRead, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerGroupAsync(key, groupName, position, createStream, entriesRead);
    }

    /// <inheritdoc cref="IStreamCommands.StreamDeleteConsumerGroupAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamDeleteConsumerGroupAsync(ValkeyKey key, ValkeyValue groupName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamDeleteConsumerGroupAsync(key, groupName);
    }

    /// <inheritdoc cref="IStreamCommands.StreamCreateConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StreamCreateConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamCreateConsumerAsync(key, groupName, consumerName);
    }

    /// <inheritdoc cref="IStreamCommands.StreamDeleteConsumerAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StreamDeleteConsumerAsync(ValkeyKey key, ValkeyValue groupName, ValkeyValue consumerName, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StreamDeleteConsumerAsync(key, groupName, consumerName);
    }
