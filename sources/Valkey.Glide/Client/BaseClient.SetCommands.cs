// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetAddAsync(key, value));

    /// <inheritdoc/>
    public async Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.SetAddAsync(key, values));

    /// <inheritdoc/>
    public async Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetRemoveAsync(key, value));

    /// <inheritdoc/>
    public async Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.SetRemoveAsync(key, values));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> SetMembersAsync(ValkeyKey key)
        => await Command(Request.SetMembersAsync(key));

    /// <inheritdoc/>
    public async Task<long> SetCardAsync(ValkeyKey key)
        => await Command(Request.SetCardAsync(key));

    /// <inheritdoc/>
    public async Task<long> SetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
        => await Command(Request.SetInterCardAsync(keys, limit));

    /// <inheritdoc/>
    public async Task<ValkeyValue> SetPopAsync(ValkeyKey key)
        => await Command(Request.SetPopAsync(key));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> SetPopAsync(ValkeyKey key, long count)
        => await Command(Request.SetPopAsync(key, count));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> SetUnionAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetUnionAsync(keys));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> SetInterAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetInterAsync(keys));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> SetDiffAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetDiffAsync(keys));

    /// <inheritdoc/>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetUnionStoreAsync(destination, keys));

    /// <inheritdoc/>
    public async Task<long> SetInterStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetInterStoreAsync(destination, keys));

    /// <inheritdoc/>
    public async Task<long> SetDiffStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetDiffStoreAsync(destination, keys));

    /// <inheritdoc/>
    public async Task<bool> SetIsMemberAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetIsMemberAsync(key, value));

    /// <inheritdoc/>
    public async Task<bool[]> SetIsMemberAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.SetIsMemberAsync(key, values));

    /// <inheritdoc/>
    public async Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key)
        => await Command(Request.SetRandomMemberAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count)
        => await Command(Request.SetRandomMembersAsync(key, count));

    /// <inheritdoc/>
    public async Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value)
        => await Command(Request.SetMoveAsync(source, destination, value));

    // TODO #287
    /// <inheritdoc/>
    public async IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ScanOptions? options = null)
    {
        long currentCursor = 0;

        do
        {
            (long nextCursor, ValkeyValue[] elements) = await Command(Request.SetScanAsync(key, currentCursor, options));

            foreach (ValkeyValue element in elements)
            {
                yield return element;
            }

            currentCursor = nextCursor;
        } while (currentCursor != 0);
    }
}
