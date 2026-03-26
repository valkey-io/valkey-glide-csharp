// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Generic commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IGenericBaseCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDeleteAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDeleteAsync(keys);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyUnlinkAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyUnlinkAsync(keys);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExistsAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExistsAsync(keys);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry, when);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry, when);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyTimeToLiveAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTimeToLiveAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyTypeAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTypeAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyRenameAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRenameAsync(key, newKey);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyRenameNXAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRenameNXAsync(key, newKey);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyPersistAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyPersistAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyPersistAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyDumpAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<byte[]?> KeyDumpAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDumpAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyRestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry, RestoreOptions? restoreOptions, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await KeyRestoreAsync(key, value, expiry, restoreOptions);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyRestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task KeyRestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry, RestoreOptions? restoreOptions, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await KeyRestoreDateTimeAsync(key, value, expiry, restoreOptions);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyTouchAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTouchAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTouchAsync(keys);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireTimeAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireTimeAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyEncodingAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<string?> KeyEncodingAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyEncodingAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyFrequencyAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long?> KeyFrequencyAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyFrequencyAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyIdleTimeAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long?> KeyIdleTimeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyIdleTimeAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyRefCountAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long?> KeyRefCountAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRefCountAsync(key);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyCopyAsync(sourceKey, destinationKey, replace);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyMoveAsync(ValkeyKey, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyMoveAsync(key, database);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyCopyAsync(sourceKey, destinationKey, destinationDatabase, replace);
    }

    /// <inheritdoc cref="IGenericBaseCommands.KeyRandomAsync()"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<string?> KeyRandomAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRandomAsync();
    }

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue}?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SortAsync(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortAsync(key, skip, take, order, sortType, by, get);
    }

    /// <inheritdoc cref="IGenericBaseCommands.WaitAsync(long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> WaitAsync(long numreplicas, long timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await WaitAsync(numreplicas, timeout);
    }
}
