// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<bool> StringGetBitAsync(ValkeyKey key, long offset)
        => await Command(Request.GetBitAsync(key, offset));

    /// <inheritdoc/>
    public async Task<bool> StringSetBitAsync(ValkeyKey key, long offset, bool value)
        => await Command(Request.SetBitAsync(key, offset, value));

    /// <inheritdoc/>
    public async Task<long> StringBitCountAsync(ValkeyKey key, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte)
        => await Command(Request.BitCountAsync(key, start, end, indexType));

    /// <inheritdoc/>
    public async Task<long> StringBitPositionAsync(ValkeyKey key, bool bit, long start = 0, long end = -1, StringIndexType indexType = StringIndexType.Byte)
        => await Command(Request.BitPositionAsync(key, bit, start, end, indexType));

    /// <inheritdoc/>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second)
        => await Command(Request.BitOperationAsync(operation, destination, first, second));

    /// <inheritdoc/>
    public async Task<long> StringBitOperationAsync(Bitwise operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.BitOperationAsync(operation, destination, [.. keys]));

    /// <inheritdoc/>
    public async Task<long[]> StringBitFieldAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldSubCommand> subCommands)
    {
        BitFieldOptions.IBitFieldSubCommand[] subCommandsArray = [.. subCommands];

        // Check if all subcommands are read-only (GET operations)
        bool allReadOnly = subCommandsArray.All(cmd => cmd is BitFieldOptions.IBitFieldReadOnlySubCommand);

        if (allReadOnly)
        {
            // Convert to read-only subcommands and use BITFIELD_RO
            BitFieldOptions.IBitFieldReadOnlySubCommand[] readOnlyCommands = [.. subCommandsArray.Cast<BitFieldOptions.IBitFieldReadOnlySubCommand>()];
            return await Command(Request.BitFieldReadOnlyAsync(key, readOnlyCommands));
        }

        return await Command(Request.BitFieldAsync(key, subCommandsArray));
    }

    /// <inheritdoc/>
    public async Task<long[]> StringBitFieldReadOnlyAsync(ValkeyKey key, IEnumerable<BitFieldOptions.IBitFieldReadOnlySubCommand> subCommands)
        => await Command(Request.BitFieldReadOnlyAsync(key, [.. subCommands]));
}
