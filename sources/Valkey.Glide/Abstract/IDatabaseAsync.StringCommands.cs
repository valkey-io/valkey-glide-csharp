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

    /// <inheritdoc cref="IBaseClient.SetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})"/>
    /// <param name="when">The condition under which the keys should be set.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
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

    /// <inheritdoc cref="IBaseClient.GetExpiryAsync(ValkeyKey, GetExpiryOptions)" path="/*[not(self::param[@name='options']) and not(self::returns)]"/>
    /// <param name="key">The key to be retrieved from the database.</param>
    /// <param name="expiry">The expiry to set. <see langword="null"/> will remove expiry.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The value of key, or <see cref="ValkeyValue.Null"/> when key does not exist.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IBaseClient.GetExpiryAsync(ValkeyKey, GetExpiryOptions)" path="/*[not(self::param[@name='options']) and not(self::returns)]"/>
    /// <param name="key">The key to be retrieved from the database.</param>
    /// <param name="expiry">The exact date and time to expire at.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The value of key, or <see cref="ValkeyValue.Null"/> when key does not exist.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
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
    Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None);

    /// <summary>
    /// Returns the longest common subsequence between the values at <paramref name="first"/> and <paramref name="second"/>,
    /// returning a list of all common sequences with their positions and match information.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/lcs/">Valkey commands – LCS</seealso>
    /// <param name="first">The key that stores the first string.</param>
    /// <param name="second">The key that stores the second string.</param>
    /// <param name="minLength">Can be used to restrict the list of matches to the ones of a given minimum length. Defaults to 0.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns>The result of LCS algorithm, containing match positions and lengths based on the given parameters.</returns>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0, CommandFlags flags = CommandFlags.None);
}
