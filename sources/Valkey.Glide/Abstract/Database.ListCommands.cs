// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.ListLeftPopAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPopAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPopAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPopAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPopAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPopAsync(keys, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)"/>
    public Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPushAsync(key, value, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When, CommandFlags)"/>
    public Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPushAsync(key, values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPopAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPopAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPopAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPopAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPopAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPopAsync(keys, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)"/>
    public Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPushAsync(key, value, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When, CommandFlags)"/>
    public Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPushAsync(key, values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRemoveAsync(ValkeyKey, ValkeyValue, long, CommandFlags)"/>
    public Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRemoveAsync(key, value, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListTrimAsync(ValkeyKey, long, long, CommandFlags)"/>
    public async Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ListTrimAsync(key, start, stop);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRangeAsync(ValkeyKey, long, long, CommandFlags)"/>
    public Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRangeAsync(key, start, stop);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListGetByIndexAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListGetByIndexAsync(key, index);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListInsertAfterAsync(key, pivot, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)"/>
    public Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListInsertBeforeAsync(key, pivot, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, CommandFlags)"/>
    public Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListPositionAsync(ValkeyKey, ValkeyValue, long, long, CommandFlags)"/>
    public Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListPositionAsync(key, element, rank, maxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long, CommandFlags)"/>
    public Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListPositionsAsync(key, element, count, rank, maxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>
    public async Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ListSetByIndexAsync(key, index, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan, CommandFlags)"/>
    public Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListBlockingLeftPopAsync(keys, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan, CommandFlags)"/>
    public Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListBlockingRightPopAsync(keys, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan, CommandFlags)"/>
    public Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListBlockingMoveAsync(source, destination, sourceSide, destinationSide, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan, CommandFlags)"/>
    public Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListBlockingPopAsync(keys, side, timeout);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan, CommandFlags)"/>
    public Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListBlockingPopAsync(keys, side, count, timeout);
    }
}
