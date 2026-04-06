// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashGetAsync(key, hashField));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashGetAsync(key, [.. hashFields]));

    /// <inheritdoc/>
    public async Task<HashEntry[]> HashGetAllAsync(ValkeyKey key)
        => await Command(Request.HashGetAllAsync(key));

    /// <inheritdoc/>
    public async Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields)
        => _ = await Command(Request.HashSetAsync(key, [.. hashFields]));

    /// <inheritdoc/>
    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always)
        => await Command(Request.HashSetAsync(key, hashField, value, when));

    /// <inheritdoc/>
    public async Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashDeleteAsync(key, hashField));

    /// <inheritdoc/>
    public async Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashDeleteAsync(key, [.. hashFields]));

    /// <inheritdoc/>
    public async Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashExistsAsync(key, hashField));

    /// <inheritdoc/>
    public async Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1)
        => await Command(Request.HashIncrementAsync(key, hashField, value));

    /// <inheritdoc/>
    public async Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value)
        => await Command(Request.HashIncrementAsync(key, hashField, value));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key)
        => await Command(Request.HashKeysAsync(key));

    /// <inheritdoc/>
    public async Task<long> HashLengthAsync(ValkeyKey key)
        => await Command(Request.HashLengthAsync(key));

    /// <inheritdoc/>
    public async IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0)
    {
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

    /// <inheritdoc/>
    public async IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0)
    {
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

    /// <inheritdoc/>
    public async Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashStringLengthAsync(key, hashField));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key)
        => await Command(Request.HashValuesAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key)
        => await Command(Request.HashRandomFieldAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count)
        => await Command(Request.HashRandomFieldsAsync(key, count));

    /// <inheritdoc/>
    public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count)
        => await Command(Request.HashRandomFieldsWithValuesAsync(key, count));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options)
        => await Command(Request.HashGetExAsync(key, [.. fields], options));

    /// <inheritdoc/>
    public async Task<long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options)
        => await Command(Request.HashSetExAsync(key, fieldValueMap, options));

    /// <inheritdoc/>
    public async Task<long[]> HashPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields)
        => await Command(Request.HashPersistAsync(key, [.. fields]));

    /// <inheritdoc/>
    public async Task<long[]> HashExpireAsync(ValkeyKey key, long seconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options)
        => await Command(Request.HashExpireAsync(key, seconds, [.. fields], options));

    /// <inheritdoc/>
    public async Task<long[]> HashPExpireAsync(ValkeyKey key, long milliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options)
        => await Command(Request.HashPExpireAsync(key, milliseconds, [.. fields], options));

    /// <inheritdoc/>
    public async Task<long[]> HashExpireAtAsync(ValkeyKey key, long unixSeconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options)
        => await Command(Request.HashExpireAtAsync(key, unixSeconds, [.. fields], options));

    /// <inheritdoc/>
    public async Task<long[]> HashPExpireAtAsync(ValkeyKey key, long unixMilliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options)
        => await Command(Request.HashPExpireAtAsync(key, unixMilliseconds, [.. fields], options));

    /// <inheritdoc/>
    public async Task<long[]> HashExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields)
        => await Command(Request.HashExpireTimeAsync(key, [.. fields]));

    /// <inheritdoc/>
    public async Task<long[]> HashPExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields)
        => await Command(Request.HashPExpireTimeAsync(key, [.. fields]));

    /// <inheritdoc/>
    public async Task<long[]> HashTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields)
        => await Command(Request.HashTtlAsync(key, [.. fields]));

    /// <inheritdoc/>
    public async Task<long[]> HashPTtlAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields)
        => await Command(Request.HashPTtlAsync(key, [.. fields]));
}
