// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> hash commands.
/// </summary>
public class HashCommandTests(TestConfiguration config)
{
    // TODO #280: Cleanup this class
    public TestConfiguration Config { get; } = config;

    #region HashSetAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashSetAsync_WhenAlways_SetsField(IDatabaseAsync db)
    {
        string key = $"ser-hset-{Guid.NewGuid()}";

        Assert.True(await db.HashSetAsync(key, "field1", "value1", When.Always));
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        // Overwrite returns false (field already existed)
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

        // Should not overwrite
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
        await db.HashSetAsync(key, entries, CommandFlags.None);

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

        ValkeyValue result = await db.HashGetAsync(key, "field1", CommandFlags.None);
        Assert.Equal("value1", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAsync_SingleField_NonExistent_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-hget-{Guid.NewGuid()}";

        ValkeyValue result = await db.HashGetAsync(key, "nonexistent", CommandFlags.None);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAsync_MultiField_ReturnsValues(IDatabaseAsync db)
    {
        string key = $"ser-hmget-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        ValkeyValue[] results = await db.HashGetAsync(key, ["field1", "field2", "nonexistent"], CommandFlags.None);
        Assert.Equal(3, results.Length);
        Assert.Equal("value1", results[0]);
        Assert.Equal("value2", results[1]);
        Assert.True(results[2].IsNull);
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

        HashEntry[] entries = await db.HashGetAllAsync(key, CommandFlags.None);
        Assert.Equal(2, entries.Length);

        Dictionary<string, string> dict = entries.ToDictionary(e => e.Name.ToString(), e => e.Value.ToString());
        Assert.Equal("value1", dict["field1"]);
        Assert.Equal("value2", dict["field2"]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAllAsync_NonExistentKey_ReturnsEmpty(IDatabaseAsync db)
    {
        string key = $"ser-hgetall-{Guid.NewGuid()}";

        HashEntry[] entries = await db.HashGetAllAsync(key, CommandFlags.None);
        Assert.Empty(entries);
    }

    #endregion
    #region HashDeleteAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashDeleteAsync_SingleField_ReturnsTrue(IDatabaseAsync db)
    {
        string key = $"ser-hdel-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        Assert.True(await db.HashDeleteAsync(key, "field1", CommandFlags.None));
        Assert.True((await db.HashGetAsync(key, "field1", CommandFlags.None)).IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashDeleteAsync_SingleField_NonExistent_ReturnsFalse(IDatabaseAsync db)
    {
        string key = $"ser-hdel-{Guid.NewGuid()}";

        Assert.False(await db.HashDeleteAsync(key, "nonexistent", CommandFlags.None));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashDeleteAsync_MultiField_ReturnsDeletedCount(IDatabaseAsync db)
    {
        string key = $"ser-hdel-multi-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        long deleted = await db.HashDeleteAsync(key, ["field1", "field2", "nonexistent"], CommandFlags.None);
        Assert.Equal(2, deleted);
    }

    #endregion
    #region HashExistsAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashExistsAsync_ExistingField_ReturnsTrue(IDatabaseAsync db)
    {
        string key = $"ser-hexists-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        Assert.True(await db.HashExistsAsync(key, "field1", CommandFlags.None));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashExistsAsync_NonExistentField_ReturnsFalse(IDatabaseAsync db)
    {
        string key = $"ser-hexists-{Guid.NewGuid()}";

        Assert.False(await db.HashExistsAsync(key, "nonexistent", CommandFlags.None));
    }

    #endregion
    #region HashKeysAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashKeysAsync_ReturnsAllKeys(IDatabaseAsync db)
    {
        string key = $"ser-hkeys-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        ValkeyValue[] keys = await db.HashKeysAsync(key, CommandFlags.None);
        Assert.Equal(2, keys.Length);
        Assert.Contains("field1", keys.Select(k => k.ToString()));
        Assert.Contains("field2", keys.Select(k => k.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashKeysAsync_NonExistentKey_ReturnsEmpty(IDatabaseAsync db)
    {
        string key = $"ser-hkeys-{Guid.NewGuid()}";

        ValkeyValue[] keys = await db.HashKeysAsync(key, CommandFlags.None);
        Assert.Empty(keys);
    }

    #endregion
    #region HashLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashLengthAsync_ReturnsFieldCount(IDatabaseAsync db)
    {
        string key = $"ser-hlen-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        Assert.Equal(2, await db.HashLengthAsync(key, CommandFlags.None));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashLengthAsync_NonExistentKey_ReturnsZero(IDatabaseAsync db)
    {
        string key = $"ser-hlen-{Guid.NewGuid()}";

        Assert.Equal(0, await db.HashLengthAsync(key, CommandFlags.None));
    }

    #endregion
    #region HashStringLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashStringLengthAsync_ReturnsValueLength(IDatabaseAsync db)
    {
        string key = $"ser-hstrlen-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "hello", When.Always);

        Assert.Equal(5, await db.HashStringLengthAsync(key, "field1", CommandFlags.None));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashStringLengthAsync_NonExistentField_ReturnsZero(IDatabaseAsync db)
    {
        string key = $"ser-hstrlen-{Guid.NewGuid()}";

        Assert.Equal(0, await db.HashStringLengthAsync(key, "nonexistent", CommandFlags.None));
    }

    #endregion
    #region HashValuesAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashValuesAsync_ReturnsAllValues(IDatabaseAsync db)
    {
        string key = $"ser-hvals-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        ValkeyValue[] values = await db.HashValuesAsync(key, CommandFlags.None);
        Assert.Equal(2, values.Length);
        Assert.Contains("value1", values.Select(v => v.ToString()));
        Assert.Contains("value2", values.Select(v => v.ToString()));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashValuesAsync_NonExistentKey_ReturnsEmpty(IDatabaseAsync db)
    {
        string key = $"ser-hvals-{Guid.NewGuid()}";

        ValkeyValue[] values = await db.HashValuesAsync(key, CommandFlags.None);
        Assert.Empty(values);
    }

    #endregion
    #region HashFieldExpireAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldExpireAsync_TimeSpan(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hexpire-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        ExpireResult[] results = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.Success, results[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldExpireAsync_DateTime(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hexpireat-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        ExpireResult[] results = await db.HashFieldExpireAsync(key, ["field1"], DateTime.UtcNow.AddMinutes(5));
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.Success, results[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldExpireAsync_AllExpireWhenValues(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hexpire-when-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        // Always — multi-field
        ExpireResult[] results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(60), ExpireWhen.Always);
        Assert.Equal(2, results.Length);
        Assert.Equal(ExpireResult.Success, results[0]);
        Assert.Equal(ExpireResult.Success, results[1]);

        // HasNoExpiry — should fail since both fields already have expiry
        results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(120), ExpireWhen.HasNoExpiry);
        Assert.Equal(2, results.Length);
        Assert.Equal(ExpireResult.ConditionNotMet, results[0]);
        Assert.Equal(ExpireResult.ConditionNotMet, results[1]);

        // HasExpiry — should succeed since both fields have expiry
        results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(30), ExpireWhen.HasExpiry);
        Assert.Equal(2, results.Length);
        Assert.Equal(ExpireResult.Success, results[0]);
        Assert.Equal(ExpireResult.Success, results[1]);

        // GreaterThanCurrentExpiry — 120s > 30s, should succeed
        results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(120), ExpireWhen.GreaterThanCurrentExpiry);
        Assert.Equal(2, results.Length);
        Assert.Equal(ExpireResult.Success, results[0]);
        Assert.Equal(ExpireResult.Success, results[1]);

        // LessThanCurrentExpiry — 10s < 120s, should succeed
        results = await db.HashFieldExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(10), ExpireWhen.LessThanCurrentExpiry);
        Assert.Equal(2, results.Length);
        Assert.Equal(ExpireResult.Success, results[0]);
        Assert.Equal(ExpireResult.Success, results[1]);

        // Non-existent field
        results = await db.HashFieldExpireAsync(key, ["nonexistent"], TimeSpan.FromSeconds(60));
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.NoSuchField, results[0]);
    }

    #endregion
    #region HashFieldGetExpireDateTimeAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetExpireDateTimeAsync(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hexpiretime-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        long[] results = await db.HashFieldGetExpireDateTimeAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equal(3, results.Length);
        Assert.True(results[0] > 0); // field1 has expiry
        Assert.Equal(-1, results[1]); // field2 is persistent
        Assert.Equal(-2, results[2]); // nonexistent
    }

    #endregion
    #region HashFieldGetTimeToLiveAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetTimeToLiveAsync(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-httl-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        long[] results = await db.HashFieldGetTimeToLiveAsync(key, ["field1", "field2", "nonexistent"]);
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
        SkipIfHashExpireNotSupported();

        string key = $"ser-hpersist-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        PersistResult[] results = await db.HashFieldPersistAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equal(3, results.Length);
        Assert.Equal(PersistResult.Success, results[0]);
        Assert.Equal(PersistResult.ConditionNotMet, results[1]);
        Assert.Equal(PersistResult.NoSuchField, results[2]);
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

        ValkeyValue field = await db.HashRandomFieldAsync(key, CommandFlags.None);
        Assert.Contains(field.ToString(), new[] { "field1", "field2" });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashRandomFieldsAsync_ReturnsFields(IDatabaseAsync db)
    {
        string key = $"ser-hrandfields-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);
        _ = await db.HashSetAsync(key, "field3", "value3", When.Always);

        ValkeyValue[] fields = await db.HashRandomFieldsAsync(key, 2, CommandFlags.None);
        Assert.Equal(2, fields.Length);
        foreach (ValkeyValue field in fields)
        {
            Assert.Contains(field.ToString(), new[] { "field1", "field2", "field3" });
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashRandomFieldsWithValuesAsync_ReturnsEntries(IDatabaseAsync db)
    {
        string key = $"ser-hrandfieldwv-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);
        _ = await db.HashSetAsync(key, "field3", "value3", When.Always);

        // SER layer returns HashEntry[]
        HashEntry[] entries = await db.HashRandomFieldsWithValuesAsync(key, 2, CommandFlags.None);
        Assert.Equal(2, entries.Length);
        foreach (HashEntry entry in entries)
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
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-single-ts-{Guid.NewGuid()}";

        // Field does not exist yet.
        ValkeyValue result = await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60));
        Assert.Equal(1, (int)result);

        // Verify field was set
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        // Verify TTL was applied
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.True(ttls[0] is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_MultiField_WithTimeSpan(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-ts-{Guid.NewGuid()}";

        HashEntry[] fields =
        [
            new("field1", "value1"),
            new("field2", "value2"),
        ];

        _ = await db.HashFieldSetAndSetExpiryAsync(key, fields, TimeSpan.FromSeconds(60));

        // Verify fields were set
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));
        Assert.Equal("value2", await db.HashGetAsync(key, "field2"));

        // Verify TTL was applied
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1", "field2"]);
        Assert.True(ttls[0] is > 0 and <= 60000);
        Assert.True(ttls[1] is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_SingleField_WithNullTimeSpan_KeepsTtl(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-keepttl-null-{Guid.NewGuid()}";

        // Set field with an expiry first.
        Assert.Equal(1, (int)await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60)));

        // Update without changing expiry.
        Assert.Equal(1, (int)await db.HashFieldSetAndSetExpiryAsync(key, "field1", "updated", expiry: null));

        // Verify value was updated
        Assert.Equal("updated", await db.HashGetAsync(key, "field1"));

        // Verify TTL was preserved
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.True(ttls[0] is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_SingleField_WithDateTime(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-single-dt-{Guid.NewGuid()}";

        // HSETEX returns 1 when the field is newly created
        ValkeyValue result = await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", DateTime.UtcNow.AddMinutes(5));
        Assert.Equal(1, (int)result);

        // Verify field was set
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        // Verify expiry was applied
        long[] expireTimes = await db.HashFieldGetExpireDateTimeAsync(key, ["field1"]);
        Assert.True(expireTimes[0] > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_MultiField_WithDateTime(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-dt-{Guid.NewGuid()}";

        HashEntry[] fields = [new("field1", "value1")];
        _ = await db.HashFieldSetAndSetExpiryAsync(key, fields, DateTime.UtcNow.AddMinutes(5));

        // Verify field was set
        Assert.Equal("value1", await db.HashGetAsync(key, "field1"));

        // Verify expiry was applied
        long[] expireTimes = await db.HashFieldGetExpireDateTimeAsync(key, ["field1"]);
        Assert.True(expireTimes[0] > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_SingleField_WithKeepTtlTrue(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-keepttl-{Guid.NewGuid()}";

        // Set field with an expiry first
        Assert.Equal(1, (int)await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60)));

        // Update with keepTtl=true
        Assert.Equal(1, (int)await db.HashFieldSetAndSetExpiryAsync(key, "field1", "updated", keepTtl: true));

        // Verify value was updated
        Assert.Equal("updated", await db.HashGetAsync(key, "field1"));

        // Verify TTL was preserved
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.True(ttls[0] is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_MultiField_WithWhenNotExists(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-nx-{Guid.NewGuid()}";

        // Set field1 first
        _ = await db.HashSetAsync(key, "field1", "original", When.Always);

        // Try to set field1 (exists) and field2 (new) with When.NotExists
        HashEntry[] fields =
        [
            new("field1", "should-not-update"),
            new("field2", "value2"),
        ];
        _ = await db.HashFieldSetAndSetExpiryAsync(key, fields, TimeSpan.FromSeconds(60), when: When.NotExists);

        // When.NotExists maps to FNX — only sets if NONE of the specified fields exist.
        // Since field1 exists, none should be set
        Assert.Equal("original", await db.HashGetAsync(key, "field1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_MultiField_WithWhenExists(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-xx-{Guid.NewGuid()}";

        // Set field1 first
        _ = await db.HashSetAsync(key, "field1", "original", When.Always);

        // Try to set field1 (exists) and field2 (new) with When.Exists
        HashEntry[] fields =
        [
            new("field1", "updated"),
            new("field2", "value2"),
        ];
        _ = await db.HashFieldSetAndSetExpiryAsync(key, fields, TimeSpan.FromSeconds(60), when: When.Exists);

        // When.Exists maps to FXX — only sets if ALL specified fields exist.
        // Since field2 doesn't exist, none should be set
        Assert.Equal("original", await db.HashGetAsync(key, "field1"));
    }

    #endregion
    #region HashFieldGetAndSetExpiryAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_SingleField_WithTimeSpan(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hgetex-single-ts-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        ValkeyValue value = await db.HashFieldGetAndSetExpiryAsync(key, "field1", TimeSpan.FromSeconds(60));
        Assert.Equal("value1", value);

        // Verify TTL was set
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.True(ttls[0] is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_MultiField_WithTimeSpan(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hgetex-ts-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);
        _ = await db.HashSetAsync(key, "field2", "value2", When.Always);

        ValkeyValue[] values = await db.HashFieldGetAndSetExpiryAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(60));
        Assert.Equal(2, values.Length);
        Assert.Equal("value1", values[0]);
        Assert.Equal("value2", values[1]);

        // Verify TTL was set
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1", "field2"]);
        Assert.True(ttls[0] is > 0 and <= 60000);
        Assert.True(ttls[1] is > 0 and <= 60000);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_SingleField_WithNullTimeSpan_Persists(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hgetex-persist-null-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        // Set an expiry first
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        // Null TimeSpan should persist (remove expiry)
        ValkeyValue value = await db.HashFieldGetAndSetExpiryAsync(key, "field1", expiry: null);
        Assert.Equal("value1", value);

        // Verify expiry was removed
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.Equal(-1, ttls[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_SingleField_WithDateTime(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hgetex-single-dt-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        ValkeyValue value = await db.HashFieldGetAndSetExpiryAsync(key, "field1", DateTime.UtcNow.AddMinutes(5));
        Assert.Equal("value1", value);

        // Verify expiry was set
        long[] expireTimes = await db.HashFieldGetExpireDateTimeAsync(key, ["field1"]);
        Assert.True(expireTimes[0] > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_MultiField_WithDateTime(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hgetex-dt-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        ValkeyValue[] values = await db.HashFieldGetAndSetExpiryAsync(key, ["field1"], DateTime.UtcNow.AddMinutes(5));
        _ = Assert.Single(values);
        Assert.Equal("value1", values[0]);

        // Verify expiry was set
        long[] expireTimes = await db.HashFieldGetExpireDateTimeAsync(key, ["field1"]);
        Assert.True(expireTimes[0] > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_SingleField_WithPersistTrue(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hgetex-persist-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        // Set an expiry first
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        // persist=true should remove expiry
        ValkeyValue value = await db.HashFieldGetAndSetExpiryAsync(key, "field1", persist: true);
        Assert.Equal("value1", value);

        // Verify expiry was removed
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.Equal(-1, ttls[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiry_MultiField_WithPersistTrue(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hgetex-multi-persist-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        // Set an expiry first
        _ = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        // persist=true should remove expiry
        ValkeyValue[] values = await db.HashFieldGetAndSetExpiryAsync(key, ["field1"], persist: true);
        _ = Assert.Single(values);
        Assert.Equal("value1", values[0]);

        // Verify expiry was removed
        long[] ttls = await db.HashFieldGetTimeToLiveAsync(key, ["field1"]);
        Assert.Equal(-1, ttls[0]);
    }

    #endregion
    #region Helpers

    // TODO #280: Extract to TestUtils
    private static void SkipIfHashExpireNotSupported()
        => Assert.SkipWhen(TestConfiguration.IsVersionLessThan("9.0.0"), "Requires Valkey 9.0+");

    #endregion
}
