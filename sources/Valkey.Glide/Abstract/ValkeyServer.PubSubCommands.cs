// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class ValkeyServer
{
    /// <inheritdoc />
    public async Task<ValkeyChannel[]> SubscriptionChannelsAsync(
        ValkeyChannel pattern = default,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        ISet<ValkeyKey> channels = pattern.IsNullOrEmpty
            ? await _conn.PubSubChannelsAsync()
            : await _conn.PubSubChannelsAsync(pattern.ToValkeyKey());

        return [.. channels.Select(ValkeyChannel.Literal)];
    }

    /// <inheritdoc />
    public async Task<long> SubscriptionPatternCountAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.PubSubNumPatAsync();
    }

    /// <inheritdoc />
    public async Task<long> SubscriptionSubscriberCountAsync(
        ValkeyChannel channel,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await _conn.PubSubNumSubAsync(channel.ToValkeyKey());
    }
}
