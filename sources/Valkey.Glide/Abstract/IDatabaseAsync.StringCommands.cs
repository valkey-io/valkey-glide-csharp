// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// <summary>
/// String commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IStringCommands" />
public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IStringCommands.StringSetAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <summary>
    /// Sets the specified keys to the given values, subject to the <paramref name="when"/> condition.
    /// </summary>
    /// <seealso href="https://valkey.io/commands/mset/">valkey.io</seealso>
    /// <seealso href="https://valkey.io/commands/msetnx/">valkey.io</seealso>
    /// <param name="values"><inheritdoc cref="IStringCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" path="/param[@name='values']"/></param>
    /// <param name="when">The condition under which the keys should be set.<see cref="When.Exists"/> is not supported.</param>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <returns><see langword="true"/> if <paramref name="when"/> is <see cref="When.Always"/>; otherwise, <see langword="true"/> if all keys were set or <see langword="false"/> if no keys were set.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="when"/> is <see cref="When.Exists"/>.</exception>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetAsync(
        IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values,
        When when = When.Always,
        CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStringCommands.StringGetAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetRangeAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringAppendAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringDecrementAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringDecrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey, double)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetDeleteAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0, CommandFlags flags = CommandFlags.None);
}
