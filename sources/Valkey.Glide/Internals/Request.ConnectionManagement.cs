// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Route;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    #region Constants

    private static readonly IReadOnlySet<string> EmptyStringSet = new HashSet<string>();

    #endregion
    #region Command Builders

    public static Cmd<GlideString, ValkeyValue> ClientGetName()
        => ToValkeyValue(RequestType.ClientGetName, [], isNullable: true);

    public static Cmd<object, ClusterValue<ValkeyValue>> ClientGetName(Route route)
        => ClientGetName().ToClusterValue(route);

    public static Cmd<long, long> ClientId()
        => Simple<long>(RequestType.ClientId, []);

    public static Cmd<string, ValkeyValue> ClientSetName(string name)
        => Ok(RequestType.ClientSetName, [name]);

    public static Cmd<string, ValkeyValue> ClientPause(TimeSpan timeout)
        => Ok(RequestType.ClientPause, [ToMilliseconds(timeout)]);

    public static Cmd<string, ValkeyValue> ClientPauseWrite(TimeSpan timeout)
        => Ok(RequestType.ClientPause, [ToMilliseconds(timeout), ValkeyLiterals.WRITE]);

    public static Cmd<string, ValkeyValue> ClientUnpause()
        => Ok(RequestType.ClientUnpause);

    public static Cmd<Dictionary<GlideString, object>, ClientTrackingInfo> ClientTrackingInfo()
        => new(RequestType.ClientTrackingInfo, [], false, ConvertClientTrackingInfoResponse);

    #endregion
    #region Response Converters

    private static ClientTrackingInfo ConvertClientTrackingInfoResponse(Dictionary<GlideString, object> map)
    {
        IReadOnlySet<string> flags =
            map.TryGetValue("flags", out object? flagsObj) && flagsObj is IEnumerable<object> flagsItems
            ? ToReadOnlyStringSet(flagsItems)
            : EmptyStringSet;

        long redirect = map.TryGetValue("redirect", out object? redirectObj)
            ? Convert.ToInt64(redirectObj)
            : -1;

        IReadOnlySet<string> prefixes =
            map.TryGetValue("prefixes", out object? prefixesObj) && prefixesObj is IEnumerable<object> prefixItems
            ? ToReadOnlyStringSet(prefixItems)
            : EmptyStringSet;

        return new ClientTrackingInfo(flags, redirect, prefixes);
    }

    #endregion
}
