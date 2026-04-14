// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.SetAddAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetAddAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetAddAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetRemoveAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetRemoveAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetRemoveAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetMembersAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetMembersAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> SetLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetIntersectionLengthAsync(keys, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetPopAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> SetPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetPopAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetPopAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetPopAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetUnionAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue[]> SetUnionAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetUnionAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetUnionAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<ValkeyValue[]> SetUnionAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetUnionAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue[]> SetIntersectAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetIntersectAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<ValkeyValue[]> SetIntersectAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetIntersectAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetDifferenceAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue[]> SetDifferenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetDifferenceAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetDifferenceAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<ValkeyValue[]> SetDifferenceAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetDifferenceAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<long> SetUnionStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetUnionStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetUnionStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<long> SetIntersectStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetIntersectStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> SetIntersectStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetIntersectStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<long> SetDifferenceStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetDifferenceStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> SetDifferenceStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetDifferenceStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetContainsAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetContainsAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetContainsAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetRandomMemberAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetRandomMemberAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetRandomMembersAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetRandomMembersAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetMoveAsync(source, destination, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)"/>
    public IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetScanAsync(key, pattern, pageSize, cursor, pageOffset);
    }
}
