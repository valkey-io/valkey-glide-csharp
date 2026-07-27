// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    #region Command Builders

    public static Cmd<object[], (Dictionary<string, IReadOnlySet<ValkeyKey>>, Dictionary<string, IReadOnlySet<ValkeyKey>>)> GetSubscriptions()
        => new(RequestType.GetSubscriptions, [], false, objects =>
            {
                // Parse desired and actual pub/sub subscription dictionaries from the response.
                var desiredDict = ConvertGetSubscriptionsResponse((Dictionary<GlideString, object>)objects[1]);
                var actualDict = ConvertGetSubscriptionsResponse((Dictionary<GlideString, object>)objects[3]);

                return (desiredDict, actualDict);
            });

    public static Cmd<object, object> PSubscribe(GlideString[] patterns)
        => Simple<object>(RequestType.PSubscribe, patterns, isNullable: true);

    public static Cmd<object, object> PSubscribeBlocking(GlideString[] patterns, TimeSpan timeout)
        => Simple<object>(RequestType.PSubscribeBlocking, [.. patterns, ToMilliseconds(timeout).ToGlideString()], isNullable: true);

    public static Cmd<object[], ISet<ValkeyKey>> PubSubChannels()
        => new(RequestType.PubSubChannels, [], false, ToValkeyKeySet);

    public static Cmd<object[], ISet<ValkeyKey>> PubSubChannels(GlideString pattern)
        => new(RequestType.PubSubChannels, [pattern], false, ToValkeyKeySet);

    public static Cmd<long, long> PubSubNumPat()
        => Simple<long>(RequestType.PubSubNumPat, []);

    public static Cmd<Dictionary<GlideString, object>, Dictionary<ValkeyKey, long>> PubSubNumSub(GlideString[] channels)
        => new(RequestType.PubSubNumSub, channels, false, ToValkeyKeyLongDict);

    public static Cmd<object[], ISet<ValkeyKey>> PubSubShardChannels()
        => new(RequestType.PubSubShardChannels, [], false, ToValkeyKeySet);

    public static Cmd<object[], ISet<ValkeyKey>> PubSubShardChannels(GlideString pattern)
        => new(RequestType.PubSubShardChannels, [pattern], false, ToValkeyKeySet);

    public static Cmd<Dictionary<GlideString, object>, Dictionary<ValkeyKey, long>> PubSubShardNumSub(GlideString[] channels)
        => new(RequestType.PubSubShardNumSub, channels, false, ToValkeyKeyLongDict);

    public static Cmd<long, long> Publish(GlideString channel, GlideString message)
        => Simple<long>(RequestType.Publish, [channel, message]);

    public static Cmd<object, object> PUnsubscribe(GlideString[] patterns)
        => Simple<object>(RequestType.PUnsubscribe, patterns, isNullable: true);

    public static Cmd<object, object> PUnsubscribeBlocking(GlideString[] patterns, TimeSpan timeout)
        => Simple<object>(RequestType.PUnsubscribeBlocking, [.. patterns, ToMilliseconds(timeout).ToGlideString()], isNullable: true);

    public static Cmd<long, long> SPublish(GlideString channel, GlideString message)
        => Simple<long>(RequestType.SPublish, [channel, message]);

    public static Cmd<object, object> SSubscribe(GlideString[] channels)
        => Simple<object>(RequestType.SSubscribe, channels, isNullable: true);

    public static Cmd<object, object> SSubscribeBlocking(GlideString[] channels, TimeSpan timeout)
        => Simple<object>(RequestType.SSubscribeBlocking, [.. channels, ToMilliseconds(timeout).ToGlideString()], isNullable: true);

    public static Cmd<object, object> Subscribe(GlideString[] channels)
        => Simple<object>(RequestType.Subscribe, channels, isNullable: true);

    public static Cmd<object, object> SubscribeBlocking(GlideString[] channels, TimeSpan timeout)
        => Simple<object>(RequestType.SubscribeBlocking, [.. channels, ToMilliseconds(timeout).ToGlideString()], isNullable: true);

    public static Cmd<object, object> SUnsubscribe(GlideString[] channels)
        => Simple<object>(RequestType.SUnsubscribe, channels, isNullable: true);

    public static Cmd<object, object> SUnsubscribeBlocking(GlideString[] channels, TimeSpan timeout)
        => Simple<object>(RequestType.SUnsubscribeBlocking, [.. channels, ToMilliseconds(timeout).ToGlideString()], isNullable: true);

    public static Cmd<object, object> Unsubscribe(GlideString[] channels)
        => Simple<object>(RequestType.Unsubscribe, channels, isNullable: true);

    public static Cmd<object, object> UnsubscribeBlocking(GlideString[] channels, TimeSpan timeout)
        => Simple<object>(RequestType.UnsubscribeBlocking, [.. channels, ToMilliseconds(timeout).ToGlideString()], isNullable: true);

    #endregion
    #region Response Converters

    /// <summary>
    /// Parses and returns a dictionary from the given <see cref="GetSubscriptions"/> response dictionary.
    /// </summary>
    private static Dictionary<string, IReadOnlySet<ValkeyKey>> ConvertGetSubscriptionsResponse(Dictionary<GlideString, object> response)
    {
        Dictionary<string, IReadOnlySet<ValkeyKey>> resultDict = [];

        foreach (var entry in response)
        {
            string channelMode = entry.Key.ToString();
            IReadOnlySet<ValkeyKey> channels = ((object[])entry.Value).Cast<GlideString>().Select(gs => (ValkeyKey)gs.Bytes).ToHashSet();

            resultDict[channelMode] = channels;
        }

        return resultDict;
    }

    #endregion
}
