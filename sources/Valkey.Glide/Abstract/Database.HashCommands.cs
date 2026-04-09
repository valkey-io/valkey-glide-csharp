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
        var dict = await HashGetAllAsync(key);
        return [.. dict.Select(kvp => new HashEntry(kvp.Key, kvp.Value))];
    }

    /// <inheritdoc/>
    public async Task HashSetAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var pairs = hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value));
        _ = await HashSetAsync(key, pairs);
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
        return [.. await HashKeysAsync(key)];
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
        return [.. await HashValuesAsync(key)];
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
        var pairs = await HashRandomFieldsWithValuesAsync(key, count);
        return [.. pairs.Select(kvp => new HashEntry(kvp.Key, kvp.Value))];
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldGetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, TimeSpan? expiry = null, bool persist = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = MapGetExpiryOptions(expiry, persist);
        return await HashGetExpiryAsync(key, hashField, options);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldGetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = GetExpiryOptions.ExpireAt(new DateTimeOffset(expiry));
        return await HashGetExpiryAsync(key, hashField, options);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashFieldGetAndSetExpiryAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, TimeSpan? expiry = null, bool persist = false, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = MapGetExpiryOptions(expiry, persist);
        return await HashGetExpiryAsync(key, hashFields, options);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue[]> HashFieldGetAndSetExpiryAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, DateTime expiry, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = GetExpiryOptions.ExpireAt(new DateTimeOffset(expiry));
        return await HashGetExpiryAsync(key, hashFields, options);
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = MapSetExpiryOptions(expiry, keepTtl);
        return await HashSetExpiryAsync(key, hashField, value, options, MapWhen(when)) ? 1 : 0;
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, ValkeyValue hashField, ValkeyValue value, DateTime expiry, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var options = SetExpiryOptions.ExpireAt(new DateTimeOffset(expiry));
        return await HashSetExpiryAsync(key, hashField, value, options, MapWhen(when)) ? 1 : 0;
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, TimeSpan? expiry = null, bool keepTtl = false, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var entries = hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value));
        var options = MapSetExpiryOptions(expiry, keepTtl);
        return await HashSetExpiryAsync(key, entries, options, MapWhen(when)) ? 1 : 0;
    }

    /// <inheritdoc/>
    public async Task<ValkeyValue> HashFieldSetAndSetExpiryAsync(ValkeyKey key, IEnumerable<HashEntry> hashFields, DateTime expiry, When when = When.Always, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var entries = hashFields.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value));
        var options = SetExpiryOptions.ExpireAt(new DateTimeOffset(expiry));
        return await HashSetExpiryAsync(key, entries, options, MapWhen(when)) ? 1 : 0;
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
        var results = await HashExpireTimeAsync(key, hashFields);
        return [.. results.Select(r => r.ExpireTimeMs)];
    }

    /// <inheritdoc/>
    public async Task<long[]> HashFieldGetTimeToLiveAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await HashTimeToLiveAsync(key, hashFields);
        return [.. results.Select(r => r.TimeToLiveMs)];
    }

    /// <inheritdoc/>
    public async Task<PersistResult[]> HashFieldPersistAsync(ValkeyKey key, IEnumerable<ValkeyValue> hashFields, CommandFlags flags = CommandFlags.None)
    {
        GuardClauses.ThrowIfCommandFlags(flags);
        var results = await HashPersistAsync(key, hashFields);
        return [.. results.Select(r => (PersistResult)(int)r)];
    }

    /// <summary>
    /// Maps the given <see cref="ExpireWhen"/> to the corresponding <see cref="ExpireCondition"/>.
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
    /// Maps the given <see cref="When"/> argument to the corresponding <see cref="HashSetCondition"/>.
    /// </summary>
    private static HashSetCondition MapWhen(When when) => when switch
    {
        When.Always => HashSetCondition.Always,
        When.NotExists => HashSetCondition.OnlyIfNoneExist,
        When.Exists => HashSetCondition.OnlyIfAllExist,
        _ => throw new ArgumentOutOfRangeException(nameof(when)),
    };

    /// <summary>
    /// Maps the given <see cref="TimeSpan"/> expiry and <see langword="bool"/> persist arguments
    /// to the corresponding <see cref="GetExpiryOptions"/>.
    /// </summary>
    private static GetExpiryOptions MapGetExpiryOptions(TimeSpan? expiry, bool persist)
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
    /// Maps the given <see cref="TimeSpan"/> expiry and <see langword="bool"/> keepTtl arguments
    /// to the corresponding <see cref="GetExpiryOptions"/>.
    /// </summary>
    private static SetExpiryOptions MapSetExpiryOptions(TimeSpan? expiry, bool keepTtl)
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
