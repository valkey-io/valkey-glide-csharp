// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashGetAsync(key, hashField));

    /// <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashGetAsync(key, [.. hashFields]));

    /// <inheritdoc cref="IBaseClient.HashGetAsync(ValkeyKey)"/>
    public async Task<IDictionary<ValkeyValue, ValkeyValue>> HashGetAsync(ValkeyKey key)
        => await Command(Request.HashGetAsync(key));

    /// <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}})"/>
    public async Task<long> HashSetAsync(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues)
        => await Command(Request.HashSetAsync(key, [.. hashFieldsAndValues]));

    /// <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => await Command(Request.HashSetAsync(key, hashField, value));

    /// <inheritdoc cref="IHashBaseCommands.HashSetIfNotExistsAsync(ValkeyKey, ValkeyValue, ValkeyValue)"/>
    public async Task<bool> HashSetIfNotExistsAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => await Command(Request.HashSetNotExistsAsync(key, hashField, value));

    /// <inheritdoc cref="IHashBaseCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashDeleteAsync(key, hashField));

    /// <inheritdoc cref="IHashBaseCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashDeleteAsync(key, [.. hashFields]));

    /// <inheritdoc cref="IHashBaseCommands.HashExistsAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashExistsAsync(key, hashField));

    /// <inheritdoc cref="IHashBaseCommands.HashIncrementByAsync(ValkeyKey, ValkeyValue, long)"/>
    public async Task<long> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, long value = 1)
        => await Command(Request.HashIncrementByAsync(key, hashField, value));

    /// <inheritdoc cref="IHashBaseCommands.HashIncrementByAsync(ValkeyKey, ValkeyValue, double)"/>
    public async Task<double> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, double value)
        => await Command(Request.HashIncrementByAsync(key, hashField, value));

    /// <inheritdoc cref="IBaseClient.HashKeysAsync(ValkeyKey)"/>
    public async Task<ISet<ValkeyValue>> HashKeysAsync(ValkeyKey key)
        => await Command(Request.HashKeysAsync(key));

    /// <inheritdoc cref="IHashBaseCommands.HashLengthAsync(ValkeyKey)"/>
    public async Task<long> HashLengthAsync(ValkeyKey key)
        => await Command(Request.HashLengthAsync(key));

    /// <inheritdoc cref="IHashBaseCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashStringLengthAsync(key, hashField));

    /// <inheritdoc cref="IBaseClient.HashValuesAsync(ValkeyKey)"/>
    public async Task<ICollection<ValkeyValue>> HashValuesAsync(ValkeyKey key)
        => await Command(Request.HashValuesAsync(key));

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldAsync(ValkeyKey)"/>
    public async Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key)
        => await Command(Request.HashRandomFieldAsync(key));

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldsAsync(ValkeyKey, long)"/>
    public async Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count)
        => await Command(Request.HashRandomFieldsAsync(key, count));

    /// <inheritdoc cref="IBaseClient.HashRandomFieldWithValueAsync(ValkeyKey)"/>
    public async Task<HashEntry?> HashRandomFieldWithValueAsync(ValkeyKey key)
    {
        var result = await Command(Request.HashRandomFieldWithValueAsync(key));
        return result.HasValue ? new HashEntry(result.Value.Key, result.Value.Value) : null;
    }

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)"/>
    public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count)
        => await Command(Request.HashRandomFieldsWithValuesAsync(key, count));

    /// <inheritdoc cref="IBaseClient.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue}, GetExpiryOptions)"/>
    public async Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, GetExpiryOptions options)
        => await Command(Request.HashGetAsync(key, hashFields, options));

    /// <inheritdoc cref="IBaseClient.HashGetAsync(ValkeyKey, ValkeyValue, GetExpiryOptions)"/>
    public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, GetExpiryOptions options)
        => (await Command(Request.HashGetAsync(key, [hashField], options))).First();

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, HashSetCondition)"/>
    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, HashSetCondition condition)
        => await Command(Request.HashSetAsync(key, hashField, value, condition));

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, HashSetCondition)"/>
    public async Task<bool> HashSetAsync(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, HashSetCondition condition)
        => await Command(Request.HashSetAsync(key, hashFieldsAndValues, condition));

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, HashSetOptions)"/>
    public async Task<bool> HashSetAsync(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetOptions options)
        => await Command(Request.HashSetAsync(key, hashFieldsAndValues, options));

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, HashSetOptions)"/>
    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value,
        HashSetOptions options)
        => await HashSetAsync(key, [new KeyValuePair<ValkeyValue, ValkeyValue>(hashField, value)], options);

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, SetExpiryOptions)"/>
    public Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, SetExpiryOptions expiry)
        => HashSetAsync(key, hashField, value, new HashSetOptions { Expiry = expiry });

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, SetExpiryOptions)"/>
    public Task<bool> HashSetAsync(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, SetExpiryOptions expiry)
        => HashSetAsync(key, hashFieldsAndValues, new HashSetOptions { Expiry = expiry });

    /// <inheritdoc cref="IBaseClient.HashPersistAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<HashPersistResult> HashPersistAsync(ValkeyKey key, ValkeyValue hashField)
        => (await Command(Request.HashPersistAsync(key, [hashField]))).First();

    /// <inheritdoc cref="IBaseClient.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<HashPersistResult[]> HashPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashPersistAsync(key, [.. hashFields]));

    /// <inheritdoc cref="IBaseClient.HashExpireAsync(ValkeyKey, ValkeyValue, TimeSpan, ExpireCondition)"/>
    public async Task<HashExpireResult> HashExpireAsync(ValkeyKey key, ValkeyValue hashField, TimeSpan expiry, ExpireCondition condition = ExpireCondition.Always)
        => (await Command(Request.HashExpireAsync(key, expiry, [hashField], condition))).First();

    /// <inheritdoc cref="IBaseClient.HashExpireAsync(ValkeyKey, IEnumerable{ValkeyValue}, TimeSpan, ExpireCondition)"/>
    public async Task<HashExpireResult[]> HashExpireAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan expiry, ExpireCondition condition = ExpireCondition.Always)
        => await Command(Request.HashExpireAsync(key, expiry, [.. hashFields], condition));

    /// <inheritdoc cref="IBaseClient.HashExpireAtAsync(ValkeyKey, ValkeyValue, DateTimeOffset, ExpireCondition)"/>
    public async Task<HashExpireResult> HashExpireAtAsync(ValkeyKey key, ValkeyValue hashField, DateTimeOffset expiry, ExpireCondition condition = ExpireCondition.Always)
        => (await Command(Request.HashExpireAtAsync(key, expiry, [hashField], condition))).First();

    /// <inheritdoc cref="IBaseClient.HashExpireAtAsync(ValkeyKey, IEnumerable{ValkeyValue}, DateTimeOffset, ExpireCondition)"/>
    public async Task<HashExpireResult[]> HashExpireAtAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTimeOffset expiry, ExpireCondition condition = ExpireCondition.Always)
        => await Command(Request.HashExpireAtAsync(key, expiry, [.. hashFields], condition));

    /// <inheritdoc cref="IBaseClient.HashExpireTimeAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<ExpireTimeResult> HashExpireTimeAsync(ValkeyKey key, ValkeyValue hashField)
        => (await Command(Request.HashExpireTimeAsync(key, [hashField]))).First();

    /// <inheritdoc cref="IBaseClient.HashExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<ExpireTimeResult[]> HashExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashExpireTimeAsync(key, [.. hashFields]));

    /// <inheritdoc cref="IBaseClient.HashTimeToLiveAsync(ValkeyKey, ValkeyValue)"/>
    public async Task<TimeToLiveResult> HashTimeToLiveAsync(ValkeyKey key, ValkeyValue hashField)
        => (await Command(Request.HashTimeToLiveAsync(key, [hashField]))).First();

    /// <inheritdoc cref="IBaseClient.HashTimeToLiveAsync(ValkeyKey, IEnumerable{ValkeyValue})"/>
    public async Task<TimeToLiveResult[]> HashTimeToLiveAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashTimeToLiveAsync(key, [.. hashFields]));
}
