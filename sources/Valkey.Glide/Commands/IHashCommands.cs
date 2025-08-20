// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Commands;

/// <summary>
/// Supports commands for the "Hash Commands" group for standalone and cluster clients.
/// <br/>
/// See more on <see href="https://valkey.io/commands#hash">valkey.io</see>.
/// </summary>
public interface IHashCommands
{
    /// <summary>
    /// Returns the value associated with field in the hash stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hget"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field in the hash to get.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The value associated with field, or <see cref="ValkeyValue.Null"/> when field is not present in the hash or key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue value = await client.HashGetAsync(key, hashField);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the values associated with the specified fields in the hash stored at key.
    /// For every field that does not exist in the hash, a <see cref="ValkeyValue.Null"/> value is returned.
    /// Because non-existing keys are treated as empty hashes, running HMGET against a non-existing key will return a list of <see cref="ValkeyValue.Null"/> values.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hmget"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFields">The fields in the hash to get.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>List of values associated with the given fields, in the same order as they are requested.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] values = await client.HashGetAsync(key, new ValkeyValue[] { field1, field2 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, ValkeyValue[] hashFields, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns all fields and values of the hash stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hgetall"/>
    /// <param name="key">The key of the hash to get all entries from.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>List of fields and their values stored in the hash, or an empty list when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// HashEntry[] entries = await client.HashGetAllAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets the specified fields to their respective values in the hash stored at key.
    /// This command overwrites any specified fields that already exist in the hash, leaving other unspecified fields untouched.
    /// If key does not exist, a new key holding a hash is created.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hmset"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFields">The entries to set in the hash.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync(key, new HashEntry[] { new HashEntry(field1, value1), new HashEntry(field2, value2) });
    /// </code>
    /// </example>
    /// </remarks>
    Task HashSetAsync(ValkeyKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets <paramref name="hashField"/> in the hash stored at <paramref name="key"/> to <paramref name="value"/>.
    /// If <paramref name="key"/> does not exist, a new key holding a hash is created.
    /// If <paramref name="hashField"/> already exists in the hash, it is overwritten.
    /// 
    /// Sets <paramref name="hashField"/> in the hash stored at <paramref name="key"/> to <paramref name="value"/>, only if <paramref name="hashField"/> does not yet exist.
    /// If <paramref name="key"/> does not exist, a new key holding a hash is created.
    /// If <paramref name="hashField"/> already exists, this operation has no effect.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hset"/>
    /// <seealso href="https://valkey.io/commands/hsetnx"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set in the hash.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="when">Which conditions under which to set the field value (defaults to always).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns><see langword="true"/> if <paramref name="hashField"/> is a new field in the hash and <paramref name="value"/> was set, <see langword="false"/> if <paramref name="hashField"/> already exists in the hash and no operation was performed.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool isNewField = await client.HashSetAsync(key, hashField, value);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified field from the hash stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hdel"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to remove from the hash.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns><see langword="true"/> if the field was removed, <see langword="false"/> if the field was not found or the key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool removed = await client.HashDeleteAsync(key, hashField);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the specified fields from the hash stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hdel"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFields">The fields to remove from the hash.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of fields that were removed from the hash, not including specified but non-existing fields.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long removedCount = await client.HashDeleteAsync(key, new ValkeyValue[] { field1, field2 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashDeleteAsync(ValkeyKey key, ValkeyValue[] hashFields, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns if field is an existing field in the hash stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexists"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to check in the hash.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns><see langword="true"/> if the hash contains the field, <see langword="false"/> if the hash does not contain the field or key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// bool exists = await client.HashExistsAsync(key, hashField);
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Increments the number stored at <paramref name="hashField"/> in the hash stored at <paramref name="key"/> by increment.
    /// By using a negative increment value, the value stored at <paramref name="hashField"/> in the hash stored at <paramref name="key"/> is decremented.
    /// If <paramref name="hashField"/> or <paramref name="key"/> does not exist, it is set to <c>0</c> before performing the operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hincrby"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field in the hash stored at <paramref name="key"/> to increment its value.</param>
    /// <param name="value">The amount to increment.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The value of <paramref name="hashField"/> in the hash stored at <paramref name="key"/> after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long newValue = await client.HashIncrementAsync(key, hashField, 5);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Increments the string representing a floating point number stored at <paramref name="hashField"/> in the hash stored at <paramref name="key"/> by increment.
    /// By using a negative increment value, the value stored at <paramref name="hashField"/> in the hash stored at <paramref name="key"/> is decremented.
    /// If <paramref name="hashField"/> or <paramref name="key"/> does not exist, it is set to <c>0</c> before performing the operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hincrbyfloat"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field in the hash stored at <paramref name="key"/> to increment its value.</param>
    /// <param name="value">The amount to increment.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The value of <paramref name="hashField"/> in the hash stored at <paramref name="key"/> after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// double newValue = await client.HashIncrementAsync(key, hashField, 2.5);
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns all field names in the hash stored at <paramref name="key"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hkeys"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array containing all the field names in the hash, or an empty array when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] fields = await client.HashKeysAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the number of fields contained in the hash stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hlen"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The number of fields in the hash, or 0 when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long fieldCount = await client.HashLengthAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Iterates fields of Hash types and their associated values.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hscan"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="pattern">The pattern to match fields against (defaults to all fields).</param>
    /// <param name="pageSize">The number of elements to return in each page (defaults to 250).</param>
    /// <param name="cursor">The cursor that points to the next iteration of results.</param>
    /// <param name="pageOffset">The page offset to start at (defaults to 0).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the HashSet.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await foreach (HashEntry entry in client.HashScanAsync(key, "*pattern*"))
    /// {
    ///     // Process each hash entry
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    IAsyncEnumerable<HashEntry> HashScanAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Iterates field names of Hash types (without values).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hscan"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="pattern">The pattern to match fields against (defaults to all fields).</param>
    /// <param name="pageSize">The number of elements to return in each page (defaults to 250).</param>
    /// <param name="cursor">The cursor that points to the next iteration of results.</param>
    /// <param name="pageOffset">The page offset to start at (defaults to 0).</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> that yields all matching elements of the HashSet.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await foreach (ValkeyValue field in client.HashScanNoValuesAsync(key, "*pattern*"))
    /// {
    ///     // Process each hash field name
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    IAsyncEnumerable<ValkeyValue> HashScanNoValuesAsync(ValkeyKey key, ValkeyValue pattern = default, int pageSize = 250, long cursor = 0, int pageOffset = 0, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the string length of the value associated with field in the hash stored at key.
    /// If the key or the field do not exist, 0 is returned.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hstrlen"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to get the string length of its value.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>The length of the string value associated with field, or 0 when field or key do not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long length = await client.HashStringLengthAsync(key, hashField);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns all values in the hash stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hvals"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>List of values in the hash, or an empty list when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] values = await client.HashValuesAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets a random field from the hash at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield"/>
    /// <note>
    /// Since: Valkey 6.2.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>A random hash field name or <see cref="ValkeyValue.Null"/> if the hash does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue randomField = await client.HashRandomFieldAsync(key);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets count field names from the hash at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield"/>
    /// <note>
    /// Since: Valkey 6.2.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="count">The number of fields to return.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of hash field names of size of at most count, or an empty array if the hash does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] randomFields = await client.HashRandomFieldsAsync(key, 3);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets count field names and values from the hash at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield"/>
    /// <note>
    /// Since: Valkey 6.2.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="count">The number of fields to return.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>An array of hash entries of size of at most count, or an empty array if the hash does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// HashEntry[] randomEntries = await client.HashRandomFieldsWithValuesAsync(key, 3);
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags = CommandFlags.None);
}
