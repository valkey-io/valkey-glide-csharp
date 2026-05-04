// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc cref="IDatabaseAsync.HashGetAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashGetAsync(key, hashField);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashGetAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashGetAsync(key, hashFields);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashGetAllAsync(ValkeyKey, CommandFlags)"/>
    public async Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var dict = await HashGetAsync(key);
        return [.. dict.Select(kvp => new HashEntry(kvp.Key, kvp.Value))];
    }

    /// <inheritdoc cref="IDatabaseAsync.HashSetAsync(ValkeyKey, IEnumerable{HashEntry}, CommandFlags)"/>
    public async Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var pairs = hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value));
        _ = await HashSetAsync(key, pairs);
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
    public Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashDeleteAsync(key, hashField);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashDeleteAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashDeleteAsync(key, hashFields);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashExistsAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashExistsAsync(key, hashField);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashIncrementAsync(ValkeyKey, ValkeyValue, long, CommandFlags)"/>
    public Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashIncrementByAsync(key, hashField, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashIncrementAsync(ValkeyKey, ValkeyValue, double, CommandFlags)"/>
    public Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashIncrementByAsync(key, hashField, value);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashKeysAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return [.. await HashKeysAsync(key)];
    }

    /// <inheritdoc cref="IDatabaseAsync.HashLengthAsync(ValkeyKey, CommandFlags)"/>
    public Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashLengthAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashStringLengthAsync(ValkeyKey, ValkeyValue, CommandFlags)"/>
    public Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashStringLengthAsync(key, hashField);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashValuesAsync(ValkeyKey, CommandFlags)"/>
    public async Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return [.. await HashValuesAsync(key)];
    }

    /// <inheritdoc cref="IDatabaseAsync.HashRandomFieldAsync(ValkeyKey, CommandFlags)"/>
    public Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashRandomFieldAsync(key);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashRandomFieldsAsync(ValkeyKey, long, CommandFlags)"/>
    public Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return HashRandomFieldsAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashRandomFieldsWithValuesAsync(ValkeyKey, long, CommandFlags)"/>
    public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsWithValuesAsync(key, count);
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldGetAndSetExpiryAsync(ValkeyKey, ValkeyValue, TimeSpan?, bool, CommandFlags)"/>
    public async Task<ValkeyValue> HashFieldGetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, TimeSpan? expiry = null, bool persist = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashField, ToGetExpiryOptions(expiry, persist));
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldGetAndSetExpiryAsync(ValkeyKey, ValkeyValue, DateTime, CommandFlags)"/>
    public async Task<ValkeyValue> HashFieldGetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashField, GetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)));
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldGetAndSetExpiryAsync(ValkeyKey, IEnumerable{ValkeyValue}, TimeSpan?, bool, CommandFlags)"/>
    public async Task<ValkeyValue[]> HashFieldGetAndSetExpiryAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan? expiry = null, bool persist = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashFields, ToGetExpiryOptions(expiry, persist));
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldGetAndSetExpiryAsync(ValkeyKey, IEnumerable{ValkeyValue}, DateTime, CommandFlags)"/>
    public async Task<ValkeyValue[]> HashFieldGetAndSetExpiryAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashFields, GetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)));
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldSetAndSetExpiryAsync(ValkeyKey, ValkeyValue, ValkeyValue, TimeSpan?, bool, When, CommandFlags)"/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = ToSetExpiryOptions(expiry, keepTtl);
        var set = options is null
            ? await HashSetAsync(key, hashField, value, ToHashSetCondition(when))
            : await HashSetAsync(key, hashField, value, new HashSetOptions { Condition = ToHashSetCondition(when), Expiry = options });
        return set ? 1 : 0;
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldSetAndSetExpiryAsync(ValkeyKey, ValkeyValue, ValkeyValue, DateTime, When, CommandFlags)"/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, DateTime expiry, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashSetAsync(key, hashField, value, new HashSetOptions { Condition = ToHashSetCondition(when), Expiry = SetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)) }) ? 1 : 0;
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldSetAndSetExpiryAsync(ValkeyKey, IEnumerable{HashEntry}, TimeSpan?, bool, When, CommandFlags)"/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var entries = hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value));
        var expiryOptions = ToSetExpiryOptions(expiry, keepTtl);

        var set = expiryOptions is null
            ? await HashSetAsync(key, entries, ToHashSetCondition(when))
            : await HashSetAsync(key, entries, new HashSetOptions { Condition = ToHashSetCondition(when), Expiry = expiryOptions });
        return set ? 1 : 0;
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldSetAndSetExpiryAsync(ValkeyKey, IEnumerable{HashEntry}, DateTime, When, CommandFlags)"/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, DateTime expiry, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var entries = hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value));
        return await HashSetAsync(key, entries, new HashSetOptions { Condition = ToHashSetCondition(when), Expiry = SetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)) }) ? 1 : 0;
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldExpireAsync(ValkeyKey, IEnumerable{ValkeyValue}, TimeSpan, ExpireWhen, CommandFlags)"/>
    public async Task<ExpireResult[]> HashFieldExpireAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await HashExpireAsync(key, hashFields, expiry, ToExpireCondition(when));
        return [.. results.Select(r => (ExpireResult)(int)r)];
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldExpireAsync(ValkeyKey, IEnumerable{ValkeyValue}, DateTime, ExpireWhen, CommandFlags)"/>
    public async Task<ExpireResult[]> HashFieldExpireAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTime expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await HashExpireAtAsync(key, hashFields, new DateTimeOffset(expiry), ToExpireCondition(when));
        return [.. results.Select(r => (ExpireResult)(int)r)];
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldGetExpireDateTimeAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long[]> HashFieldGetExpireDateTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await HashExpireTimeAsync(key, hashFields);
        return [.. results.Select(r => r.ExpireTimeMs)];
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldGetTimeToLiveAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<long[]> HashFieldGetTimeToLiveAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await HashTimeToLiveAsync(key, hashFields);
        return [.. results.Select(r => r.TimeToLiveMs)];
    }

    /// <inheritdoc cref="IDatabaseAsync.HashFieldPersistAsync(ValkeyKey, IEnumerable{ValkeyValue}, CommandFlags)"/>
    public async Task<PersistResult[]> HashFieldPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await HashPersistAsync(key, hashFields);
        return [.. results.Select(r => (PersistResult)(int)r)];
    }

    #region Private Methods

    /// <summary>
    /// Converts the given <see cref="ExpireWhen"/> to <see cref="ExpireCondition"/>.
    /// </summary>
    private static ExpireCondition ToExpireCondition(ExpireWhen when) => when switch
    {
        ExpireWhen.Always => ExpireCondition.Always,
        ExpireWhen.HasExpiry => ExpireCondition.OnlyIfExists,
        ExpireWhen.HasNoExpiry => ExpireCondition.OnlyIfNotExists,
        ExpireWhen.GreaterThanCurrentExpiry => ExpireCondition.OnlyIfGreaterThan,
        ExpireWhen.LessThanCurrentExpiry => ExpireCondition.OnlyIfLessThan,
        _ => throw new ArgumentOutOfRangeException(nameof(when)),
    };

    /// <summary>
    /// Converts the given <see cref="When"/> argument to <see cref="HashSetCondition"/>.
    /// </summary>
    private static HashSetCondition ToHashSetCondition(When when) => when switch
    {
        When.Always => HashSetCondition.Always,
        When.NotExists => HashSetCondition.OnlyIfNoneExist,
        When.Exists => HashSetCondition.OnlyIfAllExist,
        _ => throw new ArgumentOutOfRangeException(nameof(when)),
    };

    /// <summary>
    /// Converts the given <see cref="TimeSpan"/> expiry and <see langword="bool"/> persist arguments to <see cref="GetExpiryOptions"/>.
    /// </summary>
    private static GetExpiryOptions ToGetExpiryOptions(TimeSpan? expiry, bool persist)
    {
        if (expiry.HasValue && persist)
        {
            throw new ArgumentException("Cannot specify both expiry and persist=true.");
        }

        else if (expiry.HasValue)
        {
            return GetExpiryOptions.ExpireIn(expiry.Value);
        }

        else
        {
            return GetExpiryOptions.Persist();
        }
    }

    #endregion
}
