// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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

    public static Cmd<Dictionary<GlideString, object>, IDictionary<ValkeyValue, ValkeyValue>> HashGetAsync(ValkeyKey key)
        => new(RequestType.HGetAll, [key.ToGlideString()], false, dict =>
            dict.ToDictionary(kvp => (ValkeyValue)kvp.Key, kvp => (ValkeyValue)(GlideString)kvp.Value));

    public static Cmd<long, long> HashSetAsync(ValkeyKey key, KeyValuePair<ValkeyValue, ValkeyValue>[] hashFieldsAndValues)
    {
        List<GlideString> args = [key.ToGlideString()];
        AddPairs(args, hashFieldsAndValues);
        return Simple<long>(RequestType.HSet, [.. args]);
    }

    public static Cmd<long, bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => Boolean<long>(RequestType.HSet, [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);

    public static Cmd<bool, bool> HashSetNotExistsAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => Boolean<bool>(RequestType.HSetNX, [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);

    public static Cmd<long, bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField)
        => Boolean<long>(RequestType.HDel, [key.ToGlideString(), hashField.ToGlideString()]);

    public static Cmd<long, long> HashDeleteAsync(ValkeyKey key, ValkeyValue[] hashFields)
        => Simple<long>(RequestType.HDel, [key.ToGlideString(), .. hashFields.ToGlideStrings()]);

    public static Cmd<bool, bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField)
        => Boolean<bool>(RequestType.HExists, [key.ToGlideString(), hashField.ToGlideString()]);

    public static Cmd<long, long> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, long value)
        => Simple<long>(RequestType.HIncrBy, [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);

    public static Cmd<double, double> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, double value)
        => Simple<double>(RequestType.HIncrByFloat, [key.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);

    public static Cmd<object[], ISet<ValkeyValue>> HashKeysAsync(ValkeyKey key)
        => new(RequestType.HKeys, [key.ToGlideString()], false, ToValkeyValueSet);

    public static Cmd<long, long> HashLengthAsync(ValkeyKey key)
        => Simple<long>(RequestType.HLen, [key.ToGlideString()]);

    public static Cmd<long, long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField)
        => Simple<long>(RequestType.HStrlen, [key.ToGlideString(), hashField.ToGlideString()]);

    public static Cmd<object[], ICollection<ValkeyValue>> HashValuesAsync(ValkeyKey key)
        => new(RequestType.HVals, [key.ToGlideString()], false, response =>
            [.. response.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<GlideString, ValkeyValue> HashRandomFieldAsync(ValkeyKey key)
        => ToValkeyValue(RequestType.HRandField, [key.ToGlideString()], isNullable: true);

    public static Cmd<object[], ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count)
        => ObjectArrayToValkeyValueArray(RequestType.HRandField, [key.ToGlideString(), count.ToGlideString()]);

    public static Cmd<object[], KeyValuePair<ValkeyValue, ValkeyValue>?> HashRandomFieldWithValueAsync(ValkeyKey key)
    {
        GlideString[] args = [key.ToGlideString(), 1.ToGlideString(), ValkeyLiterals.WITHVALUES];
        return new(RequestType.HRandField, args, false, response =>
            response.Length > 0
                ? new KeyValuePair<ValkeyValue, ValkeyValue>((GlideString)((object[])response[0])[0], (GlideString)((object[])response[0])[1])
                : null);
    }

    public static Cmd<object[], HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count)
    {
        GlideString[] args = [key.ToGlideString(), count.ToGlideString(), ValkeyLiterals.WITHVALUES];
        return new(RequestType.HRandField, args, false, response =>
            [.. response.Select(item =>
            {
                object[] arr = (object[])item;
                return new HashEntry((GlideString)arr[0], (GlideString)arr[1]);
            })]);
    }

    public static Cmd<object[], ValkeyValue[]> HashGetAsync(
        ValkeyKey key, IEnumerable<ValkeyValue> hashFields, GetExpiryOptions options)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (options.Duration.HasValue)
        {
            args.Add(ValkeyLiterals.PX);
            args.Add(ToMilliseconds(options.Duration.Value));
        }
        else if (options.Timestamp.HasValue)
        {
            args.Add(ValkeyLiterals.PXAT);
            args.Add(options.Timestamp.Value.ToUnixTimeMilliseconds().ToGlideString());
        }
        else
        {
            args.Add(ValkeyLiterals.PERSIST);
        }

        AddFields(args, [.. hashFields]);

        return new(RequestType.HGetEx, [.. args], true, response =>
            [.. response.Select(item => item == null ? ValkeyValue.Null : (ValkeyValue)(GlideString)item)]);
    }

    public static Cmd<long, bool> HashSetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        ValkeyValue value,
        HashSetCondition condition)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (condition == HashSetCondition.OnlyIfNoneExist)
        {
            args.Add(ValkeyLiterals.FNX);
        }
        else if (condition == HashSetCondition.OnlyIfAllExist)
        {
            args.Add(ValkeyLiterals.FXX);
        }

        args.AddRange([ValkeyLiterals.FIELDS, 1.ToGlideString(), hashField.ToGlideString(), value.ToGlideString()]);
        return Boolean<long>(RequestType.HSetEx, [.. args]);
    }

    public static Cmd<long, bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetCondition condition)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (condition == HashSetCondition.OnlyIfNoneExist)
        {
            args.Add(ValkeyLiterals.FNX);
        }
        else if (condition == HashSetCondition.OnlyIfAllExist)
        {
            args.Add(ValkeyLiterals.FXX);
        }

        args.Add(ValkeyLiterals.FIELDS);
        args.Add(hashFieldsAndValues.Count().ToGlideString());
        AddPairs(args, hashFieldsAndValues);

        return Boolean<long>(RequestType.HSetEx, [.. args]);
    }

    public static Cmd<long, bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetOptions options)
    {
        List<GlideString> args = [key.ToGlideString()];

        if (options.Condition == HashSetCondition.OnlyIfNoneExist)
        {
            args.Add(ValkeyLiterals.FNX);
        }
        else if (options.Condition == HashSetCondition.OnlyIfAllExist)
        {
            args.Add(ValkeyLiterals.FXX);
        }

        if (options.Expiry is not null)
        {
            AddExpiryArgs(args, options.Expiry);
        }

        args.Add(ValkeyLiterals.FIELDS);
        args.Add(hashFieldsAndValues.Count().ToGlideString());
        AddPairs(args, hashFieldsAndValues);
        return Boolean<long>(RequestType.HSetEx, [.. args]);
    }

    public static Cmd<object[], HashPersistResult[]> HashPersistAsync(ValkeyKey key, ValkeyValue[] hashFields)
    {
        List<GlideString> args = [key.ToGlideString()];
        AddFields(args, hashFields);
        return new(RequestType.HPersist, [.. args], false, response =>
            [.. response.Select(item => (HashPersistResult)(long)item)]);
    }

    public static Cmd<object[], HashExpireResult[]> HashExpireAsync(ValkeyKey key, TimeSpan expiry, ValkeyValue[] hashFields, ExpireCondition condition)
    {
        List<GlideString> args = [key, ToMilliseconds(expiry)];

        AddExpireCondition(args, condition);
        AddFields(args, hashFields);

        return new(RequestType.HPExpire, [.. args], false, response =>
            [.. response.Select(item => (HashExpireResult)(long)item)]);
    }

    public static Cmd<object[], HashExpireResult[]> HashExpireAtAsync(ValkeyKey key, DateTimeOffset expiry, ValkeyValue[] hashFields, ExpireCondition condition)
    {
        List<GlideString> args = [key.ToGlideString(), expiry.ToUnixTimeMilliseconds().ToGlideString()];

        AddExpireCondition(args, condition);
        AddFields(args, hashFields);

        return new(RequestType.HPExpireAt, [.. args], false, response =>
            [.. response.Select(item => (HashExpireResult)(long)item)]);
    }

    public static Cmd<object[], ExpireTimeResult[]> HashExpireTimeAsync(ValkeyKey key, ValkeyValue[] hashFields)
    {
        List<GlideString> args = [key.ToGlideString()];
        AddFields(args, hashFields);
        return new(RequestType.HPExpireTime, [.. args], false, response =>
            [.. response.Select(item => new ExpireTimeResult((long)item))]);
    }

    public static Cmd<object[], TimeToLiveResult[]> HashTimeToLiveAsync(ValkeyKey key, ValkeyValue[] hashFields)
    {
        List<GlideString> args = [key.ToGlideString()];
        AddFields(args, hashFields);
        return new(RequestType.HPTtl, [.. args], false, response =>
            [.. response.Select(item => new TimeToLiveResult((long)item))]);
    }

    /// <summary>
    /// Adds the given fields to the arguments list.
    /// </summary>
    private static void AddFields(List<GlideString> args, ValkeyValue[] fields)
    {
        args.Add(ValkeyLiterals.FIELDS);
        args.Add(fields.Length.ToGlideString());
        args.AddRange(fields.ToGlideStrings());
    }

    /// <summary>
    /// Adds the given key-value pairs to the arguments list.
    /// </summary>
    private static void AddPairs(List<GlideString> args, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> pairs)
    {
        foreach (var kvp in pairs)
        {
            args.Add(kvp.Key.ToGlideString());
            args.Add(kvp.Value.ToGlideString());
        }
    }

    /// <summary>
    /// Adds the given expire condition to the arguments list.
    /// </summary>
    internal static void AddExpireCondition(List<GlideString> args, ExpireCondition condition)
    {
        switch (condition)
        {
            case ExpireCondition.Always:
                break;
            case ExpireCondition.OnlyIfNotExists:
                args.Add(ValkeyLiterals.NX);
                break;
            case ExpireCondition.OnlyIfExists:
                args.Add(ValkeyLiterals.XX);
                break;
            case ExpireCondition.OnlyIfGreaterThan:
                args.Add(ValkeyLiterals.GT);
                break;
            case ExpireCondition.OnlyIfLessThan:
                args.Add(ValkeyLiterals.LT);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(condition));
        }
    }
}
