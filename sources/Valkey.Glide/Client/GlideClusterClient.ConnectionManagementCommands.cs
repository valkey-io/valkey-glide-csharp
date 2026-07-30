// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

// TODO #462: Consolidate no-route overloads into BaseClient (glide-core default routing matches).
public partial class GlideClusterClient
{
    /// <inheritdoc cref="IGlideClusterClient.ClientGetNameAsync(Route)"/>
    public async Task<ClusterValue<ValkeyValue>> ClientGetNameAsync(Route route)
        => await Command(Request.ClientGetName().ToClusterValue(), route);

    /// <inheritdoc cref="IGlideClusterClient.ClientIdAsync(Route)"/>
    public async Task<ClusterValue<long>> ClientIdAsync(Route route)
        => await Command(Request.ClientId().ToClusterValue(), route);

    /// <inheritdoc cref="IGlideClusterClient.ClientTrackingInfoAsync(Route)"/>
    public async Task<ClusterValue<ClientTrackingInfo>> ClientTrackingInfoAsync(Route route)
        => await Command(Request.ClientTrackingInfo().ToClusterValue(), route);

    /// <inheritdoc cref="IGlideClusterClient.EchoAsync(ValkeyValue, Route)"/>
    public async Task<ClusterValue<ValkeyValue>> EchoAsync(ValkeyValue message, Route route)
        => await Command(Request.Echo(message).ToClusterValue(), route);

    /// <inheritdoc cref="IGlideClusterClient.PingAsync(Route)"/>
    public async Task<ValkeyValue> PingAsync(Route route)
        => await Command(Request.Ping(), route);

    /// <inheritdoc cref="IGlideClusterClient.PingAsync(ValkeyValue, Route)"/>
    public async Task<ValkeyValue> PingAsync(ValkeyValue message, Route route)
        => await Command(Request.Ping(message), route);
}
