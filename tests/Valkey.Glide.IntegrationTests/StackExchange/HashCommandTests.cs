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
    #region HashFieldSetAndSetExpiryAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiry_SingleField_WithTimeSpan(IDatabaseAsync db)
    {
        SkipIfHashExpireNotSupported();

        string key = $"ser-hsetex-single-ts-{Guid.NewGuid()}";

        // Field does not exist yet — previous value should be null
        ValkeyValue result = await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60));
        Assert.True(result.IsNull);

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

        // Set field with an expiry first
        Assert.Equal(
            ValkeyValue.Null,
            await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60)));

        // Update with null TimeSpan — should keep existing TTL
        Assert.Equal(
            "value1",
            await db.HashFieldSetAndSetExpiryAsync(key, "field1", "updated", expiry: null));

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

        ValkeyValue result = await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", DateTime.UtcNow.AddMinutes(5));
        Assert.True(result.IsNull);

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
        Assert.Equal(
            ValkeyValue.Null,
            await db.HashFieldSetAndSetExpiryAsync(key, "field1", "value1", TimeSpan.FromSeconds(60)));

        // Update with keepTtl=true — should preserve existing TTL
        Assert.Equal(
            "value1",
            await db.HashFieldSetAndSetExpiryAsync(key, "field1", "updated", keepTtl: true));

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
