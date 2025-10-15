// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.FFI;

namespace Valkey.Glide.Internals;

internal partial class Request
{
    public static Cmd<long, bool> GetBitAsync(ValkeyKey key, long offset)
        => new(RequestType.GetBit, [key.ToGlideString(), offset.ToGlideString()], false, response => (long)response != 0);
}