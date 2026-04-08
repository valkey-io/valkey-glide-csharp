// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public abstract Task<ValkeyValue> ClientGetNameAsync();

    /// <inheritdoc/>
    public abstract Task<long> ClientIdAsync();

    /// <inheritdoc/>
    public abstract Task<ValkeyValue> EchoAsync(ValkeyValue message);

    /// <inheritdoc/>
    public abstract Task<ValkeyValue> PingAsync();

    /// <inheritdoc/>
    public abstract Task<ValkeyValue> PingAsync(ValkeyValue message);

    /// <inheritdoc/>
    public abstract Task SelectAsync(long index);
}
