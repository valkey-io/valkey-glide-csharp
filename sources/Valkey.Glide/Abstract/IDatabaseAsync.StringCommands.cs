// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Commands.Options;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IStringBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::returns)]"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> always (throws on failure).</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetOptions)" path="/summary"/>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiry to set. <see langword="null"/> means no expiry.</param>
    /// <param name="keepTtl">Whether to retain the existing TTL.</param>
    /// <param name="when">The condition under which the key should be set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the key was set, <see langword="false"/> if the condition was not met.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="StringSetAsync(ValkeyKey, ValkeyValue, TimeSpan?, bool, When, CommandFlags)" path="/summary"/>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiry to set. <see langword="null"/> means no expiry.</param>
    /// <param name="when">The condition under which the key should be set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if the key was set, <see langword="false"/> if the condition was not met.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry, When when, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="StringSetAsync(ValkeyKey, ValkeyValue, TimeSpan?, bool, When, CommandFlags)" path="/summary"/>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiry to set. <see langword="null"/> means no expiry.</param>
    /// <param name="when">The condition under which the key should be set.</param>
    /// <returns><see langword="true"/> if the key was set, <see langword="false"/> if the condition was not met.</returns>
    Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry, When when);

    /// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue, SetOptions)" path="/summary"/>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiry to set. <see langword="null"/> means no expiry.</param>
    /// <param name="keepTtl">Whether to retain the existing TTL.</param>
    /// <param name="when">The condition under which the key should be set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The old value stored at key, or <see cref="ValkeyValue.Null"/> when key did not exist.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringSetAndGetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="StringSetAndGetAsync(ValkeyKey, ValkeyValue, TimeSpan?, bool, When, CommandFlags)" path="/summary"/>
    /// <param name="key">The key to store.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="expiry">The expiry to set. <see langword="null"/> means no expiry.</param>
    /// <param name="when">The condition under which the key should be set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The old value stored at key, or <see cref="ValkeyValue.Null"/> when key did not exist.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringSetAndGetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry, When when, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" path="/*[not(self::returns)]"/>
    /// <param name="when">The condition under which the keys should be set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if all keys were set; <see langword="false"/> if <paramref name="when"/> is <see cref="When.NotExists"/> and at least one key already existed.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GetAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GetAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GetRangeAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.SetRangeAsync(ValkeyKey, long, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.LengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.AppendAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.DecrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringDecrementAsync(ValkeyKey key, long value = 1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.DecrementAsync(ValkeyKey, double)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> StringDecrementAsync(ValkeyKey key, double value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.IncrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringIncrementAsync(ValkeyKey key, long value = 1, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.IncrementAsync(ValkeyKey, double)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> StringIncrementAsync(ValkeyKey key, double value, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GetDeleteAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the value of a key and optionally sets or removes its expiry.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getex/">Valkey commands – GETEX</seealso>
    /// <param name="key">The key to retrieve.</param>
    /// <param name="expiry">The expiry to set. <see langword="null"/> removes the existing expiry.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The value of <paramref name="key"/>, or <see cref="ValkeyValue.Null"/> when <paramref name="key"/> does not exist.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await db.StringSetAsync("key", "value");
    /// var value = await db.StringGetSetExpiryAsync("key", TimeSpan.FromSeconds(30));  // "value"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Gets the value of a key and sets its expiry to the given <see cref="DateTime"/>.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/getex/">Valkey commands – GETEX</seealso>
    /// <param name="key">The key to retrieve.</param>
    /// <param name="expiry">The absolute expiry time to set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The value of <paramref name="key"/>, or <see cref="ValkeyValue.Null"/> when <paramref name="key"/> does not exist.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await db.StringSetAsync("key", "value");
    /// var value = await db.StringGetSetExpiryAsync("key", DateTime.UtcNow.AddMinutes(5));  // "value"
    /// </code>
    /// </example>
    /// </remarks>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the longest common subsequence between the values at <paramref name="first"/> and <paramref name="second"/>,
    /// returning a string containing the common sequence.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lcs/">Valkey commands – LCS</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>A string (sequence of characters) of the LCS match.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await db.StringSetAsync("key1", "ohmytext");
    /// await db.StringSetAsync("key2", "mynewtext");
    /// var lcs = await db.StringLongestCommonSubsequenceAsync("key1", "key2");  // "mytext"
    /// </code>
    /// </example>
    /// </remarks>
    Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the longest common subsequence between the values at <paramref name="first"/> and <paramref name="second"/>,
    /// returning the length of the common sequence.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lcs/">Valkey commands – LCS</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The length of the LCS match.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await db.StringSetAsync("key1", "ohmytext");
    /// await db.StringSetAsync("key2", "mynewtext");
    /// var length = await db.StringLongestCommonSubsequenceLengthAsync("key1", "key2");  // 6
    /// </code>
    /// </example>
    /// </remarks>
    Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the longest common subsequence between the values at <paramref name="first"/> and <paramref name="second"/>,
    /// returning a list of all common sequences with their positions and match information.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lcs/">Valkey commands – LCS</seealso>
    /// <note>Since Valkey 7.0.0.</note>
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <param name="minLength">Can be used to restrict the list of matches to the ones of a given minimum length.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The result of LCS algorithm, containing match positions and lengths based on the given parameters.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    /// <remarks>
    /// <example>
    /// <code>
    /// await db.StringSetAsync("key1", "ohmytext");
    /// await db.StringSetAsync("key2", "mynewtext");
    /// var matches = await db.StringLongestCommonSubsequenceWithMatchesAsync("key1", "key2", minLength: 4);
    /// Console.WriteLine($"LCS length: {matches.LongestMatchLength}");
    /// </code>
    /// </example>
    /// </remarks>
    Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0, CommandFlags flags = CommandFlags.None);
}
