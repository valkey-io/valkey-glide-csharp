// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.HashGetAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashField);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashFields);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashGetAllAsync(ValkeyKey, CommandFlags)"/>
    public async Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAllAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashSetAsync(ValkeyKey, IEnumerable{HashEntry}, CommandFlags)"/>
    public async Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await HashSetAsync(key, hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));
    }

    /// <inheritdoc cref="IDatabaseAsync.HashSetAsync(ValkeyKey, ValkeyValue, ValkeyValue, When, CommandFlags)"/>
    public async Task<bool> HashSetAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, When when, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return when switch
        {
            When.Always => await HashSetAsync(key, hashField, value),
            When.NotExists => await HashSetIfNotExistsAsync(key, hashField, value),
            When.Exists => throw new ArgumentException(when + " is not valid in this context; the permitted values are: Always, NotExists"),
            _ => throw new NotSupportedException($"When {when} is not supported by Valkey GLIDE"),
        };
    }

    /// <inheritdoc cref="IDatabaseAsync.HashDeleteAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashDeleteAsync(key, hashField);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashDeleteAsync(key, hashFields);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashExistsAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExistsAsync(key, hashField);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashIncrementAsync(ValkeyKey, ValkeyValue, long, CommandFlags)"/>
    public async Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashIncrementByAsync(key, hashField, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)"/>
    public async Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashIncrementByAsync(key, hashField, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashKeysAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashKeysAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashLengthAsync(ValkeyKey, CommandFlags)"/>
    public async Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashStringLengthAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public async Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashStringLengthAsync(key, hashField);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashValuesAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashValuesAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashRandomFieldAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashRandomFieldsAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashRandomFieldsWithValuesAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsWithValuesAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashGetExAsync(ValkeyKey, IEnumerable{ValkeyValue}, HashGetExOptions, CommandFlags)"/>
    public async Task<ValkeyValue[]?> HashGetExAsync(ValkeyKey key, IEnumerable<ValkeyValue> fields, HashGetExOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetExAsync(key, fields, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashSetExAsync(ValkeyKey, IDictionary{ValkeyValue, ValkeyValue}, HashSetExOptions, CommandFlags)"/>
    public async Task<long> HashSetExAsync(ValkeyKey key, IDictionary<ValkeyValue, ValkeyValue> fieldValueMap, HashSetExOptions options, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashSetExAsync(key, fieldValueMap, options);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldExpireAsync(ValkeyKey, IEnumerable{ValkeyValue}, TimeSpan, ExpireWhen, CommandFlags)"/>
    public async Task<ExpireResult[]> HashFieldExpireAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        TimeSpan expiry,
        ExpireWhen when = ExpireWhen.Always,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireAsync(key, hashFields, expiry, MapExpireWhen(when));
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldExpireAsync(ValkeyKey, IEnumerable{ValkeyValue}, DateTime, ExpireWhen, CommandFlags)"/>
    public async Task<ExpireResult[]> HashFieldExpireAsync(
        ValkeyKey key,
        IEnumerable<ValkeyValue> hashFields,
        DateTime expiry,
        ExpireWhen when = ExpireWhen.Always,
        CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireAtAsync(key, hashFields, new DateTimeOffset(expiry), MapExpireWhen(when));
    }

    /// <summary>
    /// Maps the given StackExchange.Redis <see cref="ExpireWhen"/> to the corresponding Valkey GLIDE <see cref="ExpireCondition"/>.
    /// </summary>
    private static ExpireCondition MapExpireWhen(ExpireWhen when) => when switch
    {
        ExpireWhen.Always => ExpireCondition.Always,
        ExpireWhen.HasExpiry => ExpireCondition.OnlyIfExists,
        ExpireWhen.HasNoExpiry => ExpireCondition.OnlyIfNotExists,
        ExpireWhen.GreaterThanCurrentExpiry => ExpireCondition.OnlyIfGreaterThan,
        ExpireWhen.LessThanCurrentExpiry => ExpireCondition.OnlyIfLessThan,
        _ => throw new ArgumentOutOfRangeException(nameof(when)),
    };

}
