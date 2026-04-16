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
        => await Command(Request.SetAddAsync(key, [.. values]));

    /// <inheritdoc/>
    public async Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetRemoveAsync(key, value));

    /// <inheritdoc/>
    public async Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.SetRemoveAsync(key, [.. values]));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key)
        => await Command(Request.SetMembersAsync(key));

    /// <inheritdoc/>
    public async Task<long> SetLengthAsync(ValkeyKey key)
        => await Command(Request.SetLengthAsync(key));

    /// <inheritdoc/>
    public async Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
        => await Command(Request.SetIntersectionLengthAsync([.. keys], limit));

    /// <inheritdoc/>
    public async Task<ValkeyValue> SetPopAsync(ValkeyKey key)
        => await Command(Request.SetPopAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count)
        => await Command(Request.SetPopAsync(key, count));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetUnionAsync(ValkeyKey first, ValkeyKey second)
        => await SetUnionAsync([first, second]);

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetUnionAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetUnionAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetIntersectAsync(ValkeyKey first, ValkeyKey second)
        => await SetIntersectAsync([first, second]);

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetIntersectAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetIntersectAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetDifferenceAsync(ValkeyKey first, ValkeyKey second)
        => await SetDifferenceAsync([first, second]);

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetDifferenceAsync(IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetDifferenceAsync([.. keys]));

    /// <inheritdoc/>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second)
        => await SetUnionStoreAsync(destination, [first, second]);

    /// <inheritdoc/>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetUnionStoreAsync(destination, [.. keys]));

    /// <inheritdoc/>
    public async Task<long> SetIntersectStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second)
        => await SetIntersectStoreAsync(destination, [first, second]);

    /// <inheritdoc/>
    public async Task<long> SetIntersectStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetIntersectStoreAsync(destination, [.. keys]));

    /// <inheritdoc/>
    public async Task<long> SetDifferenceStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second)
        => await SetDifferenceStoreAsync(destination, [first, second]);

    /// <inheritdoc/>
    public async Task<long> SetDifferenceStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetDifferenceStoreAsync(destination, [.. keys]));

    /// <inheritdoc/>
    public async Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetContainsAsync(key, value));

    /// <inheritdoc/>
    public async Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.SetContainsAsync(key, [.. values]));

    /// <inheritdoc/>
    public async Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key)
        => await Command(Request.SetRandomMemberAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count)
        => await Command(Request.SetRandomMembersAsync(key, count));

    /// <inheritdoc/>
    public async Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value)
        => await Command(Request.SetMoveAsync(source, destination, value));

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
