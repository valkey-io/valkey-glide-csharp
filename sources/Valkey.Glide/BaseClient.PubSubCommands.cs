// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IPubSubCommands
{
    #region PublishCommands

    public async Task<long> PublishAsync(string channel, string message, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.Publish(channel, message));
    }

    #endregion
    #region SubscribeCommands

    public async Task SubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Subscribe([channel]));
    }

    public async Task SubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Subscribe(channels.ToGlideStrings()));
    }

    public async Task PSubscribeAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PSubscribe([pattern]));
    }

    public async Task PSubscribeAsync(string[] patterns, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PSubscribe(patterns.ToGlideStrings()));
    }

    #endregion
    #region UnsubscribeCommands

    public async Task UnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe([]));
    }

    public async Task UnsubscribeAsync(string channel, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe([channel]));
    }

    public async Task UnsubscribeAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.Unsubscribe(channels.ToGlideStrings()));
    }

    public async Task PUnsubscribeAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe([]));
    }

    public async Task PUnsubscribeAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe([pattern]));
    }

    public async Task PUnsubscribeAsync(string[] patterns, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.PUnsubscribe(patterns.ToGlideStrings()));
    }

    #endregion
    #region PubSubInfoCommands

    public async Task<string[]> PubSubChannelsAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubChannels());
    }

    public async Task<string[]> PubSubChannelsAsync(string pattern, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubChannels(pattern));
    }

    public async Task<Dictionary<string, long>> PubSubNumSubAsync(string[] channels, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubNumSub(channels.ToGlideStrings()));
    }

    public async Task<long> PubSubNumPatAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.PubSubNumPat());
    }

    public async Task<PubSubState> GetSubscriptionsAsync()
    {
        var response = await Command(Request.GetSubscriptions());
        return ParsePubSubState(response);
    }

    #endregion

    private static PubSubState ParsePubSubState(object[] response)
    {
        var desiredSubscriptions = ParseSubscriptionsMap((Dictionary<GlideString, object>)response[0]);
        var actualSubscriptions = ParseSubscriptionsMap((Dictionary<GlideString, object>)response[1]);

        return new PubSubState(desiredSubscriptions, actualSubscriptions);
    }

    /// <summary>
    /// Builds and returns a pub/sub subscription map from the given response dictionary.
    /// </summary>
    private static Dictionary<PubSubChannelMode, IReadOnlySet<string>> ParseSubscriptionsMap(Dictionary<GlideString, object> responseDict)
    {
        var subscriptions = new Dictionary<PubSubChannelMode, IReadOnlySet<string>>();

        // TODO: Populate with empty sets for each channel mode.

        foreach (var entry in responseDict)
        {
            var channelMode = entry.Key.ToString();

            // The channels/patterns are returned as an array of GLIDE strings.
            var channels = ((object[])entry.Value).Cast<GlideString>().Select(gs => gs.ToString()).ToHashSet();

            // TODO: Complete mapping.
        }

        return subscriptions;
    }
}
