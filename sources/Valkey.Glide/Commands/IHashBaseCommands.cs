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
    /// Returns the value associated with a hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hget/">Valkey commands – HGET</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to get.</param>
    /// <returns>The value associated with the field, or <see cref="ValkeyValue.Null"/> when the field or key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", "name", "Alice");
    /// var value = await client.HashGetAsync("myhash", "name");  // "Alice"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField);

    /// <summary>
    /// Returns the values associated with hash fields.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hmget/">Valkey commands – HMGET</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to get.</param>
    /// <returns>
    /// An array of values associated with the given fields, in the same order as they are
    /// requested, or <see cref="ValkeyValue.Null"/> when the field or key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);
    /// var values = await client.HashGetAsync("myhash", ["name", "age"]); // ["Alice", "30"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Sets a field to a value in a hash or creates the hash if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hset/">Valkey commands – HSET</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns><see langword="true"/> if the field was added (new), <see langword="false"/> if it was updated.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var added = await client.HashSetAsync("myhash", "name", "Alice");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value);

    /// <summary>
    /// Sets multiple fields to their respective values in a hash or creates them if they do not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hset/">Valkey commands – HSET</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFieldsAndValues">The field-value pairs to set.</param>
    /// <returns>The number of fields that were added (not updated).</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var added = await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashSetAsync(ValkeyKey key, IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues);

    /// <summary>
    /// Sets a field to a value in a hash, only if the field does not yet exist.
    /// Creates the hash if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetnx/">Valkey commands – HSETNX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <returns><see langword="true"/> if <paramref name="hashField"/> is a new field and <paramref name="value"/> was set, <see langword="false"/> if <paramref name="hashField"/> already exists.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var added = await client.HashSetIfNotExistsAsync("myhash", "name", "Alice");  // true
    /// var exists = await client.HashSetIfNotExistsAsync("myhash", "name", "Bob");   // false
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetIfNotExistsAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value);

    /// <summary>
    /// Removes a field from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hdel/">Valkey commands – HDEL</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to remove.</param>
    /// <returns><see langword="true"/> if the field was present in the hash, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", "name", "Alice");
    /// var removed = await client.HashDeleteAsync("myhash", "name");  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField);

    /// <summary>
    /// Removes one or more fields from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hdel/">Valkey commands – HDEL</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to remove.</param>
    /// <returns>The number of fields that were removed from the hash.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);
    /// var removedCount = await client.HashDeleteAsync("myhash", ["name", "age"]);  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Checks if a field exists in a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexists/">Valkey commands – HEXISTS</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to check.</param>
    /// <returns><see langword="true"/> if the hash contains the field, <see langword="false"/> if the hash does not contain the field or the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", "name", "Alice");
    /// var exists = await client.HashExistsAsync("myhash", "name");     // true
    /// var missing = await client.HashExistsAsync("myhash", "email");   // false
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField);

    /// <summary>
    /// Increments the integer value of a hash field by the given amount.
    /// If the key does not exist, a new hash is created. If the field does not exist, it is set to <c>0</c> before incrementing.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hincrby/">Valkey commands – HINCRBY</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to increment.</param>
    /// <param name="value">The amount to increment.</param>
    /// <returns>The value of <paramref name="hashField"/> after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var counter = await client.HashIncrementByAsync("myhash", "counter", 5);  // 5
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, long value = 1);

    /// <summary>
    /// Increments the floating-point value of a hash field by the given amount.
    /// If the key does not exist, a new hash is created.
    /// If the field does not exist, it is set to <c>0</c> before incrementing.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hincrbyfloat/">Valkey commands – HINCRBYFLOAT</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to increment.</param>
    /// <param name="value">The amount to increment.</param>
    /// <returns>The value of <paramref name="hashField"/> after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var score = await client.HashIncrementByAsync("myhash", "score", 2.5);  // 2.5
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> HashIncrementByAsync(ValkeyKey key, ValkeyValue hashField, double value);

    /// <summary>
    /// Returns the number of fields in a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hlen/">Valkey commands – HLEN</seealso>
    /// <param name="key">The hash key.</param>
    /// <returns>The number of fields in the hash, or <c>0</c> when the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);
    /// var fieldCount = await client.HashLengthAsync("myhash");  // 2
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashLengthAsync(ValkeyKey key);

    /// <summary>
    /// Returns the string length of the value associated with a hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hstrlen/">Valkey commands – HSTRLEN</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to get the string length of.</param>
    /// <returns>The length of the string value associated with the field, or <c>0</c> when the field or key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", "name", "Alice");
    /// var length = await client.HashStringLengthAsync("myhash", "name");  // 5
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField);

    /// <summary>
    /// Gets a random field name from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield/">Valkey commands – HRANDFIELD</seealso>
    /// <param name="key">The hash key.</param>
    /// <returns>A random field name or <see cref="ValkeyValue.Null"/> if the hash does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);
    /// var randomField = await client.HashRandomFieldAsync("myhash");  // "name" or "age"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key);

    /// <summary>
    /// Gets random field names from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield/">Valkey commands – HRANDFIELD</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="count">
    /// The maximum number of field names to return.
    /// If positive, returns up to <paramref name="count"/> distinct fields.
    /// If negative, allows duplicates and returns exactly <c>abs(count)</c> fields.
    /// </param>
    /// <returns>An array of field names, or an empty array if the hash does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var distinctFields = await client.HashRandomFieldsAsync("myhash", 3);
    /// // Output: ["name", "age", "city"]
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var randomFields = await client.HashRandomFieldsAsync("myhash", -3);
    /// // Output: ["name", "name", "age"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count);

    /// <summary>
    /// Gets random field-value pairs from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield/">Valkey commands – HRANDFIELD</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="count">
    /// The number of field-value pairs to return.
    /// If positive, returns up to <paramref name="count"/> distinct pairs.
    /// If negative, allows duplicates and returns exactly <c>abs(count)</c> pairs.
    /// </param>
    /// <returns>An array of field-value pairs, or an empty array if the hash does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var pairs = await client.HashRandomFieldsWithValuesAsync("myhash", 2);
    /// // Output: [{ Name: "name", Value: "Alice" }, { Name: "age", Value: "30" }]
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count);
}
