// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchListCommands
{
    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(ValkeyKey)" /></returns>
    IBatch ListLeftPop(ValkeyKey key);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(ValkeyKey, long)" /></returns>
    IBatch ListLeftPop(ValkeyKey key, long count);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch ListLeftPush(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, ValkeyValue, When)" /></returns>
    IBatch ListLeftPush(ValkeyKey key, ValkeyValue value, When when);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch ListLeftPush(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.ListLeftPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)" /></returns>
    IBatch ListLeftPush(ValkeyKey key, IEnumerable<ValkeyValue> values, When when);



    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListRightPopAsync(ValkeyKey)" /></returns>
    IBatch ListRightPop(ValkeyKey key);

    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListRightPopAsync(ValkeyKey, long)" /></returns>
    IBatch ListRightPop(ValkeyKey key, long count);

    /// <inheritdoc cref="IListBaseCommands.ListRightPushAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListRightPushAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch ListRightPush(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, When)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, ValkeyValue, When)" /></returns>
    IBatch ListRightPush(ValkeyKey key, ValkeyValue value, When when);

    /// <inheritdoc cref="IListBaseCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch ListRightPush(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IDatabaseAsync.ListRightPushAsync(ValkeyKey, IEnumerable{ValkeyValue}, When)" /></returns>
    IBatch ListRightPush(ValkeyKey key, IEnumerable<ValkeyValue> values, When when);



    /// <inheritdoc cref="IListBaseCommands.ListLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListLengthAsync(ValkeyKey)" /></returns>
    IBatch ListLength(ValkeyKey key);

    /// <inheritdoc cref="IListBaseCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long)" /></returns>
    IBatch ListRemove(ValkeyKey key, ValkeyValue value, long count = 0);

    /// <inheritdoc cref="IListBaseCommands.ListTrimAsync(ValkeyKey, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListTrimAsync(ValkeyKey, long, long)" /></returns>
    IBatch ListTrim(ValkeyKey key, long start, long stop);

    /// <inheritdoc cref="IListBaseCommands.ListRangeAsync(ValkeyKey, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListRangeAsync(ValkeyKey, long, long)" /></returns>
    IBatch ListRange(ValkeyKey key, long start = 0, long stop = -1);

    /// <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListLeftPopAsync(IEnumerable{ValkeyKey}, long)" /></returns>
    IBatch ListLeftPop(IEnumerable<ValkeyKey> keys, long count);

    /// <inheritdoc cref="IListBaseCommands.ListRightPopAsync(IEnumerable{ValkeyKey}, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListRightPopAsync(IEnumerable{ValkeyKey}, long)" /></returns>
    IBatch ListRightPop(IEnumerable<ValkeyKey> keys, long count);

    /// <inheritdoc cref="IListBaseCommands.ListGetByIndexAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListGetByIndexAsync(ValkeyKey, long)" /></returns>
    IBatch ListGetByIndex(ValkeyKey key, long index);

    /// <inheritdoc cref="IListBaseCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch ListInsertAfter(ValkeyKey key, ValkeyValue pivot, ValkeyValue value);

    /// <inheritdoc cref="IListBaseCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch ListInsertBefore(ValkeyKey key, ValkeyValue pivot, ValkeyValue value);

    /// <inheritdoc cref="IListBaseCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide)" /></returns>
    IBatch ListMove(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide);

    /// <inheritdoc cref="IListBaseCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long)" /></returns>
    IBatch ListPosition(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0);

    /// <inheritdoc cref="IListBaseCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long)" /></returns>
    IBatch ListPositions(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0);

    /// <inheritdoc cref="IListBaseCommands.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListBaseCommands.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue)" /></returns>
    IBatch ListSetByIndex(ValkeyKey key, long index, ValkeyValue value);

    /// <inheritdoc cref="IBaseClient.ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ListBlockingLeftPopAsync(IEnumerable{ValkeyKey}, TimeSpan)" /></returns>
    IBatch ListBlockingLeftPop(IEnumerable<ValkeyKey> keys, TimeSpan timeout);

    /// <inheritdoc cref="IBaseClient.ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ListBlockingRightPopAsync(IEnumerable{ValkeyKey}, TimeSpan)" /></returns>
    IBatch ListBlockingRightPop(IEnumerable<ValkeyKey> keys, TimeSpan timeout);

    /// <inheritdoc cref="IBaseClient.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan)" /></returns>
    IBatch ListBlockingMove(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout);

    /// <inheritdoc cref="IBaseClient.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, TimeSpan)" /></returns>
    IBatch ListBlockingPop(IEnumerable<ValkeyKey> keys, ListSide side, TimeSpan timeout);

    /// <inheritdoc cref="IBaseClient.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.ListBlockingPopAsync(IEnumerable{ValkeyKey}, ListSide, long, TimeSpan)" /></returns>
    IBatch ListBlockingPop(IEnumerable<ValkeyKey> keys, ListSide side, long count, TimeSpan timeout);
}
