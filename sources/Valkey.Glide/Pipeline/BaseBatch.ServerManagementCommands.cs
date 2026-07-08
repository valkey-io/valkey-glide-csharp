// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
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

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})" />
    public T ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters) => AddCmd(Request.ConfigSetAsync(parameters));

    /// <inheritdoc cref="IBatchServerManagementCommands.ConfigGetAsync(IEnumerable{ValkeyValue})" />
    public T ConfigGetAsync(IEnumerable<ValkeyValue> patterns) => AddCmd(Request.ConfigGetAsync(patterns));

    /// <inheritdoc cref="IBatchServerManagementCommands.DatabaseSizeAsync()" />
    public T DatabaseSizeAsync() => AddCmd(Request.DatabaseSizeAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushAllDatabasesAsync()" />
    public T FlushAllDatabasesAsync() => AddCmd(Request.FlushAllDatabasesAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushAllDatabasesAsync(FlushMode)" />
    public T FlushAllDatabasesAsync(FlushMode mode) => AddCmd(Request.FlushAllDatabasesAsync(mode));

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushDatabaseAsync()" />
    public T FlushDatabaseAsync() => AddCmd(Request.FlushDatabaseAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.FlushDatabaseAsync(FlushMode)" />
    public T FlushDatabaseAsync(FlushMode mode) => AddCmd(Request.FlushDatabaseAsync(mode));

    /// <inheritdoc cref="IBatchServerManagementCommands.LastSaveAsync()" />
    public T LastSaveAsync() => AddCmd(Request.LastSaveAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.TimeAsync()" />
    public T TimeAsync() => AddCmd(Request.TimeAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.LolwutAsync()" />
    public T LolwutAsync() => AddCmd(Request.LolwutAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.LolwutAsync(LolwutOptions)" />
    public T LolwutAsync(LolwutOptions options) => AddCmd(Request.LolwutAsync(options));

    /// <inheritdoc cref="IBatchServerManagementCommands.BackgroundSaveAsync()" />
    public T BackgroundSaveAsync() => AddCmd(Request.BackgroundSaveAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.BackgroundSaveScheduleAsync()" />
    public T BackgroundSaveScheduleAsync() => AddCmd(Request.BackgroundSaveScheduleAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.BackgroundSaveCancelAsync()" />
    public T BackgroundSaveCancelAsync() => AddCmd(Request.BackgroundSaveCancelAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.MemoryDoctorAsync()" />
    public T MemoryDoctorAsync() => AddCmd(Request.MemoryDoctorAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.MemoryMallocStatsAsync()" />
    public T MemoryMallocStatsAsync() => AddCmd(Request.MemoryMallocStatsAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.MemoryPurgeAsync()" />
    public T MemoryPurgeAsync() => AddCmd(Request.MemoryPurgeAsync());

    /// <inheritdoc cref="IBatchServerManagementCommands.MemoryStatsAsync()" />
    public T MemoryStatsAsync() => AddCmd(Request.MemoryStatsAsync());

    // Interface implementations
    IBatch IBatchServerManagementCommands.ConfigGetAsync(ValkeyValue pattern) => ConfigGetAsync(pattern);
    IBatch IBatchServerManagementCommands.ConfigResetStatisticsAsync() => ConfigResetStatisticsAsync();
    IBatch IBatchServerManagementCommands.ConfigRewriteAsync() => ConfigRewriteAsync();
    IBatch IBatchServerManagementCommands.ConfigSetAsync(ValkeyValue setting, ValkeyValue value) => ConfigSetAsync(setting, value);
    IBatch IBatchServerManagementCommands.ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters) => ConfigSetAsync(parameters);
    IBatch IBatchServerManagementCommands.ConfigGetAsync(IEnumerable<ValkeyValue> patterns) => ConfigGetAsync(patterns);
    IBatch IBatchServerManagementCommands.DatabaseSizeAsync() => DatabaseSizeAsync();
    IBatch IBatchServerManagementCommands.FlushAllDatabasesAsync() => FlushAllDatabasesAsync();
    IBatch IBatchServerManagementCommands.FlushAllDatabasesAsync(FlushMode mode) => FlushAllDatabasesAsync(mode);
    IBatch IBatchServerManagementCommands.FlushDatabaseAsync() => FlushDatabaseAsync();
    IBatch IBatchServerManagementCommands.FlushDatabaseAsync(FlushMode mode) => FlushDatabaseAsync(mode);
    IBatch IBatchServerManagementCommands.LastSaveAsync() => LastSaveAsync();
    IBatch IBatchServerManagementCommands.TimeAsync() => TimeAsync();
    IBatch IBatchServerManagementCommands.LolwutAsync() => LolwutAsync();
    IBatch IBatchServerManagementCommands.LolwutAsync(LolwutOptions options) => LolwutAsync(options);
    IBatch IBatchServerManagementCommands.BackgroundSaveAsync() => BackgroundSaveAsync();
    IBatch IBatchServerManagementCommands.BackgroundSaveScheduleAsync() => BackgroundSaveScheduleAsync();
    IBatch IBatchServerManagementCommands.BackgroundSaveCancelAsync() => BackgroundSaveCancelAsync();
    IBatch IBatchServerManagementCommands.MemoryDoctorAsync() => MemoryDoctorAsync();
    IBatch IBatchServerManagementCommands.MemoryMallocStatsAsync() => MemoryMallocStatsAsync();
    IBatch IBatchServerManagementCommands.MemoryPurgeAsync() => MemoryPurgeAsync();
    IBatch IBatchServerManagementCommands.MemoryStatsAsync() => MemoryStatsAsync();
}
