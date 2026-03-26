// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Set commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="ISetCommands" />
internal partial class Database
{
    /// <inheritdoc cref="ISetCommands.SetAddAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> SetAddAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetAddAsync(key, value);
    }

    /// <inheritdoc cref="ISetCommands.SetAddAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetAddAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetAddAsync(key, values);
    }

    /// <inheritdoc cref="ISetCommands.SetRemoveAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> SetRemoveAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRemoveAsync(key, value);
    }

    /// <inheritdoc cref="ISetCommands.SetRemoveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetRemoveAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRemoveAsync(key, values);
    }

    /// <inheritdoc cref="ISetCommands.SetMembersAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetMembersAsync(key);
    }

    /// <inheritdoc cref="ISetCommands.SetLengthAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetLengthAsync(key);
    }

    /// <inheritdoc cref="ISetCommands.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectionLengthAsync(keys, limit);
    }

    /// <inheritdoc cref="ISetCommands.SetPopAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> SetPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetPopAsync(key);
    }

    /// <inheritdoc cref="ISetCommands.SetPopAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetPopAsync(key, count);
    }

    /// <inheritdoc cref="ISetCommands.SetUnionAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetUnionAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetUnionAsync(first, second);
    }

    /// <inheritdoc cref="ISetCommands.SetUnionAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetUnionAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetUnionAsync(keys);
    }

    /// <inheritdoc cref="ISetCommands.SetIntersectAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetIntersectAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectAsync(first, second);
    }

    /// <inheritdoc cref="ISetCommands.SetIntersectAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetIntersectAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectAsync(keys);
    }

    /// <inheritdoc cref="ISetCommands.SetDifferenceAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetDifferenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetDifferenceAsync(first, second);
    }

    /// <inheritdoc cref="ISetCommands.SetDifferenceAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetDifferenceAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetDifferenceAsync(keys);
    }

    /// <inheritdoc cref="ISetCommands.SetUnionStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetUnionStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="ISetCommands.SetUnionStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetUnionStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetUnionStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="ISetCommands.SetIntersectStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetIntersectStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="ISetCommands.SetIntersectStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetIntersectStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetIntersectStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="ISetCommands.SetDifferenceStoreAsync(ValkeyKey, ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetDifferenceStoreAsync(ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetDifferenceStoreAsync(destination, first, second);
    }

    /// <inheritdoc cref="ISetCommands.SetDifferenceStoreAsync(ValkeyKey, IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> SetDifferenceStoreAsync(ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetDifferenceStoreAsync(destination, keys);
    }

    /// <inheritdoc cref="ISetCommands.SetContainsAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetContainsAsync(key, value);
    }

    /// <inheritdoc cref="ISetCommands.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetContainsAsync(key, values);
    }

    /// <inheritdoc cref="ISetCommands.SetRandomMemberAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> SetRandomMemberAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRandomMemberAsync(key);
    }

    /// <inheritdoc cref="ISetCommands.SetRandomMembersAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> SetRandomMembersAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRandomMembersAsync(key, count);
    }

    /// <inheritdoc cref="ISetCommands.SetMoveAsync(ValkeyKey, ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> SetMoveAsync(ValkeyKey source, ValkeyKey destination, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetMoveAsync(source, destination, value);
    }

    /// <inheritdoc cref="ISetCommands.SetScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async IAsyncEnumerable<ValkeyValue> SetScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await foreach (ValkeyValue value in SetScanAsync(key, pattern, pageSize, cursor, pageOffset))
        {
            yield return value;
        }
    }
}
