// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

// ATTENTION: Methods should only be added to this interface if they are implemented
// by both Valkey GLIDE clients and StackExchange.Redis databases.

/// <summary>
/// Hash commands for clients.
/// </summary>
/// <seealso href="https://valkey.io/commands/#hash">Valkey – Hash Commands</seealso>
public interface IHashBaseCommands
{
    /// <summary>
    /// Returns the value associated with the specified field in the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hget"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field in the hash to get.</param>
    /// <returns>The value associated with the field, or <see cref="ValkeyValue.Null"/> when field is not present in the hash or key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue value = await client.HashGetAsync("key", "field");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField);

    /// <summary>
    /// Returns the values associated with the specified fields in the hash stored at the given key.
    /// For every field that does not exist in the hash, <see cref="ValkeyValue.Null"/> value is returned.
    /// Because non-existing keys are treated as empty hashes, a non-existing key will return a list of <see cref="ValkeyValue.Null"/> values.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hmget"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFields">The fields in the hash to get.</param>
    /// <returns>List of values associated with the given fields, in the same order as they are requested.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] values = await client.HashGetAsync("key", ["field1", "field2"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Sets the specified field to its respective value in the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hset"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set in the hash.</param>
    /// <param name="value">The value to set.</param>
    /// <returns><see langword="true"/> if the field was added (new), <see langword="false"/> if it was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool added = await client.HashSetAsync("key", "field", "value");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value);

    /// <summary>
    /// Sets the specified fields to their respective values in the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hmset"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFieldsAndValues">The field-value pairs to set in the hash.</param>
    /// <returns>The number of fields that were added (not updated).</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyKey key = "myhash";
    /// long added = await client.HashSetAsync(key, new KeyValuePair&lt;ValkeyValue, ValkeyValue&gt;[] { new("field1", "value1"), new("field2", "value2") });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashSetAsync(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues);

    /// <summary>
    /// Sets the specified field to its respective value in the hash stored at the given key, only if the field does not yet exist.
    /// If the key does not exist, a new key holding a hash is created.
    /// If the key exists but the field already exists, this operation has no effect.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetnx"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set in the hash.</param>
    /// <param name="value">The value to set.</param>
    /// <returns><see langword="true"/> if <paramref name="hashField"/> is a new field in the hash and <paramref name="value"/> was set, <see langword="false"/> if <paramref name="hashField"/> already exists.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool added = await client.HashSetIfNotExistsAsync("key", "field", "value");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetIfNotExistsAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value);

    /// <summary>
    /// Removes the specified field from the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hdel"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to remove from the hash.</param>
    /// <returns><see langword="true"/> if the field was removed from the hash.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool removed = await client.HashDeleteAsync("key", "field");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField);

    /// <summary>
    /// Removes the specified fields from the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hdel"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFields">The fields to remove from the hash.</param>
    /// <returns>The number of fields that were removed from the hash.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long removedCount = await client.HashDeleteAsync("key", ["field1", "field2"]);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Returns if the specified field exists in the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexists"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to check in the hash.</param>
    /// <returns><see langword="true"/> if the hash contains the field, <see langword="false"/> if the hash does not contain the field or key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool exists = await client.HashExistsAsync("key", "field");
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField);

    /// <summary>
    /// Increments the value stored at the specified field in the hash stored at the given key by an increment.
    /// A negative increment will decremented the value.
    /// If the key does not exist, it is set to zero before performing the operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hincrby"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to increment in the hash.</param>
    /// <param name="value">The amount to increment.</param>
    /// <returns>The value of <paramref name="hashField"/> in the hash stored at <paramref name="key"/> after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long newValue = await client.HashIncrementByAsync("key", "field", 5);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, long value = 1);

    /// <summary>
    /// Increments the string representing a floating point number stored at <paramref name="hashField"/> in the hash stored at <paramref name="key"/> by increment.
    /// By using a negative increment value, the value stored at <paramref name="hashField"/> in the hash stored at <paramref name="key"/> is decremented.
    /// If <paramref name="hashField"/> or <paramref name="key"/> does not exist, it is set to <c>0</c> before performing the operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hincrbyfloat"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field in the hash stored at <paramref name="key"/> to increment its value.</param>
    /// <param name="value">The amount to increment.</param>
    /// <returns>The value of <paramref name="hashField"/> in the hash stored at <paramref name="key"/> after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// double newValue = await client.HashIncrementByAsync("key", "field", 2.5);
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, double value);

    /// <summary>
    /// Returns the number of fields contained in the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hlen"/>
    /// <param name="key">The key of the hash.</param>
    /// <returns>The number of fields in the hash, or 0 when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long fieldCount = await client.HashLengthAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashLengthAsync(ValkeyKey key);

    /// <summary>
    /// Returns the string length of the value associated with the specified field in the hash stored at the given key.
    /// If the key or the field do not exist, 0 is returned.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hstrlen"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to get the string length of its value.</param>
    /// <returns>The length of the string value associated with the field, or 0 when field or key do not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long length = await client.HashStringLengthAsync("key", "field");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField);

    /// <summary>
    /// Gets a random field name from the specified hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield"/>
    /// <note>
    /// Since: Valkey 6.2.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <returns>A random field name or <see cref="ValkeyValue.Null"/> if the hash does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue randomField = await client.HashRandomFieldAsync("key");
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key);

    /// <summary>
    /// Gets random field names from the specified hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield"/>
    /// <note>
    /// Since: Valkey 6.2.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="count">
    /// The maximum number of field names to return.
    /// If positive, returns up to <paramref name="count"/> distinct fields.
    /// If negative, allows duplicates and returns exactly <c>abs(count)</c> fields.
    /// </param>
    /// <returns>An array of field names, or an empty array if the hash does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var distinctFields = await client.HashRandomFieldsAsync("key", 3);
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var randomFields = await client.HashRandomFieldsAsync("key", -3);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count);

    /// <summary>
    /// Gets random field-value pairs from the specified hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="count">
    /// The number of field-value pairs to return.
    /// If positive, returns up to <paramref name="count"/> distinct pairs.
    /// If negative, allows duplicates and returns exactly <c>abs(count)</c> pairs.
    /// </param>
    /// <returns>An array of field-value pairs, or an empty array if the hash does not exist.</returns>
    Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count);
}
