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
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetRangeAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLengthAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringAppendAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringDecrementAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringDecrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetDeleteAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags);

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength, CommandFlags flags);
}
