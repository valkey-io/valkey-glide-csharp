// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchConnectionManagementCommands
{
    /// <inheritdoc cref="IServerManagementCommands.PingAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.PingAsync()" /></returns>
    IBatch Ping();

    /// <inheritdoc cref="IServerManagementCommands.PingAsync(ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.PingAsync(ValkeyValue)" /></returns>
    IBatch Ping(ValkeyValue message);

    /// <inheritdoc cref="IServerManagementCommands.EchoAsync(ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.EchoAsync(ValkeyValue)" /></returns>
    IBatch Echo(ValkeyValue message);

    /// <inheritdoc cref="IBaseClient.ClientGetNameAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ClientGetNameAsync()" /></returns>
    IBatch ClientGetNameAsync();

    /// <inheritdoc cref="IConnectionManagementCommands.ClientIdAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IConnectionManagementCommands.ClientIdAsync()" /></returns>
    IBatch ClientIdAsync();

    /// <inheritdoc cref="IServerManagementCommands.SelectAsync(long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.SelectAsync(long)" /></returns>
    IBatch SelectAsync(long index);
}
