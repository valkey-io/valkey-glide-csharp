// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Constants;
using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<long, bool> SetAddAsync(ValkeyKey key, ValkeyValue value)
        => Boolean<long>(RequestType.SAdd, [key.ToGlideString(), value.ToGlideString()]);

    public static Cmd<long, long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => Simple<long>(RequestType.SAdd, [key.ToGlideString(), .. values.ToGlideStrings()]);

    public static Cmd<long, bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value)
        => Boolean<long>(RequestType.SRem, [key.ToGlideString(), value.ToGlideString()]);

    public static Cmd<long, long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => Simple<long>(RequestType.SRem, [key.ToGlideString(), .. values.ToGlideStrings()]);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetMembersAsync(ValkeyKey key)
        => new(RequestType.SMembers, [key.ToGlideString()], false, ToValkeyValueSet);

    public static Cmd<long, long> SetCardAsync(ValkeyKey key)
        => Simple<long>(RequestType.SCard, [key.ToGlideString()]);

    public static Cmd<long, long> SetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
    {
        List<GlideString> args = [keys.Count().ToGlideString(), .. keys.ToGlideStrings()];
        if (limit > 0)
        {
            args.AddRange([Constants.LimitKeyword, limit.ToGlideString()]);
        }
        return Simple<long>(RequestType.SInterCard, [.. args]);
    }

    public static Cmd<GlideString, GlideString> SetPopAsync(ValkeyKey key)
        => Simple<GlideString>(RequestType.SPop, [key.ToGlideString()], true);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetPopAsync(ValkeyKey key, long count)
        => new(RequestType.SPop, [key.ToGlideString(), count.ToGlideString()], false, ToValkeyValueSet);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetUnionAsync(IEnumerable<ValkeyKey> keys)
        => new(RequestType.SUnion, keys.ToGlideStrings(), false, ToValkeyValueSet);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetInterAsync(IEnumerable<ValkeyKey> keys)
        => new(RequestType.SInter, keys.ToGlideStrings(), false, ToValkeyValueSet);

    public static Cmd<HashSet<object>, ISet<ValkeyValue>> SetDiffAsync(IEnumerable<ValkeyKey> keys)
        => new(RequestType.SDiff, keys.ToGlideStrings(), false, ToValkeyValueSet);

    public static Cmd<long, long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Simple<long>(RequestType.SUnionStore, [destination.ToGlideString(), .. keys.ToGlideStrings()]);

    public static Cmd<long, long> SetInterStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Simple<long>(RequestType.SInterStore, [destination.ToGlideString(), .. keys.ToGlideStrings()]);

    public static Cmd<long, long> SetDiffStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => Simple<long>(RequestType.SDiffStore, [destination.ToGlideString(), .. keys.ToGlideStrings()]);

    public static Cmd<bool, bool> SetIsMemberAsync(ValkeyKey key, ValkeyValue value)
        => Simple<bool>(RequestType.SIsMember, [key.ToGlideString(), value.ToGlideString()]);

    public static Cmd<object[], bool[]> SetIsMemberAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => new(RequestType.SMIsMember, [key.ToGlideString(), .. values.ToGlideStrings()], false, arr => [.. arr.Cast<bool>()]);

    public static Cmd<GlideString, ValkeyValue> SetRandomMemberAsync(ValkeyKey key)
        => new(RequestType.SRandMember, [key.ToGlideString()], true, response => response is null ? ValkeyValue.Null : (ValkeyValue)response, allowConverterToHandleNull: true);

    public static Cmd<object[], ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count)
        => new(RequestType.SRandMember, [key.ToGlideString(), count.ToGlideString()], false, arr => [.. arr.Cast<GlideString>().Select(gs => (ValkeyValue)gs)]);

    public static Cmd<bool, bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value)
        => Simple<bool>(RequestType.SMove, [source.ToGlideString(), destination.ToGlideString(), value.ToGlideString()]);

    public static Cmd<object[], (long, ValkeyValue[])> SetScanAsync(ValkeyKey key, long cursor, ScanOptions? options = null)
    {
        List<GlideString> args = [key.ToGlideString(), cursor.ToGlideString()];

        ScanOptions.AppendTo(args, options);

        return new(RequestType.SScan, [.. args], false, arr =>
        {
            object[] scanArray = arr;
            long nextCursor = long.Parse(((GlideString)scanArray[0]).ToString());
            ValkeyValue[] elements = [.. ((object[])scanArray[1]).Cast<GlideString>().Select(gs => (ValkeyValue)gs)];
            return (nextCursor, elements);
        });
    }
}
