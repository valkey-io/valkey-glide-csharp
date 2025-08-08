// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchListCommands
{
    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch ListLeftPop(ValkeyKey key);

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey, long, CommandFlags)" /></returns>
    IBatch ListLeftPop(ValkeyKey key, long count);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)" /></returns>
    IBatch ListLeftPush(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)" /></returns>
    IBatch ListLeftPush(ValkeyKey key, ValkeyValue value, When when);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue[], When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue[], When, CommandFlags)" /></returns>
    IBatch ListLeftPush(ValkeyKey key, ValkeyValue[] values);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue[], When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue[], When, CommandFlags)" /></returns>
    IBatch ListLeftPush(ValkeyKey key, ValkeyValue[] values, When when);

    /// <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLeftPushAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch ListLeftPush(ValkeyKey key, ValkeyValue[] values, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch ListRightPop(ValkeyKey key);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey, long, CommandFlags)" /></returns>
    IBatch ListRightPop(ValkeyKey key, long count);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)" /></returns>
    IBatch ListRightPush(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue, When, CommandFlags)" /></returns>
    IBatch ListRightPush(ValkeyKey key, ValkeyValue value, When when);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue[], When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue[], When, CommandFlags)" /></returns>
    IBatch ListRightPush(ValkeyKey key, ValkeyValue[] values);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue[], When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue[], When, CommandFlags)" /></returns>
    IBatch ListRightPush(ValkeyKey key, ValkeyValue[] values, When when);

    /// <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRightPushAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch ListRightPush(ValkeyKey key, ValkeyValue[] values, CommandFlags flags);

    /// <inheritdoc cref="IListCommands.ListLengthAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLengthAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch ListLength(ValkeyKey key);

    /// <inheritdoc cref="IListCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRemoveAsync(ValkeyKey, ValkeyValue, long, CommandFlags)" /></returns>
    IBatch ListRemove(ValkeyKey key, ValkeyValue value, long count = 0);

    /// <inheritdoc cref="IListCommands.ListTrimAsync(ValkeyKey, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListTrimAsync(ValkeyKey, long, long, CommandFlags)" /></returns>
    IBatch ListTrim(ValkeyKey key, long start, long stop);

    /// <inheritdoc cref="IListCommands.ListRangeAsync(ValkeyKey, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRangeAsync(ValkeyKey, long, long, CommandFlags)" /></returns>
    IBatch ListRange(ValkeyKey key, long start = 0, long stop = -1);

    /// <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey[], long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListLeftPopAsync(ValkeyKey[], long, CommandFlags)" /></returns>
    IBatch ListLeftPop(ValkeyKey[] keys, long count);

    /// <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey[], long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListRightPopAsync(ValkeyKey[], long, CommandFlags)" /></returns>
    IBatch ListRightPop(ValkeyKey[] keys, long count);

    /// <inheritdoc cref="IListCommands.ListGetByIndexAsync(ValkeyKey, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListGetByIndexAsync(ValkeyKey, long, CommandFlags)" /></returns>
    IBatch ListGetByIndex(ValkeyKey key, long index);

    /// <inheritdoc cref="IListCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListInsertAfterAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)" /></returns>
    IBatch ListInsertAfter(ValkeyKey key, ValkeyValue pivot, ValkeyValue value);

    /// <inheritdoc cref="IListCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListInsertBeforeAsync(ValkeyKey, ValkeyValue, ValkeyValue, CommandFlags)" /></returns>
    IBatch ListInsertBefore(ValkeyKey key, ValkeyValue pivot, ValkeyValue value);

    /// <inheritdoc cref="IListCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, CommandFlags)" /></returns>
    IBatch ListMove(ValkeyKey sourceKey, ValkeyKey destinationKey, ListSide sourceSide, ListSide destinationSide);

    /// <inheritdoc cref="IListCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListPositionAsync(ValkeyKey, ValkeyValue, long, long, CommandFlags)" /></returns>
    IBatch ListPosition(ValkeyKey key, ValkeyValue element, long rank = 1, long maxLength = 0);

    /// <inheritdoc cref="IListCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListPositionsAsync(ValkeyKey, ValkeyValue, long, long, long, CommandFlags)" /></returns>
    IBatch ListPositions(ValkeyKey key, ValkeyValue element, long count, long rank = 1, long maxLength = 0);

    /// <inheritdoc cref="IListCommands.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListSetByIndexAsync(ValkeyKey, long, ValkeyValue, CommandFlags)" /></returns>
    IBatch ListSetByIndex(ValkeyKey key, long index, ValkeyValue value);

    /// <inheritdoc cref="IListCommands.ListBlockingLeftPopAsync(ValkeyKey[], TimeSpan, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListBlockingLeftPopAsync(ValkeyKey[], TimeSpan, CommandFlags)" /></returns>
    IBatch ListBlockingLeftPop(ValkeyKey[] keys, TimeSpan timeout);

    /// <inheritdoc cref="IListCommands.ListBlockingRightPopAsync(ValkeyKey[], TimeSpan, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListBlockingRightPopAsync(ValkeyKey[], TimeSpan, CommandFlags)" /></returns>
    IBatch ListBlockingRightPop(ValkeyKey[] keys, TimeSpan timeout);

    /// <inheritdoc cref="IListCommands.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListBlockingMoveAsync(ValkeyKey, ValkeyKey, ListSide, ListSide, TimeSpan, CommandFlags)" /></returns>
    IBatch ListBlockingMove(ValkeyKey source, ValkeyKey destination, ListSide sourceSide, ListSide destinationSide, TimeSpan timeout);

    /// <inheritdoc cref="IListCommands.ListBlockingPopAsync(ValkeyKey[], ListSide, TimeSpan, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListBlockingPopAsync(ValkeyKey[], ListSide, TimeSpan, CommandFlags)" /></returns>
    IBatch ListBlockingPop(ValkeyKey[] keys, ListSide side, TimeSpan timeout);

    /// <inheritdoc cref="IListCommands.ListBlockingPopAsync(ValkeyKey[], ListSide, long, TimeSpan, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IListCommands.ListBlockingPopAsync(ValkeyKey[], ListSide, long, TimeSpan, CommandFlags)" /></returns>
    IBatch ListBlockingPop(ValkeyKey[] keys, ListSide side, long count, TimeSpan timeout);
}
