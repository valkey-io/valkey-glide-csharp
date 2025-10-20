// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchBitmapCommands.StringGetBit(ValkeyKey, long)" />
    public T StringGetBitAsync(ValkeyKey key, long offset) => AddCmd(Request.GetBitAsync(key, offset));

    /// <inheritdoc cref="IBatchBitmapCommands.StringSetBit(ValkeyKey, long, bool)" />
    public T StringSetBitAsync(ValkeyKey key, long offset, bool value) => AddCmd(Request.SetBitAsync(key, offset, value));

    /// <inheritdoc cref="IBatchBitmapCommands.StringBitCount(ValkeyKey, long, long, StringIndexType)" />
    public T StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte) => AddCmd(Request.BitCountAsync(key, start, end, indexType));

    /// <inheritdoc cref="IBatchBitmapCommands.StringBitPosition(ValkeyKey, bool, long, long, StringIndexType)" />
    public T StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte) => AddCmd(Request.BitPositionAsync(key, bit, start, end, indexType));

    /// <inheritdoc cref="IBatchBitmapCommands.StringBitOperation(Bitwise, ValkeyKey, ValkeyKey, ValkeyKey)" />
    public T StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second) => AddCmd(Request.BitOperationAsync(operation, destination, first, second));

    /// <inheritdoc cref="IBatchBitmapCommands.StringBitOperation(Bitwise, ValkeyKey, ValkeyKey[])" />
    public T StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey[] keys) => AddCmd(Request.BitOperationAsync(operation, destination, keys));

    /// <inheritdoc cref="IBatchBitmapCommands.StringBitField(ValkeyKey, BitFieldOptions.IBitFieldSubCommand[])" />
    public T StringBitFieldAsync(ValkeyKey key, BitFieldOptions.IBitFieldSubCommand[] subCommands) => AddCmd(Request.BitFieldAsync(key, subCommands));

    /// <inheritdoc cref="IBatchBitmapCommands.StringBitFieldReadOnly(ValkeyKey, BitFieldOptions.IBitFieldReadOnlySubCommand[])" />
    public T StringBitFieldReadOnlyAsync(ValkeyKey key, BitFieldOptions.IBitFieldReadOnlySubCommand[] subCommands) => AddCmd(Request.BitFieldReadOnlyAsync(key, subCommands));

    IBatch IBatchBitmapCommands.StringGetBit(ValkeyKey key, long offset) => StringGetBitAsync(key, offset);
    IBatch IBatchBitmapCommands.StringSetBit(ValkeyKey key, long offset, bool value) => StringSetBitAsync(key, offset, value);
    IBatch IBatchBitmapCommands.StringBitCount(ValkeyKey key, long start, long end, StringIndexType indexType) => StringBitCountAsync(key, start, end, indexType);
    IBatch IBatchBitmapCommands.StringBitPosition(ValkeyKey key, bool bit, long start, long end, StringIndexType indexType) => StringBitPositionAsync(key, bit, start, end, indexType);
    IBatch IBatchBitmapCommands.StringBitOperation(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second) => StringBitOperationAsync(operation, destination, first, second);
    IBatch IBatchBitmapCommands.StringBitOperation(Bitwise operation, ValkeyKey destination, ValkeyKey[] keys) => StringBitOperationAsync(operation, destination, keys);
    IBatch IBatchBitmapCommands.StringBitField(ValkeyKey key, BitFieldOptions.IBitFieldSubCommand[] subCommands) => StringBitFieldAsync(key, subCommands);
    IBatch IBatchBitmapCommands.StringBitFieldReadOnly(ValkeyKey key, BitFieldOptions.IBitFieldReadOnlySubCommand[] subCommands) => StringBitFieldReadOnlyAsync(key, subCommands);
}