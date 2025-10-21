// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
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

    /// <summary>
    /// Lists all active channels.
    /// </summary>
    /// <returns>Command that returns an array of active channel names.</returns>
    public static Cmd<object[], string[]> PubSubChannels()
        => new(RequestType.PubSubChannels, [], false, objects => objects.Cast<GlideString>().Select(gs => gs.ToString()).ToArray());

    /// <summary>
    /// Lists active channels matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match channel names against.</param>
    /// <returns>Command that returns an array of matching channel names.</returns>
    public static Cmd<object[], string[]> PubSubChannels(GlideString pattern)
        => new(RequestType.PubSubChannels, [pattern], false, objects => objects.Cast<GlideString>().Select(gs => gs.ToString()).ToArray());

    /// <summary>
    /// Returns the number of subscribers for the specified channels.
    /// </summary>
    /// <param name="channels">The channels to query.</param>
    /// <returns>Command that returns a dictionary mapping channel names to subscriber counts.</returns>
    public static Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> PubSubNumSub(GlideString[] channels)
    {
        return new(RequestType.PubSubNumSub, channels, false, dict =>
        {
            Dictionary<string, long> result = new();
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
        => new(RequestType.PubSubShardChannels, [], false, objects => objects.Cast<GlideString>().Select(gs => gs.ToString()).ToArray());

    /// <summary>
    /// Lists active sharded channels matching the specified pattern (cluster mode).
    /// </summary>
    /// <param name="pattern">The pattern to match channel names against.</param>
    /// <returns>Command that returns an array of matching sharded channel names.</returns>
    public static Cmd<object[], string[]> PubSubShardChannels(GlideString pattern)
        => new(RequestType.PubSubShardChannels, [pattern], false, objects => objects.Cast<GlideString>().Select(gs => gs.ToString()).ToArray());

    /// <summary>
    /// Returns the number of subscribers for the specified sharded channels (cluster mode).
    /// </summary>
    /// <param name="channels">The sharded channels to query.</param>
    /// <returns>Command that returns a dictionary mapping sharded channel names to subscriber counts.</returns>
    public static Cmd<Dictionary<GlideString, object>, Dictionary<string, long>> PubSubShardNumSub(GlideString[] channels)
    {
        return new(RequestType.PubSubShardNumSub, channels, false, dict =>
        {
            Dictionary<string, long> result = new();
            foreach (var kvp in dict)
            {
                string channel = kvp.Key.ToString();
                long count = Convert.ToInt64(kvp.Value);
                result[channel] = count;
            }
            return result;
        });
    }
}
