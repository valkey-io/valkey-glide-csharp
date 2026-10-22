// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;
using static Valkey.Glide.Internals.TimeUtils;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    #region Command Builders

    public static Cmd<long, bool> HashDelete(ValkeyKey key, ValkeyValue hashField)
        => Boolean<long>(RequestType.HDel, [key, hashField]);

    public static Cmd<long, long> HashDelete(ValkeyKey key, ValkeyValue[] hashFields)
        => Simple<long>(RequestType.HDel, [key, .. hashFields.ToGlideStrings()]);

    public static Cmd<bool, bool> HashExists(ValkeyKey key, ValkeyValue hashField)
        => Boolean<bool>(RequestType.HExists, [key, hashField]);

    public static Cmd<object[], HashExpireResult[]> HashExpire(ValkeyKey key, TimeSpan expiry, ValkeyValue[] hashFields, ExpireCondition condition)
    {
        List<GlideString> args = [key, ToULongMs(expiry, nameof(expiry)).ToGlideString()];

        args.AddRange(condition.ToArgs());
        args.AddRange(ToArgs(ValkeyLiterals.FIELDS, hashFields));

        return new(
            RequestType.HPExpire,
            [.. args],
            false,
            response =>
                [.. response.Select(item => (HashExpireResult)(long)item)]);
    }

    public static Cmd<object[], HashExpireResult[]> HashExpireAt(ValkeyKey key, DateTimeOffset expiry, ValkeyValue[] hashFields, ExpireCondition condition)
    {
        List<GlideString> args = [key, expiry.ToUnixTimeMilliseconds().ToGlideString()];

        args.AddRange(condition.ToArgs());
        args.AddRange(ToArgs(ValkeyLiterals.FIELDS, hashFields));

        return new(
            RequestType.HPExpireAt,
            [.. args],
            false,
            response =>
                [.. response.Select(item => (HashExpireResult)(long)item)]);
    }

    public static Cmd<object[], ExpireTimeResult[]> HashExpireTime(ValkeyKey key, ValkeyValue[] hashFields)
    {
        List<GlideString> args = [key, .. ToArgs(ValkeyLiterals.FIELDS, hashFields)];
        return new(
            RequestType.HPExpireTime,
            [.. args],
            false,
            response =>
                [.. response.Select(item => new ExpireTimeResult((long)item))]);
    }

    public static Cmd<GlideString, ValkeyValue> HashGet(ValkeyKey key, ValkeyValue hashField)
        => ToValkeyValue(RequestType.HGet, [key, hashField], isNullable: true);

    public static Cmd<object[], ValkeyValue[]> HashGet(ValkeyKey key, ValkeyValue[] hashFields)
    {
        GlideString[] args = [key, .. hashFields.ToGlideStrings()];
        return new(
            RequestType.HMGet,
            args,
            false,
            response => [.. response.Select(item =>
                item == null ? ValkeyValue.Null : (ValkeyValue)(GlideString)item)]);
    }

    public static Cmd<Dictionary<GlideString, object>, IDictionary<ValkeyValue, ValkeyValue>> HashGet(ValkeyKey key)
        => new(
            RequestType.HGetAll,
            [key],
            false,
            dict =>
                dict.ToDictionary(kvp => (ValkeyValue)kvp.Key, kvp => (ValkeyValue)(GlideString)kvp.Value));

    public static Cmd<object[], ValkeyValue[]> HashGet(
        ValkeyKey key, IEnumerable<ValkeyValue> hashFields, GetExpiryOptions options)
            => new(
                RequestType.HGetEx,
                [key, .. options.ToArgs(), .. ToArgs(ValkeyLiterals.FIELDS, hashFields)],
                true,
                response
                    => [.. response.Select(item => item == null ? ValkeyValue.Null : (ValkeyValue)(GlideString)item)]);

    public static Cmd<object[], ValkeyValue[]> HashGetDelete(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => ToValkeyValues(RequestType.HGetDel, [key, .. ToArgs(ValkeyLiterals.FIELDS, hashFields)]);

    public static Cmd<long, long> HashIncrementBy(ValkeyKey key, ValkeyValue hashField, long value)
        => Simple<long>(RequestType.HIncrBy, [key, hashField, value.ToGlideString()]);

    public static Cmd<double, double> HashIncrementBy(ValkeyKey key, ValkeyValue hashField, double value)
        => Simple<double>(RequestType.HIncrByFloat, [key, hashField, value.ToGlideString()]);

    public static Cmd<object[], ISet<ValkeyValue>> HashKeys(ValkeyKey key)
        => new(RequestType.HKeys, [key], false, ToValkeyValueSet);

    public static Cmd<long, long> HashLength(ValkeyKey key)
        => Simple<long>(RequestType.HLen, [key]);

    public static Cmd<object[], HashPersistResult[]> HashPersist(ValkeyKey key, ValkeyValue[] hashFields)
    {
        List<GlideString> args = [key];
        args.AddRange(ToArgs(ValkeyLiterals.FIELDS, hashFields));
        return new(
            RequestType.HPersist,
            [.. args],
            false,
            response =>
                [.. response.Select(item => (HashPersistResult)(long)item)]);
    }

    public static Cmd<GlideString, ValkeyValue> HashRandomField(ValkeyKey key)
        => ToValkeyValue(RequestType.HRandField, [key], isNullable: true);

    public static Cmd<object[], ValkeyValue[]> HashRandomFields(ValkeyKey key, long count)
        => ToValkeyValues(RequestType.HRandField, [key, count.ToGlideString()]);

    public static Cmd<object[], HashEntry[]> HashRandomFieldsWithValues(ValkeyKey key, long count)
    {
        GlideString[] args = [key, count.ToGlideString(), ValkeyLiterals.WITHVALUES];
        return new(
            RequestType.HRandField,
            args,
            false,
            response =>
                [.. response.Select(item =>
                {
                    object[] arr = (object[])item;
                    return new HashEntry((GlideString)arr[0], (GlideString)arr[1]);
                })]);
    }

    public static Cmd<object[], KeyValuePair<ValkeyValue, ValkeyValue>?> HashRandomFieldWithValue(ValkeyKey key)
    {
        GlideString[] args = [key, 1.ToGlideString(), ValkeyLiterals.WITHVALUES];
        return new(
            RequestType.HRandField,
            args,
            false,
            response =>
                response.Length > 0
                    ? new KeyValuePair<ValkeyValue, ValkeyValue>((GlideString)((object[])response[0])[0], (GlideString)((object[])response[0])[1])
                    : null);
    }

    public static Cmd<long, long> HashSet(ValkeyKey key, KeyValuePair<ValkeyValue, ValkeyValue>[] hashFieldsAndValues)
        => Simple<long>(RequestType.HSet, [key, .. ToValkeyValues(hashFieldsAndValues)]);

    public static Cmd<long, bool> HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => Boolean<long>(RequestType.HSet, [key, hashField, value]);

    public static Cmd<long, bool> HashSet(
        ValkeyKey key,
        ValkeyValue hashField,
        ValkeyValue value,
        HashSetCondition condition)
        => HashSet(key, [new KeyValuePair<ValkeyValue, ValkeyValue>(hashField, value)], condition);

    public static Cmd<long, bool> HashSet(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetCondition condition)
        => Boolean<long>(RequestType.HSetEx, [key, .. condition.ToArgs(), .. ToFields(hashFieldsAndValues)]);

    public static Cmd<long, bool> HashSet(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetOptions options)
        => Boolean<long>(RequestType.HSetEx, [key, .. options.ToArgs(), .. ToFields(hashFieldsAndValues)]);

    public static Cmd<bool, bool> HashSetNotExists(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => Boolean<bool>(RequestType.HSetNX, [key, hashField, value]);

    public static Cmd<long, long> HashStringLength(ValkeyKey key, ValkeyValue hashField)
        => Simple<long>(RequestType.HStrlen, [key, hashField]);

    public static Cmd<object[], TimeToLiveResult[]> HashTimeToLive(ValkeyKey key, ValkeyValue[] hashFields)
    {
        List<GlideString> args = [key, .. ToArgs(ValkeyLiterals.FIELDS, hashFields)];
        return new(
            RequestType.HPTtl,
            [.. args],
            false,
            response =>
                [.. response.Select(item => new TimeToLiveResult((long)item))]);
    }

    public static Cmd<object[], ICollection<ValkeyValue>> HashValues(ValkeyKey key)
        => new(
            RequestType.HVals,
            [key],
            false,
            response =>
                [.. response.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    #endregion
    #region Argument Builders

    /// <summary>
    /// Converts field-value pairs into a counted array: <c>FIELDS count field1 value1 field2 value2 ...</c>.
    /// </summary>
    /// <param name="pairs">The field-value pairs to convert.</param>
    private static GlideString[] ToFields(IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> pairs)
    {
        ValkeyValue[] values = ToValkeyValues(pairs);
        return [ValkeyLiterals.FIELDS, (values.Length / 2).ToGlideString(), .. values];
    }

    #endregion
}
