// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
    public async Task<ISet<ValkeyValue>> SetMembersAsync(ValkeyKey key)
        => new HashSet<ValkeyValue>(await Command(Request.SetMembersAsync(key)));

    /// <inheritdoc/>
    public async Task<long> SetCardAsync(ValkeyKey key)
        => await Command(Request.SetLengthAsync(key));

    /// <inheritdoc/>
    public async Task<long> SetInterCardAsync(IEnumerable<ValkeyKey> keys, long limit = 0)
        => await Command(Request.SetIntersectionLengthAsync([.. keys], limit));

    /// <inheritdoc/>
    public async Task<ValkeyValue> SetPopAsync(ValkeyKey key)
        => await Command(Request.SetPopAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count)
        => await Command(Request.SetPopAsync(key, count));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> SetUnionAsync(IEnumerable<ValkeyKey> keys)
        => new HashSet<ValkeyValue>(await Command(Request.SetUnionAsync([.. keys])));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> SetInterAsync(IEnumerable<ValkeyKey> keys)
        => new HashSet<ValkeyValue>(await Command(Request.SetIntersectAsync([.. keys])));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> SetDiffAsync(IEnumerable<ValkeyKey> keys)
        => new HashSet<ValkeyValue>(await Command(Request.SetDifferenceAsync([.. keys])));

    /// <inheritdoc/>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetUnionStoreAsync(destination, [.. keys]));

    /// <inheritdoc/>
    public async Task<long> SetInterStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetIntersectStoreAsync(destination, [.. keys]));

    /// <inheritdoc/>
    public async Task<long> SetDiffStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys)
        => await Command(Request.SetDifferenceStoreAsync(destination, [.. keys]));

    /// <inheritdoc/>
    public async Task<bool> SetIsMemberAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.SetContainsAsync(key, value));

    /// <inheritdoc/>
    public async Task<bool[]> SetIsMemberAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
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

    // TODO #287
    /// <inheritdoc/>
    public async IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0)
    {
        long currentCursor = cursor;
        int currentOffset = pageOffset;

        do
        {
            (long nextCursor, ValkeyValue[] elements) = await Command(Request.SetScanAsync(key, currentCursor, pattern, pageSize));

            IEnumerable<ValkeyValue> elementsToYield = currentOffset > 0 ? elements.Skip(currentOffset) : elements;

            foreach (ValkeyValue element in elementsToYield)
            {
                yield return element;
            }

            currentCursor = nextCursor;
        } while (currentCursor != 0);
    }
}
