// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

internal interface IBatchHashCommands
{
    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashGet(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashGet(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <inheritdoc cref="IHashCommands.HashGetAllAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashGetAllAsync(ValkeyKey)" /></returns>
    IBatch HashGetAll(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, IEnumerable{HashEntry})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, IEnumerable{HashEntry})" /></returns>
    IBatch HashSet(ValkeyKey key, IEnumerable<HashEntry> hashFields);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When)" /></returns>
    IBatch HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashDelete(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashDelete(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <inheritdoc cref="IHashCommands.HashExistsAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashExistsAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashExists(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long)" /></returns>
    IBatch HashIncrement(ValkeyKey key, ValkeyValue hashField, long value = 1);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double)" /></returns>
    IBatch HashIncrement(ValkeyKey key, ValkeyValue hashField, double value);

    /// <inheritdoc cref="IHashCommands.HashKeysAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashKeysAsync(ValkeyKey)" /></returns>
    IBatch HashKeys(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashLengthAsync(ValkeyKey)" /></returns>
    IBatch HashLength(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashStringLength(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashCommands.HashValuesAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashValuesAsync(ValkeyKey)" /></returns>
    IBatch HashValues(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashRandomFieldAsync(ValkeyKey)" /></returns>
    IBatch HashRandomField(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashRandomFieldsAsync(ValkeyKey, long)" /></returns>
    IBatch HashRandomFields(ValkeyKey key, long count);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)" /></returns>
    IBatch HashRandomFieldsWithValues(ValkeyKey key, long count);

    /// <inheritdoc cref="IHashCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int)" /></returns>
    IBatch HashScan(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0);

    /// <inheritdoc cref="IHashCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int)" /></returns>
    IBatch HashScanNoValues(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0);

    // Hash Field Expire Commands (Valkey 9.0+)

    /// <inheritdoc cref="IHashCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)" /></returns>
    IBatch HashGetEx(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options);

    /// <inheritdoc cref="IHashCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)" /></returns>
    IBatch HashSetEx(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options);

    /// <inheritdoc cref="IHashCommands.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashPersist(ValkeyKey key, IEnumerable<ValkeyValue> fields);

    /// <inheritdoc cref="IHashCommands.HashExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" /></returns>
    IBatch HashExpire(ValkeyKey key, long seconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options);

    /// <inheritdoc cref="IHashCommands.HashPExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPExpireAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" /></returns>
    IBatch HashPExpire(ValkeyKey key, long milliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options);

    /// <inheritdoc cref="IHashCommands.HashExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" /></returns>
    IBatch HashExpireAt(ValkeyKey key, long unixSeconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options);

    /// <inheritdoc cref="IHashCommands.HashPExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPExpireAtAsync(ValkeyKey, long, IEnumerable{ValkeyValue}, HashFieldExpirationConditionOptions)" /></returns>
    IBatch HashPExpireAt(ValkeyKey key, long unixMilliseconds, IEnumerable<ValkeyValue> fields, HashFieldExpirationConditionOptions options);

    /// <inheritdoc cref="IHashCommands.HashExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashExpireTime(ValkeyKey key, IEnumerable<ValkeyValue> fields);

    /// <inheritdoc cref="IHashCommands.HashPExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashPExpireTime(ValkeyKey key, IEnumerable<ValkeyValue> fields);

    /// <inheritdoc cref="IHashCommands.HashTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashTtl(ValkeyKey key, IEnumerable<ValkeyValue> fields);

    /// <inheritdoc cref="IHashCommands.HashPTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPTtlAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashPTtl(ValkeyKey key, IEnumerable<ValkeyValue> fields);
}
