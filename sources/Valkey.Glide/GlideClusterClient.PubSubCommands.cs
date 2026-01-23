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
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Subscribe([channel]));
    }

    /// <inheritdoc/>
    public async Task SubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Subscribe(ToGlideStrings(channels)));
    }

    /// <inheritdoc/>
    public async Task PSubscribeAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PSubscribe([pattern]));
    }

    /// <inheritdoc/>
    public async Task PSubscribeAsync(string[] patterns, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PSubscribe(ToGlideStrings(patterns)));
    }

    /// <inheritdoc/>
    public async Task SSubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SSubscribe([channel]), Route.Random);
    }

    /// <inheritdoc/>
    public async Task SSubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SSubscribe(ToGlideStrings(channels)), Route.Random);
    }

    #endregion
    #region UnsubscribeCommands

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe());
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe([channel]));
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe(ToGlideStrings(channels)));
    }

    /// <inheritdoc/>
    public async Task PUnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe());
    }

    /// <inheritdoc/>
    public async Task PUnsubscribeAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe([(GlideString)pattern]));
    }

    /// <inheritdoc/>
    public async Task PUnsubscribeAsync(string[] patterns, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe(ToGlideStrings(patterns)));
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe(), Route.Random);
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe([(GlideString)channel]), Route.Random);
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe(ToGlideStrings(channels)), Route.Random);
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
        // In cluster mode, route to all primaries to aggregate subscriber counts
        return await Command(Request.PubSubNumSub(ToGlideStrings(channels)), Route.AllPrimaries);
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
        // In cluster mode, route to all primaries to aggregate shard subscriber counts
        return await Command(Request.PubSubShardNumSub(ToGlideStrings(channels)), Route.AllPrimaries);
    }

    #endregion

    /// <summary>
    /// Converts the given <c>string[]</c> to a <c>GlideString[]</c>.
    /// </summary>
    /// <param name="values">The <c>string[]</c> to convert.</param>
    /// <returns>A <c>GlideString[]</c> containing the converted values.</returns>
    private static GlideString[] ToGlideStrings(string[] values)
    {
        return [.. values.Select(v => (GlideString)v)];
    }
}
