// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IPubSubCommands
{
    /// <summary>Maps from the channel mode strings returned by GLIDE core to the corresponding PubSubChannelMode enum value.</summary>
    private static readonly Dictionary<string, PubSubChannelMode> ChannelModeMap = new()
    {
        { "Exact", PubSubChannelMode.Exact },
        { "Pattern", PubSubChannelMode.Pattern },
        { "Sharded", PubSubChannelMode.Sharded }
    };

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
        var (desiredResponse, actualResponse) = await Command(Request.GetSubscriptions());

        var desired = BuildPubSubSubscriptionsMap(desiredResponse);
        var actual = BuildPubSubSubscriptionsMap(actualResponse);

        return new PubSubState(desired, actual);
    }

    #endregion

    /// <summary>
    /// Builds a pub/sub subscriptions map from the given response dictionary returned by GLIDE core.
    /// </summary>
    private static Dictionary<PubSubChannelMode, IReadOnlySet<string>> BuildPubSubSubscriptionsMap(Dictionary<string, string[]> response)
    {
        var subscriptionsMap = new Dictionary<PubSubChannelMode, IReadOnlySet<string>>();

        // Populate with empty sets for each channel mode.
        foreach (var mode in Enum.GetValues<PubSubChannelMode>())
            subscriptionsMap[mode] = new HashSet<string>();

        foreach (var entry in response)
        {
            if (!ChannelModeMap.TryGetValue(entry.Key, out var mode))
                throw new ArgumentException($"Unexpected channel mode '{entry.Key}' returned by GLIDE core.");

            subscriptionsMap[mode] = entry.Value.ToHashSet();
        }

        return subscriptionsMap;
    }
}
