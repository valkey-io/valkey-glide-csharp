// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide.Pipeline;

public abstract partial class BaseBatch<T> where T : BaseBatch<T>
{
    /// <inheritdoc cref="IBatchStringCommands.Get(ValkeyKey)" />
    public T GetAsync(ValkeyKey key) => AddCmd(Request.Get(key));

    /// <inheritdoc cref="IBatchStringCommands.Get(IEnumerable{ValkeyKey})" />
    public T GetAsync(IEnumerable<ValkeyKey> keys) => AddCmd(Request.Get(keys));

    /// <inheritdoc cref="IBatchStringCommands.Set(ValkeyKey, ValkeyValue)" />
    public T SetAsync(ValkeyKey key, ValkeyValue value) => AddCmd(Request.Set(key, value, new SetOptions()));

    /// <inheritdoc cref="IBatchStringCommands.Set(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" />
    public T SetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) => AddCmd(Request.Set([.. values]));

    /// <inheritdoc cref="IBatchStringCommands.SetIfNotExists(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})" />
    public T SetIfNotExistsAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) => AddCmd(Request.SetIfNotExists([.. values]));

    /// <inheritdoc cref="IBatchStringCommands.Set(ValkeyKey, ValkeyValue, SetCondition)" />
    public T SetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition) => AddCmd(Request.Set(key, value, new SetOptions { Condition = condition }));

    /// <inheritdoc cref="IBatchStringCommands.Set(ValkeyKey, ValkeyValue, SetOptions)" />
    public T SetAsync(ValkeyKey key, ValkeyValue value, SetOptions options) => AddCmd(Request.Set(key, value, options));

    /// <inheritdoc cref="IBatchStringCommands.SetExpiry(ValkeyKey, ValkeyValue, SetExpiryOptions)" />
    public T SetExpiryAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry) => AddCmd(Request.Set(key, value, new SetOptions { Expiry = expiry }));

    /// <inheritdoc cref="IBatchStringCommands.GetSet(ValkeyKey, ValkeyValue)" />
    public T GetSetAsync(ValkeyKey key, ValkeyValue value) => AddCmd(Request.GetSet(key, value, new SetOptions()));

    /// <inheritdoc cref="IBatchStringCommands.GetSet(ValkeyKey, ValkeyValue, SetCondition)" />
    public T GetSetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition) => AddCmd(Request.GetSet(key, value, new SetOptions { Condition = condition }));

    /// <inheritdoc cref="IBatchStringCommands.GetSet(ValkeyKey, ValkeyValue, SetOptions)" />
    public T GetSetAsync(ValkeyKey key, ValkeyValue value, SetOptions options) => AddCmd(Request.GetSet(key, value, options));

    /// <inheritdoc cref="IBatchStringCommands.GetSetExpiry(ValkeyKey, ValkeyValue, SetExpiryOptions)" />
    public T GetSetExpiryAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry) => AddCmd(Request.GetSet(key, value, new SetOptions { Expiry = expiry }));

    /// <inheritdoc cref="IBatchStringCommands.GetRange(ValkeyKey, long, long)" />
    public T GetRangeAsync(ValkeyKey key, long start, long end) => AddCmd(Request.GetRange(key, start, end));

    /// <inheritdoc cref="IBatchStringCommands.SetRange(ValkeyKey, long, ValkeyValue)" />
    public T SetRangeAsync(ValkeyKey key, long offset, ValkeyValue value) => AddCmd(Request.SetRange(key, offset, value));

    /// <inheritdoc cref="IBatchStringCommands.Length(ValkeyKey)" />
    public T LengthAsync(ValkeyKey key) => AddCmd(Request.Length(key));

    /// <inheritdoc cref="IBatchStringCommands.Append(ValkeyKey, ValkeyValue)" />
    public T AppendAsync(ValkeyKey key, ValkeyValue value) => AddCmd(Request.Append(key, value));

    /// <inheritdoc cref="IBatchStringCommands.Decrement(ValkeyKey, long)" />
    public T DecrementAsync(ValkeyKey key, long value = 1) =>
        value == 1
            ? AddCmd(Request.Decrement(key))
            : AddCmd(Request.DecrementBy(key, value));

    /// <inheritdoc cref="IBatchStringCommands.Decrement(ValkeyKey, double)" />
    public T DecrementAsync(ValkeyKey key, double value) => AddCmd(Request.IncrementByFloat(key, -value));

    /// <inheritdoc cref="IBatchStringCommands.Increment(ValkeyKey, long)" />
    public T IncrementAsync(ValkeyKey key, long value = 1) =>
        value == 1
            ? AddCmd(Request.Increment(key))
            : AddCmd(Request.IncrementBy(key, value));

    /// <inheritdoc cref="IBatchStringCommands.Increment(ValkeyKey, double)" />
    public T IncrementAsync(ValkeyKey key, double value) => AddCmd(Request.IncrementByFloat(key, value));

    /// <inheritdoc cref="IBatchStringCommands.GetDelete(ValkeyKey)" />
    public T GetDeleteAsync(ValkeyKey key) => AddCmd(Request.GetDelete(key));

    /// <inheritdoc cref="IBatchStringCommands.GetExpiry(ValkeyKey, GetExpiryOptions)" />
    public T GetExpiryAsync(ValkeyKey key, GetExpiryOptions option) => AddCmd(Request.GetExpiry(key, option));

    /// <inheritdoc cref="IBatchStringCommands.StringLongestCommonSubsequence(ValkeyKey, ValkeyKey)" />
    public T StringLongestCommonSubsequenceAsync(ValkeyKey first, ValkeyKey second) => AddCmd(Request.LongestCommonSubsequence(first, second));

    /// <inheritdoc cref="IBatchStringCommands.StringLongestCommonSubsequenceLength(ValkeyKey, ValkeyKey)" />
    public T StringLongestCommonSubsequenceLengthAsync(ValkeyKey first, ValkeyKey second) => AddCmd(Request.LongestCommonSubsequenceLength(first, second));

    /// <inheritdoc cref="IBatchStringCommands.StringLongestCommonSubsequenceWithMatches(ValkeyKey, ValkeyKey, long)" />
    public T StringLongestCommonSubsequenceWithMatchesAsync(ValkeyKey first, ValkeyKey second, long minLength = 0) => AddCmd(Request.LongestCommonSubsequenceWithMatches(first, second, minLength));

    IBatch IBatchStringCommands.Get(ValkeyKey key) => GetAsync(key);
    IBatch IBatchStringCommands.Get(IEnumerable<ValkeyKey> keys) => GetAsync(keys);
    IBatch IBatchStringCommands.Set(ValkeyKey key, ValkeyValue value) => SetAsync(key, value);
    IBatch IBatchStringCommands.Set(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) => SetAsync(values);
    IBatch IBatchStringCommands.SetIfNotExists(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) => SetIfNotExistsAsync(values);
    IBatch IBatchStringCommands.Set(ValkeyKey key, ValkeyValue value, SetCondition condition) => SetAsync(key, value, condition);
    IBatch IBatchStringCommands.Set(ValkeyKey key, ValkeyValue value, SetOptions options) => SetAsync(key, value, options);
    IBatch IBatchStringCommands.SetExpiry(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry) => SetExpiryAsync(key, value, expiry);
    IBatch IBatchStringCommands.GetSet(ValkeyKey key, ValkeyValue value) => GetSetAsync(key, value);
    IBatch IBatchStringCommands.GetSet(ValkeyKey key, ValkeyValue value, SetCondition condition) => GetSetAsync(key, value, condition);
    IBatch IBatchStringCommands.GetSet(ValkeyKey key, ValkeyValue value, SetOptions options) => GetSetAsync(key, value, options);
    IBatch IBatchStringCommands.GetSetExpiry(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry) => GetSetExpiryAsync(key, value, expiry);
    IBatch IBatchStringCommands.GetRange(ValkeyKey key, long start, long end) => GetRangeAsync(key, start, end);
    IBatch IBatchStringCommands.SetRange(ValkeyKey key, long offset, ValkeyValue value) => SetRangeAsync(key, offset, value);
    IBatch IBatchStringCommands.Length(ValkeyKey key) => LengthAsync(key);
    IBatch IBatchStringCommands.Append(ValkeyKey key, ValkeyValue value) => AppendAsync(key, value);
    IBatch IBatchStringCommands.Decrement(ValkeyKey key, long value) => DecrementAsync(key, value);
    IBatch IBatchStringCommands.Decrement(ValkeyKey key, double value) => DecrementAsync(key, value);
    IBatch IBatchStringCommands.Increment(ValkeyKey key, long value) => IncrementAsync(key, value);
    IBatch IBatchStringCommands.Increment(ValkeyKey key, double value) => IncrementAsync(key, value);
    IBatch IBatchStringCommands.GetDelete(ValkeyKey key) => GetDeleteAsync(key);
    IBatch IBatchStringCommands.GetExpiry(ValkeyKey key, GetExpiryOptions option) => GetExpiryAsync(key, option);
    IBatch IBatchStringCommands.StringLongestCommonSubsequence(ValkeyKey first, ValkeyKey second) => StringLongestCommonSubsequenceAsync(first, second);
    IBatch IBatchStringCommands.StringLongestCommonSubsequenceLength(ValkeyKey first, ValkeyKey second) => StringLongestCommonSubsequenceLengthAsync(first, second);
    IBatch IBatchStringCommands.StringLongestCommonSubsequenceWithMatches(ValkeyKey first, ValkeyKey second, long minLength) => StringLongestCommonSubsequenceWithMatchesAsync(first, second, minLength);
}
