// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient : IPubSubClusterCommands
{
    /// <inheritdoc/>
    protected override Route? PubSubRoute { get; } = Route.Random;

    /// <inheritdoc/>
    protected override Route? PubSubInfoRoute { get; } = Route.AllPrimaries;

    #region PublishCommands

    /// <inheritdoc/>
    public async Task<long> SPublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.SPublish(channel, message), PubSubRoute);
    }

    #endregion
    #region SubscribeCommands

    /// <inheritdoc/>
    public async Task SSubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SSubscribe([channel]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task SSubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SSubscribe(channels.ToGlideStrings()), PubSubRoute);
    }

    #endregion
    #region UnsubscribeCommands

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe([]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe([channel]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe(channels.ToGlideStrings()), PubSubRoute);
    }

    #endregion
    #region PubSubInfoCommands

    /// <inheritdoc/>
    public async Task<string[]> PubSubShardChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubShardChannels(), PubSubInfoRoute);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubShardChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubShardChannels(pattern), PubSubInfoRoute);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, long>> PubSubShardNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubShardNumSub(channels.ToGlideStrings()), PubSubInfoRoute);
    }

    #endregion
}
