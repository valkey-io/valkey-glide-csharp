// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Generic commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchGenericCommands.Delete(ValkeyKey)" />
    public T Delete(ValkeyKey key) => AddCmd(DeleteAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Delete(IEnumerable{ValkeyKey})" />
    public T Delete(IEnumerable<ValkeyKey> keys) => AddCmd(DeleteAsync([.. keys]));

    /// <inheritdoc cref="IBatchGenericCommands.Unlink(ValkeyKey)" />
    public T Unlink(ValkeyKey key) => AddCmd(UnlinkAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Unlink(IEnumerable{ValkeyKey})" />
    public T Unlink(IEnumerable<ValkeyKey> keys) => AddCmd(UnlinkAsync([.. keys]));

    /// <inheritdoc cref="IBatchGenericCommands.Exists(ValkeyKey)" />
    public T Exists(ValkeyKey key) => AddCmd(ExistsAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Exists(IEnumerable{ValkeyKey})" />
    public T Exists(IEnumerable<ValkeyKey> keys) => AddCmd(ExistsAsync([.. keys]));

    /// <inheritdoc cref="IBatchGenericCommands.Expire(ValkeyKey, TimeSpan?)" />
    public T Expire(ValkeyKey key, TimeSpan? expiry) => AddCmd(ExpireAsync(key, expiry));

    /// <inheritdoc cref="IBatchGenericCommands.Expire(ValkeyKey, TimeSpan?, ExpireWhen)" />
    public T Expire(ValkeyKey key, TimeSpan? expiry, ExpireWhen when) => AddCmd(ExpireAsync(key, expiry, when));

    /// <inheritdoc cref="IBatchGenericCommands.Expire(ValkeyKey, DateTime?)" />
    public T Expire(ValkeyKey key, DateTime? expiry) => AddCmd(ExpireAsync(key, expiry));

    /// <inheritdoc cref="IBatchGenericCommands.Expire(ValkeyKey, DateTime?, ExpireWhen)" />
    public T Expire(ValkeyKey key, DateTime? expiry, ExpireWhen when) => AddCmd(ExpireAsync(key, expiry, when));

    /// <inheritdoc cref="IBatchGenericCommands.TimeToLive(ValkeyKey)" />
    public T TimeToLive(ValkeyKey key) => AddCmd(TimeToLiveAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Type(ValkeyKey)" />
    public T Type(ValkeyKey key) => AddCmd(TypeAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Rename(ValkeyKey, ValkeyKey)" />
    public T Rename(ValkeyKey key, ValkeyKey newKey) => AddCmd(RenameAsync(key, newKey));

    /// <inheritdoc cref="IBatchGenericCommands.RenameNX(ValkeyKey, ValkeyKey)" />
    public T RenameNX(ValkeyKey key, ValkeyKey newKey) => AddCmd(RenameNXAsync(key, newKey));

    /// <inheritdoc cref="IBatchGenericCommands.Persist(ValkeyKey)" />
    public T Persist(ValkeyKey key) => AddCmd(PersistAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Dump(ValkeyKey)" />
    public T Dump(ValkeyKey key) => AddCmd(DumpAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Restore(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" />
    public T Restore(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null) => AddCmd(RestoreAsync(key, value, expiry, restoreOptions));

    /// <inheritdoc cref="IBatchGenericCommands.RestoreDateTime(ValkeyKey, byte[], DateTime?, RestoreOptions?)" />
    public T RestoreDateTime(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null) => AddCmd(RestoreDateTimeAsync(key, value, expiry, restoreOptions));

    /// <inheritdoc cref="IBatchGenericCommands.Touch(ValkeyKey)" />
    public T Touch(ValkeyKey key) => AddCmd(TouchAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Touch(IEnumerable{ValkeyKey})" />
    public T Touch(IEnumerable<ValkeyKey> keys) => AddCmd(TouchAsync([.. keys]));

    /// <inheritdoc cref="IBatchGenericCommands.ExpireTime(ValkeyKey)" />
    public T ExpireTime(ValkeyKey key) => AddCmd(ExpireTimeAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.ObjectEncoding(ValkeyKey)" />
    public T ObjectEncoding(ValkeyKey key) => AddCmd(ObjectEncodingAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.ObjectFrequency(ValkeyKey)" />
    public T ObjectFrequency(ValkeyKey key) => AddCmd(ObjectFrequencyAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.ObjectIdleTime(ValkeyKey)" />
    public T ObjectIdleTime(ValkeyKey key) => AddCmd(ObjectIdleTimeAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.ObjectRefCount(ValkeyKey)" />
    public T ObjectRefCount(ValkeyKey key) => AddCmd(ObjectRefCountAsync(key));

    /// <inheritdoc cref="IBatchGenericCommands.Copy(ValkeyKey, ValkeyKey, int, bool)" />
    public T Copy(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase = -1, bool replace = false)
        => destinationDatabase == -1
            ? AddCmd(CopyAsync(sourceKey, destinationKey, replace))
            : AddCmd(CopyAsync(sourceKey, destinationKey, destinationDatabase, replace));

    /// <inheritdoc cref="IBatchGenericCommands.Move(ValkeyKey, int)" />
    public T Move(ValkeyKey key, int database) => AddCmd(MoveAsync(key, database));

    /// <inheritdoc cref="IBatchGenericCommands.RandomKey()" />
    public T RandomKey() => AddCmd(RandomKeyAsync());

    /// <inheritdoc cref="IBatchGenericCommands.Sort(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" />
    public T Sort(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null) => AddCmd(SortAsync(key, skip, take, order, sortType, by, get is null ? null : [.. get], null));

    /// <inheritdoc cref="IBatchGenericCommands.Wait(long, TimeSpan)" />
    public T Wait(long numreplicas, TimeSpan timeout) => AddCmd(WaitAsync(numreplicas, timeout));

    // Explicit interface implementations for IBatchGenericCommands
    IBatch IBatchGenericCommands.Delete(ValkeyKey key) => Delete(key);
    IBatch IBatchGenericCommands.Delete(IEnumerable<ValkeyKey> keys) => Delete(keys);
    IBatch IBatchGenericCommands.Unlink(ValkeyKey key) => Unlink(key);
    IBatch IBatchGenericCommands.Unlink(IEnumerable<ValkeyKey> keys) => Unlink(keys);
    IBatch IBatchGenericCommands.Exists(ValkeyKey key) => Exists(key);
    IBatch IBatchGenericCommands.Exists(IEnumerable<ValkeyKey> keys) => Exists(keys);
    IBatch IBatchGenericCommands.Expire(ValkeyKey key, TimeSpan? expiry) => Expire(key, expiry);
    IBatch IBatchGenericCommands.Expire(ValkeyKey key, TimeSpan? expiry, ExpireWhen when) => Expire(key, expiry, when);
    IBatch IBatchGenericCommands.Expire(ValkeyKey key, DateTime? expiry) => Expire(key, expiry);
    IBatch IBatchGenericCommands.Expire(ValkeyKey key, DateTime? expiry, ExpireWhen when) => Expire(key, expiry, when);
    IBatch IBatchGenericCommands.TimeToLive(ValkeyKey key) => TimeToLive(key);
    IBatch IBatchGenericCommands.Type(ValkeyKey key) => Type(key);
    IBatch IBatchGenericCommands.Rename(ValkeyKey key, ValkeyKey newKey) => Rename(key, newKey);
    IBatch IBatchGenericCommands.RenameNX(ValkeyKey key, ValkeyKey newKey) => RenameNX(key, newKey);
    IBatch IBatchGenericCommands.Persist(ValkeyKey key) => Persist(key);
    IBatch IBatchGenericCommands.Dump(ValkeyKey key) => Dump(key);
    IBatch IBatchGenericCommands.Restore(ValkeyKey key, byte[] value, TimeSpan? expiry, RestoreOptions? restoreOptions) => Restore(key, value, expiry, restoreOptions);
    IBatch IBatchGenericCommands.RestoreDateTime(ValkeyKey key, byte[] value, DateTime? expiry, RestoreOptions? restoreOptions) => RestoreDateTime(key, value, expiry, restoreOptions);
    IBatch IBatchGenericCommands.Touch(ValkeyKey key) => Touch(key);
    IBatch IBatchGenericCommands.Touch(IEnumerable<ValkeyKey> keys) => Touch(keys);
    IBatch IBatchGenericCommands.ExpireTime(ValkeyKey key) => ExpireTime(key);
    IBatch IBatchGenericCommands.ObjectEncoding(ValkeyKey key) => ObjectEncoding(key);
    IBatch IBatchGenericCommands.ObjectFrequency(ValkeyKey key) => ObjectFrequency(key);
    IBatch IBatchGenericCommands.ObjectIdleTime(ValkeyKey key) => ObjectIdleTime(key);
    IBatch IBatchGenericCommands.ObjectRefCount(ValkeyKey key) => ObjectRefCount(key);
    IBatch IBatchGenericCommands.Copy(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace) => Copy(sourceKey, destinationKey, destinationDatabase, replace);
    IBatch IBatchGenericCommands.Move(ValkeyKey key, int database) => Move(key, database);
    IBatch IBatchGenericCommands.RandomKey() => RandomKey();
    IBatch IBatchGenericCommands.Sort(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get) => Sort(key, skip, take, order, sortType, by, get);
    IBatch IBatchGenericCommands.Wait(long numreplicas, TimeSpan timeout) => Wait(numreplicas, timeout);
}
