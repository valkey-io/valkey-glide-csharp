// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

internal interface IBatchGenericCommands
{
    /// <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(ValkeyKey)" /></returns>
    IBatch KeyDelete(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch KeyDelete(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(ValkeyKey)" /></returns>
    IBatch KeyUnlink(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch KeyUnlink(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(ValkeyKey)" /></returns>
    IBatch KeyExists(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch KeyExists(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?)" /></returns>
    IBatch KeyExpire(ValkeyKey key, TimeSpan? expiry);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)" /></returns>
    IBatch KeyExpire(ValkeyKey key, TimeSpan? expiry, ExpireWhen when);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?)" /></returns>
    IBatch KeyExpire(ValkeyKey key, DateTime? expiry);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen)" /></returns>
    IBatch KeyExpire(ValkeyKey key, DateTime? expiry, ExpireWhen when);

    /// <inheritdoc cref="IDatabaseAsync.KeyTimeToLiveAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyTimeToLiveAsync(ValkeyKey)" /></returns>
    IBatch KeyTimeToLive(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyTypeAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyTypeAsync(ValkeyKey)" /></returns>
    IBatch KeyType(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyRenameAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyRenameAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch KeyRename(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IDatabaseAsync.KeyRenameNXAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyRenameNXAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch KeyRenameNX(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IDatabaseAsync.KeyPersistAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyPersistAsync(ValkeyKey)" /></returns>
    IBatch KeyPersist(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyDumpAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyDumpAsync(ValkeyKey)" /></returns>
    IBatch KeyDump(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.RestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.RestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" /></returns>
    IBatch KeyRestore(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IBaseClient.RestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.RestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)" /></returns>
    IBatch KeyRestoreDateTime(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(ValkeyKey)" /></returns>
    IBatch KeyTouch(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch KeyTouch(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireTimeAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireTimeAsync(ValkeyKey)" /></returns>
    IBatch KeyExpireTime(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyEncodingAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyEncodingAsync(ValkeyKey)" /></returns>
    IBatch KeyEncoding(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyFrequencyAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyFrequencyAsync(ValkeyKey)" /></returns>
    IBatch KeyFrequency(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyIdleTimeAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyIdleTimeAsync(ValkeyKey)" /></returns>
    IBatch KeyIdleTime(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyRefCountAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyRefCountAsync(ValkeyKey)" /></returns>
    IBatch KeyRefCount(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyCopyAsync(ValkeyKey, ValkeyKey, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyCopyAsync(ValkeyKey, ValkeyKey, bool)" /></returns>
    IBatch KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false);

    /// <inheritdoc cref="IDatabaseAsync.KeyMoveAsync(ValkeyKey, int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyMoveAsync(ValkeyKey, int)" /></returns>
    IBatch KeyMove(ValkeyKey key, int database);

    /// <inheritdoc cref="IDatabaseAsync.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool)" /></returns>
    IBatch KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false);

    /// <inheritdoc cref="IDatabaseAsync.KeyRandomAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyRandomAsync()" /></returns>
    IBatch KeyRandom();

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" /></returns>
    IBatch Sort(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

    /// <inheritdoc cref="IGenericBaseCommands.WaitAsync(long, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.WaitAsync(long, TimeSpan)" /></returns>
    IBatch Wait(long numreplicas, TimeSpan timeout);
}
