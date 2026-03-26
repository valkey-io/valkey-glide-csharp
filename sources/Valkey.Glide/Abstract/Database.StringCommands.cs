// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When, CommandFlags)"/>
    public async Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetAsync(values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetRangeAsync(ValkeyKey, long, long, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetRangeAsync(key, start, end);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetRangeAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>
    public async Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetRangeAsync(key, offset, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLengthAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringAppendAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringAppendAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringDecrementAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringDecrementAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringDecrementAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringDecrementAsync(key, decrement);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key, increment);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, double, CommandFlags)"/>
    public async Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key, increment);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetDeleteAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetDeleteAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetSetExpiryAsync(key, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetExpiryAsync(ValkeyKey, DateTime, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetSetExpiryAsync(key, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public async Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceLengthAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long, CommandFlags)"/>
    public async Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceWithMatchesAsync(first, second, minLength);
    }
}
