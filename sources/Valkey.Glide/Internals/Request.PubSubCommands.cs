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
    /// Subscribes to the specified channels.
    /// </summary>
    /// <param name="channels">The channels to subscribe to.</param>
    /// <returns>Command for subscribing to channels.</returns>
    public static Cmd<object, object> Subscribe(GlideString[] channels)
        => Simple<object>(RequestType.Subscribe, channels);

    /// <summary>
    /// Subscribes to the specified patterns.
    /// </summary>
    /// <param name="patterns">The patterns to subscribe to.</param>
    /// <returns>Command for subscribing to patterns.</returns>
    public static Cmd<object, object> PSubscribe(GlideString[] patterns)
        => Simple<object>(RequestType.PSubscribe, patterns);

    /// <summary>
    /// Subscribes to the specified sharded channels.
    /// </summary>
    /// <param name="channels">The sharded channels to subscribe to.</param>
    /// <returns>Command for subscribing to sharded channels.</returns>
    public static Cmd<object, object> SSubscribe(GlideString[] channels)
        => Simple<object>(RequestType.SSubscribe, channels);

    #endregion
    #region UnsubscribeCommands

    /// <summary>
    /// Unsubscribes from the specified channels.
    /// If no channels are specified, unsubscribes from all channels.
    /// </summary>
    /// <param name="channels">The channels to unsubscribe from.</param>
    /// <returns>Command for unsubscribing from channels.</returns>
    public static Cmd<object, object> Unsubscribe(GlideString[] channels)
        => Simple<object>(RequestType.Unsubscribe, channels);

    /// <summary>
    /// Unsubscribes from the specified patterns.
    /// If no patterns are specified, unsubscribes from all patterns.
    /// </summary>
    /// <param name="patterns">The patterns to unsubscribe from.</param>
    /// <returns>Command for unsubscribing from patterns.</returns>
    public static Cmd<object, object> PUnsubscribe(GlideString[] patterns)
        => Simple<object>(RequestType.PUnsubscribe, patterns);

    /// <summary>
    /// Unsubscribes from the specified sharded channels.
    /// If no sharded channels are specified, unsubscribes from all sharded channels.
    /// </summary>
    /// <param name="channels">The sharded channels to unsubscribe from.</param>
    /// <returns>Command for unsubscribing from sharded channels.</returns>
    public static Cmd<object, object> SUnsubscribe(GlideString[] channels)
        => Simple<object>(RequestType.SUnsubscribe, channels);
    #endregion
    #region PubSubInfoCommands

    /// <summary>
    /// Lists all active channels.
    /// </summary>
    /// <returns>Command that returns an array of active channel names.</returns>
    public static Cmd<object[], string[]> PubSubChannels()
        => new(RequestType.PubSubChannels, [], false, objects => [.. objects.Cast<GlideString>().Select(gs => gs.ToString())]);

    /// <summary>
    /// Lists active channels matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match channel names against.</param>
    /// <returns>Command that returns an array of matching channel names.</returns>
    public static Cmd<object[], string[]> PubSubChannels(GlideString pattern)
        => new(RequestType.PubSubChannels, [pattern], false, objects => [.. objects.Cast<GlideString>().Select(gs => gs.ToString())]);

    /// <summary>
    /// Returns the number of subscribers for the specified channels.
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
    /// Returns the number of active pattern subscriptions.
    /// </summary>
    /// <returns>Command that returns the number of pattern subscriptions.</returns>
    public static Cmd<long, long> PubSubNumPat()
        => Simple<long>(RequestType.PubSubNumPat, []);

    /// <summary>
    /// Lists all active sharded channels (cluster mode).
    /// </summary>
    /// <returns>Command that returns an array of active sharded channel names.</returns>
    public static Cmd<object[], string[]> PubSubShardChannels()
        => new(RequestType.PubSubShardChannels, [], false, objects => [.. objects.Cast<GlideString>().Select(gs => gs.ToString())]);

    /// <summary>
    /// Lists active sharded channels matching the specified pattern (cluster mode).
    /// </summary>
    /// <param name="pattern">The pattern to match channel names against.</param>
    /// <returns>Command that returns an array of matching sharded channel names.</returns>
    public static Cmd<object[], string[]> PubSubShardChannels(GlideString pattern)
        => new(RequestType.PubSubShardChannels, [pattern], false, objects => [.. objects.Cast<GlideString>().Select(gs => gs.ToString())]);

    /// <summary>
    /// Returns the number of subscribers for the specified sharded channels (cluster mode).
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

    #endregion
}
