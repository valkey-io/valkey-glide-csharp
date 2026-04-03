// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IStringCommands
{
    /// <inheritdoc/>
    public async Task StringSetAsync(ValkeyKey key, ValkeyValue value) =>
        _ = await Command(Request.StringSet(key, value));

    /// <inheritdoc/>
    public async Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, When when) =>
        when switch
        {
            When.Always => await Command(Request.StringSet(key, value)),
            When.NotExists => await Command(Request.StringSetNX(key, value)),
            When.Exists => await Command(Request.StringSetXX(key, value)),
            _ => throw new ArgumentOutOfRangeException(nameof(when), $"{when} is not supported for StringSetAsync.")
        };

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetAsync(ValkeyKey key) =>
        await Command(Request.StringGet(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys) =>
        await Command(Request.StringGetMultiple([.. keys]));

    /// <inheritdoc/>
    public async Task StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) =>
        _ = await Command(Request.StringSetMultiple([.. values]));

    /// <inheritdoc/>
    public async Task<bool> StringSetNXAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values)
        => await Command(Request.StringSetMultipleNX([.. values]));

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end) =>
        await Command(Request.StringGetRange(key, start, end));

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value) =>
        await Command(Request.StringSetRange(key, offset, value));

    /// <inheritdoc/>
    public async Task<long> StringLengthAsync(ValkeyKey key) =>
        await Command(Request.StringLength(key));

    /// <inheritdoc/>
    public async Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value) =>
        await Command(Request.StringAppend(key, value));

    /// <inheritdoc/>
    public async Task<long> StringDecrementAsync(ValkeyKey key) =>
        await Command(Request.StringDecr(key));

    /// <inheritdoc/>
    public async Task<long> StringDecrementAsync(ValkeyKey key, long decrement) =>
        await Command(Request.StringDecrBy(key, decrement));

    /// <inheritdoc/>
    public async Task<long> StringIncrementAsync(ValkeyKey key) =>
        await Command(Request.StringIncr(key));

    /// <inheritdoc/>
    public async Task<long> StringIncrementAsync(ValkeyKey key, long increment) =>
        await Command(Request.StringIncrBy(key, increment));

    /// <inheritdoc/>
    public async Task<double> StringIncrementAsync(ValkeyKey key, double increment) =>
        await Command(Request.StringIncrByFloat(key, increment));

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key) =>
        await Command(Request.StringGetDelete(key));

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry) =>
        await Command(Request.StringGetSetExpiry(key, expiry));

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry) =>
        await Command(Request.StringGetSetExpiry(key, expiry));

    /// <inheritdoc/>
    public async Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second)
    {
        ValkeyValue result = await Command(Request.StringLongestCommonSubsequence(first, second));
        return result.IsNull ? null : result.ToString();
    }

    /// <inheritdoc/>
    public async Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second) =>
        await Command(Request.StringLongestCommonSubsequenceLength(first, second));

    /// <inheritdoc/>
    public async Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0) =>
        await Command(Request.StringLongestCommonSubsequenceWithMatches(first, second, minLength));
}
