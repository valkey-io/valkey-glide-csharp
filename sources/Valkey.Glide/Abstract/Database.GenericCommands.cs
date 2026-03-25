// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Generic Commands with CommandFlags (SER Compatibility)

    public async Task<bool> KeyDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDeleteAsync(key);
    }

    public async Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDeleteAsync(keys);
    }

    public async Task<bool> KeyUnlinkAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyUnlinkAsync(key);
    }

    public async Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyUnlinkAsync(keys);
    }

    public async Task<bool> KeyExistsAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExistsAsync(key);
    }

    public async Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExistsAsync(keys);
    }

    public async Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry);
    }

    public async Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry, when);
    }

    public async Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry);
    }

    public async Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireAsync(key, expiry, when);
    }

    public async Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTimeToLiveAsync(key);
    }

    public async Task<ValkeyType> KeyTypeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTypeAsync(key);
    }

    public async Task<bool> KeyRenameAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRenameAsync(key, newKey);
    }

    public async Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRenameNXAsync(key, newKey);
    }

    public async Task<bool> KeyPersistAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyPersistAsync(key);
    }

    public async Task<byte[]?> KeyDumpAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyDumpAsync(key);
    }

    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry, RestoreOptions? restoreOptions, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await KeyRestoreAsync(key, value, expiry, restoreOptions);
    }

    public async Task KeyRestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry, RestoreOptions? restoreOptions, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await KeyRestoreDateTimeAsync(key, value, expiry, restoreOptions);
    }

    public async Task<bool> KeyTouchAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTouchAsync(key);
    }

    public async Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyTouchAsync(keys);
    }

    public async Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyExpireTimeAsync(key);
    }

    public async Task<string?> KeyEncodingAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyEncodingAsync(key);
    }

    public async Task<long?> KeyFrequencyAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyFrequencyAsync(key);
    }

    public async Task<long?> KeyIdleTimeAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyIdleTimeAsync(key);
    }

    public async Task<long?> KeyRefCountAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRefCountAsync(key);
    }

    public async Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyCopyAsync(sourceKey, destinationKey, replace);
    }

    public async Task<bool> KeyMoveAsync(ValkeyKey key, int database, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyMoveAsync(key, database);
    }

    public async Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyCopyAsync(sourceKey, destinationKey, destinationDatabase, replace);
    }

    public async Task<string?> KeyRandomAsync(CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await KeyRandomAsync();
    }

    public async Task<ValkeyValue[]> SortAsync(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SortAsync(key, skip, take, order, sortType, by, get);
    }

    public async Task<long> WaitAsync(long numreplicas, long timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await WaitAsync(numreplicas, timeout);
    }

    #endregion
}
