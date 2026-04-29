// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    #region Public Methods

    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        await SetAsync(key, value);
        return true;
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
    public Task<ValkeyValue> StringSetAndGetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetSetAsync(key, value, ToSetOptions(when, expiry, keepTtl));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetAndGetAsync(ValkeyKey, ValkeyValue, TimeSpan?, When, CommandFlags)"/>
    public Task<ValkeyValue> StringSetAndGetAsync(ValkeyKey key, ValkeyValue value, TimeSpan? expiry, When when, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetSetAsync(key, value, ToSetOptions(when, expiry, keepTtl: false));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When, CommandFlags)"/>
    public async Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        switch (when)
        {
            case When.Always:
                await SetAsync(values);
                return true;
            case When.NotExists:
                return await SetIfNotExistsAsync(values);
            case When.Exists:
                throw new ArgumentException(when + " is not valid in this context; the permitted values are: Always, NotExists");
            default:
                throw new NotSupportedException($"Enum value {when} is not supported by Valkey GLIDE");
        }
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<ValkeyValue> StringGetSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetSetAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetAsync(IEnumerable{ValkeyKey}, CommandFlags)"/>
    public Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetAsync(keys);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetRangeAsync(ValkeyKey, long, long, CommandFlags)"/>
    public Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetRangeAsync(key, start, end);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringSetRangeAsync(ValkeyKey, long, ValkeyValue, CommandFlags)"/>
    public Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return SetRangeAsync(key, offset, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return LengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringAppendAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return AppendAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringDecrementAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<long> StringDecrementAsync(ValkeyKey key, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return DecrementAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringDecrementAsync(ValkeyKey, double, CommandFlags)"/>
    public Task<double> StringDecrementAsync(ValkeyKey key, double value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return DecrementAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<long> StringIncrementAsync(ValkeyKey key, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return IncrementAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringIncrementAsync(ValkeyKey, double, CommandFlags)"/>
    public Task<double> StringIncrementAsync(ValkeyKey key, double value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return IncrementAsync(key, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetDeleteAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetDeleteAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?, CommandFlags)"/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = expiry.HasValue ? GetExpiryOptions.ExpireIn(expiry.Value) : GetExpiryOptions.Persist();
        return await GetExpiryAsync(key, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.StringGetSetExpiryAsync(ValkeyKey, DateTime, CommandFlags)"/>
    public Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return GetExpiryAsync(key, GetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.LongestCommonSubsequence(first, second));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey, CommandFlags)"/>
    public Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.LongestCommonSubsequenceLength(first, second));
    }

    /// <inheritdoc cref="IDatabaseAsync.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long, CommandFlags)"/>
    public Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return Command(Request.LongestCommonSubsequenceWithMatches(first, second, minLength));
    }

    #endregion
    #region Private Methods

    private static SetOptions ToSetOptions(When when, TimeSpan? expiry, bool keepTtl)
        => new() { Condition = ToSetCondition(when), Expiry = ToSetExpiryOptions(expiry, keepTtl) };

    private static SetCondition ToSetCondition(When when) => when switch
    {
        When.Always => SetCondition.Always,
        When.NotExists => SetCondition.OnlyIfDoesNotExist,
        When.Exists => SetCondition.OnlyIfExists,
        _ => throw new ArgumentException($"{when} is not valid in this context; the permitted values are: Always, NotExists, Exists"),
    };

    private static SetExpiryOptions? ToSetExpiryOptions(TimeSpan? expiry, bool keepTtl)
    {
        if (expiry.HasValue && keepTtl)
        {
            throw new ArgumentException("Cannot specify both expiry and keepTtl=true.");
        }

        if (expiry.HasValue)
        {
            return SetExpiryOptions.ExpireIn(expiry.Value);
        }

        return keepTtl ? SetExpiryOptions.KeepTimeToLive() : null;
    }

    #endregion
}
