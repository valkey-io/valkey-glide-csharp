// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class BaseClient : IStreamCommands
{
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            minId ?? default,
            useApproximateTrimming,
            [new NameValueEntry(streamField, streamValue)],
            limit,
            mode,
            noMakeStream));
    }

    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, ValkeyValue? minId = null, bool useApproximateTrimming = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, bool noMakeStream = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            minId ?? default,
            useApproximateTrimming,
            streamPairs,
            limit,
            mode,
            noMakeStream));
    }

    public async Task<StreamEntry[]> StreamReadAsync(ValkeyKey key, ValkeyValue position, int? count = null, int? block = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadAsync(key, position, count, block));
    }

    public async Task<ValkeyStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? count = null, int? block = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamReadAsync(streamPositions, count, block));
    }
}
