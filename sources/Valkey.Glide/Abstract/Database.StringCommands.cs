// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

/// <summary>
/// String commands with <see cref="CommandFlags"/> for StackExchange.Redis compatibility.
/// </summary>
/// <seealso cref="IStringCommands" />
internal partial class Database
{
    /// <inheritdoc cref="IStringCommands.StringSetAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetAsync(key, value);
    }

    /// <inheritdoc cref="IStringCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetAsync(values, when);
    }

    /// <inheritdoc cref="IStringCommands.StringGetAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetAsync(key);
    }

    /// <inheritdoc cref="IStringCommands.StringGetAsync(IEnumerable{ValkeyKey})"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetAsync(keys);
    }

    /// <inheritdoc cref="IStringCommands.StringGetRangeAsync(ValkeyKey, long, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetRangeAsync(key, start, end);
    }

    /// <inheritdoc cref="IStringCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetRangeAsync(key, offset, value);
    }

    /// <inheritdoc cref="IStringCommands.StringLengthAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLengthAsync(key);
    }

    /// <inheritdoc cref="IStringCommands.StringAppendAsync(ValkeyKey, ValkeyValue)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringAppendAsync(key, value);
    }

    /// <inheritdoc cref="IStringCommands.StringDecrementAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringDecrementAsync(key);
    }

    /// <inheritdoc cref="IStringCommands.StringDecrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringDecrementAsync(key, decrement);
    }

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key);
    }

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key, increment);
    }

    /// <inheritdoc cref="IStringCommands.StringIncrementAsync(ValkeyKey, double)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key, increment);
    }

    /// <inheritdoc cref="IStringCommands.StringGetDeleteAsync(ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetDeleteAsync(key);
    }

    /// <inheritdoc cref="IStringCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetSetExpiryAsync(key, expiry);
    }

    /// <inheritdoc cref="IStringCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetSetExpiryAsync(key, expiry);
    }

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceAsync(first, second);
    }

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceLengthAsync(first, second);
    }

    /// <inheritdoc cref="IStringCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)"/>
    /// <param name="flags">The flags to use for this operation. Currently flags are ignored.</param>
    public async Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceWithMatchesAsync(first, second, minLength);
    }
}
