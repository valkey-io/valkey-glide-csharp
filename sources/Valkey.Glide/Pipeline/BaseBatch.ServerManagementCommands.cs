// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigGetAsync(ValkeyValue)" />
    public T ConfigGetAsync(ValkeyValue pattern = default) => AddCmd(Request.ConfigGetAsync(pattern));

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigResetStatisticsAsync()" />
    public T ConfigResetStatisticsAsync() => AddCmd(Request.ConfigResetStatisticsAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigRewriteAsync()" />
    public T ConfigRewriteAsync() => AddCmd(Request.ConfigRewriteAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigSetAsync(ValkeyValue, ValkeyValue)" />
    public T ConfigSetAsync(ValkeyValue setting, ValkeyValue value) => AddCmd(Request.ConfigSetAsync(setting, value));

    /// <inheritdoc cref="IBatchServerManagementCommands.DatabaseSizeAsync(int)" />
    public T DatabaseSizeAsync(int database = -1) => AddCmd(Request.DatabaseSizeAsync(database));

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushAllDatabasesAsync()" />
    public T FlushAllDatabasesAsync() => AddCmd(Request.FlushAllDatabasesAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushDatabaseAsync(int)" />
    public T FlushDatabaseAsync(int database = -1) => AddCmd(Request.FlushDatabaseAsync(database));

    /// <inheritdoc cref="IBatchServerManagementCommands.LastSaveAsync()" />
    public T LastSaveAsync() => AddCmd(Request.LastSaveAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.TimeAsync()" />
    public T TimeAsync() => AddCmd(Request.TimeAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.LolwutAsync()" />
    public T LolwutAsync() => AddCmd(Request.LolwutAsync());

    // Interface implementations
    IBatch IBatchServerManagementCommands.ConfigGetAsync(ValkeyValue pattern) => ConfigGetAsync(pattern);
    IBatch IBatchServerManagementCommands.ConfigResetStatisticsAsync() => ConfigResetStatisticsAsync();
    IBatch IBatchServerManagementCommands.ConfigRewriteAsync() => ConfigRewriteAsync();
    IBatch IBatchServerManagementCommands.ConfigSetAsync(ValkeyValue setting, ValkeyValue value) => ConfigSetAsync(setting, value);
    IBatch IBatchServerManagementCommands.DatabaseSizeAsync(int database) => DatabaseSizeAsync(database);
    IBatch IBatchServerManagementCommands.FlushAllDatabasesAsync() => FlushAllDatabasesAsync();
    IBatch IBatchServerManagementCommands.FlushDatabaseAsync(int database) => FlushDatabaseAsync(database);
    IBatch IBatchServerManagementCommands.LastSaveAsync() => LastSaveAsync();
    IBatch IBatchServerManagementCommands.TimeAsync() => TimeAsync();
    IBatch IBatchServerManagementCommands.LolwutAsync() => LolwutAsync();
}
