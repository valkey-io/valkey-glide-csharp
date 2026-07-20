// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

// TODO #462: Consolidate no-route overloads into BaseClient (glide-core default routing matches).
public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.ClientGetNameAsync()"/>
    public abstract Task<ValkeyValue> ClientGetNameAsync();

    /// <inheritdoc cref="IBaseClient.ClientIdAsync()"/>
    public abstract Task<long> ClientIdAsync();

    /// <inheritdoc cref="IBaseClient.ClientPauseAsync(TimeSpan)"/>
    public abstract Task ClientPauseAsync(TimeSpan timeout);

    /// <inheritdoc cref="IBaseClient.ClientPauseWriteAsync(TimeSpan)"/>
    public abstract Task ClientPauseWriteAsync(TimeSpan timeout);

    /// <inheritdoc cref="IBaseClient.ClientTrackingInfoAsync()"/>
    public abstract Task<ClientTrackingInfo> ClientTrackingInfoAsync();

    /// <inheritdoc cref="IBaseClient.ClientUnpauseAsync()"/>
    public abstract Task ClientUnpauseAsync();

    /// <inheritdoc cref="IBaseClient.EchoAsync(ValkeyValue)"/>
    public abstract Task<ValkeyValue> EchoAsync(ValkeyValue message);

    /// <inheritdoc cref="IBaseClient.PingAsync()"/>
    public abstract Task<ValkeyValue> PingAsync();

    /// <inheritdoc cref="IBaseClient.PingAsync(ValkeyValue)"/>
    public abstract Task<ValkeyValue> PingAsync(ValkeyValue message);

    /// <inheritdoc cref="IBaseClient.ResetAsync()"/>
    public async Task ResetAsync()
        => _ = await Command(Request.Reset());

    /// <inheritdoc cref="IConnectionManagementBaseCommands.SelectAsync(long)"/>
    public abstract Task SelectAsync(long index);
}
