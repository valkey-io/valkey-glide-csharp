// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

internal interface IBatchServerManagementCommands
{
    /// <inheritdoc cref="IGlideClient.ConfigGetAsync(ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.ConfigGetAsync(ValkeyValue)" /></returns>
    IBatch ConfigGetAsync(ValkeyValue pattern = default);

    /// <inheritdoc cref="IGlideClient.ConfigResetStatisticsAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.ConfigResetStatisticsAsync()" /></returns>
    IBatch ConfigResetStatisticsAsync();

    /// <inheritdoc cref="IGlideClient.ConfigRewriteAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.ConfigRewriteAsync()" /></returns>
    IBatch ConfigRewriteAsync();

    /// <inheritdoc cref="IGlideClient.ConfigSetAsync(ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.ConfigSetAsync(ValkeyValue, ValkeyValue)" /></returns>
    IBatch ConfigSetAsync(ValkeyValue setting, ValkeyValue value);

    /// <inheritdoc cref="IBaseClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})" /></returns>
    IBatch ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters);

    /// <inheritdoc cref="IBaseClient.ConfigGetAsync(IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ConfigGetAsync(IEnumerable{ValkeyValue})" /></returns>
    IBatch ConfigGetAsync(IEnumerable<ValkeyValue> patterns);

    /// <inheritdoc cref="IGlideClient.DatabaseSizeAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.DatabaseSizeAsync()" /></returns>
    IBatch DatabaseSizeAsync();

    /// <inheritdoc cref="IGlideClient.FlushAllDatabasesAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.FlushAllDatabasesAsync()" /></returns>
    IBatch FlushAllDatabasesAsync();

    /// <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)" /></returns>
    IBatch FlushAllDatabasesAsync(FlushMode mode);

    /// <inheritdoc cref="IGlideClient.FlushDatabaseAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.FlushDatabaseAsync()" /></returns>
    IBatch FlushDatabaseAsync();

    /// <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)" /></returns>
    IBatch FlushDatabaseAsync(FlushMode mode);

    /// <inheritdoc cref="IGlideClient.LastSaveAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.LastSaveAsync()" /></returns>
    IBatch LastSaveAsync();

    /// <inheritdoc cref="IGlideClient.TimeAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.TimeAsync()" /></returns>
    IBatch TimeAsync();

    /// <inheritdoc cref="IGlideClient.LolwutAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGlideClient.LolwutAsync()" /></returns>
    IBatch LolwutAsync();

    /// <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)" /></returns>
    IBatch LolwutAsync(LolwutOptions options);
}
