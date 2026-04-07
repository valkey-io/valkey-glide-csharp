// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchBitmapCommands.GetBit(ValkeyKey, long)" />
    public T GetBitAsync(ValkeyKey key, long offset) => AddCmd(Request.GetBitAsync(key, offset));

    /// <inheritdoc cref="IBatchBitmapCommands.SetBit(ValkeyKey, long, bool)" />
    public T SetBitAsync(ValkeyKey key, long offset, bool value) => AddCmd(Request.SetBitAsync(key, offset, value));

    /// <inheritdoc cref="IBatchBitmapCommands.BitCount(ValkeyKey, long, long, BitmapIndexType)" />
    public T BitCountAsync(ValkeyKey key, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte) => AddCmd(Request.BitCountAsync(key, start, end, indexType));

    /// <inheritdoc cref="IBatchBitmapCommands.BitPos(ValkeyKey, bool, long, long, BitmapIndexType)" />
    public T BitPosAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte) => AddCmd(Request.BitPosAsync(key, bit, start, end, indexType));

    /// <inheritdoc cref="IBatchBitmapCommands.BitOp(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey)" />
    public T BitOpAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second) => AddCmd(Request.BitOpAsync(operation, destination, first, second));

    /// <inheritdoc cref="IBatchBitmapCommands.BitOp(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})" />
    public T BitOpAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(Request.BitOpAsync(operation, destination, [.. keys]));

    /// <inheritdoc cref="IBatchBitmapCommands.BitField(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldSubCommand})" />
    public T BitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands) => AddCmd(Request.BitFieldAsync(key, [.. subCommands]));

    /// <inheritdoc cref="IBatchBitmapCommands.BitFieldReadOnly(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldReadOnlySubCommand})" />
    public T BitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands) => AddCmd(Request.BitFieldReadOnlyAsync(key, [.. subCommands]));

    IBatch IBatchBitmapCommands.GetBit(ValkeyKey key, long offset) => GetBitAsync(key, offset);
    IBatch IBatchBitmapCommands.SetBit(ValkeyKey key, long offset, bool value) => SetBitAsync(key, offset, value);
    IBatch IBatchBitmapCommands.BitCount(ValkeyKey key, long start, long end, BitmapIndexType indexType) => BitCountAsync(key, start, end, indexType);
    IBatch IBatchBitmapCommands.BitPos(ValkeyKey key, bool bit, long start, long end, BitmapIndexType indexType) => BitPosAsync(key, bit, start, end, indexType);
    IBatch IBatchBitmapCommands.BitOp(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second) => BitOpAsync(operation, destination, first, second);
    IBatch IBatchBitmapCommands.BitOp(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys) => BitOpAsync(operation, destination, keys);
    IBatch IBatchBitmapCommands.BitField(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands) => BitFieldAsync(key, subCommands);
    IBatch IBatchBitmapCommands.BitFieldReadOnly(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands) => BitFieldReadOnlyAsync(key, subCommands);
}
