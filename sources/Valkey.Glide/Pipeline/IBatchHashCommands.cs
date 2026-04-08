// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

internal interface IBatchHashCommands
{
    /// <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashGet(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashGet(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <inheritdoc cref="IHashBaseCommands.HashGetAllAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashGetAllAsync(ValkeyKey)" /></returns>
    IBatch HashGetAll(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}})" /></returns>
    IBatch HashSet(ValkeyKey key, IEnumerable<HashEntry> hashFields);

    /// <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue)" /></returns>
    IBatch HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always);

    /// <inheritdoc cref="IHashBaseCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashDeleteAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashDelete(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashBaseCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashDelete(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <inheritdoc cref="IHashBaseCommands.HashExistsAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashExistsAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashExists(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashBaseCommands.HashIncrementByAsync(ValkeyKey, ValkeyValue, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashIncrementByAsync(ValkeyKey, ValkeyValue, long)" /></returns>
    IBatch HashIncrement(ValkeyKey key, ValkeyValue hashField, long value = 1);

    /// <inheritdoc cref="IHashBaseCommands.HashIncrementByAsync(ValkeyKey, ValkeyValue, double)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashIncrementByAsync(ValkeyKey, ValkeyValue, double)" /></returns>
    IBatch HashIncrement(ValkeyKey key, ValkeyValue hashField, double value);

    /// <inheritdoc cref="IHashBaseCommands.HashKeysAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashKeysAsync(ValkeyKey)" /></returns>
    IBatch HashKeys(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashLengthAsync(ValkeyKey)" /></returns>
    IBatch HashLength(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashStringLength(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IHashBaseCommands.HashValuesAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashValuesAsync(ValkeyKey)" /></returns>
    IBatch HashValues(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashRandomFieldAsync(ValkeyKey)" /></returns>
    IBatch HashRandomField(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldsAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashRandomFieldsAsync(ValkeyKey, long)" /></returns>
    IBatch HashRandomFields(ValkeyKey key, long count);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)" /></returns>
    IBatch HashRandomFieldsWithValues(ValkeyKey key, long count);

    // Hash Field Expire Commands (Valkey 9.0+)

    /// <inheritdoc cref="IHashBaseCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions)" /></returns>
    IBatch HashGetEx(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options);

    /// <inheritdoc cref="IHashBaseCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions)" /></returns>
    IBatch HashSetEx(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options);

    /// <inheritdoc cref="IHashBaseCommands.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})" /></returns>
    IBatch HashPersist(ValkeyKey key, IEnumerable<ValkeyValue> fields);

    /// <inheritdoc cref="IBaseClient.HashExpireAsync(ValkeyKey, IEnumerable{ValkeyValue}, TimeSpan, ExpireCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashExpireAsync(ValkeyKey, IEnumerable{ValkeyValue}, TimeSpan, ExpireCondition)" path="/returns" /></returns>
    IBatch HashExpire(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan expiry, ExpireCondition condition = ExpireCondition.Always);

    /// <inheritdoc cref="IBaseClient.HashExpireAtAsync(ValkeyKey, IEnumerable{ValkeyValue}, DateTimeOffset, ExpireCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashExpireAtAsync(ValkeyKey, IEnumerable{ValkeyValue}, DateTimeOffset, ExpireCondition)" path="/returns" /></returns>
    IBatch HashExpireAt(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTimeOffset expiry, ExpireCondition condition = ExpireCondition.Always);

    /// <inheritdoc cref="IBaseClient.HashExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashExpireTimeAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/returns" /></returns>
    IBatch HashExpireTime(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <inheritdoc cref="IBaseClient.HashTimeToLiveAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashTimeToLiveAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/returns" /></returns>
    IBatch HashTimeToLive(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);
}
