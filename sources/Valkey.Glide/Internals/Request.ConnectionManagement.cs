// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Route;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, ValkeyValue> ClientGetName()
        => new(RequestType.ClientGetName, [], true, gs => gs is null ? ValkeyValue.Null : (ValkeyValue)gs, true);

    public static Cmd<object, ClusterValue<ValkeyValue>> ClientGetNameCluster(Route? route = null)
        => new(RequestType.ClientGetName, [], true, ResponseConverters.MakeClusterValueHandler<object?, ValkeyValue>(v => ValkeyValue.Unbox(v), route is SingleNodeRoute), true);

    public static Cmd<long, long> ClientId()
        => Simple<long>(RequestType.ClientId, []);

    public static Cmd<string, string> ClientSetName(string connectionName)
        => OK(RequestType.ClientSetName, [connectionName.ToGlideString()]);
}
