// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// List commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchListCommands.ListLeftPop(ValkeyKey)" />
    public T ListLeftPop(ValkeyKey key) => AddCmd(Request.ListLeftPop(key));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPop(ValkeyKey, long)" />
    public T ListLeftPop(ValkeyKey key, long count) => AddCmd(Request.ListLeftPop(key, count));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, ValkeyValue)" />
    public T ListLeftPush(ValkeyKey key, ValkeyValue value) => AddCmd(Request.ListLeftPush(key, value, When.Always));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, ValkeyValue, When)" />
    public T ListLeftPush(ValkeyKey key, ValkeyValue value, When when) => AddCmd(Request.ListLeftPush(key, value, when));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T ListLeftPush(ValkeyKey key, IEnumerable<ValkeyValue> values) => AddCmd(Request.ListLeftPush(key, [.. values], When.Always));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPush(ValkeyKey, IEnumerable{ValkeyValue}, When)" />
    public T ListLeftPush(ValkeyKey key, IEnumerable<ValkeyValue> values, When when) => AddCmd(Request.ListLeftPush(key, [.. values], when));

    /// <inheritdoc cref="IBatchListCommands.ListRightPop(ValkeyKey)" />
    public T ListRightPop(ValkeyKey key) => AddCmd(Request.ListRightPop(key));

    /// <inheritdoc cref="IBatchListCommands.ListRightPop(ValkeyKey, long)" />
    public T ListRightPop(ValkeyKey key, long count) => AddCmd(Request.ListRightPop(key, count));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, ValkeyValue)" />
    public T ListRightPush(ValkeyKey key, ValkeyValue value) => AddCmd(Request.ListRightPush(key, value, When.Always));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, ValkeyValue, When)" />
    public T ListRightPush(ValkeyKey key, ValkeyValue value, When when) => AddCmd(Request.ListRightPush(key, value, when));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T ListRightPush(ValkeyKey key, IEnumerable<ValkeyValue> values) => AddCmd(Request.ListRightPush(key, [.. values], When.Always));

    /// <inheritdoc cref="IBatchListCommands.ListRightPush(ValkeyKey, IEnumerable{ValkeyValue}, When)" />
    public T ListRightPush(ValkeyKey key, IEnumerable<ValkeyValue> values, When when) => AddCmd(Request.ListRightPush(key, [.. values], when));

    /// <inheritdoc cref="IBatchListCommands.ListLength(ValkeyKey)" />
    public T ListLength(ValkeyKey key) => AddCmd(Request.ListLength(key));

    /// <inheritdoc cref="IBatchListCommands.ListRemove(ValkeyKey, ValkeyValue, long)" />
    public T ListRemove(ValkeyKey key, ValkeyValue value, long count = 0) => AddCmd(Request.ListRemove(key, value, count));

    /// <inheritdoc cref="IBatchListCommands.ListTrim(ValkeyKey, long, long)" />
    public T ListTrim(ValkeyKey key, long start = 0, long stop = -1) => AddCmd(Request.ListTrim(key, start, stop));

    /// <inheritdoc cref="IBatchListCommands.ListRange(ValkeyKey, long, long)" />
    public T ListRange(ValkeyKey key, long start = 0, long stop = -1) => AddCmd(Request.ListRange(key, start, stop));

    /// <inheritdoc cref="IBatchListCommands.ListLeftPop(IEnumerable{ValkeyKey}, long)" />
    public T ListLeftPop(IEnumerable<ValkeyKey> keys, long count) => AddCmd(Request.ListLeftPop([.. keys], count));

    /// <inheritdoc cref="IBatchListCommands.ListRightPop(IEnumerable{ValkeyKey}, long)" />
    public T ListRightPop(IEnumerable<ValkeyKey> keys, long count) => AddCmd(Request.ListRightPop([.. keys], count));

    /// <inheritdoc cref="IBatchListCommands.ListIndex(ValkeyKey, long)" />
    public T ListIndex(ValkeyKey key, long index) => AddCmd(Request.ListGetByIndex(key, index));

    /// <inheritdoc cref="IBatchListCommands.ListInsertAfter(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T ListInsertAfter(ValkeyKey key, ValkeyValue pivot, ValkeyValue value) => AddCmd(Request.ListInsertAfter(key, pivot, value));

    /// <inheritdoc cref="IBatchListCommands.ListInsertBefore(ValkeyKey, ValkeyValue, ValkeyValue)" />
    public T ListInsertBefore(ValkeyKey key, ValkeyValue pivot, ValkeyValue value) => AddCmd(Request.ListInsertBefore(key, pivot, value));

    /// <inheritdoc cref="IBatchListCommands.ListMove(ValkeyKey, ValkeyKey, ListSide, ListSide)" />
    public T ListMove(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide) => AddCmd(Request.ListMove(sourceKey, destinationKey, sourceSide, destinationSide));

    /// <inheritdoc cref="IBatchListCommands.ListPosition(ValkeyKey, ValkeyValue, long, long)" />
    public T ListPosition(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0) => AddCmd(Request.ListPosition(key, element, rank, maxLength));

    /// <inheritdoc cref="IBatchListCommands.ListPositions(ValkeyKey, ValkeyValue, long, long, long)" />
    public T ListPositions(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0) => AddCmd(Request.ListPositions(key, element, count, rank, maxLength));

    /// <inheritdoc cref="IBatchListCommands.ListSet(ValkeyKey, long, ValkeyValue)" />
    public T ListSet(ValkeyKey key, long index, ValkeyValue value) => AddCmd(Request.ListSetByIndex(key, index, value));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingLeftPop(IEnumerable{ValkeyKey}, TimeSpan)" />
    public T ListBlockingLeftPop(IEnumerable<ValkeyKey> keys, TimeSpan timeout) => AddCmd(Request.ListBlockingLeftPop([.. keys], timeout));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingRightPop(IEnumerable{ValkeyKey}, TimeSpan)" />
    public T ListBlockingRightPop(IEnumerable<ValkeyKey> keys, TimeSpan timeout) => AddCmd(Request.ListBlockingRightPop([.. keys], timeout));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingMove(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan)" />
    public T ListBlockingMove(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout) => AddCmd(Request.ListBlockingMove(source, destination, sourceSide, destinationSide, timeout));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingPop(IEnumerable{ValkeyKey}, ListSide, TimeSpan)" />
    public T ListBlockingPop(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout) => AddCmd(Request.ListBlockingPop([.. keys], side, timeout));

    /// <inheritdoc cref="IBatchListCommands.ListBlockingPop(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan)" />
    public T ListBlockingPop(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout) => AddCmd(Request.ListBlockingPop([.. keys], side, count, timeout));

    // Explicit interface implementations for IBatchListCommands
    IBatch IBatchListCommands.ListLeftPop(ValkeyKey key) => ListLeftPop(key);
    IBatch IBatchListCommands.ListLeftPop(ValkeyKey key, long count) => ListLeftPop(key, count);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, ValkeyValue value) => ListLeftPush(key, value);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, ValkeyValue value, When when) => ListLeftPush(key, value, when);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, IEnumerable<ValkeyValue> values) => ListLeftPush(key, values);
    IBatch IBatchListCommands.ListLeftPush(ValkeyKey key, IEnumerable<ValkeyValue> values, When when) => ListLeftPush(key, values, when);
    IBatch IBatchListCommands.ListRightPop(ValkeyKey key) => ListRightPop(key);
    IBatch IBatchListCommands.ListRightPop(ValkeyKey key, long count) => ListRightPop(key, count);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, ValkeyValue value) => ListRightPush(key, value);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, ValkeyValue value, When when) => ListRightPush(key, value, when);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, IEnumerable<ValkeyValue> values) => ListRightPush(key, values);
    IBatch IBatchListCommands.ListRightPush(ValkeyKey key, IEnumerable<ValkeyValue> values, When when) => ListRightPush(key, values, when);
    IBatch IBatchListCommands.ListLength(ValkeyKey key) => ListLength(key);
    IBatch IBatchListCommands.ListRemove(ValkeyKey key, ValkeyValue value, long count) => ListRemove(key, value, count);
    IBatch IBatchListCommands.ListTrim(ValkeyKey key, long start, long stop) => ListTrim(key, start, stop);
    IBatch IBatchListCommands.ListRange(ValkeyKey key, long start, long stop) => ListRange(key, start, stop);
    IBatch IBatchListCommands.ListLeftPop(IEnumerable<ValkeyKey> keys, long count) => ListLeftPop(keys, count);
    IBatch IBatchListCommands.ListRightPop(IEnumerable<ValkeyKey> keys, long count) => ListRightPop(keys, count);
    IBatch IBatchListCommands.ListIndex(ValkeyKey key, long index) => ListIndex(key, index);
    IBatch IBatchListCommands.ListInsertAfter(ValkeyKey key, ValkeyValue pivot, ValkeyValue value) => ListInsertAfter(key, pivot, value);
    IBatch IBatchListCommands.ListInsertBefore(ValkeyKey key, ValkeyValue pivot, ValkeyValue value) => ListInsertBefore(key, pivot, value);
    IBatch IBatchListCommands.ListMove(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide) => ListMove(sourceKey, destinationKey, sourceSide, destinationSide);
    IBatch IBatchListCommands.ListPosition(ValkeyKey key, ValkeyValue element, long rank, long maxLength) => ListPosition(key, element, rank, maxLength);
    IBatch IBatchListCommands.ListPositions(ValkeyKey key, ValkeyValue element, long count, long rank, long maxLength) => ListPositions(key, element, count, rank, maxLength);
    IBatch IBatchListCommands.ListSet(ValkeyKey key, long index, ValkeyValue value) => ListSet(key, index, value);
    IBatch IBatchListCommands.ListBlockingLeftPop(IEnumerable<ValkeyKey> keys, TimeSpan timeout) => ListBlockingLeftPop(keys, timeout);
    IBatch IBatchListCommands.ListBlockingRightPop(IEnumerable<ValkeyKey> keys, TimeSpan timeout) => ListBlockingRightPop(keys, timeout);
    IBatch IBatchListCommands.ListBlockingMove(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout) => ListBlockingMove(source, destination, sourceSide, destinationSide, timeout);
    IBatch IBatchListCommands.ListBlockingPop(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout) => ListBlockingPop(keys, side, timeout);
    IBatch IBatchListCommands.ListBlockingPop(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout) => ListBlockingPop(keys, side, count, timeout);
}
