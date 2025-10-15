// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchBitmapCommands.StringGetBit(ValkeyKey, long)" />
    public T StringGetBitAsync(ValkeyKey key, long offset) => AddCmd(Request.GetBitAsync(key, offset));

    /// <inheritdoc cref="IBatchBitmapCommands.StringSetBit(ValkeyKey, long, bool)" />
    public T StringSetBitAsync(ValkeyKey key, long offset, bool value) => AddCmd(Request.SetBitAsync(key, offset, value));

    IBatch IBatchBitmapCommands.StringGetBit(ValkeyKey key, long offset) => StringGetBitAsync(key, offset);
    IBatch IBatchBitmapCommands.StringSetBit(ValkeyKey key, long offset, bool value) => StringSetBitAsync(key, offset, value);
}