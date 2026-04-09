// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(ValkeyKey, CommandFlags)"/>
    public async Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDeleteAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDeleteAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(ValkeyKey, CommandFlags)"/>
    public async Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyUnlinkAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyUnlinkAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(ValkeyKey, CommandFlags)"/>
    public async Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExistsAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExistsAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, CommandFlags)"/>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen, CommandFlags)"/>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, CommandFlags)"/>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen, CommandFlags)"/>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTimeToLiveAsync(ValkeyKey)"/>
    public async Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key)
    {
        long ttl = await TimeToLiveAsync(key);
        return ttl is -1 or -2 ? null : TimeSpan.FromMilliseconds(ttl);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTimeToLiveAsync(ValkeyKey, CommandFlags)"/>
    public async Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTimeToLiveAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTypeAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTypeAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRenameAsync(ValkeyKey, ValkeyKey, When, CommandFlags)"/>
    public async Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        if (when == When.Exists)
        {
            throw new ArgumentException("RENAME does not support When.Exists; use When.Always or When.NotExists", nameof(when));
        }

        if (when == When.NotExists)
        {
            return await KeyRenameNXAsync(key, newKey);
        }

        await RenameAsync(key, newKey);
        return true;
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRenameNXAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRenameNXAsync(key, newKey);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyPersistAsync(ValkeyKey, CommandFlags)"/>
    public async Task<bool> KeyPersistAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyPersistAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyDumpAsync(ValkeyKey, CommandFlags)"/>
    public async Task<byte[]?> KeyDumpAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDumpAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRestoreAsync(ValkeyKey, byte[], TimeSpan?, CommandFlags)"/>
    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await KeyRestoreAsync(key, value, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRestoreAsync(ValkeyKey, byte[], TimeSpan?)"/>
    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null)
        => await RestoreAsync(key, value, expiry);

    /// <inheritdoc cref="IDatabaseAsync.KeyRestoreAsync(ValkeyKey, byte[], DateTime?)"/>
    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, DateTime? expiry)
        => await KeyRestoreDateTimeAsync(key, value, expiry);

    /// <inheritdoc cref="IDatabaseAsync.KeyRestoreAsync(ValkeyKey, byte[], DateTime?, CommandFlags)"/>
    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, DateTime? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await KeyRestoreDateTimeAsync(key, value, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(ValkeyKey, CommandFlags)"/>
    public async Task<bool> KeyTouchAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTouchAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTouchAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireTimeAsync(ValkeyKey, CommandFlags)"/>
    public async Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireTimeAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyEncodingAsync(ValkeyKey, CommandFlags)"/>
    public async Task<string?> KeyEncodingAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyEncodingAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyFrequencyAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long?> KeyFrequencyAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyFrequencyAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyIdleTimeAsync(ValkeyKey, CommandFlags)"/>
    public async Task<TimeSpan?> KeyIdleTimeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyIdleTimeAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRefCountAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long?> KeyRefCountAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRefCountAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool, CommandFlags)"/>
    public new async Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase = -1, bool replace = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return destinationDatabase == -1
            ? await CopyAsync(sourceKey, destinationKey, replace)
            : await CopyAsync(sourceKey, destinationKey, destinationDatabase, replace);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyMoveAsync(ValkeyKey, int, CommandFlags)"/>
    public async Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await MoveAsync(key, database);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRandomAsync(CommandFlags)"/>
    public new async Task<ValkeyKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await RandomKeyAsync() ?? default;
    }

    /// <inheritdoc cref="IDatabaseAsync.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public async Task<ValkeyValue[]> SortAsync(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ((IGenericBaseCommands)this).SortAsync(key, skip, take, order, sortType, by, get);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortAndStoreAsync(ValkeyKey, ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public async Task<long> SortAndStoreAsync(ValkeyKey destination, ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ((IGenericBaseCommands)this).SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get);
    }

}
