// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Constants;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField)
    {
        GlideString[] args = [key.ToGlideString(), hashField.ToGlideString()];
        return ToValkeyValue(RequestType.HGet, args, true);
    }

    public static Cmd<object[], ValkeyValue[]> HashGetAsync(ValkeyKey key, ValkeyValue[] hashFields)
    {
        GlideString[] args = [key.ToGlideString(), .. hashFields.ToGlideStrings()];
        return new(RequestType.HMGet, args, false, response => [.. response.Select(item =>
            item == null ? ValkeyValue.Null : (ValkeyValue)(GlideString)item)]);
    }

    public static Cmd<Dictionary<GlideString, object>, HashEntry[]> HashGetAllAsync(ValkeyKey key)
    {
        GlideString[] args = [key.ToGlideString()];
        return DictionaryToHashEntries(RequestType.HGetAll, args);
    }

    public static Cmd<string, string> HashSetAsync(ValkeyKey key, HashEntry[] hashFields)
    {
        List<GlideString> args = [key.ToGlideString()];
        foreach (HashEntry entry in hashFields)
        {
            args.Add(entry.Name.ToGlideString());
            args.Add(entry.Value.ToGlideString());
        }
        return OK(RequestType.HMSet, [.. args]);
    }

    public static Cmd<object, bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when)
    {
        GlideString[] args = [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()];

        GuardClauses.WhenAlwaysOrNotExists(when);
        return value.IsNull
            ? Boolean<object>(RequestType.HDel, args[..2])
            : when == When.Always
                ? Boolean<object>(RequestType.HSet, args)
                : Boolean<object>(RequestType.HSetNX, args);
    }

    public static Cmd<long, bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField)
    {
        GlideString[] args = [key.ToGlideString(), hashField.ToGlideString()];
        return Boolean<long>(RequestType.HDel, args);
    }

    public static Cmd<long, long> HashDeleteAsync(ValkeyKey key, ValkeyValue[] hashFields)
    {
        GlideString[] args = [key.ToGlideString(), .. hashFields.ToGlideStrings()];
        return Simple<long>(RequestType.HDel, args);
    }

    public static Cmd<bool, bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField)
    {
        GlideString[] args = [key.ToGlideString(), hashField.ToGlideString()];
        return Boolean<bool>(RequestType.HExists, args);
    }

    public static Cmd<long, long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value)
    {
        GlideString[] args = [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()];
        return Simple<long>(RequestType.HIncrBy, args);
    }

    public static Cmd<double, double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value)
    {
        GlideString[] args = [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()];
        return Simple<double>(RequestType.HIncrByFloat, args);
    }

    public static Cmd<object[], ValkeyValue[]> HashKeysAsync(ValkeyKey key)
    {
        GlideString[] args = [key.ToGlideString()];
        return ObjectArrayToValkeyValueArray(RequestType.HKeys, args);
    }

    public static Cmd<long, long> HashLengthAsync(ValkeyKey key)
    {
        GlideString[] args = [key.ToGlideString()];
        return Simple<long>(RequestType.HLen, args);
    }

    public static Cmd<object[], (long, HashEntry[])> HashScanAsync(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0)
    {
        List<GlideString> args = [key.ToGlideString(), cursor.ToGlideString()];

        if (!pattern.IsNull)
        {
            args.AddRange([Constants.MatchKeyword.ToGlideString(), pattern.ToGlideString()]);
        }

        if (count > 0)
        {
            args.AddRange([Constants.CountKeyword.ToGlideString(), count.ToGlideString()]);
        }

        return new(RequestType.HScan, [.. args], false, arr =>
        {
            object[] scanArray = arr;
            long nextCursor = long.Parse(((GlideString)scanArray[0]).ToString());
            object[] items = (object[])scanArray[1];

            HashEntry[] entries = new HashEntry[items.Length / 2];
            for (int i = 0; i < items.Length; i += 2)
            {
                ValkeyValue field = (ValkeyValue)(GlideString)items[i];
                ValkeyValue value = (ValkeyValue)(GlideString)items[i + 1];
                entries[i / 2] = new HashEntry(field, value);
            }

            return (nextCursor, entries);
        });
    }

    public static Cmd<object[], (long, ValkeyValue[])> HashScanNoValuesAsync(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0)
    {
        List<GlideString> args = [key.ToGlideString(), cursor.ToGlideString()];

        if (!pattern.IsNull)
        {
            args.AddRange([Constants.MatchKeyword.ToGlideString(), pattern.ToGlideString()]);
        }

        if (count > 0)
        {
            args.AddRange([Constants.CountKeyword.ToGlideString(), count.ToGlideString()]);
        }

        return new(RequestType.HScan, [.. args], false, arr =>
        {
            object[] scanArray = arr;
            long nextCursor = long.Parse(((GlideString)scanArray[0]).ToString());
            object[] items = (object[])scanArray[1];

            // For HashScanNoValues, we only return the field names (every other item)
            ValkeyValue[] fields = new ValkeyValue[items.Length / 2];
            for (int i = 0; i < items.Length; i += 2)
            {
                fields[i / 2] = (ValkeyValue)(GlideString)items[i];
            }

            return (nextCursor, fields);
        });
    }

    public static Cmd<long, long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField)
    {
        GlideString[] args = [key.ToGlideString(), hashField.ToGlideString()];
        return Simple<long>(RequestType.HStrlen, args);
    }

    public static Cmd<object[], ValkeyValue[]> HashValuesAsync(ValkeyKey key)
    {
        GlideString[] args = [key.ToGlideString()];
        return ObjectArrayToValkeyValueArray(RequestType.HVals, args);
    }

    public static Cmd<GlideString, ValkeyValue> HashRandomFieldAsync(ValkeyKey key)
    {
        GlideString[] args = [key.ToGlideString()];
        return ToValkeyValue(RequestType.HRandField, args, true);
    }

    public static Cmd<object[], ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count)
    {
        GlideString[] args = [key.ToGlideString(), count.ToGlideString()];
        return ObjectArrayToValkeyValueArray(RequestType.HRandField, args);
    }

    public static Cmd<object[], HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count)
    {
        GlideString[] args = [key.ToGlideString(), count.ToGlideString(), Constants.WithValuesKeyword];
        return ObjectArrayToHashEntries(RequestType.HRandField, args);
    }

}
