// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Constants;
using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<long, bool> DeleteAsync(ValkeyKey key)
        => Boolean<long>(RequestType.Del, [key.ToGlideString()]);

    public static Cmd<long, long> DeleteAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.Del, keys.ToGlideStrings());

    public static Cmd<long, bool> UnlinkAsync(ValkeyKey key)
        => Boolean<long>(RequestType.Unlink, [key.ToGlideString()]);

    public static Cmd<long, long> UnlinkAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.Unlink, keys.ToGlideStrings());

    public static Cmd<long, bool> ExistsAsync(ValkeyKey key)
        => Boolean<long>(RequestType.Exists, [key.ToGlideString()]);

    public static Cmd<long, long> ExistsAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.Exists, keys.ToGlideStrings());

    public static Cmd<bool, bool> ExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when = ExpireWhen.Always)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (expiry.HasValue)
        {
            args.Add(ToMilliseconds(expiry.Value).ToGlideString());
        }
        else
        {
            args.Add((-1).ToGlideString()); // Instant expiry
        }

        if (when != ExpireWhen.Always)
        {
            args.Add(when.ToLiteral().ToGlideString());
        }

        return Simple<bool>(RequestType.PExpire, [.. args]);
    }

    // TODO #269: Replace DateTime with DateTimeOffset.
    public static Cmd<bool, bool> ExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when = ExpireWhen.Always)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (expiry.HasValue)
        {
            // TODO #269: Remove implicit DateTime-to-DateTimeOffset cast once this method accepts DateTimeOffset.
            args.Add(((DateTimeOffset)expiry.Value).ToUnixTimeMilliseconds().ToGlideString());
        }
        else
        {
            args.Add((-1).ToGlideString()); // Instant expiry
        }

        if (when != ExpireWhen.Always)
        {
            args.Add(when.ToLiteral().ToGlideString());
        }

        return Simple<bool>(RequestType.PExpireAt, [.. args]);
    }

    public static Cmd<long, long> TimeToLiveAsync(ValkeyKey key)
        => new(RequestType.PTTL, [key.ToGlideString()], true, response => response);

    public static Cmd<GlideString, ValkeyType> TypeAsync(ValkeyKey key)
        => new(RequestType.Type, [key.ToGlideString()], false, response =>
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

    public static Cmd<string, bool> RenameAsync(ValkeyKey key, ValkeyKey newKey)
        => OKToBool(RequestType.Rename, [key.ToGlideString(), newKey.ToGlideString()]);

    public static Cmd<bool, bool> RenameIfNotExistsAsync(ValkeyKey key, ValkeyKey newKey)
        => Simple<bool>(RequestType.RenameNX, [key.ToGlideString(), newKey.ToGlideString()]);

    public static Cmd<bool, bool> PersistAsync(ValkeyKey key)
        => Simple<bool>(RequestType.Persist, [key.ToGlideString()]);

    public static Cmd<GlideString, byte[]?> DumpAsync(ValkeyKey key)
        => new(RequestType.Dump, [key.ToGlideString()], true, response => response?.Bytes);

    public static Cmd<string, ValkeyValue> RestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (expiry.HasValue)
        {
            args.Add(ToMilliseconds(expiry.Value).ToGlideString());
        }
        else
        {
            args.Add(0.ToGlideString());
        }

        args.Add(value.ToGlideString());

        if (restoreOptions != null)
        {
            args.AddRange(restoreOptions.ToArgs());
        }

        return Ok(RequestType.Restore, [.. args]);
    }

    // TODO #269: Replace DateTime with DateTimeOffset.
    public static Cmd<string, ValkeyValue> RestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (expiry.HasValue)
        {
            args.Add(((DateTimeOffset)expiry).ToUnixTimeMilliseconds().ToGlideString());
        }
        else
        {
            args.Add(0.ToGlideString());
        }

        args.Add(value.ToGlideString());
        args.Add(Constants.AbsttlKeyword); // By default needs to be added here

        if (restoreOptions != null)
        {
            args.AddRange(restoreOptions.ToArgs());
        }

        return Ok(RequestType.Restore, [.. args]);
    }

    public static Cmd<long, bool> TouchAsync(ValkeyKey key)
        => Boolean<long>(RequestType.Touch, [key.ToGlideString()]);

    public static Cmd<long, long> TouchAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.Touch, keys.ToGlideStrings());

    public static Cmd<bool, bool> CopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false)
    {
        List<GlideString> args = [sourceKey.ToGlideString(), destinationKey.ToGlideString()];

        if (replace)
        {
            args.Add(Constants.ReplaceKeyword);
        }

        return Simple<bool>(RequestType.Copy, [.. args]);
    }

    public static Cmd<bool, bool> CopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false)
    {
        List<GlideString> args = [sourceKey.ToGlideString(), destinationKey.ToGlideString()];

        args.AddRange([Constants.DbKeyword, destinationDatabase.ToGlideString()]);

        if (replace)
        {
            args.Add(Constants.ReplaceKeyword);
        }

        return Simple<bool>(RequestType.Copy, [.. args]);
    }

    // TODO #269: Replace DateTime with DateTimeOffset.
    public static Cmd<long, DateTime?> ExpireTimeAsync(ValkeyKey key)
        => new(RequestType.PExpireTime, [key.ToGlideString()], true, response =>
            response is -1 or -2 ? null : DateTimeOffset.FromUnixTimeMilliseconds(response).DateTime);

    public static Cmd<GlideString, string?> ObjectEncodingAsync(ValkeyKey key)
        => new(RequestType.ObjectEncoding, [key.ToGlideString()], true, response => response?.ToString());

    public static Cmd<long, long?> ObjectFrequencyAsync(ValkeyKey key)
        => new(RequestType.ObjectFreq, [key.ToGlideString()], true, response => response == -1 ? null : response);

    public static Cmd<long, TimeSpan?> ObjectIdleTimeAsync(ValkeyKey key)
        => new(RequestType.ObjectIdleTime, [key.ToGlideString()], true, response => response == -1 ? null : TimeSpan.FromSeconds(response));

    public static Cmd<long, long?> ObjectRefCountAsync(ValkeyKey key)
        => new(RequestType.ObjectRefCount, [key.ToGlideString()], true, response => response == -1 ? null : response);

    public static Cmd<GlideString, ValkeyKey?> RandomKeyAsync()
        => new(RequestType.RandomKey, [], true, response => response is null ? (ValkeyKey?)null : new ValkeyKey(response.ToString()));

    public static Cmd<object[], ValkeyValue[]> SortAsync(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, ValkeyValue[]? get = null, Version? serverVersion = null)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (!by.IsNull)
        {
            args.Add(Constants.ByKeyword);
            args.Add(by.ToGlideString());
        }

        if (skip != 0 || take != -1)
        {
            args.Add(Constants.LimitKeyword);
            args.Add(skip.ToGlideString());
            args.Add(take.ToGlideString());
        }

        if (get != null)
        {
            foreach (var pattern in get)
            {
                args.Add(Constants.GetKeyword);
                args.Add(pattern.ToGlideString());
            }
        }

        if (order == Order.Descending)
        {
            args.Add(Constants.DescKeyword);
        }

        if (sortType == SortType.Alphabetic)
        {
            args.Add(Constants.AlphaKeyword);
        }

        // Use SORT_RO for version 7.0.0+ if server version is available, otherwise use SORT_RO as default
        var requestType = serverVersion != null && serverVersion < new Version(7, 0, 0)
            ? RequestType.Sort
            : RequestType.SortReadOnly;

        return new(requestType, [.. args], false, response => response?.Cast<GlideString>().Select(item => (ValkeyValue)item).ToArray() ?? []);
    }

    public static Cmd<object[], ValkeyValue[]> SortReadOnlyAsync(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, ValkeyValue[]? get = null)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (!by.IsNull)
        {
            args.Add(Constants.ByKeyword);
            args.Add(by.ToGlideString());
        }

        if (skip != 0 || take != -1)
        {
            args.Add(Constants.LimitKeyword);
            args.Add(skip.ToGlideString());
            args.Add(take.ToGlideString());
        }

        if (get != null)
        {
            foreach (var pattern in get)
            {
                args.Add(Constants.GetKeyword);
                args.Add(pattern.ToGlideString());
            }
        }

        if (order == Order.Descending)
        {
            args.Add(Constants.DescKeyword);
        }

        if (sortType == SortType.Alphabetic)
        {
            args.Add(Constants.AlphaKeyword);
        }

        return new(RequestType.SortReadOnly, [.. args], false, response => response?.Cast<GlideString>().Select(item => (ValkeyValue)item).ToArray() ?? []);
    }

    public static Cmd<long, long> SortAndStoreAsync(ValkeyKey destination, ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, ValkeyValue[]? get = null)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (!by.IsNull)
        {
            args.Add(Constants.ByKeyword);
            args.Add(by.ToGlideString());
        }

        if (skip != 0 || take != -1)
        {
            args.Add(Constants.LimitKeyword);
            args.Add(skip.ToGlideString());
            args.Add(take.ToGlideString());
        }

        if (get != null)
        {
            foreach (var pattern in get)
            {
                args.Add(Constants.GetKeyword);
                args.Add(pattern.ToGlideString());
            }
        }

        if (order == Order.Descending)
        {
            args.Add(Constants.DescKeyword);
        }

        if (sortType == SortType.Alphabetic)
        {
            args.Add(Constants.AlphaKeyword);
        }

        args.Add(Constants.StoreKeyword);
        args.Add(destination.ToGlideString());

        return Simple<long>(RequestType.Sort, [.. args]);
    }

    public static Cmd<bool, bool> MoveAsync(ValkeyKey key, int database)
        => Simple<bool>(RequestType.Move, [key.ToGlideString(), database.ToGlideString()]);

    public static Cmd<object[], (string, ValkeyKey[])> ScanAsync(string cursor, ScanOptions? options = null)
    {
        List<GlideString> args = [cursor.ToGlideString()];

        if (options != null)
        {
            args.AddRange(options.ToArgs().Select(arg => arg.ToGlideString()));
        }

        return new(RequestType.Scan, [.. args], false, arr =>
        {
            string nextCursor = arr[0].ToString() ?? "0";
            ValkeyKey[] keys = [.. ((object[])arr[1]).Select(item => new ValkeyKey(item.ToString()))];
            return (nextCursor, keys);
        });
    }

    public static Cmd<long, long> WaitAsync(long numreplicas, TimeSpan timeout)
        => Simple<long>(RequestType.Wait, [numreplicas.ToGlideString(), ToMilliseconds(timeout).ToGlideString()]);
}
