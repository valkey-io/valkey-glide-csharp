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

    /// <inheritdoc cref="IBaseClient.HashGetAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashGetAsync(ValkeyKey)" path="/returns" /></returns>
    IBatch HashGet(ValkeyKey key);

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

    /// <inheritdoc cref="IBaseClient.HashKeysAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashKeysAsync(ValkeyKey)" path="/returns" /></returns>
    IBatch HashKeys(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashLengthAsync(ValkeyKey)" /></returns>
    IBatch HashLength(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashStringLengthAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch HashStringLength(ValkeyKey key, ValkeyValue hashField);

    /// <inheritdoc cref="IBaseClient.HashValuesAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashValuesAsync(ValkeyKey)" path="/returns" /></returns>
    IBatch HashValues(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashRandomFieldAsync(ValkeyKey)" /></returns>
    IBatch HashRandomField(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldsAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashRandomFieldsAsync(ValkeyKey, long)" /></returns>
    IBatch HashRandomFields(ValkeyKey key, long count);

    /// <inheritdoc cref="IBaseClient.HashRandomFieldWithValueAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashRandomFieldWithValueAsync(ValkeyKey)" path="/returns" /></returns>
    IBatch HashRandomFieldWithValue(ValkeyKey key);

    /// <inheritdoc cref="IHashBaseCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IHashBaseCommands.HashRandomFieldsWithValuesAsync(ValkeyKey, long)" path="/returns" /></returns>
    IBatch HashRandomFieldsWithValues(ValkeyKey key, long count);

    /// <inheritdoc cref="IBaseClient.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue}, GetExpiryOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue}, GetExpiryOptions)" path="/returns" /></returns>
    IBatch HashGet(ValkeyKey key, IEnumerable<ValkeyValue> fields, GetExpiryOptions options);

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, HashSetCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, HashSetCondition)" path="/returns" /></returns>
    IBatch HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, HashSetCondition condition);

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, HashSetCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, HashSetCondition)" path="/returns" /></returns>
    IBatch HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, HashSetCondition condition);

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, HashSetOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, HashSetOptions)" path="/returns" /></returns>
    IBatch HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, HashSetOptions options);

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, SetExpiryOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, SetExpiryOptions)" path="/returns" /></returns>
    IBatch HashSet(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, SetExpiryOptions expiry);

    /// <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, SetExpiryOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashSetAsync(ValkeyKey, IEnumerable{KeyValuePair{ValkeyValue, ValkeyValue}}, SetExpiryOptions)" path="/returns" /></returns>
    IBatch HashSet(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues, SetExpiryOptions expiry);

    /// <inheritdoc cref="IBaseClient.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.HashPersistAsync(ValkeyKey, IEnumerable{ValkeyValue})" path="/returns" /></returns>
    IBatch HashPersist(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

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
