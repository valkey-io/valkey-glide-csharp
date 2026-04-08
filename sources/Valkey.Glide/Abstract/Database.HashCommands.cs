// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;
using Valkey.Glide.Internals;

namespace Valkey.Glide;

internal partial class Database
{
    /// <inheritdoc/>
    public async Task<ValkeyValue> HashGetAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashField);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashGetAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAsync(key, hashFields);
    }

    /// <inheritdoc/>
    public async Task<HashEntry[]> HashGetAllAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetAllAsync(key);
    }

    /// <inheritdoc/>
    public async Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await HashSetAsync(key, hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<bool> HashDeleteAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashDeleteAsync(key, hashField);
    }

    /// <inheritdoc/>
    public async Task<long> HashDeleteAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashDeleteAsync(key, hashFields);
    }

    /// <inheritdoc/>
    public async Task<bool> HashExistsAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExistsAsync(key, hashField);
    }

    /// <inheritdoc/>
    public async Task<long> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashIncrementByAsync(key, hashField, value);
    }

    /// <inheritdoc/>
    public async Task<double> HashIncrementAsync(ValkeyKey key, ValkeyValue hashField, double value, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashIncrementByAsync(key, hashField, value);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashKeysAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashKeysAsync(key);
    }

    /// <inheritdoc/>
    public async Task<long> HashLengthAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashLengthAsync(key);
    }

    /// <inheritdoc/>
    public async Task<long> HashStringLengthAsync(ValkeyKey key, ValkeyValue hashField, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashStringLengthAsync(key, hashField);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashValuesAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashValuesAsync(key);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashRandomFieldAsync(ValkeyKey key, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldAsync(key);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashRandomFieldsAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsAsync(key, count);
    }

    /// <inheritdoc/>
    public async Task<HashEntry[]> HashRandomFieldsWithValuesAsync(ValkeyKey key, long count, CommandFlags flags)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashRandomFieldsWithValuesAsync(key, count);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldGetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, TimeSpan? expiry = null, bool persist = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GetExpiryOptions options = MapExpiryAndPersist(expiry, persist);
        return await HashGetExpiryAsync(key, hashField, options);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldGetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetExpiryAsync(key, hashField, GetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashFieldGetAndSetExpiryAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan? expiry = null, bool persist = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        GetExpiryOptions options = MapExpiryAndPersist(expiry, persist);
        return await HashGetExpiryAsync(key, hashFields, options);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashFieldGetAndSetExpiryAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashGetExpiryAsync(key, hashFields, GetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)));
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        SetExpiryOptions options = ResolveSetExpiryOptions(expiry, keepTtl);
        ValkeyValue previous = await HashGetExpiryAsync(key, hashField, GetExpiryOptions.Persist());
        _ = await HashSetExpiryAsync(key, hashField, value, options, MapWhen(when));
        return previous;
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, DateTime expiry, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        ValkeyValue previous = await HashGetExpiryAsync(key, hashField, GetExpiryOptions.Persist());
        _ = await HashSetExpiryAsync(key, hashField, value, SetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)), MapWhen(when));
        return previous;
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        SetExpiryOptions options = ResolveSetExpiryOptions(expiry, keepTtl);
        _ = await HashSetExpiryAsync(key, hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)), options, MapWhen(when));
        return ValkeyValue.Null;
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, DateTime expiry, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        _ = await HashSetExpiryAsync(key, hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)), SetExpiryOptions.ExpireAt(new DateTimeOffset(expiry)), MapWhen(when));
        return ValkeyValue.Null;
    }

    /// <inheritdoc/>
    public async Task<ExpireResult[]> HashFieldExpireAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireAsync(key, hashFields, expiry, MapExpireWhen(when));
    }

    /// <inheritdoc/>
    public async Task<ExpireResult[]> HashFieldExpireAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTime expiry, ExpireWhen when = ExpireWhen.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        return await HashExpireAtAsync(key, hashFields, new DateTimeOffset(expiry), MapExpireWhen(when));
    }

    /// <inheritdoc/>
    public async Task<long[]> HashFieldGetExpireDateTimeAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        ExpireTimeResult[] results = await HashExpireTimeAsync(key, hashFields);
        return [.. results.Select(r => r.ExpireTimeMs)];
    }

    /// <inheritdoc/>
    public async Task<long[]> HashFieldGetTimeToLiveAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        TimeToLiveResult[] results = await HashTimeToLiveAsync(key, hashFields);
        return [.. results.Select(r => r.TimeToLiveMs)];
    }

    /// <inheritdoc/>
    public async Task<PersistResult[]> HashFieldPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        HashPersistResult[] results = await HashPersistAsync(key, hashFields);
        return [.. results.Select(r => (PersistResult)(int)r)];
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

    /// <summary>
    /// Maps the given when argument to the corresponding <see cref="HashSetCondition"/>.
    /// </summary>
    private static HashSetCondition MapWhen(When when) => when switch
    {
        When.Always => HashSetCondition.Always,
        When.NotExists => HashSetCondition.OnlyIfNoneExist,
        When.Exists => HashSetCondition.OnlyIfAllExist,
        _ => throw new ArgumentOutOfRangeException(nameof(when)),
    };

    /// <summary>
    /// Maps the given expiry and persist arguments to the corresponding <see cref="GetExpiryOptions"/>.
    /// </summary>
    private static GetExpiryOptions MapExpiryAndPersist(TimeSpan? expiry, bool persist)
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

    /// <summary>
    /// Maps the given expiry and keepTtl arguments to the corresponding <see cref="SetExpiryOptions"/>.
    /// </summary>
    private static SetExpiryOptions ResolveSetExpiryOptions(TimeSpan? expiry, bool keepTtl)
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
}
