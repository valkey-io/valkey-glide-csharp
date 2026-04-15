// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> set commands.
/// </summary>
public class SetCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region SetScanAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetScanAsync_EmptySet_ReturnsEmpty(IDatabaseAsync db)
    {
        string key = $"ser-sscan-empty-{Guid.NewGuid()}";

        List<ValkeyValue> results = [];
        await foreach (var value in db.SetScanAsync(key))
        {
            results.Add(value);
        }

        Assert.Empty(results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetScanAsync_ReturnsAllMembers(IDatabaseAsync db)
    {
        string key = $"ser-sscan-all-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["member1", "member2", "member3"]);

        List<ValkeyValue> results = [];
        await foreach (ValkeyValue value in db.SetScanAsync(key))
        {
            results.Add(value);
        }

        Assert.Equal(3, results.Count);
        Assert.Contains((ValkeyValue)"member1", results);
        Assert.Contains((ValkeyValue)"member2", results);
        Assert.Contains((ValkeyValue)"member3", results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetScanAsync_WithPattern_FiltersResults(IDatabaseAsync db)
    {
        string key = $"ser-sscan-pattern-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["test1", "test2", "other1", "other2"]);

        List<ValkeyValue> results = [];
        await foreach (ValkeyValue value in db.SetScanAsync(key, "test*"))
        {
            results.Add(value);
        }

        Assert.Equal(2, results.Count);
        Assert.Contains((ValkeyValue)"test1", results);
        Assert.Contains((ValkeyValue)"test2", results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetScanAsync_WithPageSize_ReturnsAllMembers(IDatabaseAsync db)
    {
        string key = $"ser-sscan-pagesize-{Guid.NewGuid()}";
        // Add enough members to require multiple pages
        ValkeyValue[] members = [.. Enumerable.Range(1, 100).Select(i => (ValkeyValue)$"member{i}")];
        _ = await db.SetAddAsync(key, members);

        List<ValkeyValue> results = [];
        await foreach (ValkeyValue value in db.SetScanAsync(key, pageSize: 10))
        {
            results.Add(value);
        }

        Assert.Equal(100, results.Count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetScanAsync_WithPageOffset_SkipsElements(IDatabaseAsync db)
    {
        string key = $"ser-sscan-offset-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b", "c", "d", "e"]);

        // With pageOffset, we skip elements from the first page
        List<ValkeyValue> results = [];
        await foreach (ValkeyValue value in db.SetScanAsync(key, pageSize: 1000, pageOffset: 2))
        {
            results.Add(value);
        }

        // Should have 3 elements (5 - 2 skipped)
        Assert.Equal(3, results.Count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetScanAsync_WithCommandFlags_ThrowsOnNonNone(IDatabaseAsync db)
    {
        string key = $"ser-sscan-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(async () =>
        {
            await foreach (ValkeyValue _ in db.SetScanAsync(key, flags: CommandFlags.DemandMaster))
            {
                // Should not reach here
            }
        });
    }

    #endregion
}
