// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "String Commands" group for batch requests.
/// </summary>
internal interface IBatchStringCommands
{
    /// <inheritdoc cref="Commands.IStringBaseCommands.StringGetAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringGetAsync(ValkeyKey)" /></returns>
    IBatch StringGet(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringGetAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringGetAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch StringGet(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringSetAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringSetAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StringSet(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}}, When)" /></returns>
    IBatch StringSet(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringGetRangeAsync(ValkeyKey, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringGetRangeAsync(ValkeyKey, long, long)" /></returns>
    IBatch StringGetRange(ValkeyKey key, long start, long end);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)" /></returns>
    IBatch StringSetRange(ValkeyKey key, long offset, ValkeyValue value);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringLengthAsync(ValkeyKey)" /></returns>
    IBatch StringLength(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringAppendAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringAppendAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StringAppend(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringDecrementAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringDecrementAsync(ValkeyKey)" /></returns>
    IBatch StringDecrement(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringDecrementAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringDecrementAsync(ValkeyKey, long)" /></returns>
    IBatch StringDecrement(ValkeyKey key, long decrement);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringIncrementAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringIncrementAsync(ValkeyKey)" /></returns>
    IBatch StringIncrement(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringIncrementAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringIncrementAsync(ValkeyKey, long)" /></returns>
    IBatch StringIncrement(ValkeyKey key, long increment);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringIncrementAsync(ValkeyKey, double)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringIncrementAsync(ValkeyKey, double)" /></returns>
    IBatch StringIncrement(ValkeyKey key, double increment);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringGetDeleteAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringGetDeleteAsync(ValkeyKey)" /></returns>
    IBatch StringGetDelete(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)" /></returns>
    IBatch StringGetSetExpiry(ValkeyKey key, TimeSpan? expiry);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)" /></returns>
    IBatch StringGetSetExpiry(ValkeyKey key, DateTime expiry);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch StringLongestCommonSubsequence(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch StringLongestCommonSubsequenceLength(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)" /></returns>
    IBatch StringLongestCommonSubsequenceWithMatches(ValkeyKey first, ValkeyKey second, long minLength = 0);
}
