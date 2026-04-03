// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "String Commands" group for batch requests.
/// </summary>
internal interface IBatchStringCommands
{
    /// <inheritdoc cref="Commands.IStringCommands.StringGetAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringGetAsync(ValkeyKey)" /></returns>
    IBatch StringGet(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringCommands.StringGetAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringGetAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch StringGet(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="Commands.IStringCommands.StringSetAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringSetAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StringSet(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="Commands.IStringCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringSetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" /></returns>
    IBatch StringSet(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <inheritdoc cref="Commands.IStringCommands.StringSetNXAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringSetNXAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" /></returns>
    IBatch StringSetNX(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <inheritdoc cref="Commands.IStringCommands.StringGetRangeAsync(ValkeyKey, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringGetRangeAsync(ValkeyKey, long, long)" /></returns>
    IBatch StringGetRange(ValkeyKey key, long start, long end);

    /// <inheritdoc cref="Commands.IStringCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringSetRangeAsync(ValkeyKey, long, ValkeyValue)" /></returns>
    IBatch StringSetRange(ValkeyKey key, long offset, ValkeyValue value);

    /// <inheritdoc cref="Commands.IStringCommands.StringLengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringLengthAsync(ValkeyKey)" /></returns>
    IBatch StringLength(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringCommands.StringAppendAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringAppendAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch StringAppend(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="Commands.IStringCommands.StringDecrementAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringDecrementAsync(ValkeyKey)" /></returns>
    IBatch StringDecrement(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringCommands.StringDecrementAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringDecrementAsync(ValkeyKey, long)" /></returns>
    IBatch StringDecrement(ValkeyKey key, long decrement);

    /// <inheritdoc cref="Commands.IStringCommands.StringIncrementAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringIncrementAsync(ValkeyKey)" /></returns>
    IBatch StringIncrement(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringCommands.StringIncrementAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringIncrementAsync(ValkeyKey, long)" /></returns>
    IBatch StringIncrement(ValkeyKey key, long increment);

    /// <inheritdoc cref="Commands.IStringCommands.StringIncrementAsync(ValkeyKey, double)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringIncrementAsync(ValkeyKey, double)" /></returns>
    IBatch StringIncrement(ValkeyKey key, double increment);

    /// <inheritdoc cref="Commands.IStringCommands.StringGetDeleteAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringGetDeleteAsync(ValkeyKey)" /></returns>
    IBatch StringGetDelete(ValkeyKey key);

    /// <inheritdoc cref="Commands.IStringCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringGetSetExpiryAsync(ValkeyKey, TimeSpan?)" /></returns>
    IBatch StringGetSetExpiry(ValkeyKey key, TimeSpan? expiry);

    /// <inheritdoc cref="Commands.IStringCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringGetSetExpiryAsync(ValkeyKey, DateTime)" /></returns>
    IBatch StringGetSetExpiry(ValkeyKey key, DateTime expiry);

    /// <inheritdoc cref="Commands.IStringCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringLongestCommonSubsequenceAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch StringLongestCommonSubsequence(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="Commands.IStringCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringLongestCommonSubsequenceLengthAsync(ValkeyKey, ValkeyKey)" /></returns>
    IBatch StringLongestCommonSubsequenceLength(ValkeyKey first, ValkeyKey second);

    /// <inheritdoc cref="Commands.IStringCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringCommands.StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey, ValkeyKey, long)" /></returns>
    IBatch StringLongestCommonSubsequenceWithMatches(ValkeyKey first, ValkeyKey second, long minLength = 0);
}
