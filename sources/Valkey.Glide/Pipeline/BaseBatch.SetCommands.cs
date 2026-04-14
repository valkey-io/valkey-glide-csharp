// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Set commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchSetCommands.SetAdd(ValkeyKey, ValkeyValue)" />
    public T SetAdd(ValkeyKey key, ValkeyValue value) => AddCmd(SetAddAsync(key, value));

    /// <inheritdoc cref="IBatchSetCommands.SetAdd(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T SetAdd(ValkeyKey key, IEnumerable<ValkeyValue> values) => AddCmd(SetAddAsync(key, [.. values]));

    /// <inheritdoc cref="IBatchSetCommands.SetRemove(ValkeyKey, ValkeyValue)" />
    public T SetRemove(ValkeyKey key, ValkeyValue value) => AddCmd(SetRemoveAsync(key, value));

    /// <inheritdoc cref="IBatchSetCommands.SetRemove(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T SetRemove(ValkeyKey key, IEnumerable<ValkeyValue> values) => AddCmd(SetRemoveAsync(key, [.. values]));

    /// <inheritdoc cref="IBatchSetCommands.SetMembers(ValkeyKey)" />
    public T SetMembers(ValkeyKey key) => AddCmd(SetMembersAsync(key));

    /// <inheritdoc cref="IBatchSetCommands.SetCard(ValkeyKey)" />
    public T SetCard(ValkeyKey key) => AddCmd(SetLengthAsync(key));

    /// <inheritdoc cref="IBatchSetCommands.SetInterCard(IEnumerable{ValkeyKey}, long)" />
    public T SetInterCard(IEnumerable<ValkeyKey> keys, long limit = 0) => AddCmd(SetIntersectionLengthAsync([.. keys], limit));

    /// <inheritdoc cref="IBatchSetCommands.SetPop(ValkeyKey)" />
    public T SetPop(ValkeyKey key) => AddCmd(SetPopAsync(key));

    /// <inheritdoc cref="IBatchSetCommands.SetPop(ValkeyKey, long)" />
    public T SetPop(ValkeyKey key, long count) => AddCmd(SetPopAsync(key, count));

    /// <inheritdoc cref="IBatchSetCommands.SetUnion(ValkeyKey, ValkeyKey)" />
    public T SetUnion(ValkeyKey first, ValkeyKey second) => AddCmd(SetUnionAsync([first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetUnion(IEnumerable{ValkeyKey})" />
    public T SetUnion(IEnumerable<ValkeyKey> keys) => AddCmd(SetUnionAsync([.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetInter(ValkeyKey, ValkeyKey)" />
    public T SetInter(ValkeyKey first, ValkeyKey second) => AddCmd(SetIntersectAsync([first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetInter(IEnumerable{ValkeyKey})" />
    public T SetInter(IEnumerable<ValkeyKey> keys) => AddCmd(SetIntersectAsync([.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetDiff(ValkeyKey, ValkeyKey)" />
    public T SetDiff(ValkeyKey first, ValkeyKey second) => AddCmd(SetDifferenceAsync([first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetDiff(IEnumerable{ValkeyKey})" />
    public T SetDiff(IEnumerable<ValkeyKey> keys) => AddCmd(SetDifferenceAsync([.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetUnionStore(ValkeyKey, ValkeyKey, ValkeyKey)" />
    public T SetUnionStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => AddCmd(SetUnionStoreAsync(destination, [first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetUnionStore(ValkeyKey, IEnumerable{ValkeyKey})" />
    public T SetUnionStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(SetUnionStoreAsync(destination, [.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetInterStore(ValkeyKey, ValkeyKey, ValkeyKey)" />
    public T SetInterStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => AddCmd(SetIntersectStoreAsync(destination, [first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetInterStore(ValkeyKey, IEnumerable{ValkeyKey})" />
    public T SetInterStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(SetIntersectStoreAsync(destination, [.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetDiffStore(ValkeyKey, ValkeyKey, ValkeyKey)" />
    public T SetDiffStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => AddCmd(SetDifferenceStoreAsync(destination, [first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetDiffStore(ValkeyKey, IEnumerable{ValkeyKey})" />
    public T SetDiffStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(SetDifferenceStoreAsync(destination, [.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetIsMember(ValkeyKey, ValkeyValue)" />
    public T SetIsMember(ValkeyKey key, ValkeyValue value) => AddCmd(SetContainsAsync(key, value));

    /// <inheritdoc cref="IBatchSetCommands.SetIsMember(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T SetIsMember(ValkeyKey key, IEnumerable<ValkeyValue> values) => AddCmd(SetContainsAsync(key, [.. values]));

    /// <inheritdoc cref="IBatchSetCommands.SetRandomMember(ValkeyKey)" />
    public T SetRandomMember(ValkeyKey key) => AddCmd(SetRandomMemberAsync(key));

    /// <inheritdoc cref="IBatchSetCommands.SetRandomMembers(ValkeyKey, long)" />
    public T SetRandomMembers(ValkeyKey key, long count) => AddCmd(SetRandomMembersAsync(key, count));

    /// <inheritdoc cref="IBatchSetCommands.SetMove(ValkeyKey, ValkeyKey, ValkeyValue)" />
    public T SetMove(ValkeyKey source, ValkeyKey destination, ValkeyValue value) => AddCmd(SetMoveAsync(source, destination, value));

    /// <inheritdoc cref="IBatchSetCommands.SetScan(ValkeyKey, long, ValkeyValue, long)" />
    public T SetScan(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0) => AddCmd(SetScanAsync(key, cursor, pattern, count));

    // Explicit interface implementations for IBatchSetCommands
    IBatch IBatchSetCommands.SetAdd(ValkeyKey key, ValkeyValue value) => SetAdd(key, value);
    IBatch IBatchSetCommands.SetAdd(ValkeyKey key, IEnumerable<ValkeyValue> values) => SetAdd(key, values);
    IBatch IBatchSetCommands.SetRemove(ValkeyKey key, ValkeyValue value) => SetRemove(key, value);
    IBatch IBatchSetCommands.SetRemove(ValkeyKey key, IEnumerable<ValkeyValue> values) => SetRemove(key, values);
    IBatch IBatchSetCommands.SetMembers(ValkeyKey key) => SetMembers(key);
    IBatch IBatchSetCommands.SetCard(ValkeyKey key) => SetCard(key);
    IBatch IBatchSetCommands.SetInterCard(IEnumerable<ValkeyKey> keys, long limit) => SetInterCard(keys, limit);
    IBatch IBatchSetCommands.SetPop(ValkeyKey key) => SetPop(key);
    IBatch IBatchSetCommands.SetPop(ValkeyKey key, long count) => SetPop(key, count);
    IBatch IBatchSetCommands.SetUnion(ValkeyKey first, ValkeyKey second) => SetUnion(first, second);
    IBatch IBatchSetCommands.SetUnion(IEnumerable<ValkeyKey> keys) => SetUnion(keys);
    IBatch IBatchSetCommands.SetInter(ValkeyKey first, ValkeyKey second) => SetInter(first, second);
    IBatch IBatchSetCommands.SetInter(IEnumerable<ValkeyKey> keys) => SetInter(keys);
    IBatch IBatchSetCommands.SetDiff(ValkeyKey first, ValkeyKey second) => SetDiff(first, second);
    IBatch IBatchSetCommands.SetDiff(IEnumerable<ValkeyKey> keys) => SetDiff(keys);
    IBatch IBatchSetCommands.SetUnionStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => SetUnionStore(destination, first, second);
    IBatch IBatchSetCommands.SetUnionStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => SetUnionStore(destination, keys);
    IBatch IBatchSetCommands.SetInterStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => SetInterStore(destination, first, second);
    IBatch IBatchSetCommands.SetInterStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => SetInterStore(destination, keys);
    IBatch IBatchSetCommands.SetDiffStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => SetDiffStore(destination, first, second);
    IBatch IBatchSetCommands.SetDiffStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => SetDiffStore(destination, keys);
    IBatch IBatchSetCommands.SetIsMember(ValkeyKey key, ValkeyValue value) => SetIsMember(key, value);
    IBatch IBatchSetCommands.SetIsMember(ValkeyKey key, IEnumerable<ValkeyValue> values) => SetIsMember(key, values);
    IBatch IBatchSetCommands.SetRandomMember(ValkeyKey key) => SetRandomMember(key);
    IBatch IBatchSetCommands.SetRandomMembers(ValkeyKey key, long count) => SetRandomMembers(key, count);
    IBatch IBatchSetCommands.SetMove(ValkeyKey source, ValkeyKey destination, ValkeyValue value) => SetMove(source, destination, value);
    IBatch IBatchSetCommands.SetScan(ValkeyKey key, long cursor, ValkeyValue pattern, long count) => SetScan(key, cursor, pattern, count);
}
