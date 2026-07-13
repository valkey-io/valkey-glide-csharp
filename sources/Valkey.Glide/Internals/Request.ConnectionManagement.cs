// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Route;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, ValkeyValue> ClientGetName()
        => new(RequestType.ClientGetName, [], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs, allowConverterToHandleNull: true);

    public static Cmd<object, ClusterValue<ValkeyValue>> ClientGetNameCluster(Route? route = null)
        => new(RequestType.ClientGetName, [], true, ResponseConverters.MakeClusterValueHandler<object?, ValkeyValue>(ValkeyValue.Unbox, route is SingleNodeRoute), allowConverterToHandleNull: true);

    public static Cmd<long, long> ClientId()
        => Simple<long>(RequestType.ClientId, []);

    public static Cmd<string, ValkeyValue> ClientSetName(string connectionName)
        => Ok(RequestType.ClientSetName, [connectionName.ToGlideString()]);

    public static Cmd<string, ValkeyValue> ClientPause(TimeSpan timeout)
        => Ok(RequestType.ClientPause, [ToMilliseconds(timeout)]);

    public static Cmd<string, ValkeyValue> ClientPauseWrite(TimeSpan timeout)
        => Ok(RequestType.ClientPause, [ToMilliseconds(timeout), ValkeyLiterals.WRITE.ToGlideString()]);

    public static Cmd<string, ValkeyValue> ClientUnpause()
        => Ok(RequestType.ClientUnpause, []);

    public static Cmd<Dictionary<GlideString, object>, ClientTrackingInfo> ClientTrackingInfo()
        => new(RequestType.ClientTrackingInfo, [], false, ConvertClientTrackingInfoResponse);

    private static ClientTrackingInfo ConvertClientTrackingInfoResponse(Dictionary<GlideString, object> map)
    {
        ISet<string> flags = map.TryGetValue("flags", out object? flagsObj) && flagsObj is IEnumerable<object> flagsItems
            ? ToStringSet(flagsItems)
            : new HashSet<string>();

        long redirect = map.TryGetValue("redirect", out object? redirectObj)
            ? Convert.ToInt64(redirectObj)
            : -1;

        ISet<string> prefixes = map.TryGetValue("prefixes", out object? prefixesObj) && prefixesObj is IEnumerable<object> prefixItems
            ? ToStringSet(prefixItems)
            : new HashSet<string>();

        return new ClientTrackingInfo(flags, redirect, prefixes);
    }
}
