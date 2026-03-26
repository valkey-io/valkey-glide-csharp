// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// List commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IListCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(key);
    }

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(key, count);
    }

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ListPopResult> ListLeftPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPopAsync(keys, count);
    }

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, value, when);
    }

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, values, when);
    }

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListLeftPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLeftPushAsync(key, values);
    }

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(key);
    }

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(key, count);
    }

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ListPopResult> ListRightPopAsync(IEnumerable<ValkeyKey> keys, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPopAsync(keys, count);
    }

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, value, when);
    }

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, values, when);
    }

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListRightPushAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRightPushAsync(key, values);
    }

    /// <inheritdoc cref="IListCommands.ListLengthAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListLengthAsync(key);
    }

    /// <inheritdoc cref="IListCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRemoveAsync(key, value, count);
    }

    /// <inheritdoc cref="IListCommands.ListTrimAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ListTrimAsync(key, start, stop);
    }

    /// <inheritdoc cref="IListCommands.ListRangeAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start, long stop, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListRangeAsync(key, start, stop);
    }

    /// <inheritdoc cref="IListCommands.ListGetByIndexAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListGetByIndexAsync(key, index);
    }

    /// <inheritdoc cref="IListCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListInsertAfterAsync(key, pivot, value);
    }

    /// <inheritdoc cref="IListCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListInsertBeforeAsync(key, pivot, value);
    }

    /// <inheritdoc cref="IListCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide);
    }

    /// <inheritdoc cref="IListCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank, long maxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListPositionAsync(key, element, rank, maxLength);
    }

    /// <inheritdoc cref="IListCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank, long maxLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListPositionsAsync(key, element, count, rank, maxLength);
    }

    /// <inheritdoc cref="IListCommands.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await ListSetByIndexAsync(key, index, value);
    }

    /// <inheritdoc cref="IListCommands.ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]?> ListBlockingLeftPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingLeftPopAsync(keys, timeout);
    }

    /// <inheritdoc cref="IListCommands.ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]?> ListBlockingRightPopAsync(IEnumerable<ValkeyKey> keys, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingRightPopAsync(keys, timeout);
    }

    /// <inheritdoc cref="IListCommands.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingMoveAsync(source, destination, sourceSide, destinationSide, timeout);
    }

    /// <inheritdoc cref="IListCommands.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingPopAsync(keys, side, timeout);
    }

    /// <inheritdoc cref="IListCommands.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ListPopResult> ListBlockingPopAsync(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await ListBlockingPopAsync(keys, side, count, timeout);
    }
}
