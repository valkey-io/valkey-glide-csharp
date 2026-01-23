// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient : IPubSubClusterCommands
{
    #region PublishCommands

    /// <inheritdoc/>
    public async Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.Publish(channel, message), Route.Random);
    }

    /// <inheritdoc/>
    public async Task<long> SPublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.SPublish(channel, message), Route.Random);
    }

    #endregion
    #region SubscribeCommands

    /// <inheritdoc/>
    public async Task SubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        await SubscribeAsync([channel], flags);
    }

    // TODO #193: Implement SubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task SubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    // TODO #193: Implement UnsubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task UnsubscribeAsync(CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        await UnsubscribeAsync([channel], flags);
    }

    // TODO #193: Implement UnsubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task UnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task PSubscribeAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        await PSubscribeAsync([pattern], flags);
    }

    // TODO #193: Implement PSubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task PSubscribeAsync(string[] patterns, CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    // TODO #193: Implement PUnsubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task PUnsubscribeAsync(CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task PUnsubscribeAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        await PUnsubscribeAsync([pattern], flags);
    }

    // TODO #193: Implement PUnsubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task PUnsubscribeAsync(string[] patterns, CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task SSubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        await SSubscribeAsync([channel], flags);
    }

    // TODO #193: Implement SSubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task SSubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    // TODO #193: Implement SUnsubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task SUnsubscribeAsync(CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        await SUnsubscribeAsync([channel], flags);
    }

    // TODO #193: Implement SUnsubscribeAsync
    /// <inheritdoc/>
#pragma warning disable CS1998
    public async Task SUnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
#pragma warning restore CS1998
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        throw new NotImplementedException();
    }

    #endregion
    #region PubSubInfoCommands

    /// <inheritdoc/>
    public async Task<string[]> PubSubChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        // In cluster mode, route to all primaries to get complete channel list
        return await Command(Request.PubSubChannels(), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        // In cluster mode, route to all primaries to get complete channel list
        return await Command(Request.PubSubChannels(pattern), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, long>> PubSubNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GlideString[] channelArgs = [.. channels.Select(c => (GlideString)c)];
        // In cluster mode, route to all primaries to aggregate subscriber counts
        return await Command(Request.PubSubNumSub(channelArgs), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<long> PubSubNumPatAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        // In cluster mode, route to all primaries to aggregate pattern counts
        return await Command(Request.PubSubNumPat(), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubShardChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        // In cluster mode, route to all primaries to get complete shard channel list
        return await Command(Request.PubSubShardChannels(), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubShardChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        // In cluster mode, route to all primaries to get complete shard channel list
        return await Command(Request.PubSubShardChannels(pattern), Route.AllPrimaries);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, long>> PubSubShardNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GlideString[] channelArgs = [.. channels.Select(c => (GlideString)c)];
        // In cluster mode, route to all primaries to aggregate shard subscriber counts
        return await Command(Request.PubSubShardNumSub(channelArgs), Route.AllPrimaries);
    }

    #endregion
}
