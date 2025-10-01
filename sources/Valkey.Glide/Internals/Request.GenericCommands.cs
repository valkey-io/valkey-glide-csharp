// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Constants;
using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<long, bool> KeyDeleteAsync(ValkeyKey key)
        => Boolean<long>(RequestType.Del, [key.ToGlideString()]);

    public static Cmd<long, long> KeyDeleteAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.Del, keys.ToGlideStrings());

    public static Cmd<long, bool> KeyUnlinkAsync(ValkeyKey key)
        => Boolean<long>(RequestType.Unlink, [key.ToGlideString()]);

    public static Cmd<long, long> KeyUnlinkAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.Unlink, keys.ToGlideStrings());

    public static Cmd<long, bool> KeyExistsAsync(ValkeyKey key)
        => Boolean<long>(RequestType.Exists, [key.ToGlideString()]);

    public static Cmd<long, long> KeyExistsAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.Exists, keys.ToGlideStrings());

    public static Cmd<bool, bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when = ExpireWhen.Always)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (expiry.HasValue)
        {
            long milliseconds = (long)expiry.Value.TotalMilliseconds;
            if (milliseconds % 1000 == 0)
            {
                // Use seconds precision
                args.Add((milliseconds / 1000).ToGlideString());
            }
            else
            {
                // Use milliseconds precision
                args.Add(milliseconds.ToGlideString());
            }
        }
        else
        {
            args.Add((-1).ToGlideString()); // Instant expiry
        }

        if (when != ExpireWhen.Always)
        {
            args.Add(when.ToLiteral().ToGlideString());
        }

        // Choose command based on precision
        var command = expiry.HasValue && (long)expiry.Value.TotalMilliseconds % 1000 != 0
            ? RequestType.PExpire
            : RequestType.Expire;

        return Simple<bool>(command, [.. args]);
    }

    public static Cmd<bool, bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when = ExpireWhen.Always)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (expiry.HasValue)
        {
            long unixMilliseconds = ((DateTimeOffset)expiry.Value).ToUnixTimeMilliseconds();
            if (unixMilliseconds % 1000 == 0)
            {
                // Use seconds precision
                args.Add((unixMilliseconds / 1000).ToGlideString());
            }
            else
            {
                // Use milliseconds precision
                args.Add(unixMilliseconds.ToGlideString());
            }
        }
        else
        {
            args.Add((-1).ToGlideString()); // Instant expiry
        }

        if (when != ExpireWhen.Always)
        {
            args.Add(when.ToLiteral().ToGlideString());
        }

        // Choose command based on precision
        var command = expiry.HasValue && ((DateTimeOffset)expiry.Value).ToUnixTimeMilliseconds() % 1000 != 0
            ? RequestType.PExpireAt
            : RequestType.ExpireAt;

        return Simple<bool>(command, [.. args]);
    }

    public static Cmd<long, TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key)
        => new(RequestType.PTTL, [key.ToGlideString()], true, response =>
            response is -1 or -2 ? null : TimeSpan.FromMilliseconds(response));

    public static Cmd<GlideString, ValkeyType> KeyTypeAsync(ValkeyKey key)
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

    public static Cmd<string, bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey)
        => OKToBool(RequestType.Rename, [key.ToGlideString(), newKey.ToGlideString()]);

    public static Cmd<bool, bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey)
        => Simple<bool>(RequestType.RenameNX, [key.ToGlideString(), newKey.ToGlideString()]);

    public static Cmd<bool, bool> KeyPersistAsync(ValkeyKey key)
        => Simple<bool>(RequestType.Persist, [key.ToGlideString()]);

    public static Cmd<GlideString, byte[]?> KeyDumpAsync(ValkeyKey key)
        => new(RequestType.Dump, [key.ToGlideString()], true, response => response?.Bytes);

    public static Cmd<string, string> KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (expiry.HasValue)
        {
            args.Add(((long)expiry.Value.TotalMilliseconds).ToGlideString());
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

        return OK(RequestType.Restore, [.. args]);
    }

    public static Cmd<string, string> KeyRestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null)
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

        return OK(RequestType.Restore, [.. args]);
    }

    public static Cmd<long, bool> KeyTouchAsync(ValkeyKey key)
        => Boolean<long>(RequestType.Touch, [key.ToGlideString()]);

    public static Cmd<long, long> KeyTouchAsync(ValkeyKey[] keys)
        => Simple<long>(RequestType.Touch, keys.ToGlideStrings());

    public static Cmd<bool, bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false)
    {
        List<GlideString> args = [sourceKey.ToGlideString(), destinationKey.ToGlideString()];

        if (replace)
        {
            args.Add(Constants.ReplaceKeyword);
        }

        return Simple<bool>(RequestType.Copy, [.. args]);
    }

    public static Cmd<bool, bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false)
    {
        List<GlideString> args = [sourceKey.ToGlideString(), destinationKey.ToGlideString()];

        args.AddRange([Constants.DbKeyword, destinationDatabase.ToGlideString()]);

        if (replace)
        {
            args.Add(Constants.ReplaceKeyword);
        }

        return Simple<bool>(RequestType.Copy, [.. args]);
    }

    public static Cmd<long, DateTime?> KeyExpireTimeAsync(ValkeyKey key)
        => new(RequestType.PExpireTime, [key.ToGlideString()], true, response =>
            response is -1 or -2 ? null : DateTimeOffset.FromUnixTimeMilliseconds(response).DateTime);

    public static Cmd<GlideString, string?> KeyEncodingAsync(ValkeyKey key)
        => new(RequestType.ObjectEncoding, [key.ToGlideString()], true, response => response?.ToString());

    public static Cmd<long, long?> KeyFrequencyAsync(ValkeyKey key)
        => new(RequestType.ObjectFreq, [key.ToGlideString()], true, response => response == -1 ? null : response);

    public static Cmd<long, long?> KeyIdleTimeAsync(ValkeyKey key)
        => new(RequestType.ObjectIdleTime, [key.ToGlideString()], true, response => response == -1 ? null : response);

    public static Cmd<long, long?> KeyRefCountAsync(ValkeyKey key)
        => new(RequestType.ObjectRefCount, [key.ToGlideString()], true, response => response == -1 ? null : response);

    public static Cmd<GlideString, string?> KeyRandomAsync()
        => new(RequestType.RandomKey, [], true, response => response?.ToString());

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

    public static Cmd<bool, bool> KeyMoveAsync(ValkeyKey key, int database)
        => Simple<bool>(RequestType.Move, [key.ToGlideString(), database.ToGlideString()]);

    public static Cmd<object[], (long, ValkeyKey[])> ScanAsync(long cursor, ValkeyValue pattern = default, long pageSize = 0)
    {
        List<GlideString> args = [cursor.ToGlideString()];

        if (!pattern.IsNull)
        {
            args.AddRange([Constants.MatchKeyword.ToGlideString(), pattern.ToGlideString()]);
        }

        if (pageSize > 0)
        {
            args.AddRange([Constants.CountKeyword.ToGlideString(), pageSize.ToGlideString()]);
        }

        return new(RequestType.Scan, [.. args], false, arr =>
        {
            object[] scanArray = arr;
            long nextCursor = scanArray[0] is long l ? l : long.Parse(scanArray[0].ToString() ?? "0");
            ValkeyKey[] keys = [.. ((object[])scanArray[1]).Cast<GlideString>().Select(gs => new ValkeyKey(gs))];
            return (nextCursor, keys);
        });
    }

    public static Cmd<long, long> WaitAsync(long numreplicas, long timeout)
        => Simple<long>(RequestType.Wait, [numreplicas.ToGlideString(), timeout.ToGlideString()]);
}
