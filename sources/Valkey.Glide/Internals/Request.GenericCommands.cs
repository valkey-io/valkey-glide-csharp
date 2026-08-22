// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    #region Constants

    private static readonly GlideString InstantExpiry = "-1";

    #endregion Constants
    #region Command Builders

    public static Cmd<bool, bool> Copy(ValkeyKey source, ValkeyKey destination, bool replace = false)
    {
        List<GlideString> args = [source, destination];

        if (replace)
        {
            args.Add(ValkeyLiterals.REPLACE);
        }

        return Simple<bool>(RequestType.Copy, [.. args]);
    }

    public static Cmd<bool, bool> Copy(ValkeyKey source, ValkeyKey destination, int destinationDatabase, bool replace = false)
    {
        List<GlideString> args = [source, destination, ValkeyLiterals.DB, destinationDatabase.ToGlideString()];

        if (replace)
        {
            args.Add(ValkeyLiterals.REPLACE);
        }

        return Simple<bool>(RequestType.Copy, [.. args]);
    }

    public static Cmd<long, bool> Delete(ValkeyKey key)
        => Boolean<long>(RequestType.Del, [key]);

    public static Cmd<long, long> Delete(ValkeyKey[] keys)
        => Simple<long>(RequestType.Del, keys.ToGlideStrings());

    public static Cmd<GlideString, byte[]?> Dump(ValkeyKey key)
        => new(RequestType.Dump, [key], true, response => response?.Bytes);

    public static Cmd<long, bool> Exists(ValkeyKey key)
        => Boolean<long>(RequestType.Exists, [key]);

    public static Cmd<long, long> Exists(ValkeyKey[] keys)
        => Simple<long>(RequestType.Exists, keys.ToGlideStrings());

    public static Cmd<bool, bool> Expire(ValkeyKey key, TimeSpan? expiry, ExpireCondition condition = ExpireCondition.Always)
    {
        List<GlideString> args = [key];
        args.Add(expiry.HasValue ? ToULongMs(expiry.Value, nameof(expiry)).ToGlideString() : InstantExpiry);
        AddExpireCondition(args, condition);

        return Simple<bool>(RequestType.PExpire, [.. args]);
    }

    public static Cmd<bool, bool> Expire(ValkeyKey key, DateTimeOffset? expiry, ExpireCondition condition = ExpireCondition.Always)
    {
        List<GlideString> args = [key];
        args.Add(expiry.HasValue ? expiry.Value.ToUnixTimeMilliseconds().ToGlideString() : InstantExpiry);
        AddExpireCondition(args, condition);

        return Simple<bool>(RequestType.PExpireAt, [.. args]);
    }

    public static Cmd<long, DateTimeOffset?> ExpireTime(ValkeyKey key)
        => new(RequestType.PExpireTime, [key], true, response =>
            response is -1 or -2 ? null : DateTimeOffset.FromUnixTimeMilliseconds(response));

    public static Cmd<object, bool> Migrate(IEnumerable<ValkeyKey> keys, MigrateOptions options)
        => new(RequestType.Migrate, options.ToArgs(keys.ToGlideStrings()), false, response => response is string s && s == "OK");

    public static Cmd<bool, bool> Move(ValkeyKey key, int database)
        => Simple<bool>(RequestType.Move, [(GlideString)key, database.ToGlideString()]);

    public static Cmd<GlideString, string?> ObjectEncoding(ValkeyKey key)
        => new(RequestType.ObjectEncoding, [key], true, response => response?.ToString());

    public static Cmd<long, long?> ObjectFrequency(ValkeyKey key)
        => new(RequestType.ObjectFreq, [key], true, response => response == -1 ? null : response);

    public static Cmd<long, TimeSpan?> ObjectIdleTime(ValkeyKey key)
        => new(RequestType.ObjectIdleTime, [key], true, response => response == -1 ? null : TimeSpan.FromSeconds(response));

    public static Cmd<long, long?> ObjectRefCount(ValkeyKey key)
        => new(RequestType.ObjectRefCount, [key], true, response => response == -1 ? null : response);

    public static Cmd<bool, bool> Persist(ValkeyKey key)
        => Simple<bool>(RequestType.Persist, [key]);

    public static Cmd<GlideString, ValkeyKey?> RandomKey()
        => new(RequestType.RandomKey, [], true, response => response is null ? (ValkeyKey?)null : new ValkeyKey(response.ToString()));

    // TODO #454: Should return ValkeyValue.Ok instead of bool.
    public static Cmd<string, bool> Rename(ValkeyKey key, ValkeyKey newKey)
        => new(RequestType.Rename, [key, newKey], false, _ => true);

    public static Cmd<bool, bool> RenameIfNotExists(ValkeyKey key, ValkeyKey newKey)
        => Simple<bool>(RequestType.RenameNX, [key, newKey]);

    public static Cmd<string, ValkeyValue> Restore(ValkeyKey key, byte[] value, RestoreOptions? options = null)
    {
        List<GlideString> args = [key];

        if (options != null)
        {
            var (ttlMs, _) = options.GetTtlArgs();
            args.Add(ttlMs.ToGlideString());
        }
        else
        {
            args.Add(0.ToGlideString());
        }

        args.Add(value.ToGlideString());

        if (options != null)
        {
            args.AddRange(options.ToArgs());
        }

        return Ok(RequestType.Restore, [.. args]);
    }

    public static Cmd<object[], (string, ValkeyKey[])> Scan(string cursor, ScanOptions? options = null)
    {
        List<GlideString> args = [cursor.ToGlideString()];
        args.AddRange(ToScanArgs(options));

        return new(RequestType.Scan, [.. args], false, arr =>
        {
            string nextCursor = arr[0].ToString() ?? "0";
            ValkeyKey[] keys = [.. ((object[])arr[1]).Select(item => new ValkeyKey(item.ToString()))];
            return (nextCursor, keys);
        });
    }

    public static Cmd<long, long> SortAndStore(ValkeyKey destination, ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, ValkeyValue[]? get = null)
    {
        List<GlideString> args = [key];

        if (!by.IsNull)
        {
            args.Add(ValkeyLiterals.BY);
            args.Add(by);
        }

        if (skip != 0 || take != -1)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add(skip.ToGlideString());
            args.Add(take.ToGlideString());
        }

        if (get != null)
        {
            foreach (var pattern in get)
            {
                args.Add(ValkeyLiterals.GET);
                args.Add(pattern);
            }
        }

        if (order == Order.Descending)
        {
            args.Add(ValkeyLiterals.DESC);
        }

        if (sortType == SortType.Alphabetic)
        {
            args.Add(ValkeyLiterals.ALPHA);
        }

        args.Add(ValkeyLiterals.STORE);
        args.Add(destination);

        return Simple<long>(RequestType.Sort, [.. args]);
    }

    public static Cmd<object[], ValkeyValue[]> Sort(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, ValkeyValue[]? get = null, Version? serverVersion = null)
    {
        List<GlideString> args = [key];

        if (!by.IsNull)
        {
            args.Add(ValkeyLiterals.BY);
            args.Add(by);
        }

        if (skip != 0 || take != -1)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add(skip.ToGlideString());
            args.Add(take.ToGlideString());
        }

        if (get != null)
        {
            foreach (var pattern in get)
            {
                args.Add(ValkeyLiterals.GET);
                args.Add(pattern);
            }
        }

        if (order == Order.Descending)
        {
            args.Add(ValkeyLiterals.DESC);
        }

        if (sortType == SortType.Alphabetic)
        {
            args.Add(ValkeyLiterals.ALPHA);
        }

        var requestType = serverVersion != null && serverVersion < new Version(7, 0, 0)
            ? RequestType.Sort
            : RequestType.SortReadOnly;

        return new(requestType, [.. args], false, response => response?.Cast<GlideString>().Select(item => (ValkeyValue)item).ToArray() ?? []);
    }

    public static Cmd<object[], ValkeyValue[]> SortReadOnly(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, ValkeyValue[]? get = null)
    {
        List<GlideString> args = [key];

        if (!by.IsNull)
        {
            args.Add(ValkeyLiterals.BY);
            args.Add(by);
        }

        if (skip != 0 || take != -1)
        {
            args.Add(ValkeyLiterals.LIMIT);
            args.Add(skip.ToGlideString());
            args.Add(take.ToGlideString());
        }

        if (get != null)
        {
            foreach (var pattern in get)
            {
                args.Add(ValkeyLiterals.GET);
                args.Add(pattern);
            }
        }

        if (order == Order.Descending)
        {
            args.Add(ValkeyLiterals.DESC);
        }

        if (sortType == SortType.Alphabetic)
        {
            args.Add(ValkeyLiterals.ALPHA);
        }

        return new(RequestType.SortReadOnly, [.. args], false, response => response?.Cast<GlideString>().Select(item => (ValkeyValue)item).ToArray() ?? []);
    }

    public static Cmd<long, TimeToLiveResult> TimeToLive(ValkeyKey key)
        => new(RequestType.PTTL, [key], true, response => new TimeToLiveResult(response));

    public static Cmd<long, bool> Touch(ValkeyKey key)
        => Boolean<long>(RequestType.Touch, [key]);

    public static Cmd<long, long> Touch(ValkeyKey[] keys)
        => Simple<long>(RequestType.Touch, keys.ToGlideStrings());

    public static Cmd<GlideString, ValkeyType> Type(ValkeyKey key)
        => new(RequestType.Type, [key], false, response =>
        {
            string typeStr = response.ToString();
            return typeStr switch
            {
                "string" => ValkeyType.String,
                "list" => ValkeyType.List,
                "set" => ValkeyType.Set,
                "zset" => ValkeyType.SortedSet,
                "hash" => ValkeyType.Hash,
                "stream" => ValkeyType.Stream,
                _ => ValkeyType.None
            };
        });

    public static Cmd<long, bool> Unlink(ValkeyKey key)
        => Boolean<long>(RequestType.Unlink, [key]);

    public static Cmd<long, long> Unlink(ValkeyKey[] keys)
        => Simple<long>(RequestType.Unlink, keys.ToGlideStrings());

    public static Cmd<long, long> Wait(long numreplicas, TimeSpan timeout)
        => Simple<long>(RequestType.Wait, [numreplicas.ToGlideString(), ToULongMs(timeout, nameof(timeout)).ToGlideString()]);

    public static Cmd<object[], long[]> WaitAof(bool localAof, long numreplicas, TimeSpan timeout)
        => new(RequestType.WaitAof, [(localAof ? 1L : 0L).ToGlideString(), numreplicas.ToGlideString(), ToULongMs(timeout, nameof(timeout)).ToGlideString()], false, arr =>
            {
                long local = Convert.ToInt64(arr[0] is GlideString gs0 ? gs0.ToString() : arr[0]);
                long replicas = Convert.ToInt64(arr[1] is GlideString gs1 ? gs1.ToString() : arr[1]);
                return [local, replicas];
            });

    #endregion Command Builders
}
