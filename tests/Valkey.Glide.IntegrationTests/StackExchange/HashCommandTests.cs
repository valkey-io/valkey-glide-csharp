// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// SER-compat layer tests for hash commands.
/// </summary>
public class SERHashCommandTests(TestConfiguration config)
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
    {
        string key = $"ser-hset-exists-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<ArgumentException>(
            () => db.HashSetAsync(key, "field1", "value1", When.Exists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashSetAsync_MultiField_SetsEntries(IDatabaseAsync db)
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

        Assert.Equal(2.5, await db.HashIncrementAsync(key, "counter", 2.5, CommandFlags.None));
        Assert.Equal(5.0, await db.HashIncrementAsync(key, "counter", 2.5, CommandFlags.None));
    }

    #endregion

    #region HashFieldExpireAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldExpireAsync_TimeSpan(IDatabaseAsync db)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("9.0.0"), "HEXPIRE requires Valkey 9.0+");

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
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("9.0.0"), "HEXPIREAT requires Valkey 9.0+");

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
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("9.0.0"), "HEXPIRE requires Valkey 9.0+");

        string key = $"ser-hexpire-when-{Guid.NewGuid()}";
        _ = await db.HashSetAsync(key, "field1", "value1", When.Always);

        // Always
        ExpireResult[] results = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60), ExpireWhen.Always);
        Assert.Equal(ExpireResult.Success, results[0]);

        // HasNoExpiry — should fail since field already has expiry
        results = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(120), ExpireWhen.HasNoExpiry);
        Assert.Equal(ExpireResult.ConditionNotMet, results[0]);

        // HasExpiry — should succeed since field has expiry
        results = await db.HashFieldExpireAsync(key, ["field1"], TimeSpan.FromSeconds(30), ExpireWhen.HasExpiry);
        Assert.Equal(ExpireResult.Success, results[0]);
    }

    #endregion
}
