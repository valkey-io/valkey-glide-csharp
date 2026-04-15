// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> sorted set commands.
/// </summary>
public class SortedSetCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region SortedSetScanAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScanAsync_EmptySet_ReturnsEmpty(IDatabaseAsync db)
    {
        string key = $"ser-zscan-empty-{Guid.NewGuid()}";

        List<SortedSetEntry> results = [];
        await foreach (var entry in db.SortedSetScanAsync(key))
        {
            results.Add(entry);
        }

        Assert.Empty(results);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScanAsync_ReturnsAllMembers(IDatabaseAsync db)
    {
        string key = $"ser-zscan-all-{Guid.NewGuid()}";
        _ = await db.SortedSetAddAsync(key, [
            new SortedSetEntry("member1", 1.0),
            new SortedSetEntry("member2", 2.0),
            new SortedSetEntry("member3", 3.0)
        ]);

        List<SortedSetEntry> results = [];
        await foreach (SortedSetEntry entry in db.SortedSetScanAsync(key))
        {
            results.Add(entry);
        }

        Assert.Equal(3, results.Count);
        Assert.Contains(results, e => e.Element == "member1" && e.Score == 1.0);
        Assert.Contains(results, e => e.Element == "member2" && e.Score == 2.0);
        Assert.Contains(results, e => e.Element == "member3" && e.Score == 3.0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScanAsync_WithPattern_FiltersResults(IDatabaseAsync db)
    {
        string key = $"ser-zscan-pattern-{Guid.NewGuid()}";
        _ = await db.SortedSetAddAsync(key, [
            new SortedSetEntry("test1", 1.0),
            new SortedSetEntry("test2", 2.0),
            new SortedSetEntry("other1", 3.0),
            new SortedSetEntry("other2", 4.0)
        ]);

        List<SortedSetEntry> results = [];
        await foreach (SortedSetEntry entry in db.SortedSetScanAsync(key, "test*"))
        {
            results.Add(entry);
        }

        Assert.Equal(2, results.Count);
        Assert.Contains(results, e => e.Element == "test1");
        Assert.Contains(results, e => e.Element == "test2");
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScanAsync_WithPageSize_ReturnsAllMembers(IDatabaseAsync db)
    {
        string key = $"ser-zscan-pagesize-{Guid.NewGuid()}";
        // Add enough members to require multiple pages
        SortedSetEntry[] entries = [.. Enumerable.Range(1, 100)
            .Select(i => new SortedSetEntry($"member{i}", i))];
        _ = await db.SortedSetAddAsync(key, entries);

        List<SortedSetEntry> results = [];
        await foreach (SortedSetEntry entry in db.SortedSetScanAsync(key, pageSize: 10))
        {
            results.Add(entry);
        }

        Assert.Equal(100, results.Count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScanAsync_WithPageOffset_SkipsElements(IDatabaseAsync db)
    {
        string key = $"ser-zscan-offset-{Guid.NewGuid()}";
        _ = await db.SortedSetAddAsync(key, [
            new SortedSetEntry("a", 1.0),
            new SortedSetEntry("b", 2.0),
            new SortedSetEntry("c", 3.0),
            new SortedSetEntry("d", 4.0),
            new SortedSetEntry("e", 5.0)
        ]);

        // With pageOffset, we skip elements from the first page
        List<SortedSetEntry> results = [];
        await foreach (SortedSetEntry entry in db.SortedSetScanAsync(key, pageSize: 1000, pageOffset: 2))
        {
            results.Add(entry);
        }

        // Should have 3 elements (5 - 2 skipped)
        Assert.Equal(3, results.Count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScanAsync_PreservesScores(IDatabaseAsync db)
    {
        string key = $"ser-zscan-scores-{Guid.NewGuid()}";
        _ = await db.SortedSetAddAsync(key, [
            new SortedSetEntry("member1", 1.5),
            new SortedSetEntry("member2", 2.75),
            new SortedSetEntry("member3", 3.125)
        ]);

        List<SortedSetEntry> results = [];
        await foreach (SortedSetEntry entry in db.SortedSetScanAsync(key))
        {
            results.Add(entry);
        }

        Assert.Equal(3, results.Count);
        Assert.Contains(results, e => e.Element == "member1" && Math.Abs(e.Score - 1.5) < 0.001);
        Assert.Contains(results, e => e.Element == "member2" && Math.Abs(e.Score - 2.75) < 0.001);
        Assert.Contains(results, e => e.Element == "member3" && Math.Abs(e.Score - 3.125) < 0.001);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScanAsync_WithCommandFlags_ThrowsOnNonNone(IDatabaseAsync db)
    {
        string key = $"ser-zscan-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(async () =>
        {
            await foreach (SortedSetEntry _ in db.SortedSetScanAsync(key, flags: CommandFlags.DemandMaster))
            {
                // Should not reach here
            }
        });
    }

    #endregion
}
