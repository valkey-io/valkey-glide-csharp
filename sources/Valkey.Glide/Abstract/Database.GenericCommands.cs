// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    // ==================== SER-compat methods (delegate to GLIDE methods) ====================

    /// <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(ValkeyKey, CommandFlags)"/>
    public Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return DeleteAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return DeleteAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(ValkeyKey, CommandFlags)"/>
    public Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return UnlinkAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return UnlinkAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(ValkeyKey, CommandFlags)"/>
    public Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ExistsAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ExistsAsync(keys);
    }


    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, CommandFlags)"/>
    public Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ExpireAsync(key, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen, CommandFlags)"/>
    public Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ExpireAsync(key, expiry, when.ToExpireCondition());
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, CommandFlags)"/>
    public Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ExpireAsync(key, expiry.HasValue ? new DateTimeOffset(expiry.Value) : null);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen, CommandFlags)"/>
    public Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ExpireAsync(key, expiry.HasValue ? new DateTimeOffset(expiry.Value) : null, when.ToExpireCondition());
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTimeToLiveAsync(ValkeyKey, CommandFlags)"/>
    public async Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var result = await TimeToLiveAsync(key);
        return result.TimeToLive;
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTypeAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return TypeAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRenameAsync(ValkeyKey, ValkeyKey)"/>
    public async Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey)
    {
        await RenameAsync(key, newKey);
        return true;
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
            return await RenameIfNotExistsAsync(key, newKey);
        }

        await RenameAsync(key, newKey);
        return true;
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRenameNXAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return RenameIfNotExistsAsync(key, newKey);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyPersistAsync(ValkeyKey, CommandFlags)"/>
    public Task<bool> KeyPersistAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return PersistAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyDumpAsync(ValkeyKey, CommandFlags)"/>
    public Task<byte[]?> KeyDumpAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return DumpAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRestoreAsync(ValkeyKey, byte[], TimeSpan?, CommandFlags)"/>
    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await RestoreAsync(key, value, expiry.HasValue ? new RestoreOptions { Ttl = expiry } : null);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRestoreAsync(ValkeyKey, byte[], DateTime?, CommandFlags)"/>
    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, DateTime? expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await RestoreAsync(key, value, expiry.HasValue ? new RestoreOptions { ExpireAt = new DateTimeOffset(expiry.Value) } : null);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(ValkeyKey, CommandFlags)"/>
    public Task<bool> KeyTouchAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return TouchAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return TouchAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireTimeAsync(ValkeyKey, CommandFlags)"/>
    public async Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var result = await ExpireTimeAsync(key);
        return result?.DateTime;
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyEncodingAsync(ValkeyKey, CommandFlags)"/>
    public Task<string?> KeyEncodingAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ObjectEncodingAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyFrequencyAsync(ValkeyKey, CommandFlags)"/>
    public Task<long?> KeyFrequencyAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ObjectFrequencyAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyIdleTimeAsync(ValkeyKey, CommandFlags)"/>
    public Task<TimeSpan?> KeyIdleTimeAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ObjectIdleTimeAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRefCountAsync(ValkeyKey, CommandFlags)"/>
    public Task<long?> KeyRefCountAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ObjectRefCountAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool, CommandFlags)"/>
    public async Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase = -1, bool replace = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return destinationDatabase == -1
            ? await CopyAsync(sourceKey, destinationKey, replace)
            : await CopyAsync(sourceKey, destinationKey, destinationDatabase, replace);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyMoveAsync(ValkeyKey, int, CommandFlags)"/>
    public Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return MoveAsync(key, database);
    }

    /// <inheritdoc cref="IDatabaseAsync.KeyRandomAsync(CommandFlags)"/>
    public async Task<ValkeyKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await RandomKeyAsync() ?? default;
    }

    /// <inheritdoc cref="IDatabaseAsync.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public Task<ValkeyValue[]> SortAsync(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ((IGenericBaseCommands)this).SortAsync(key, skip, take, order, sortType, by, get);
    }

    /// <inheritdoc cref="IDatabaseAsync.SortAndStoreAsync(ValkeyKey, ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue}?, CommandFlags)"/>
    public Task<long> SortAndStoreAsync(ValkeyKey destination, ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ((IGenericBaseCommands)this).SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get);
    }
}
