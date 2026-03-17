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

    /// <inheritdoc cref="IServerManagementCommands.EchoAsync(ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.EchoAsync(ValkeyValue, CommandFlags)" /></returns>
    IBatch Echo(ValkeyValue message);

    /// <inheritdoc cref="IConnectionManagementCommands.ClientGetNameAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IConnectionManagementCommands.ClientGetNameAsync(CommandFlags)" /></returns>
    IBatch ClientGetNameAsync();

    /// <inheritdoc cref="IConnectionManagementCommands.ClientIdAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IConnectionManagementCommands.ClientIdAsync(CommandFlags)" /></returns>
    IBatch ClientIdAsync();

    /// <inheritdoc cref="IServerManagementCommands.SelectAsync(long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.SelectAsync(long, CommandFlags)" /></returns>
    IBatch SelectAsync(long index);
}
