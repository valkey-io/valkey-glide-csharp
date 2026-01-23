// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClient : IPubSubStandaloneCommands
{
    #region PublishCommands

    /// <inheritdoc/>
    public async Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.Publish(channel, message));
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

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        await UnsubscribeAsync([], flags);
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

    #endregion
    #region PubSubInfoCommands

    /// <inheritdoc/>
    public async Task<string[]> PubSubChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubChannels());
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubChannels(pattern));
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, long>> PubSubNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GlideString[] channelArgs = [.. channels.Select(c => (GlideString)c)];
        return await Command(Request.PubSubNumSub(channelArgs));
    }

    /// <inheritdoc/>
    public async Task<long> PubSubNumPatAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubNumPat());
    }

    #endregion
}
