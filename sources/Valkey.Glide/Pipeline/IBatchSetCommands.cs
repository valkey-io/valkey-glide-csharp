// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

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

    /// <inheritdoc cref="IBaseClient.SetMembersAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetMembersAsync(ValkeyKey)" /></returns>
    IBatch SetMembers(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.SetCardAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetCardAsync(ValkeyKey)" /></returns>
    IBatch SetCard(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.SetInterCardAsync(IEnumerable{ValkeyKey}, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetInterCardAsync(IEnumerable{ValkeyKey}, long)" /></returns>
    IBatch SetInterCard(IEnumerable<ValkeyKey> keys, long limit = 0);

    /// <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="ISetBaseCommands.SetPopAsync(ValkeyKey)" /></returns>
    IBatch SetPop(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.SetPopAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetPopAsync(ValkeyKey, long)" /></returns>
    IBatch SetPop(ValkeyKey key, long count);

    /// <inheritdoc cref="IBaseClient.SetUnionAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetUnionAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetUnion(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IBaseClient.SetUnionAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetUnionAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetUnion(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SetInterAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetInterAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetInter(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IBaseClient.SetInterAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetInterAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetInter(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SetDiffAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetDiffAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetDiff(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IBaseClient.SetDiffAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetDiffAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch SetDiff(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetUnionStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IBaseClient.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetUnionStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SetInterStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetInterStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetInterStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IBaseClient.SetInterStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetInterStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetInterStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SetDiffStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetDiffStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetDiffStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="IBaseClient.SetDiffStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetDiffStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})" /></returns>
    IBatch SetDiffStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SetIsMemberAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetIsMemberAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch SetIsMember(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="IBaseClient.SetIsMemberAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetIsMemberAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch SetIsMember(ValkeyKey key, IEnumerable<ValkeyValue> values);

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
    /// <param name="cursor">The cursor position to start at (use 0 to start a new iteration).</param>
    /// <param name="options">Optional scan options including pattern and count hint.</param>
    /// <returns>Command Response - A tuple of (cursor, elements) for the current page.</returns>
    IBatch SetScan(ValkeyKey key, long cursor = 0, ScanOptions? options = null);
}
