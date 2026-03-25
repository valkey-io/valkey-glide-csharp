// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IStringCommands
{
    public async Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value) =>
        await Command(Request.StringSet(key, value));

    public async Task<ValkeyValue> StringGetAsync(ValkeyKey key) =>
        await Command(Request.StringGet(key));

    public async Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys) =>
        await Command(Request.StringGetMultiple([.. keys]));

    public async Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when = When.Always) =>
        when switch
        {
            When.Always => await Command(Request.StringSetMultiple([.. values])),
            When.Exists => throw new NotImplementedException($"{when} is not supported for StringSetAsync."),
            When.NotExists => await Command(Request.StringSetMultipleNX([.. values])),
            _ => throw new ArgumentOutOfRangeException(nameof(when), $"{when} is not supported for StringSetAsync.")
        };

    public async Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end) =>
        await Command(Request.StringGetRange(key, start, end));

    public async Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value) =>
        await Command(Request.StringSetRange(key, offset, value));

    public async Task<long> StringLengthAsync(ValkeyKey key) =>
        await Command(Request.StringLength(key));

    public async Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value) =>
        await Command(Request.StringAppend(key, value));

    public async Task<long> StringDecrementAsync(ValkeyKey key) =>
        await Command(Request.StringDecr(key));

    public async Task<long> StringDecrementAsync(ValkeyKey key, long decrement) =>
        await Command(Request.StringDecrBy(key, decrement));

    public async Task<long> StringIncrementAsync(ValkeyKey key) =>
        await Command(Request.StringIncr(key));

    public async Task<long> StringIncrementAsync(ValkeyKey key, long increment) =>
        await Command(Request.StringIncrBy(key, increment));

    public async Task<double> StringIncrementAsync(ValkeyKey key, double increment) =>
        await Command(Request.StringIncrByFloat(key, increment));

    public async Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key) =>
        await Command(Request.StringGetDelete(key));

    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry) =>
        await Command(Request.StringGetSetExpiry(key, expiry));

    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry) =>
        await Command(Request.StringGetSetExpiry(key, expiry));

    public async Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second)
    {
        ValkeyValue result = await Command(Request.StringLongestCommonSubsequence(first, second));
        return result.IsNull ? null : result.ToString();
    }

    public async Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second) =>
        await Command(Request.StringLongestCommonSubsequenceLength(first, second));

    public async Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0) =>
        await Command(Request.StringLongestCommonSubsequenceWithMatches(first, second, minLength));
}
