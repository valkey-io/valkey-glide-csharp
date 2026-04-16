// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc/>
    public Task SetAsync(ValkeyKey key, ValkeyValue value)
        => Command(Request.Set(key, value, new SetOptions()));

    /// <inheritdoc/>
    public Task<bool> SetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition) =>
        Command(Request.Set(key, value, new SetOptions { Condition = condition }));

    /// <inheritdoc/>
    public Task<bool> SetAsync(ValkeyKey key, ValkeyValue value, SetOptions options) =>
        Command(Request.Set(key, value, options));

    /// <inheritdoc/>
    public Task SetAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry) =>
        Command(Request.Set(key, value, new SetOptions { Expiry = expiry }));

    /// <inheritdoc/>
    public Task<ValkeyValue> GetAsync(ValkeyKey key) =>
        Command(Request.Get(key));

    /// <inheritdoc/>
    public Task<ValkeyValue[]> GetAsync(IEnumerable<ValkeyKey> keys) =>
        Command(Request.Get(keys));

    /// <inheritdoc/>
    public Task SetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) =>
        Command(Request.Set([.. values]));

    /// <inheritdoc/>
    public Task<bool> SetIfNotExistsAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) =>
        Command(Request.SetIfNotExists([.. values]));

    /// <inheritdoc/>
    public Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value) =>
        Command(Request.GetSet(key, value, new SetOptions()));

    /// <inheritdoc/>
    public Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition) =>
        Command(Request.GetSet(key, value, new SetOptions { Condition = condition }));

    /// <inheritdoc/>
    public Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value, SetOptions options) =>
        Command(Request.GetSet(key, value, options));

    /// <inheritdoc/>
    public Task<ValkeyValue> GetSetExpiryAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry) =>
        Command(Request.GetSet(key, value, new SetOptions { Expiry = expiry }));

    /// <inheritdoc/>
    public Task<ValkeyValue> GetRangeAsync(ValkeyKey key, long start, long end) =>
        Command(Request.GetRange(key, start, end));

    /// <inheritdoc/>
    public Task<ValkeyValue> SetRangeAsync(ValkeyKey key, long offset, ValkeyValue value) =>
        Command(Request.SetRange(key, offset, value));

    /// <inheritdoc/>
    public Task<long> LengthAsync(ValkeyKey key) =>
        Command(Request.Length(key));

    /// <inheritdoc/>
    public Task<long> AppendAsync(ValkeyKey key, ValkeyValue value) =>
        Command(Request.Append(key, value));

    /// <inheritdoc/>
    public Task<long> DecrementAsync(ValkeyKey key, long value = 1) =>
        value == 1
            ? Command(Request.Decrement(key))
            : Command(Request.DecrementBy(key, value));

    /// <inheritdoc/>
    public Task<double> DecrementAsync(ValkeyKey key, double value) =>
        Command(Request.IncrementByFloat(key, -value));

    /// <inheritdoc/>
    public Task<long> IncrementAsync(ValkeyKey key, long value = 1) =>
        value == 1
            ? Command(Request.Increment(key))
            : Command(Request.IncrementBy(key, value));

    /// <inheritdoc/>
    public Task<double> IncrementAsync(ValkeyKey key, double value) =>
        Command(Request.IncrementByFloat(key, value));

    /// <inheritdoc/>
    public Task<ValkeyValue> GetDeleteAsync(ValkeyKey key) =>
        Command(Request.GetDelete(key));

    /// <inheritdoc/>
    public Task<ValkeyValue> GetExpiryAsync(ValkeyKey key, GetExpiryOptions options) =>
        Command(Request.GetExpiry(key, options));

    /// <inheritdoc/>
    public async Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second)
    {
        ValkeyValue result = await Command(Request.LongestCommonSubsequence(first, second));
        return result.IsNull ? null : result.ToString();
    }

    /// <inheritdoc/>
    public Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second) =>
        Command(Request.LongestCommonSubsequenceLength(first, second));

    /// <inheritdoc/>
    public Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0) =>
        Command(Request.LongestCommonSubsequenceWithMatches(first, second, minLength));
}
