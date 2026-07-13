// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, string> Info(InfoOptions.Section[] sections)
        => new(RequestType.Info, sections.ToGlideStrings(), false, gs => gs.ToString());

    public static Cmd<GlideString, ValkeyValue> Ping()
        => new(RequestType.Ping, [], false, gs => (ValkeyValue)gs);

    public static Cmd<GlideString, ValkeyValue> Ping(ValkeyValue message)
    {
        GlideString[] args = [message.ToGlideString()];
        return new(RequestType.Ping, args, false, gs => (ValkeyValue)gs);
    }

    public static Cmd<GlideString, ValkeyValue> Echo(ValkeyValue message)
    {
        GlideString[] args = [message.ToGlideString()];
        return new(RequestType.Echo, args, false, gs => (ValkeyValue)gs);
    }

    public static Cmd<object, KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default)
    {
        GlideString[] args = pattern.IsNull ? ["*"] : [pattern.ToGlideString()];
        return ConfigGetAsyncInternal(args);
    }

    public static Cmd<object, KeyValuePair<string, string>[]> ConfigGetAsync(IEnumerable<ValkeyValue> patterns)
    {
        GlideString[] args = [.. patterns.Select(p => p.ToGlideString())];
        return ConfigGetAsyncInternal(args);
    }

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
        => Ok(RequestType.ConfigResetStat);

    public static Cmd<string, ValkeyValue> ConfigRewriteAsync()
        => Ok(RequestType.ConfigRewrite);

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
        => Ok(RequestType.FlushAll);

    public static Cmd<string, ValkeyValue> FlushAllDatabasesAsync(FlushMode mode)
        => Ok(RequestType.FlushAll, [mode == FlushMode.Sync ? ValkeyLiterals.SYNC : ValkeyLiterals.ASYNC]);

    public static Cmd<string, ValkeyValue> FlushDatabaseAsync()
        => Ok(RequestType.FlushDB);

    public static Cmd<string, ValkeyValue> FlushDatabaseAsync(FlushMode mode)
        => Ok(RequestType.FlushDB, [mode == FlushMode.Sync ? ValkeyLiterals.SYNC : ValkeyLiterals.ASYNC]);

    public static Cmd<string, ValkeyValue> SaveAsync()
        => Ok(RequestType.Save);

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
    {
        GlideString[] args = options is null ? [] : [.. options.ToArgs().Select(a => a.ToGlideString())];
        return new(RequestType.Lolwut, args, false, gs => gs.ToString());
    }

    public static Cmd<GlideString, string> BgRewriteAofAsync()
        => new(RequestType.BgRewriteAof, [], false, gs => gs.ToString());

    public static Cmd<string, ValkeyValue> Select(long index)
        => Ok(RequestType.Select, [index.ToGlideString()]);

    public static Cmd<object, LatencyEntry[]> LatencyHistoryAsync(ValkeyValue @event)
        => new(RequestType.LatencyHistory, [@event.ToGlideString()], false, ConvertLatencyHistoryResponse);

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
        => new(RequestType.BgSave, [ValkeyLiterals.SCHEDULE.ToGlideString()], false, gs => gs.ToString());

    public static Cmd<GlideString, string> BackgroundSaveCancelAsync()
        => new(RequestType.BgSave, [ValkeyLiterals.CANCEL.ToGlideString()], false, gs => gs.ToString());
}
