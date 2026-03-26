// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.ListLeftPopAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPopAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPopAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public async Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(keys, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)"/>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, value, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When, CommandFlags)"/>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPopAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPopAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPopAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public async Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(keys, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)"/>
    public async Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, value, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When, CommandFlags)"/>
    public async Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLengthAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRemoveAsync(ValkeyKey, ValkeyValue, long, CommandFlags)"/>
    public async Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRemoveAsync(key, value, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListTrimAsync(ValkeyKey, long, long, CommandFlags)"/>
    public async Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ListTrimAsync(key, start, stop);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRangeAsync(ValkeyKey, long, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRangeAsync(key, start, stop);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListGetByIndexAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListGetByIndexAsync(key, index);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListInsertAfterAsync(key, pivot, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public async Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListInsertBeforeAsync(key, pivot, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, CommandFlags)"/>
    public async Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListPositionAsync(ValkeyKey, ValkeyValue, long, long, CommandFlags)"/>
    public async Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank, long maxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListPositionAsync(key, element, rank, maxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long, CommandFlags)"/>
    public async Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank, long maxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListPositionsAsync(key, element, count, rank, maxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>
    public async Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ListSetByIndexAsync(key, index, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan, CommandFlags)"/>
    public async Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingLeftPopAsync(keys, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan, CommandFlags)"/>
    public async Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingRightPopAsync(keys, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan, CommandFlags)"/>
    public async Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingMoveAsync(source, destination, sourceSide, destinationSide, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan, CommandFlags)"/>
    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingPopAsync(keys, side, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan, CommandFlags)"/>
    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingPopAsync(keys, side, count, timeout);
    }
}
