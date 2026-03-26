// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// Hash commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IHashCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashField);
    }

    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashFields);
    }

    /// <inheritdoc cref="IHashCommands.HashGetAllAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAllAsync(key);
    }

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, IEnumerable{HashEntry})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await HashSetAsync(key, hashFields);
    }

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashSetAsync(key, hashField, value, when);
    }

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashDeleteAsync(key, hashField);
    }

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashDeleteAsync(key, hashFields);
    }

    /// <inheritdoc cref="IHashCommands.HashExistsAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExistsAsync(key, hashField);
    }

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashIncrementAsync(key, hashField, value);
    }

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashIncrementAsync(key, hashField, value);
    }

    /// <inheritdoc cref="IHashCommands.HashKeysAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashKeysAsync(key);
    }

    /// <inheritdoc cref="IHashCommands.HashLengthAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashLengthAsync(key);
    }

    /// <inheritdoc cref="IHashCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await foreach (HashEntry entry in HashScanAsync(key, pattern, pageSize, cursor, pageOffset))
        {
            yield return entry;
        }
    }

    /// <inheritdoc cref="IHashCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await foreach (ValkeyValue field in HashScanNoValuesAsync(key, pattern, pageSize, cursor, pageOffset))
        {
            yield return field;
        }
    }

    /// <inheritdoc cref="IHashCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashStringLengthAsync(key, hashField);
    }

    /// <inheritdoc cref="IHashCommands.HashValuesAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashValuesAsync(key);
    }

    /// <inheritdoc cref="IHashCommands.HashRandomFieldAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldAsync(key);
    }

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsAsync(key, count);
    }

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsWithValuesAsync(key, count);
    }

    /// <inheritdoc cref="IHashCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetExAsync(key, fields, options);
    }

    /// <inheritdoc cref="IHashCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashSetExAsync(key, fieldValueMap, options);
    }

    /// <inheritdoc cref="IHashCommands.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPersistAsync(key, fields);
    }

    /// <inheritdoc cref="IHashCommands.HashExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashExpireAsync(ValkeyKey key, long seconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireAsync(key, seconds, fields, options);
    }

    /// <inheritdoc cref="IHashCommands.HashPExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashPExpireAsync(ValkeyKey key, long milliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPExpireAsync(key, milliseconds, fields, options);
    }

    /// <inheritdoc cref="IHashCommands.HashExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashExpireAtAsync(ValkeyKey key, long unixSeconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireAtAsync(key, unixSeconds, fields, options);
    }

    /// <inheritdoc cref="IHashCommands.HashPExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashPExpireAtAsync(ValkeyKey key, long unixMilliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPExpireAtAsync(key, unixMilliseconds, fields, options);
    }

    /// <inheritdoc cref="IHashCommands.HashExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireTimeAsync(key, fields);
    }

    /// <inheritdoc cref="IHashCommands.HashPExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashPExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPExpireTimeAsync(key, fields);
    }

    /// <inheritdoc cref="IHashCommands.HashTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashTtlAsync(key, fields);
    }

    /// <inheritdoc cref="IHashCommands.HashPTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long[]> HashPTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPTtlAsync(key, fields);
    }
}
