// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.GetBitAsync(ValkeyKey, long)"/>
    public async Task<bool> GetBitAsync(ValkeyKey key, long offset)
        => await Command(Request.GetBitAsync(key, offset));

    /// <inheritdoc cref="IBaseClient.SetBitAsync(ValkeyKey, long, bool)"/>
    public async Task<bool> SetBitAsync(ValkeyKey key, long offset, bool value)
        => await Command(Request.SetBitAsync(key, offset, value));

    /// <inheritdoc cref="IBaseClient.BitCountAsync(ValkeyKey, long, long, BitmapIndexType)"/>
    public async Task<long> BitCountAsync(ValkeyKey key, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte)
        => await Command(Request.BitCountAsync(key, start, end, indexType));

    /// <inheritdoc cref="IBaseClient.BitCountAsync(ValkeyKey, BitOffsetOptions?)"/>
    public async Task<long> BitCountAsync(ValkeyKey key, BitOffsetOptions? options)
    {
        options ??= new BitOffsetOptions();
        return await BitCountAsync(key, options.Start, options.End, options.IndexType);
    }

    /// <inheritdoc cref="IBaseClient.BitPosAsync(ValkeyKey, bool, long, long, BitmapIndexType)"/>
    public async Task<long> BitPosAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, BitmapIndexType indexType = BitmapIndexType.Byte)
        => await Command(Request.BitPosAsync(key, bit, start, end, indexType));

    /// <inheritdoc cref="IBaseClient.BitPosAsync(ValkeyKey, bool, BitOffsetOptions?)"/>
    public async Task<long> BitPosAsync(ValkeyKey key, bool bit, BitOffsetOptions? options)
    {
        options ??= new BitOffsetOptions();
        return await BitPosAsync(key, bit, options.Start, options.End, options.IndexType);
    }

    /// <inheritdoc cref="IBaseClient.BitOpAsync(Bitwise, ValkeyKey, IEnumerable{ValkeyKey})"/>
    public async Task<long> BitOpAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.BitOpAsync(operation, destination, [.. keys]));

    /// <inheritdoc cref="IBaseClient.BitFieldAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldSubCommand})"/>
    public async Task<long?[]> BitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands)
    {
        BitFieldOptions.IBitFieldSubCommand[] subCommandsArray = [.. subCommands];

        // Check if all subcommands are read-only (GET operations)
        bool allReadOnly = subCommandsArray.All(cmd => cmd is BitFieldOptions.IBitFieldReadOnlySubCommand);

        if (allReadOnly)
        {
            // Convert to read-only subcommands and use BITFIELD_RO
            // Note: BITFIELD_RO never returns null (no overflow possible with GET)
            BitFieldOptions.IBitFieldReadOnlySubCommand[] readOnlyCommands = [.. subCommandsArray.Cast<BitFieldOptions.IBitFieldReadOnlySubCommand>()];
            long[] results = await Command(Request.BitFieldReadOnlyAsync(key, readOnlyCommands));
            return [.. results.Select(r => (long?)r)];
        }

        return await Command(Request.BitFieldAsync(key, subCommandsArray));
    }

    /// <inheritdoc cref="IBaseClient.BitFieldAsync(ValkeyKey, BitFieldOptions.IBitFieldSubCommand)"/>
    public async Task<long?> BitFieldAsync(ValkeyKey key, BitFieldOptions.IBitFieldSubCommand subCommand)
    {
        long?[] results = await BitFieldAsync(key, [subCommand]);
        return results.Length > 0 ? results[0] : null;
    }

    /// <inheritdoc cref="IBaseClient.BitFieldReadOnlyAsync(ValkeyKey, IEnumerable{BitFieldOptions.IBitFieldReadOnlySubCommand})"/>
    public async Task<long[]> BitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands)
        => await Command(Request.BitFieldReadOnlyAsync(key, [.. subCommands]));

    /// <inheritdoc cref="IBaseClient.BitFieldReadOnlyAsync(ValkeyKey, BitFieldOptions.IBitFieldReadOnlySubCommand)"/>
    public async Task<long> BitFieldReadOnlyAsync(ValkeyKey key, BitFieldOptions.IBitFieldReadOnlySubCommand subCommand)
    {
        long[] results = await BitFieldReadOnlyAsync(key, [subCommand]);
        return results.Length > 0 ? results[0] : 0;
    }
}
