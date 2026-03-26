// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchServerManagementCommands
{
    /// <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(ValkeyValue)" /></returns>
    IBatch ConfigGetAsync(ValkeyValue pattern = default);

    /// <inheritdoc cref="IServerManagementCommands.ConfigResetStatisticsAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigResetStatisticsAsync()" /></returns>
    IBatch ConfigResetStatisticsAsync();

    /// <inheritdoc cref="IServerManagementCommands.ConfigRewriteAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigRewriteAsync()" /></returns>
    IBatch ConfigRewriteAsync();

    /// <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue)" /></returns>
    IBatch ConfigSetAsync(ValkeyValue setting, ValkeyValue value);

    /// <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync(int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync(int)" /></returns>
    IBatch DatabaseSizeAsync(int database = -1);

    /// <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync()" /></returns>
    IBatch FlushAllDatabasesAsync();

    /// <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(int)" /></returns>
    IBatch FlushDatabaseAsync(int database = -1);

    /// <inheritdoc cref="IServerManagementCommands.LastSaveAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.LastSaveAsync()" /></returns>
    IBatch LastSaveAsync();

    /// <inheritdoc cref="IServerManagementCommands.TimeAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.TimeAsync()" /></returns>
    IBatch TimeAsync();

    /// <inheritdoc cref="IServerManagementCommands.LolwutAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.LolwutAsync()" /></returns>
    IBatch LolwutAsync();
}
