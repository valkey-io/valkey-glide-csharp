// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key)
        => await Command(Request.ListLeftPopAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count)
        => await Command(Request.ListLeftPopAsync(key, count));

    /// <inheritdoc/>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always)
        => await Command(Request.ListLeftPushAsync(key, value, when));

    /// <inheritdoc/>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when = When.Always)
        => await Command(Request.ListLeftPushAsync(key, [.. values], when));

    /// <inheritdoc/>
    public async Task<ValkeyValue> ListRightPopAsync(ValkeyKey key)
        => await Command(Request.ListRightPopAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count)
        => await Command(Request.ListRightPopAsync(key, count));

    /// <inheritdoc/>
    public async Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always)
        => await Command(Request.ListRightPushAsync(key, value, when));

    /// <inheritdoc/>
    public async Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when = When.Always)
        => await Command(Request.ListRightPushAsync(key, [.. values], when));

    /// <inheritdoc/>
    public async Task<long> ListLengthAsync(ValkeyKey key)
        => await Command(Request.ListLengthAsync(key));

    /// <inheritdoc/>
    public async Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0)
        => await Command(Request.ListRemoveAsync(key, value, count));

    /// <inheritdoc/>
    public async Task ListTrimAsync(ValkeyKey key, long start, long stop)
        => _ = await Command(Request.ListTrimAsync(key, start, stop));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start = 0, long stop = -1)
        => await Command(Request.ListRangeAsync(key, start, stop));

    /// <inheritdoc/>
    public async Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count)
        => await Command(Request.ListLeftPopAsync([.. keys], count));

    /// <inheritdoc/>
    public async Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count)
        => await Command(Request.ListRightPopAsync([.. keys], count));

    /// <inheritdoc/>
    public async Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index)
        => await Command(Request.ListGetByIndexAsync(key, index));

    /// <inheritdoc/>
    public async Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value)
        => await Command(Request.ListInsertAfterAsync(key, pivot, value));

    /// <inheritdoc/>
    public async Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value)
        => await Command(Request.ListInsertBeforeAsync(key, pivot, value));

    /// <inheritdoc/>
    public async Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide)
        => await Command(Request.ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide));

    /// <inheritdoc/>
    public async Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0)
        => await Command(Request.ListPositionAsync(key, element, rank, maxLength));

    /// <inheritdoc/>
    public async Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0)
        => await Command(Request.ListPositionsAsync(key, element, count, rank, maxLength));

    /// <inheritdoc/>
    public async Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value)
        => _ = await Command(Request.ListSetByIndexAsync(key, index, value));

    // Blocking list operations (Blocking Commands aren't supported in SER so this is according to our own implementation)

    /// <inheritdoc/>
    public async Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
        => await Command(Request.ListBlockingLeftPopAsync([.. keys], timeout));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
        => await Command(Request.ListBlockingRightPopAsync([.. keys], timeout));

    /// <inheritdoc/>
    public async Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout)
        => await Command(Request.ListBlockingMoveAsync(source, destination, sourceSide, destinationSide, timeout));

    /// <inheritdoc/>
    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout)
        => await Command(Request.ListBlockingPopAsync([.. keys], side, timeout));

    /// <inheritdoc/>
    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout)
        => await Command(Request.ListBlockingPopAsync([.. keys], side, count, timeout));
}
