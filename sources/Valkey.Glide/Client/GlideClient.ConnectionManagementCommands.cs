// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClient
{
    /// <inheritdoc cref="IBaseClient.ClientGetNameAsync()"/>
    public override async Task<ValkeyValue> ClientGetNameAsync()
        => await Command(Request.ClientGetName());

    /// <inheritdoc cref="IBaseClient.ClientIdAsync()"/>
    public override async Task<long> ClientIdAsync()
        => await Command(Request.ClientId());

    /// <inheritdoc cref="IBaseClient.EchoAsync(ValkeyValue)"/>
    public override async Task<ValkeyValue> EchoAsync(ValkeyValue message)
        => await Command(Request.Echo(message));

    /// <inheritdoc cref="IBaseClient.PingAsync()"/>
    public override async Task<ValkeyValue> PingAsync()
        => await Command(Request.Ping());

    /// <inheritdoc cref="IBaseClient.PingAsync(ValkeyValue)"/>
    public override async Task<ValkeyValue> PingAsync(ValkeyValue message)
        => await Command(Request.Ping(message));

    /// <inheritdoc cref="IConnectionManagementBaseCommands.SelectAsync(long)"/>
    public override async Task SelectAsync(long index)
        => _ = await Command(Request.Select(index));
}
