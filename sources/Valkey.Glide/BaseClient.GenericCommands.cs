// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IGenericBaseCommands
{
    /// <inheritdoc/>
    public async Task<bool> KeyDeleteAsync(ValkeyKey key)
        => await Command(Request.KeyDeleteAsync(key));

    /// <inheritdoc/>
    public async Task<long> KeyDeleteAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.KeyDeleteAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<bool> KeyUnlinkAsync(ValkeyKey key)
        => await Command(Request.KeyUnlinkAsync(key));

    /// <inheritdoc/>
    public async Task<long> KeyUnlinkAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.KeyUnlinkAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<bool> KeyExistsAsync(ValkeyKey key)
        => await Command(Request.KeyExistsAsync(key));

    /// <inheritdoc/>
    public async Task<long> KeyExistsAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.KeyExistsAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry)
        => await KeyExpireAsync(key, expiry, ExpireWhen.Always);

    /// <inheritdoc/>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when)
        => await Command(Request.KeyExpireAsync(key, expiry, when));

    /// <inheritdoc/>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry)
        => await KeyExpireAsync(key, expiry, ExpireWhen.Always);

    /// <inheritdoc/>
    public async Task<bool> KeyExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when)
        => await Command(Request.KeyExpireAsync(key, expiry, when));

    /// <inheritdoc/>
    public async Task<TimeSpan?> KeyTimeToLiveAsync(ValkeyKey key)
        => await Command(Request.KeyTimeToLiveAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyType> KeyTypeAsync(ValkeyKey key)
        => await Command(Request.KeyTypeAsync(key));

    /// <inheritdoc/>
    public async Task KeyRenameAsync(ValkeyKey key, ValkeyKey newKey)
        => _ = await Command(Request.KeyRenameAsync(key, newKey));

    /// <inheritdoc/>
    public async Task<bool> KeyRenameNXAsync(ValkeyKey key, ValkeyKey newKey)
        => await Command(Request.KeyRenameNXAsync(key, newKey));

    /// <inheritdoc/>
    public async Task<bool> KeyPersistAsync(ValkeyKey key)
        => await Command(Request.KeyPersistAsync(key));

    /// <inheritdoc/>
    public async Task<byte[]?> KeyDumpAsync(ValkeyKey key)
        => await Command(Request.KeyDumpAsync(key));

    /// <inheritdoc/>
    public async Task KeyRestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null)
        => _ = await Command(Request.KeyRestoreAsync(key, value, expiry, restoreOptions));

    /// <inheritdoc/>
    public async Task KeyRestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null)
        => _ = await Command(Request.KeyRestoreDateTimeAsync(key, value, expiry, restoreOptions));

    /// <inheritdoc/>
    public async Task<bool> KeyTouchAsync(ValkeyKey key)
        => await Command(Request.KeyTouchAsync(key));

    /// <inheritdoc/>
    public async Task<long> KeyTouchAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.KeyTouchAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<DateTime?> KeyExpireTimeAsync(ValkeyKey key)
        => await Command(Request.KeyExpireTimeAsync(key));

    /// <inheritdoc/>
    public async Task<string?> KeyEncodingAsync(ValkeyKey key)
        => await Command(Request.KeyEncodingAsync(key));

    /// <inheritdoc/>
    public async Task<long?> KeyFrequencyAsync(ValkeyKey key)
        => await Command(Request.KeyFrequencyAsync(key));

    /// <inheritdoc/>
    public async Task<long?> KeyIdleTimeAsync(ValkeyKey key)
        => await Command(Request.KeyIdleTimeAsync(key));

    /// <inheritdoc/>
    public async Task<long?> KeyRefCountAsync(ValkeyKey key)
        => await Command(Request.KeyRefCountAsync(key));

    /// <inheritdoc/>
    public async Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false)
        => await Command(Request.KeyCopyAsync(sourceKey, destinationKey, replace));

    /// <inheritdoc/>
    public async Task<bool> KeyMoveAsync(ValkeyKey key, int database)
        => await Command(Request.KeyMoveAsync(key, database));

    /// <inheritdoc/>
    public async Task<bool> KeyCopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false)
        => await Command(Request.KeyCopyAsync(sourceKey, destinationKey, destinationDatabase, replace));

    /// <inheritdoc/>
    public async Task<string?> KeyRandomAsync()
        => await Command(Request.KeyRandomAsync());

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SortAsync(ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null)
        => await Command(Request.SortAsync(key, skip, take, order, sortType, by, get?.ToArray(), await GetServerVersionAsync()));

    /// <inheritdoc/>
    public async Task<long> SortAndStoreAsync(ValkeyKey destination, ValkeyKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, ValkeyValue by = default, IEnumerable<ValkeyValue>? get = null)
        => await Command(Request.SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get?.ToArray()));

    /// <inheritdoc/>
    public async Task<long> WaitAsync(long numreplicas, TimeSpan timeout)
        => await Command(Request.WaitAsync(numreplicas, timeout));
}
