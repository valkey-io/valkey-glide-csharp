// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.Pipeline;

/// <summary>
/// Supports commands for the "String Commands" group for batch requests.
/// </summary>
internal interface IBatchStringCommands
{
    /// <inheritdoc cref="IBaseClient.GetAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GetAsync(ValkeyKey)" /></returns>
    IBatch Get(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.GetAsync(IEnumerable{ValkeyKey})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GetAsync(IEnumerable{ValkeyKey})" /></returns>
    IBatch Get(IEnumerable<ValkeyKey> keys);

    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch Set(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="IBaseClient.SetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" /></returns>
    IBatch Set(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <inheritdoc cref="IBaseClient.SetIfNotExistsAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetIfNotExistsAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" /></returns>
    IBatch SetIfNotExists(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values);

    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetCondition)" /></returns>
    IBatch Set(ValkeyKey key, ValkeyValue value, SetCondition condition);

    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetOptions)" /></returns>
    IBatch Set(ValkeyKey key, ValkeyValue value, SetOptions options);

    /// <inheritdoc cref="IBaseClient.SetExpiryAsync(ValkeyKey, ValkeyValue, SetExpiryOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetExpiryAsync(ValkeyKey, ValkeyValue, SetExpiryOptions)" /></returns>
    IBatch SetExpiry(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry);

    /// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch GetSet(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue, SetCondition)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue, SetCondition)" /></returns>
    IBatch GetSet(ValkeyKey key, ValkeyValue value, SetCondition condition);

    /// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue, SetOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue, SetOptions)" /></returns>
    IBatch GetSet(ValkeyKey key, ValkeyValue value, SetOptions options);

    /// <inheritdoc cref="IBaseClient.GetSetExpiryAsync(ValkeyKey, ValkeyValue, SetExpiryOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GetSetExpiryAsync(ValkeyKey, ValkeyValue, SetExpiryOptions)" /></returns>
    IBatch GetSetExpiry(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry);

    /// <inheritdoc cref="Commands.IStringBaseCommands.StringGetRangeAsync(ValkeyKey, long, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="Commands.IStringBaseCommands.StringGetRangeAsync(ValkeyKey, long, long)" /></returns>
    IBatch StringGetRange(ValkeyKey key, long start, long end);

    /// <inheritdoc cref="IBaseClient.SetRangeAsync(ValkeyKey, long, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.SetRangeAsync(ValkeyKey, long, ValkeyValue)" /></returns>
    IBatch SetRange(ValkeyKey key, long offset, ValkeyValue value);

    /// <inheritdoc cref="IBaseClient.LengthAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.LengthAsync(ValkeyKey)" /></returns>
    IBatch Length(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.AppendAsync(ValkeyKey, ValkeyValue)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.AppendAsync(ValkeyKey, ValkeyValue)" /></returns>
    IBatch Append(ValkeyKey key, ValkeyValue value);

    /// <inheritdoc cref="IBaseClient.DecrementAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.DecrementAsync(ValkeyKey, long)" /></returns>
    IBatch Decrement(ValkeyKey key, long value = 1);

    /// <inheritdoc cref="IBaseClient.DecrementAsync(ValkeyKey, double)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.DecrementAsync(ValkeyKey, double)" /></returns>
    IBatch Decrement(ValkeyKey key, double value);

    /// <inheritdoc cref="IBaseClient.IncrementAsync(ValkeyKey, long)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.IncrementAsync(ValkeyKey, long)" /></returns>
    IBatch Increment(ValkeyKey key, long value = 1);

    /// <inheritdoc cref="IBaseClient.IncrementAsync(ValkeyKey, double)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.IncrementAsync(ValkeyKey, double)" /></returns>
    IBatch Increment(ValkeyKey key, double value);

    /// <inheritdoc cref="IBaseClient.GetDeleteAsync(ValkeyKey)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GetDeleteAsync(ValkeyKey)" /></returns>
    IBatch GetDelete(ValkeyKey key);

    /// <inheritdoc cref="IBaseClient.GetExpiryAsync(ValkeyKey, GetExpiryOptions)" path="/*[not(self::remarks) and not(self::returns)]" />
    /// <returns>Command Response - <inheritdoc cref="IBaseClient.GetExpiryAsync(ValkeyKey, GetExpiryOptions)" /></returns>
    IBatch GetExpiry(ValkeyKey key, GetExpiryOptions options);

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
