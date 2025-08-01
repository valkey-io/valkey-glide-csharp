// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// List commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchListCommands.ListLeftPop(ValkeyKey)" />
    public T ListLeftPop(ValkeyKey key) => AddCmd(ListLeftPopAsync(key));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPop(ValkeyKey, long)" />
    public T ListLeftPop(ValkeyKey key, long count) => AddCmd(ListLeftPopAsync(key, count));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, ValkeyValue)" />
    public T ListLeftPush(ValkeyKey key, ValkeyValue value) => AddCmd(ListLeftPushAsync(key, value, When.Always));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, ValkeyValue, When)" />
    public T ListLeftPush(ValkeyKey key, ValkeyValue value, When when) => AddCmd(ListLeftPushAsync(key, value, when));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, ValkeyValue[])" />
    public T ListLeftPush(ValkeyKey key, ValkeyValue[] values) => AddCmd(ListLeftPushAsync(key, values, When.Always));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, ValkeyValue[], When)" />
    public T ListLeftPush(ValkeyKey key, ValkeyValue[] values, When when) => AddCmd(ListLeftPushAsync(key, values, when));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, ValkeyValue[], CommandFlags)" />
    public T ListLeftPush(ValkeyKey key, ValkeyValue[] values, CommandFlags _) => AddCmd(ListLeftPushAsync(key, values));

    /// <inheritdoc cref="IBatchListCommands.ListRightPop(ValkeyKey)" />
    public T ListRightPop(ValkeyKey key) => AddCmd(ListRightPopAsync(key));

    /// <inheritdoc cref="IBatchListCommands.ListRightPop(ValkeyKey, long)" />
    public T ListRightPop(ValkeyKey key, long count) => AddCmd(ListRightPopAsync(key, count));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, ValkeyValue)" />
    public T ListRightPush(ValkeyKey key, ValkeyValue value) => AddCmd(ListRightPushAsync(key, value, When.Always));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, ValkeyValue, When)" />
    public T ListRightPush(ValkeyKey key, ValkeyValue value, When when) => AddCmd(ListRightPushAsync(key, value, when));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, ValkeyValue[])" />
    public T ListRightPush(ValkeyKey key, ValkeyValue[] values) => AddCmd(ListRightPushAsync(key, values, When.Always));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, ValkeyValue[], When)" />
    public T ListRightPush(ValkeyKey key, ValkeyValue[] values, When when) => AddCmd(ListRightPushAsync(key, values, when));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, ValkeyValue[], CommandFlags)" />
    public T ListRightPush(ValkeyKey key, ValkeyValue[] values, CommandFlags _) => AddCmd(ListRightPushAsync(key, values));

    /// <inheritdoc cref="IBatchListCommands.ListLength(ValkeyKey)" />
    public T ListLength(ValkeyKey key) => AddCmd(ListLengthAsync(key));

    /// <inheritdoc cref="IBatchListCommands.ListRemove(ValkeyKey, ValkeyValue, long)" />
    public T ListRemove(ValkeyKey key, ValkeyValue value, long count = 0) => AddCmd(ListRemoveAsync(key, value, count));

    /// <inheritdoc cref="IBatchListCommands.ListTrim(ValkeyKey, long, long)" />
    public T ListTrim(ValkeyKey key, long start, long stop) => AddCmd(ListTrimAsync(key, start, stop));

    /// <inheritdoc cref="IBatchListCommands.ListRange(ValkeyKey, long, long)" />
    public T ListRange(ValkeyKey key, long start = 0, long stop = -1) => AddCmd(ListRangeAsync(key, start, stop));

    // New list commands

    /// <inheritdoc cref="IBatchListCommands.ListLeftPop(ValkeyKey[], long)" />
    public T ListLeftPop(ValkeyKey[] keys, long count) => AddCmd(ListLeftPopAsync(keys, count));

    /// <inheritdoc cref="IBatchListCommands.ListRightPop(ValkeyKey[], long)" />
    public T ListRightPop(ValkeyKey[] keys, long count) => AddCmd(ListRightPopAsync(keys, count));

    /// <inheritdoc cref="IBatchListCommands.ListGetByIndex(ValkeyKey, long)" />
    public T ListGetByIndex(ValkeyKey key, long index) => AddCmd(ListGetByIndexAsync(key, index));

    /// <inheritdoc cref="IBatchListCommands.ListInsertAfter(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T ListInsertAfter(ValkeyKey key, ValkeyValue pivot, ValkeyValue value) => AddCmd(ListInsertAfterAsync(key, pivot, value));

    /// <inheritdoc cref="IBatchListCommands.ListInsertBefore(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T ListInsertBefore(ValkeyKey key, ValkeyValue pivot, ValkeyValue value) => AddCmd(ListInsertBeforeAsync(key, pivot, value));

    /// <inheritdoc cref="IBatchListCommands.ListMove(ValkeyKey, ValkeyKey, ListSide, ListSide)" />
    public T ListMove(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide) => AddCmd(ListMoveAsync(sourceKey, destinationKey, sourceSide, destinationSide));

    /// <inheritdoc cref="IBatchListCommands.ListPosition(ValkeyKey, ValkeyValue, long, long)" />
    public T ListPosition(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0) => AddCmd(ListPositionAsync(key, element, rank, maxLength));

    /// <inheritdoc cref="IBatchListCommands.ListPositions(ValkeyKey, ValkeyValue, long, long, long)" />
    public T ListPositions(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0) => AddCmd(ListPositionsAsync(key, element, count, rank, maxLength));

    /// <inheritdoc cref="IBatchListCommands.ListSetByIndex(ValkeyKey, long, ValkeyValue)" />
    public T ListSetByIndex(ValkeyKey key, long index, ValkeyValue value) => AddCmd(ListSetByIndexAsync(key, index, value));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingLeftPop(ValkeyKey[], TimeSpan)" />
    public T ListBlockingLeftPop(ValkeyKey[] keys, TimeSpan timeout) => AddCmd(ListBlockingLeftPopAsync(keys, timeout));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingRightPop(ValkeyKey[], TimeSpan)" />
    public T ListBlockingRightPop(ValkeyKey[] keys, TimeSpan timeout) => AddCmd(ListBlockingRightPopAsync(keys, timeout));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingMove(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan)" />
    public T ListBlockingMove(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout) => AddCmd(ListBlockingMoveAsync(source, destination, sourceSide, destinationSide, timeout));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingPop(ValkeyKey[], ListSide, TimeSpan)" />
    public T ListBlockingPop(ValkeyKey[] keys, ListSide side, TimeSpan timeout) => AddCmd(ListBlockingPopAsync(keys, side, timeout));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingPop(ValkeyKey[], ListSide, long, TimeSpan)" />
    public T ListBlockingPop(ValkeyKey[] keys, ListSide side, long count, TimeSpan timeout) => AddCmd(ListBlockingPopAsync(keys, side, count, timeout));

    // Explicit interface implementations for IBatchListCommands
    IBatch IBatchListCommands.ListLeftPop(ValkeyKey key) => ListLeftPop(key);
    IBatch IBatchListCommands.ListLeftPop(ValkeyKey key, long count) => ListLeftPop(key, count);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, ValkeyValue value) => ListLeftPush(key, value);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, ValkeyValue value, When when) => ListLeftPush(key, value, when);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, ValkeyValue[] values) => ListLeftPush(key, values);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, ValkeyValue[] values, When when) => ListLeftPush(key, values, when);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, ValkeyValue[] values, CommandFlags flags) => ListLeftPush(key, values, flags);
    IBatch IBatchListCommands.ListRightPop(ValkeyKey key) => ListRightPop(key);
    IBatch IBatchListCommands.ListRightPop(ValkeyKey key, long count) => ListRightPop(key, count);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, ValkeyValue value) => ListRightPush(key, value);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, ValkeyValue value, When when) => ListRightPush(key, value, when);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, ValkeyValue[] values) => ListRightPush(key, values);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, ValkeyValue[] values, When when) => ListRightPush(key, values, when);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, ValkeyValue[] values, CommandFlags flags) => ListRightPush(key, values, flags);
    IBatch IBatchListCommands.ListLength(ValkeyKey key) => ListLength(key);
    IBatch IBatchListCommands.ListRemove(ValkeyKey key, ValkeyValue value, long count) => ListRemove(key, value, count);
    IBatch IBatchListCommands.ListTrim(ValkeyKey key, long start, long stop) => ListTrim(key, start, stop);
    IBatch IBatchListCommands.ListRange(ValkeyKey key, long start, long stop) => ListRange(key, start, stop);

    // New list commands explicit interface implementations
    IBatch IBatchListCommands.ListLeftPop(ValkeyKey[] keys, long count) => ListLeftPop(keys, count);
    IBatch IBatchListCommands.ListRightPop(ValkeyKey[] keys, long count) => ListRightPop(keys, count);
    IBatch IBatchListCommands.ListGetByIndex(ValkeyKey key, long index) => ListGetByIndex(key, index);
    IBatch IBatchListCommands.ListInsertAfter(ValkeyKey key, ValkeyValue pivot, ValkeyValue value) => ListInsertAfter(key, pivot, value);
    IBatch IBatchListCommands.ListInsertBefore(ValkeyKey key, ValkeyValue pivot, ValkeyValue value) => ListInsertBefore(key, pivot, value);
    IBatch IBatchListCommands.ListMove(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide) => ListMove(sourceKey, destinationKey, sourceSide, destinationSide);
    IBatch IBatchListCommands.ListPosition(ValkeyKey key, ValkeyValue element, long rank, long maxLength) => ListPosition(key, element, rank, maxLength);
    IBatch IBatchListCommands.ListPositions(ValkeyKey key, ValkeyValue element, long count, long rank, long maxLength) => ListPositions(key, element, count, rank, maxLength);
    IBatch IBatchListCommands.ListSetByIndex(ValkeyKey key, long index, ValkeyValue value) => ListSetByIndex(key, index, value);

    // Blocking list operations explicit interface implementations
    IBatch IBatchListCommands.ListBlockingLeftPop(ValkeyKey[] keys, TimeSpan timeout) => ListBlockingLeftPop(keys, timeout);
    IBatch IBatchListCommands.ListBlockingRightPop(ValkeyKey[] keys, TimeSpan timeout) => ListBlockingRightPop(keys, timeout);
    IBatch IBatchListCommands.ListBlockingMove(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout) => ListBlockingMove(source, destination, sourceSide, destinationSide, timeout);
    IBatch IBatchListCommands.ListBlockingPop(ValkeyKey[] keys, ListSide side, TimeSpan timeout) => ListBlockingPop(keys, side, timeout);
    IBatch IBatchListCommands.ListBlockingPop(ValkeyKey[] keys, ListSide side, long count, TimeSpan timeout) => ListBlockingPop(keys, side, count, timeout);
}
