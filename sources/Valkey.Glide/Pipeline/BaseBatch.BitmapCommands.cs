// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchBitmapCommands.StringGetBit(ValkeyKey, long)" />
    public T StringGetBitAsync(ValkeyKey key, long offset) => AddCmd(Request.GetBitAsync(key, offset));

    IBatch IBatchBitmapCommands.StringGetBit(ValkeyKey key, long offset) => StringGetBitAsync(key, offset);
}