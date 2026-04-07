// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchConnectionManagementCommands
{
    /// <inheritdoc cref="IBaseClient.PingAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.PingAsync()" /></returns>
    IBatch Ping();

    /// <inheritdoc cref="IBaseClient.PingAsync(ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.PingAsync(ValkeyValue)" /></returns>
    IBatch Ping(ValkeyValue message);

    /// <inheritdoc cref="IBaseClient.EchoAsync(ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.EchoAsync(ValkeyValue)" /></returns>
    IBatch Echo(ValkeyValue message);

    /// <inheritdoc cref="IBaseClient.ClientGetNameAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ClientGetNameAsync()" /></returns>
    IBatch ClientGetNameAsync();

    /// <inheritdoc cref="IBaseClient.ClientIdAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ClientIdAsync()" /></returns>
    IBatch ClientIdAsync();

    /// <inheritdoc cref="IConnectionManagementBaseCommands.SelectAsync(long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IConnectionManagementBaseCommands.SelectAsync(long)" /></returns>
    IBatch SelectAsync(long index);
}
