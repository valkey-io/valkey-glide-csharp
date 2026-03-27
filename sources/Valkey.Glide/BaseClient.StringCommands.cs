// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient : IStringCommands
{
    /// <inheritdoc/>
    public async Task<bool> StringSetAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringSet(key, value));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringGet(key));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> StringGetAsync(IEnumerable<ValkeyKey> keys, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringGetMultiple([.. keys]));
    }

    /// <inheritdoc/>
    public async Task<bool> StringSetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return when switch
        {
            When.Always => await Command(Request.StringSetMultiple([.. values])),
            When.Exists => throw new NotImplementedException($"{when} is not supported for StringSetAsync."),
            When.NotExists => await Command(Request.StringSetMultipleNX([.. values])),
            _ => throw new ArgumentOutOfRangeException(nameof(when), $"{when} is not supported for StringSetAsync.")
        };
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetRangeAsync(ValkeyKey key, long start, long end, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringGetRange(key, start, end));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringSetRangeAsync(ValkeyKey key, long offset, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringSetRange(key, offset, value));
    }

    /// <inheritdoc/>
    public async Task<long> StringLengthAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringLength(key));
    }

    /// <inheritdoc/>
    public async Task<long> StringAppendAsync(ValkeyKey key, ValkeyValue value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringAppend(key, value));
    }

    /// <inheritdoc/>
    public async Task<long> StringDecrementAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringDecr(key));
    }

    /// <inheritdoc/>
    public async Task<long> StringDecrementAsync(ValkeyKey key, long decrement, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringDecrBy(key, decrement));
    }

    /// <inheritdoc/>
    public async Task<long> StringIncrementAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringIncr(key));
    }

    /// <inheritdoc/>
    public async Task<long> StringIncrementAsync(ValkeyKey key, long increment, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringIncrBy(key, increment));
    }

    /// <inheritdoc/>
    public async Task<double> StringIncrementAsync(ValkeyKey key, double increment, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringIncrByFloat(key, increment));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetDeleteAsync(ValkeyKey key, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringGetDelete(key));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringGetSetExpiry(key, expiry));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> StringGetSetExpiryAsync(ValkeyKey key, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringGetSetExpiry(key, expiry));
    }

    /// <inheritdoc/>
    public async Task<string?> StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);

        ValkeyValue result = await Command(Request.StringLongestCommonSubsequence(first, second));
        return result.IsNull ? null : result.ToString();
    }

    /// <inheritdoc/>
    public async Task<long> StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringLongestCommonSubsequenceLength(first, second));
    }

    /// <inheritdoc/>
    public async Task<LCSMatchResult> StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await Command(Request.StringLongestCommonSubsequenceWithMatches(first, second, minLength));
    }
}
