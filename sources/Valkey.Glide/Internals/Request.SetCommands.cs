// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal static partial class Request
{
    public static Cmd<long, bool> SetAdd(ValkeyKey key, ValkeyValue value)
        => Boolean<long>(RequestType.SAdd, [key, value]);

    public static Cmd<long, long> SetAdd(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => Simple<long>(RequestType.SAdd, [key, .. values]);

    public static Cmd<long, bool> SetRemove(ValkeyKey key, ValkeyValue value)
        => Boolean<long>(RequestType.SRem, [key, value]);

    public static Cmd<long, long> SetRemove(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => Simple<long>(RequestType.SRem, [key, .. values]);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetMembers(ValkeyKey key)
        => new(RequestType.SMembers, [key], false, ToValkeyValueSet);

    public static Cmd<long, long> SetCard(ValkeyKey key)
        => Simple<long>(RequestType.SCard, [key]);

    public static Cmd<long, long> SetInterCard(IEnumerable<ValkeyKey> keys, long limit = 0)
    {
        List<GlideString> args = [keys.Count().ToGlideString(), .. keys];
        if (limit > 0)
        {
            args.AddRange([ValkeyLiterals.LIMIT, limit.ToGlideString()]);
        }

        return Simple<long>(RequestType.SInterCard, [.. args]);
    }

    public static Cmd<GlideString, GlideString> SetPop(ValkeyKey key)
        => Simple<GlideString>(RequestType.SPop, [key], true);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetPop(ValkeyKey key, long count)
        => new(RequestType.SPop, [key, count.ToGlideString()], false, ToValkeyValueSet);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetUnion(IEnumerable<ValkeyKey> keys)
        => new(RequestType.SUnion, keys.ToGlideStrings(), false, ToValkeyValueSet);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetInter(IEnumerable<ValkeyKey> keys)
        => new(RequestType.SInter, keys.ToGlideStrings(), false, ToValkeyValueSet);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetDiff(IEnumerable<ValkeyKey> keys)
        => new(RequestType.SDiff, keys.ToGlideStrings(), false, ToValkeyValueSet);

    public static Cmd<long, long> SetUnionStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Simple<long>(RequestType.SUnionStore, [destination, .. keys]);

    public static Cmd<long, long> SetInterStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Simple<long>(RequestType.SInterStore, [destination, .. keys]);

    public static Cmd<long, long> SetDiffStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Simple<long>(RequestType.SDiffStore, [destination, .. keys]);

    public static Cmd<bool, bool> SetIsMember(ValkeyKey key, ValkeyValue value)
        => Boolean<bool>(RequestType.SIsMember, [key, value]);

    public static Cmd<object[], bool[]> SetIsMember(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => new(RequestType.SMIsMember, [key, .. values], false, arr => [.. arr.Cast<bool>()]);

    public static Cmd<long, bool?> SetIsMemberIfExists(ValkeyKey key, ValkeyValue value)
        => OptionalBoolean(RequestType.SIsMember, [key, value, ValkeyLiterals.XX]);

    public static Cmd<GlideString, ValkeyValue> SetRandomMember(ValkeyKey key)
        => ToValkeyValue(RequestType.SRandMember, [key], isNullable: true);

    public static Cmd<object[], ValkeyValue[]> SetRandomMembers(ValkeyKey key, long count)
        => ToValkeyValues(RequestType.SRandMember, [key, count.ToGlideString()]);

    public static Cmd<bool, bool> SetMove(ValkeyKey source, ValkeyKey destination, ValkeyValue value)
        => Simple<bool>(RequestType.SMove, [source, destination, value]);

    public static Cmd<object[], (long, ValkeyValue[])> SetScan(ValkeyKey key, long cursor, ScanOptions? options = null)
        => new(RequestType.SScan, [key, cursor.ToGlideString(), .. options?.ToArgs() ?? []], false, ConvertSetScanResponse);

    private static (long cursor, ValkeyValue[] items) ConvertSetScanResponse(object[] response)
        => (ToLong(response[0]), ToValkeyValues((object[])response[1]));
}
