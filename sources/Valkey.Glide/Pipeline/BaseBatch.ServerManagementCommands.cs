// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    // TODO #538: Rename '*Async' methods
#pragma warning disable RCS1047 // Non-asynchronous method name should not end with 'Async'

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigGetAsync(ValkeyValue)" />
    public T ConfigGetAsync(ValkeyValue pattern = default) => AddCmd(Request.ConfigGet(pattern));

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigGetAsync(IEnumerable{ValkeyValue})" />
    public T ConfigGetAsync(IEnumerable<ValkeyValue> patterns) => AddCmd(Request.ConfigGet(patterns));

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigResetStatisticsAsync()" />
    public T ConfigResetStatisticsAsync() => AddCmd(Request.ConfigResetStatistics());

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigRewriteAsync()" />
    public T ConfigRewriteAsync() => AddCmd(Request.ConfigRewrite());

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue)" />
    public T ConfigSetAsync(ValkeyValue setting, ValkeyValue value) => AddCmd(Request.ConfigSet(setting, value));

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})" />
    public T ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters) => AddCmd(Request.ConfigSet(parameters));

    /// <inheritdoc cref="IBatchServerManagementCommands.DatabaseSizeAsync()" />
    public T DatabaseSizeAsync() => AddCmd(Request.DatabaseSize());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushAllDatabasesAsync()" />
    public T FlushAllDatabasesAsync() => AddCmd(Request.FlushAllDatabases());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushAllDatabasesAsync(FlushMode)" />
    public T FlushAllDatabasesAsync(FlushMode mode) => AddCmd(Request.FlushAllDatabases(mode));

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushDatabaseAsync()" />
    public T FlushDatabaseAsync() => AddCmd(Request.FlushDatabase());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushDatabaseAsync(FlushMode)" />
    public T FlushDatabaseAsync(FlushMode mode) => AddCmd(Request.FlushDatabase(mode));

    /// <inheritdoc cref="IBatchServerManagementCommands.LastSaveAsync()" />
    public T LastSaveAsync() => AddCmd(Request.LastSave());

    /// <inheritdoc cref="IBatchServerManagementCommands.LolwutAsync()" />
    public T LolwutAsync() => AddCmd(Request.Lolwut());

    /// <inheritdoc cref="IBatchServerManagementCommands.LolwutAsync(LolwutOptions)" />
    public T LolwutAsync(LolwutOptions options) => AddCmd(Request.Lolwut(options));

    /// <inheritdoc cref="IBatchServerManagementCommands.TimeAsync()" />
    public T TimeAsync() => AddCmd(Request.Time());

    // Interface implementations
    IBatch IBatchServerManagementCommands.ConfigGetAsync(ValkeyValue pattern) => ConfigGetAsync(pattern);
    IBatch IBatchServerManagementCommands.ConfigGetAsync(IEnumerable<ValkeyValue> patterns) => ConfigGetAsync(patterns);
    IBatch IBatchServerManagementCommands.ConfigResetStatisticsAsync() => ConfigResetStatisticsAsync();
    IBatch IBatchServerManagementCommands.ConfigRewriteAsync() => ConfigRewriteAsync();
    IBatch IBatchServerManagementCommands.ConfigSetAsync(ValkeyValue setting, ValkeyValue value) => ConfigSetAsync(setting, value);
    IBatch IBatchServerManagementCommands.ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters) => ConfigSetAsync(parameters);
    IBatch IBatchServerManagementCommands.DatabaseSizeAsync() => DatabaseSizeAsync();
    IBatch IBatchServerManagementCommands.FlushAllDatabasesAsync() => FlushAllDatabasesAsync();
    IBatch IBatchServerManagementCommands.FlushAllDatabasesAsync(FlushMode mode) => FlushAllDatabasesAsync(mode);
    IBatch IBatchServerManagementCommands.FlushDatabaseAsync() => FlushDatabaseAsync();
    IBatch IBatchServerManagementCommands.FlushDatabaseAsync(FlushMode mode) => FlushDatabaseAsync(mode);
    IBatch IBatchServerManagementCommands.LastSaveAsync() => LastSaveAsync();
    IBatch IBatchServerManagementCommands.LolwutAsync() => LolwutAsync();
    IBatch IBatchServerManagementCommands.LolwutAsync(LolwutOptions options) => LolwutAsync(options);
    IBatch IBatchServerManagementCommands.TimeAsync() => TimeAsync();

#pragma warning restore RCS1047
}
