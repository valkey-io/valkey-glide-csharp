// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IPubSubCommands
{
    /// <summary>
    /// Route to use for pub/sub commands (e.g. subscribe, publish).
    /// </summary>
    protected virtual Route? PubSubRoute { get; } = null;

    /// <summary>
    /// Route to use for pub/sub info commands (e.g. pubsub channels, numsub).
    /// </summary>
    protected virtual Route? PubSubInfoRoute { get; } = null;

    #region PublishCommands

    /// <inheritdoc/>
    public async Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.Publish(channel, message), PubSubRoute);
    }

    #endregion
    #region SubscribeCommands

    /// <inheritdoc/>
    public async Task SubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Subscribe([channel]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task SubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Subscribe(channels.ToGlideStrings()), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task PSubscribeAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PSubscribe([pattern]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task PSubscribeAsync(string[] patterns, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PSubscribe(patterns.ToGlideStrings()), PubSubRoute);
    }

    #endregion
    #region UnsubscribeCommands

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe([]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe([channel]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task UnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe(channels.ToGlideStrings()), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task PUnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe([]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task PUnsubscribeAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe([pattern]), PubSubRoute);
    }

    /// <inheritdoc/>
    public async Task PUnsubscribeAsync(string[] patterns, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe(patterns.ToGlideStrings()), PubSubRoute);
    }

    #endregion
    #region PubSubInfoCommands

    /// <inheritdoc/>
    public async Task<string[]> PubSubChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubChannels(), PubSubInfoRoute);
    }

    /// <inheritdoc/>
    public async Task<string[]> PubSubChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubChannels(pattern), PubSubInfoRoute);
    }

    /// <inheritdoc/>
    public async Task<Dictionary<string, long>> PubSubNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubNumSub(channels.ToGlideStrings()), PubSubInfoRoute);
    }

    /// <inheritdoc/>
    public async Task<long> PubSubNumPatAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubNumPat(), PubSubInfoRoute);
    }

    #endregion
}
