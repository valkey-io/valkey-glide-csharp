// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region List Commands with CommandFlags (SER Compatibility)

    public async Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(key);
    }

    public async Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(key, count);
    }

    public async Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(keys, count);
    }

    public async Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, value, when);
    }

    public async Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, values, when);
    }

    public async Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, values);
    }

    public async Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(key);
    }

    public async Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(key, count);
    }

    public async Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(keys, count);
    }

    public async Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, value, when);
    }

    public async Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, values, when);
    }

    public async Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, values);
    }

    public async Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLengthAsync(key);
    }

    public async Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRemoveAsync(key, value, count);
    }

    public async Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ListTrimAsync(key, start, stop);
    }

    public async Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRangeAsync(key, start, stop);
    }

    public async Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListGetByIndexAsync(key, index);
    }

    public async Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListInsertAfterAsync(key, pivot, value);
    }

    public async Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListInsertBeforeAsync(key, pivot, value);
    }

    public async Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide);
    }

    public async Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank, long maxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListPositionAsync(key, element, rank, maxLength);
    }

    public async Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank, long maxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListPositionsAsync(key, element, count, rank, maxLength);
    }

    public async Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ListSetByIndexAsync(key, index, value);
    }

    public async Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingLeftPopAsync(keys, timeout);
    }

    public async Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingRightPopAsync(keys, timeout);
    }

    public async Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingMoveAsync(source, destination, sourceSide, destinationSide, timeout);
    }

    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingPopAsync(keys, side, timeout);
    }

    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingPopAsync(keys, side, count, timeout);
    }

    #endregion
}
