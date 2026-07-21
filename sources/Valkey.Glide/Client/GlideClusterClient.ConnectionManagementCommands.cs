// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

public partial class GlideClusterClient
{
    /// <inheritdoc cref="IBaseClient.ClientGetNameAsync()"/>
    public override async Task<ValkeyValue> ClientGetNameAsync()
        => await Command(Request.ClientGetName(), Route.Random);

    /// <inheritdoc cref="IGlideClusterClient.ClientGetNameAsync(Route)"/>
    public async Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route)
        => await Command(Request.ClientGetName(route), route);

    /// <inheritdoc cref="IBaseClient.ClientIdAsync()"/>
    public override async Task<long> ClientIdAsync()
        => await Command(Request.ClientId(), Route.Random);

    /// <inheritdoc cref="IGlideClusterClient.ClientIdAsync(Route)"/>
    public async Task<ClusterValue<long>> ClientIdAsync(Route route)
        => await Command(Request.ClientId().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IBaseClient.ClientPauseAsync(TimeSpan)"/>
    public override async Task ClientPauseAsync(TimeSpan timeout)
        => _ = await Command(Request.ClientPause(timeout), AllPrimaries);

    /// <inheritdoc cref="IBaseClient.ClientPauseWriteAsync(TimeSpan)"/>
    public override async Task ClientPauseWriteAsync(TimeSpan timeout)
        => _ = await Command(Request.ClientPauseWrite(timeout), AllPrimaries);

    /// <inheritdoc cref="IBaseClient.ClientTrackingInfoAsync()"/>
    public override async Task<ClientTrackingInfo> ClientTrackingInfoAsync()
        => await Command(Request.ClientTrackingInfo(), Route.Random);

    /// <inheritdoc cref="IGlideClusterClient.ClientTrackingInfoAsync(Route)"/>
    public async Task<ClusterValue<ClientTrackingInfo>> ClientTrackingInfoAsync(Route route)
        => await Command(Request.ClientTrackingInfo().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IBaseClient.ClientUnpauseAsync()"/>
    public override async Task ClientUnpauseAsync()
        => _ = await Command(Request.ClientUnpause(), AllPrimaries);

    /// <inheritdoc cref="IBaseClient.EchoAsync(ValkeyValue)"/>
    public override async Task<ValkeyValue> EchoAsync(ValkeyValue message)
        => await Command(Request.Echo(message), Route.Random);

    /// <inheritdoc cref="IGlideClusterClient.EchoAsync(ValkeyValue, Route)"/>
    public async Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route)
        => await Command(Request.Echo(message).ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IBaseClient.PingAsync()"/>
    public override async Task<ValkeyValue> PingAsync()
        => await Command(Request.Ping(), AllPrimaries);

    /// <inheritdoc cref="IBaseClient.PingAsync(ValkeyValue)"/>
    public override async Task<ValkeyValue> PingAsync(ValkeyValue message)
        => await Command(Request.Ping(message), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.PingAsync(Route)"/>
    public async Task<ValkeyValue> PingAsync(Route route)
        => await Command(Request.Ping(), route);

    /// <inheritdoc cref="IGlideClusterClient.PingAsync(ValkeyValue, Route)"/>
    public async Task<ValkeyValue> PingAsync(ValkeyValue message, Route route)
        => await Command(Request.Ping(message), route);

    /// <inheritdoc cref="IConnectionManagementBaseCommands.SelectAsync(long)"/>
    public override async Task SelectAsync(long index)
        => _ = await Command(Request.Select(index), Route.Random);
}
