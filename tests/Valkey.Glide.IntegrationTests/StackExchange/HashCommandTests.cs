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
}
