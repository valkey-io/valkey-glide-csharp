// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

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

    /// <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})" /></returns>
    IBatch ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters);

    /// <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.ConfigGetAsync(IEnumerable{ValkeyValue})" /></returns>
    IBatch ConfigGetAsync(IEnumerable<ValkeyValue> patterns);

    /// <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.DatabaseSizeAsync()" /></returns>
    IBatch DatabaseSizeAsync();

    /// <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync()" /></returns>
    IBatch FlushAllDatabasesAsync();

    /// <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync(FlushMode)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.FlushAllDatabasesAsync(FlushMode)" /></returns>
    IBatch FlushAllDatabasesAsync(FlushMode mode);

    /// <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync()" /></returns>
    IBatch FlushDatabaseAsync();

    /// <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(FlushMode)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.FlushDatabaseAsync(FlushMode)" /></returns>
    IBatch FlushDatabaseAsync(FlushMode mode);

    /// <inheritdoc cref="IServerManagementCommands.LastSaveAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.LastSaveAsync()" /></returns>
    IBatch LastSaveAsync();

    /// <inheritdoc cref="IServerManagementCommands.TimeAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.TimeAsync()" /></returns>
    IBatch TimeAsync();

    /// <inheritdoc cref="IServerManagementCommands.LolwutAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.LolwutAsync()" /></returns>
    IBatch LolwutAsync();

    /// <inheritdoc cref="IServerManagementCommands.LolwutAsync(LolwutOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IServerManagementCommands.LolwutAsync(LolwutOptions)" /></returns>
    IBatch LolwutAsync(LolwutOptions options);
}
