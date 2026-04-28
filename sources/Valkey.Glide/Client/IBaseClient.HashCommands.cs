// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Returns all fields and values from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hgetall/">Valkey commands – HGETALL</seealso>
    /// <param name="key">The hash key.</param>
    /// <returns>
    /// A dictionary of field-value pairs, or an empty dictionary if <paramref name="key"/> does not exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entries = await client.HashGetAsync("myhash");
    /// foreach (var entry in entries)
    /// {
    ///     Console.WriteLine($"{entry.Key}: {entry.Value}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<IDictionary<ValkeyValue, ValkeyValue>> HashGetAsync(ValkeyKey key);

    /// <summary>
    /// Gets the value of a hash field and sets its expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hgetex/">Valkey commands – HGETEX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to retrieve and set the expiry for.</param>
    /// <param name="options">The expiry options to apply.</param>
    /// <returns>The <see cref="ValkeyValue"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var value = await client.HashGetAsync("myhash", "field1", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(30)));
    /// // value == "value1"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> HashGetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        GetExpiryOptions options);

    /// <summary>
    /// Gets the values of multiple hash fields and sets their expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hgetex/">Valkey commands – HGETEX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to retrieve and set the expiry for.</param>
    /// <param name="options">The expiry options to apply.</param>
    /// <returns>A <see cref="ValkeyValue"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var values = await client.HashGetAsync("myhash", ["field1", "field2"], GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(30)));
    /// // values[0] == value of field1, values[1] == value of field2
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> HashGetAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        GetExpiryOptions options);

    /// <summary>
    /// Returns all field names from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hkeys/">Valkey commands – HKEYS</seealso>
    /// <param name="key">The hash key.</param>
    /// <returns>A set of field names, or an empty set if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var fields = await client.HashKeysAsync("myhash");
    /// foreach (var field in fields)
    /// {
    ///     Console.WriteLine(field);
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ISet<ValkeyValue>> HashKeysAsync(ValkeyKey key);

    /// <summary>
    /// Returns all values from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hvals/">Valkey commands – HVALS</seealso>
    /// <param name="key">The hash key.</param>
    /// <returns>A collection of values, or an empty collection if <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var values = await client.HashValuesAsync("myhash");
    /// foreach (var value in values)
    /// {
    ///     Console.WriteLine(value);
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<ICollection<ValkeyValue>> HashValuesAsync(ValkeyKey key);

    /// <summary>
    /// Sets the expiry duration for a hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpire/">Valkey commands – HEXPIRE</seealso>
    /// <seealso href="https://valkey.io/commands/hpexpire/">Valkey commands – HPEXPIRE</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set the expiry for.</param>
    /// <param name="expiry">The expiry duration.</param>
    /// <param name="condition">The condition under which to set the expiry. Defaults to <see cref="ExpireCondition.Always"/>.</param>
    /// <returns>A <see cref="HashExpireResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.HashExpireAsync("myhash", "field1", TimeSpan.FromSeconds(60));
    /// // result == HashExpireResult.Success
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashExpireResult> HashExpireAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        TimeSpan expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Sets the expiry duration for multiple hash fields.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpire/">Valkey commands – HEXPIRE</seealso>
    /// <seealso href="https://valkey.io/commands/hpexpire/">Valkey commands – HPEXPIRE</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to set the expiry for.</param>
    /// <param name="expiry">The expiry duration.</param>
    /// <param name="condition">The condition under which to set the expiry. Defaults to <see cref="ExpireCondition.Always"/>.</param>
    /// <returns>A <see cref="HashExpireResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var results = await client.HashExpireAsync("myhash", ["field1", "field2"], TimeSpan.FromSeconds(60));
    /// // results[0] == result for field1, results[1] == result for field2
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashExpireResult[]> HashExpireAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        TimeSpan expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Sets the expiry timestamp for a hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpireat/">Valkey commands – HEXPIREAT</seealso>
    /// <seealso href="https://valkey.io/commands/hpexpireat/">Valkey commands – HPEXPIREAT</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set the expiry for.</param>
    /// <param name="expiry">The expiry timestamp.</param>
    /// <param name="condition">The condition under which to set the expiry. Defaults to <see cref="ExpireCondition.Always"/>.</param>
    /// <returns>A <see cref="HashExpireResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireAt = DateTimeOffset.UtcNow.AddMinutes(5);
    /// var result = await client.HashExpireAtAsync("myhash", "field1", expireAt);
    /// // result == HashExpireResult.Success
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashExpireResult> HashExpireAtAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        DateTimeOffset expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Sets the expiry timestamp for multiple hash fields.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpireat/">Valkey commands – HEXPIREAT</seealso>
    /// <seealso href="https://valkey.io/commands/hpexpireat/">Valkey commands – HPEXPIREAT</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to set the expiry for.</param>
    /// <param name="expiry">The expiry timestamp.</param>
    /// <param name="condition">The condition under which to set the expiry. Defaults to <see cref="ExpireCondition.Always"/>.</param>
    /// <returns>A <see cref="HashExpireResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireAt = DateTimeOffset.UtcNow.AddMinutes(5);
    /// var results = await client.HashExpireAtAsync("myhash", ["field1", "field2"], expireAt);
    /// // results[0] == result for field1, results[1] == result for field2
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashExpireResult[]> HashExpireAtAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        DateTimeOffset expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Gets the expiry time for a hash field as an absolute Unix timestamp.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpiretime/">Valkey commands – HEXPIRETIME</seealso>
    /// <seealso href="https://valkey.io/commands/hpexpiretime/">Valkey commands – HPEXPIRETIME</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to get the expiry time for.</param>
    /// <returns>An <see cref="ExpireTimeResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireTime = await client.HashExpireTimeAsync("myhash", "field1");
    /// // expireTime contains the expiry timestamp
    /// </code>
    /// </example>
    /// </remarks>
    Task<ExpireTimeResult> HashExpireTimeAsync(
        ValkeyKey key,
        ValkeyValue hashField);

    /// <summary>
    /// Gets the expiry time for multiple hash fields as absolute Unix timestamps.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpiretime/">Valkey commands – HEXPIRETIME</seealso>
    /// <seealso href="https://valkey.io/commands/hpexpiretime/">Valkey commands – HPEXPIRETIME</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to get the expiry time for.</param>
    /// <returns>An <see cref="ExpireTimeResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireTimes = await client.HashExpireTimeAsync("myhash", ["field1", "field2"]);
    /// // expireTimes[0] == expiry time for field1, expireTimes[1] == expiry time for field2
    /// </code>
    /// </example>
    /// </remarks>
    Task<ExpireTimeResult[]> HashExpireTimeAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Gets the remaining time to live for a hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/httl/">Valkey commands – HTTL</seealso>
    /// <seealso href="https://valkey.io/commands/hpttl/">Valkey commands – HPTTL</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to get the time to live for.</param>
    /// <returns>A <see cref="TimeToLiveResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var ttl = await client.HashTimeToLiveAsync("myhash", "field1");
    /// // ttl contains the remaining time to live
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeToLiveResult> HashTimeToLiveAsync(
        ValkeyKey key,
        ValkeyValue hashField);

    /// <summary>
    /// Gets the remaining time to live for multiple hash fields.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/httl/">Valkey commands – HTTL</seealso>
    /// <seealso href="https://valkey.io/commands/hpttl/">Valkey commands – HPTTL</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to get the time to live for.</param>
    /// <returns>A <see cref="TimeToLiveResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var ttls = await client.HashTimeToLiveAsync("myhash", ["field1", "field2"]);
    /// // ttls[0] == TTL for field1, ttls[1] == TTL for field2
    /// </code>
    /// </example>
    /// </remarks>
    Task<TimeToLiveResult[]> HashTimeToLiveAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Removes the expiry from a hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpersist/">Valkey commands – HPERSIST</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to remove the expiry from.</param>
    /// <returns>A <see cref="HashPersistResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.HashPersistAsync("myhash", "field1");
    /// // result == HashPersistResult.Success
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashPersistResult> HashPersistAsync(
        ValkeyKey key,
        ValkeyValue hashField);

    /// <summary>
    /// Removes the expiry from multiple hash fields.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpersist/">Valkey commands – HPERSIST</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to remove the expiry from.</param>
    /// <returns>A <see cref="HashPersistResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var results = await client.HashPersistAsync("myhash", ["field1", "field2"]);
    /// // results[0] == result for field1, results[1] == result for field2
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashPersistResult[]> HashPersistAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Gets a random field-value pair from a hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield/">Valkey commands – HRANDFIELD</seealso>
    /// <param name="key">The hash key.</param>
    /// <returns>A random <see cref="HashEntry"/>, or <see langword="null" /> if the hash does not exist or is empty.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var entry = await client.HashRandomFieldWithValueAsync("myhash");
    /// if (entry.HasValue)
    /// {
    ///     Console.WriteLine($"{entry.Value.Name}: {entry.Value.Value}");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashEntry?> HashRandomFieldWithValueAsync(ValkeyKey key);

    /// <summary>
    /// Sets a hash field value with a condition, without expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex/">Valkey commands – HSETEX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="condition">The condition under which to set the field.</param>
    /// <returns><see langword="true"/> if the field was set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var wasSet = await client.HashSetAsync("myhash", "field1", "value1", HashSetCondition.OnlyIfNoneExist);
    /// // wasSet == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        ValkeyValue value,
        HashSetCondition condition);

    /// <summary>
    /// Sets multiple hash field values with a condition, without expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex/">Valkey commands – HSETEX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFieldsAndValues">The field-value pairs to set.</param>
    /// <param name="condition">The condition under which to set the fields.</param>
    /// <returns><see langword="true"/> if the fields were set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// KeyValuePair&lt;ValkeyValue, ValkeyValue&gt;[] pairs = [
    ///     new("field1", "value1"),
    ///     new("field2", "value2")
    /// ];
    /// var wasSet = await client.HashSetAsync("myhash", pairs, HashSetCondition.OnlyIfNoneExist);
    /// // wasSet == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetCondition condition);

    /// <summary>
    /// Sets a hash field value with options for expiry and condition.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex/">Valkey commands – HSETEX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="options">The options including expiry and condition.</param>
    /// <returns><see langword="true"/> if the field was set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var setOptions = new HashSetOptions
    /// {
    ///     Condition = HashSetCondition.OnlyIfNoneExist,
    ///     Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))
    /// };
    /// var wasSet = await client.HashSetAsync("myhash", "field1", "value1", setOptions);
    /// // wasSet == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        ValkeyValue value,
        HashSetOptions options);

    /// <summary>
    /// Sets multiple hash field values with options for expiry and condition.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex/">Valkey commands – HSETEX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFieldsAndValues">The field-value pairs to set.</param>
    /// <param name="options">The options including expiry and condition.</param>
    /// <returns><see langword="true"/> if the fields were set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var setOptions = new HashSetOptions
    /// {
    ///     Condition = HashSetCondition.OnlyIfNoneExist,
    ///     Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))
    /// };
    /// KeyValuePair&lt;ValkeyValue, ValkeyValue&gt;[] pairs = [
    ///     new("field1", "value1"),
    ///     new("field2", "value2")
    /// ];
    /// var wasSet = await client.HashSetAsync("myhash", pairs, setOptions);
    /// // wasSet == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetOptions options);

    /// <summary>
    /// Sets a hash field value with an expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex/">Valkey commands – HSETEX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expiry">The expiry configuration for the field.</param>
    /// <returns><see langword="true"/> if the field was set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var wasSet = await client.HashSetAsync("myhash", "field1", "value1", SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)));
    /// // wasSet == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        ValkeyValue value, SetExpiryOptions expiry);

    /// <summary>
    /// Sets multiple hash field values with an expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex/">Valkey commands – HSETEX</seealso>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFieldsAndValues">The field-value pairs to set.</param>
    /// <param name="expiry">The expiry configuration for the fields.</param>
    /// <returns><see langword="true"/> if the fields were set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// KeyValuePair&lt;ValkeyValue, ValkeyValue&gt;[] pairs = [
    ///     new("field1", "value1"),
    ///     new("field2", "value2")
    /// ];
    /// var wasSet = await client.HashSetAsync("myhash", pairs, SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)));
    /// // wasSet == true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        SetExpiryOptions expiry);
}
