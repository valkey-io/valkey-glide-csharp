// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region String Commands with CommandFlags (SER Compatibility)

    public async Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetAsync(key, value);
    }

    public async Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetAsync(values, when);
    }

    public async Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetAsync(key);
    }

    public async Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetAsync(keys);
    }

    public async Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetRangeAsync(key, start, end);
    }

    public async Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringSetRangeAsync(key, offset, value);
    }

    public async Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLengthAsync(key);
    }

    public async Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringAppendAsync(key, value);
    }

    public async Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringDecrementAsync(key);
    }

    public async Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringDecrementAsync(key, decrement);
    }

    public async Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key);
    }

    public async Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key, increment);
    }

    public async Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringIncrementAsync(key, increment);
    }

    public async Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetDeleteAsync(key);
    }

    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetSetExpiryAsync(key, expiry);
    }

    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringGetSetExpiryAsync(key, expiry);
    }

    public async Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceAsync(first, second);
    }

    public async Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceLengthAsync(first, second);
    }

    public async Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await StringLongestCommonSubsequenceWithMatchesAsync(first, second, minLength);
    }

    #endregion
}
