// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

internal interface IBatchHashCommands
{
    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch HashGet(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashGetAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch HashGet(ValkeyKey key, ValkeyValue[] hashFields);

    /// <inheritdoc cref="IHashCommands.HashGetAllAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashGetAllAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch HashGetAll(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, HashEntry[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, HashEntry[], CommandFlags)" /></returns>
    IBatch HashSet(ValkeyKey key, HashEntry[] hashFields);

    /// <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When, CommandFlags)" /></returns>
    IBatch HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch HashDelete(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashDeleteAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch HashDelete(ValkeyKey key, ValkeyValue[] hashFields);

    /// <inheritdoc cref="IHashCommands.HashExistsAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashExistsAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch HashExists(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, long, CommandFlags)" /></returns>
    IBatch HashIncrement(ValkeyKey key, ValkeyValue hashField, long value = 1);

    /// <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)" /></returns>
    IBatch HashIncrement(ValkeyKey key, ValkeyValue hashField, double value);

    /// <inheritdoc cref="IHashCommands.HashKeysAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashKeysAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch HashKeys(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashLengthAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashLengthAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch HashLength(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue, CommandFlags)" /></returns>
    IBatch HashStringLength(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashCommands.HashValuesAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashValuesAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch HashValues(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldAsync(ValkeyKey, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashRandomFieldAsync(ValkeyKey, CommandFlags)" /></returns>
    IBatch HashRandomField(ValkeyKey key);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsAsync(ValkeyKey, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashRandomFieldsAsync(ValkeyKey, long, CommandFlags)" /></returns>
    IBatch HashRandomFields(ValkeyKey key, long count);

    /// <inheritdoc cref="IHashCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long, CommandFlags)" /></returns>
    IBatch HashRandomFieldsWithValues(ValkeyKey key, long count);

    /// <inheritdoc cref="IHashCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashScanAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)" /></returns>
    IBatch HashScan(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0);

    /// <inheritdoc cref="IHashCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashScanNoValuesAsync(ValkeyKey, ValkeyValue, int, long, int, CommandFlags)" /></returns>
    IBatch HashScanNoValues(ValkeyKey key, long cursor, ValkeyValue pattern = default, long count = 0);

    // Hash Field Expire Commands (Valkey 9.0+)

    /// <inheritdoc cref="IHashCommands.HashGetExAsync(ValkeyKey, ValkeyValue[], HashGetExOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashGetExAsync(ValkeyKey, ValkeyValue[], HashGetExOptions, CommandFlags)" /></returns>
    IBatch HashGetEx(ValkeyKey key, ValkeyValue[] fields, HashGetExOptions options);

    /// <inheritdoc cref="IHashCommands.HashSetExAsync(ValkeyKey, Dictionary{ValkeyValue, ValkeyValue}, HashSetExOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashSetExAsync(ValkeyKey, Dictionary{ValkeyValue, ValkeyValue}, HashSetExOptions, CommandFlags)" /></returns>
    IBatch HashSetEx(ValkeyKey key, Dictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options);

    /// <inheritdoc cref="IHashCommands.HashPersistAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPersistAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch HashPersist(ValkeyKey key, ValkeyValue[] fields);

    /// <inheritdoc cref="IHashCommands.HashExpireAsync(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashExpireAsync(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions, CommandFlags)" /></returns>
    IBatch HashExpire(ValkeyKey key, long seconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options);

    /// <inheritdoc cref="IHashCommands.HashPExpireAsync(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPExpireAsync(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions, CommandFlags)" /></returns>
    IBatch HashPExpire(ValkeyKey key, long milliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options);

    /// <inheritdoc cref="IHashCommands.HashExpireAtAsync(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashExpireAtAsync(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions, CommandFlags)" /></returns>
    IBatch HashExpireAt(ValkeyKey key, long unixSeconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options);

    /// <inheritdoc cref="IHashCommands.HashPExpireAtAsync(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions, CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPExpireAtAsync(ValkeyKey, long, ValkeyValue[], HashFieldExpirationConditionOptions, CommandFlags)" /></returns>
    IBatch HashPExpireAt(ValkeyKey key, long unixMilliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options);

    /// <inheritdoc cref="IHashCommands.HashExpireTimeAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashExpireTimeAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch HashExpireTime(ValkeyKey key, ValkeyValue[] fields);

    /// <inheritdoc cref="IHashCommands.HashPExpireTimeAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPExpireTimeAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch HashPExpireTime(ValkeyKey key, ValkeyValue[] fields);

    /// <inheritdoc cref="IHashCommands.HashTtlAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashTtlAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch HashTtl(ValkeyKey key, ValkeyValue[] fields);

    /// <inheritdoc cref="IHashCommands.HashPTtlAsync(ValkeyKey, ValkeyValue[], CommandFlags)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashCommands.HashPTtlAsync(ValkeyKey, ValkeyValue[], CommandFlags)" /></returns>
    IBatch HashPTtl(ValkeyKey key, ValkeyValue[] fields);
}
