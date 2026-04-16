// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Returns all fields and values from the specified hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hgetall"/>
    /// <param name="key">The key of the hash.</param>
    /// <returns>A dictionary of field-value pairs, or an empty dictionary if the key does not exist.</returns>
    Task<IDictionary<ValkeyValue, ValkeyValue>> HashGetAsync(ValkeyKey key);

    /// <summary>
    /// Gets the value and sets the expiry for the specified hash field(s).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hgetex"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to retrieve and set the expiry for.</param>
    /// <param name="options">The options for setting the expiry.</param>
    /// <returns>The <see cref="ValkeyValue"/> for the field.</returns>
    Task<ValkeyValue> HashGetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        GetExpiryOptions options);

    /// <inheritdoc cref="HashGetAsync(ValkeyKey, ValkeyValue, GetExpiryOptions)"/>
    /// <param name="hashFields">The fields to retrieve and set the expiry for.</param>
    /// <returns>A <see cref="ValkeyValue"/> array with one entry per field.</returns>
    Task<ValkeyValue[]> HashGetAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        GetExpiryOptions options);

    /// <summary>
    /// Returns all field names in the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hkeys"/>
    /// <param name="key">The key of the hash.</param>
    /// <returns>A set of field names, or an empty set if the key does not exist.</returns>
    Task<ISet<ValkeyValue>> HashKeysAsync(ValkeyKey key);

    /// <summary>
    /// Returns all values in the hash stored at the given key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hvals"/>
    /// <param name="key">The key of the hash.</param>
    /// <returns>A collection of values, or an empty collection if the key does not exist.</returns>
    Task<ICollection<ValkeyValue>> HashValuesAsync(ValkeyKey key);

    /// <summary>
    /// Sets the expiry duration for the specified hash field(s).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpire"/>
    /// <seealso href="https://valkey.io/commands/hpexpire"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set the expiry for.</param>
    /// <param name="expiry">The expiry duration.</param>
    /// <param name="condition">The condition under which to set the expiry.</param>
    /// <returns>A <see cref="HashExpireResult"/> for the field.</returns>
    Task<HashExpireResult> HashExpireAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        TimeSpan expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <inheritdoc cref="HashExpireAsync(ValkeyKey, ValkeyValue, TimeSpan, ExpireCondition)"/>
    /// <param name="hashFields">The fields to set the expiry for.</param>
    /// <returns>A <see cref="HashExpireResult"/> array with one entry per field.</returns>
    Task<HashExpireResult[]> HashExpireAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        TimeSpan expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Sets the expiry timestamp for the specified hash field(s).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpireat"/>
    /// <seealso href="https://valkey.io/commands/hpexpireat"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set the expiry for.</param>
    /// <param name="expiry">The expiry timestamp.</param>
    /// <param name="condition">The condition under which to set the expiry.</param>
    /// <returns>A <see cref="HashExpireResult"/> for the field.</returns>
    Task<HashExpireResult> HashExpireAtAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        DateTimeOffset expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <inheritdoc cref="HashExpireAtAsync(ValkeyKey, ValkeyValue, DateTimeOffset, ExpireCondition)"/>
    /// <param name="hashFields">The fields to set the expiry for.</param>
    /// <returns>A <see cref="HashExpireResult"/> array with one entry per field.</returns>
    Task<HashExpireResult[]> HashExpireAtAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        DateTimeOffset expiry,
        ExpireCondition condition = ExpireCondition.Always);

    /// <summary>
    /// Gets the expiry time for the specified hash field(s).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hexpiretime"/>
    /// <seealso href="https://valkey.io/commands/hpexpiretime"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to get the expiry time for.</param>
    /// <returns>An <see cref="ExpireTimeResult"/> for the field.</returns>
    Task<ExpireTimeResult> HashExpireTimeAsync(
        ValkeyKey key,
        ValkeyValue hashField);

    /// <inheritdoc cref="HashExpireTimeAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="hashFields">The fields to get the expiry time for.</param>
    /// <returns>An <see cref="ExpireTimeResult"/> array with one entry per field.</returns>
    Task<ExpireTimeResult[]> HashExpireTimeAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Returns the time to live for the specified hash field(s).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/httl"/>
    /// <seealso href="https://valkey.io/commands/hpttl"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to get the time to live for.</param>
    /// <returns>A <see cref="TimeToLiveResult"/> for the field.</returns>
    Task<TimeToLiveResult> HashTimeToLiveAsync(
        ValkeyKey key,
        ValkeyValue hashField);

    /// <inheritdoc cref="HashTimeToLiveAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="hashFields">The fields to get the time to live for.</param>
    /// <returns>A <see cref="TimeToLiveResult"/> array with one entry per field.</returns>
    Task<TimeToLiveResult[]> HashTimeToLiveAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Removes the expiry from the specified hash field(s).
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hpersist"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to remove the expiry from.</param>
    /// <returns>A <see cref="HashPersistResult"/> for the field.</returns>
    Task<HashPersistResult> HashPersistAsync(
        ValkeyKey key,
        ValkeyValue hashField);

    /// <inheritdoc cref="HashPersistAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="hashFields">The fields to remove the expiry from.</param>
    /// <returns>A <see cref="HashPersistResult"/> array with one entry per field.</returns>
    Task<HashPersistResult[]> HashPersistAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields);

    /// <summary>
    /// Gets a random field-value pair from the specified hash.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hrandfield"/>
    /// <param name="key">The key of the hash.</param>
    /// <returns>A random field-value pair, or <see langword="null"/> if the hash does not exist or is empty.</returns>
    Task<HashEntry?> HashRandomFieldWithValueAsync(ValkeyKey key);

    /// <summary>
    /// Sets the value for the specified hash field using HSETEX without expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="condition">The condition under which to set the field.</param>
    /// <returns><see langword="true"/> if the field was set, <see langword="false"/> otherwise.</returns>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        ValkeyValue value,
        HashSetCondition condition);

    /// <summary>
    /// Sets the values for the specified hash fields using HSETEX without expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashFieldsAndValues">The field-value pairs to set.</param>
    /// <param name="condition">The condition under which to set the fields.</param>
    /// <returns><see langword="true"/> if the fields were set, <see langword="false"/> otherwise.</returns>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetCondition condition);

    /// <summary>
    /// Sets the value for the specified hash field using HSETEX with options for expiry and/or condition.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="options">The options for setting the field(s), including expiry and condition.</param>
    /// <returns><see langword="true"/> if the field was set, <see langword="false"/> otherwise.</returns>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        ValkeyValue value,
        HashSetOptions options);

    /// <inheritdoc cref="HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, HashSetOptions)"/>
    /// <param name="hashFieldsAndValues">The field-value pairs to set.</param>
    /// <returns><see langword="true"/> if the fields were set, <see langword="false"/> otherwise.</returns>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        HashSetOptions options);

    /// <summary>
    /// Sets the value for the specified hash field.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/hsetex"/>
    /// <param name="key">The key of the hash.</param>
    /// <param name="hashField">The field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="expiry">The expiry configuration for the field.</param>
    /// <returns><see langword="true"/> if the field was set, <see langword="false"/> otherwise.</returns>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        ValkeyValue hashField,
        ValkeyValue value, SetExpiryOptions expiry);

    /// <inheritdoc cref="HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, SetExpiryOptions)"/>
    /// <param name="hashFieldsAndValues">The field-value pairs to set.</param>
    /// <returns><see langword="true"/> if the fields were set, <see langword="false"/> otherwise.</returns>
    Task<bool> HashSetAsync(
        ValkeyKey key,
        IEnumerable<KeyValuePair<ValkeyValue, ValkeyValue>> hashFieldsAndValues,
        SetExpiryOptions expiry);
}
