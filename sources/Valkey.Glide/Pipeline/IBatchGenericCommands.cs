// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

internal interface IBatchGenericCommands
{
    /// <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(ValkeyKey)" /></returns>
    IBatch KeyDelete(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyDeleteAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch KeyDelete(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(ValkeyKey)" /></returns>
    IBatch KeyUnlink(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyUnlinkAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch KeyUnlink(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(ValkeyKey)" /></returns>
    IBatch KeyExists(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyExistsAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch KeyExists(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?)" /></returns>
    IBatch KeyExpire(ValkeyKey key, TimeSpan? expiry);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen)" /></returns>
    IBatch KeyExpire(ValkeyKey key, TimeSpan? expiry, ExpireWhen when);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?)" /></returns>
    IBatch KeyExpire(ValkeyKey key, DateTime? expiry);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen)" /></returns>
    IBatch KeyExpire(ValkeyKey key, DateTime? expiry, ExpireWhen when);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTimeToLiveAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyTimeToLiveAsync(ValkeyKey)" /></returns>
    IBatch KeyTimeToLive(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTypeAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyTypeAsync(ValkeyKey)" /></returns>
    IBatch KeyType(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRenameAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyRenameAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch KeyRename(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRenameNXAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyRenameNXAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch KeyRenameNX(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IGenericBaseCommands.KeyPersistAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyPersistAsync(ValkeyKey)" /></returns>
    IBatch KeyPersist(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyDumpAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyDumpAsync(ValkeyKey)" /></returns>
    IBatch KeyDump(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyRestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" /></returns>
    IBatch KeyRestore(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyRestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)" /></returns>
    IBatch KeyRestoreDateTime(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(ValkeyKey)" /></returns>
    IBatch KeyTouch(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyTouchAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch KeyTouch(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IGenericBaseCommands.KeyExpireTimeAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyExpireTimeAsync(ValkeyKey)" /></returns>
    IBatch KeyExpireTime(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyEncodingAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyEncodingAsync(ValkeyKey)" /></returns>
    IBatch KeyEncoding(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyFrequencyAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyFrequencyAsync(ValkeyKey)" /></returns>
    IBatch KeyFrequency(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyIdleTimeAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyIdleTimeAsync(ValkeyKey)" /></returns>
    IBatch KeyIdleTime(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRefCountAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyRefCountAsync(ValkeyKey)" /></returns>
    IBatch KeyRefCount(ValkeyKey key);

    /// <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, bool)" /></returns>
    IBatch KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false);

    /// <inheritdoc cref="IGenericBaseCommands.KeyMoveAsync(ValkeyKey, int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyMoveAsync(ValkeyKey, int)" /></returns>
    IBatch KeyMove(ValkeyKey key, int database);

    /// <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool)" /></returns>
    IBatch KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false);

    /// <inheritdoc cref="IGenericBaseCommands.KeyRandomAsync()" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.KeyRandomAsync()" /></returns>
    IBatch KeyRandom();

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" /></returns>
    IBatch Sort(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

    /// <inheritdoc cref="IGenericBaseCommands.WaitAsync(long, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.WaitAsync(long, TimeSpan)" /></returns>
    IBatch Wait(long numreplicas, TimeSpan timeout);
}
