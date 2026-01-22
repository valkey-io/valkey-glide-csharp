// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IHashCommands
{
    public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashGetAsync(key, hashField));
    }

    public async Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, ValkeyValue[] hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashGetAsync(key, hashFields));
    }

    public async Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashGetAllAsync(key));
    }

    public async Task HashSetAsync(ValkeyKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await Command(Request.HashSetAsync(key, hashFields));
    }

    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashSetAsync(key, hashField, value, when));
    }

    public async Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashDeleteAsync(key, hashField));
    }

    public async Task<long> HashDeleteAsync(ValkeyKey key, ValkeyValue[] hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashDeleteAsync(key, hashFields));
    }

    public async Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashExistsAsync(key, hashField));
    }

    public async Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashIncrementAsync(key, hashField, value));
    }

    public async Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashIncrementAsync(key, hashField, value));
    }

    public async Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashKeysAsync(key));
    }

    public async Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashLengthAsync(key));
    }

    public async IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        long currentCursor = cursor;

        do
        {
            (long nextCursor, HashEntry[] entries) = await Command(Request.HashScanAsync<HashEntry[]>(key, currentCursor, pattern, pageSize, true));

            IEnumerable<HashEntry> entriesToYield = pageOffset > 0 ? entries.Skip(pageOffset) : entries;

            foreach (HashEntry entry in entriesToYield)
            {
                yield return entry;
            }

            currentCursor = nextCursor;
        } while (currentCursor != 0);
    }

    public async IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        long currentCursor = cursor;

        do
        {
            (long nextCursor, ValkeyValue[] fields) = await Command(Request.HashScanAsync<ValkeyValue[]>(key, currentCursor, pattern, pageSize, false));

            IEnumerable<ValkeyValue> fieldsToYield = pageOffset > 0 ? fields.Skip(pageOffset) : fields;

            foreach (ValkeyValue field in fieldsToYield)
            {
                yield return field;
            }

            currentCursor = nextCursor;
        } while (currentCursor != 0);
    }

    public async Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashStringLengthAsync(key, hashField));
    }

    public async Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashValuesAsync(key));
    }

    public async Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashRandomFieldAsync(key));
    }

    public async Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashRandomFieldsAsync(key, count));
    }

    public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashRandomFieldsWithValuesAsync(key, count));
    }

    public async Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, ValkeyValue[] fields, HashGetExOptions options, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashGetExAsync(key, fields, options));
    }

    public async Task<long> HashSetExAsync(ValkeyKey key, Dictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashSetExAsync(key, fieldValueMap, options));
    }

    public async Task<long[]> HashPersistAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashPersistAsync(key, fields));
    }

    public async Task<long[]> HashExpireAsync(ValkeyKey key, long seconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashExpireAsync(key, seconds, fields, options));
    }

    public async Task<long[]> HashPExpireAsync(ValkeyKey key, long milliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashPExpireAsync(key, milliseconds, fields, options));
    }

    public async Task<long[]> HashExpireAtAsync(ValkeyKey key, long unixSeconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashExpireAtAsync(key, unixSeconds, fields, options));
    }

    public async Task<long[]> HashPExpireAtAsync(ValkeyKey key, long unixMilliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashPExpireAtAsync(key, unixMilliseconds, fields, options));
    }

    public async Task<long[]> HashExpireTimeAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashExpireTimeAsync(key, fields));
    }

    public async Task<long[]> HashPExpireTimeAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashPExpireTimeAsync(key, fields));
    }

    public async Task<long[]> HashTtlAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashTtlAsync(key, fields));
    }

    public async Task<long[]> HashPTtlAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.HashPTtlAsync(key, fields));
    }
}
