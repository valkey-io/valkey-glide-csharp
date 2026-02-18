// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    #region PublishCommands

    /// <summary>
    /// Publishes a message to the specified channel.
    /// </summary>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <returns>Command that returns the number of clients that received the message.</returns>
    public static Cmd<long, long> Publish(GlideString channel, GlideString message)
        => Simple<long>(RequestType.Publish, [channel, message]);

    /// <summary>
    /// Publishes a message to the specified sharded channel (cluster mode).
    /// </summary>
    /// <param name="channel">The sharded channel to publish to.</param>
    /// <param name="message">The message to publish.</param>
    /// <returns>Command that returns the number of clients that received the message.</returns>
    public static Cmd<long, long> SPublish(GlideString channel, GlideString message)
        => Simple<long>(RequestType.SPublish, [channel, message]);

    #endregion
    #region SubscribeCommands

    /// <summary>
    /// Subscribes to the specified channels and returns without waiting for server confirmation.
    /// </summary>
    /// <param name="channels">The channels to subscribe to.</param>
    /// <returns>Command for subscribing to channels.</returns>
    public static Cmd<object, object> Subscribe(GlideString[] channels)
        => Simple<object>(RequestType.Subscribe, channels, isNullable: true);

    /// <summary>
    /// Subscribes to the specified channels and waits for server confirmation.
    /// </summary>
    /// <param name="channels">The channels to subscribe to.</param>
    /// <param name="timeoutMs">Timeout in milliseconds.</param>
    /// <returns>Command for subscribing to channels with blocking confirmation.</returns>
    public static Cmd<object, object> SubscribeBlocking(GlideString[] channels, uint timeoutMs)
        => Simple<object>(RequestType.SubscribeBlocking, [.. channels, timeoutMs.ToString()], isNullable: true);

    /// <summary>
    /// Subscribes to the specified patterns and returns without waiting for server confirmation.
    /// </summary>
    /// <param name="patterns">The patterns to subscribe to.</param>
    /// <returns>Command for subscribing to patterns.</returns>
    public static Cmd<object, object> PSubscribe(GlideString[] patterns)
        => Simple<object>(RequestType.PSubscribe, patterns, isNullable: true);

    /// <summary>
    /// Subscribes to the specified patterns and waits for server confirmation.
    /// </summary>
    /// <param name="patterns">The patterns to subscribe to.</param>
    /// <param name="timeoutMs">Timeout in milliseconds.</param>
    /// <returns>Command for subscribing to patterns with blocking confirmation.</returns>
    public static Cmd<object, object> PSubscribeBlocking(GlideString[] patterns, uint timeoutMs)
        => Simple<object>(RequestType.PSubscribeBlocking, [.. patterns, timeoutMs.ToString()], isNullable: true);

    /// <summary>
    /// Subscribes to the specified sharded channels and returns without waiting for server confirmation.
    /// </summary>
    /// <param name="channels">The sharded channels to subscribe to.</param>
    /// <returns>Command for subscribing to sharded channels.</returns>
    public static Cmd<object, object> SSubscribe(GlideString[] channels)
        => Simple<object>(RequestType.SSubscribe, channels, isNullable: true);

    /// <summary>
    /// Subscribes to the specified sharded channels and waits for server confirmation.
    /// </summary>
    /// <param name="channels">The sharded channels to subscribe to.</param>
    /// <param name="timeoutMs">Timeout in milliseconds.</param>
    /// <returns>Command for subscribing to sharded channels with blocking confirmation.</returns>
    public static Cmd<object, object> SSubscribeBlocking(GlideString[] channels, uint timeoutMs)
        => Simple<object>(RequestType.SSubscribeBlocking, [.. channels, timeoutMs.ToString()], isNullable: true);

    #endregion
    #region UnsubscribeCommands

    /// <summary>
    /// Unsubscribes from the specified channels and returns without waiting for server confirmation.
    /// If no channels are specified, unsubscribes from all channels.
    /// </summary>
    /// <param name="channels">The channels to unsubscribe from.</param>
    /// <returns>Command for unsubscribing from channels.</returns>
    public static Cmd<object, object> Unsubscribe(GlideString[] channels)
        => Simple<object>(RequestType.Unsubscribe, channels, isNullable: true);

    /// <summary>
    /// Unsubscribes from the specified channels and waits for server confirmation.
    /// If no channels are specified, unsubscribes from all channels.
    /// </summary>
    /// <param name="channels">The channels to unsubscribe from.</param>
    /// <param name="timeoutMs">Timeout in milliseconds.</param>
    /// <returns>Command for unsubscribing from channels.</returns>
    public static Cmd<object, object> UnsubscribeBlocking(GlideString[] channels, uint timeoutMs)
        => Simple<object>(RequestType.UnsubscribeBlocking, [.. channels, timeoutMs.ToString()], isNullable: true);

    /// <summary>
    /// Unsubscribes from the specified patterns and returns without waiting for server confirmation.
    /// If no patterns are specified, unsubscribes from all patterns.
    /// </summary>
    /// <param name="patterns">The patterns to unsubscribe from.</param>
    /// <returns>Command for unsubscribing from patterns.</returns>
    public static Cmd<object, object> PUnsubscribe(GlideString[] patterns)
        => Simple<object>(RequestType.PUnsubscribe, patterns, isNullable: true);

    /// <summary>
    /// Unsubscribes from the specified patterns and waits for server confirmation.
    /// If no patterns are specified, unsubscribes from all patterns.
    /// </summary>
    /// <param name="patterns">The patterns to unsubscribe from.</param>
    /// <param name="timeoutMs">Timeout in milliseconds.</param>
    /// <returns>Command for unsubscribing from patterns.</returns>
    public static Cmd<object, object> PUnsubscribeBlocking(GlideString[] patterns, uint timeoutMs)
        => Simple<object>(RequestType.PUnsubscribeBlocking, [.. patterns, timeoutMs.ToString()], isNullable: true);

    /// <summary>
    /// Unsubscribes from the specified sharded channels and returns without waiting for server confirmation.
    /// If no sharded channels are specified, unsubscribes from all sharded channels.
    /// </summary>
    /// <param name="channels">The sharded channels to unsubscribe from.</param>
    /// <returns>Command for unsubscribing from sharded channels.</returns>
    public static Cmd<object, object> SUnsubscribe(GlideString[] channels)
        => Simple<object>(RequestType.SUnsubscribe, channels, isNullable: true);

    /// <summary>
    /// Unsubscribes from the specified sharded channels and waits for server confirmation.
    /// If no sharded channels are specified, unsubscribes from all sharded channels.
    /// </summary>
    /// <param name="channels">The sharded channels to unsubscribe from.</param>
    /// <param name="timeoutMs">Timeout in milliseconds.</param>
    /// <returns>Command for unsubscribing from sharded channels.</returns>
    public static Cmd<object, object> SUnsubscribeBlocking(GlideString[] channels, uint timeoutMs)
        => Simple<object>(RequestType.SUnsubscribeBlocking, [.. channels, timeoutMs.ToString()], isNullable: true);

    #endregion
    #region IntrospectionCommands

    /// <summary>
    /// Lists all active channels.
    /// </summary>
    /// <returns>Command that returns a set of active channel names.</returns>
    public static Cmd<object[], ISet<string>> PubSubChannels()
        => new(RequestType.PubSubChannels, [], false, ToStringSet);

    /// <summary>
    /// Lists active channels matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match channel names against.</param>
    /// <returns>Command that returns a set of matching channel names.</returns>
    public static Cmd<object[], ISet<string>> PubSubChannels(GlideString pattern)
        => new(RequestType.PubSubChannels, [pattern], false, ToStringSet);

    /// <summary>
    /// Lists the number of subscribers for the specified channels.
    /// </summary>
    /// <param name="channels">The channels to query.</param>
    /// <returns>Command that returns a dictionary mapping channel names to subscriber counts.</returns>
    public static Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> PubSubNumSub(GlideString[] channels)
    {
        return new(RequestType.PubSubNumSub, channels, false, dict =>
        {
            Dictionary<string, long> result = [];
            foreach (var kvp in dict)
            {
                string channel = kvp.Key.ToString();
                long count = Convert.ToInt64(kvp.Value);
                result[channel] = count;
            }
            return result;
        });
    }

    /// <summary>
    /// Gets the number of active pattern subscriptions.
    /// </summary>
    /// <returns>Command that returns the number of pattern subscriptions.</returns>
    public static Cmd<long, long> PubSubNumPat()
        => Simple<long>(RequestType.PubSubNumPat, []);

    /// <summary>
    /// Lists all active sharded channels (cluster mode).
    /// </summary>
    /// <returns>Command that returns a set of active sharded channel names.</returns>
    public static Cmd<object[], ISet<string>> PubSubShardChannels()
        => new(RequestType.PubSubShardChannels, [], false, ToStringSet);

    /// <summary>
    /// Lists active sharded channels matching the specified pattern (cluster mode).
    /// </summary>
    /// <param name="pattern">The pattern to match channel names against.</param>
    /// <returns>Command that returns a set of matching sharded channel names.</returns>
    public static Cmd<object[], ISet<string>> PubSubShardChannels(GlideString pattern)
        => new(RequestType.PubSubShardChannels, [pattern], false, ToStringSet);

    /// <summary>
    /// Lists the number of subscribers for the specified sharded channels (cluster mode).
    /// </summary>
    /// <param name="channels">The sharded channels to query.</param>
    /// <returns>Command that returns a dictionary mapping sharded channel names to subscriber counts.</returns>
    public static Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> PubSubShardNumSub(GlideString[] channels)
    {
        return new(RequestType.PubSubShardNumSub, channels, false, dict =>
        {
            Dictionary<string, long> result = [];
            foreach (var kvp in dict)
            {
                string channel = kvp.Key.ToString();
                long count = Convert.ToInt64(kvp.Value);
                result[channel] = count;
            }
            return result;
        });
    }

    /// <summary>
    /// Gets the current pub/sub subscription state.
    /// </summary>
    /// <returns>Command that returns a tuple of desired and actual pub/sub subscription dictionaries.</returns>
    public static Cmd<object[], (Dictionary<string, IReadOnlySet<string>>, Dictionary<string, IReadOnlySet<string>>)> GetSubscriptions()
    {
        return new(RequestType.GetSubscriptions, [], false, objects =>
        {
            // Parse desired and actual pub/sub subscription dictionaries from the response.
            var desiredDict = ParseGetSubscriptionsResponse((Dictionary<GlideString, object>)objects[1]);
            var actualDict = ParseGetSubscriptionsResponse((Dictionary<GlideString, object>)objects[3]);

            return (desiredDict, actualDict);
        });
    }

    #endregion

    /// <summary>
    /// Parses and returns a dictionary from the given <see cref="GetSubscriptions"/> response dictionary.
    /// </summary>
    private static Dictionary<string, IReadOnlySet<string>> ParseGetSubscriptionsResponse(Dictionary<GlideString, object> response)
    {
        Dictionary<string, IReadOnlySet<string>> resultDict = [];

        foreach (var entry in response)
        {
            string channelMode = entry.Key.ToString();
            IReadOnlySet<string> channels = ((object[])entry.Value).Cast<GlideString>().Select(gs => gs.ToString()).ToHashSet();

            resultDict[channelMode] = channels;
        }

        return resultDict;
    }
}
