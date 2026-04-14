// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;

namespace Valkey.Glide;

/// ATTENTION: Methods should only be added to this interface if they are implemented
/// by StackExchange.Redis databases but NOT by Valkey GLIDE clients. Methods implemented
/// by both should be added to <see cref="IStringBaseCommands"/> instead.

public partial interface IDatabaseAsync
{
    /// <inheritdoc cref="IStringBaseCommands.StringSetAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    // TODO #262: Update to delegate to StringSetAsync(values) and StringSetNXAsync(values).
    /// <inheritdoc cref="IStringBaseCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when = When.Always, CommandFlags flags = CommandFlags.None);

    /// <inheritdoc cref="IStringBaseCommands.StringGetAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringGetAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringGetRangeAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringLengthAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringAppendAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringDecrementAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringDecrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringIncrementAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringIncrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringIncrementAsync(ValkeyKey, double)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringGetDeleteAsync(ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IStringBaseCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)"/>
    /// <param name="flags">Command flags (currently not supported by GLIDE).</param>
    /// <exception cref="NotImplementedException">Thrown if <paramref name="flags"/> is not <see cref="CommandFlags.None"/>.</exception>
    Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0, CommandFlags flags = CommandFlags.None);
}
