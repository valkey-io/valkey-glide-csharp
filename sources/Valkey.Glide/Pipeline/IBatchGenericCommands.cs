// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

internal interface IBatchGenericCommands
{
    /// <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyDelete(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(IEnumerable{ValkeyKey}, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyDeleteAsync(IEnumerable{ValkeyKey}, CommandFlags)" /></returns>
    IBatch KeyDelete(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyUnlink(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(IEnumerable{ValkeyKey}, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyUnlinkAsync(IEnumerable{ValkeyKey}, CommandFlags)" /></returns>
    IBatch KeyUnlink(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyExists(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(IEnumerable{ValkeyKey}, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExistsAsync(IEnumerable{ValkeyKey}, CommandFlags)" /></returns>
    IBatch KeyExists(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, CommandFlags)" /></returns>
    IBatch KeyExpire(ValkeyKey key, TimeSpan? expiry);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, TimeSpan?, ExpireWhen, CommandFlags)" /></returns>
    IBatch KeyExpire(ValkeyKey key, TimeSpan? expiry, ExpireWhen when);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, CommandFlags)" /></returns>
    IBatch KeyExpire(ValkeyKey key, DateTime? expiry);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireAsync(ValkeyKey, DateTime?, ExpireWhen, CommandFlags)" /></returns>
    IBatch KeyExpire(ValkeyKey key, DateTime? expiry, ExpireWhen when);

    /// <inheritdoc cref="IDatabaseAsync.KeyTimeToLiveAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyTimeToLiveAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyTimeToLive(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyTypeAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyTypeAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyType(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyRenameAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyRenameAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch KeyRename(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IDatabaseAsync.KeyRenameNXAsync(ValkeyKey, ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyRenameNXAsync(ValkeyKey, ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyRenameNX(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IDatabaseAsync.KeyPersistAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyPersistAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyPersist(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyDumpAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyDumpAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyDump(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.RestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.RestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" /></returns>
    IBatch KeyRestore(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IBaseClient.RestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.RestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)" /></returns>
    IBatch KeyRestoreDateTime(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyTouch(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(IEnumerable{ValkeyKey}, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyTouchAsync(IEnumerable{ValkeyKey}, CommandFlags)" /></returns>
    IBatch KeyTouch(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IDatabaseAsync.KeyExpireTimeAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyExpireTimeAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyExpireTime(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyEncodingAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyEncodingAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyEncoding(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyFrequencyAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyFrequencyAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyFrequency(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyIdleTimeAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyIdleTimeAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyIdleTime(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyRefCountAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyRefCountAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch KeyRefCount(ValkeyKey key);

    /// <inheritdoc cref="IDatabaseAsync.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyCopyAsync(ValkeyKey, ValkeyKey, int, bool, CommandFlags)" /></returns>
    IBatch KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase = -1, bool replace = false);

    /// <inheritdoc cref="IDatabaseAsync.KeyMoveAsync(ValkeyKey, int, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyMoveAsync(ValkeyKey, int, CommandFlags)" /></returns>
    IBatch KeyMove(ValkeyKey key, int database);

    /// <inheritdoc cref="IDatabaseAsync.KeyRandomAsync(CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.KeyRandomAsync(CommandFlags)" /></returns>
    IBatch KeyRandom();

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" /></returns>
    IBatch Sort(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

    /// <inheritdoc cref="IBaseClient.WaitAsync(long, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.WaitAsync(long, TimeSpan)" /></returns>
    IBatch Wait(long numreplicas, TimeSpan timeout);
}
