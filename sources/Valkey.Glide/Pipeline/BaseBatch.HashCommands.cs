// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Internals.Request;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Hash commands for BaseBatch.
/// </summary>
public abstract partial class BaseBatch<T>
{
    /// <inheritdoc cref="IBatchHashCommands.HashGet(ValkeyKey, ValkeyValue)" />
    public T HashGet(ValkeyKey key, ValkeyValue hashField) => AddCmd(HashGetAsync(key, hashField));

    /// <inheritdoc cref="IBatchHashCommands.HashGet(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T HashGet(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => AddCmd(HashGetAsync(key, [.. hashFields]));

    /// <inheritdoc cref="IBatchHashCommands.HashGet(ValkeyKey)" />
    public T HashGet(ValkeyKey key) => AddCmd(HashGetAsync(key));

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, IEnumerable{HashEntry})" />
    public T HashSet(ValkeyKey key, IEnumerable<HashEntry> hashFields) => AddCmd(HashSetAsync(key, [.. hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value))]));

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, ValkeyValue, ValkeyValue, When)" />
    public T HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always)
        => when switch
        {
            When.Always => AddCmd(HashSetAsync(key, hashField, value)),
            When.NotExists => AddCmd(HashSetNotExistsAsync(key, hashField, value)),
            When.Exists => throw new ArgumentException(when + " is not valid in this context; the permitted values are: Always, NotExists"),
            _ => throw new NotSupportedException($"When {when} is not supported by Valkey GLIDE"),
        };

    /// <inheritdoc cref="IBatchHashCommands.HashDelete(ValkeyKey, ValkeyValue)" />
    public T HashDelete(ValkeyKey key, ValkeyValue hashField) => AddCmd(HashDeleteAsync(key, hashField));

    /// <inheritdoc cref="IBatchHashCommands.HashDelete(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T HashDelete(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => AddCmd(HashDeleteAsync(key, [.. hashFields]));

    /// <inheritdoc cref="IBatchHashCommands.HashExists(ValkeyKey, ValkeyValue)" />
    public T HashExists(ValkeyKey key, ValkeyValue hashField) => AddCmd(HashExistsAsync(key, hashField));

    /// <inheritdoc cref="IBatchHashCommands.HashIncrement(ValkeyKey, ValkeyValue, long)" />
    public T HashIncrement(ValkeyKey key, ValkeyValue hashField, long value = 1) => AddCmd(HashIncrementByAsync(key, hashField, value));

    /// <inheritdoc cref="IBatchHashCommands.HashIncrement(ValkeyKey, ValkeyValue, double)" />
    public T HashIncrement(ValkeyKey key, ValkeyValue hashField, double value) => AddCmd(HashIncrementByAsync(key, hashField, value));

    /// <inheritdoc cref="IBatchHashCommands.HashKeys(ValkeyKey)" />
    public T HashKeys(ValkeyKey key) => AddCmd(HashKeysAsync(key));

    /// <inheritdoc cref="IBatchHashCommands.HashLength(ValkeyKey)" />
    public T HashLength(ValkeyKey key) => AddCmd(HashLengthAsync(key));

    /// <inheritdoc cref="IBatchHashCommands.HashStringLength(ValkeyKey, ValkeyValue)" />
    public T HashStringLength(ValkeyKey key, ValkeyValue hashField) => AddCmd(HashStringLengthAsync(key, hashField));

    /// <inheritdoc cref="IBatchHashCommands.HashValues(ValkeyKey)" />
    public T HashValues(ValkeyKey key) => AddCmd(HashValuesAsync(key));

    /// <inheritdoc cref="IBatchHashCommands.HashRandomField(ValkeyKey)" />
    public T HashRandomField(ValkeyKey key) => AddCmd(HashRandomFieldAsync(key));

    /// <inheritdoc cref="IBatchHashCommands.HashRandomFields(ValkeyKey, long)" />
    public T HashRandomFields(ValkeyKey key, long count) => AddCmd(HashRandomFieldsAsync(key, count));

    /// <inheritdoc cref="IBatchHashCommands.HashRandomFieldWithValue(ValkeyKey)" />
    public T HashRandomFieldWithValue(ValkeyKey key) => AddCmd(HashRandomFieldWithValueAsync(key));

    /// <inheritdoc cref="IBatchHashCommands.HashRandomFieldsWithValues(ValkeyKey, long)" />
    public T HashRandomFieldsWithValues(ValkeyKey key, long count) => AddCmd(HashRandomFieldsWithValuesAsync(key, count));

    /// <inheritdoc cref="IBatchHashCommands.HashGet(ValkeyKey, IEnumerable{ValkeyValue}, GetExpiryOptions)" />
    public T HashGet(ValkeyKey key, IEnumerable<ValkeyValue> fields, GetExpiryOptions options) => AddCmd(HashGetAsync(key, [.. fields], options));

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, ValkeyValue, ValkeyValue, HashSetCondition)" />
    public T HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, HashSetCondition condition)
        => AddCmd(HashSetAsync(key, hashField, value, condition));

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, HashSetCondition)" />
    public T HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, HashSetCondition condition)
        => AddCmd(HashSetAsync(key, hashFieldsAndValues, condition));

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, HashSetOptions)" />
    public T HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, HashSetOptions options)
        => AddCmd(HashSetAsync(key, hashFieldsAndValues, options));

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, ValkeyValue, ValkeyValue, SetExpiryOptions)" />
    public T HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, SetExpiryOptions expiry)
        => HashSet(key, [new KeyValuePair<ValkeyValue, ValkeyValue>(hashField, value)], new HashSetOptions { Expiry = expiry });

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, SetExpiryOptions)" />
    public T HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, SetExpiryOptions expiry)
        => HashSet(key, hashFieldsAndValues, new HashSetOptions { Expiry = expiry });

    /// <inheritdoc cref="IBatchHashCommands.HashPersist(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T HashPersist(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => AddCmd(HashPersistAsync(key, [.. hashFields]));

    /// <inheritdoc cref="IBatchHashCommands.HashExpire(ValkeyKey, IEnumerable{ValkeyValue}, TimeSpan, ExpireCondition)" />
    public T HashExpire(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan expiry, ExpireCondition condition = ExpireCondition.Always) => AddCmd(HashExpireAsync(key, expiry, [.. hashFields], condition));

    /// <inheritdoc cref="IBatchHashCommands.HashExpireAt(ValkeyKey, IEnumerable{ValkeyValue}, DateTimeOffset, ExpireCondition)" />
    public T HashExpireAt(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTimeOffset expiry, ExpireCondition condition = ExpireCondition.Always) => AddCmd(HashExpireAtAsync(key, expiry, [.. hashFields], condition));

    /// <inheritdoc cref="IBatchHashCommands.HashExpireTime(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T HashExpireTime(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => AddCmd(HashExpireTimeAsync(key, [.. hashFields]));

    /// <inheritdoc cref="IBatchHashCommands.HashTimeToLive(ValkeyKey, IEnumerable{ValkeyValue})" />
    public T HashTimeToLive(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => AddCmd(HashTimeToLiveAsync(key, [.. hashFields]));


    // Explicit interface implementations for IBatchHashCommands
    IBatch IBatchHashCommands.HashGet(ValkeyKey key, ValkeyValue hashField) => HashGet(key, hashField);
    IBatch IBatchHashCommands.HashGet(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => HashGet(key, hashFields);
    IBatch IBatchHashCommands.HashGet(ValkeyKey key) => HashGet(key);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, IEnumerable<HashEntry> hashFields) => HashSet(key, hashFields);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when) => HashSet(key, hashField, value, when);
    IBatch IBatchHashCommands.HashDelete(ValkeyKey key, ValkeyValue hashField) => HashDelete(key, hashField);
    IBatch IBatchHashCommands.HashDelete(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => HashDelete(key, hashFields);
    IBatch IBatchHashCommands.HashExists(ValkeyKey key, ValkeyValue hashField) => HashExists(key, hashField);
    IBatch IBatchHashCommands.HashIncrement(ValkeyKey key, ValkeyValue hashField, long value) => HashIncrement(key, hashField, value);
    IBatch IBatchHashCommands.HashIncrement(ValkeyKey key, ValkeyValue hashField, double value) => HashIncrement(key, hashField, value);
    IBatch IBatchHashCommands.HashKeys(ValkeyKey key) => HashKeys(key);
    IBatch IBatchHashCommands.HashLength(ValkeyKey key) => HashLength(key);
    IBatch IBatchHashCommands.HashStringLength(ValkeyKey key, ValkeyValue hashField) => HashStringLength(key, hashField);
    IBatch IBatchHashCommands.HashValues(ValkeyKey key) => HashValues(key);
    IBatch IBatchHashCommands.HashRandomField(ValkeyKey key) => HashRandomField(key);
    IBatch IBatchHashCommands.HashRandomFields(ValkeyKey key, long count) => HashRandomFields(key, count);
    IBatch IBatchHashCommands.HashRandomFieldWithValue(ValkeyKey key) => HashRandomFieldWithValue(key);
    IBatch IBatchHashCommands.HashRandomFieldsWithValues(ValkeyKey key, long count) => HashRandomFieldsWithValues(key, count);

    // Hash Field Expire Commands explicit interface implementations
    IBatch IBatchHashCommands.HashGet(ValkeyKey key, IEnumerable<ValkeyValue> fields, GetExpiryOptions options) => HashGet(key, fields, options);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, HashSetCondition condition) => HashSet(key, hashField, value, condition);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, HashSetCondition condition) => HashSet(key, hashFieldsAndValues, condition);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, HashSetOptions options) => HashSet(key, hashFieldsAndValues, options);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, SetExpiryOptions expiry) => HashSet(key, hashField, value, expiry);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, SetExpiryOptions expiry) => HashSet(key, hashFieldsAndValues, expiry);
    IBatch IBatchHashCommands.HashPersist(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => HashPersist(key, hashFields);
    IBatch IBatchHashCommands.HashExpire(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan expiry, ExpireCondition condition) => HashExpire(key, hashFields, expiry, condition);
    IBatch IBatchHashCommands.HashExpireAt(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTimeOffset expiry, ExpireCondition condition) => HashExpireAt(key, hashFields, expiry, condition);
    IBatch IBatchHashCommands.HashExpireTime(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => HashExpireTime(key, hashFields);
    IBatch IBatchHashCommands.HashTimeToLive(ValkeyKey key, IEnumerable<ValkeyValue> hashFields) => HashTimeToLive(key, hashFields);
}
