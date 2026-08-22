// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Generic commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchGenericCommands.Copy(ValkeyKey, ValkeyKey, bool)" />
    public T Copy(ValkeyKey source, ValkeyKey destination, bool replace = false)
        => AddCmd(Request.Copy(source, destination, replace));

    /// <inheritdoc cref="IBatchGenericCommands.Delete(ValkeyKey)" />
    public T Delete(ValkeyKey key) => AddCmd(Request.Delete(key));

    /// <inheritdoc cref="IBatchGenericCommands.Delete(IEnumerable{ValkeyKey})" />
    public T Delete(IEnumerable<ValkeyKey> keys) => AddCmd(Request.Delete([.. keys]));

    /// <inheritdoc cref="IBatchGenericCommands.Dump(ValkeyKey)" />
    public T Dump(ValkeyKey key) => AddCmd(Request.Dump(key));

    /// <inheritdoc cref="IBatchGenericCommands.Exists(ValkeyKey)" />
    public T Exists(ValkeyKey key) => AddCmd(Request.Exists(key));

    /// <inheritdoc cref="IBatchGenericCommands.Exists(IEnumerable{ValkeyKey})" />
    public T Exists(IEnumerable<ValkeyKey> keys) => AddCmd(Request.Exists([.. keys]));

    /// <inheritdoc cref="IBatchGenericCommands.Expire(ValkeyKey, TimeSpan?, ExpireCondition)" />
    public T Expire(ValkeyKey key, TimeSpan? expiry, ExpireCondition condition = ExpireCondition.Always) => AddCmd(Request.Expire(key, expiry, condition));

    /// <inheritdoc cref="IBatchGenericCommands.Expire(ValkeyKey, DateTimeOffset?, ExpireCondition)" />
    public T Expire(ValkeyKey key, DateTimeOffset? expiry, ExpireCondition condition = ExpireCondition.Always) => AddCmd(Request.Expire(key, expiry, condition));

    /// <inheritdoc cref="IBatchGenericCommands.ExpireTime(ValkeyKey)" />
    public T ExpireTime(ValkeyKey key) => AddCmd(Request.ExpireTime(key));

    /// <inheritdoc cref="IBatchGenericCommands.Migrate(ValkeyKey, MigrateOptions)" />
    public T Migrate(ValkeyKey key, MigrateOptions options)
        => AddCmd(Request.Migrate([key], options));

    /// <inheritdoc cref="IBatchGenericCommands.ObjectEncoding(ValkeyKey)" />
    public T ObjectEncoding(ValkeyKey key) => AddCmd(Request.ObjectEncoding(key));

    /// <inheritdoc cref="IBatchGenericCommands.ObjectFrequency(ValkeyKey)" />
    public T ObjectFrequency(ValkeyKey key) => AddCmd(Request.ObjectFrequency(key));

    /// <inheritdoc cref="IBatchGenericCommands.ObjectIdleTime(ValkeyKey)" />
    public T ObjectIdleTime(ValkeyKey key) => AddCmd(Request.ObjectIdleTime(key));

    /// <inheritdoc cref="IBatchGenericCommands.ObjectRefCount(ValkeyKey)" />
    public T ObjectRefCount(ValkeyKey key) => AddCmd(Request.ObjectRefCount(key));

    /// <inheritdoc cref="IBatchGenericCommands.Persist(ValkeyKey)" />
    public T Persist(ValkeyKey key) => AddCmd(Request.Persist(key));

    /// <inheritdoc cref="IBatchGenericCommands.RandomKey()" />
    public T RandomKey() => AddCmd(Request.RandomKey());

    /// <inheritdoc cref="IBatchGenericCommands.Rename(ValkeyKey, ValkeyKey)" />
    public T Rename(ValkeyKey key, ValkeyKey newKey) => AddCmd(Request.Rename(key, newKey));

    /// <inheritdoc cref="IBatchGenericCommands.RenameIfNotExists(ValkeyKey, ValkeyKey)" />
    public T RenameIfNotExists(ValkeyKey key, ValkeyKey newKey) => AddCmd(Request.RenameIfNotExists(key, newKey));

    /// <inheritdoc cref="IBatchGenericCommands.Restore(ValkeyKey, byte[], RestoreOptions?)" />
    public T Restore(ValkeyKey key, byte[] value, RestoreOptions? options = null) => AddCmd(Request.Restore(key, value, options));

    /// <inheritdoc cref="IBatchGenericCommands.Sort(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" />
    public T Sort(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null) => AddCmd(Request.Sort(key, skip, take, order, sortType, by, get is null ? null : [.. get], null));

    /// <inheritdoc cref="IBatchGenericCommands.Sort(ValkeyKey, SortOptions?)" />
    public T Sort(ValkeyKey key, SortOptions? options)
    {
        var opts = options ?? new SortOptions();
        return Sort(key, opts.Skip, opts.Take, opts.Order.ToOrder(), opts.SortType, opts.By, opts.Get);
    }

    /// <inheritdoc cref="IBatchGenericCommands.SortAndStore(ValkeyKey, ValkeyKey, SortOptions?)" />
    public T SortAndStore(ValkeyKey destination, ValkeyKey key, SortOptions? options)
    {
        var opts = options ?? new SortOptions();
        return AddCmd(Request.SortAndStore(destination, key, opts.Skip, opts.Take, opts.Order.ToOrder(), opts.SortType, opts.By, opts.Get is null ? null : [.. opts.Get]));
    }

    /// <inheritdoc cref="IBatchGenericCommands.SortReadOnly(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" />
    public T SortReadOnly(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null) => AddCmd(Request.SortReadOnly(key, skip, take, order, sortType, by, get is null ? null : [.. get]));

    /// <inheritdoc cref="IBatchGenericCommands.SortReadOnly(ValkeyKey, SortOptions?)" />
    public T SortReadOnly(ValkeyKey key, SortOptions? options)
    {
        var opts = options ?? new SortOptions();
        return SortReadOnly(key, opts.Skip, opts.Take, opts.Order.ToOrder(), opts.SortType, opts.By, opts.Get);
    }

    /// <inheritdoc cref="IBatchGenericCommands.TimeToLive(ValkeyKey)" />
    public T TimeToLive(ValkeyKey key) => AddCmd(Request.TimeToLive(key));

    /// <inheritdoc cref="IBatchGenericCommands.Touch(ValkeyKey)" />
    public T Touch(ValkeyKey key) => AddCmd(Request.Touch(key));

    /// <inheritdoc cref="IBatchGenericCommands.Touch(IEnumerable{ValkeyKey})" />
    public T Touch(IEnumerable<ValkeyKey> keys) => AddCmd(Request.Touch([.. keys]));

    /// <inheritdoc cref="IBatchGenericCommands.Type(ValkeyKey)" />
    public T Type(ValkeyKey key) => AddCmd(Request.Type(key));

    /// <inheritdoc cref="IBatchGenericCommands.Unlink(ValkeyKey)" />
    public T Unlink(ValkeyKey key) => AddCmd(Request.Unlink(key));

    /// <inheritdoc cref="IBatchGenericCommands.Unlink(IEnumerable{ValkeyKey})" />
    public T Unlink(IEnumerable<ValkeyKey> keys) => AddCmd(Request.Unlink([.. keys]));

    /// <inheritdoc cref="IBatchGenericCommands.Wait(long, TimeSpan)" />
    public T Wait(long numreplicas, TimeSpan timeout) => AddCmd(Request.Wait(numreplicas, timeout));

    IBatch IBatchGenericCommands.Copy(ValkeyKey source, ValkeyKey destination, bool replace) => Copy(source, destination, replace);
    IBatch IBatchGenericCommands.Delete(IEnumerable<ValkeyKey> keys) => Delete(keys);
    IBatch IBatchGenericCommands.Delete(ValkeyKey key) => Delete(key);
    IBatch IBatchGenericCommands.Dump(ValkeyKey key) => Dump(key);
    IBatch IBatchGenericCommands.Exists(IEnumerable<ValkeyKey> keys) => Exists(keys);
    IBatch IBatchGenericCommands.Exists(ValkeyKey key) => Exists(key);
    IBatch IBatchGenericCommands.Expire(ValkeyKey key, DateTimeOffset? expiry, ExpireCondition condition) => Expire(key, expiry, condition);
    IBatch IBatchGenericCommands.Expire(ValkeyKey key, TimeSpan? expiry, ExpireCondition condition) => Expire(key, expiry, condition);
    IBatch IBatchGenericCommands.ExpireTime(ValkeyKey key) => ExpireTime(key);
    IBatch IBatchGenericCommands.Migrate(ValkeyKey key, MigrateOptions options) => Migrate(key, options);
    IBatch IBatchGenericCommands.ObjectEncoding(ValkeyKey key) => ObjectEncoding(key);
    IBatch IBatchGenericCommands.ObjectFrequency(ValkeyKey key) => ObjectFrequency(key);
    IBatch IBatchGenericCommands.ObjectIdleTime(ValkeyKey key) => ObjectIdleTime(key);
    IBatch IBatchGenericCommands.ObjectRefCount(ValkeyKey key) => ObjectRefCount(key);
    IBatch IBatchGenericCommands.Persist(ValkeyKey key) => Persist(key);
    IBatch IBatchGenericCommands.RandomKey() => RandomKey();
    IBatch IBatchGenericCommands.Rename(ValkeyKey key, ValkeyKey newKey) => Rename(key, newKey);
    IBatch IBatchGenericCommands.RenameIfNotExists(ValkeyKey key, ValkeyKey newKey) => RenameIfNotExists(key, newKey);
    IBatch IBatchGenericCommands.Restore(ValkeyKey key, byte[] value, RestoreOptions? options) => Restore(key, value, options);
    IBatch IBatchGenericCommands.Sort(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get) => Sort(key, skip, take, order, sortType, by, get);
    IBatch IBatchGenericCommands.Sort(ValkeyKey key, SortOptions? options) => Sort(key, options);
    IBatch IBatchGenericCommands.SortAndStore(ValkeyKey destination, ValkeyKey key, SortOptions? options) => SortAndStore(destination, key, options);
    IBatch IBatchGenericCommands.SortReadOnly(ValkeyKey key, long skip, long take, Order order, SortType sortType, ValkeyValue by, IEnumerable<ValkeyValue>? get) => SortReadOnly(key, skip, take, order, sortType, by, get);
    IBatch IBatchGenericCommands.SortReadOnly(ValkeyKey key, SortOptions? options) => SortReadOnly(key, options);
    IBatch IBatchGenericCommands.TimeToLive(ValkeyKey key) => TimeToLive(key);
    IBatch IBatchGenericCommands.Touch(IEnumerable<ValkeyKey> keys) => Touch(keys);
    IBatch IBatchGenericCommands.Touch(ValkeyKey key) => Touch(key);
    IBatch IBatchGenericCommands.Type(ValkeyKey key) => Type(key);
    IBatch IBatchGenericCommands.Unlink(IEnumerable<ValkeyKey> keys) => Unlink(keys);
    IBatch IBatchGenericCommands.Unlink(ValkeyKey key) => Unlink(key);
    IBatch IBatchGenericCommands.Wait(long numreplicas, TimeSpan timeout) => Wait(numreplicas, timeout);
}
