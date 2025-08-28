// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

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

    // TODO: ALIGN HASH FIELD EXPIRE COMMANDS WITH SER AFTER SER IMPLEMENTS THEM

    /// <summary>
    /// Retrieves the values of specified fields from the hash stored at <paramref name="key"/> and
    /// optionally sets their expiration or removes it.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hgetex"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields in the hash stored at <paramref name="key"/> to retrieve from the database.</param>
    /// <param name="options">Optional parameters for the command including expiry settings or persist option.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of values associated with the given fields, in the same order as they are requested.
    /// For every field that does not exist in the hash, a <see cref="ValkeyValue.Null"/> value is returned.
    /// If <paramref name="key"/> does not exist, it returns <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new HashGetExOptions().SetExpiry(HGetExExpiry.Seconds(60));
    /// ValkeyValue[] values = await client.HashGetExAsync(key, new ValkeyValue[] { field1, field2 }, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, ValkeyValue[] fields, HashGetExOptions options, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets the specified fields to their respective values in the hash stored at <paramref name="key"/>
    /// with optional expiration and conditional options.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="fieldValueMap">A field-value map consisting of fields and their corresponding values to be set in the hash stored at the specified key.</param>
    /// <param name="options">Optional parameters for the command including conditional changes and expiry settings.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns><see langword="1"/> if all the fields' values and expiration times were set successfully, <see langword="0"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new HashSetExOptions().SetExpiry(ExpirySet.Seconds(60));
    /// long result = await client.HashSetExAsync(key, new Dictionary&lt;ValkeyValue, ValkeyValue&gt; { { field1, value1 }, { field2, value2 } }, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> HashSetExAsync(ValkeyKey key, Dictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Removes the expiration time for each specified field, turning the field from volatile (a field
    /// with expiration time) to persistent (a field that will never expire as no expiration time is
    /// associated).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpersist"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to remove expiration from.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of <see langword="long"/> values, each corresponding to a field:
    /// <list type="bullet">
    /// <item><description><c>1</c> if the expiration time was successfully removed from the field.</description></item>
    /// <item><description><c>-1</c> if the field exists but has no expiration time.</description></item>
    /// <item><description><c>-2</c> if the field does not exist in the provided hash key, or the hash key does not exist.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long[] results = await client.HashPersistAsync(key, new ValkeyValue[] { field1, field2, field3 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashPersistAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets expiration time for hash fields. HEXPIRE sets the expiration time in seconds for the
    /// specified fields of the hash stored at <paramref name="key"/>. You can specify whether to set the
    /// expiration only if the field has no expiration, only if the field has an existing expiration,
    /// only if the new expiration is greater than the current one, or only if the new expiration is
    /// less than the current one.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpire"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="seconds">The expiration time in seconds.</param>
    /// <param name="fields">The fields in the hash stored at <paramref name="key"/> to set expiration for.</param>
    /// <param name="options">The expiration condition options.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of <see langword="long"/> values indicating the result of setting expiration for each field:
    /// <list type="bullet">
    /// <item><description><c>1</c> if the expiration time was successfully set for the field.</description></item>
    /// <item><description><c>0</c> if the specified condition was not met.</description></item>
    /// <item><description><c>-2</c> if the field does not exist in the HASH, or key does not exist.</description></item>
    /// <item><description><c>2</c> when called with 0 seconds.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new HashFieldExpirationConditionOptions().SetCondition(ExpireOptions.HAS_NO_EXPIRY);
    /// long[] results = await client.HashExpireAsync(key, 60, new ValkeyValue[] { field1, field2 }, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashExpireAsync(ValkeyKey key, long seconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets expiration time for hash fields, in milliseconds. Creates the hash if it doesn't exist. If
    /// a field is already expired, it will be deleted rather than expired.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpexpire"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="milliseconds">The expiration time to set for the fields, in milliseconds.</param>
    /// <param name="fields">The fields to set expiration for.</param>
    /// <param name="options">The expiration options.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of <see langword="long"/> values, each corresponding to a field:
    /// <list type="bullet">
    /// <item><description><c>1</c> if the expiration time was successfully set for the field.</description></item>
    /// <item><description><c>0</c> if the specified condition was not met.</description></item>
    /// <item><description><c>-2</c> if the field does not exist in the HASH, or HASH is empty.</description></item>
    /// <item><description><c>2</c> when called with 0 milliseconds.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new HashFieldExpirationConditionOptions().SetCondition(ExpireOptions.HAS_NO_EXPIRY);
    /// long[] results = await client.HashPExpireAsync(key, 5000, new ValkeyValue[] { field1, field2 }, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashPExpireAsync(ValkeyKey key, long milliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets expiration time for hash fields, in seconds, using an absolute Unix timestamp. Creates the
    /// hash if it doesn't exist. If a field is already expired, it will be deleted rather than
    /// expired.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpireat"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="unixSeconds">The expiration time to set for the fields, as a Unix timestamp in seconds.</param>
    /// <param name="fields">The fields to set expiration for.</param>
    /// <param name="options">The expiration options.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of <see langword="long"/> values, each corresponding to a field:
    /// <list type="bullet">
    /// <item><description><c>1</c> if the expiration time was successfully set for the field.</description></item>
    /// <item><description><c>0</c> if the specified condition was not met.</description></item>
    /// <item><description><c>-2</c> if the field does not exist in the HASH, or HASH is empty.</description></item>
    /// <item><description><c>2</c> when called with 0 seconds or past Unix time.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new HashFieldExpirationConditionOptions().SetCondition(ExpireOptions.HAS_NO_EXPIRY);
    /// long[] results = await client.HashExpireAtAsync(key, 1672531200, new ValkeyValue[] { field1, field2 }, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashExpireAtAsync(ValkeyKey key, long unixSeconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Sets expiration time for hash fields, using an absolute Unix timestamp in milliseconds. 
    /// HPEXPIREAT has the same effect and semantic as HEXPIREAT, but the Unix time
    /// at which the field will expire is specified in milliseconds instead of seconds.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpexpireat"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="unixMilliseconds">The expiration time to set for the fields, as a Unix timestamp in milliseconds.</param>
    /// <param name="fields">An array of hash field names for which to set the expiration.</param>
    /// <param name="options">Optional conditions and configurations for the expiration.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of <see langword="long"/> values indicating the result for each field:
    /// <list type="bullet">
    /// <item><description><c>1</c> if the expiration time was successfully set for the field.</description></item>
    /// <item><description><c>0</c> if the specified condition was not met.</description></item>
    /// <item><description><c>-2</c> if the field does not exist in the HASH, or HASH is empty.</description></item>
    /// <item><description><c>2</c> when called with 0 seconds or past Unix time in milliseconds.</description></item>
    /// </list>
    /// If <paramref name="unixMilliseconds"/> is in the past, the field will be deleted rather than expired.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var options = new HashFieldExpirationConditionOptions().SetCondition(ExpireOptions.HAS_NO_EXPIRY);
    /// long[] results = await client.HashPExpireAtAsync(key, 1672531200000, new ValkeyValue[] { field1, field2 }, options);
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashPExpireAtAsync(ValkeyKey key, long unixMilliseconds, ValkeyValue[] fields, HashFieldExpirationConditionOptions options, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the absolute Unix timestamp (in seconds) at which the given hash fields will expire.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpiretime"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to get the expiration timestamp for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of expiration timestamps in seconds for the specified fields:
    /// <list type="bullet">
    /// <item><description>For fields with a timeout, returns the absolute Unix timestamp in seconds.</description></item>
    /// <item><description>For fields that exist but have no associated expire, returns <c>-1</c>.</description></item>
    /// <item><description>For fields that do not exist in the provided hash key, or the hash key is empty, returns <c>-2</c>.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long[] timestamps = await client.HashExpireTimeAsync(key, new ValkeyValue[] { field1, field2, field3 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashExpireTimeAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the absolute Unix timestamp (in milliseconds) at which the given hash fields will expire.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpexpiretime"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to get the expiration timestamp for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of expiration timestamps in milliseconds for the specified fields:
    /// <list type="bullet">
    /// <item><description>For fields with a timeout, returns the absolute Unix timestamp in milliseconds.</description></item>
    /// <item><description>For fields that exist but have no associated expire, returns <c>-1</c>.</description></item>
    /// <item><description>For fields that do not exist in the provided hash key, or the hash key is empty, returns <c>-2</c>.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long[] timestamps = await client.HashPExpireTimeAsync(key, new ValkeyValue[] { field1, field2, field3 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashPExpireTimeAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the remaining time to live of hash fields that have a timeout, in seconds.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/httl"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to get the TTL for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of <see langword="long"/> values, each corresponding to a field:
    /// <list type="bullet">
    /// <item><description>TTL in seconds if the field exists and has a timeout.</description></item>
    /// <item><description><c>-1</c> if the field exists but has no associated expire.</description></item>
    /// <item><description><c>-2</c> if the field does not exist in the provided hash key, or the hash key is empty.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long[] ttls = await client.HashTtlAsync(key, new ValkeyValue[] { field1, field2, field3 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashTtlAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the remaining time to live of hash fields that have a timeout, in milliseconds.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpttl"/>
    /// <note>
    /// Since: Valkey 9.0 and above.
    /// </note>
    /// <param name="key">The key of the hash.</param>
    /// <param name="fields">The fields to get the TTL for.</param>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    /// <returns>
    /// An array of TTL values in milliseconds for the specified fields:
    /// <list type="bullet">
    /// <item><description>For fields with a timeout, returns the remaining TTL in milliseconds.</description></item>
    /// <item><description>For fields that exist but have no associated expire, returns <c>-1</c>.</description></item>
    /// <item><description>For fields that do not exist in the provided hash key, or the hash key is empty, returns <c>-2</c>.</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// long[] ttls = await client.HashPTtlAsync(key, new ValkeyValue[] { field1, field2, field3 });
    /// </code>
    /// </example>
    /// </remarks>
    Task<long[]> HashPTtlAsync(ValkeyKey key, ValkeyValue[] fields, CommandFlags flags = CommandFlags.None);
}
