// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Constants;
using Valkey.Glide.Commands.Options;

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

    public static Cmd<object[], (long, T)> HashScanAsync<T>(ValkeyKey key, long cursor, ValkeyValue pattern, long count, bool includeValues = true)
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
            object[] items = (object[])scanArray[1]; // This array will always have an even-length

            if (includeValues)
            {
                // Return HashEntry[] with both field names and values
                HashEntry[] entries = new HashEntry[items.Length / 2];
                for (int i = 0; i < items.Length; i += 2)
                {
                    ValkeyValue field = (GlideString)items[i];
                    ValkeyValue value = (GlideString)items[i + 1];
                    entries[i / 2] = new HashEntry(field, value);
                }
                return (nextCursor, (T)(object)entries);
            }
            else
            {
                // Return ValkeyValue[] with only field names
                ValkeyValue[] fields = new ValkeyValue[items.Length / 2];
                for (int i = 0; i < items.Length; i += 2)
                {
                    fields[i / 2] = (GlideString)items[i];
                }
                return (nextCursor, (T)(object)fields);
            }
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

    public static Cmd<object[], ValkeyValue[]?> HashGetExAsync(ValkeyKey key, ValkeyValue[] fields, HashGetExOptions options)
    {
        List<GlideString> args = [key.ToGlideString()];

        // Add expiry options before FIELDS keyword
        if (options.Expiry != null)
        {
            switch (options.Expiry.Type)
            {
                case HGetExExpiryType.Seconds:
                    args.AddRange([Constants.ExKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case HGetExExpiryType.Milliseconds:
                    args.AddRange([Constants.PxKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case HGetExExpiryType.UnixSeconds:
                    args.AddRange([Constants.ExAtKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case HGetExExpiryType.UnixMilliseconds:
                    args.AddRange([Constants.PxAtKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case HGetExExpiryType.Persist:
                    args.Add(Constants.PersistKeyword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HGetEx, [.. args], true, response =>
            response == null ? null : [.. response.Select(item =>
                item == null ? ValkeyValue.Null : (ValkeyValue)(GlideString)item)], allowConverterToHandleNull: true);
    }

    public static Cmd<long, long> HashSetExAsync(ValkeyKey key, Dictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options)
    {
        List<GlideString> args = [key.ToGlideString()];

        // Add field existence condition options first (FNX/FXX)
        if (options.OnlyIfNoneExist)
        {
            args.Add(Constants.FnxKeyword);
        }
        else if (options.OnlyIfAllExist)
        {
            args.Add(Constants.FxxKeyword);
        }

        // Add expiry options after conditional options
        if (options.Expiry != null)
        {
            switch (options.Expiry.Type)
            {
                case ExpiryType.Seconds:
                    args.AddRange([Constants.ExKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case ExpiryType.Milliseconds:
                    args.AddRange([Constants.PxKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case ExpiryType.KeepExisting:
                    args.Add(Constants.KeepTtlKeyword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fieldValueMap.Count.ToGlideString());

#pragma warning disable IDE0008 // Use explicit type
        // Add field-value pairs
        foreach (var kvp in fieldValueMap)
        {
            args.Add(kvp.Key.ToGlideString());
            args.Add(kvp.Value.ToGlideString());
        }
#pragma warning restore IDE0008 // Use explicit type

        return Simple<long>(RequestType.HSetEx, [.. args]);
    }

    public static Cmd<object[], long[]> HashPersistAsync(ValkeyKey key, ValkeyValue[] fields)
    {
        List<GlideString> args = [key.ToGlideString()];

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HPersist, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

    public static Cmd<object[], long[]> HashExpireAsync(ValkeyKey key, long seconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options)
    {
        List<GlideString> args = [key.ToGlideString(), seconds.ToGlideString()];

        // Add condition options before FIELDS keyword
        if (options.Condition != null)
        {
            switch (options.Condition)
            {
                case ExpireOptions.HAS_NO_EXPIRY:
                    args.Add(Constants.NxKeyword);
                    break;
                case ExpireOptions.HAS_EXISTING_EXPIRY:
                    args.Add(Constants.XxKeyword);
                    break;
                case ExpireOptions.NEW_EXPIRY_GREATER_THAN_CURRENT:
                    args.Add(Constants.GtKeyword);
                    break;
                case ExpireOptions.NEW_EXPIRY_LESS_THAN_CURRENT:
                    args.Add(Constants.LtKeyword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HExpire, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

    public static Cmd<object[], long[]> HashPExpireAsync(ValkeyKey key, long milliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options)
    {
        List<GlideString> args = [key.ToGlideString(), milliseconds.ToGlideString()];

        // Add condition options before FIELDS keyword
        if (options.Condition != null)
        {
            switch (options.Condition)
            {
                case ExpireOptions.HAS_NO_EXPIRY:
                    args.Add(Constants.NxKeyword);
                    break;
                case ExpireOptions.HAS_EXISTING_EXPIRY:
                    args.Add(Constants.XxKeyword);
                    break;
                case ExpireOptions.NEW_EXPIRY_GREATER_THAN_CURRENT:
                    args.Add(Constants.GtKeyword);
                    break;
                case ExpireOptions.NEW_EXPIRY_LESS_THAN_CURRENT:
                    args.Add(Constants.LtKeyword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HPExpire, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

    public static Cmd<object[], long[]> HashExpireAtAsync(ValkeyKey key, long unixSeconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options)
    {
        List<GlideString> args = [key.ToGlideString(), unixSeconds.ToGlideString()];

        // Add condition options before FIELDS keyword
        if (options.Condition != null)
        {
            switch (options.Condition)
            {
                case ExpireOptions.HAS_NO_EXPIRY:
                    args.Add(Constants.NxKeyword);
                    break;
                case ExpireOptions.HAS_EXISTING_EXPIRY:
                    args.Add(Constants.XxKeyword);
                    break;
                case ExpireOptions.NEW_EXPIRY_GREATER_THAN_CURRENT:
                    args.Add(Constants.GtKeyword);
                    break;
                case ExpireOptions.NEW_EXPIRY_LESS_THAN_CURRENT:
                    args.Add(Constants.LtKeyword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HExpireAt, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

    public static Cmd<object[], long[]> HashPExpireAtAsync(ValkeyKey key, long unixMilliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options)
    {
        List<GlideString> args = [key.ToGlideString(), unixMilliseconds.ToGlideString()];

        // Add condition options before FIELDS keyword
        if (options.Condition != null)
        {
            switch (options.Condition)
            {
                case ExpireOptions.HAS_NO_EXPIRY:
                    args.Add(Constants.NxKeyword);
                    break;
                case ExpireOptions.HAS_EXISTING_EXPIRY:
                    args.Add(Constants.XxKeyword);
                    break;
                case ExpireOptions.NEW_EXPIRY_GREATER_THAN_CURRENT:
                    args.Add(Constants.GtKeyword);
                    break;
                case ExpireOptions.NEW_EXPIRY_LESS_THAN_CURRENT:
                    args.Add(Constants.LtKeyword);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HPExpireAt, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

    public static Cmd<object[], long[]> HashExpireTimeAsync(ValkeyKey key, ValkeyValue[] fields)
    {
        List<GlideString> args = [key.ToGlideString()];

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HExpireTime, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

    public static Cmd<object[], long[]> HashPExpireTimeAsync(ValkeyKey key, ValkeyValue[] fields)
    {
        List<GlideString> args = [key.ToGlideString()];

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HPExpireTime, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

    public static Cmd<object[], long[]> HashTtlAsync(ValkeyKey key, ValkeyValue[] fields)
    {
        List<GlideString> args = [key.ToGlideString()];

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HTtl, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

    public static Cmd<object[], long[]> HashPTtlAsync(ValkeyKey key, ValkeyValue[] fields)
    {
        List<GlideString> args = [key.ToGlideString()];

        // Add FIELDS keyword and field count
        args.Add(Constants.FieldsKeyword);
        args.Add(fields.Length.ToGlideString());

        // Add field names
        args.AddRange(fields.ToGlideStrings());

        return new(RequestType.HPTtl, [.. args], false, response =>
            [.. response.Select(item => (long)item)]);
    }

}
