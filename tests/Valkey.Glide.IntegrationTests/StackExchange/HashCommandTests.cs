// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> hash commands.
/// </summary>
public class HashCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region HashSetAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashSetAsync_WhenAlways_SetsField(IDatabaseAsync db)
    {
        string key = $"ser-hset-{Guid.NewGuid()}";

        Assert.True(await db.HashSetAsync(key, "field1", "value1", When.Always));
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        Assert.False(await db.HashSetAsync(key, "field1", "updated", When.Always));
        Assert.Equal("updated", await db.HashGetAsync(key, "field1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashSetAsync_WhenNotExists_SetsOnlyNewField(IDatabaseAsync db)
    {
        string key = $"ser-hsetnx-{Guid.NewGuid()}";

        Assert.True(await db.HashSetAsync(key, "field1", "value1", When.NotExists));
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        Assert.False(await db.HashSetAsync(key, "field1", "should-not-update", When.NotExists));
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashSetAsync_WhenExists_Throws(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<ArgumentException>(
            () => db.HashSetAsync("key", "field1", "value1", When.Exists));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashSetAsync_MultiField_WhenAlways_SetsFields(IDatabaseAsync db)
    {
        string key = $"ser-hmset-{Guid.NewGuid()}";

        HashEntry[] entries =
        [
            new("field1", "value1"),
            new("field2", "value2"),
        ];
        await db.HashSetAsync(key, entries);

        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));
        Assert.Equal("value2", await db.HashGetAsync(key, "field2"));
    }

    #endregion
    #region HashIncrementAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashIncrementAsync_Long(IDatabaseAsync db)
    {
        string key = $"ser-hincr-{Guid.NewGuid()}";

        Assert.Equal(5, await db.HashIncrementAsync(key, "counter", 5));
        Assert.Equal(6, await db.HashIncrementAsync(key, "counter"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashIncrementAsync_Double(IDatabaseAsync db)
    {
        string key = $"ser-hincrf-{Guid.NewGuid()}";

        Assert.Equal(2.5, await db.HashIncrementAsync(key, "counter", 2.5));
        Assert.Equal(5.0, await db.HashIncrementAsync(key, "counter", 2.5));
    }

    #endregion
    #region HashGetAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAsync_SingleField_ReturnsValue(IDatabaseAsync db)
    {
        string key = $"ser-hget-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAsync_SingleField_NonExistent_ReturnsNull(IDatabaseAsync db)
        => Assert.True((await db.HashGetAsync(Guid.NewGuid().ToString(), "nonexistent")).IsNull);

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAsync_MultiField_ReturnsValues(IDatabaseAsync db)
    {
        string key = $"ser-hmget-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        var results = await db.HashGetAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equivalent(new ValkeyValue[] { "value1", "value2", ValkeyValue.Null }, results);
    }

    #endregion
    #region HashGetAllAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAllAsync_ReturnsAllEntries(IDatabaseAsync db)
    {
        string key = $"ser-hgetall-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        var entries = await db.HashGetAllAsync(key);
        Assert.Equivalent(new HashEntry[] { new("field1", "value1"), new("field2", "value2") }, entries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAllAsync_NonExistentKey_ReturnsEmpty(IDatabaseAsync db)
        => Assert.Empty(await db.HashGetAllAsync(Guid.NewGuid().ToString()));

    #endregion
    #region HashDeleteAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashDeleteAsync_SingleField_ReturnsTrue(IDatabaseAsync db)
    {
        string key = $"ser-hdel-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        Assert.True(await db.HashDeleteAsync(key, "field1"));
        Assert.True((await db.HashGetAsync(key, "field1")).IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashDeleteAsync_SingleField_NonExistent_ReturnsFalse(IDatabaseAsync db)
        => Assert.False(await db.HashDeleteAsync(Guid.NewGuid().ToString(), "nonexistent"));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashDeleteAsync_MultiField_ReturnsDeletedCount(IDatabaseAsync db)
    {
        string key = $"ser-hdel-multi-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        Assert.Equal(2, await db.HashDeleteAsync(key, ["field1", "field2", "nonexistent"]));
    }

    #endregion
    #region HashExistsAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashExistsAsync_ExistingField_ReturnsTrue(IDatabaseAsync db)
    {
        string key = $"ser-hexists-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        Assert.True(await db.HashExistsAsync(key, "field1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashExistsAsync_NonExistentField_ReturnsFalse(IDatabaseAsync db)
        => Assert.False(await db.HashExistsAsync($"ser-hexists-{Guid.NewGuid()}", "nonexistent"));

    #endregion
    #region HashKeysAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashKeysAsync_ReturnsAllKeys(IDatabaseAsync db)
    {
        string key = $"ser-hkeys-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        var keys = await db.HashKeysAsync(key);
        Assert.Equivalent(new[] { "field1", "field2" }, keys);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashKeysAsync_NonExistentKey_ReturnsEmpty(IDatabaseAsync db)
        => Assert.Empty(await db.HashKeysAsync(Guid.NewGuid().ToString()));

    #endregion
    #region HashLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashLengthAsync_ReturnsFieldCount(IDatabaseAsync db)
    {
        string key = $"ser-hlen-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        Assert.Equal(2, await db.HashLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashLengthAsync_NonExistentKey_ReturnsZero(IDatabaseAsync db)
        => Assert.Equal(0, await db.HashLengthAsync(Guid.NewGuid().ToString()));

    #endregion
    #region HashStringLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashStringLengthAsync_ReturnsValueLength(IDatabaseAsync db)
    {
        string key = $"ser-hstrlen-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "hello", When.Always);

        Assert.Equal(5, await db.HashStringLengthAsync(key, "field1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashStringLengthAsync_NonExistentField_ReturnsZero(IDatabaseAsync db)
        => Assert.Equal(0, await db.HashStringLengthAsync($"ser-hstrlen-{Guid.NewGuid()}", "nonexistent"));

    #endregion
    #region HashValuesAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashValuesAsync_ReturnsAllValues(IDatabaseAsync db)
    {
        string key = $"ser-hvals-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        var values = await db.HashValuesAsync(key);
        Assert.Equivalent(new[] { "value1", "value2" }, values);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashValuesAsync_NonExistentKey_ReturnsEmpty(IDatabaseAsync db)
        => Assert.Empty(await db.HashValuesAsync(Guid.NewGuid().ToString()));

    #endregion
    #region HashFieldExpireAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldExpireAsync_TimeSpan(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hexpire-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        var results = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));
        Assert.Equivalent(new[] { ExpireResult.Success }, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldExpireAsync_DateTime(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hexpireat-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        var results = await db.HashFieldExpireAsync(key, ["field1"], DateTime.UtcNow.AddMinutes(5));
        Assert.Equivalent(new[] { ExpireResult.Success }, results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldExpireAsync_AllExpireWhenValues(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hexpire-when-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        // Always — should succeed
        var results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(60), ExpireWhen.Always);
        Assert.Equivalent(new[] { ExpireResult.Success, ExpireResult.Success }, results);

        // HasNoExpiry — should fail since both fields already have expiry
        results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(120), ExpireWhen.HasNoExpiry);
        Assert.Equivalent(new[] { ExpireResult.ConditionNotMet, ExpireResult.ConditionNotMet }, results);

        // HasExpiry — should succeed since both fields have expiry
        results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(30), ExpireWhen.HasExpiry);
        Assert.Equivalent(new[] { ExpireResult.Success, ExpireResult.Success }, results);

        // GreaterThanCurrentExpiry — 120s > 30s, should succeed
        results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(120), ExpireWhen.GreaterThanCurrentExpiry);
        Assert.Equivalent(new[] { ExpireResult.Success, ExpireResult.Success }, results);

        // LessThanCurrentExpiry — 10s < 120s, should succeed
        results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(10), ExpireWhen.LessThanCurrentExpiry);
        Assert.Equivalent(new[] { ExpireResult.Success, ExpireResult.Success }, results);

        // Non-existent field
        results = await db.HashFieldExpireAsync(key, ["nonexistent"], TimeSpan.FromSeconds(60));
        Assert.Equivalent(new[] { ExpireResult.NoSuchField }, results);
    }

    #endregion
    #region HashFieldGetExpireDateTimeAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetExpireDateTimeAsync(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hexpiretime-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        var results = await db.HashFieldGetExpireDateTimeAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equal(3, results.Length);
        Assert.True(results[0] > 0);
        Assert.Equal(-1, results[1]);
        Assert.Equal(-2, results[2]);
    }

    #endregion
    #region HashFieldGetTimeToLiveAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetTimeToLiveAsync(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-httl-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        var results = await db.HashFieldGetTimeToLiveAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equal(3, results.Length);
        Assert.True(results[0] is > 0 and <= 60000);
        Assert.Equal(-1, results[1]);
        Assert.Equal(-2, results[2]);
    }

    #endregion
    #region HashFieldPersistAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldPersistAsync(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hpersist-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        var results = await db.HashFieldPersistAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equivalent(new[] { PersistResult.Success, PersistResult.ConditionNotMet, PersistResult.NoSuchField }, results);
    }

    #endregion
    #region HashRandomFieldAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashRandomFieldAsync_ReturnsField(IDatabaseAsync db)
    {
        string key = $"ser-hrandfield-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        var field = await db.HashRandomFieldAsync(key);
        Assert.Contains(field.ToString(), new[] { "field1", "field2" });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashRandomFieldsAsync_ReturnsFields(IDatabaseAsync db)
    {
        string key = $"ser-hrandfields-{Guid.NewGuid()}";
        await db.HashSetAsync(key, [new("field1", "value1"), new("field2", "value2"), new("field3", "value3")]);

        var fields = await db.HashRandomFieldsAsync(key, 2);
        Assert.Equal(2, fields.Length);
        foreach (var field in fields)
        {
            Assert.Contains(field.ToString(), new[] { "field1", "field2", "field3" });
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashRandomFieldsWithValuesAsync_ReturnsEntries(IDatabaseAsync db)
    {
        string key = $"ser-hrandfieldwv-{Guid.NewGuid()}";
        await db.HashSetAsync(key, [new("field1", "value1"), new("field2", "value2"), new("field3", "value3")]);

        var entries = await db.HashRandomFieldsWithValuesAsync(key, 2);
        Assert.Equal(2, entries.Length);
        foreach (var entry in entries)
        {
            string fieldName = entry.Name.ToString();
            Assert.Contains(fieldName, new[] { "field1", "field2", "field3" });
            Assert.Equal("value" + fieldName[5..], entry.Value.ToString());
        }
    }

    #endregion
    #region HashFieldSetAndSetExpiryAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_SingleField_WithTimeSpan(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hsetex-single-ts-{Guid.NewGuid()}";

        var result = await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60));

        Assert.Equal(1, (int)result);
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        var ttl = Assert.Single(await db.HashFieldGetTimeToLiveAsync(key, ["field1"]));
        Assert.True(ttl is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_MultiField_WithTimeSpan(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hsetex-ts-{Guid.NewGuid()}";

        HashEntry[] fields =
        [
            new("field1", "value1"),
            new("field2", "value2"),
        ];
        _ = await db.HashFieldSetAndSetExpiryAsync(key, fields, TimeSpan.FromSeconds(60));

        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));
        Assert.Equal("value2", await db.HashGetAsync(key, "field2"));

        var ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1", "field2"]);
        Assert.Equal(2, ttls.Length);
        Assert.True(ttls[0] is > 0 and <= 60000);
        Assert.True(ttls[1] is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_SingleField_WithNullTimeSpan_ClearsTtl(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hsetex-keepttl-null-{Guid.NewGuid()}";
        _ = await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60));

        Assert.Equal(1, (int)await db.HashFieldSetAndSetExpiryAsync(key, "field1", "updated", expiry: null));

        Assert.Equal("updated", await db.HashGetAsync(key, "field1"));
        Assert.Equal(-1, Assert.Single(await db.HashFieldGetTimeToLiveAsync(key, ["field1"])));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_SingleField_WithDateTime(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hsetex-single-dt-{Guid.NewGuid()}";

        var result = await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", DateTime.UtcNow.AddMinutes(5));
        Assert.Equal(1, (int)result);

        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        var expireTime = Assert.Single(await db.HashFieldGetExpireDateTimeAsync(key, ["field1"]));
        Assert.True(expireTime > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_MultiField_WithDateTime(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hsetex-dt-{Guid.NewGuid()}";
        _ = await db.HashFieldSetAndSetExpiryAsync(key, [new HashEntry("field1", "value1")], DateTime.UtcNow.AddMinutes(5));

        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        var expireTime = Assert.Single(await db.HashFieldGetExpireDateTimeAsync(key, ["field1"]));
        Assert.True(expireTime > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_SingleField_WithKeepTtlTrue(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hsetex-keepttl-{Guid.NewGuid()}";
        _ = db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60));

        Assert.Equal(1, (int)await db.HashFieldSetAndSetExpiryAsync(key, "field1", "updated", keepTtl: true));
        Assert.Equal("updated", await db.HashGetAsync(key, "field1"));

        var ttl = Assert.Single(await db.HashFieldGetTimeToLiveAsync(key, ["field1"]));
        Assert.True(ttl is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_MultiField_WithWhenNotExists(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hsetex-nx-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "original", When.Always);

        HashEntry[] fields =
        [
            new("field1", "should-not-update"),
            new("field2", "value2"),
        ];
        _ = await db.HashFieldSetAndSetExpiryAsync(key, fields, TimeSpan.FromSeconds(60), when: When.NotExists);

        Assert.Equal("original", await db.HashGetAsync(key, "field1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_MultiField_WithWhenExists(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hsetex-xx-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "original", When.Always);

        HashEntry[] fields =
        [
            new("field1", "updated"),
            new("field2", "value2"),
        ];
        _ = await db.HashFieldSetAndSetExpiryAsync(key, fields, TimeSpan.FromSeconds(60), when: When.Exists);

        Assert.Equal("original", await db.HashGetAsync(key, "field1"));
    }

    #endregion
    #region HashFieldGetAndSetExpiryAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_SingleField_WithTimeSpan(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hgetex-single-ts-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        var value = await db.HashFieldGetAndSetExpiryAsync(key, "field1", TimeSpan.FromSeconds(60));
        Assert.Equal("value1", value);

        var ttl = Assert.Single(await db.HashFieldGetTimeToLiveAsync(key, ["field1"]));
        Assert.True(ttl is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_MultiField_WithTimeSpan(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hgetex-ts-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        var values = await db.HashFieldGetAndSetExpiryAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(60));
        Assert.Equivalent(new[] { "value1", "value2" }, values);

        var ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1", "field2"]);
        Assert.Equal(2, ttls.Length);
        Assert.True(ttls[0] is > 0 and <= 60000);
        Assert.True(ttls[1] is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_SingleField_WithNullTimeSpan_Persists(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hgetex-persist-null-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        var value = await db.HashFieldGetAndSetExpiryAsync(key, "field1", expiry: null);
        Assert.Equal("value1", value);

        var ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.Equivalent(new[] { -1 }, ttls);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_SingleField_WithDateTime(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hgetex-single-dt-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        var value = await db.HashFieldGetAndSetExpiryAsync(key, "field1", DateTime.UtcNow.AddMinutes(5));
        Assert.Equal("value1", value);

        var expireTimes = await db.HashFieldGetExpireDateTimeAsync(key, ["field1"]);
        Assert.True(expireTimes[0] > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_MultiField_WithDateTime(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hgetex-dt-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        var values = await db.HashFieldGetAndSetExpiryAsync(key, ["field1"], DateTime.UtcNow.AddMinutes(5));
        Assert.Equivalent(new[] { "value1" }, values);

        var expireTimes = await db.HashFieldGetExpireDateTimeAsync(key, ["field1"]);
        Assert.True(expireTimes[0] > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_SingleField_WithPersistTrue(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hgetex-persist-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        var value = await db.HashFieldGetAndSetExpiryAsync(key, "field1", persist: true);
        Assert.Equal("value1", value);

        var ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.Equivalent(new[] { -1 }, ttls);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_MultiField_WithPersistTrue(IDatabaseAsync db)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = $"ser-hgetex-multi-persist-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        var values = await db.HashFieldGetAndSetExpiryAsync(key, ["field1"], persist: true);
        Assert.Equivalent(new[] { "value1" }, values);

        var ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.Equivalent(new[] { -1 }, ttls);
    }

    #endregion
}
