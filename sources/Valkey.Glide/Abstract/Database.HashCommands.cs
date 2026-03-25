// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Hash Commands with CommandFlags (SER Compatibility)

    public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashField);
    }

    public async Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashFields);
    }

    public async Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAllAsync(key);
    }

    public async Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await HashSetAsync(key, hashFields);
    }

    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashSetAsync(key, hashField, value, when);
    }

    public async Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashDeleteAsync(key, hashField);
    }

    public async Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashDeleteAsync(key, hashFields);
    }

    public async Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExistsAsync(key, hashField);
    }

    public async Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashIncrementAsync(key, hashField, value);
    }

    public async Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashIncrementAsync(key, hashField, value);
    }

    public async Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashKeysAsync(key);
    }

    public async Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashLengthAsync(key);
    }

    public async IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await foreach (HashEntry entry in HashScanAsync(key, pattern, pageSize, cursor, pageOffset))
        {
            yield return entry;
        }
    }

    public async IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern, int pageSize, long cursor, int pageOffset, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await foreach (ValkeyValue field in HashScanNoValuesAsync(key, pattern, pageSize, cursor, pageOffset))
        {
            yield return field;
        }
    }

    public async Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashStringLengthAsync(key, hashField);
    }

    public async Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashValuesAsync(key);
    }

    public async Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldAsync(key);
    }

    public async Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsAsync(key, count);
    }

    public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsWithValuesAsync(key, count);
    }

    public async Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetExAsync(key, fields, options);
    }

    public async Task<long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashSetExAsync(key, fieldValueMap, options);
    }

    public async Task<long[]> HashPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPersistAsync(key, fields);
    }

    public async Task<long[]> HashExpireAsync(ValkeyKey key, long seconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireAsync(key, seconds, fields, options);
    }

    public async Task<long[]> HashPExpireAsync(ValkeyKey key, long milliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPExpireAsync(key, milliseconds, fields, options);
    }

    public async Task<long[]> HashExpireAtAsync(ValkeyKey key, long unixSeconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireAtAsync(key, unixSeconds, fields, options);
    }

    public async Task<long[]> HashPExpireAtAsync(ValkeyKey key, long unixMilliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPExpireAtAsync(key, unixMilliseconds, fields, options);
    }

    public async Task<long[]> HashExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireTimeAsync(key, fields);
    }

    public async Task<long[]> HashPExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPExpireTimeAsync(key, fields);
    }

    public async Task<long[]> HashTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashTtlAsync(key, fields);
    }

    public async Task<long[]> HashPTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashPTtlAsync(key, fields);
    }

    #endregion
}
