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
    public T DatabaseSizeAsync(int database = -1) => AddCmd(Request.DatabaseSizeAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushAllDatabasesAsync()" />
    public T FlushAllDatabasesAsync() => AddCmd(Request.FlushAllDatabasesAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushDatabaseAsync(int)" />
    public T FlushDatabaseAsync(int database = -1) => AddCmd(Request.FlushDatabaseAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.LastSaveAsync()" />
    public T LastSaveAsync() => AddCmd(Request.LastSaveAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.TimeAsync()" />
    public T TimeAsync() => AddCmd(Request.TimeAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.LolwutAsync()" />
    public T LolwutAsync() => AddCmd(Request.LolwutAsync());

    // Interface implementations
    Task<KeyValuePair<string, string>[]> IBatchServerManagementCommands.ConfigGetAsync(ValkeyValue pattern) => throw new NotImplementedException("Use the non-async version for batch operations");
    Task IBatchServerManagementCommands.ConfigResetStatisticsAsync() => throw new NotImplementedException("Use the non-async version for batch operations");
    Task IBatchServerManagementCommands.ConfigRewriteAsync() => throw new NotImplementedException("Use the non-async version for batch operations");
    Task IBatchServerManagementCommands.ConfigSetAsync(ValkeyValue setting, ValkeyValue value) => throw new NotImplementedException("Use the non-async version for batch operations");
    Task<long> IBatchServerManagementCommands.DatabaseSizeAsync(int database) => throw new NotImplementedException("Use the non-async version for batch operations");
    Task IBatchServerManagementCommands.FlushAllDatabasesAsync() => throw new NotImplementedException("Use the non-async version for batch operations");
    Task IBatchServerManagementCommands.FlushDatabaseAsync(int database) => throw new NotImplementedException("Use the non-async version for batch operations");
    Task<DateTime> IBatchServerManagementCommands.LastSaveAsync() => throw new NotImplementedException("Use the non-async version for batch operations");
    Task<DateTime> IBatchServerManagementCommands.TimeAsync() => throw new NotImplementedException("Use the non-async version for batch operations");
    Task<string> IBatchServerManagementCommands.LolwutAsync() => throw new NotImplementedException("Use the non-async version for batch operations");
}
