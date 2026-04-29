// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Gets the value of a key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/get/">Valkey commands – GET</seealso>
    /// <param name="key">The key to retrieve.</param>
    /// <returns>The value of the key, or <see cref="ValkeyValue.Null"/> if it doesn't exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "hello");
    /// var value = await client.GetAsync("key");  // "hello"
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var missing = await client.GetAsync("nonexistent");  // ValkeyValue.Null
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetAsync(ValkeyKey key);

    /// <summary>
    /// Returns the values of keys.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/mget/">Valkey commands – MGET</seealso>
    /// <note>In cluster mode, if keys in <paramref name="keys"/> map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.</note>
    /// <param name="keys">The keys to retrieve.</param>
    /// <returns>An array with the value for each key, or <see cref="ValkeyValue.Null"/> if it does not exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "hello");
    /// var values = await client.GetAsync(["key", "nonexistent"]);  // ["hello", ValkeyValue.Null]
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> GetAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Appends a value to a string. If the key does not exist,
    /// create it with an empty string before appending.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/append/">Valkey commands – APPEND</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="value">The value to append.</param>
    /// <returns>The length of the string after the append operation.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "Hello");
    /// var length = await client.AppendAsync("key", " World");  // 11
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> AppendAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Decrements the integer value of a string by a given amount.
    /// If the key does not exist, it is set to 0 before decrementing.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/decr/">Valkey commands – DECR</seealso>
    /// <seealso href="https://valkey.io/commands/decrby/">Valkey commands – DECRBY</seealso>
    /// <param name="key">The key of the string to decrement.</param>
    /// <param name="value">The amount to decrement by.</param>
    /// <returns>The value of <paramref name="key"/> after the decrement.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "10");
    /// var decremented = await client.DecrementAsync("key");  // 9
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> DecrementAsync(ValkeyKey key, long value = 1);

    /// <summary>
    /// Decrements the floating point value of a string by a given amount.
    /// If the key does not exist, it is set to 0 before decrementing.
    /// Implemented via INCRBYFLOAT with a negated value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/incrbyfloat/">Valkey commands – INCRBYFLOAT</seealso>
    /// <param name="key">The key of the string to decrement.</param>
    /// <param name="value">The amount to decrement by.</param>
    /// <returns>The value of <paramref name="key"/> after the decrement.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "10.5");
    /// var decremented = await client.DecrementAsync("key", 0.5);  // 10.0
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> DecrementAsync(ValkeyKey key, double value);

    /// <summary>
    /// Increments the integer value of a string by a given amount.
    /// If the key does not exist, it is set to 0 before incrementing.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/incr/">Valkey commands – INCR</seealso>
    /// <seealso href="https://valkey.io/commands/incrby/">Valkey commands – INCRBY</seealso>
    /// <param name="key">The key of the string to increment.</param>
    /// <param name="value">The amount to increment by.</param>
    /// <returns>The value of <paramref name="key"/> after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "10");
    /// var incremented = await client.IncrementAsync("key");  // 11
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> IncrementAsync(ValkeyKey key, long value = 1);

    /// <summary>
    /// Increments the floating point value of a string by a given amount.
    /// If the key does not exist, it is set to 0 before incrementing.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/incrbyfloat/">Valkey commands – INCRBYFLOAT</seealso>
    /// <param name="key">The key of the string to increment.</param>
    /// <param name="value">The amount to increment by.</param>
    /// <returns>The value of <paramref name="key"/> after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "10.5");
    /// var incremented = await client.IncrementAsync("key", 0.5);  // 11.0
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> IncrementAsync(ValkeyKey key, double value);

    /// <summary>
    /// Gets the value of a key and deletes it.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getdel/">Valkey commands – GETDEL</seealso>
    /// <param name="key">The key to get and delete.</param>
    /// <returns>The value of <paramref name="key"/>, or <see cref="ValkeyValue.Null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "value");
    /// var deleted = await client.GetDeleteAsync("key");  // "value"
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var missing = await client.GetDeleteAsync("nonexistent");  // ValkeyValue.Null
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetDeleteAsync(ValkeyKey key);

    /// <summary>
    /// Gets the value of a key and optionally set or remove its expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getex/">Valkey commands – GETEX</seealso>
    /// <param name="key">The key to retrieve.</param>
    /// <param name="options">The expiry option to apply (expire in duration, expire at timestamp, or persist).</param>
    /// <returns>The value of <paramref name="key"/>, or <see cref="ValkeyValue.Null"/> when <paramref name="key"/> does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "value");
    /// var expiring = await client.GetExpiryAsync("key", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)));  // "value"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetExpiryAsync(ValkeyKey key, GetExpiryOptions options);

    /// <summary>
    /// Returns a substring of a string value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getrange/">Valkey commands – GETRANGE</seealso>
    /// <param name="key">The key of the string.</param>
    /// <param name="start">The starting offset (inclusive).</param>
    /// <param name="end">The ending offset (inclusive).</param>
    /// <returns>
    /// A substring extracted from the value stored at <paramref name="key"/>.<br/>
    /// An empty string is returned if <paramref name="key"/> does not exist or if the offsets are out of range.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "Hello World");
    /// var substring = await client.GetRangeAsync("key", 0, 4);  // "Hello"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetRangeAsync(ValkeyKey key, long start, long end);

    /// <summary>
    /// Sets the value of a key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">Valkey commands – SET</seealso>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given <paramref name="key"/>.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "value");
    /// </code>
    /// </example>
    /// </remarks>
    Task SetAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Sets multiple keys to multiple values in a single unconditional operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/mset/">Valkey commands – MSET</seealso>
    /// <note>In cluster mode, if keys in <paramref name="values"/> map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.</note>
    /// <param name="values">A collection of key-value pairs to set.</param>
    /// <returns>A task that completes when all values have been set.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// KeyValuePair&lt;ValkeyKey, ValkeyValue&gt;[] pairs = [
    ///     new("key1", "value1"),
    ///     new("key2", "value2")
    /// ];
    /// await client.SetAsync(pairs);
    /// </code>
    /// </example>
    /// </remarks>
    Task SetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <summary>
    /// Sets multiple keys to multiple values only if none of the specified keys exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/msetnx/">Valkey commands – MSETNX</seealso>
    /// <param name="values">A collection of key-value pairs to set.</param>
    /// <returns><see langword="true"/> if all keys were set, <see langword="false"/> if no key was set (at least one key already existed).</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// KeyValuePair&lt;ValkeyKey, ValkeyValue&gt;[] pairs = [
    ///     new("key1", "value1"),
    ///     new("key2", "value2")
    /// ];
    /// var allSet = await client.SetIfNotExistsAsync(pairs);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetIfNotExistsAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <summary>
    /// Sets the value of a key based on a specified condition.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">Valkey commands – SET</seealso>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given <paramref name="key"/>.</param>
    /// <param name="condition">The condition under which the key should be set.</param>
    /// <returns><see langword="true"/> if the key was set, <see langword="false"/> if the condition was not met.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var wasSet = await client.SetAsync("key", "value", SetCondition.OnlyIfDoesNotExist);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition);

    /// <summary>
    /// Sets the value of a key with options including condition and expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">Valkey commands – SET</seealso>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given <paramref name="key"/>.</param>
    /// <param name="options">The options for the SET command, including condition and expiry.</param>
    /// <returns><see langword="true"/> if the key was set, <see langword="false"/> if the condition was not met.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// var setOptions = new SetOptions { Condition = SetCondition.OnlyIfDoesNotExist };
    /// var wasSet = await client.SetAsync("key", "value", setOptions);  // true
    /// </code>
    /// </example>
    /// </remarks>
    Task<bool> SetAsync(ValkeyKey key, ValkeyValue value, SetOptions options);

    /// <summary>
    /// Sets the value of a key with an expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">Valkey commands – SET</seealso>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given <paramref name="key"/>.</param>
    /// <param name="expiry">The expiry configuration for the key.</param>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "value", SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)));
    /// </code>
    /// </example>
    /// </remarks>
    Task SetAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry);

    /// <summary>
    /// Gets the old value of a key and sets it to a new value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">Valkey commands – SET</seealso>
    /// <param name="key">The key to get and set.</param>
    /// <param name="value">The new value to store.</param>
    /// <returns>The old value stored at <paramref name="key"/>, or <see cref="ValkeyValue.Null"/> when <paramref name="key"/> did not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "oldValue");
    /// var previous = await client.GetSetAsync("key", "newValue");  // "oldValue"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Gets the old value of a key and sets it to a new value with a condition.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">Valkey commands – SET</seealso>
    /// <param name="key">The key to get and set.</param>
    /// <param name="value">The new value to store.</param>
    /// <param name="condition">The condition under which the key should be set.</param>
    /// <returns>The old value stored at <paramref name="key"/>, or <see cref="ValkeyValue.Null"/> when <paramref name="key"/> did not exist or the condition was not met.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "oldValue");
    /// var previous = await client.GetSetAsync("key", "newValue", SetCondition.OnlyIfExists);  // "oldValue"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition);

    /// <summary>
    /// Gets the old value of a key and sets it to a new value with options including condition and expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">Valkey commands – SET</seealso>
    /// <param name="key">The key to get and set.</param>
    /// <param name="value">The new value to store.</param>
    /// <param name="options">The options for the SET command, including condition and expiry.</param>
    /// <returns>The old value stored at <paramref name="key"/>, or <see cref="ValkeyValue.Null"/> when <paramref name="key"/> did not exist or the condition was not met.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "oldValue");
    /// var setOptions = new SetOptions { Condition = SetCondition.OnlyIfExists };
    /// var previous = await client.GetSetAsync("key", "newValue", setOptions);  // "oldValue"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value, SetOptions options);

    /// <summary>
    /// Gets the old value of a key and sets it to a new value with an expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">Valkey commands – SET</seealso>
    /// <param name="key">The key to get and set.</param>
    /// <param name="value">The new value to store.</param>
    /// <param name="expiry">The expiry configuration for the key.</param>
    /// <returns>The old value stored at <paramref name="key"/>, or <see cref="ValkeyValue.Null"/> when <paramref name="key"/> did not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "oldValue");
    /// var previous = await client.GetSetExpiryAsync("key", "newValue", SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)));  // "oldValue"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetSetExpiryAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry);

    /// <summary>
    /// Returns the length of a string value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/strlen/">Valkey commands – STRLEN</seealso>
    /// <param name="key">The key to check.</param>
    /// <returns>
    /// The length of the string stored at <paramref name="key"/>, or <c>0</c> if <paramref name="key"/> does not exist.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "Hello World");
    /// var length = await client.LengthAsync("key");  // 11
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> LengthAsync(ValkeyKey key);

    /// <summary>
    /// Overwrites part of a string, starting at the specified
    /// <paramref name="offset"/>, for the entire length of <paramref name="value"/>.
    /// If <paramref name="offset"/> is larger than the current length, the string is padded with zero bytes.
    /// Creates the key if it does not exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/setrange/">Valkey commands – SETRANGE</seealso>
    /// <param name="key">The key of the string to update.</param>
    /// <param name="offset">The position in the string where <paramref name="value"/> should be written.</param>
    /// <param name="value">The string to write at <paramref name="offset"/>.</param>
    /// <returns>The length of the string stored at <paramref name="key"/> after the modification.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "Hello World");
    /// var newLength = await client.SetRangeAsync("key", 6, "Valkey");  // 12
    ///
    /// var updated = await client.GetAsync("key");  // "Hello Valkey"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> SetRangeAsync(ValkeyKey key, long offset, ValkeyValue value);
}
