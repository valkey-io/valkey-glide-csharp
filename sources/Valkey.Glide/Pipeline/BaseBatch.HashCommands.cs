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

    /// <inheritdoc cref="IBatchHashCommands.HashGet(ValkeyKey, ValkeyValue[])" />
    public T HashGet(ValkeyKey key, ValkeyValue[] hashFields) => AddCmd(HashGetAsync(key, hashFields));

    /// <inheritdoc cref="IBatchHashCommands.HashGetAll(ValkeyKey)" />
    public T HashGetAll(ValkeyKey key) => AddCmd(HashGetAllAsync(key));

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, HashEntry[])" />
    public T HashSet(ValkeyKey key, HashEntry[] hashFields) => AddCmd(HashSetAsync(key, hashFields));

    /// <inheritdoc cref="IBatchHashCommands.HashSet(ValkeyKey, ValkeyValue, ValkeyValue, When)" />
    public T HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always) => AddCmd(HashSetAsync(key, hashField, value, when));

    /// <inheritdoc cref="IBatchHashCommands.HashDelete(ValkeyKey, ValkeyValue)" />
    public T HashDelete(ValkeyKey key, ValkeyValue hashField) => AddCmd(HashDeleteAsync(key, hashField));

    /// <inheritdoc cref="IBatchHashCommands.HashDelete(ValkeyKey, ValkeyValue[])" />
    public T HashDelete(ValkeyKey key, ValkeyValue[] hashFields) => AddCmd(HashDeleteAsync(key, hashFields));

    /// <inheritdoc cref="IBatchHashCommands.HashExists(ValkeyKey, ValkeyValue)" />
    public T HashExists(ValkeyKey key, ValkeyValue hashField) => AddCmd(HashExistsAsync(key, hashField));

    /// <inheritdoc cref="IBatchHashCommands.HashIncrement(ValkeyKey, ValkeyValue, long)" />
    public T HashIncrement(ValkeyKey key, ValkeyValue hashField, long value = 1) => AddCmd(HashIncrementAsync(key, hashField, value));

    /// <inheritdoc cref="IBatchHashCommands.HashIncrement(ValkeyKey, ValkeyValue, double)" />
    public T HashIncrement(ValkeyKey key, ValkeyValue hashField, double value) => AddCmd(HashIncrementAsync(key, hashField, value));

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

    /// <inheritdoc cref="IBatchHashCommands.HashRandomFieldsWithValues(ValkeyKey, long)" />
    public T HashRandomFieldsWithValues(ValkeyKey key, long count) => AddCmd(HashRandomFieldsWithValuesAsync(key, count));

    /// <inheritdoc cref="IBatchHashCommands.HashScan(ValkeyKey, long, ValkeyValue, long)" />
    public T HashScan(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0) => AddCmd(HashScanAsync<HashEntry[]>(key, cursor, pattern, count, true));

    /// <inheritdoc cref="IBatchHashCommands.HashScanNoValues(ValkeyKey, long, ValkeyValue, long)" />
    public T HashScanNoValues(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0) => AddCmd(HashScanAsync<ValkeyValue[]>(key, cursor, pattern, count, false));

    // Hash Field Expire Commands (Valkey 9.0+)

    /// <inheritdoc cref="IBatchHashCommands.HashGetEx(ValkeyKey, ValkeyValue[], HashGetExOptions)" />
    public T HashGetEx(ValkeyKey key, ValkeyValue[] fields, HashGetExOptions options) => AddCmd(HashGetExAsync(key, fields, options));

    /// <inheritdoc cref="IBatchHashCommands.HashSetEx(ValkeyKey, Dictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)" />
    public T HashSetEx(ValkeyKey key, Dictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options) => AddCmd(HashSetExAsync(key, fieldValueMap, options));

    /// <inheritdoc cref="IBatchHashCommands.HashPersist(ValkeyKey, ValkeyValue[])" />
    public T HashPersist(ValkeyKey key, ValkeyValue[] fields) => AddCmd(HashPersistAsync(key, fields));

    /// <inheritdoc cref="IBatchHashCommands.HashExpire(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions)" />
    public T HashExpire(ValkeyKey key, long seconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options) => AddCmd(HashExpireAsync(key, seconds, fields, options));

    /// <inheritdoc cref="IBatchHashCommands.HashPExpire(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions)" />
    public T HashPExpire(ValkeyKey key, long milliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options) => AddCmd(HashPExpireAsync(key, milliseconds, fields, options));

    /// <inheritdoc cref="IBatchHashCommands.HashExpireAt(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions)" />
    public T HashExpireAt(ValkeyKey key, long unixSeconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options) => AddCmd(HashExpireAtAsync(key, unixSeconds, fields, options));

    /// <inheritdoc cref="IBatchHashCommands.HashPExpireAt(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions)" />
    public T HashPExpireAt(ValkeyKey key, long unixMilliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options) => AddCmd(HashPExpireAtAsync(key, unixMilliseconds, fields, options));

    /// <inheritdoc cref="IBatchHashCommands.HashExpireTime(ValkeyKey, ValkeyValue[])" />
    public T HashExpireTime(ValkeyKey key, ValkeyValue[] fields) => AddCmd(HashExpireTimeAsync(key, fields));

    /// <inheritdoc cref="IBatchHashCommands.HashPExpireTime(ValkeyKey, ValkeyValue[])" />
    public T HashPExpireTime(ValkeyKey key, ValkeyValue[] fields) => AddCmd(HashPExpireTimeAsync(key, fields));

    /// <inheritdoc cref="IBatchHashCommands.HashTtl(ValkeyKey, ValkeyValue[])" />
    public T HashTtl(ValkeyKey key, ValkeyValue[] fields) => AddCmd(HashTtlAsync(key, fields));

    /// <inheritdoc cref="IBatchHashCommands.HashPTtl(ValkeyKey, ValkeyValue[])" />
    public T HashPTtl(ValkeyKey key, ValkeyValue[] fields) => AddCmd(HashPTtlAsync(key, fields));

    // Explicit interface implementations for IBatchHashCommands
    IBatch IBatchHashCommands.HashGet(ValkeyKey key, ValkeyValue hashField) => HashGet(key, hashField);
    IBatch IBatchHashCommands.HashGet(ValkeyKey key, ValkeyValue[] hashFields) => HashGet(key, hashFields);
    IBatch IBatchHashCommands.HashGetAll(ValkeyKey key) => HashGetAll(key);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, HashEntry[] hashFields) => HashSet(key, hashFields);
    IBatch IBatchHashCommands.HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when) => HashSet(key, hashField, value, when);
    IBatch IBatchHashCommands.HashDelete(ValkeyKey key, ValkeyValue hashField) => HashDelete(key, hashField);
    IBatch IBatchHashCommands.HashDelete(ValkeyKey key, ValkeyValue[] hashFields) => HashDelete(key, hashFields);
    IBatch IBatchHashCommands.HashExists(ValkeyKey key, ValkeyValue hashField) => HashExists(key, hashField);
    IBatch IBatchHashCommands.HashIncrement(ValkeyKey key, ValkeyValue hashField, long value) => HashIncrement(key, hashField, value);
    IBatch IBatchHashCommands.HashIncrement(ValkeyKey key, ValkeyValue hashField, double value) => HashIncrement(key, hashField, value);
    IBatch IBatchHashCommands.HashKeys(ValkeyKey key) => HashKeys(key);
    IBatch IBatchHashCommands.HashLength(ValkeyKey key) => HashLength(key);
    IBatch IBatchHashCommands.HashStringLength(ValkeyKey key, ValkeyValue hashField) => HashStringLength(key, hashField);
    IBatch IBatchHashCommands.HashValues(ValkeyKey key) => HashValues(key);
    IBatch IBatchHashCommands.HashRandomField(ValkeyKey key) => HashRandomField(key);
    IBatch IBatchHashCommands.HashRandomFields(ValkeyKey key, long count) => HashRandomFields(key, count);
    IBatch IBatchHashCommands.HashRandomFieldsWithValues(ValkeyKey key, long count) => HashRandomFieldsWithValues(key, count);
    IBatch IBatchHashCommands.HashScan(ValkeyKey key, long cursor, ValkeyValue pattern, long count) => HashScan(key, cursor, pattern, count);
    IBatch IBatchHashCommands.HashScanNoValues(ValkeyKey key, long cursor, ValkeyValue pattern, long count) => HashScanNoValues(key, cursor, pattern, count);

    // Hash Field Expire Commands explicit interface implementations
    IBatch IBatchHashCommands.HashGetEx(ValkeyKey key, ValkeyValue[] fields, HashGetExOptions options) => HashGetEx(key, fields, options);
    IBatch IBatchHashCommands.HashSetEx(ValkeyKey key, Dictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options) => HashSetEx(key, fieldValueMap, options);
    IBatch IBatchHashCommands.HashPersist(ValkeyKey key, ValkeyValue[] fields) => HashPersist(key, fields);
    IBatch IBatchHashCommands.HashExpire(ValkeyKey key, long seconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options) => HashExpire(key, seconds, fields, options);
    IBatch IBatchHashCommands.HashPExpire(ValkeyKey key, long milliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options) => HashPExpire(key, milliseconds, fields, options);
    IBatch IBatchHashCommands.HashExpireAt(ValkeyKey key, long unixSeconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options) => HashExpireAt(key, unixSeconds, fields, options);
    IBatch IBatchHashCommands.HashPExpireAt(ValkeyKey key, long unixMilliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options) => HashPExpireAt(key, unixMilliseconds, fields, options);
    IBatch IBatchHashCommands.HashExpireTime(ValkeyKey key, ValkeyValue[] fields) => HashExpireTime(key, fields);
    IBatch IBatchHashCommands.HashPExpireTime(ValkeyKey key, ValkeyValue[] fields) => HashPExpireTime(key, fields);
    IBatch IBatchHashCommands.HashTtl(ValkeyKey key, ValkeyValue[] fields) => HashTtl(key, fields);
    IBatch IBatchHashCommands.HashPTtl(ValkeyKey key, ValkeyValue[] fields) => HashPTtl(key, fields);
}
