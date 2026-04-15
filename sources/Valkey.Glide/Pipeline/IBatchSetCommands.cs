// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide.Pipeline;

internal interface IBatchSetCommands
{
    /// <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetAdd(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SetAdd(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetRemove(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SetRemove(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <inheritdoc cref="ISetBaseCommands.SetMembersAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetMembersAsync(ValkeyKey)" /></returns>
    IBatch SetMembers(ValkeyKey key);

    /// <inheritdoc cref="ISetBaseCommands.SetLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetLengthAsync(ValkeyKey)" /></returns>
    IBatch SetLength(ValkeyKey key);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)" /></returns>
    IBatch SetIntersectionLength(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey)" /></returns>
    IBatch SetPop(ValkeyKey key);

    /// <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey, long)" /></returns>
    IBatch SetPop(ValkeyKey key, long count);

    /// <inheritdoc cref="ISetBaseCommands.SetUnionAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetUnionAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetUnion(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetBaseCommands.SetUnionAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetUnionAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetUnion(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetIntersectAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetIntersect(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetIntersectAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetIntersect(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetBaseCommands.SetDifferenceAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetDifferenceAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetDifference(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetBaseCommands.SetDifferenceAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetDifferenceAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetDifference(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetBaseCommands.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetUnionStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetBaseCommands.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetUnionStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetIntersectStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetBaseCommands.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetIntersectStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetBaseCommands.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)" /></returns>
    IBatch SetDifferenceStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="ISetBaseCommands.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetDifferenceStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="ISetBaseCommands.SetContainsAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetContainsAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetContains(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="ISetBaseCommands.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SetContains(ValkeyKey key, IEnumerable<ValkeyValue> values);

    /// <inheritdoc cref="ISetBaseCommands.SetRandomMemberAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetRandomMemberAsync(ValkeyKey)" /></returns>
    IBatch SetRandomMember(ValkeyKey key);

    /// <inheritdoc cref="ISetBaseCommands.SetRandomMembersAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetRandomMembersAsync(ValkeyKey, long)" /></returns>
    IBatch SetRandomMembers(ValkeyKey key, long count);

    /// <inheritdoc cref="ISetBaseCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetMove(ValkeyKey source, ValkeyKey destination, ValkeyValue value);

    /// <summary>
    /// Iterates elements over a set (single page).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/sscan"/>
    /// <param name="key">The key of the set.</param>
    /// <param name="pattern">The pattern to match.</param>
    /// <param name="pageSize">The number of elements to return per iteration (hint to the server).</param>
    /// <param name="cursor">The cursor position to start at.</param>
    /// <returns>Command Response - A tuple of (cursor, elements) for the current page.</returns>
    IBatch SetScan(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0);
}
