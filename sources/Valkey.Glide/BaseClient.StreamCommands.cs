// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class BaseClient : IStreamCommands
{
    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, ValkeyValue streamField, ValkeyValue streamValue, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            useApproximateMaxLength,
            [new NameValueEntry(streamField, streamValue)],
            limit,
            mode));
    }

    public async Task<ValkeyValue> StreamAddAsync(ValkeyKey key, NameValueEntry[] streamPairs, ValkeyValue? messageId = null, long? maxLength = null, bool useApproximateMaxLength = false, long? limit = null, StreamTrimMode mode = StreamTrimMode.KeepReferences, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StreamAddAsync(
            key,
            messageId ?? default,
            maxLength,
            useApproximateMaxLength,
            streamPairs,
            limit,
            mode));
    }
}
