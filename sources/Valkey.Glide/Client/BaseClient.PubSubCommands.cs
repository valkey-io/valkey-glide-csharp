// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <summary>
    /// Maps from the channel mode strings returned by GLIDE core to the corresponding PubSubChannelMode enum value.
    /// </summary>
    private static readonly Dictionary<string, PubSubChannelMode> ChannelModeMap = new()
    {
        { "Exact", PubSubChannelMode.Exact },
        { "Pattern", PubSubChannelMode.Pattern },
        { "Sharded", PubSubChannelMode.Sharded }
    };

    #region PublishCommands

    /// <inheritdoc/>
    public async Task<long> PublishAsync(ValkeyKey channel, ValkeyValue message)
        => await Command(Request.Publish(channel, message));

    #endregion
    #region SubscribeCommands

    /// <inheritdoc cref="IBaseClient.SubscribeAsync(ValkeyKey, TimeSpan)"/>
    public async Task SubscribeAsync(ValkeyKey channel, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.SubscribeBlocking([channel], timeout));
    }

    /// <inheritdoc cref="IBaseClient.SubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    public async Task SubscribeAsync(IEnumerable<ValkeyKey> channels, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.SubscribeBlocking(channels.ToGlideStrings(), timeout));
    }

    /// <inheritdoc cref="IBaseClient.SubscribeLazyAsync(ValkeyKey)"/>
    public async Task SubscribeLazyAsync(ValkeyKey channel)
        => await Command(Request.Subscribe([channel]));

    /// <inheritdoc cref="IBaseClient.SubscribeLazyAsync(IEnumerable{ValkeyKey})"/>
    public async Task SubscribeLazyAsync(IEnumerable<ValkeyKey> channels)
        => await Command(Request.Subscribe(channels.ToGlideStrings()));

    /// <inheritdoc cref="IBaseClient.PSubscribeAsync(ValkeyKey, TimeSpan)"/>
    public async Task PSubscribeAsync(ValkeyKey pattern, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.PSubscribeBlocking([pattern], timeout));
    }

    /// <inheritdoc cref="IBaseClient.PSubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    public async Task PSubscribeAsync(IEnumerable<ValkeyKey> patterns, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.PSubscribeBlocking(patterns.ToGlideStrings(), timeout));
    }

    /// <inheritdoc cref="IBaseClient.PSubscribeLazyAsync(ValkeyKey)"/>
    public async Task PSubscribeLazyAsync(ValkeyKey pattern)
        => await Command(Request.PSubscribe([pattern]));

    /// <inheritdoc cref="IBaseClient.PSubscribeLazyAsync(IEnumerable{ValkeyKey})"/>
    public async Task PSubscribeLazyAsync(IEnumerable<ValkeyKey> patterns)
        => await Command(Request.PSubscribe(patterns.ToGlideStrings()));

    #endregion
    #region UnsubscribeCommands

    /// <inheritdoc cref="IBaseClient.UnsubscribeAsync(TimeSpan)"/>
    public async Task UnsubscribeAsync(TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.UnsubscribeBlocking([], timeout));
    }

    /// <inheritdoc cref="IBaseClient.UnsubscribeAsync(ValkeyKey, TimeSpan)"/>
    public async Task UnsubscribeAsync(ValkeyKey channel, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.UnsubscribeBlocking([channel], timeout));
    }

    /// <inheritdoc cref="IBaseClient.UnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    public async Task UnsubscribeAsync(IEnumerable<ValkeyKey> channels, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.UnsubscribeBlocking(channels.ToGlideStrings(), timeout));
    }

    /// <inheritdoc cref="IBaseClient.UnsubscribeLazyAsync()"/>
    public async Task UnsubscribeLazyAsync()
        => await Command(Request.Unsubscribe([]));

    /// <inheritdoc cref="IBaseClient.UnsubscribeLazyAsync(ValkeyKey)"/>
    public async Task UnsubscribeLazyAsync(ValkeyKey channel)
        => await Command(Request.Unsubscribe([channel]));

    /// <inheritdoc cref="IBaseClient.UnsubscribeLazyAsync(IEnumerable{ValkeyKey})"/>
    public async Task UnsubscribeLazyAsync(IEnumerable<ValkeyKey> channels)
        => await Command(Request.Unsubscribe(channels.ToGlideStrings()));

    /// <inheritdoc cref="IBaseClient.PUnsubscribeAsync(TimeSpan)"/>
    public async Task PUnsubscribeAsync(TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.PUnsubscribeBlocking([], timeout));
    }

    /// <inheritdoc cref="IBaseClient.PUnsubscribeAsync(ValkeyKey, TimeSpan)"/>
    public async Task PUnsubscribeAsync(ValkeyKey pattern, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.PUnsubscribeBlocking([pattern], timeout));
    }

    /// <inheritdoc cref="IBaseClient.PUnsubscribeAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    public async Task PUnsubscribeAsync(IEnumerable<ValkeyKey> patterns, TimeSpan timeout)
    {
        GuardClauses.ThrowIfTimeSpanNegative(timeout);
        _ = await Command(Request.PUnsubscribeBlocking(patterns.ToGlideStrings(), timeout));
    }

    /// <inheritdoc cref="IBaseClient.PUnsubscribeLazyAsync()"/>
    public async Task PUnsubscribeLazyAsync()
        => await Command(Request.PUnsubscribe([]));

    /// <inheritdoc cref="IBaseClient.PUnsubscribeLazyAsync(ValkeyKey)"/>
    public async Task PUnsubscribeLazyAsync(ValkeyKey pattern)
        => await Command(Request.PUnsubscribe([pattern]));

    /// <inheritdoc cref="IBaseClient.PUnsubscribeLazyAsync(IEnumerable{ValkeyKey})"/>
    public async Task PUnsubscribeLazyAsync(IEnumerable<ValkeyKey> patterns)
        => await Command(Request.PUnsubscribe(patterns.ToGlideStrings()));

    #endregion
    #region IntrospectionCommands

    /// <inheritdoc cref="IBaseClient.PubSubChannelsAsync()"/>
    public async Task<ISet<ValkeyKey>> PubSubChannelsAsync()
        => await Command(Request.PubSubChannels());

    /// <inheritdoc cref="IBaseClient.PubSubChannelsAsync(ValkeyKey)"/>
    public async Task<ISet<ValkeyKey>> PubSubChannelsAsync(ValkeyKey pattern)
        => await Command(Request.PubSubChannels(pattern));

    /// <inheritdoc cref="IBaseClient.PubSubNumSubAsync(ValkeyKey)"/>
    public async Task<long> PubSubNumSubAsync(ValkeyKey channel)
    {
        Dictionary<ValkeyKey, long> result = await Command(Request.PubSubNumSub([channel]));
        return result.GetValueOrDefault(channel, 0L);
    }

    /// <inheritdoc cref="IBaseClient.PubSubNumSubAsync(IEnumerable{ValkeyKey})"/>
    public async Task<Dictionary<ValkeyKey, long>> PubSubNumSubAsync(IEnumerable<ValkeyKey> channels)
        => await Command(Request.PubSubNumSub(channels.ToGlideStrings()));

    /// <inheritdoc/>
    public async Task<long> PubSubNumPatAsync()
        => await Command(Request.PubSubNumPat());

    /// <inheritdoc/>
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
    private static Dictionary<PubSubChannelMode, IReadOnlySet<ValkeyKey>> BuildPubSubSubscriptionsMap(
        Dictionary<string, IReadOnlySet<ValkeyKey>> response)
    {
        var subscriptionsMap = new Dictionary<PubSubChannelMode, IReadOnlySet<ValkeyKey>>();

        // Populate with empty sets for each channel mode.
        foreach (var mode in Enum.GetValues<PubSubChannelMode>())
        {
            subscriptionsMap[mode] = new HashSet<ValkeyKey>();
        }

        foreach (var entry in response)
        {
            if (!ChannelModeMap.TryGetValue(entry.Key, out var mode))
            {
                throw new ArgumentException($"Unexpected channel mode '{entry.Key}' returned by GLIDE core.");
            }

            subscriptionsMap[mode] = entry.Value;
        }

        return subscriptionsMap;
    }
}
