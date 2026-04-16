// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

public partial interface IBaseClient
{
    /// <summary>
    /// Get the value of key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/get/">valkey.io</seealso>
    /// <param name="key">The key to retrieve from the database.</param>
    /// <returns>
    /// If key exists, returns the value of key as a <see cref="ValkeyValue" />.<br/>
    /// Otherwise, returns <see cref="ValkeyValue.Null" />.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue value = await client.GetAsync("key");
    /// Console.WriteLine(value.ToString()); // Output: "value" or null if key doesn't exist
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetAsync(ValkeyKey key);

    /// <summary>
    /// Returns the values of all specified keys.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/mget/">valkey.io</seealso>
    /// <note> In cluster mode, if keys in <paramref name="keys"/> map to different hash slots, the command
    /// will be split across these slots and executed separately for each. This means the command
    /// is atomic only at the slot level. If one or more slot-specific requests fail, the entire
    /// call will return the first encountered error, even though some requests may have succeeded
    /// while others did not. If this behavior impacts your application logic, consider splitting
    /// the request into sub-requests per slot to ensure atomicity.</note>
    /// <param name="keys">A list of keys to retrieve values for.</param>
    /// <returns>
    /// An array of values corresponding to the provided keys.<br/>
    /// If a key is not found, its corresponding value in the list will be <see cref="ValkeyValue.Null" />.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// ValkeyValue[] values = await client.GetAsync(["key1", "key2", "key3"]);
    /// Console.WriteLine(values[0].ToString()); // Output: value of key1 or null
    /// Console.WriteLine(values[1].ToString()); // Output: value of key2 or null
    /// Console.WriteLine(values[2].ToString()); // Output: value of key3 or null
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue[]> GetAsync(IEnumerable<ValkeyKey> keys);

    /// <summary>
    /// Appends a value to the string stored at key. If the key does not exist, it is created and set to an empty string before performing the operation.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/append/">valkey.io</seealso>
    /// <param name="key">The key of the string to append to.</param>
    /// <param name="value">The value to append to the string.</param>
    /// <returns>
    /// The length of the string after the append operation.<br/>
    /// If key does not exist, it is treated as an empty string, and the command returns the length of the appended value.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.AppendAsync("key", " World");
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> AppendAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Decrements the number stored at key by the specified value. If the key does not exist, it is set to 0 before performing the operation.
    /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
    /// This operation is limited to 64 bit signed integers.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/decr/">valkey.io</seealso>
    /// <seealso href="https://valkey.io/commands/decrby/">valkey.io</seealso>
    /// <param name="key">The key of the string to decrement.</param>
    /// <param name="value">The amount to decrement by. Defaults to 1.</param>
    /// <returns>The value of key after the decrement.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "10");
    /// long newValue = await client.DecrementAsync("key");
    /// Console.WriteLine(newValue); // Output: 9
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> DecrementAsync(ValkeyKey key, long value = 1);

    /// <summary>
    /// Decrements the string representing a floating point number stored at key by the specified value.
    /// If the key does not exist, it is set to 0 before performing the operation.
    /// Implemented via INCRBYFLOAT with a negated value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/incrbyfloat/">valkey.io</seealso>
    /// <param name="key">The key of the string to decrement.</param>
    /// <param name="value">The amount to decrement by.</param>
    /// <returns>The value of key after the decrement as a double precision floating point number.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "10.5");
    /// double newValue = await client.DecrementAsync("key", 0.5);
    /// Console.WriteLine(newValue); // Output: 10.0
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> DecrementAsync(ValkeyKey key, double value);

    /// <summary>
    /// Increments the number stored at key by the specified value. If the key does not exist, it is set to 0 before performing the operation.
    /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as integer.
    /// This operation is limited to 64 bit signed integers.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/incr/">valkey.io</seealso>
    /// <seealso href="https://valkey.io/commands/incrby/">valkey.io</seealso>
    /// <param name="key">The key of the string to increment.</param>
    /// <param name="value">The amount to increment by. Defaults to 1.</param>
    /// <returns>The value of key after the increment.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "10");
    /// long newValue = await client.IncrementAsync("key");
    /// Console.WriteLine(newValue); // Output: 11
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> IncrementAsync(ValkeyKey key, long value = 1);

    /// <summary>
    /// Increments the string representing a floating point number stored at key by the specified value.
    /// If the key does not exist, it is set to 0 before performing the operation.
    /// An error is returned if the key contains a value of the wrong type or contains a string that is not representable as a floating point number.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/incrbyfloat/">valkey.io</seealso>
    /// <param name="key">The key of the string to increment.</param>
    /// <param name="value">The amount to increment by.</param>
    /// <returns>The value of key after the increment as a double precision floating point number.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "10.5");
    /// double newValue = await client.IncrementAsync("key", 0.5);
    /// Console.WriteLine(newValue); // Output: 11.0
    /// </code>
    /// </example>
    /// </remarks>
    Task<double> IncrementAsync(ValkeyKey key, double value);

    /// <summary>
    /// Get the value of key and delete the key. If the key does not exist the special value <see cref="ValkeyValue.Null"/> is returned.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getdel/">valkey.io</seealso>
    /// <param name="key">The key to get and delete.</param>
    /// <returns>The value of key, or <see cref="ValkeyValue.Null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "value");
    /// var response = await client.GetDeleteAsync("key");
    /// Console.WriteLine(response); // Output: "value"
    /// </code>
    /// </example>
    /// <example>
    /// <code>
    /// var response = await client.GetDeleteAsync("nonexistent");
    /// Console.WriteLine(response.IsNull); // Output: true (key does not exist)
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetDeleteAsync(ValkeyKey key);

    /// <summary>
    /// Gets the string value associated with the key and optionally sets or removes its expiry.
    /// If the key does not exist, the result will be <see cref="ValkeyValue.Null"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getex/">valkey.io</seealso>
    /// <note>Since Valkey 6.2.0 and above.</note>
    /// <param name="key">The key to be retrieved from the database.</param>
    /// <param name="options">The expiry option to apply (expire in duration, expire at timestamp, or persist).</param>
    /// <returns>The value of key, or <see cref="ValkeyValue.Null"/> when key does not exist.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "value");
    /// var response = await client.GetExpiryAsync("key", GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)));
    /// Console.WriteLine(response); // Output: "value"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> GetExpiryAsync(ValkeyKey key, GetExpiryOptions options);

    /// <summary>
    /// Sets the value of a key to a string.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">valkey.io</seealso>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given key.</param>
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
    /// This always succeeds or throws; there is no failure return value.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/mset/">valkey.io</seealso>
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
    /// KeyValuePair&lt;ValkeyKey, ValkeyValue&gt;[] values = [
    ///     new("key1", "value1"),
    ///     new("key2", "value2")
    /// ];
    /// await client.SetAsync(values);
    /// </code>
    /// </example>
    /// </remarks>
    Task SetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <summary>
    /// Sets multiple keys to multiple values only if none of the specified keys exist.
    /// This is an atomic operation — either all keys are set, or none are.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/msetnx/">valkey.io</seealso>
    /// <param name="values">A collection of key-value pairs to set.</param>
    /// <returns><see langword="true"/> if all keys were set, <see langword="false"/> if no key was set (at least one key already existed).</returns>
    Task<bool> SetIfNotExistsAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <summary>
    /// Sets the value of a key based on a specified condition.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">valkey.io</seealso>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given key.</param>
    /// <param name="condition">The condition under which the key should be set.</param>
    /// <returns><see langword="true"/> if the key was set, <see langword="false"/> if the condition was not met.</returns>
    Task<bool> SetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition);

    /// <summary>
    /// Sets the value of a key with options including condition and expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">valkey.io</seealso>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given key.</param>
    /// <param name="options">The options for the SET command, including condition and expiry.</param>
    /// <returns><see langword="true"/> if the key was set, <see langword="false"/> if the condition was not met.</returns>
    Task<bool> SetAsync(ValkeyKey key, ValkeyValue value, SetOptions options);

    /// <summary>
    /// Sets the value of a key with an expiry duration or timestamp.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">valkey.io</seealso>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store with the given key.</param>
    /// <param name="expiry">The expiry configuration for the key.</param>
    Task SetExpiryAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry);

    /// <summary>
    /// Gets the old value stored at key and sets it to a new value. If the key does not exist, <see cref="ValkeyValue.Null"/> is returned.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getset/">valkey.io</seealso>
    /// <param name="key">The key to get and set.</param>
    /// <param name="value">The new value to store.</param>
    /// <returns>The old value stored at key, or <see cref="ValkeyValue.Null"/> when key did not exist.</returns>
    Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value);

    /// <summary>
    /// Gets the old value stored at key and sets it to a new value with a condition.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">valkey.io</seealso>
    /// <param name="key">The key to get and set.</param>
    /// <param name="value">The new value to store.</param>
    /// <param name="condition">The condition under which the key should be set.</param>
    /// <returns>The old value stored at key, or <see cref="ValkeyValue.Null"/> when key did not exist or condition was not met.</returns>
    Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition);

    /// <summary>
    /// Gets the old value stored at key and sets it to a new value with options including condition and expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">valkey.io</seealso>
    /// <param name="key">The key to get and set.</param>
    /// <param name="value">The new value to store.</param>
    /// <param name="options">The options for the SET command, including condition and expiry.</param>
    /// <returns>The old value stored at key, or <see cref="ValkeyValue.Null"/> when key did not exist or condition was not met.</returns>
    Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value, SetOptions options);

    /// <summary>
    /// Gets the old value stored at key and sets it to a new value with an expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/set/">valkey.io</seealso>
    /// <param name="key">The key to get and set.</param>
    /// <param name="value">The new value to store.</param>
    /// <param name="expiry">The expiry configuration for the key.</param>
    /// <returns>The old value stored at key, or <see cref="ValkeyValue.Null"/> when key did not exist.</returns>
    Task<ValkeyValue> GetSetExpiryAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry);

    /// <summary>
    /// Returns the length of the string value stored at key.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/strlen/">valkey.io</seealso>
    /// <param name="key">The key to check its length.</param>
    /// <returns>
    /// The length of the string value stored at key.<br/>
    /// If key does not exist, it is treated as an empty string, and the command returns <c>0</c>.
    /// </returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "Hello World");
    /// long length = await client.LengthAsync("key");
    /// Console.WriteLine(length); // Output: 11
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> LengthAsync(ValkeyKey key);

    /// <summary>
    /// Overwrites part of the string stored at key, starting at the specified offset,
    /// for the entire length of value.
    /// If the offset is larger than the current length of the string at key, the string is padded with zero bytes to make
    /// offset fit.
    /// Creates the key if it doesn't exist.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/setrange/">valkey.io</seealso>
    /// <param name="key">The key of the string to update.</param>
    /// <param name="offset">The position in the string where value should be written.</param>
    /// <param name="value">The string written with offset.</param>
    /// <returns>The length of the string stored at key after it was modified.</returns>
    /// <remarks>
    /// <example>
    /// <code>
    /// await client.SetAsync("key", "Hello World");
    /// ValkeyValue newLength = await client.SetRangeAsync("key", 6, "Valkey");
    /// Console.WriteLine(newLength); // Output: 12
    ///
    /// ValkeyValue value = await client.GetAsync("key");
    /// Console.WriteLine(value.ToString()); // Output: "Hello Valkey"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> SetRangeAsync(ValkeyKey key, long offset, ValkeyValue value);
}
