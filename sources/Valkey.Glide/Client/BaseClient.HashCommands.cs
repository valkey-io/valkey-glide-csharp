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
    public async Task<IDictionary<ValkeyValue, ValkeyValue>> HashGetAllAsync(ValkeyKey key)
        => await Command(Request.HashGetAllAsync(key));

    /// <inheritdoc/>
    public async Task<long> HashSetAsync(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues)
        => await Command(Request.HashSetAsync(key, [.. hashFieldsAndValues]));

    /// <inheritdoc/>
    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => await Command(Request.HashSetAsync(key, hashField, value));

    /// <inheritdoc/>
    public async Task<bool> HashSetIfNotExistsAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value)
        => await Command(Request.HashSetNotExistsAsync(key, hashField, value));

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
    public async Task<long> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, long value = 1)
        => await Command(Request.HashIncrementByAsync(key, hashField, value));

    /// <inheritdoc/>
    public async Task<double> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, double value)
        => await Command(Request.HashIncrementByAsync(key, hashField, value));

    /// <inheritdoc/>
    public async Task<ISet<ValkeyValue>> HashKeysAsync(ValkeyKey key)
        => await Command(Request.HashKeysAsync(key));

    /// <inheritdoc/>
    public async Task<long> HashLengthAsync(ValkeyKey key)
        => await Command(Request.HashLengthAsync(key));

    /// <inheritdoc/>
    public async Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField)
        => await Command(Request.HashStringLengthAsync(key, hashField));

    /// <inheritdoc/>
    public async Task<ICollection<ValkeyValue>> HashValuesAsync(ValkeyKey key)
        => await Command(Request.HashValuesAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key)
        => await Command(Request.HashRandomFieldAsync(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count)
        => await Command(Request.HashRandomFieldsAsync(key, count));

    /// <inheritdoc/>
    public async Task<KeyValuePair<ValkeyValue, ValkeyValue>?> HashRandomFieldWithValueAsync(ValkeyKey key)
        => await Command(Request.HashRandomFieldWithValueAsync(key));

    /// <inheritdoc/>
    public async Task<ICollection<KeyValuePair<ValkeyValue, ValkeyValue>>> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count)
        => await Command(Request.HashRandomFieldsWithValuesAsync(key, count));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashGetExpiryAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, GetExpiryOptions options)
        => await Command(Request.HashGetExpiryAsync(key, hashFields, options));

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashGetExpiryAsync(ValkeyKey key, ValkeyValue hashField, GetExpiryOptions options)
        => (await Command(Request.HashGetExpiryAsync(key, [hashField], options))).First();

    /// <inheritdoc/>
    public async Task<bool> HashSetExpiryAsync(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        SetExpiryOptions options, HashSetCondition condition = HashSetCondition.Always)
        => await Command(Request.HashSetExpiryAsync(key, hashFieldsAndValues, options, condition));

    /// <inheritdoc/>
    public async Task<bool> HashSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value,
        SetExpiryOptions options, HashSetCondition condition = HashSetCondition.Always)
        => await HashSetExpiryAsync(key, [new KeyValuePair<ValkeyValue, ValkeyValue>(hashField, value)], options, condition);

    /// <inheritdoc/>
    public async Task<HashPersistResult> HashPersistAsync(ValkeyKey key, ValkeyValue hashField)
        => (await Command(Request.HashPersistAsync(key, [hashField]))).First();

    /// <inheritdoc/>
    public async Task<HashPersistResult[]> HashPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashPersistAsync(key, [.. hashFields]));

    /// <inheritdoc/>
    public async Task<HashExpireResult> HashExpireAsync(ValkeyKey key, ValkeyValue hashField, TimeSpan expiry, ExpireCondition condition = ExpireCondition.Always)
        => (await Command(Request.HashExpireAsync(key, expiry, [hashField], condition))).First();

    /// <inheritdoc/>
    public async Task<HashExpireResult[]> HashExpireAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan expiry, ExpireCondition condition = ExpireCondition.Always)
        => await Command(Request.HashExpireAsync(key, expiry, [.. hashFields], condition));

    /// <inheritdoc/>
    public async Task<HashExpireResult> HashExpireAtAsync(ValkeyKey key, ValkeyValue hashField, DateTimeOffset expiry, ExpireCondition condition = ExpireCondition.Always)
        => (await Command(Request.HashExpireAtAsync(key, expiry, [hashField], condition))).First();

    /// <inheritdoc/>
    public async Task<HashExpireResult[]> HashExpireAtAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTimeOffset expiry, ExpireCondition condition = ExpireCondition.Always)
        => await Command(Request.HashExpireAtAsync(key, expiry, [.. hashFields], condition));

    /// <inheritdoc/>
    public async Task<ExpireTimeResult> HashExpireTimeAsync(ValkeyKey key, ValkeyValue hashField)
        => (await Command(Request.HashExpireTimeAsync(key, [hashField]))).First();

    /// <inheritdoc/>
    public async Task<ExpireTimeResult[]> HashExpireTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashExpireTimeAsync(key, [.. hashFields]));

    /// <inheritdoc/>
    public async Task<TimeToLiveResult> HashTimeToLiveAsync(ValkeyKey key, ValkeyValue hashField)
        => (await Command(Request.HashTimeToLiveAsync(key, [hashField]))).First();

    /// <inheritdoc/>
    public async Task<TimeToLiveResult[]> HashTimeToLiveAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields)
        => await Command(Request.HashTimeToLiveAsync(key, [.. hashFields]));
}
