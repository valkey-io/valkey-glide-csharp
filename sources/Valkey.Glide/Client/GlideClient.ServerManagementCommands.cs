// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public partial class GlideClient
{
    /// <inheritdoc cref="IGlideClient.BackgroundSaveAsync()"/>
    public async Task<string> BackgroundSaveAsync()
        => await Command(Request.BackgroundSave());

    /// <inheritdoc cref="IGlideClient.BackgroundSaveCancelAsync()"/>
    public async Task<string> BackgroundSaveCancelAsync()
        => await Command(Request.BackgroundSaveCancel());

    /// <inheritdoc cref="IGlideClient.BackgroundSaveScheduleAsync()"/>
    public async Task<string> BackgroundSaveScheduleAsync()
        => await Command(Request.BackgroundSaveSchedule());

    /// <inheritdoc cref="IGlideClient.BgRewriteAofAsync()"/>
    public async Task<string> BgRewriteAofAsync()
        => await Command(Request.BgRewriteAof());

    /// <inheritdoc cref="IGlideClient.ConfigGetAsync(ValkeyValue)"/>
    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default)
        => await Command(Request.ConfigGet(pattern));

    /// <inheritdoc cref="IGlideClient.ConfigResetStatisticsAsync()"/>
    public async Task ConfigResetStatisticsAsync()
        => _ = await Command(Request.ConfigResetStatistics());

    /// <inheritdoc cref="IGlideClient.ConfigRewriteAsync()"/>
    public async Task ConfigRewriteAsync()
        => _ = await Command(Request.ConfigRewrite());

    /// <inheritdoc cref="IGlideClient.ConfigSetAsync(ValkeyValue, ValkeyValue)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
        => _ = await Command(Request.ConfigSet(setting, value));

    /// <inheritdoc cref="IGlideClient.DatabaseSizeAsync()"/>
    public async Task<long> DatabaseSizeAsync()
        => await Command(Request.DatabaseSize());

    /// <inheritdoc cref="IGlideClient.FailoverAsync()"/>
    public async Task FailoverAsync()
        => _ = await Command(Request.Failover());

    /// <inheritdoc cref="IGlideClient.FailoverAsync(FailoverOptions)"/>
    public async Task FailoverAsync(FailoverOptions options)
        => _ = await Command(Request.Failover(options));

    /// <inheritdoc cref="IGlideClient.FlushAllDatabasesAsync()"/>
    public async Task FlushAllDatabasesAsync()
        => _ = await Command(Request.FlushAllDatabases());

    /// <inheritdoc cref="IGlideClient.FlushDatabaseAsync()"/>
    public async Task FlushDatabaseAsync()
        => _ = await Command(Request.FlushDatabase());

    /// <inheritdoc cref="IGlideClient.InfoAsync()"/>
    public async Task<string> InfoAsync() => await InfoAsync([]);

    /// <inheritdoc cref="IGlideClient.InfoAsync(IEnumerable{InfoOptions.Section})"/>
    public async Task<string> InfoAsync(IEnumerable<InfoOptions.Section> sections)
        => await Command(Request.Info([.. sections]));

    /// <inheritdoc cref="IGlideClient.LastSaveAsync()"/>
    public Task<DateTimeOffset> LastSaveAsync()
        => Command(Request.LastSave());

    /// <inheritdoc cref="IGlideClient.LatencyHistoryAsync(ValkeyValue)"/>
    public async Task<LatencyEntry[]> LatencyHistoryAsync(ValkeyValue @event)
        => await Command(Request.LatencyHistory(@event));

    /// <inheritdoc cref="IGlideClient.LatencyLatestAsync()"/>
    public async Task<LatencyEventInfo[]> LatencyLatestAsync()
        => await Command(Request.LatencyLatest());

    /// <inheritdoc cref="IGlideClient.LolwutAsync()"/>
    public async Task<string> LolwutAsync()
        => await Command(Request.Lolwut());

    /// <inheritdoc cref="IGlideClient.MemoryDoctorAsync()"/>
    public async Task<string> MemoryDoctorAsync()
        => await Command(Request.MemoryDoctor());

    /// <inheritdoc cref="IGlideClient.MemoryMallocStatsAsync()"/>
    public async Task<string> MemoryMallocStatsAsync()
        => await Command(Request.MemoryMallocStats());

    /// <inheritdoc cref="IGlideClient.MemoryPurgeAsync()"/>
    public async Task MemoryPurgeAsync()
        => _ = await Command(Request.MemoryPurge());

    /// <inheritdoc cref="IGlideClient.MemoryStatsAsync()"/>
    public async Task<MemoryStats> MemoryStatsAsync()
        => await Command(Request.MemoryStats());

    /// <inheritdoc cref="IGlideClient.ReplicaOfAsync(string, int)"/>
    public async Task ReplicaOfAsync(string host, int port)
        => _ = await Command(Request.ReplicaOf(host, port));

    /// <inheritdoc cref="IGlideClient.ReplicaOfNoOneAsync()"/>
    public async Task ReplicaOfNoOneAsync()
        => _ = await Command(Request.ReplicaOfNoOne());

    /// <inheritdoc cref="IGlideClient.TimeAsync()"/>
    public Task<DateTimeOffset> TimeAsync()
        => Command(Request.Time());
}
