// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, string> MemoryDoctorAsync()
        => new(RequestType.MemoryDoctor, [], false, gs => gs.ToString());

    public static Cmd<GlideString, string> MemoryMallocStatsAsync()
        => new(RequestType.MemoryMallocStats, [], false, gs => gs.ToString());

    public static Cmd<string, ValkeyValue> MemoryPurgeAsync()
        => Ok(RequestType.MemoryPurge, []);

    public static Cmd<Dictionary<GlideString, object>, MemoryStats> MemoryStatsAsync()
        => new(RequestType.MemoryStats, [], false, ParseMemoryStats);

    private const string MemoryStatsDbPrefix = "db.";

    private static MemoryStats ParseMemoryStats(Dictionary<GlideString, object> map)
    {
        Dictionary<int, MemoryStatsDb> db = [];
        foreach (KeyValuePair<GlideString, object> kvp in map)
        {
            string key = kvp.Key.ToString();
            if (key.StartsWith(MemoryStatsDbPrefix) && key != "db.dict.rehashing.count")
            {
                string suffix = key[MemoryStatsDbPrefix.Length..];
                if (int.TryParse(suffix, out int dbIndex))
                {
                    db[dbIndex] = ParseMemoryStatsDb((Dictionary<GlideString, object>)kvp.Value);
                }
            }
        }

        return new MemoryStats
        {
            Db = db,
            AllocatorActive = GetLong(map, "allocator.active"),
            AllocatorAllocated = GetLong(map, "allocator.allocated"),
            AllocatorFragmentationBytes = GetLong(map, "allocator-fragmentation.bytes"),
            AllocatorResident = GetLong(map, "allocator.resident"),
            AllocatorRssBytes = GetLong(map, "allocator-rss.bytes"),
            AofBuffer = GetLong(map, "aof.buffer"),
            ClientsNormal = GetLong(map, "clients.normal"),
            ClientsSlaves = GetLong(map, "clients.slaves"),
            DatasetBytes = GetLong(map, "dataset.bytes"),
            FragmentationBytes = GetLong(map, "fragmentation.bytes"),
            KeysBytesPerKey = GetLong(map, "keys.bytes-per-key"),
            KeysCount = GetLong(map, "keys.count"),
            LuaCaches = GetLong(map, "lua.caches"),
            OverheadTotal = GetLong(map, "overhead.total"),
            PeakAllocated = GetLong(map, "peak.allocated"),
            ReplicationBacklog = GetLong(map, "replication.backlog"),
            RssOverheadBytes = GetLong(map, "rss-overhead.bytes"),
            StartupAllocated = GetLong(map, "startup.allocated"),
            TotalAllocated = GetLong(map, "total.allocated"),
            AllocatorFragmentationRatio = GetDouble(map, "allocator-fragmentation.ratio"),
            AllocatorRssRatio = GetDouble(map, "allocator-rss.ratio"),
            DatasetPercentage = GetDouble(map, "dataset.percentage"),
            Fragmentation = GetDouble(map, "fragmentation"),
            PeakPercentage = GetDouble(map, "peak.percentage"),
            RssOverheadRatio = GetDouble(map, "rss-overhead.ratio"),

            // Optional Valkey 7.0+ fields
            ClusterLinks = TryGetLong(map, "cluster.links"),
            FunctionsCaches = TryGetLong(map, "functions.caches"),

            // Optional Valkey 8.0+ fields
            AllocatorMuzzy = TryGetLong(map, "allocator.muzzy"),
            DbDictRehashingCount = TryGetLong(map, "db.dict.rehashing.count"),
            OverheadDbHashtableLut = TryGetLong(map, "overhead.db.hashtable.lut"),
            OverheadDbHashtableRehashing = TryGetLong(map, "overhead.db.hashtable.rehashing"),
        };
    }

    private static MemoryStatsDb ParseMemoryStatsDb(Dictionary<GlideString, object> map)
        => new()
        {
            OverheadHashtableMain = GetLong(map, "overhead.hashtable.main"),
            OverheadHashtableExpires = GetLong(map, "overhead.hashtable.expires"),
        };

    public static Cmd<GlideString, string> Info(InfoOptions.Section[] sections)
        => new(RequestType.Info, sections.ToGlideStrings(), false, gs => gs.ToString());

    public static Cmd<GlideString, ValkeyValue> Ping()
        => new(RequestType.Ping, [], false, gs => (ValkeyValue)gs);

    public static Cmd<GlideString, ValkeyValue> Ping(ValkeyValue message)
        => new(RequestType.Ping, [message], false, gs => (ValkeyValue)gs);

    public static Cmd<GlideString, ValkeyValue> Echo(ValkeyValue message)
        => new(RequestType.Echo, [message], false, gs => (ValkeyValue)gs);

    public static Cmd<object, KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default)
        => ConfigGetAsyncInternal(pattern.IsNull ? ["*"] : [pattern]);

    public static Cmd<object, KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns)
        => ConfigGetAsyncInternal([.. patterns.Select(static p => (GlideString)p)]);

    private static Cmd<object, KeyValuePair<string, string>[]> ConfigGetAsyncInternal(GlideString[] args)
    {
        return new(RequestType.ConfigGet, args, false, response =>
        {
            // Handle both array and dictionary formats
            if (response is null)
            {
                return [];
            }

            // If it's a dictionary, convert directly
            if (response is Dictionary<GlideString, object> dict)
            {
                if (dict.Count == 0)
                {
                    return [];
                }

                List<KeyValuePair<string, string>> result = [];
                foreach (KeyValuePair<GlideString, object> kvp in dict)
                {
                    string key = kvp.Key.ToString();
                    string value = kvp.Value is GlideString gs ? gs.ToString() : kvp.Value?.ToString() ?? string.Empty;
                    result.Add(new KeyValuePair<string, string>(key, value));
                }
                return [.. result];
            }

            // If it's an array, convert from array
            if (response is object[] array)
            {
                if (array.Length == 0)
                {
                    return [];
                }

                List<KeyValuePair<string, string>> result = [];
                for (int i = 0; i < array.Length; i += 2)
                {
                    if (i + 1 < array.Length)
                    {
                        string key = array[i] is GlideString gs1 ? gs1.ToString() : "";
                        string value = array[i + 1] is GlideString gs2 ? gs2.ToString() : array[i + 1]?.ToString() ?? "";
                        result.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
                return [.. result];
            }

            // Fallback for unexpected types
            return [];
        });
    }

    public static Cmd<string, ValkeyValue> ConfigResetStatisticsAsync()
        => Ok(RequestType.ConfigResetStat, []);

    public static Cmd<string, ValkeyValue> ConfigRewriteAsync()
        => Ok(RequestType.ConfigRewrite, []);

    public static Cmd<string, ValkeyValue> ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
        => Ok(RequestType.ConfigSet, [setting, value]);

    public static Cmd<string, ValkeyValue> ConfigSetAsync(IDictionary<ValkeyValue, ValkeyValue> parameters)
    {
        List<GlideString> args = [];
        foreach (KeyValuePair<ValkeyValue, ValkeyValue> kvp in parameters)
        {
            args.Add(kvp.Key);
            args.Add(kvp.Value);
        }
        return Ok(RequestType.ConfigSet, [.. args]);
    }

    public static Cmd<long, long> DatabaseSizeAsync()
        => new(RequestType.DBSize, [], false, l => l);

    public static Cmd<string, ValkeyValue> FlushAllDatabasesAsync()
        => Ok(RequestType.FlushAll, []);

    public static Cmd<string, ValkeyValue> FlushAllDatabasesAsync(FlushMode mode)
        => Ok(RequestType.FlushAll, [mode == FlushMode.Sync ? ValkeyLiterals.SYNC : ValkeyLiterals.ASYNC]);

    public static Cmd<string, ValkeyValue> FlushDatabaseAsync()
        => Ok(RequestType.FlushDB, []);

    public static Cmd<string, ValkeyValue> FlushDatabaseAsync(FlushMode mode)
        => Ok(RequestType.FlushDB, [mode == FlushMode.Sync ? ValkeyLiterals.SYNC : ValkeyLiterals.ASYNC]);

    public static Cmd<string, ValkeyValue> SaveAsync()
        => Ok(RequestType.Save, []);

    public static Cmd<long, DateTimeOffset> LastSaveAsync()
        => new(RequestType.LastSave, [], false, DateTimeOffset.FromUnixTimeSeconds);

    public static Cmd<object[], DateTimeOffset> TimeAsync()
        => new(RequestType.Time, [], false, arr =>
        {
            long seconds = long.Parse(arr[0] is GlideString gs1 ? gs1.ToString() : arr[0].ToString()!);
            long microseconds = long.Parse(arr[1] is GlideString gs2 ? gs2.ToString() : arr[1].ToString()!);
            return DateTimeOffset.FromUnixTimeSeconds(seconds)
                .AddMicroseconds(microseconds);
        });

    public static Cmd<GlideString, string> LolwutAsync(LolwutOptions? options = null)
        => new(RequestType.Lolwut, options is null ? [] : [.. options.ToArgs()], false, gs => gs.ToString());

    public static Cmd<GlideString, string> BgRewriteAofAsync()
        => new(RequestType.BgRewriteAof, [], false, gs => gs.ToString());

    public static Cmd<string, ValkeyValue> Select(long index)
        => Ok(RequestType.Select, [index.ToGlideString()]);

    public static Cmd<object, LatencyEntry[]> LatencyHistoryAsync(ValkeyValue @event)
        => new(RequestType.LatencyHistory, [@event], false, ConvertLatencyHistoryResponse);

    public static Cmd<object, LatencyEventInfo[]> LatencyLatestAsync()
        => new(RequestType.LatencyLatest, [], false, ConvertLatencyLatestResponse);

    public static Cmd<long, long> LatencyResetAsync(IEnumerable<ValkeyValue> events)
        => new(RequestType.LatencyReset, events.ToGlideStrings(), false, l => l);

    private static LatencyEntry[] ConvertLatencyHistoryResponse(object response)
    {
        object[] array = (object[])response;
        LatencyEntry[] entries = new LatencyEntry[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            object[] entry = (object[])array[i];
            entries[i] = new()
            {
                Time = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(entry[0])),
                Duration = TimeSpan.FromMilliseconds(Convert.ToInt64(entry[1])),
            };
        }
        return entries;
    }

    private static LatencyEventInfo[] ConvertLatencyLatestResponse(object response)
    {
        object[] array = (object[])response;
        LatencyEventInfo[] entries = new LatencyEventInfo[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            object[] entry = (object[])array[i];
            entries[i] = new()
            {
                EventName = ((GlideString)entry[0]).ToString(),
                LatestTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(entry[1])),
                LatestDuration = TimeSpan.FromMilliseconds(Convert.ToInt64(entry[2])),
                MaxDuration = TimeSpan.FromMilliseconds(Convert.ToInt64(entry[3])),
                Sum = entry.Length > 4 ? TimeSpan.FromMilliseconds(Convert.ToInt64(entry[4])) : null,
                Count = entry.Length > 5 ? Convert.ToInt64(entry[5]) : null,
            };
        }
        return entries;
    }

    public static Cmd<GlideString, string> BackgroundSaveAsync()
        => new(RequestType.BgSave, [], false, gs => gs.ToString());

    public static Cmd<GlideString, string> BackgroundSaveScheduleAsync()
        => new(RequestType.BgSave, [ValkeyLiterals.SCHEDULE], false, gs => gs.ToString());

    public static Cmd<GlideString, string> BackgroundSaveCancelAsync()
        => new(RequestType.BgSave, [ValkeyLiterals.CANCEL], false, gs => gs.ToString());

    public static Cmd<string, ValkeyValue> FailoverAsync()
        => Ok(RequestType.FailOver, []);

    public static Cmd<string, ValkeyValue> FailoverAsync(FailoverOptions options)
        => Ok(RequestType.FailOver, options.ToArgs());

    public static Cmd<string, ValkeyValue> ReplicaOfAsync(string host, int port)
        => Ok(RequestType.ReplicaOf, [host, port.ToGlideString()]);

    public static Cmd<string, ValkeyValue> ReplicaOfNoOneAsync()
        => Ok(RequestType.ReplicaOf, [ValkeyLiterals.NO, ValkeyLiterals.ONE]);
}
