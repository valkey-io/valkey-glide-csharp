// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using System.Diagnostics;

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, string> Info(InfoOptions.Section[] sections)
        => new(RequestType.Info, sections.ToGlideStrings(), false, gs => gs.ToString());

    public static Cmd<GlideString, TimeSpan> Ping()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        return new(RequestType.Ping, [], false, _ =>
        {
            stopwatch.Stop();
            return stopwatch.Elapsed;
        });
    }

    public static Cmd<GlideString, TimeSpan> Ping(ValkeyValue message)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        GlideString[] args = [message.ToGlideString()];
        return new(RequestType.Ping, args, false, _ =>
        {
            stopwatch.Stop();
            return stopwatch.Elapsed;
        });
    }

    public static Cmd<GlideString, ValkeyValue> Echo(ValkeyValue message)
    {
        GlideString[] args = [message.ToGlideString()];
        return new(RequestType.Echo, args, false, gs => (ValkeyValue)gs);
    }

    public static Cmd<object, KeyValuePair<string, string>[]> ConfigGetAsync(ValkeyValue pattern = default)
    {
        GlideString[] args = pattern.IsNull ? ["*"] : [pattern.ToGlideString()];
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

    public static Cmd<string, object?> ConfigResetStatisticsAsync()
        => new(RequestType.ConfigResetStat, [], false, _ => ValkeyValue.Null);

    public static Cmd<string, object?> ConfigRewriteAsync()
        => new(RequestType.ConfigRewrite, [], false, _ => ValkeyValue.Null);

    public static Cmd<string, object?> ConfigSetAsync(ValkeyValue setting, ValkeyValue value)
    {
        GlideString[] args = [setting.ToGlideString(), value.ToGlideString()];
        return new(RequestType.ConfigSet, args, false, _ => ValkeyValue.Null);
    }

    public static Cmd<long, long> DatabaseSizeAsync(int database = -1)
        // DBSIZE doesn't take database parameter - it operates on current database
        // Database selection should be handled at connection level
        => database != -1
            ? throw new ArgumentException("DBSIZE command does not support database selection. Use SELECT command first.")
            : new(RequestType.DBSize, [], false, l => l);

    public static Cmd<string, object?> FlushAllDatabasesAsync()
        => new(RequestType.FlushAll, [], false, _ => ValkeyValue.Null);

    public static Cmd<string, object?> FlushDatabaseAsync(int database = -1)
        // FLUSHDB doesn't take database parameter - it operates on current database
        // Database selection should be handled at connection level
        => database != -1
            ? throw new ArgumentException("FLUSHDB command does not support database selection. Use SELECT command first.")
            : new(RequestType.FlushDB, [], false, _ => ValkeyValue.Null);

    public static Cmd<long, DateTime> LastSaveAsync()
        => new(RequestType.LastSave, [], false, l => DateTime.UnixEpoch.AddSeconds(l));

    public static Cmd<object[], DateTime> TimeAsync()
        => new(RequestType.Time, [], false, arr =>
        {
            long seconds = long.Parse(arr[0] is GlideString gs1 ? gs1.ToString() : arr[0].ToString()!);
            long microseconds = long.Parse(arr[1] is GlideString gs2 ? gs2.ToString() : arr[1].ToString()!);
            return DateTime.UnixEpoch.AddSeconds(seconds)
#if NET8_0_OR_GREATER
                .AddMicroseconds(microseconds);
#else
                .AddTicks(microseconds * 10);
#endif
        });

    public static Cmd<GlideString, string> LolwutAsync()
        => new(RequestType.Lolwut, [], false, gs => gs.ToString());
    public static Cmd<string, string> Select(long index)
        => OK(RequestType.Select, [index.ToString().ToGlideString()]);
}
