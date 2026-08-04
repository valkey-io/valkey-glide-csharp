// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.ClientGetNameAsync()"/>
    public async Task<ValkeyValue> ClientGetNameAsync()
        => await Command(Request.ClientGetName());

    /// <inheritdoc cref="IBaseClient.ClientIdAsync()"/>
    public async Task<long> ClientIdAsync()
        => await Command(Request.ClientId());

    /// <inheritdoc cref="IBaseClient.ClientKillAsync()"/>
    public async Task<long> ClientKillAsync()
        => await Command(Request.ClientKill(new ClientFilterOptions().WithSkipMe(true)));

    /// <inheritdoc cref="IBaseClient.ClientKillAsync(string, ushort)"/>
    public async Task ClientKillAsync(string host, ushort port)
        => _ = await Command(Request.ClientKill(host, port));

    /// <inheritdoc cref="IBaseClient.ClientKillAsync(ClientFilterOptions)"/>
    public async Task<long> ClientKillAsync(ClientFilterOptions options)
        => await Command(Request.ClientKill(options));

    /// <inheritdoc cref="IBaseClient.ClientPauseAsync(TimeSpan)"/>
    public async Task ClientPauseAsync(TimeSpan timeout)
        => _ = await Command(Request.ClientPause(timeout));

    /// <inheritdoc cref="IBaseClient.ClientPauseWriteAsync(TimeSpan)"/>
    public async Task ClientPauseWriteAsync(TimeSpan timeout)
        => _ = await Command(Request.ClientPauseWrite(timeout));

    /// <inheritdoc cref="IBaseClient.ClientTrackingInfoAsync()"/>
    public async Task<ClientTrackingInfo> ClientTrackingInfoAsync()
        => await Command(Request.ClientTrackingInfo());

    /// <inheritdoc cref="IBaseClient.ClientUnpauseAsync()"/>
    public async Task ClientUnpauseAsync()
        => _ = await Command(Request.ClientUnpause());

    /// <inheritdoc cref="IBaseClient.EchoAsync(ValkeyValue)"/>
    public async Task<ValkeyValue> EchoAsync(ValkeyValue message)
        => await Command(Request.Echo(message));

    /// <inheritdoc cref="IBaseClient.PingAsync()"/>
    public async Task<ValkeyValue> PingAsync()
        => await Command(Request.Ping());

    /// <inheritdoc cref="IBaseClient.PingAsync(ValkeyValue)"/>
    public async Task<ValkeyValue> PingAsync(ValkeyValue message)
        => await Command(Request.Ping(message));

    /// <inheritdoc cref="IBaseClient.ResetAsync()"/>
    public async Task ResetAsync()
        => _ = await Command(Request.Reset());

    /// <inheritdoc cref="IConnectionManagementBaseCommands.SelectAsync(long)"/>
    public async Task SelectAsync(long index)
        => _ = await Command(Request.Select(index));
}
