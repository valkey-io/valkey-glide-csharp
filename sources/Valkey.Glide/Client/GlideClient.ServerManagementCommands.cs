// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

// TODO #462: Consolidate no-route overloads into BaseClient (glide-core default routing matches).
public partial class GlideClient
{
    /// <inheritdoc cref="IGlideClient.BackgroundSaveAsync()"/>
    public async Task<string> BackgroundSaveAsync()
        => await Command(Request.BackgroundSaveAsync());

    /// <inheritdoc cref="IGlideClient.BackgroundSaveCancelAsync()"/>
    public async Task<string> BackgroundSaveCancelAsync()
        => await Command(Request.BackgroundSaveCancelAsync());

    /// <inheritdoc cref="IGlideClient.BackgroundSaveScheduleAsync()"/>
    public async Task<string> BackgroundSaveScheduleAsync()
        => await Command(Request.BackgroundSaveScheduleAsync());

    /// <inheritdoc cref="IGlideClient.BgRewriteAofAsync()"/>
    public async Task<string> BgRewriteAofAsync()
        => await Command(Request.BgRewriteAofAsync());

    /// <inheritdoc cref="IGlideClient.ConfigGetAsync(ValkeyValue)"/>
    public async Task<KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default)
        => await Command(Request.ConfigGetAsync(pattern));

    /// <inheritdoc cref="IBaseClient.ConfigGetAsync(IEnumerable{ValkeyValue})"/>
    public override async Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns)
        => await Command(Request.ConfigGetAsync(patterns));

    /// <inheritdoc cref="IGlideClient.ConfigResetStatisticsAsync()"/>
    public async Task ConfigResetStatisticsAsync()
        => _ = await Command(Request.ConfigResetStatisticsAsync());

    /// <inheritdoc cref="IGlideClient.ConfigRewriteAsync()"/>
    public async Task ConfigRewriteAsync()
        => _ = await Command(Request.ConfigRewriteAsync());

    /// <inheritdoc cref="IGlideClient.ConfigSetAsync(ValkeyValue, ValkeyValue)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
        => _ = await Command(Request.ConfigSetAsync(setting, value));

    /// <inheritdoc cref="IBaseClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})"/>
    public override async Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters)
        => _ = await Command(Request.ConfigSetAsync(parameters));

    /// <inheritdoc cref="IGlideClient.DatabaseSizeAsync()"/>
    public async Task<long> DatabaseSizeAsync()
        => await Command(Request.DatabaseSizeAsync());

    /// <inheritdoc cref="IGlideClient.FailoverAsync()"/>
    public async Task FailoverAsync()
        => _ = await Command(Request.FailoverAsync());

    /// <inheritdoc cref="IGlideClient.FailoverAsync(FailoverOptions)"/>
    public async Task FailoverAsync(FailoverOptions options)
        => _ = await Command(Request.FailoverAsync(options));

    /// <inheritdoc cref="IGlideClient.FlushAllDatabasesAsync()"/>
    public async Task FlushAllDatabasesAsync()
        => _ = await Command(Request.FlushAllDatabasesAsync());

    /// <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)"/>
    public override async Task FlushAllDatabasesAsync(FlushMode mode)
        => _ = await Command(Request.FlushAllDatabasesAsync(mode));

    /// <inheritdoc cref="IGlideClient.FlushDatabaseAsync()"/>
    public async Task FlushDatabaseAsync()
        => _ = await Command(Request.FlushDatabaseAsync());

    /// <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)"/>
    public override async Task FlushDatabaseAsync(FlushMode mode)
        => _ = await Command(Request.FlushDatabaseAsync(mode));

    /// <inheritdoc cref="IGlideClient.InfoAsync()"/>
    public async Task<string> InfoAsync() => await InfoAsync([]);

    /// <inheritdoc cref="IGlideClient.InfoAsync(IEnumerable{InfoOptions.Section})"/>
    public async Task<string> InfoAsync(IEnumerable<InfoOptions.Section> sections)
        => await Command(Request.Info([.. sections]));

    /// <inheritdoc cref="IGlideClient.LastSaveAsync()"/>
    public Task<DateTimeOffset> LastSaveAsync()
        => Command(Request.LastSaveAsync());

    /// <inheritdoc cref="IGlideClient.LatencyHistoryAsync(ValkeyValue)"/>
    public async Task<LatencyEntry[]> LatencyHistoryAsync(ValkeyValue @event)
        => await Command(Request.LatencyHistoryAsync(@event));

    /// <inheritdoc cref="IGlideClient.LatencyLatestAsync()"/>
    public async Task<LatencyEventInfo[]> LatencyLatestAsync()
        => await Command(Request.LatencyLatestAsync());

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync()"/>
    public override async Task<long> LatencyResetAsync()
        => await Command(Request.LatencyResetAsync([]));

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync(ValkeyValue)"/>
    public override async Task<long> LatencyResetAsync(ValkeyValue @event)
        => await Command(Request.LatencyResetAsync([@event]));

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync(IEnumerable{ValkeyValue})"/>
    public override async Task<long> LatencyResetAsync(IEnumerable<ValkeyValue> events)
        => await Command(Request.LatencyResetAsync(events));

    /// <inheritdoc cref="IGlideClient.LolwutAsync()"/>
    // TODO #475: Move to BaseClient.
    public async Task<string> LolwutAsync()
        => await Command(Request.LolwutAsync());

    /// <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)"/>
    public override async Task<string> LolwutAsync(LolwutOptions options)
        => await Command(Request.LolwutAsync(options));

    /// <inheritdoc cref="IGlideClient.MemoryDoctorAsync()"/>
    public async Task<string> MemoryDoctorAsync()
        => await Command(Request.MemoryDoctorAsync());

    /// <inheritdoc cref="IGlideClient.MemoryMallocStatsAsync()"/>
    public async Task<string> MemoryMallocStatsAsync()
        => await Command(Request.MemoryMallocStatsAsync());

    /// <inheritdoc cref="IGlideClient.MemoryPurgeAsync()"/>
    public async Task MemoryPurgeAsync()
        => _ = await Command(Request.MemoryPurgeAsync());

    /// <inheritdoc cref="IGlideClient.MemoryStatsAsync()"/>
    public async Task<MemoryStats> MemoryStatsAsync()
        => await Command(Request.MemoryStatsAsync());

    /// <inheritdoc cref="IGlideClient.ReplicaOfAsync(string, int)"/>
    public async Task ReplicaOfAsync(string host, int port)
        => _ = await Command(Request.ReplicaOfAsync(host, port));

    /// <inheritdoc cref="IGlideClient.ReplicaOfNoOneAsync()"/>
    public async Task ReplicaOfNoOneAsync()
        => _ = await Command(Request.ReplicaOfNoOneAsync());

    /// <inheritdoc cref="IBaseClient.SaveAsync()"/>
    public override async Task SaveAsync()
        => _ = await Command(Request.SaveAsync());

    /// <inheritdoc cref="IGlideClient.TimeAsync()"/>
    public Task<DateTimeOffset> TimeAsync()
        => Command(Request.TimeAsync());
}
