// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<long, bool> GetBitAsync(ValkeyKey key, long offset)
        => new(RequestType.GetBit, [key.ToGlideString(), offset.ToGlideString()], false, response => (long)response != 0);

    public static Cmd<long, bool> SetBitAsync(ValkeyKey key, long offset, bool value)
        => new(RequestType.SetBit, [key.ToGlideString(), offset.ToGlideString(), (value ? 1 : 0).ToGlideString()], false, response => (long)response != 0);

    public static Cmd<long, long> BitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte)
    {
        List<GlideString> args = [key.ToGlideString(), start.ToGlideString(), end.ToGlideString()];
        if (indexType != StringIndexType.Byte)
        {
            args.Add(indexType.ToLiteral().ToGlideString());
        }
        return Simple<long>(RequestType.BitCount, [.. args]);
    }

    public static Cmd<long, long> BitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte)
    {
        List<GlideString> args = [key.ToGlideString(), (bit ? 1 : 0).ToGlideString(), start.ToGlideString(), end.ToGlideString()];
        if (indexType != StringIndexType.Byte)
        {
            args.Add(indexType.ToLiteral().ToGlideString());
        }
        return Simple<long>(RequestType.BitPos, [.. args]);
    }

    public static Cmd<long, long> BitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second)
    {
        GlideString[] args = [ValkeyLiterals.Get(operation).ToGlideString(), destination.ToGlideString(), first.ToGlideString(), second.ToGlideString()];
        return Simple<long>(RequestType.BitOp, args);
    }

    public static Cmd<long, long> BitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey[] keys)
    {
        List<GlideString> args = [ValkeyLiterals.Get(operation).ToGlideString(), destination.ToGlideString()];
        args.AddRange(keys.ToGlideStrings());
        return Simple<long>(RequestType.BitOp, [.. args]);
    }
}