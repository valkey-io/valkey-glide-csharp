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

    /// <inheritdoc cref="IBatchSetCommands.SetLength(ValkeyKey)" />
    public T SetLength(ValkeyKey key) => AddCmd(SetLengthAsync(key));

    /// <inheritdoc cref="IBatchSetCommands.SetIntersectionLength(IEnumerable{ValkeyKey}, long)" />
    public T SetIntersectionLength(IEnumerable<ValkeyKey> keys, long limit = 0) => AddCmd(SetIntersectionLengthAsync([.. keys], limit));

    /// <inheritdoc cref="IBatchSetCommands.SetPop(ValkeyKey)" />
    public T SetPop(ValkeyKey key) => AddCmd(SetPopAsync(key));

    /// <inheritdoc cref="IBatchSetCommands.SetPop(ValkeyKey, long)" />
    public T SetPop(ValkeyKey key, long count) => AddCmd(SetPopAsync(key, count));

    /// <inheritdoc cref="IBatchSetCommands.SetUnion(ValkeyKey, ValkeyKey)" />
    public T SetUnion(ValkeyKey first, ValkeyKey second) => AddCmd(SetUnionAsync([first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetUnion(IEnumerable{ValkeyKey})" />
    public T SetUnion(IEnumerable<ValkeyKey> keys) => AddCmd(SetUnionAsync([.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetIntersect(ValkeyKey, ValkeyKey)" />
    public T SetIntersect(ValkeyKey first, ValkeyKey second) => AddCmd(SetIntersectAsync([first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetIntersect(IEnumerable{ValkeyKey})" />
    public T SetIntersect(IEnumerable<ValkeyKey> keys) => AddCmd(SetIntersectAsync([.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetDifference(ValkeyKey, ValkeyKey)" />
    public T SetDifference(ValkeyKey first, ValkeyKey second) => AddCmd(SetDifferenceAsync([first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetDifference(IEnumerable{ValkeyKey})" />
    public T SetDifference(IEnumerable<ValkeyKey> keys) => AddCmd(SetDifferenceAsync([.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetUnionStore(ValkeyKey, ValkeyKey, ValkeyKey)" />
    public T SetUnionStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => AddCmd(SetUnionStoreAsync(destination, [first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetUnionStore(ValkeyKey, IEnumerable{ValkeyKey})" />
    public T SetUnionStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(SetUnionStoreAsync(destination, [.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetIntersectStore(ValkeyKey, ValkeyKey, ValkeyKey)" />
    public T SetIntersectStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => AddCmd(SetIntersectStoreAsync(destination, [first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetIntersectStore(ValkeyKey, IEnumerable{ValkeyKey})" />
    public T SetIntersectStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(SetIntersectStoreAsync(destination, [.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetDifferenceStore(ValkeyKey, ValkeyKey, ValkeyKey)" />
    public T SetDifferenceStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => AddCmd(SetDifferenceStoreAsync(destination, [first, second]));

    /// <inheritdoc cref="IBatchSetCommands.SetDifferenceStore(ValkeyKey, IEnumerable{ValkeyKey})" />
    public T SetDifferenceStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => AddCmd(SetDifferenceStoreAsync(destination, [.. keys]));

    /// <inheritdoc cref="IBatchSetCommands.SetContains(ValkeyKey, ValkeyValue)" />
    public T SetContains(ValkeyKey key, ValkeyValue value) => AddCmd(SetContainsAsync(key, value));

    /// <inheritdoc cref="IBatchSetCommands.SetContains(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T SetContains(ValkeyKey key, IEnumerable<ValkeyValue> values) => AddCmd(SetContainsAsync(key, [.. values]));

    /// <inheritdoc cref="IBatchSetCommands.SetRandomMember(ValkeyKey)" />
    public T SetRandomMember(ValkeyKey key) => AddCmd(SetRandomMemberAsync(key));

    /// <inheritdoc cref="IBatchSetCommands.SetRandomMembers(ValkeyKey, long)" />
    public T SetRandomMembers(ValkeyKey key, long count) => AddCmd(SetRandomMembersAsync(key, count));

    /// <inheritdoc cref="IBatchSetCommands.SetMove(ValkeyKey, ValkeyKey, ValkeyValue)" />
    public T SetMove(ValkeyKey source, ValkeyKey destination, ValkeyValue value) => AddCmd(SetMoveAsync(source, destination, value));

    /// <inheritdoc cref="IBatchSetCommands.SetScan(ValkeyKey, ValkeyValue, int, long)" />
    public T SetScan(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0) => AddCmd(SetScanAsync(key, cursor, pattern, pageSize));

    // Explicit interface implementations for IBatchSetCommands
    IBatch IBatchSetCommands.SetAdd(ValkeyKey key, ValkeyValue value) => SetAdd(key, value);
    IBatch IBatchSetCommands.SetAdd(ValkeyKey key, IEnumerable<ValkeyValue> values) => SetAdd(key, values);
    IBatch IBatchSetCommands.SetRemove(ValkeyKey key, ValkeyValue value) => SetRemove(key, value);
    IBatch IBatchSetCommands.SetRemove(ValkeyKey key, IEnumerable<ValkeyValue> values) => SetRemove(key, values);
    IBatch IBatchSetCommands.SetMembers(ValkeyKey key) => SetMembers(key);
    IBatch IBatchSetCommands.SetLength(ValkeyKey key) => SetLength(key);
    IBatch IBatchSetCommands.SetIntersectionLength(IEnumerable<ValkeyKey> keys, long limit) => SetIntersectionLength(keys, limit);
    IBatch IBatchSetCommands.SetPop(ValkeyKey key) => SetPop(key);
    IBatch IBatchSetCommands.SetPop(ValkeyKey key, long count) => SetPop(key, count);
    IBatch IBatchSetCommands.SetUnion(ValkeyKey first, ValkeyKey second) => SetUnion(first, second);
    IBatch IBatchSetCommands.SetUnion(IEnumerable<ValkeyKey> keys) => SetUnion(keys);
    IBatch IBatchSetCommands.SetIntersect(ValkeyKey first, ValkeyKey second) => SetIntersect(first, second);
    IBatch IBatchSetCommands.SetIntersect(IEnumerable<ValkeyKey> keys) => SetIntersect(keys);
    IBatch IBatchSetCommands.SetDifference(ValkeyKey first, ValkeyKey second) => SetDifference(first, second);
    IBatch IBatchSetCommands.SetDifference(IEnumerable<ValkeyKey> keys) => SetDifference(keys);
    IBatch IBatchSetCommands.SetUnionStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => SetUnionStore(destination, first, second);
    IBatch IBatchSetCommands.SetUnionStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => SetUnionStore(destination, keys);
    IBatch IBatchSetCommands.SetIntersectStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => SetIntersectStore(destination, first, second);
    IBatch IBatchSetCommands.SetIntersectStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => SetIntersectStore(destination, keys);
    IBatch IBatchSetCommands.SetDifferenceStore(ValkeyKey destination, ValkeyKey first, ValkeyKey second) => SetDifferenceStore(destination, first, second);
    IBatch IBatchSetCommands.SetDifferenceStore(ValkeyKey destination, IEnumerable<ValkeyKey> keys) => SetDifferenceStore(destination, keys);
    IBatch IBatchSetCommands.SetContains(ValkeyKey key, ValkeyValue value) => SetContains(key, value);
    IBatch IBatchSetCommands.SetContains(ValkeyKey key, IEnumerable<ValkeyValue> values) => SetContains(key, values);
    IBatch IBatchSetCommands.SetRandomMember(ValkeyKey key) => SetRandomMember(key);
    IBatch IBatchSetCommands.SetRandomMembers(ValkeyKey key, long count) => SetRandomMembers(key, count);
    IBatch IBatchSetCommands.SetMove(ValkeyKey source, ValkeyKey destination, ValkeyValue value) => SetMove(source, destination, value);
    IBatch IBatchSetCommands.SetScan(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor) => SetScan(key, pattern, pageSize, cursor);
}
