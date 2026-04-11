// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

internal interface IBatchGenericCommands
{
    /// <inheritdoc cref="IBaseClient.DeleteAsync(ValkeyKey)" />
    /// <returns>Command Response - <see langword="true"/> if the key was removed.</returns>
    IBatch Delete(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.DeleteAsync(IEnumerable{ValkeyKey})" />
    /// <returns>Command Response - The number of keys that were removed.</returns>
    IBatch Delete(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.UnlinkAsync(ValkeyKey)" />
    /// <returns>Command Response - <see langword="true"/> if the key was unlinked.</returns>
    IBatch Unlink(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.UnlinkAsync(IEnumerable{ValkeyKey})" />
    /// <returns>Command Response - The number of keys that were unlinked.</returns>
    IBatch Unlink(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(ValkeyKey)" />
    /// <returns>Command Response - <see langword="true"/> if the key exists.</returns>
    IBatch Exists(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ExistsAsync(IEnumerable{ValkeyKey})" />
    /// <returns>Command Response - The number of existing keys.</returns>
    IBatch Exists(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, TimeSpan?, ExpireCondition)" />
    /// <returns>Command Response - <see langword="true"/> if the timeout was set.</returns>
    IBatch Expire(ValkeyKey key, TimeSpan? expiry, ExpireCondition condition = ExpireCondition.Always);

    /// <inheritdoc cref="IBaseClient.ExpireAsync(ValkeyKey, DateTimeOffset?, ExpireCondition)" />
    /// <returns>Command Response - <see langword="true"/> if the timeout was set.</returns>
    IBatch Expire(ValkeyKey key, DateTimeOffset? expiry, ExpireCondition condition = ExpireCondition.Always);

    /// <inheritdoc cref="IBaseClient.TimeToLiveAsync(ValkeyKey)" />
    /// <returns>Command Response - A <see cref="TimeToLiveResult"/> with TTL information.</returns>
    IBatch TimeToLive(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.TypeAsync(ValkeyKey)" />
    /// <returns>Command Response - Type of key, or none when key does not exist.</returns>
    IBatch Type(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.RenameAsync(ValkeyKey, ValkeyKey)" />
    /// <returns>Command Response - No value is returned (void command).</returns>
    IBatch Rename(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IBaseClient.RenameIfNotExistsAsync(ValkeyKey, ValkeyKey)" />
    /// <returns>Command Response - <see langword="true"/> if the key was renamed.</returns>
    IBatch RenameIfNotExists(ValkeyKey key, ValkeyKey newKey);

    /// <inheritdoc cref="IBaseClient.PersistAsync(ValkeyKey)" />
    /// <returns>Command Response - <see langword="true"/> if the timeout was removed.</returns>
    IBatch Persist(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.DumpAsync(ValkeyKey)" />
    /// <returns>Command Response - The serialized value, or <see langword="null"/> if key does not exist.</returns>
    IBatch Dump(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.RestoreAsync(ValkeyKey, byte[], TimeSpan?, RestoreOptions?)" />
    /// <returns>Command Response - No value is returned (void command).</returns>
    IBatch Restore(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IBaseClient.RestoreDateTimeAsync(ValkeyKey, byte[], DateTime?, RestoreOptions?)" />
    /// <returns>Command Response - No value is returned (void command).</returns>
    IBatch RestoreDateTime(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null);

    /// <inheritdoc cref="IBaseClient.TouchAsync(ValkeyKey)" />
    /// <returns>Command Response - <see langword="true"/> if the key was touched.</returns>
    IBatch Touch(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.TouchAsync(IEnumerable{ValkeyKey})" />
    /// <returns>Command Response - The number of keys that were updated.</returns>
    IBatch Touch(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.ExpireTimeAsync(ValkeyKey)" />
    /// <returns>Command Response - A <see cref="DateTimeOffset"/> with the expiry time, or <see langword="null"/> if the key does not exist or has no expiry.</returns>
    IBatch ExpireTime(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ObjectEncodingAsync(ValkeyKey)" />
    /// <returns>Command Response - The encoding of the object, or <see langword="null"/> if key does not exist.</returns>
    IBatch ObjectEncoding(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ObjectFrequencyAsync(ValkeyKey)" />
    /// <returns>Command Response - The frequency counter, or <see langword="null"/> if key does not exist.</returns>
    IBatch ObjectFrequency(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ObjectIdleTimeAsync(ValkeyKey)" />
    /// <returns>Command Response - The idle time, or <see langword="null"/> if key does not exist.</returns>
    IBatch ObjectIdleTime(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.ObjectRefCountAsync(ValkeyKey)" />
    /// <returns>Command Response - The reference count, or <see langword="null"/> if key does not exist.</returns>
    IBatch ObjectRefCount(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.CopyAsync(ValkeyKey, ValkeyKey, bool)" />
    /// <returns>Command Response - <see langword="true"/> if the key was copied.</returns>
    IBatch Copy(ValkeyKey source, ValkeyKey destination, bool replace = false);

    /// <inheritdoc cref="IBaseClient.RandomKeyAsync()" />
    /// <returns>Command Response - A random key, or <see langword="null"/> when the database is empty.</returns>
    IBatch RandomKey();

    /// <inheritdoc cref="IGenericBaseCommands.SortAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" />
    /// <returns>Command Response - The sorted elements.</returns>
    IBatch Sort(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

    /// <inheritdoc cref="IBaseClient.SortAsync(ValkeyKey, SortOptions?)" />
    /// <returns>Command Response - The sorted elements.</returns>
    IBatch Sort(ValkeyKey key, SortOptions? options);

    /// <inheritdoc cref="IBaseClient.SortAndStoreAsync(ValkeyKey, ValkeyKey, SortOptions?)" />
    /// <returns>Command Response - The number of elements stored in destination.</returns>
    IBatch SortAndStore(ValkeyKey destination, ValkeyKey key, SortOptions? options);

    /// <inheritdoc cref="IBaseClient.SortReadOnlyAsync(ValkeyKey, long, long, Order, SortType, ValkeyValue, IEnumerable{ValkeyValue})" />
    /// <returns>Command Response - The sorted elements.</returns>
    IBatch SortReadOnly(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null);

    /// <inheritdoc cref="IBaseClient.SortReadOnlyAsync(ValkeyKey, SortOptions?)" />
    /// <returns>Command Response - The sorted elements.</returns>
    IBatch SortReadOnly(ValkeyKey key, SortOptions? options);

    /// <inheritdoc cref="IBaseClient.WaitAsync(long, TimeSpan)" />
    /// <returns>Command Response - The number of replicas that acknowledged the write commands.</returns>
    IBatch Wait(long numreplicas, TimeSpan timeout);
}
