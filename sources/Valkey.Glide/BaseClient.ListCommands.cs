// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IListCommands
{
    public async Task<ValkeyValue> ListLeftPopAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListLeftPopAsync(key));
    }

    public async Task<ValkeyValue[]?> ListLeftPopAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListLeftPopAsync(key, count));
    }

    public async Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListLeftPushAsync(key, value, when));
    }

    public async Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListLeftPushAsync(key, values, when));
    }

    public async Task<long> ListLeftPushAsync(ValkeyKey key, ValkeyValue[] values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListLeftPushAsync(key, values));
    }

    public async Task<ValkeyValue> ListRightPopAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListRightPopAsync(key));
    }

    public async Task<ValkeyValue[]?> ListRightPopAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListRightPopAsync(key, count));
    }

    public async Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListRightPushAsync(key, value, when));
    }

    public async Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListRightPushAsync(key, values, when));
    }

    public async Task<long> ListRightPushAsync(ValkeyKey key, ValkeyValue[] values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListRightPushAsync(key, values));
    }

    public async Task<long> ListLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListLengthAsync(key));
    }

    public async Task<long> ListRemoveAsync(ValkeyKey key, ValkeyValue value, long count = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListRemoveAsync(key, value, count));
    }

    public async Task ListTrimAsync(ValkeyKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.ListTrimAsync(key, start, stop));
    }

    public async Task<ValkeyValue[]> ListRangeAsync(ValkeyKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListRangeAsync(key, start, stop));
    }

    public async Task<ListPopResult> ListLeftPopAsync(ValkeyKey[] keys, long count, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListLeftPopAsync(keys, count));
    }

    public async Task<ListPopResult> ListRightPopAsync(ValkeyKey[] keys, long count, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListRightPopAsync(keys, count));
    }

    public async Task<ValkeyValue> ListGetByIndexAsync(ValkeyKey key, long index, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListGetByIndexAsync(key, index));
    }

    public async Task<long> ListInsertAfterAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListInsertAfterAsync(key, pivot, value));
    }

    public async Task<long> ListInsertBeforeAsync(ValkeyKey key, ValkeyValue pivot, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListInsertBeforeAsync(key, pivot, value));
    }

    public async Task<ValkeyValue> ListMoveAsync(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide));
    }

    public async Task<long> ListPositionAsync(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListPositionAsync(key, element, rank, maxLength));
    }

    public async Task<long[]> ListPositionsAsync(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListPositionsAsync(key, element, count, rank, maxLength));
    }

    public async Task ListSetByIndexAsync(ValkeyKey key, long index, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.ListSetByIndexAsync(key, index, value));
    }

    // Blocking list operations (Blocking Commands aren't supported in SER so this is according to our own implementation)

    public async Task<ValkeyValue[]?> ListBlockingLeftPopAsync(ValkeyKey[] keys, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListBlockingLeftPopAsync(keys, timeout));
    }

    public async Task<ValkeyValue[]?> ListBlockingRightPopAsync(ValkeyKey[] keys, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListBlockingRightPopAsync(keys, timeout));
    }

    public async Task<ValkeyValue> ListBlockingMoveAsync(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListBlockingMoveAsync(source, destination, sourceSide, destinationSide, timeout));
    }

    public async Task<ListPopResult> ListBlockingPopAsync(ValkeyKey[] keys, ListSide side, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListBlockingPopAsync(keys, side, timeout));
    }

    public async Task<ListPopResult> ListBlockingPopAsync(ValkeyKey[] keys, ListSide side, long count, TimeSpan timeout, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.ListBlockingPopAsync(keys, side, count, timeout));
    }
}
