// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchSetCommands
{
    /// <inheritdoc cref="ISetCommands.SetAddAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetAddAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetAdd(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="ISetCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SetAdd(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <inheritdoc cref="ISetCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetRemove(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="ISetCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SetRemove(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <inheritdoc cref="ISetCommands.SetMembersAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetMembersAsync(ValkeyKey)" /></returns>
    IBatch SetMembers(ValkeyKey key);

    /// <inheritdoc cref="ISetCommands.SetLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetLengthAsync(ValkeyKey)" /></returns>
    IBatch SetLength(ValkeyKey key);

    /// <inheritdoc cref="ISetCommands.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)" /></returns>
    IBatch SetIntersectionLength(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <inheritdoc cref="ISetCommands.SetPopAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetPopAsync(ValkeyKey)" /></returns>
    IBatch SetPop(ValkeyKey key);

    /// <inheritdoc cref="ISetCommands.SetPopAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetPopAsync(ValkeyKey, long)" /></returns>
    IBatch SetPop(ValkeyKey key, long count);

    /// <inheritdoc cref="ISetCommands.SetUnionAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetUnionAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetUnion(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetCommands.SetUnionAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetUnionAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetUnion(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetCommands.SetIntersectAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetIntersectAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetIntersect(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetCommands.SetIntersectAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetIntersectAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetIntersect(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetCommands.SetDifferenceAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetDifferenceAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetDifference(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetCommands.SetDifferenceAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetDifferenceAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetDifference(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetCommands.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetUnionStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetCommands.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetUnionStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetCommands.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetIntersectStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetCommands.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetIntersectStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetCommands.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetDifferenceStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetCommands.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetDifferenceStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetCommands.SetContainsAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetContainsAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetContains(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="ISetCommands.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SetContains(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <inheritdoc cref="ISetCommands.SetRandomMemberAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetRandomMemberAsync(ValkeyKey)" /></returns>
    IBatch SetRandomMember(ValkeyKey key);

    /// <inheritdoc cref="ISetCommands.SetRandomMembersAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetRandomMembersAsync(ValkeyKey, long)" /></returns>
    IBatch SetRandomMembers(ValkeyKey key, long count);

    /// <inheritdoc cref="ISetCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetMove(ValkeyKey source, ValkeyKey destination, ValkeyValue value);

    /// <inheritdoc cref="ISetCommands.SetScanAsync(ValkeyKey, ValkeyValue, int, long, int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetCommands.SetScanAsync(ValkeyKey, ValkeyValue, int, long, int)" /></returns>
    IBatch SetScan(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0);
}
