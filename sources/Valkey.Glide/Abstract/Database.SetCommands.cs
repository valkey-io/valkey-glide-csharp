// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

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
    public async Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        ISet<ValkeyValue> result = await base.SetMembersAsync(key);
        return [.. result];
    }

    /// <inheritdoc cref="IDatabaseAsync.SetLengthAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> SetLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetCardAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public async Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetInterCardAsync(keys, limit);
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

    /// <inheritdoc cref="IDatabaseAsync.SetCombineAsync(SetOperation, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetCombineAsync(SetOperation operation, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return [.. await GetCombineResultAsync(operation, [first, second])];
    }

    /// <inheritdoc cref="IDatabaseAsync.SetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return [.. await GetCombineResultAsync(operation, keys)];
    }

    /// <inheritdoc cref="IDatabaseAsync.SetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<long> SetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetCombineAndStoreResultAsync(operation, destination, [first, second]);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<long> SetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetCombineAndStoreResultAsync(operation, destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetContainsAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIsMemberAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIsMemberAsync(key, values);
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

    // TODO #287
    /// <inheritdoc cref="IDatabaseAsync.SetScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)"/>
    public IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetScanAsync(key, pattern, pageSize, cursor, pageOffset);
    }

    private async Task<ISet<ValkeyValue>> GetCombineResultAsync(SetOperation operation, IEnumerable<ValkeyKey> keys) => operation switch
    {
        SetOperation.Union => await SetUnionAsync(keys),
        SetOperation.Intersect => await SetInterAsync(keys),
        SetOperation.Difference => await SetDiffAsync(keys),
        _ => throw new ArgumentOutOfRangeException(nameof(operation)),
    };

    private async Task<long> GetCombineAndStoreResultAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys) => operation switch
    {
        SetOperation.Union => await SetUnionStoreAsync(destination, keys),
        SetOperation.Intersect => await SetInterStoreAsync(destination, keys),
        SetOperation.Difference => await SetDiffStoreAsync(destination, keys),
        _ => throw new ArgumentOutOfRangeException(nameof(operation)),
    };
}
