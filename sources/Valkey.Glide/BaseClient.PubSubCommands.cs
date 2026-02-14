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

    public async Task<long> PublishAsync(string channel, string message)
        => await Command(Request.Publish(channel, message));

    #endregion
    #region SubscribeCommands

    public async Task SubscribeAsync(string channel, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.SubscribeBlocking([channel], timeout.TotalMilliseconds));
    }

    public async Task SubscribeAsync(IEnumerable<string> channels, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.SubscribeBlocking(channels.ToGlideStrings(), timeout.TotalMilliseconds));
    }

    public async Task SubscribeLazyAsync(string channel)
        => await Command(Request.Subscribe([channel]));

    public async Task SubscribeLazyAsync(IEnumerable<string> channels)
        => await Command(Request.Subscribe(channels.ToGlideStrings()));

    public async Task PSubscribeAsync(string pattern, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.PSubscribeBlocking([pattern], timeout.TotalMilliseconds));
    }

    public async Task PSubscribeAsync(IEnumerable<string> patterns, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.PSubscribeBlocking(patterns.ToGlideStrings(), timeout.TotalMilliseconds));
    }

    public async Task PSubscribeLazyAsync(string pattern)
        => await Command(Request.PSubscribe([pattern]));

    public async Task PSubscribeLazyAsync(IEnumerable<string> patterns)
        => await Command(Request.PSubscribe(patterns.ToGlideStrings()));

    #endregion
    #region UnsubscribeCommands

    public async Task UnsubscribeAsync(TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.UnsubscribeBlocking([], timeout.TotalMilliseconds));
    }

    public async Task UnsubscribeAsync(string channel, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.UnsubscribeBlocking([channel], timeout.TotalMilliseconds));
    }

    public async Task UnsubscribeAsync(IEnumerable<string> channels, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.UnsubscribeBlocking(channels.ToGlideStrings(), timeout.TotalMilliseconds));
    }

    public async Task UnsubscribeLazyAsync()
        => await Command(Request.Unsubscribe([]));

    public async Task UnsubscribeLazyAsync(string channel)
        => await Command(Request.Unsubscribe([channel]));

    public async Task UnsubscribeLazyAsync(IEnumerable<string> channels)
        => await Command(Request.Unsubscribe(channels.ToGlideStrings()));

    public async Task PUnsubscribeAsync(TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.PUnsubscribeBlocking([], timeout.TotalMilliseconds));
    }

    public async Task PUnsubscribeAsync(string pattern, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.PUnsubscribeBlocking([pattern], timeout.TotalMilliseconds));
    }

    public async Task PUnsubscribeAsync(IEnumerable<string> patterns, TimeSpan timeout = default)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        await Command(Request.PUnsubscribeBlocking(patterns.ToGlideStrings(), timeout.TotalMilliseconds));
    }

    public async Task PUnsubscribeLazyAsync()
        => await Command(Request.PUnsubscribe([]));

    public async Task PUnsubscribeLazyAsync(string pattern)
        => await Command(Request.PUnsubscribe([pattern]));

    public async Task PUnsubscribeLazyAsync(IEnumerable<string> patterns)
        => await Command(Request.PUnsubscribe(patterns.ToGlideStrings()));

    #endregion
    #region IntrospectionCommands

    public async Task<ISet<string>> PubSubChannelsAsync()
        => (await Command(Request.PubSubChannels())).ToHashSet();

    public async Task<ISet<string>> PubSubChannelsAsync(string pattern)
        => await Command(Request.PubSubChannels(pattern));

    public async Task<Dictionary<string, long>> PubSubNumSubAsync(IEnumerable<string> channels)
        => await Command(Request.PubSubNumSub(channels.ToGlideStrings()));

    public async Task<long> PubSubNumPatAsync()
        => await Command(Request.PubSubNumPat());

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
    private static Dictionary<PubSubChannelMode, IReadOnlySet<string>> BuildPubSubSubscriptionsMap(Dictionary<string, IReadOnlySet<string>> response)
    {
        var subscriptionsMap = new Dictionary<PubSubChannelMode, IReadOnlySet<string>>();

        // Populate with empty sets for each channel mode.
        foreach (var mode in Enum.GetValues<PubSubChannelMode>())
            subscriptionsMap[mode] = new HashSet<string>();

        foreach (var entry in response)
        {
            if (!ChannelModeMap.TryGetValue(entry.Key, out var mode))
                throw new ArgumentException($"Unexpected channel mode '{entry.Key}' returned by GLIDE core.");

            subscriptionsMap[mode] = entry.Value;
        }

        return subscriptionsMap;
    }
}
