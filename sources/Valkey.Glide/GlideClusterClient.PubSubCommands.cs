// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient
{
    /// <inheritdoc/>
    /// Route cluster pub/sub commands to a random shard.
    protected sealed override Route? PubSubRoute => Route.Random;

    /// <inheritdoc/>
    /// Route cluster pub/sub info commands to all primaries to produce aggregated results.
    protected sealed override Route? PubSubInfoRoute => Route.AllPrimaries;

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
        await Command(Request.SSubscribe(ToGlideStrings(channels)), PubSubRoute);
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
        await Command(Request.SUnsubscribe([(GlideString)channel]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task SUnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.SUnsubscribe(ToGlideStrings(channels)), PubSubRoute);
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
        return await Command(Request.PubSubShardNumSub(ToGlideStrings(channels)), PubSubInfoRoute);
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
