// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClient
{
    /// <inheritdoc/>
    public override async Task<ValkeyValue> ClientGetNameAsync()
        => await Command(Request.ClientGetName());

    /// <inheritdoc/>
    public override async Task<long> ClientIdAsync()
        => await Command(Request.ClientId());

    /// <inheritdoc/>
    public override async Task<ValkeyValue> EchoAsync(ValkeyValue message)
        => await Command(Request.Echo(message));

    /// <inheritdoc/>
    public override async Task<ValkeyValue> PingAsync()
        => await Command(Request.Ping());

    /// <inheritdoc/>
    public override async Task<ValkeyValue> PingAsync(ValkeyValue message)
        => await Command(Request.Ping(message));

    /// <inheritdoc/>
    public override async Task SelectAsync(long index)
        => _ = await Command(Request.Select(index));
}
