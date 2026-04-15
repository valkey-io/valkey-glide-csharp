// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> sorted set commands.
/// </summary>
public class SortedSetCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region SortedSetAddAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_SingleEntry(IDatabaseAsync db)
    {
        string key = $"ser-zadd-entry-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, new SortedSetEntry("member1", 10.5)));
        Assert.False(await db.SortedSetAddAsync(key, new SortedSetEntry("member1", 15.0)));
        Assert.Equal(15.0, await db.SortedSetScoreAsync(key, "member1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_MultipleEntries(IDatabaseAsync db)
    {
        string key = $"ser-zadd-entries-{Guid.NewGuid()}";

        SortedSetEntry[] entries = [new("member1", 10.5), new("member2", 8.25)];
        Assert.Equal(2, await db.SortedSetAddAsync(key, entries));
        Assert.Equal(0, await db.SortedSetAddAsync(key, entries));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_SingleMember_WithSortedSetWhen_Always(IDatabaseAsync db)
    {
        string key = $"ser-zadd-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.5));
        Assert.False(await db.SortedSetAddAsync(key, "member1", 15.0));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_SingleMember_WithSortedSetWhen_NotExists(IDatabaseAsync db)
    {
        string key = $"ser-zadd-ssw-nx-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.5, SortedSetWhen.NotExists));
        Assert.False(await db.SortedSetAddAsync(key, "member1", 15.0, SortedSetWhen.NotExists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_SingleMember_WithSortedSetWhen_Exists(IDatabaseAsync db)
    {
        string key = $"ser-zadd-xx-{Guid.NewGuid()}";

        // XX on non-existent member should fail
        Assert.False(await db.SortedSetAddAsync(key, "member1", 10.5, SortedSetWhen.Exists));

        // Add member first, then update with XX
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.5));
        Assert.False(await db.SortedSetAddAsync(key, "member1", 15.0, SortedSetWhen.Exists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_SingleMember_WithSortedSetWhen_GreaterThan(IDatabaseAsync db)
    {
        string key = $"ser-zadd-ssw-gt-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.0));

        // GT: higher score should succeed (returns false because member already exists)
        Assert.False(await db.SortedSetAddAsync(key, "member1", 15.0, SortedSetWhen.GreaterThan));

        // GT: lower score should not update (returns false)
        Assert.False(await db.SortedSetAddAsync(key, "member1", 5.0, SortedSetWhen.GreaterThan));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_MultiMember_WithSortedSetWhen_Always(IDatabaseAsync db)
    {
        string key = $"ser-zadd-multi-{Guid.NewGuid()}";

        SortedSetEntry[] entries =
        [
            new("member1", 10.5),
            new("member2", 8.2),
        ];
        Assert.Equal(2, await db.SortedSetAddAsync(key, entries));

        // Adding same members again should return 0 (no new members)
        Assert.Equal(0, await db.SortedSetAddAsync(key, entries));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_MultiMember_WithSortedSetWhen_NotExists(IDatabaseAsync db)
    {
        string key = $"ser-zadd-multi-nx-{Guid.NewGuid()}";

        SortedSetEntry[] entries =
        [
            new("member1", 10.5),
            new("member2", 8.2),
        ];
        Assert.Equal(2, await db.SortedSetAddAsync(key, entries, SortedSetWhen.NotExists));

        // NX: existing members should not be added again
        SortedSetEntry[] moreEntries =
        [
            new("member1", 20.0),
            new("member3", 15.0),
        ];
        Assert.Equal(1, await db.SortedSetAddAsync(key, moreEntries, SortedSetWhen.NotExists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_MultiMember_WithSortedSetWhen(IDatabaseAsync db)
    {
        string key = $"ser-zadd-multi-ssw-{Guid.NewGuid()}";

        SortedSetEntry[] entries =
        [
            new("member1", 10.5),
            new("member2", 8.2),
        ];
        Assert.Equal(2, await db.SortedSetAddAsync(key, entries));

        // XX: only update existing members
        SortedSetEntry[] xxEntries =
        [
            new("member1", 20.0),
            new("member3", 15.0),
        ];
        Assert.Equal(0, await db.SortedSetAddAsync(key, xxEntries, SortedSetWhen.Exists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zadd-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetAddAsync(key, "member1", 10.5, SortedSetWhen.Always, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetCombineAndStoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAndStoreAsync_TwoKey_Union(IDatabaseAsync db)
    {
        string key1 = $"{{ser-zunionstore}}-1-{Guid.NewGuid()}";
        string key2 = $"{{ser-zunionstore}}-2-{Guid.NewGuid()}";
        string dest = $"{{ser-zunionstore}}-dest-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m2", 20.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m3", 30.0));

        long stored = await db.SortedSetCombineAndStoreAsync(SetOperation.Union, dest, key1, key2);
        Assert.Equal(3, stored);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAndStoreAsync_MultiKey_Intersect(IDatabaseAsync db)
    {
        string key1 = $"{{ser-zinterstore}}-1-{Guid.NewGuid()}";
        string key2 = $"{{ser-zinterstore}}-2-{Guid.NewGuid()}";
        string dest = $"{{ser-zinterstore}}-dest-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m2", 20.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m3", 30.0));

        IEnumerable<ValkeyKey> interKeys = [key1, key2];
        long stored = await db.SortedSetCombineAndStoreAsync(SetOperation.Intersect, dest, interKeys);
        Assert.Equal(1, stored);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAndStoreAsync_Difference(IDatabaseAsync db)
    {
        string key1 = $"{{ser-zdiffstore}}-1-{Guid.NewGuid()}";
        string key2 = $"{{ser-zdiffstore}}-2-{Guid.NewGuid()}";
        string dest = $"{{ser-zdiffstore}}-dest-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m2", 20.0));

        long stored = await db.SortedSetCombineAndStoreAsync(SetOperation.Difference, dest, key1, key2);
        Assert.Equal(1, stored);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAndStoreAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zcombinestore-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetCombineAndStoreAsync(SetOperation.Union, key, key, key, Aggregate.Sum, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetCombineAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAsync_Union_WithCommandFlags(IDatabaseAsync db)
    {
        string key1 = $"{{ser-zunion}}-1-{Guid.NewGuid()}";
        string key2 = $"{{ser-zunion}}-2-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m2", 20.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m3", 30.0));

        ValkeyValue[] result = await db.SortedSetCombineAsync(SetOperation.Union, [key1, key2]);
        Assert.Equal(3, result.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAsync_Intersect_WithCommandFlags(IDatabaseAsync db)
    {
        string key1 = $"{{ser-zinter}}-1-{Guid.NewGuid()}";
        string key2 = $"{{ser-zinter}}-2-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m2", 20.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m3", 30.0));

        ValkeyValue[] result = await db.SortedSetCombineAsync(SetOperation.Intersect, [key1, key2]);
        _ = Assert.Single(result);
        Assert.Equal("m2", result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAsync_Difference_WithCommandFlags(IDatabaseAsync db)
    {
        string key1 = $"{{ser-zdiff}}-1-{Guid.NewGuid()}";
        string key2 = $"{{ser-zdiff}}-2-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m2", 20.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m3", 30.0));

        ValkeyValue[] result = await db.SortedSetCombineAsync(SetOperation.Difference, [key1, key2]);
        _ = Assert.Single(result);
        Assert.Equal("m1", result[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zcombine-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetCombineAsync(SetOperation.Union, [key], null, Aggregate.Sum, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetCombineWithScoresAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineWithScoresAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key1 = $"{{ser-zunion-ws}}-1-{Guid.NewGuid()}";
        string key2 = $"{{ser-zunion-ws}}-2-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m2", 20.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m3", 30.0));

        SortedSetEntry[] result = await db.SortedSetCombineWithScoresAsync(SetOperation.Union, [key1, key2]);
        Assert.Equal(3, result.Length);
    }

    #endregion
    #region SortedSetDecrementAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetDecrementAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zdecr-{Guid.NewGuid()}";

        // Decrement on non-existent member creates it with negated value
        double result = await db.SortedSetDecrementAsync(key, "member1", 10.0);
        Assert.Equal(-10.0, result);

        // Decrement existing member
        result = await db.SortedSetDecrementAsync(key, "member1", 5.0);
        Assert.Equal(-15.0, result);

        // Decrement by zero
        result = await db.SortedSetDecrementAsync(key, "member1", 0.0);
        Assert.Equal(-15.0, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetDecrementAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zdecr-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetDecrementAsync(key, "member1", 1.0, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetIncrementAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetIncrementAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zincrby-{Guid.NewGuid()}";

        // Increment on non-existent member creates it
        double result = await db.SortedSetIncrementAsync(key, "member1", 10.5);
        Assert.Equal(10.5, result);

        // Increment existing member
        result = await db.SortedSetIncrementAsync(key, "member1", 5.0);
        Assert.Equal(15.5, result);

        // Negative increment
        result = await db.SortedSetIncrementAsync(key, "member1", -3.0);
        Assert.Equal(12.5, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetIncrementAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zincrby-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetIncrementAsync(key, "member1", 1.0, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetIntersectionLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetIntersectionLengthAsync_WithCommandFlags(IDatabaseAsync db)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZINTERCARD is supported since 7.0.0");

        string key1 = $"{{ser-zintercard}}-{Guid.NewGuid()}";
        string key2 = $"{{ser-zintercard}}-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m3", 3.0));

        Assert.True(await db.SortedSetAddAsync(key2, "m2", 20.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m3", 30.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m4", 40.0));

        // Default (no limit)
        Assert.Equal(2, await db.SortedSetIntersectionLengthAsync([key1, key2]));

        // With limit
        Assert.Equal(1, await db.SortedSetIntersectionLengthAsync([key1, key2], limit: 1));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetIntersectionLengthAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zintercard-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetIntersectionLengthAsync([key], flags: CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetLengthAsync_DefaultRange_UsesZCard(IDatabaseAsync db)
    {
        string key = $"ser-zlen-{Guid.NewGuid()}";

        Assert.Equal(0, await db.SortedSetLengthAsync(key));

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 3.0));

        Assert.Equal(3, await db.SortedSetLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetLengthAsync_WithRange_UsesZCount(IDatabaseAsync db)
    {
        string key = $"ser-zlen-range-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.5));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 5.0));
        Assert.True(await db.SortedSetAddAsync(key, "m4", 8.0));

        Assert.Equal(2, await db.SortedSetLengthAsync(key, 2.0, 6.0));
        Assert.Equal(1, await db.SortedSetLengthAsync(key, 2.5, 5.0, Exclude.Start));
        Assert.Equal(1, await db.SortedSetLengthAsync(key, 2.5, 5.0, Exclude.Stop));
        Assert.Equal(0, await db.SortedSetLengthAsync(key, 2.5, 5.0, Exclude.Both));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetLengthAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zcount-{Guid.NewGuid()}";

        Assert.Equal(0, await db.SortedSetLengthAsync(key));

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.5));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 5.0));
        Assert.True(await db.SortedSetAddAsync(key, "m4", 10.0));

        Assert.Equal(4, await db.SortedSetLengthAsync(key));
        Assert.Equal(2, await db.SortedSetLengthAsync(key, 2.0, 6.0));
        Assert.Equal(1, await db.SortedSetLengthAsync(key, 2.5, 5.0, Exclude.Start));
        Assert.Equal(0, await db.SortedSetLengthAsync(key, 15.0, 20.0));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetLengthAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zlen-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetLengthAsync(key, flags: CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetLengthByValueAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetLengthByValueAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zlexcount-{Guid.NewGuid()}";

        // Add members with same score for lexicographical ordering
        Assert.True(await db.SortedSetAddAsync(key, "apple", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "banana", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "cherry", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "date", 0.0));

        // Full range
        Assert.Equal(4, await db.SortedSetLengthByValueAsync(key, "a", "z"));

        // Specific range
        Assert.Equal(2, await db.SortedSetLengthByValueAsync(key, "b", "d"));

        // With exclusions
        Assert.Equal(1, await db.SortedSetLengthByValueAsync(key, "banana", "date", Exclude.Both));

        // Non-existent key
        Assert.Equal(0, await db.SortedSetLengthByValueAsync($"ser-zlexcount-nonexist-{Guid.NewGuid()}", "a", "z"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetLengthByValueAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zlexcount-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetLengthByValueAsync(key, "a", "z", flags: CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetPopAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetPopAsync_SingleElement_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zpop-{Guid.NewGuid()}";

        // Add test data
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.0));
        Assert.True(await db.SortedSetAddAsync(key, "member2", 5.0));
        Assert.True(await db.SortedSetAddAsync(key, "member3", 15.0));

        // Pop min (ascending)
        SortedSetEntry? minResult = await db.SortedSetPopAsync(key);
        _ = Assert.NotNull(minResult);
        Assert.Equal("member2", minResult.Value.Element);
        Assert.Equal(5.0, minResult.Value.Score);

        // Pop max (descending)
        SortedSetEntry? maxResult = await db.SortedSetPopAsync(key, Order.Descending);
        _ = Assert.NotNull(maxResult);
        Assert.Equal("member3", maxResult.Value.Element);
        Assert.Equal(15.0, maxResult.Value.Score);

        // Pop from key with one remaining element
        SortedSetEntry? lastResult = await db.SortedSetPopAsync(key);
        _ = Assert.NotNull(lastResult);
        Assert.Equal("member1", lastResult.Value.Element);

        // Pop from empty key
        SortedSetEntry? emptyResult = await db.SortedSetPopAsync(key);
        Assert.Null(emptyResult);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetPopAsync_MultipleElements_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zpop-multi-{Guid.NewGuid()}";

        // Add test data
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.0));
        Assert.True(await db.SortedSetAddAsync(key, "member2", 5.0));
        Assert.True(await db.SortedSetAddAsync(key, "member3", 15.0));

        // Pop 1 min element (ascending) — GLIDE currently only supports count=1
        SortedSetEntry[] minResults = await db.SortedSetPopAsync(key, 1);
        _ = Assert.Single(minResults);
        Assert.Equal("member2", minResults[0].Element);
        Assert.Equal(5.0, minResults[0].Score);

        // Pop 1 max element (descending)
        SortedSetEntry[] maxResults = await db.SortedSetPopAsync(key, 1, Order.Descending);
        _ = Assert.Single(maxResults);
        Assert.Equal("member3", maxResults[0].Element);
        Assert.Equal(15.0, maxResults[0].Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetPopAsync_NonExistentKey(IDatabaseAsync db)
    {
        string key = $"ser-zpop-nonexist-{Guid.NewGuid()}";

        SortedSetEntry? result = await db.SortedSetPopAsync(key);
        Assert.Null(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetPopAsync_MultiKey_WithCommandFlags(IDatabaseAsync db)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZMPOP is supported since 7.0.0");

        string key1 = $"{{ser-zmpop}}-{Guid.NewGuid()}";
        string key2 = $"{{ser-zmpop}}-{Guid.NewGuid()}";

        // Add test data
        Assert.True(await db.SortedSetAddAsync(key1, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key1, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key2, "m3", 3.0));

        // Pop from multiple keys
        SortedSetPopResult result = await db.SortedSetPopAsync([key1, key2], 1);
        Assert.False(result.IsNull);
        Assert.Equal(key1, result.Key);
        _ = Assert.Single(result.Entries);
        Assert.Equal("m1", result.Entries[0].Element);
        Assert.Equal(1.0, result.Entries[0].Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetPopAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zpop-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetPopAsync(key, Order.Ascending, CommandFlags.DemandMaster));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetPopAsync_MultiKey_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zmpop-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetPopAsync([key], 1, Order.Ascending, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetRandomMemberAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRandomMemberAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrandmember-{Guid.NewGuid()}";

        // Non-existent key
        ValkeyValue result = await db.SortedSetRandomMemberAsync(key);
        Assert.True(result.IsNull);

        // Add test data
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.5));
        Assert.True(await db.SortedSetAddAsync(key, "member2", 8.2));

        // Get random member
        result = await db.SortedSetRandomMemberAsync(key);
        Assert.False(result.IsNull);
        Assert.Contains(result.ToString(), new[] { "member1", "member2" });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRandomMemberAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zrandmember-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRandomMemberAsync(key, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetRandomMembersAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRandomMembersAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrandmember-multi-{Guid.NewGuid()}";

        // Add test data
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.5));
        Assert.True(await db.SortedSetAddAsync(key, "member2", 8.2));
        Assert.True(await db.SortedSetAddAsync(key, "member3", 15.0));

        // Get multiple random members
        ValkeyValue[] results = await db.SortedSetRandomMembersAsync(key, 2);
        Assert.Equal(2, results.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRandomMembersAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zrandmember-multi-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRandomMembersAsync(key, 2, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetRandomMembersWithScoresAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRandomMembersWithScoresAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrandmember-scores-{Guid.NewGuid()}";

        // Add test data
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.5));
        Assert.True(await db.SortedSetAddAsync(key, "member2", 8.2));
        Assert.True(await db.SortedSetAddAsync(key, "member3", 15.0));

        // Get random members with scores
        SortedSetEntry[] results = await db.SortedSetRandomMembersWithScoresAsync(key, 2);
        Assert.Equal(2, results.Length);
        Assert.All(results, entry =>
        {
            Assert.Contains(entry.Element.ToString(), new[] { "member1", "member2", "member3" });
            Assert.True(entry.Score > 0);
        });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRandomMembersWithScoresAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zrandmember-scores-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRandomMembersWithScoresAsync(key, 2, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetRangeAndStoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeAndStoreAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string srcKey = $"{{ser-zrangestore}}-src-{Guid.NewGuid()}";
        string destKey = $"{{ser-zrangestore}}-dest-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(srcKey, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(srcKey, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(srcKey, "m3", 3.0));
        Assert.True(await db.SortedSetAddAsync(srcKey, "m4", 4.0));

        long stored = await db.SortedSetRangeAndStoreAsync(srcKey, destKey, 1, 2);
        Assert.Equal(2, stored);
    }

    #endregion
    #region SortedSetRangeByRankAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByRankAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrange-rank-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 3.0));
        Assert.True(await db.SortedSetAddAsync(key, "m4", 4.0));

        // Default ascending
        ValkeyValue[] result = await db.SortedSetRangeByRankAsync(key);
        Assert.Equal(4, result.Length);
        Assert.Equal("m1", result[0]);
        Assert.Equal("m4", result[3]);

        // Specific range
        result = await db.SortedSetRangeByRankAsync(key, 1, 2);
        Assert.Equal(2, result.Length);
        Assert.Equal("m2", result[0]);
        Assert.Equal("m3", result[1]);

        // Descending
        result = await db.SortedSetRangeByRankAsync(key, 0, 1, Order.Descending);
        Assert.Equal(2, result.Length);
        Assert.Equal("m4", result[0]);
        Assert.Equal("m3", result[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByRankAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zrange-rank-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRangeByRankAsync(key, 0, -1, Order.Ascending, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetRangeByRankWithScoresAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByRankWithScoresAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrange-rank-scores-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.5));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.5));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 3.5));

        SortedSetEntry[] result = await db.SortedSetRangeByRankWithScoresAsync(key);
        Assert.Equal(3, result.Length);
        Assert.Equal("m1", result[0].Element);
        Assert.Equal(1.5, result[0].Score);
        Assert.Equal("m3", result[2].Element);
        Assert.Equal(3.5, result[2].Score);
    }

    #endregion
    #region SortedSetRangeByScoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByScoreAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrange-score-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.5));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 5.0));
        Assert.True(await db.SortedSetAddAsync(key, "m4", 10.0));

        ValkeyValue[] result = await db.SortedSetRangeByScoreAsync(key, 2.0, 6.0);
        Assert.Equal(2, result.Length);
        Assert.Equal("m2", result[0]);
        Assert.Equal("m3", result[1]);

        // With exclusion
        result = await db.SortedSetRangeByScoreAsync(key, 2.5, 5.0, Exclude.Start);
        _ = Assert.Single(result);
        Assert.Equal("m3", result[0]);
    }

    #endregion
    #region SortedSetRangeByScoreWithScoresAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByScoreWithScoresAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrange-score-ws-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.5));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 5.0));

        SortedSetEntry[] result = await db.SortedSetRangeByScoreWithScoresAsync(key, 1.0, 5.0);
        Assert.Equal(3, result.Length);
        Assert.Equal("m1", result[0].Element);
        Assert.Equal(1.0, result[0].Score);
    }

    #endregion
    #region SortedSetRangeByValueAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByValueAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrange-value-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "apple", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "banana", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "cherry", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "date", 0.0));

        ValkeyValue[] result = await db.SortedSetRangeByValueAsync(key, "b", "d");
        Assert.Equal(2, result.Length);
        Assert.Equal("banana", result[0]);
        Assert.Equal("cherry", result[1]);
    }

    #endregion
    #region SortedSetRankAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRankAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrank-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 3.0));

        // Ascending rank
        long? rank = await db.SortedSetRankAsync(key, "m1");
        _ = Assert.NotNull(rank);
        Assert.Equal(0, rank.Value);

        rank = await db.SortedSetRankAsync(key, "m3");
        _ = Assert.NotNull(rank);
        Assert.Equal(2, rank.Value);

        // Descending rank (ZREVRANK)
        rank = await db.SortedSetRankAsync(key, "m1", Order.Descending);
        _ = Assert.NotNull(rank);
        Assert.Equal(2, rank.Value);

        // Non-existent member
        rank = await db.SortedSetRankAsync(key, "nonexistent");
        Assert.Null(rank);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRankAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zrank-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRankAsync(key, "m1", Order.Ascending, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetRemoveAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveAsync_SingleMember_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrem-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.0));

        Assert.True(await db.SortedSetRemoveAsync(key, "m1"));
        Assert.False(await db.SortedSetRemoveAsync(key, "nonexistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveAsync_MultiMember_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zrem-multi-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 3.0));

        IEnumerable<ValkeyValue> removeMembers1 = ["m1", "m3"];
        Assert.Equal(2, await db.SortedSetRemoveAsync(key, removeMembers1));
        IEnumerable<ValkeyValue> removeMembers2 = ["m1", "nonexistent"];
        Assert.Equal(0, await db.SortedSetRemoveAsync(key, removeMembers2));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zrem-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRemoveAsync(key, "m1", CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetRemoveRangeByRankAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveRangeByRankAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zremrangebyrank-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.0));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 3.0));
        Assert.True(await db.SortedSetAddAsync(key, "m4", 4.0));

        long removed = await db.SortedSetRemoveRangeByRankAsync(key, 1, 2);
        Assert.Equal(2, removed);

        Assert.Equal(2, await db.SortedSetLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveRangeByRankAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zremrangebyrank-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRemoveRangeByRankAsync(key, 0, 1, CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetRemoveRangeByScoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveRangeByScoreAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zremrangebyscore-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.5));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 5.0));
        Assert.True(await db.SortedSetAddAsync(key, "m4", 10.0));

        long removed = await db.SortedSetRemoveRangeByScoreAsync(key, 2.0, 6.0);
        Assert.Equal(2, removed);

        Assert.Equal(2, await db.SortedSetLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveRangeByScoreAsync_WithExclusion(IDatabaseAsync db)
    {
        string key = $"ser-zremrangebyscore-excl-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "m1", 1.0));
        Assert.True(await db.SortedSetAddAsync(key, "m2", 2.5));
        Assert.True(await db.SortedSetAddAsync(key, "m3", 5.0));

        long removed = await db.SortedSetRemoveRangeByScoreAsync(key, 2.5, 5.0, Exclude.Both);
        Assert.Equal(0, removed);
    }

    #endregion
    #region SortedSetRemoveRangeByValueAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveRangeByValueAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zremrangebylex-{Guid.NewGuid()}";

        Assert.True(await db.SortedSetAddAsync(key, "apple", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "banana", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "cherry", 0.0));
        Assert.True(await db.SortedSetAddAsync(key, "date", 0.0));

        long removed = await db.SortedSetRemoveRangeByValueAsync(key, "b", "d");
        Assert.Equal(2, removed);

        Assert.Equal(2, await db.SortedSetLengthAsync(key));
    }

    #endregion
    #region SortedSetScoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScoreAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zscore-{Guid.NewGuid()}";

        // Non-existent key
        Assert.Null(await db.SortedSetScoreAsync(key, "member1"));

        // Add test data
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.5));
        Assert.True(await db.SortedSetAddAsync(key, "member2", 8.2));

        // Get score of existing member
        double? score = await db.SortedSetScoreAsync(key, "member1");
        _ = Assert.NotNull(score);
        Assert.Equal(10.5, score.Value);

        // Get score of non-existent member
        Assert.Null(await db.SortedSetScoreAsync(key, "nonexistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScoreAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zscore-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetScoreAsync(key, "member1", CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetScoresAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScoresAsync_WithCommandFlags(IDatabaseAsync db)
    {
        string key = $"ser-zmscore-{Guid.NewGuid()}";

        // Add test data
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.5));
        Assert.True(await db.SortedSetAddAsync(key, "member2", 8.2));

        // Get scores of multiple members
        double?[] scores = await db.SortedSetScoresAsync(key, ["member1", "member2", "nonexistent"]);
        Assert.Equal(3, scores.Length);
        Assert.Equal(10.5, scores[0]);
        Assert.Equal(8.2, scores[1]);
        Assert.Null(scores[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScoresAsync_UnsupportedCommandFlags_Throws(IDatabaseAsync db)
    {
        string key = $"ser-zmscore-flags-{Guid.NewGuid()}";

        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetScoresAsync(key, ["member1"], CommandFlags.DemandMaster));
    }

    #endregion
    #region SortedSetUpdateAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetUpdateAsync_SingleMember(IDatabaseAsync db)
    {
        string key = $"ser-zadd-update-{Guid.NewGuid()}";

        // Add initial member
        Assert.True(await db.SortedSetAddAsync(key, "member1", 10.0));

        // Update with CH flag via SortedSetUpdateAsync — returns true if score changed
        Assert.True(await db.SortedSetUpdateAsync(key, "member1", 15.0));

        // Update with same score — returns false (no change)
        Assert.False(await db.SortedSetUpdateAsync(key, "member1", 15.0));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetUpdateAsync_MultiMember(IDatabaseAsync db)
    {
        string key = $"ser-zadd-update-multi-{Guid.NewGuid()}";

        // Add initial members
        SortedSetEntry[] entries =
        [
            new("member1", 10.0),
            new("member2", 20.0),
        ];
        Assert.Equal(2, await db.SortedSetAddAsync(key, entries));

        // Update scores — returns count of changed members
        SortedSetEntry[] updatedEntries =
        [
            new("member1", 15.0),
            new("member2", 25.0),
        ];
        Assert.Equal(2, await db.SortedSetUpdateAsync(key, updatedEntries));

        // Update with same scores — returns 0 (no changes)
        Assert.Equal(0, await db.SortedSetUpdateAsync(key, updatedEntries));
    }

    #endregion
}
