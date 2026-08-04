// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClusterClient
{
    /// <inheritdoc cref="IGlideClusterClient.ClientGetNameAsync(Route)"/>
    public async Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route)
        => await Command(Request.ClientGetName().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.ClientIdAsync(Route)"/>
    public async Task<ClusterValue<long>> ClientIdAsync(Route route)
        => await Command(Request.ClientId().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.ClientKillAsync(Route)"/>
    public async Task<long> ClientKillAsync(Route route)
        => await ClientKillAsync(new ClientFilterOptions().WithSkipMe(true), route);

    /// <inheritdoc cref="IGlideClusterClient.ClientKillAsync(string, ushort, Route)"/>
    public async Task ClientKillAsync(string host, ushort port, Route route)
        => _ = await Command(Request.ClientKill(host, port), route);

    /// <inheritdoc cref="IGlideClusterClient.ClientKillAsync(ClientFilterOptions, Route)"/>
    public async Task<long> ClientKillAsync(ClientFilterOptions options, Route route)
    {
        ClusterValue<long> result = await Command(Request.ClientKill(options).ToClusterValue(route), route);
        return result.HasMultiData ? result.MultiValue.Values.Sum() : result.SingleValue;
    }

    /// <inheritdoc cref="IGlideClusterClient.ClientTrackingInfoAsync(Route)"/>
    public async Task<ClusterValue<ClientTrackingInfo>> ClientTrackingInfoAsync(Route route)
        => await Command(Request.ClientTrackingInfo().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.EchoAsync(ValkeyValue, Route)"/>
    public async Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route)
        => await Command(Request.Echo(message).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.PingAsync(Route)"/>
    public async Task<ValkeyValue> PingAsync(Route route)
        => await Command(Request.Ping(), route);

    /// <inheritdoc cref="IGlideClusterClient.PingAsync(ValkeyValue, Route)"/>
    public async Task<ValkeyValue> PingAsync(ValkeyValue message, Route route)
        => await Command(Request.Ping(message), route);
}
