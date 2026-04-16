// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetAsync(key, value).ContinueWith(_ => true, TaskContinuationOptions.ExecuteSynchronously);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(ValkeyKey, ValkeyValue, TimeSpan?, bool, When, CommandFlags)"/>
    public Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetAsync(key, value, ToSetOptions(when, expiry, keepTtl));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(ValkeyKey, ValkeyValue, TimeSpan?, When, CommandFlags)"/>
    public Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry, When when, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetAsync(key, value, ToSetOptions(when, expiry, keepTtl: false));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(ValkeyKey, ValkeyValue, TimeSpan?, When)"/>
    public Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry, When when)
        => SetAsync(key, value, ToSetOptions(when, expiry, keepTtl: false));

    /// <inheritdoc cref="IDatabaseAsync.StringSetAndGetAsync(ValkeyKey, ValkeyValue, TimeSpan?, bool, When, CommandFlags)"/>
    public async Task<ValkeyValue> StringSetAndGetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetSetAsync(key, value, ToSetOptions(when, expiry, keepTtl));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetAndGetAsync(ValkeyKey, ValkeyValue, TimeSpan?, When, CommandFlags)"/>
    public async Task<ValkeyValue> StringSetAndGetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry, When when, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetSetAsync(key, value, ToSetOptions(when, expiry, keepTtl: false));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When, CommandFlags)"/>
    public Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return when switch
        {
            When.Always => SetAsync(values).ContinueWith(_ => true, TaskContinuationOptions.ExecuteSynchronously),
            When.NotExists => SetIfNotExistsAsync(values),
            When.Exists => throw new ArgumentException(when + " is not valid in this context; the permitted values are: Always, NotExists"),
            _ => throw new NotSupportedException($"When {when} is not supported by Valkey GLIDE"),
        };
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetSetAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public async Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetRangeAsync(ValkeyKey, long, long, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetRangeAsync(key, start, end);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetRangeAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>
    public async Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await SetRangeAsync(key, offset, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLengthAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await LengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringAppendAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await AppendAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringDecrementAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<long> StringDecrementAsync(ValkeyKey key, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await DecrementAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringDecrementAsync(ValkeyKey, double, CommandFlags)"/>
    public async Task<double> StringDecrementAsync(ValkeyKey key, double value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await DecrementAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<long> StringIncrementAsync(ValkeyKey key, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await IncrementAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, double, CommandFlags)"/>
    public async Task<double> StringIncrementAsync(ValkeyKey key, double value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await IncrementAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetDeleteAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetDeleteAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = expiry.HasValue ? GetExpiryOptions.ExpireIn(expiry.Value) : GetExpiryOptions.Persist();
        return await GetExpiryAsync(key, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetExpiryAsync(ValkeyKey, DateTime, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await GetExpiryAsync(key, GetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)));
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
        return await base.StringLongestCommonSubsequenceWithMatchesAsync(first, second, minLength);
    }

    #region Private Methods

    private static SetOptions ToSetOptions(When when, TimeSpan? expiry, bool keepTtl)
        => new() { Condition = ToSetCondition(when), Expiry = ToSetExpiryOption(expiry, keepTtl) };

    private static SetCondition ToSetCondition(When when) => when switch
    {
        When.Always => SetCondition.Always,
        When.NotExists => SetCondition.OnlyIfDoesNotExist,
        When.Exists => SetCondition.OnlyIfExists,
        _ => throw new ArgumentException($"{when} is not valid in this context; the permitted values are: Always, NotExists, Exists"),
    };

    private static SetExpiryOptions ToSetExpiryOption(TimeSpan? expiry, bool keepTtl)
    {
        if (expiry.HasValue && keepTtl)
        {
            throw new ArgumentException("Cannot specify both expiry and keepTtl=true.");
        }

        else if (expiry.HasValue)
        {
            return SetExpiryOptions.ExpireIn(expiry.Value);
        }

        else
        {
            return SetExpiryOptions.KeepTimeToLive();
        }
    }

    #endregion
}
