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
    /// await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);
    /// var entries = await client.HashGetAsync("myhash");  // {name: "Alice", age: 30}
    /// </code>
    /// </example>
    /// </remarks>
    Task<IDictionary<ValkeyValue, ValkeyValue>> HashGetAsync(ValkeyKey key);

    /// <summary>
    /// Gets the value of a hash field and sets its expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hgetex/">Valkey commands – HGETEX</seealso>
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to retrieve and set the expiry for.</param>
    /// <param name="options">The expiry options to apply.</param>
    /// <returns>The <see cref="ValkeyValue"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", "field1", "value1");
    /// var options = GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(30));
    /// var value = await client.HashGetAsync("myhash", "field1", options);  // "value1"
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to retrieve and set the expiry for.</param>
    /// <param name="options">The expiry options to apply.</param>
    /// <returns>A <see cref="ValkeyValue"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.HashSetAsync("myhash", [new("field1", "value1"), new("field2", "value2")]);
    /// var options = GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(30));
    /// var values = await client.HashGetAsync("myhash", ["field1", "field2"], options);  // ["value1", "value2"]
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
    /// await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);
    /// var fields = await client.HashKeysAsync("myhash");  // {"name", "age"}
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
    /// await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);
    /// var values = await client.HashValuesAsync("myhash");  // ["Alice", "30"]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ICollection<ValkeyValue>> HashValuesAsync(ValkeyKey key);

    /// <summary>
    /// Sets the expiry duration for a hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpire/">Valkey commands – HEXPIRE</seealso>
    /// <seealso href="https://valkey.io/commands/hpexpire/">Valkey commands – HPEXPIRE</seealso>
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set the expiry for.</param>
    /// <param name="expiry">The expiry duration.</param>
    /// <param name="condition">The condition under which to set the expiry. Defaults to <see cref="ExpireCondition.Always"/>.</param>
    /// <returns>A <see cref="HashExpireResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireIn = TimeSpan.FromSeconds(60);
    /// var result = await client.HashExpireAsync("myhash", "field1", expireIn);  // HashExpireResult.Success
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to set the expiry for.</param>
    /// <param name="expiry">The expiry duration.</param>
    /// <param name="condition">The condition under which to set the expiry. Defaults to <see cref="ExpireCondition.Always"/>.</param>
    /// <returns>A <see cref="HashExpireResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireIn = TimeSpan.FromSeconds(60);
    /// var results = await client.HashExpireAsync("myhash", ["field1", "field2"], expireIn);  // [HashExpireResult.Success, HashExpireResult.Success]
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set the expiry for.</param>
    /// <param name="expiry">The expiry timestamp.</param>
    /// <param name="condition">The condition under which to set the expiry. Defaults to <see cref="ExpireCondition.Always"/>.</param>
    /// <returns>A <see cref="HashExpireResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireAt = DateTimeOffset.UtcNow.AddMinutes(5);
    /// var result = await client.HashExpireAtAsync("myhash", "field1", expireAt);  // HashExpireResult.Success
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to set the expiry for.</param>
    /// <param name="expiry">The expiry timestamp.</param>
    /// <param name="condition">The condition under which to set the expiry. Defaults to <see cref="ExpireCondition.Always"/>.</param>
    /// <returns>A <see cref="HashExpireResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireAt = DateTimeOffset.UtcNow.AddMinutes(5);
    /// var results = await client.HashExpireAtAsync("myhash", ["field1", "field2"], expireAt);  // [HashExpireResult.Success, HashExpireResult.Success]
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to get the expiry time for.</param>
    /// <returns>An <see cref="ExpireTimeResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireTime = await client.HashExpireTimeAsync("myhash", "field1");
    /// Console.WriteLine($"field1 expires at Unix timestamp {expireTime}");
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to get the expiry time for.</param>
    /// <returns>An <see cref="ExpireTimeResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expireTimes = await client.HashExpireTimeAsync("myhash", ["field1", "field2"]);
    /// Console.WriteLine($"field2 expires at Unix timestamp {expireTimes[1]}");
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to get the time to live for.</param>
    /// <returns>A <see cref="TimeToLiveResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var ttl = await client.HashTimeToLiveAsync("myhash", "field1");
    /// Console.WriteLine($"field1 has {ttl} remaining");
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to get the time to live for.</param>
    /// <returns>A <see cref="TimeToLiveResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var ttls = await client.HashTimeToLiveAsync("myhash", ["field1", "field2"]);
    /// Console.WriteLine($"field2 has {ttls[1]} remaining");
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to remove the expiry from.</param>
    /// <returns>A <see cref="HashPersistResult"/> for <paramref name="hashField"/>.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var result = await client.HashPersistAsync("myhash", "field1");  // HashPersistResult.Success
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashFields">The fields to remove the expiry from.</param>
    /// <returns>A <see cref="HashPersistResult"/> array with one entry per field.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var results = await client.HashPersistAsync("myhash", ["field1", "field2"]);  // [HashPersistResult.Success, HashPersistResult.Success]
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
    /// await client.HashSetAsync("myhash", [new("name", "Alice"), new("age", "30")]);
    /// var entry = await client.HashRandomFieldWithValueAsync("myhash");
    /// Console.WriteLine($"Picked {entry?.Name} = {entry?.Value}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<HashEntry?> HashRandomFieldWithValueAsync(ValkeyKey key);

    /// <summary>
    /// Sets a hash field value with a condition, without expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex/">Valkey commands – HSETEX</seealso>
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="condition">The condition under which to set the field.</param>
    /// <returns><see langword="true"/> if the field was set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var wasSet = await client.HashSetAsync("myhash", "field1", "value1", HashSetCondition.OnlyIfNoneExist);  // true
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
    /// <note>Since Valkey 9.0.0.</note>
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
    /// var wasSet = await client.HashSetAsync("myhash", pairs, HashSetCondition.OnlyIfNoneExist);  // true
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
    /// <note>Since Valkey 9.0.0.</note>
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
    /// var wasSet = await client.HashSetAsync("myhash", "field1", "value1", setOptions);  // true
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
    /// <note>Since Valkey 9.0.0.</note>
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
    /// var wasSet = await client.HashSetAsync("myhash", pairs, setOptions);  // true
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
    /// <note>Since Valkey 9.0.0.</note>
    /// <param name="key">The hash key.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expiry">The expiry configuration for the field.</param>
    /// <returns><see langword="true"/> if the field was set, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60));
    /// var wasSet = await client.HashSetAsync("myhash", "field1", "value1", expiry);  // true
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
    /// <note>Since Valkey 9.0.0.</note>
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
    /// var expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60));
    /// var wasSet = await client.HashSetAsync("myhash", pairs, expiry);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        SetExpiryOptions expiry);
}
