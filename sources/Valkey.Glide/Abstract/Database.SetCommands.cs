// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <inheritdoc cref="IDatabaseAsync" path="//*[not(self::seealso)]"/>
/// <seealso cref="ISetCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.SetAddAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetAddAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetAddAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetRemoveAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRemoveAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRemoveAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetMembersAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetMembersAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetLengthAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> SetLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public async Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectionLengthAsync(keys, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetPopAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> SetPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetPopAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetPopAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetPopAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetUnionAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetUnionAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetUnionAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetUnionAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetUnionAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetUnionAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetIntersectAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetIntersectAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetDifferenceAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetDifferenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetDifferenceAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetDifferenceAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetDifferenceAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetDifferenceAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetUnionStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetUnionStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<long> SetIntersectStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> SetIntersectStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<long> SetDifferenceStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetDifferenceStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> SetDifferenceStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetDifferenceStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetContainsAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetContainsAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetContainsAsync(key, values);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetRandomMemberAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRandomMemberAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetRandomMembersAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRandomMembersAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetMoveAsync(source, destination, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)"/>
    public async IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await foreach (ValkeyValue value in SetScanAsync(key, pattern, pageSize, cursor, pageOffset))
        {
            yield return value;
        }
    }
}
