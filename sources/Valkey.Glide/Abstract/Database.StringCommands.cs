// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await StringSetAsync(key, value);
        return true;
    }

    // TODO #262: Update to delegate to StringSetAsync(values) and StringSetNXAsync(values).
    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When, CommandFlags)"/>
    public Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringSetAsync(values, when);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringGetAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringGetAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetRangeAsync(ValkeyKey, long, long, CommandFlags)"/>
    public Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringGetRangeAsync(key, start, end);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetRangeAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>
    public Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringSetRangeAsync(key, offset, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringAppendAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringAppendAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringDecrementAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringDecrementAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringDecrementAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringDecrementAsync(key, decrement);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringIncrementAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringIncrementAsync(key, increment);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, double, CommandFlags)"/>
    public Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringIncrementAsync(key, increment);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetDeleteAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringGetDeleteAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?, CommandFlags)"/>
    public Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringGetSetExpiryAsync(key, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetExpiryAsync(ValkeyKey, DateTime, CommandFlags)"/>
    public Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringGetSetExpiryAsync(key, expiry);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringLongestCommonSubsequenceAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringLongestCommonSubsequenceLengthAsync(first, second);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long, CommandFlags)"/>
    public Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return StringLongestCommonSubsequenceWithMatchesAsync(first, second, minLength);
    }
}
