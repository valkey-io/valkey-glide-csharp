// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient : IPubSubClusterCommands
{
    #region PublishCommands

    public async Task<long> SPublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.SPublish(channel, message));
    }

    #endregion
    #region SubscribeCommands

    public async Task SSubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SSubscribe([channel]));
    }

    public async Task SSubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SSubscribe(channels.ToGlideStrings()));
    }

    #endregion
    #region UnsubscribeCommands

    public async Task SUnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe([]));
    }

    public async Task SUnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe([channel]));
    }

    public async Task SUnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe(channels.ToGlideStrings()));
    }

    #endregion
    #region PubSubInfoCommands

    public async Task<string[]> PubSubShardChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubShardChannels());
    }

    public async Task<string[]> PubSubShardChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubShardChannels(pattern));
    }

    public async Task<Dictionary<string, long>> PubSubShardNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubShardNumSub(channels.ToGlideStrings()));
    }

    #endregion
}
