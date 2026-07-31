// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

public partial class GlideClusterClient
{
    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveAsync()"/>
    public Task<ClusterValue<string>> BackgroundSaveAsync()
        => Command(Request.BackgroundSaveAsync().ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveAsync(Route)"/>
    public Task<ClusterValue<string>> BackgroundSaveAsync(Route route)
        => Command(Request.BackgroundSaveAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveCancelAsync()"/>
    public Task<ClusterValue<string>> BackgroundSaveCancelAsync()
        => Command(Request.BackgroundSaveCancelAsync().ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveCancelAsync(Route)"/>
    public Task<ClusterValue<string>> BackgroundSaveCancelAsync(Route route)
        => Command(Request.BackgroundSaveCancelAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveScheduleAsync()"/>
    public Task<ClusterValue<string>> BackgroundSaveScheduleAsync()
        => Command(Request.BackgroundSaveScheduleAsync().ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveScheduleAsync(Route)"/>
    public Task<ClusterValue<string>> BackgroundSaveScheduleAsync(Route route)
        => Command(Request.BackgroundSaveScheduleAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.BgRewriteAofAsync()"/>
    public Task<ClusterValue<string>> BgRewriteAofAsync()
        => Command(Request.BgRewriteAofAsync().ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.BgRewriteAofAsync(Route)"/>
    public Task<ClusterValue<string>> BgRewriteAofAsync(Route route)
        => Command(Request.BgRewriteAofAsync().ToClusterValue(route), route);

    // TODO #495: Remove method; consolidate single-value version into BaseClient.
    /// <inheritdoc cref="IGlideClusterClient.ConfigGetAsync(ValkeyValue)"/>
    [Obsolete("Use ConfigGetAsync(ValkeyValue, Route) instead. See #495.")]
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern = default)
        => await Command(Request.ConfigGetAsync(pattern).ToClusterValue(), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.ConfigGetAsync(ValkeyValue, Route)"/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route)
        => await Command(Request.ConfigGetAsync(pattern).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigGetAsync(IEnumerable{ValkeyValue}, Route)"/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(IEnumerable<ValkeyValue> patterns, Route route)
        => await Command(Request.ConfigGetAsync(patterns).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigResetStatisticsAsync()"/>
    public async Task ConfigResetStatisticsAsync()
        => _ = await Command(Request.ConfigResetStatisticsAsync());

    /// <inheritdoc cref="IGlideClusterClient.ConfigResetStatisticsAsync(Route)"/>
    public async Task ConfigResetStatisticsAsync(Route route)
        => _ = await Command(Request.ConfigResetStatisticsAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigRewriteAsync()"/>
    public async Task ConfigRewriteAsync()
        => _ = await Command(Request.ConfigRewriteAsync());

    /// <inheritdoc cref="IGlideClusterClient.ConfigRewriteAsync(Route)"/>
    public async Task ConfigRewriteAsync(Route route)
        => _ = await Command(Request.ConfigRewriteAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigSetAsync(ValkeyValue, ValkeyValue)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
        => _ = await Command(Request.ConfigSetAsync(setting, value));

    /// <inheritdoc cref="IGlideClusterClient.ConfigSetAsync(ValkeyValue, ValkeyValue, Route)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route)
        => _ = await Command(Request.ConfigSetAsync(setting, value), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue}, Route)"/>
    public async Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters, Route route)
        => _ = await Command(Request.ConfigSetAsync(parameters), route);

    /// <inheritdoc cref="IGlideClusterClient.DatabaseSizeAsync()"/>
    public async Task<long> DatabaseSizeAsync()
    {
        ClusterValue<long> result = await Command(Request.DatabaseSizeAsync().ToClusterValue());
        return result.HasMultiData ? result.MultiValue.Values.Sum() : result.SingleValue;
    }

    /// <inheritdoc cref="IGlideClusterClient.DatabaseSizeAsync(Route)"/>
    public async Task<long> DatabaseSizeAsync(Route route)
    {
        ClusterValue<long> result = await Command(Request.DatabaseSizeAsync().ToClusterValue(route), route);
        return result.HasMultiData ? result.MultiValue.Values.Sum() : result.SingleValue;
    }

    /// <inheritdoc cref="IGlideClusterClient.FlushAllDatabasesAsync()"/>
    public async Task FlushAllDatabasesAsync()
        => _ = await Command(Request.FlushAllDatabasesAsync());

    /// <inheritdoc cref="IGlideClusterClient.FlushAllDatabasesAsync(Route)"/>
    public async Task FlushAllDatabasesAsync(Route route)
        => _ = await Command(Request.FlushAllDatabasesAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.FlushAllDatabasesAsync(FlushMode, Route)"/>
    public async Task FlushAllDatabasesAsync(FlushMode mode, Route route)
        => _ = await Command(Request.FlushAllDatabasesAsync(mode), route);

    /// <inheritdoc cref="IGlideClusterClient.FlushDatabaseAsync()"/>
    public async Task FlushDatabaseAsync()
        => _ = await Command(Request.FlushDatabaseAsync());

    /// <inheritdoc cref="IGlideClusterClient.FlushDatabaseAsync(Route)"/>
    public async Task FlushDatabaseAsync(Route route)
        => _ = await Command(Request.FlushDatabaseAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.FlushDatabaseAsync(FlushMode, Route)"/>
    public async Task FlushDatabaseAsync(FlushMode mode, Route route)
        => _ = await Command(Request.FlushDatabaseAsync(mode), route);

    /// <inheritdoc cref="IGlideClusterClient.InfoAsync()"/>
    public async Task<Dictionary<string, string>> InfoAsync() => await InfoAsync([]);

    /// <inheritdoc cref="IGlideClusterClient.InfoAsync(IEnumerable{InfoOptions.Section})"/>
    public async Task<Dictionary<string, string>> InfoAsync(IEnumerable<InfoOptions.Section> sections)
        => await Command(Request.Info([.. sections]).ToMultiNodeValue());

    /// <inheritdoc cref="IGlideClusterClient.InfoAsync(Route)"/>
    public async Task<ClusterValue<string>> InfoAsync(Route route) => await InfoAsync([], route);

    /// <inheritdoc cref="IGlideClusterClient.InfoAsync(IEnumerable{InfoOptions.Section}, Route)"/>
    public async Task<ClusterValue<string>> InfoAsync(IEnumerable<InfoOptions.Section> sections, Route route)
        => await Command(Request.Info([.. sections]).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.LastSaveAsync()"/>
    public async Task<Dictionary<string, DateTimeOffset>> LastSaveAsync()
    {
        var result = await Command(Request.LastSaveAsync().ToClusterValue());
        return result.HasMultiData ? result.MultiValue : new Dictionary<string, DateTimeOffset> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc cref="IGlideClusterClient.LastSaveAsync(Route)"/>
    public Task<ClusterValue<DateTimeOffset>> LastSaveAsync(Route route)
        => Command(Request.LastSaveAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyHistoryAsync(ValkeyValue)"/>
    public async Task<ClusterValue<LatencyEntry[]>> LatencyHistoryAsync(ValkeyValue @event)
        => await Command(Request.LatencyHistoryAsync(@event).ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.LatencyHistoryAsync(ValkeyValue, Route)"/>
    public async Task<ClusterValue<LatencyEntry[]>> LatencyHistoryAsync(ValkeyValue @event, Route route)
        => await Command(Request.LatencyHistoryAsync(@event).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyLatestAsync()"/>
    public async Task<ClusterValue<LatencyEventInfo[]>> LatencyLatestAsync()
        => await Command(Request.LatencyLatestAsync().ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.LatencyLatestAsync(Route)"/>
    public async Task<ClusterValue<LatencyEventInfo[]>> LatencyLatestAsync(Route route)
        => await Command(Request.LatencyLatestAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyResetAsync(Route)"/>
    public async Task<long> LatencyResetAsync(Route route)
        => await Command(Request.LatencyResetAsync([]), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyResetAsync(ValkeyValue, Route)"/>
    public async Task<long> LatencyResetAsync(ValkeyValue @event, Route route)
        => await Command(Request.LatencyResetAsync([@event]), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyResetAsync(IEnumerable{ValkeyValue}, Route)"/>
    public async Task<long> LatencyResetAsync(IEnumerable<ValkeyValue> events, Route route)
        => await Command(Request.LatencyResetAsync(events), route);

    /// <inheritdoc cref="IGlideClusterClient.LolwutAsync()"/>
    [Obsolete("This method will be updated to return Task<string> in future. Use LolwutAsync(Route.Random) instead")]
    public async Task<Dictionary<string, string>> LolwutAsync()
    {
        var result = await Command(Request.LolwutAsync().ToClusterValue());
        return result.HasMultiData ? result.MultiValue : new Dictionary<string, string> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc cref="IGlideClusterClient.LolwutAsync(Route)"/>
    public async Task<ClusterValue<string>> LolwutAsync(Route route)
        => await Command(Request.LolwutAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.LolwutAsync(LolwutOptions, Route)"/>
    public async Task<ClusterValue<string>> LolwutAsync(LolwutOptions options, Route route)
        => await Command(Request.LolwutAsync(options).ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.MemoryDoctorAsync()"/>
    public async Task<ClusterValue<string>> MemoryDoctorAsync()
        => await Command(Request.MemoryDoctorAsync().ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.MemoryDoctorAsync(Route)"/>
    public async Task<ClusterValue<string>> MemoryDoctorAsync(Route route)
        => await Command(Request.MemoryDoctorAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.MemoryMallocStatsAsync()"/>
    public async Task<ClusterValue<string>> MemoryMallocStatsAsync()
        => await Command(Request.MemoryMallocStatsAsync().ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.MemoryMallocStatsAsync(Route)"/>
    public async Task<ClusterValue<string>> MemoryMallocStatsAsync(Route route)
        => await Command(Request.MemoryMallocStatsAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.MemoryPurgeAsync()"/>
    public async Task MemoryPurgeAsync()
        => _ = await Command(Request.MemoryPurgeAsync());

    /// <inheritdoc cref="IGlideClusterClient.MemoryPurgeAsync(Route)"/>
    public async Task MemoryPurgeAsync(Route route)
        => _ = await Command(Request.MemoryPurgeAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.MemoryStatsAsync()"/>
    public async Task<ClusterValue<MemoryStats>> MemoryStatsAsync()
        => await Command(Request.MemoryStatsAsync().ToClusterValue());

    /// <inheritdoc cref="IGlideClusterClient.MemoryStatsAsync(Route)"/>
    public async Task<ClusterValue<MemoryStats>> MemoryStatsAsync(Route route)
        => await Command(Request.MemoryStatsAsync().ToClusterValue(route), route);

    /// <inheritdoc cref="IGlideClusterClient.SaveAsync(Route)"/>
    public async Task SaveAsync(Route route)
        => _ = await Command(Request.SaveAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.TimeAsync()"/>
    public async Task<Dictionary<string, DateTimeOffset>> TimeAsync()
    {
        var result = await Command(Request.TimeAsync().ToClusterValue());
        return result.HasMultiData ? result.MultiValue : new Dictionary<string, DateTimeOffset> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc cref="IGlideClusterClient.TimeAsync(Route)"/>
    public Task<ClusterValue<DateTimeOffset>> TimeAsync(Route route)
        => Command(Request.TimeAsync().ToClusterValue(route), route);
}
