// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Generic commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchGenericCommands.KeyDelete(ValkeyKey)" />
    public T KeyDelete(ValkeyKey key) => AddCmd(KeyDeleteAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyDelete(ValkeyKey[])" />
    public T KeyDelete(ValkeyKey[] keys) => AddCmd(KeyDeleteAsync(keys));

    /// <inheritdoc cref="IBatchGenericCommands.KeyUnlink(ValkeyKey)" />
    public T KeyUnlink(ValkeyKey key) => AddCmd(KeyUnlinkAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyUnlink(ValkeyKey[])" />
    public T KeyUnlink(ValkeyKey[] keys) => AddCmd(KeyUnlinkAsync(keys));

    /// <inheritdoc cref="IBatchGenericCommands.KeyExists(ValkeyKey)" />
    public T KeyExists(ValkeyKey key) => AddCmd(KeyExistsAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyExists(ValkeyKey[])" />
    public T KeyExists(ValkeyKey[] keys) => AddCmd(KeyExistsAsync(keys));

    /// <inheritdoc cref="IBatchGenericCommands.KeyExpire(ValkeyKey, TimeSpan?)" />
    public T KeyExpire(ValkeyKey key, TimeSpan? expiry) => AddCmd(KeyExpireAsync(key, expiry));

    /// <inheritdoc cref="IBatchGenericCommands.KeyExpire(ValkeyKey, TimeSpan?, ExpireWhen)" />
    public T KeyExpire(ValkeyKey key, TimeSpan? expiry, ExpireWhen when) => AddCmd(KeyExpireAsync(key, expiry, when));

    /// <inheritdoc cref="IBatchGenericCommands.KeyExpire(ValkeyKey, DateTime?)" />
    public T KeyExpire(ValkeyKey key, DateTime? expiry) => AddCmd(KeyExpireAsync(key, expiry));

    /// <inheritdoc cref="IBatchGenericCommands.KeyExpire(ValkeyKey, DateTime?, ExpireWhen)" />
    public T KeyExpire(ValkeyKey key, DateTime? expiry, ExpireWhen when) => AddCmd(KeyExpireAsync(key, expiry, when));

    /// <inheritdoc cref="IBatchGenericCommands.KeyTimeToLive(ValkeyKey)" />
    public T KeyTimeToLive(ValkeyKey key) => AddCmd(KeyTimeToLiveAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyType(ValkeyKey)" />
    public T KeyType(ValkeyKey key) => AddCmd(KeyTypeAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyRename(ValkeyKey, ValkeyKey)" />
    public T KeyRename(ValkeyKey key, ValkeyKey newKey) => AddCmd(KeyRenameAsync(key, newKey));

    /// <inheritdoc cref="IBatchGenericCommands.KeyRenameNX(ValkeyKey, ValkeyKey)" />
    public T KeyRenameNX(ValkeyKey key, ValkeyKey newKey) => AddCmd(KeyRenameNXAsync(key, newKey));

    /// <inheritdoc cref="IBatchGenericCommands.KeyPersist(ValkeyKey)" />
    public T KeyPersist(ValkeyKey key) => AddCmd(KeyPersistAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyDump(ValkeyKey)" />
    public T KeyDump(ValkeyKey key) => AddCmd(KeyDumpAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyRestore(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" />
    public T KeyRestore(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null) => AddCmd(KeyRestoreAsync(key, value, expiry, restoreOptions));

    /// <inheritdoc cref="IBatchGenericCommands.KeyRestoreDateTime(ValkeyKey, byte[], DateTime?, RestoreOptions?)" />
    public T KeyRestoreDateTime(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null) => AddCmd(KeyRestoreDateTimeAsync(key, value, expiry, restoreOptions));

    /// <inheritdoc cref="IBatchGenericCommands.KeyTouch(ValkeyKey)" />
    public T KeyTouch(ValkeyKey key) => AddCmd(KeyTouchAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyTouch(ValkeyKey[])" />
    public T KeyTouch(ValkeyKey[] keys) => AddCmd(KeyTouchAsync(keys));

    /// <inheritdoc cref="IBatchGenericCommands.KeyExpireTime(ValkeyKey)" />
    public T KeyExpireTime(ValkeyKey key) => AddCmd(KeyExpireTimeAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyEncoding(ValkeyKey)" />
    public T KeyEncoding(ValkeyKey key) => AddCmd(KeyEncodingAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyFrequency(ValkeyKey)" />
    public T KeyFrequency(ValkeyKey key) => AddCmd(KeyFrequencyAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyIdleTime(ValkeyKey)" />
    public T KeyIdleTime(ValkeyKey key) => AddCmd(KeyIdleTimeAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyRefCount(ValkeyKey)" />
    public T KeyRefCount(ValkeyKey key) => AddCmd(KeyRefCountAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.KeyCopy(ValkeyKey, ValkeyKey, bool)" />
    public T KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false) => AddCmd(KeyCopyAsync(sourceKey, destinationKey, replace));

    /// <inheritdoc cref="IBatchGenericCommands.KeyMove(ValkeyKey, int)" />
    public T KeyMove(ValkeyKey key, int database) => AddCmd(KeyMoveAsync(key, database));

    /// <inheritdoc cref="IBatchGenericCommands.KeyCopy(ValkeyKey, ValkeyKey, int, bool)" />
    public T KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false) => AddCmd(KeyCopyAsync(sourceKey, destinationKey, destinationDatabase, replace));
    /// <inheritdoc cref="IBatchGenericCommands.KeyRandom()" />
    public T KeyRandom() => AddCmd(KeyRandomAsync());

    /// <inheritdoc cref="IBatchGenericCommands.Sort(ValkeyKey, long, long, Order, SortType, ValkeyValue, ValkeyValue[])" />
    public T Sort(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, ValkeyValue[]? get = null) => AddCmd(SortAsync(key, skip, take, order, sortType, by, get, null));

    /// <inheritdoc cref="IBatchGenericCommands.Wait(long, long)" />
    public T Wait(long numreplicas, long timeout) => AddCmd(WaitAsync(numreplicas, timeout));

    // Explicit interface implementations for IBatchGenericCommands
    IBatch IBatchGenericCommands.KeyDelete(ValkeyKey key) => KeyDelete(key);
    IBatch IBatchGenericCommands.KeyDelete(ValkeyKey[] keys) => KeyDelete(keys);
    IBatch IBatchGenericCommands.KeyUnlink(ValkeyKey key) => KeyUnlink(key);
    IBatch IBatchGenericCommands.KeyUnlink(ValkeyKey[] keys) => KeyUnlink(keys);
    IBatch IBatchGenericCommands.KeyExists(ValkeyKey key) => KeyExists(key);
    IBatch IBatchGenericCommands.KeyExists(ValkeyKey[] keys) => KeyExists(keys);
    IBatch IBatchGenericCommands.KeyExpire(ValkeyKey key, TimeSpan? expiry) => KeyExpire(key, expiry);
    IBatch IBatchGenericCommands.KeyExpire(ValkeyKey key, TimeSpan? expiry, ExpireWhen when) => KeyExpire(key, expiry, when);
    IBatch IBatchGenericCommands.KeyExpire(ValkeyKey key, DateTime? expiry) => KeyExpire(key, expiry);
    IBatch IBatchGenericCommands.KeyExpire(ValkeyKey key, DateTime? expiry, ExpireWhen when) => KeyExpire(key, expiry, when);
    IBatch IBatchGenericCommands.KeyTimeToLive(ValkeyKey key) => KeyTimeToLive(key);
    IBatch IBatchGenericCommands.KeyType(ValkeyKey key) => KeyType(key);
    IBatch IBatchGenericCommands.KeyRename(ValkeyKey key, ValkeyKey newKey) => KeyRename(key, newKey);
    IBatch IBatchGenericCommands.KeyRenameNX(ValkeyKey key, ValkeyKey newKey) => KeyRenameNX(key, newKey);
    IBatch IBatchGenericCommands.KeyPersist(ValkeyKey key) => KeyPersist(key);
    IBatch IBatchGenericCommands.KeyDump(ValkeyKey key) => KeyDump(key);
    IBatch IBatchGenericCommands.KeyRestore(ValkeyKey key, byte[] value, TimeSpan? expiry, RestoreOptions? restoreOptions) => KeyRestore(key, value, expiry, restoreOptions);
    IBatch IBatchGenericCommands.KeyRestoreDateTime(ValkeyKey key, byte[] value, DateTime? expiry, RestoreOptions? restoreOptions) => KeyRestoreDateTime(key, value, expiry, restoreOptions);
    IBatch IBatchGenericCommands.KeyTouch(ValkeyKey key) => KeyTouch(key);
    IBatch IBatchGenericCommands.KeyTouch(ValkeyKey[] keys) => KeyTouch(keys);
    IBatch IBatchGenericCommands.KeyExpireTime(ValkeyKey key) => KeyExpireTime(key);
    IBatch IBatchGenericCommands.KeyEncoding(ValkeyKey key) => KeyEncoding(key);
    IBatch IBatchGenericCommands.KeyFrequency(ValkeyKey key) => KeyFrequency(key);
    IBatch IBatchGenericCommands.KeyIdleTime(ValkeyKey key) => KeyIdleTime(key);
    IBatch IBatchGenericCommands.KeyRefCount(ValkeyKey key) => KeyRefCount(key);
    IBatch IBatchGenericCommands.KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace) => KeyCopy(sourceKey, destinationKey, replace);
<<<<<<< HEAD
    IBatch IBatchGenericCommands.KeyMove(ValkeyKey key, int database) => KeyMove(key, database);
    IBatch IBatchGenericCommands.KeyCopy(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace) => KeyCopy(sourceKey, destinationKey, destinationDatabase, replace);
=======
    IBatch IBatchGenericCommands.KeyRandom() => KeyRandom();
    IBatch IBatchGenericCommands.Sort(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, ValkeyValue[]? get) => Sort(key, skip, take, order, sortType, by, get);
    IBatch IBatchGenericCommands.Wait(long numreplicas, long timeout) => Wait(numreplicas, timeout);
>>>>>>> 21f0ecef5ad79cbf806c757e5ddb8e8825824032
}
