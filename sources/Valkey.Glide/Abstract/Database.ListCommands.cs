// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    // ===== LPUSH/RPUSH with When parameter (SER-specific implementations) =====

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)"/>
    public Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when)
        => Command(Request.ListLeftPushAsync(key, value, when));

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    public Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when)
        => Command(Request.ListLeftPushAsync(key, [.. values], when));

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, When)"/>
    public Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when)
        => Command(Request.ListRightPushAsync(key, value, when));

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    public Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when)
        => Command(Request.ListRightPushAsync(key, [.. values], when));

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
    public Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPushAsync(key, value, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPushAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When, CommandFlags)"/>
    public Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPushAsync(key, values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLeftPushAsync(key, values);
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
    public Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPushAsync(key, value, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPushAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When, CommandFlags)"/>
    public Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPushAsync(key, values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRightPushAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRemoveAsync(ValkeyKey, ValkeyValue, long, CommandFlags)"/>
    public Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRemoveAsync(key, value, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListTrimAsync(ValkeyKey, long, long, CommandFlags)"/>
    public Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListTrimAsync(key, start, stop);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListRangeAsync(ValkeyKey, long, long, CommandFlags)"/>
    public Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListRangeAsync(key, start, stop);
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

    /// <inheritdoc cref="IDatabaseAsync.ListRightPopLeftPushAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> ListRightPopLeftPushAsync(ValkeyKey source, ValkeyKey destination, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListMoveAsync(source, destination, ListSide.Right, ListSide.Left);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListPositionAsync(ValkeyKey, ValkeyValue, long, long, CommandFlags)"/>
    public Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank, long maxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListPositionAsync(key, element, rank, maxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long, CommandFlags)"/>
    public Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank, long maxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListPositionsAsync(key, element, count, rank, maxLength);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>
    public Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListSetAsync(key, index, value);
    }

    // ===== LINDEX / LSET - SER-style naming (delegates to GLIDE-style) =====

    /// <inheritdoc cref="IDatabaseAsync.ListGetByIndexAsync(ValkeyKey, long)"/>
    public Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index)
        => ListIndexAsync(key, index);

    /// <inheritdoc cref="IDatabaseAsync.ListGetByIndexAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return ListIndexAsync(key, index);
    }

    /// <inheritdoc cref="IDatabaseAsync.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue)"/>
    public Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value)
        => ListSetAsync(key, index, value);
}
