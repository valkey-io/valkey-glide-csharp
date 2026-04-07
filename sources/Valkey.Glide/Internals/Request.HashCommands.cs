// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Constants;
using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<GlideString, ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField)
        => ToValkeyValue(RequestType.HGet, [key.ToGlideString(), hashField.ToGlideString()], isNullable: true);

    public static Cmd<object[], ValkeyValue[]> HashGetAsync(ValkeyKey key, ValkeyValue[] hashFields)
    {
        GlideString[] args = [key.ToGlideString(), .. hashFields.ToGlideStrings()];
        return new(RequestType.HMGet, args, false, response => [.. response.Select(item =>
            item == null ? ValkeyValue.Null : (ValkeyValue)(GlideString)item)]);
    }

    public static Cmd<Dictionary<GlideString, object>, HashEntry[]> HashGetAllAsync(ValkeyKey key)
        => DictionaryToHashEntries(RequestType.HGetAll, [key.ToGlideString()]);

    public static Cmd<long, long> HashSetAsync(ValkeyKey key, KeyValuePair<ValkeyValue, ValkeyValue>[] hashFieldsAndValues)
    {
        List<GlideString> args = [key.ToGlideString()];
        foreach (var kvp in hashFieldsAndValues)
        {
            args.Add(kvp.Key.ToGlideString());
            args.Add(kvp.Value.ToGlideString());
        }
        return Simple<long>(RequestType.HSet, [.. args]);
    }

    public static Cmd<long, bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => Boolean<long>(RequestType.HSet, [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);

    public static Cmd<long, bool> HashSetNotExistsAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => Boolean<long>(RequestType.HSetNX, [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);

    public static Cmd<long, bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField)
        => Boolean<long>(RequestType.HDel, [key.ToGlideString(), hashField.ToGlideString()]);

    public static Cmd<long, long> HashDeleteAsync(ValkeyKey key, ValkeyValue[] hashFields)
        => Simple<long>(RequestType.HDel, [key.ToGlideString(), .. hashFields.ToGlideStrings()]);

    public static Cmd<bool, bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField)
        => Boolean<bool>(RequestType.HExists, [key.ToGlideString(), hashField.ToGlideString()]);

    public static Cmd<long, long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value)
        => Simple<long>(RequestType.HIncrBy, [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);

    public static Cmd<double, double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value)
        => Simple<double>(RequestType.HIncrByFloat, [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);

    public static Cmd<object[], ValkeyValue[]> HashKeysAsync(ValkeyKey key)
        => ObjectArrayToValkeyValueArray(RequestType.HKeys, [key.ToGlideString()]);

    public static Cmd<long, long> HashLengthAsync(ValkeyKey key)
        => Simple<long>(RequestType.HLen, [key.ToGlideString()]);

    public static Cmd<long, long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField)
        => Simple<long>(RequestType.HStrlen, [key.ToGlideString(), hashField.ToGlideString()]);

    public static Cmd<object[], ValkeyValue[]> HashValuesAsync(ValkeyKey key)
        => ObjectArrayToValkeyValueArray(RequestType.HVals, [key.ToGlideString()]);

    public static Cmd<GlideString, ValkeyValue> HashRandomFieldAsync(ValkeyKey key)
        => ToValkeyValue(RequestType.HRandField, [key.ToGlideString()], isNullable: true);

    public static Cmd<object[], ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count)
        => ObjectArrayToValkeyValueArray(RequestType.HRandField, [key.ToGlideString(), count.ToGlideString()]);

    public static Cmd<object[], HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count)
        => ObjectArrayToHashEntries(RequestType.HRandField, [key.ToGlideString(), count.ToGlideString(), Constants.WithValuesKeyword]);

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

    public static Cmd<long, long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options)
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
                case Commands.Options.ExpiryType.Seconds:
                    args.AddRange([Constants.ExKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case Commands.Options.ExpiryType.Milliseconds:
                    args.AddRange([Constants.PxKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case Commands.Options.ExpiryType.UnixSeconds:
                    args.AddRange([Constants.ExAtKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case Commands.Options.ExpiryType.UnixMilliseconds:
                    args.AddRange([Constants.PxAtKeyword, options.Expiry.Value!.Value.ToGlideString()]);
                    break;
                case Commands.Options.ExpiryType.KeepExisting:
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

    public static Cmd<object[], long[]> HashExpireAsync(ValkeyKey key, TimeSpan expiry, ValkeyValue[] fields, HashFieldExpirationConditionOptions options)
    {
        List<GlideString> args = [key, ToMilliseconds(expiry).ToGlideString()];

        // Add condition options before FIELDS keyword
        if (options.Condition != null)
        {
            switch (options.Condition)
            {
                case ExpireOptions.HasNoExpiry:
                    args.Add(Constants.NxKeyword);
                    break;
                case ExpireOptions.HasExistingExpiry:
                    args.Add(Constants.XxKeyword);
                    break;
                case ExpireOptions.NewExpiryGreaterThanCurrent:
                    args.Add(Constants.GtKeyword);
                    break;
                case ExpireOptions.NewExpiryLessThanCurrent:
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

    public static Cmd<object[], long[]> HashExpireAtAsync(ValkeyKey key, DateTimeOffset expiry, ValkeyValue[] fields, HashFieldExpirationConditionOptions options)
    {
        List<GlideString> args = [key.ToGlideString(), expiry.ToUnixTimeMilliseconds().ToGlideString()];

        // Add condition options before FIELDS keyword
        if (options.Condition != null)
        {
            switch (options.Condition)
            {
                case ExpireOptions.HasNoExpiry:
                    args.Add(Constants.NxKeyword);
                    break;
                case ExpireOptions.HasExistingExpiry:
                    args.Add(Constants.XxKeyword);
                    break;
                case ExpireOptions.NewExpiryGreaterThanCurrent:
                    args.Add(Constants.GtKeyword);
                    break;
                case ExpireOptions.NewExpiryLessThanCurrent:
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
