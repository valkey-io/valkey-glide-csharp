// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchServerManagementCommands
{
    /// <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(ValkeyValue, CommandFlags)" /></returns>
    IBatch ConfigGetAsync(ValkeyValue pattern = default);

    /// <inheritdoc cref="IServerManagementCommands.ConfigResetStatisticsAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigResetStatisticsAsync(CommandFlags)" /></returns>
    IBatch ConfigResetStatisticsAsync();

    /// <inheritdoc cref="IServerManagementCommands.ConfigRewriteAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigRewriteAsync(CommandFlags)" /></returns>
    IBatch ConfigRewriteAsync();

    /// <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue, CommandFlags)" /></returns>
    IBatch ConfigSetAsync(ValkeyValue setting, ValkeyValue value);

    /// <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync(int, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync(int, CommandFlags)" /></returns>
    IBatch DatabaseSizeAsync(int database = -1);

    /// <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync(CommandFlags)" /></returns>
    IBatch FlushAllDatabasesAsync();

    /// <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(int, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(int, CommandFlags)" /></returns>
    IBatch FlushDatabaseAsync(int database = -1);

    /// <inheritdoc cref="IServerManagementCommands.LastSaveAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.LastSaveAsync(CommandFlags)" /></returns>
    IBatch LastSaveAsync();

    /// <inheritdoc cref="IServerManagementCommands.TimeAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.TimeAsync(CommandFlags)" /></returns>
    IBatch TimeAsync();

    /// <inheritdoc cref="IServerManagementCommands.LolwutAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.LolwutAsync(CommandFlags)" /></returns>
    IBatch LolwutAsync();
}
