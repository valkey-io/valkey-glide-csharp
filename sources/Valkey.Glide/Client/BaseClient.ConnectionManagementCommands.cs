// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.ClientGetNameAsync()"/>
    public abstract Task<ValkeyValue> ClientGetNameAsync();

    /// <inheritdoc cref="IBaseClient.ClientIdAsync()"/>
    public abstract Task<long> ClientIdAsync();

    /// <inheritdoc cref="IBaseClient.EchoAsync(ValkeyValue)"/>
    public abstract Task<ValkeyValue> EchoAsync(ValkeyValue message);

    /// <inheritdoc cref="IBaseClient.PingAsync()"/>
    public abstract Task<ValkeyValue> PingAsync();

    /// <inheritdoc cref="IBaseClient.PingAsync(ValkeyValue)"/>
    public abstract Task<ValkeyValue> PingAsync(ValkeyValue message);

    /// <inheritdoc cref="IConnectionManagementBaseCommands.SelectAsync(long)"/>
    public abstract Task SelectAsync(long index);

    /// <inheritdoc cref="IBaseClient.ClientPauseAsync(TimeSpan)"/>
    public abstract Task ClientPauseAsync(TimeSpan timeout);

    /// <inheritdoc cref="IBaseClient.ClientPauseWriteAsync(TimeSpan)"/>
    public abstract Task ClientPauseWriteAsync(TimeSpan timeout);

    /// <inheritdoc cref="IBaseClient.ClientUnpauseAsync()"/>
    public abstract Task ClientUnpauseAsync();

    /// <inheritdoc cref="IBaseClient.ClientTrackingInfoAsync()"/>
    public abstract Task<ClientTrackingInfo> ClientTrackingInfoAsync();

    /// <inheritdoc cref="IBaseClient.ResetAsync()"/>
    public abstract Task ResetAsync();
}
