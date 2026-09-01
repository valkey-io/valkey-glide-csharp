// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(ValkeyKey)"/>
    public async Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key)
        => await Command(Request.ListLeftPop(key));

    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(ValkeyKey, long)"/>
    public async Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count)
        => await Command(Request.ListLeftPop(key, count));

    /// <inheritdoc cref="IListBaseCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.ListLeftPush(key, value, When.Always));

    /// <inheritdoc cref="IListBaseCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.ListLeftPush(key, [.. values], When.Always));

    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(ValkeyKey)"/>
    public async Task<ValkeyValue> ListRightPopAsync(ValkeyKey key)
        => await Command(Request.ListRightPop(key));

    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(ValkeyKey, long)"/>
    public async Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count)
        => await Command(Request.ListRightPop(key, count));

    /// <inheritdoc cref="IListBaseCommands.ListRightPushAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.ListRightPush(key, value, When.Always));

    /// <inheritdoc cref="IListBaseCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.ListRightPush(key, [.. values], When.Always));

    /// <inheritdoc cref="IListBaseCommands.ListLengthAsync(ValkeyKey)"/>
    public async Task<long> ListLengthAsync(ValkeyKey key)
        => await Command(Request.ListLength(key));

    /// <inheritdoc cref="IListBaseCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long)"/>
    public async Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0)
        => await Command(Request.ListRemove(key, value, count));

    /// <inheritdoc cref="IListBaseCommands.ListTrimAsync(ValkeyKey, long, long)"/>
    public async Task ListTrimAsync(ValkeyKey key, long start = 0, long stop = -1)
        => _ = await Command(Request.ListTrim(key, start, stop));

    /// <inheritdoc cref="IListBaseCommands.ListRangeAsync(ValkeyKey, long, long)"/>
    public async Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start = 0, long stop = -1)
        => await Command(Request.ListRange(key, start, stop));

    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)"/>
    public async Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count)
        => await Command(Request.ListLeftPop([.. keys], count));

    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(IEnumerable{ValkeyKey}, long)"/>
    public async Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count)
        => await Command(Request.ListRightPop([.. keys], count));

    /// <inheritdoc cref="IBaseClient.ListIndexAsync(ValkeyKey, long)"/>
    public async Task<ValkeyValue> ListIndexAsync(ValkeyKey key, long index)
        => await Command(Request.ListGetByIndex(key, index));

    /// <inheritdoc cref="IListBaseCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public async Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value)
        => await Command(Request.ListInsertAfter(key, pivot, value));

    /// <inheritdoc cref="IListBaseCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public async Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value)
        => await Command(Request.ListInsertBefore(key, pivot, value));

    /// <inheritdoc cref="IListBaseCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide)"/>
    public async Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide)
        => await Command(Request.ListMove(sourceKey, destinationKey, sourceSide, destinationSide));

    /// <inheritdoc cref="IListBaseCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long)"/>
    public async Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0)
        => await Command(Request.ListPosition(key, element, rank, maxLength));

    /// <inheritdoc cref="IListBaseCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long)"/>
    public async Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0)
        => await Command(Request.ListPositions(key, element, count, rank, maxLength));

    /// <inheritdoc cref="IBaseClient.ListSetAsync(ValkeyKey, long, ValkeyValue)"/>
    public async Task ListSetAsync(ValkeyKey key, long index, ValkeyValue value)
        => _ = await Command(Request.ListSetByIndex(key, index, value));

    // Blocking list operations (Blocking Commands aren't supported in SER so this is according to our own implementation)

    /// <inheritdoc cref="IBaseClient.ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    public async Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
        => await Command(Request.ListBlockingLeftPop([.. keys], timeout));

    /// <inheritdoc cref="IBaseClient.ListBlockingLeftPopAsync(ValkeyKey, TimeSpan)"/>
    public async Task<ValkeyValue[]?> ListBlockingLeftPopAsync(ValkeyKey key, TimeSpan timeout)
        => await ListBlockingLeftPopAsync([key], timeout);

    /// <inheritdoc cref="IBaseClient.ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    public async Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout)
        => await Command(Request.ListBlockingRightPop([.. keys], timeout));

    /// <inheritdoc cref="IBaseClient.ListBlockingRightPopAsync(ValkeyKey, TimeSpan)"/>
    public async Task<ValkeyValue[]?> ListBlockingRightPopAsync(ValkeyKey key, TimeSpan timeout)
        => await ListBlockingRightPopAsync([key], timeout);

    /// <inheritdoc cref="IBaseClient.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan)"/>
    public async Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout)
        => await Command(Request.ListBlockingMove(source, destination, sourceSide, destinationSide, timeout));

    /// <inheritdoc cref="IBaseClient.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan)"/>
    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout)
        => await Command(Request.ListBlockingPop([.. keys], side, timeout));

    /// <inheritdoc cref="IBaseClient.ListBlockingPopAsync(ValkeyKey, ListSide, TimeSpan)"/>
    public async Task<ListPopResult> ListBlockingPopAsync(ValkeyKey key, ListSide side, TimeSpan timeout)
        => await ListBlockingPopAsync([key], side, timeout);

    /// <inheritdoc cref="IBaseClient.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan)"/>
    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout)
        => await Command(Request.ListBlockingPop([.. keys], side, count, timeout));

    /// <inheritdoc cref="IBaseClient.ListBlockingPopAsync(ValkeyKey, ListSide, long, TimeSpan)"/>
    public async Task<ListPopResult> ListBlockingPopAsync(ValkeyKey key, ListSide side, long count, TimeSpan timeout)
        => await ListBlockingPopAsync([key], side, count, timeout);

    // LPUSHX / RPUSHX - Explicit Methods (GLIDE-style API)

    /// <inheritdoc cref="IBaseClient.ListLeftPushIfExistsAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<long> ListLeftPushIfExistsAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.ListLeftPush(key, value, When.Exists));

    /// <inheritdoc cref="IBaseClient.ListLeftPushIfExistsAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<long> ListLeftPushIfExistsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.ListLeftPush(key, [.. values], When.Exists));

    /// <inheritdoc cref="IBaseClient.ListRightPushIfExistsAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<long> ListRightPushIfExistsAsync(ValkeyKey key, ValkeyValue value)
        => await Command(Request.ListRightPush(key, value, When.Exists));

    /// <inheritdoc cref="IBaseClient.ListRightPushIfExistsAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<long> ListRightPushIfExistsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values)
        => await Command(Request.ListRightPush(key, [.. values], When.Exists));
}
