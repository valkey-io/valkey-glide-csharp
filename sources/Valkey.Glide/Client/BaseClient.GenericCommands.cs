// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    // ==================== GLIDE-style methods (primary implementations) ====================

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(ValkeyKey key)
        => await Command(Request.KeyDeleteAsync(key));

    /// <inheritdoc/>
    public async Task<long> DeleteAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.KeyDeleteAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<bool> UnlinkAsync(ValkeyKey key)
        => await Command(Request.KeyUnlinkAsync(key));

    /// <inheritdoc/>
    public async Task<long> UnlinkAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.KeyUnlinkAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<bool> ExistsAsync(ValkeyKey key)
        => await Command(Request.KeyExistsAsync(key));

    /// <inheritdoc/>
    public async Task<long> ExistsAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.KeyExistsAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<bool> ExpireAsync(ValkeyKey key, TimeSpan? expiry)
        => await ExpireAsync(key, expiry, ExpireWhen.Always);

    /// <inheritdoc/>
    public async Task<bool> ExpireAsync(ValkeyKey key, TimeSpan? expiry, ExpireWhen when)
        => await Command(Request.KeyExpireAsync(key, expiry, when));

    /// <inheritdoc/>
    public async Task<bool> ExpireAsync(ValkeyKey key, DateTime? expiry)
        => await ExpireAsync(key, expiry, ExpireWhen.Always);

    /// <inheritdoc/>
    public async Task<bool> ExpireAsync(ValkeyKey key, DateTime? expiry, ExpireWhen when)
        => await Command(Request.KeyExpireAsync(key, expiry, when));

    /// <inheritdoc/>
    public async Task<long> TimeToLiveAsync(ValkeyKey key)
        => await Command(Request.KeyTimeToLiveAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyType> TypeAsync(ValkeyKey key)
        => await Command(Request.KeyTypeAsync(key));

    /// <inheritdoc/>
    public async Task RenameAsync(ValkeyKey key, ValkeyKey newKey)
        => _ = await Command(Request.KeyRenameAsync(key, newKey));

    /// <inheritdoc/>
    public async Task<bool> RenameNXAsync(ValkeyKey key, ValkeyKey newKey)
        => await Command(Request.KeyRenameNXAsync(key, newKey));

    /// <inheritdoc/>
    public async Task<bool> PersistAsync(ValkeyKey key)
        => await Command(Request.KeyPersistAsync(key));

    /// <inheritdoc/>
    public async Task<byte[]?> DumpAsync(ValkeyKey key)
        => await Command(Request.KeyDumpAsync(key));

    /// <inheritdoc/>
    public async Task RestoreAsync(ValkeyKey key, byte[] value, TimeSpan? expiry = null, RestoreOptions? restoreOptions = null)
        => _ = await Command(Request.KeyRestoreAsync(key, value, expiry, restoreOptions));

    /// <inheritdoc/>
    public async Task RestoreDateTimeAsync(ValkeyKey key, byte[] value, DateTime? expiry = null, RestoreOptions? restoreOptions = null)
        => _ = await Command(Request.KeyRestoreDateTimeAsync(key, value, expiry, restoreOptions));

    /// <inheritdoc/>
    public async Task<bool> TouchAsync(ValkeyKey key)
        => await Command(Request.KeyTouchAsync(key));

    /// <inheritdoc/>
    public async Task<long> TouchAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.KeyTouchAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<DateTime?> ExpireTimeAsync(ValkeyKey key)
        => await Command(Request.KeyExpireTimeAsync(key));

    /// <inheritdoc/>
    public async Task<string?> ObjectEncodingAsync(ValkeyKey key)
        => await Command(Request.KeyEncodingAsync(key));

    /// <inheritdoc/>
    public async Task<long?> ObjectFrequencyAsync(ValkeyKey key)
        => await Command(Request.KeyFrequencyAsync(key));

    /// <inheritdoc/>
    public async Task<TimeSpan?> ObjectIdleTimeAsync(ValkeyKey key)
        => await Command(Request.KeyIdleTimeAsync(key));

    /// <inheritdoc/>
    public async Task<long?> ObjectRefCountAsync(ValkeyKey key)
        => await Command(Request.KeyRefCountAsync(key));

    /// <inheritdoc/>
    public async Task<bool> CopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, bool replace = false)
        => await Command(Request.KeyCopyAsync(sourceKey, destinationKey, replace));

    /// <inheritdoc/>
    public async Task<bool> MoveAsync(ValkeyKey key, int database)
        => await Command(Request.KeyMoveAsync(key, database));

    /// <inheritdoc/>
    public async Task<bool> CopyAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, int destinationDatabase, bool replace = false)
        => await Command(Request.KeyCopyAsync(sourceKey, destinationKey, destinationDatabase, replace));

    /// <inheritdoc/>
    public async Task<ValkeyKey?> RandomKeyAsync()
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
