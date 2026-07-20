// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

using static Valkey.Glide.Route;

namespace Valkey.Glide;

// TODO #462: Consolidate no-route overloads into BaseClient (glide-core default routing matches).
public partial class GlideClusterClient
{
    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveAsync()"/>
    public Task<ClusterValue<string>> BackgroundSaveAsync()
        => BackgroundSaveAsync(AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveAsync(Route)"/>
    public Task<ClusterValue<string>> BackgroundSaveAsync(Route route)
        => Command(Request.BackgroundSaveAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveCancelAsync()"/>
    public Task<ClusterValue<string>> BackgroundSaveCancelAsync()
        => BackgroundSaveCancelAsync(AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveCancelAsync(Route)"/>
    public Task<ClusterValue<string>> BackgroundSaveCancelAsync(Route route)
        => Command(Request.BackgroundSaveCancelAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveScheduleAsync()"/>
    public Task<ClusterValue<string>> BackgroundSaveScheduleAsync()
        => BackgroundSaveScheduleAsync(AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.BackgroundSaveScheduleAsync(Route)"/>
    public Task<ClusterValue<string>> BackgroundSaveScheduleAsync(Route route)
        => Command(Request.BackgroundSaveScheduleAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.BgRewriteAofAsync()"/>
    public Task<ClusterValue<string>> BgRewriteAofAsync()
        => BgRewriteAofAsync(AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.BgRewriteAofAsync(Route)"/>
    public Task<ClusterValue<string>> BgRewriteAofAsync(Route route)
        => Command(Request.BgRewriteAofAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigGetAsync(ValkeyValue)"/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern = default)
        => await Command(Request.ConfigGetAsync(pattern).ToClusterValue(false), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.ConfigGetAsync(ValkeyValue, Route)"/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(ValkeyValue pattern, Route route)
        => await Command(Request.ConfigGetAsync(pattern).ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IBaseClient.ConfigGetAsync(IEnumerable{ValkeyValue})"/>
    public override async Task<KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns)
        => await Command(Request.ConfigGetAsync(patterns));

    /// <inheritdoc cref="IGlideClusterClient.ConfigGetAsync(IEnumerable{ValkeyValue}, Route)"/>
    public async Task<ClusterValue<KeyValuePair<string, string>[]>> ConfigGetAsync(IEnumerable<ValkeyValue> patterns, Route route)
        => await Command(Request.ConfigGetAsync(patterns).ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigResetStatisticsAsync()"/>
    public async Task ConfigResetStatisticsAsync()
        => _ = await Command(Request.ConfigResetStatisticsAsync(), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.ConfigResetStatisticsAsync(Route)"/>
    public async Task ConfigResetStatisticsAsync(Route route)
        => _ = await Command(Request.ConfigResetStatisticsAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigRewriteAsync()"/>
    public async Task ConfigRewriteAsync()
        => _ = await Command(Request.ConfigRewriteAsync(), Route.Random);

    /// <inheritdoc cref="IGlideClusterClient.ConfigRewriteAsync(Route)"/>
    public async Task ConfigRewriteAsync(Route route)
        => _ = await Command(Request.ConfigRewriteAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.ConfigSetAsync(ValkeyValue, ValkeyValue)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
        => _ = await Command(Request.ConfigSetAsync(setting, value), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.ConfigSetAsync(ValkeyValue, ValkeyValue, Route)"/>
    public async Task ConfigSetAsync(ValkeyValue setting, ValkeyValue value, Route route)
        => _ = await Command(Request.ConfigSetAsync(setting, value), route);

    /// <inheritdoc cref="IBaseClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue})"/>
    public override async Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters)
        => _ = await Command(Request.ConfigSetAsync(parameters), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.ConfigSetAsync(IDictionary{ValkeyValue, ValkeyValue}, Route)"/>
    public async Task ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters, Route route)
        => _ = await Command(Request.ConfigSetAsync(parameters), route);

    /// <inheritdoc cref="IGlideClusterClient.DatabaseSizeAsync()"/>
    public async Task<long> DatabaseSizeAsync()
        => await DatabaseSizeAsync(AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.DatabaseSizeAsync(Route)"/>
    public async Task<long> DatabaseSizeAsync(Route route)
    {
        ClusterValue<long> result = await Command(Request.DatabaseSizeAsync().ToClusterValue(false), route);
        return result.HasMultiData ? result.MultiValue.Values.Sum() : result.SingleValue;
    }

    /// <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)"/>
    public async Task FlushAllDatabasesAsync()
        => await FlushAllDatabasesAsync(AllPrimaries);

    /// <inheritdoc cref="IBaseClient.FlushAllDatabasesAsync(FlushMode)"/>
    public override async Task FlushAllDatabasesAsync(FlushMode mode)
        => await FlushAllDatabasesAsync(mode, AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.FlushAllDatabasesAsync(Route)"/>
    public async Task FlushAllDatabasesAsync(Route route)
        => _ = await Command(Request.FlushAllDatabasesAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.FlushAllDatabasesAsync(FlushMode, Route)"/>
    public async Task FlushAllDatabasesAsync(FlushMode mode, Route route)
        => _ = await Command(Request.FlushAllDatabasesAsync(mode), route);

    /// <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)"/>
    public async Task FlushDatabaseAsync()
        => await FlushDatabaseAsync(AllPrimaries);

    /// <inheritdoc cref="IBaseClient.FlushDatabaseAsync(FlushMode)"/>
    public override async Task FlushDatabaseAsync(FlushMode mode)
        => await FlushDatabaseAsync(mode, AllPrimaries);

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
        => await Command(Request.Info([.. sections]).ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.LastSaveAsync()"/>
    public async Task<Dictionary<string, DateTimeOffset>> LastSaveAsync()
    {
        var result = await Command(Request.LastSaveAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, DateTimeOffset> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc cref="IGlideClusterClient.LastSaveAsync(Route)"/>
    public Task<ClusterValue<DateTimeOffset>> LastSaveAsync(Route route)
        => Command(Request.LastSaveAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyHistoryAsync(ValkeyValue)"/>
    public async Task<ClusterValue<LatencyEntry[]>> LatencyHistoryAsync(ValkeyValue @event)
        => await Command(Request.LatencyHistoryAsync(@event).ToClusterValue(false), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.LatencyHistoryAsync(ValkeyValue, Route)"/>
    public async Task<ClusterValue<LatencyEntry[]>> LatencyHistoryAsync(ValkeyValue @event, Route route)
        => await Command(Request.LatencyHistoryAsync(@event).ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyLatestAsync()"/>
    public async Task<ClusterValue<LatencyEventInfo[]>> LatencyLatestAsync()
        => await Command(Request.LatencyLatestAsync().ToClusterValue(false), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.LatencyLatestAsync(Route)"/>
    public async Task<ClusterValue<LatencyEventInfo[]>> LatencyLatestAsync(Route route)
        => await Command(Request.LatencyLatestAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync()"/>
    public override async Task<long> LatencyResetAsync()
        => await Command(Request.LatencyResetAsync([]), AllPrimaries);

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync(ValkeyValue)"/>
    public override async Task<long> LatencyResetAsync(ValkeyValue @event)
        => await Command(Request.LatencyResetAsync([@event]), AllPrimaries);

    /// <inheritdoc cref="IBaseClient.LatencyResetAsync(IEnumerable{ValkeyValue})"/>
    public override async Task<long> LatencyResetAsync(IEnumerable<ValkeyValue> events)
        => await Command(Request.LatencyResetAsync(events), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.LatencyResetAsync(Route)"/>
    public async Task<long> LatencyResetAsync(Route route)
        => await Command(Request.LatencyResetAsync([]), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyResetAsync(ValkeyValue, Route)"/>
    public async Task<long> LatencyResetAsync(ValkeyValue @event, Route route)
        => await Command(Request.LatencyResetAsync([@event]), route);

    /// <inheritdoc cref="IGlideClusterClient.LatencyResetAsync(IEnumerable{ValkeyValue}, Route)"/>
    public async Task<long> LatencyResetAsync(IEnumerable<ValkeyValue> events, Route route)
        => await Command(Request.LatencyResetAsync(events), route);

    /// <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)"/>
    public async Task<Dictionary<string, string>> LolwutAsync()
    {
        ClusterValue<string> result = await Command(Request.LolwutAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, string> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc cref="IBaseClient.LolwutAsync(LolwutOptions)"/>
    public override async Task<string> LolwutAsync(LolwutOptions options)
        => await Command(Request.LolwutAsync(options), Route.Random);

    /// <inheritdoc cref="IGlideClusterClient.LolwutAsync(Route)"/>
    public async Task<ClusterValue<string>> LolwutAsync(Route route)
        => await Command(Request.LolwutAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.LolwutAsync(LolwutOptions, Route)"/>
    public async Task<ClusterValue<string>> LolwutAsync(LolwutOptions options, Route route)
        => await Command(Request.LolwutAsync(options).ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.MemoryDoctorAsync()"/>
    public async Task<ClusterValue<string>> MemoryDoctorAsync()
        => await Command(Request.MemoryDoctorAsync().ToClusterValue(false), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.MemoryDoctorAsync(Route)"/>
    public async Task<ClusterValue<string>> MemoryDoctorAsync(Route route)
        => await Command(Request.MemoryDoctorAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.MemoryMallocStatsAsync()"/>
    public async Task<ClusterValue<string>> MemoryMallocStatsAsync()
        => await Command(Request.MemoryMallocStatsAsync().ToClusterValue(false), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.MemoryMallocStatsAsync(Route)"/>
    public async Task<ClusterValue<string>> MemoryMallocStatsAsync(Route route)
        => await Command(Request.MemoryMallocStatsAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IGlideClusterClient.MemoryPurgeAsync()"/>
    public async Task MemoryPurgeAsync()
        => _ = await Command(Request.MemoryPurgeAsync(), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.MemoryPurgeAsync(Route)"/>
    public async Task MemoryPurgeAsync(Route route)
        => _ = await Command(Request.MemoryPurgeAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.MemoryStatsAsync()"/>
    public async Task<ClusterValue<MemoryStats>> MemoryStatsAsync()
        => await Command(Request.MemoryStatsAsync().ToClusterValue(false), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.MemoryStatsAsync(Route)"/>
    public async Task<ClusterValue<MemoryStats>> MemoryStatsAsync(Route route)
        => await Command(Request.MemoryStatsAsync().ToClusterValue(route is SingleNodeRoute), route);

    /// <inheritdoc cref="IBaseClient.SaveAsync()"/>
    public override async Task SaveAsync()
        => _ = await Command(Request.SaveAsync(), AllPrimaries);

    /// <inheritdoc cref="IGlideClusterClient.SaveAsync(Route)"/>
    public async Task SaveAsync(Route route)
        => _ = await Command(Request.SaveAsync(), route);

    /// <inheritdoc cref="IGlideClusterClient.TimeAsync()"/>
    public async Task<Dictionary<string, DateTimeOffset>> TimeAsync()
    {
        var result = await Command(Request.TimeAsync().ToClusterValue(false), Route.Random);
        if (result.HasMultiData)
        {
            return result.MultiValue;
        }
        // If we got a single value, create a dictionary with a single entry
        return new Dictionary<string, DateTimeOffset> { ["single_node"] = result.SingleValue };
    }

    /// <inheritdoc cref="IGlideClusterClient.TimeAsync(Route)"/>
    public Task<ClusterValue<DateTimeOffset>> TimeAsync(Route route)
        => Command(Request.TimeAsync().ToClusterValue(route is SingleNodeRoute), route);
}
