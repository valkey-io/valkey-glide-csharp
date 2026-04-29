// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

public abstract partial class BaseClient
{
    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue)"/>
    public Task SetAsync(ValkeyKey key, ValkeyValue value)
        => Command(Request.Set(key, value, new SetOptions()));

    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetCondition)"/>
    public Task<bool> SetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition) =>
        Command(Request.Set(key, value, new SetOptions { Condition = condition }));

    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetOptions)"/>
    public Task<bool> SetAsync(ValkeyKey key, ValkeyValue value, SetOptions options) =>
        Command(Request.Set(key, value, options));

    /// <inheritdoc cref="IBaseClient.SetAsync(ValkeyKey, ValkeyValue, SetExpiryOptions)"/>
    public Task SetAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry) =>
        Command(Request.Set(key, value, new SetOptions { Expiry = expiry }));

    /// <inheritdoc cref="IBaseClient.GetAsync(ValkeyKey)"/>
    public Task<ValkeyValue> GetAsync(ValkeyKey key) =>
        Command(Request.Get(key));

    /// <inheritdoc cref="IBaseClient.GetAsync(IEnumerable{ValkeyKey})"/>
    public Task<ValkeyValue[]> GetAsync(IEnumerable<ValkeyKey> keys) =>
        Command(Request.Get(keys));

    /// <inheritdoc cref="IBaseClient.SetAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})"/>
    public Task SetAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) =>
        Command(Request.Set([.. values]));

    /// <inheritdoc cref="IBaseClient.SetIfNotExistsAsync(IEnumerable{KeyValuePair{ValkeyKey, ValkeyValue}})"/>
    public Task<bool> SetIfNotExistsAsync(IEnumerable<KeyValuePair<ValkeyKey, ValkeyValue>> values) =>
        Command(Request.SetIfNotExists([.. values]));

    /// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue)"/>
    public Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value) =>
        Command(Request.GetSet(key, value, new SetOptions()));

    /// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue, SetCondition)"/>
    public Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value, SetCondition condition) =>
        Command(Request.GetSet(key, value, new SetOptions { Condition = condition }));

    /// <inheritdoc cref="IBaseClient.GetSetAsync(ValkeyKey, ValkeyValue, SetOptions)"/>
    public Task<ValkeyValue> GetSetAsync(ValkeyKey key, ValkeyValue value, SetOptions options) =>
        Command(Request.GetSet(key, value, options));

    /// <inheritdoc cref="IBaseClient.GetSetExpiryAsync(ValkeyKey, ValkeyValue, SetExpiryOptions)"/>
    public Task<ValkeyValue> GetSetExpiryAsync(ValkeyKey key, ValkeyValue value, SetExpiryOptions expiry) =>
        Command(Request.GetSet(key, value, new SetOptions { Expiry = expiry }));

    /// <inheritdoc cref="IBaseClient.GetRangeAsync(ValkeyKey, long, long)"/>
    public Task<ValkeyValue> GetRangeAsync(ValkeyKey key, long start, long end) =>
        Command(Request.GetRange(key, start, end));

    /// <inheritdoc cref="IBaseClient.SetRangeAsync(ValkeyKey, long, ValkeyValue)"/>
    public Task<ValkeyValue> SetRangeAsync(ValkeyKey key, long offset, ValkeyValue value) =>
        Command(Request.SetRange(key, offset, value));

    /// <inheritdoc cref="IBaseClient.LengthAsync(ValkeyKey)"/>
    public Task<long> LengthAsync(ValkeyKey key) =>
        Command(Request.Length(key));

    /// <inheritdoc cref="IBaseClient.AppendAsync(ValkeyKey, ValkeyValue)"/>
    public Task<long> AppendAsync(ValkeyKey key, ValkeyValue value) =>
        Command(Request.Append(key, value));

    /// <inheritdoc cref="IBaseClient.DecrementAsync(ValkeyKey, long)"/>
    public Task<long> DecrementAsync(ValkeyKey key, long value = 1) =>
        value == 1
            ? Command(Request.Decrement(key))
            : Command(Request.DecrementBy(key, value));

    /// <inheritdoc cref="IBaseClient.DecrementAsync(ValkeyKey, double)"/>
    public Task<double> DecrementAsync(ValkeyKey key, double value) =>
        Command(Request.IncrementByFloat(key, -value));

    /// <inheritdoc cref="IBaseClient.IncrementAsync(ValkeyKey, long)"/>
    public Task<long> IncrementAsync(ValkeyKey key, long value = 1) =>
        value == 1
            ? Command(Request.Increment(key))
            : Command(Request.IncrementBy(key, value));

    /// <inheritdoc cref="IBaseClient.IncrementAsync(ValkeyKey, double)"/>
    public Task<double> IncrementAsync(ValkeyKey key, double value) =>
        Command(Request.IncrementByFloat(key, value));

    /// <inheritdoc cref="IBaseClient.GetDeleteAsync(ValkeyKey)"/>
    public Task<ValkeyValue> GetDeleteAsync(ValkeyKey key) =>
        Command(Request.GetDelete(key));

    /// <inheritdoc cref="IBaseClient.GetExpiryAsync(ValkeyKey, GetExpiryOptions)"/>
    public Task<ValkeyValue> GetExpiryAsync(ValkeyKey key, GetExpiryOptions options) =>
        Command(Request.GetExpiry(key, options));

}
