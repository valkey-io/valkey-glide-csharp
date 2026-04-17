// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
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
    public async Task<ValkeyValue[]> SetMembersAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var result = await base.SetMembersAsync(key);
        return [.. result];
    }

    /// <inheritdoc cref="IDatabaseAsync.SetLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> SetLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetCardAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetIntersectionLengthAsync(IEnumerable{ValkeyKey}, long, CommandFlags)"/>
    public Task<long> SetIntersectionLengthAsync(IEnumerable<ValkeyKey> keys, long limit = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetInterCardAsync(keys, limit);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetPopAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> SetPopAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetPopAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetPopAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetPopAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var result = await base.SetPopAsync(key, count);
        return [.. result];
    }

    /// <inheritdoc cref="IDatabaseAsync.SetCombineAsync(SetOperation, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetCombineAsync(SetOperation operation, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var result = await GetCombineResultAsync(operation, [first, second]);
        return [.. result];
    }

    /// <inheritdoc cref="IDatabaseAsync.SetCombineAsync(SetOperation, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<ValkeyValue[]> SetCombineAsync(SetOperation operation, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var result = await GetCombineResultAsync(operation, keys);
        return [.. result];
    }

    /// <inheritdoc cref="IDatabaseAsync.SetCombineAndStoreAsync(SetOperation, ValkeyKey, ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<long> SetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetCombineAndStoreResultAsync(operation, destination, [first, second]);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetCombineAndStoreAsync(SetOperation, ValkeyKey, IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<long> SetCombineAndStoreAsync(SetOperation operation, ValkeyKey destination, IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetCombineAndStoreResultAsync(operation, destination, keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetContainsAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> SetContainsAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetIsMemberAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.SetContainsAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<bool[]> SetContainsAsync(ValkeyKey key, IEnumerable<ValkeyValue> values, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetIsMemberAsync(key, values);
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

        // TODO cleanup?
        // Build ScanOptions from the SER parameters
        ScanOptions? options = null;
        if (!pattern.IsNull || pageSize != 250)
        {
            options = new ScanOptions();
            if (!pattern.IsNull)
            {
                options.MatchPattern = pattern;
            }
            if (pageSize != 250)
            {
                options.Count = pageSize;
            }
        }

        return SetScanWithOffsetAsync(key, options, cursor, pageOffset);
    }

    private async IAsyncEnumerable<ValkeyValue> SetScanWithOffsetAsync(ValkeyKey key, ScanOptions? options, long cursor, int pageOffset)
    {
        long currentCursor = cursor;
        int currentOffset = pageOffset;

        do
        {
            (long nextCursor, ValkeyValue[] elements) = await Command(Request.SetScanAsync(key, currentCursor, options));

            IEnumerable<ValkeyValue> elementsToYield = currentOffset > 0 ? elements.Skip(currentOffset) : elements;

            foreach (ValkeyValue element in elementsToYield)
            {
                yield return element;
            }

            currentCursor = nextCursor;
            currentOffset = 0; // Only skip on first iteration
        } while (currentCursor != 0);
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
