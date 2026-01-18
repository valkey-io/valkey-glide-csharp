// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class SortedSetCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;
    private static readonly double BlockingTimeoutSecs = 1.0;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_SingleMember(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test adding a new member
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));

        // Test updating existing member (should return false)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_MultipleMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        SortedSetEntry[] entries = [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0)
        ];

        // Test adding multiple new members
        Assert.Equal(3, await client.SortedSetAddAsync(key, entries));

        // Test adding mix of new and existing members
        SortedSetEntry[] newEntries = [
            new("member1", 20.0), // Update existing
            new("member4", 12.0)  // Add new
        ];
        Assert.Equal(1, await client.SortedSetAddAsync(key, newEntries)); // Only member4 is new
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithNotExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial member
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));

        // Try to add existing member with NX (should fail)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetWhen.NotExists));

        // Add new member with NX (should succeed)
        Assert.True(await client.SortedSetAddAsync(key, "member2", 8.0, SortedSetWhen.NotExists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Try to update non-existing member with XX (should fail)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 10.5, SortedSetWhen.Exists));

        // Add member normally first
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));

        // Update existing member with XX (should succeed)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetWhen.Exists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithGreaterThan(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial member
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.0));

        // Update with higher score using GT (should succeed)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetWhen.GreaterThan));

        // Try to update with lower score using GT (should fail)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 5.0, SortedSetWhen.GreaterThan));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithLessThan(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial member
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.0));

        // Update with lower score using LT (should succeed)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 5.0, SortedSetWhen.LessThan));

        // Try to update with higher score using LT (should fail)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetWhen.LessThan));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_MultipleWithConditions(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial members
        SortedSetEntry[] initialEntries = [
            new("member1", 10.0),
            new("member2", 8.0)
        ];
        Assert.Equal(2, await client.SortedSetAddAsync(key, initialEntries));

        // Try to add with NX (should only add new members)
        SortedSetEntry[] nxEntries = [
            new("member1", 15.0), // Existing, should not update
            new("member3", 12.0)  // New, should add
        ];
        Assert.Equal(1, await client.SortedSetAddAsync(key, nxEntries, SortedSetWhen.NotExists));

        // Update existing members with XX
        SortedSetEntry[] xxEntries = [
            new("member1", 20.0), // Existing, should update
            new("member4", 5.0)   // New, should not add
        ];
        Assert.Equal(0, await client.SortedSetAddAsync(key, xxEntries, SortedSetWhen.Exists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_NegativeScores(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test with negative scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", -10.5));
        Assert.True(await client.SortedSetAddAsync(key, "member2", -5.0));

        SortedSetEntry[] entries = [
            new("member3", -15.0),
            new("member4", 0.0)
        ];
        Assert.Equal(2, await client.SortedSetAddAsync(key, entries));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_SpecialScores(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test with special double values that server supports
        Assert.True(await client.SortedSetAddAsync(key, "inf", double.PositiveInfinity));
        Assert.True(await client.SortedSetAddAsync(key, "neginf", double.NegativeInfinity));
        Assert.True(await client.SortedSetAddAsync(key, "zero", 0.0));

        // Test with very large/small values (but not Min/Max which might not be supported)
        Assert.True(await client.SortedSetAddAsync(key, "large", 1e100));
        Assert.True(await client.SortedSetAddAsync(key, "small", -1e100));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_EmptyArray(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Adding empty array should throw an exception
        await Assert.ThrowsAsync<RequestException>(async () => await client.SortedSetAddAsync(key, []));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_ObsoleteOverloads(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test overload with default parameters
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));

        // Test obsolete overload with When enum
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, When.Exists));

        // Test array overloads
        SortedSetEntry[] entries = [new("member2", 8.0)];
        Assert.Equal(1, await client.SortedSetAddAsync(key, entries));
        Assert.Equal(0, await client.SortedSetAddAsync(key, entries, When.Exists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRemove_SingleMember(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test removing from non-existent key
        Assert.False(await client.SortedSetRemoveAsync(key, "member1"));

        // Add members first
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 8.2));

        // Test removing existing member
        Assert.True(await client.SortedSetRemoveAsync(key, "member1"));

        // Test removing already removed member
        Assert.False(await client.SortedSetRemoveAsync(key, "member1"));

        // Test removing non-existent member
        Assert.False(await client.SortedSetRemoveAsync(key, "nonexistent"));

        // Verify remaining member still exists
        Assert.True(await client.SortedSetRemoveAsync(key, "member2"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRemove_MultipleMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test removing from non-existent key
        Assert.Equal(0, await client.SortedSetRemoveAsync(key, ["member1", "member2"]));

        // Add members first
        SortedSetEntry[] entries = [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0),
            new("member4", 12.0)
        ];
        Assert.Equal(4, await client.SortedSetAddAsync(key, entries));

        // Test removing multiple existing members
        Assert.Equal(2, await client.SortedSetRemoveAsync(key, ["member1", "member3"]));

        // Test removing mix of existing and non-existing members
        Assert.Equal(1, await client.SortedSetRemoveAsync(key, ["member2", "nonexistent", "member5"]));

        // Test removing already removed members
        Assert.Equal(0, await client.SortedSetRemoveAsync(key, ["member1", "member2"]));

        // Verify only member4 remains
        Assert.Equal(1, await client.SortedSetRemoveAsync(key, ["member4"]));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRemove_EmptyArray(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add some members first
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));

        // Test removing empty array should throw an exception
        await Assert.ThrowsAsync<RequestException>(async () => await client.SortedSetRemoveAsync(key, []));

        // Verify member still exists
        Assert.True(await client.SortedSetRemoveAsync(key, "member1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRemove_DuplicateMembers(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add members first
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 8.2));

        // Test removing with duplicate member names in array
        ValkeyValue[] membersWithDuplicates = ["member1", "member1", "member2", "member1"];
        Assert.Equal(2, await client.SortedSetRemoveAsync(key, membersWithDuplicates));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRemove_SpecialValues(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test with special string values
        ValkeyValue[] specialMembers = ["", " ", "null", "0", "-1", "true", "false"];

        // Add special members with various scores
        for (int i = 0; i < specialMembers.Length; i++)
        {
            Assert.True(await client.SortedSetAddAsync(key, specialMembers[i], i * 1.5));
        }

        // Remove some special members
        Assert.Equal(3, await client.SortedSetRemoveAsync(key, ["", "null", "false"]));

        // Remove remaining members one by one
        Assert.True(await client.SortedSetRemoveAsync(key, " "));
        Assert.True(await client.SortedSetRemoveAsync(key, "0"));
        Assert.True(await client.SortedSetRemoveAsync(key, "-1"));
        Assert.True(await client.SortedSetRemoveAsync(key, "true"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetLengthAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        Assert.Equal(0, await client.SortedSetLengthAsync(key));
        Assert.Equal(0, await client.SortedSetLengthAsync(key, 1.0, 10.0));

        // Add members with different scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.5));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 5.0));
        Assert.True(await client.SortedSetAddAsync(key, "member4", 8.0));

        // Test cardinality (default infinity parameters use ZCARD)
        Assert.Equal(4, await client.SortedSetLengthAsync(key));
        Assert.Equal(4, await client.SortedSetLengthAsync(key, double.NegativeInfinity, double.PositiveInfinity));

        // Test count with range parameters (uses ZCOUNT)
        Assert.Equal(2, await client.SortedSetLengthAsync(key, 2.0, 6.0));
        Assert.Equal(1, await client.SortedSetLengthAsync(key, 2.5, 5.0, Exclude.Start));
        Assert.Equal(1, await client.SortedSetLengthAsync(key, 2.5, 5.0, Exclude.Stop));
        Assert.Equal(0, await client.SortedSetLengthAsync(key, 2.5, 5.0, Exclude.Both));

        // Test with no matches
        Assert.Equal(0, await client.SortedSetLengthAsync(key, 15.0, 20.0));

        // Remove a member and test both modes
        Assert.True(await client.SortedSetRemoveAsync(key, "member2"));
        Assert.Equal(3, await client.SortedSetLengthAsync(key));
        Assert.Equal(1, await client.SortedSetLengthAsync(key, 2.0, 6.0));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetCardAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        Assert.Equal(0, await client.SortedSetCardAsync(key));

        // Add members and test cardinality
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.0));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 3.0));

        Assert.Equal(3, await client.SortedSetCardAsync(key));

        // Remove a member and test cardinality
        Assert.True(await client.SortedSetRemoveAsync(key, "member2"));
        Assert.Equal(2, await client.SortedSetCardAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetCountAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        Assert.Equal(0, await client.SortedSetCountAsync(key));

        // Add members with different scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.5));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 5.0));
        Assert.True(await client.SortedSetAddAsync(key, "member4", 10.0));

        // Test count with default range (all elements)
        Assert.Equal(4, await client.SortedSetCountAsync(key));

        // Test count with specific range
        Assert.Equal(2, await client.SortedSetCountAsync(key, 2.0, 6.0));

        // Test count with exclusive bounds
        Assert.Equal(1, await client.SortedSetCountAsync(key, 2.5, 5.0, Exclude.Start));  // Exclude member2 (2.5), include member3 (5.0)
        Assert.Equal(1, await client.SortedSetCountAsync(key, 2.5, 5.0, Exclude.Stop));   // Include member2 (2.5), exclude member3 (5.0)
        Assert.Equal(0, await client.SortedSetCountAsync(key, 2.5, 5.0, Exclude.Both));   // Exclude both member2 and member3

        // Test count with infinity bounds
        Assert.Equal(4, await client.SortedSetCountAsync(key, double.NegativeInfinity, double.PositiveInfinity));

        // Test count with no matches
        Assert.Equal(0, await client.SortedSetCountAsync(key, 15.0, 20.0));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByRankAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        ValkeyValue[] result = await client.SortedSetRangeByRankAsync(key);
        Assert.Empty(result);

        // Add members with scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.0));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 3.0));
        Assert.True(await client.SortedSetAddAsync(key, "member4", 4.0));

        // Test default range (all elements, ascending)
        result = await client.SortedSetRangeByRankAsync(key);
        Assert.Equal(4, result.Length);
        Assert.Equal("member1", result[0]);
        Assert.Equal("member2", result[1]);
        Assert.Equal("member3", result[2]);
        Assert.Equal("member4", result[3]);

        // Test specific range
        result = await client.SortedSetRangeByRankAsync(key, 1, 2);
        Assert.Equal(2, result.Length);
        Assert.Equal("member2", result[0]);
        Assert.Equal("member3", result[1]);

        // Test descending order
        result = await client.SortedSetRangeByRankAsync(key, 0, 1, Order.Descending);
        Assert.Equal(2, result.Length);
        Assert.Equal("member4", result[0]);
        Assert.Equal("member3", result[1]);

        // Test negative indices
        result = await client.SortedSetRangeByRankAsync(key, -2, -1);
        Assert.Equal(2, result.Length);
        Assert.Equal("member3", result[0]);
        Assert.Equal("member4", result[1]);

        // Test single element range
        result = await client.SortedSetRangeByRankAsync(key, 0, 0);
        Assert.Single(result);
        Assert.Equal("member1", result[0]);

        // Test out of range
        result = await client.SortedSetRangeByRankAsync(key, 10, 20);
        Assert.Empty(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByRankWithScoresAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        SortedSetEntry[] result = await client.SortedSetRangeByRankWithScoresAsync(key);
        Assert.Empty(result);

        // Add members with scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.5));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.5));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 3.5));

        // Test default range (all elements, ascending)
        result = await client.SortedSetRangeByRankWithScoresAsync(key);
        Assert.Equal(3, result.Length);
        Assert.Equal("member1", result[0].Element);
        Assert.Equal(1.5, result[0].Score);
        Assert.Equal("member2", result[1].Element);
        Assert.Equal(2.5, result[1].Score);
        Assert.Equal("member3", result[2].Element);
        Assert.Equal(3.5, result[2].Score);

        // Test specific range
        result = await client.SortedSetRangeByRankWithScoresAsync(key, 0, 1);
        Assert.Equal(2, result.Length);
        Assert.Equal("member1", result[0].Element);
        Assert.Equal(1.5, result[0].Score);
        Assert.Equal("member2", result[1].Element);
        Assert.Equal(2.5, result[1].Score);

        // Test descending order
        result = await client.SortedSetRangeByRankWithScoresAsync(key, 0, 1, Order.Descending);
        Assert.Equal(2, result.Length);
        Assert.Equal("member3", result[0].Element);
        Assert.Equal(3.5, result[0].Score);
        Assert.Equal("member2", result[1].Element);
        Assert.Equal(2.5, result[1].Score);

        // Test single element
        result = await client.SortedSetRangeByRankWithScoresAsync(key, 1, 1);
        Assert.Single(result);
        Assert.Equal("member2", result[0].Element);
        Assert.Equal(2.5, result[0].Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByRank_SpecialScores(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add members with special scores
        Assert.True(await client.SortedSetAddAsync(key, "neginf", double.NegativeInfinity));
        Assert.True(await client.SortedSetAddAsync(key, "zero", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "posinf", double.PositiveInfinity));

        // Test range with special scores
        ValkeyValue[] result = await client.SortedSetRangeByRankAsync(key);
        Assert.Equal(3, result.Length);
        Assert.Equal("neginf", result[0]);
        Assert.Equal("zero", result[1]);
        Assert.Equal("posinf", result[2]);

        // Test with scores
        SortedSetEntry[] resultWithScores = await client.SortedSetRangeByRankWithScoresAsync(key);
        Assert.Equal(3, resultWithScores.Length);
        Assert.Equal("neginf", resultWithScores[0].Element);
        Assert.True(double.IsNegativeInfinity(resultWithScores[0].Score));
        Assert.Equal("zero", resultWithScores[1].Element);
        Assert.Equal(0.0, resultWithScores[1].Score);
        Assert.Equal("posinf", resultWithScores[2].Element);
        Assert.True(double.IsPositiveInfinity(resultWithScores[2].Score));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByScoreWithScoresAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        SortedSetEntry[] result = await client.SortedSetRangeByScoreWithScoresAsync(key);
        Assert.Empty(result);

        // Add members with scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.5));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 5.0));
        Assert.True(await client.SortedSetAddAsync(key, "member4", 10.0));

        // Test default range (all elements, ascending)
        result = await client.SortedSetRangeByScoreWithScoresAsync(key);
        Assert.Equal(4, result.Length);
        Assert.Equal("member1", result[0].Element);
        Assert.Equal(1.0, result[0].Score);
        Assert.Equal("member2", result[1].Element);
        Assert.Equal(2.5, result[1].Score);
        Assert.Equal("member3", result[2].Element);
        Assert.Equal(5.0, result[2].Score);
        Assert.Equal("member4", result[3].Element);
        Assert.Equal(10.0, result[3].Score);

        // Test specific score range
        result = await client.SortedSetRangeByScoreWithScoresAsync(key, 2.0, 6.0);
        Assert.Equal(2, result.Length);
        Assert.Equal("member2", result[0].Element);
        Assert.Equal(2.5, result[0].Score);
        Assert.Equal("member3", result[1].Element);
        Assert.Equal(5.0, result[1].Score);

        // Test descending order
        result = await client.SortedSetRangeByScoreWithScoresAsync(key, 2.0, 6.0, order: Order.Descending);
        Assert.Equal(2, result.Length);
        Assert.Equal("member3", result[0].Element);
        Assert.Equal(5.0, result[0].Score);
        Assert.Equal("member2", result[1].Element);
        Assert.Equal(2.5, result[1].Score);

        // Test with exclusions
        result = await client.SortedSetRangeByScoreWithScoresAsync(key, 2.5, 5.0, Exclude.Start);
        Assert.Single(result);
        Assert.Equal("member3", result[0].Element);
        Assert.Equal(5.0, result[0].Score);

        // Test with limit
        result = await client.SortedSetRangeByScoreWithScoresAsync(key, double.NegativeInfinity, double.PositiveInfinity, skip: 1, take: 2);
        Assert.Equal(2, result.Length);
        Assert.Equal("member2", result[0].Element);
        Assert.Equal(2.5, result[0].Score);
        Assert.Equal("member3", result[1].Element);
        Assert.Equal(5.0, result[1].Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByValueAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        ValkeyValue[] result = await client.SortedSetRangeByValueAsync(key, "a", "z", Exclude.None, Order.Ascending, 0, -1);
        Assert.Empty(result);

        // Add members with same score for lexicographical ordering
        Assert.True(await client.SortedSetAddAsync(key, "apple", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "banana", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "cherry", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "date", 0.0));

        // Test specific range
        result = await client.SortedSetRangeByValueAsync(key, "b", "d", Exclude.None, Order.Ascending, 0, -1);
        Assert.Equal(2, result.Length);
        Assert.Equal("banana", result[0]);
        Assert.Equal("cherry", result[1]);

        // Test with exclusions
        result = await client.SortedSetRangeByValueAsync(key, "banana", "cherry", Exclude.Start, Order.Ascending, 0, -1);
        Assert.Single(result);
        Assert.Equal("cherry", result[0]);

        // Test with limit
        result = await client.SortedSetRangeByValueAsync(key, "a", "z", Exclude.None, Order.Ascending, 1, 2);
        Assert.Equal(2, result.Length);
        Assert.Equal("banana", result[0]);
        Assert.Equal("cherry", result[1]);

        // Test full range
        result = await client.SortedSetRangeByValueAsync(key, double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Ascending, 0, -1);
        Assert.Equal(4, result.Length);
        Assert.Equal("apple", result[0]);
        Assert.Equal("banana", result[1]);
        Assert.Equal("cherry", result[2]);
        Assert.Equal("date", result[3]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByValueWithOrderAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        ValkeyValue[] result = await client.SortedSetRangeByValueAsync(key, order: Order.Descending);
        Assert.Empty(result);

        // Add members with same score for lexicographical ordering
        Assert.True(await client.SortedSetAddAsync(key, "apple", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "banana", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "cherry", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "date", 0.0));

        // Test ascending order (default)
        result = await client.SortedSetRangeByValueAsync(key, order: Order.Ascending);
        Assert.Equal(4, result.Length);
        Assert.Equal("apple", result[0]);
        Assert.Equal("banana", result[1]);
        Assert.Equal("cherry", result[2]);
        Assert.Equal("date", result[3]);

        // Test descending order
        result = await client.SortedSetRangeByValueAsync(key, order: Order.Descending);
        Assert.Equal(4, result.Length);
        Assert.Equal("date", result[0]);
        Assert.Equal("cherry", result[1]);
        Assert.Equal("banana", result[2]);
        Assert.Equal("apple", result[3]);

        // Test specific range with descending order
        result = await client.SortedSetRangeByValueAsync(key, "b", "d", order: Order.Descending);
        Assert.Equal(2, result.Length);
        Assert.Equal("cherry", result[0]);
        Assert.Equal("banana", result[1]);

        // Test with limit and descending order
        result = await client.SortedSetRangeByValueAsync(key, order: Order.Descending, skip: 1, take: 2);
        Assert.Equal(2, result.Length);
        Assert.Equal("cherry", result[0]);
        Assert.Equal("banana", result[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetCombine(BaseClient client)
    {
        string key1 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key2 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key3 = $"{{sortedSetKey}}-{Guid.NewGuid()}";

        // Setup test data
        await client.SortedSetAddAsync(key1, [
            new("member1", 10.0),
            new("member2", 20.0)
        ]);
        await client.SortedSetAddAsync(key2, [
            new("member2", 15.0),
            new("member3", 25.0)
        ]);

        // Test union
        ValkeyValue[] result = await client.SortedSetCombineAsync(SetOperation.Union, [key1, key2]);
        Assert.Equal(3, result.Length);
        Assert.Contains("member1", result.Select(v => v.ToString()));
        Assert.Contains("member2", result.Select(v => v.ToString()));
        Assert.Contains("member3", result.Select(v => v.ToString()));

        // Test intersection
        result = await client.SortedSetCombineAsync(SetOperation.Intersect, [key1, key2]);
        Assert.Single(result);
        Assert.Equal("member2", result[0]);

        // Test difference
        result = await client.SortedSetCombineAsync(SetOperation.Difference, [key1, key2]);
        Assert.Single(result);
        Assert.Equal("member1", result[0]);

        // Test with non-existent key
        result = await client.SortedSetCombineAsync(SetOperation.Union, [key1, key3]);
        Assert.Equal(2, result.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetCombineWithScores(BaseClient client)
    {
        string key1 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key2 = $"{{sortedSetKey}}-{Guid.NewGuid()}";

        // Setup test data
        await client.SortedSetAddAsync(key1, [
            new("member1", 10.0),
            new("member2", 20.0)
        ]);
        await client.SortedSetAddAsync(key2, [
            new("member2", 15.0),
            new("member3", 25.0)
        ]);

        // Test union with scores
        SortedSetEntry[] result = await client.SortedSetCombineWithScoresAsync(SetOperation.Union, [key1, key2]);
        Assert.Equal(3, result.Length);

        // Test intersection with scores
        result = await client.SortedSetCombineWithScoresAsync(SetOperation.Intersect, [key1, key2]);
        Assert.Single(result);
        Assert.Equal("member2", result[0].Element);
        Assert.Equal(35.0, result[0].Score); // Sum aggregation: 20 + 15

        // Test with weights
        result = await client.SortedSetCombineWithScoresAsync(SetOperation.Union, [key1, key2], [2.0, 0.5]);
        Assert.Equal(3, result.Length);
        SortedSetEntry member2Entry = result.First(e => e.Element == "member2");
        Assert.Equal(47.5, member2Entry.Score); // (20 * 2) + (15 * 0.5)
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetCombineAndStore(BaseClient client)
    {
        string key1 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key2 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string destKey = $"{{sortedSetKey}}-{Guid.NewGuid()}";

        // Setup test data
        await client.SortedSetAddAsync(key1, [
            new("member1", 10.0),
            new("member2", 20.0)
        ]);
        await client.SortedSetAddAsync(key2, [
            new("member2", 15.0),
            new("member3", 25.0)
        ]);

        // Test union and store
        long result = await client.SortedSetCombineAndStoreAsync(SetOperation.Union, destKey, key1, key2);
        Assert.Equal(3, result);

        // Verify stored result
        long count = await client.SortedSetCardAsync(destKey);
        Assert.Equal(3, count);

        // Test intersection and store with multiple keys
        result = await client.SortedSetCombineAndStoreAsync(SetOperation.Intersect, destKey, [key1, key2]);
        Assert.Equal(1, result);

        count = await client.SortedSetCardAsync(destKey);
        Assert.Equal(1, count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetIncrement(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test increment on non-existent member
        double result = await client.SortedSetIncrementAsync(key, "member1", 10.5);
        Assert.Equal(10.5, result);

        // Test increment on existing member
        result = await client.SortedSetIncrementAsync(key, "member1", 5.0);
        Assert.Equal(15.5, result);

        // Test negative increment
        result = await client.SortedSetIncrementAsync(key, "member1", -3.0);
        Assert.Equal(12.5, result);

        // Test increment by zero
        result = await client.SortedSetIncrementAsync(key, "member1", 0.0);
        Assert.Equal(12.5, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetIntersectionLength(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZINTERCARD is supported since 7.0.0"
        );
        string key1 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key2 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key3 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string emptyKey = $"{{sortedSetKey}}-{Guid.NewGuid()}";

        // Setup test data
        await client.SortedSetAddAsync(key1, [
            new("member1", 10.0),
            new("member2", 20.0),
            new("member3", 30.0)
        ]);
        await client.SortedSetAddAsync(key2, [
            new("member2", 15.0),
            new("member3", 25.0),
            new("member4", 35.0)
        ]);
        await client.SortedSetAddAsync(key3, [
            new("member3", 40.0),
            new("member5", 50.0)
        ]);

        // Test intersection of two sets
        long result = await client.SortedSetIntersectionLengthAsync([key1, key2]);
        Assert.Equal(2, result); // member2, member3

        // Test intersection of three sets
        result = await client.SortedSetIntersectionLengthAsync([key1, key2, key3]);
        Assert.Equal(1, result); // member3

        // Test with limit
        result = await client.SortedSetIntersectionLengthAsync([key1, key2], 1);
        Assert.Equal(1, result);

        // Test with non-existent key
        result = await client.SortedSetIntersectionLengthAsync([key1, emptyKey]);
        Assert.Equal(0, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetLengthByValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Setup test data with same scores for lexicographical ordering
        await client.SortedSetAddAsync(key, [
            new("apple", 0.0),
            new("banana", 0.0),
            new("cherry", 0.0),
            new("date", 0.0)
        ]);

        // Test full range
        long result = await client.SortedSetLengthByValueAsync(key, "a", "z");
        Assert.Equal(4, result);

        // Test specific range
        result = await client.SortedSetLengthByValueAsync(key, "b", "d");
        Assert.Equal(2, result); // banana, cherry

        // Test with exclusions
        result = await client.SortedSetLengthByValueAsync(key, "b", "d", Exclude.Both);
        Assert.Equal(2, result);

        // Test with exclusions
        result = await client.SortedSetLengthByValueAsync(key, "banana", "date", Exclude.Both);
        Assert.Equal(1, result); // cherry

        // Test with non-existent key
        result = await client.SortedSetLengthByValueAsync(Guid.NewGuid().ToString(), "a", "z");
        Assert.Equal(0, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetPopMultiKey(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZMPOP is supported since 7.0.0"
        );
        string key1 = $"{{sortedSetKey}}1-{Guid.NewGuid()}";
        string key2 = $"{{sortedSetKey}}2-{Guid.NewGuid()}";
        string emptyKey = $"{{sortedSetKey}}empty-{Guid.NewGuid()}";

        // Setup test data
        await client.SortedSetAddAsync(key1, [
            new("member1", 10.0),
            new("member2", 20.0),
            new("member3", 30.0)
        ]);

        // Test pop min (ascending)
        SortedSetPopResult result = await client.SortedSetPopAsync([key1, key2], 2);
        Assert.False(result.IsNull);
        Assert.Equal(key1, result.Key);
        Assert.Equal(2, result.Entries.Length);
        Assert.Equal("member1", result.Entries[0].Element);
        Assert.Equal(10.0, result.Entries[0].Score);
        Assert.Equal("member2", result.Entries[1].Element);
        Assert.Equal(20.0, result.Entries[1].Score);

        // Test pop max (descending)
        result = await client.SortedSetPopAsync([key1], 1, Order.Descending);
        Assert.False(result.IsNull);
        Assert.Equal(key1, result.Key);
        Assert.Single(result.Entries);
        Assert.Equal("member3", result.Entries[0].Element);
        Assert.Equal(30.0, result.Entries[0].Score);

        // Test pop from empty sets
        result = await client.SortedSetPopAsync([emptyKey], 1);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetScores(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Setup test data
        await client.SortedSetAddAsync(key, [
            new("member1", 10.5),
            new("member2", 20.0),
            new("member3", 30.5)
        ]);

        // Test getting scores for existing members
        double?[] result = await client.SortedSetScoresAsync(key, ["member1", "member2", "member3"]);
        Assert.Equal(3, result.Length);
        Assert.Equal(10.5, result[0]);
        Assert.Equal(20.0, result[1]);
        Assert.Equal(30.5, result[2]);

        // Test getting scores for mix of existing and non-existing members
        result = await client.SortedSetScoresAsync(key, ["member1", "nonexistent", "member3"]);
        Assert.Equal(3, result.Length);
        Assert.Equal(10.5, result[0]);
        Assert.Null(result[1]);
        Assert.Equal(30.5, result[2]);

        // Test with non-existent key
        result = await client.SortedSetScoresAsync(Guid.NewGuid().ToString(), ["member1"]);
        Assert.Single(result);
        Assert.Null(result[0]);

        // Test with empty members array
        await Assert.ThrowsAsync<RequestException>(async () => await client.SortedSetScoresAsync(key, []));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetBlockingPop(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "BZMPOP is supported since 7.0.0"
        );
        string key1 = $"{{testKey}}-{Guid.NewGuid()}";
        string key2 = $"{{testKey}}-{Guid.NewGuid()}";

        // Setup test data
        await client.SortedSetAddAsync(key1, [
            new("member1", 10.0),
            new("member2", 20.0),
            new("member3", 30.0)
        ]);

        // Test single-key blocking pop with MIN order (single element)
        SortedSetEntry? result = await client.SortedSetBlockingPopAsync(key1, Order.Ascending, BlockingTimeoutSecs);
        Assert.NotNull(result);
        Assert.Equal("member1", result.Value.Element);
        Assert.Equal(10.0, result.Value.Score);

        // Test single-key blocking pop with MAX order (single element)
        result = await client.SortedSetBlockingPopAsync(key1, Order.Descending, BlockingTimeoutSecs);
        Assert.NotNull(result);
        Assert.Equal("member3", result.Value.Element);
        Assert.Equal(30.0, result.Value.Score);

        // Test single-key blocking pop with multiple elements
        SortedSetEntry[] multiResult = await client.SortedSetBlockingPopAsync(key1, 1, Order.Ascending, BlockingTimeoutSecs);
        Assert.Single(multiResult);
        Assert.Equal("member2", multiResult[0].Element);
        Assert.Equal(20.0, multiResult[0].Score);

        // Add more test data for multi-key tests
        await client.SortedSetAddAsync(key2, [
            new("member4", 40.0),
            new("member5", 50.0)
        ]);

        // Test multi-key blocking pop with multiple elements
        SortedSetPopResult popResult = await client.SortedSetBlockingPopAsync([key1, key2], 2, Order.Ascending, BlockingTimeoutSecs);
        Assert.False(popResult.IsNull);
        Assert.Equal(key2, popResult.Key);
        Assert.Equal(2, popResult.Entries.Length);
        Assert.Equal("member4", popResult.Entries[0].Element);
        Assert.Equal(40.0, popResult.Entries[0].Score);
        Assert.Equal("member5", popResult.Entries[1].Element);
        Assert.Equal(50.0, popResult.Entries[1].Score);

        // Test timeout with empty keys
        result = await client.SortedSetBlockingPopAsync(key1, Order.Ascending, 0.1);
        Assert.Null(result);

        multiResult = await client.SortedSetBlockingPopAsync(key1, 1, Order.Ascending, 0.1);
        Assert.Empty(multiResult);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetBlockingCommands_NonExistentKeys(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "BZMPOP is supported since 7.0.0"
        );
        string key1 = $"{{testKey}}-{Guid.NewGuid()}";
        string key2 = $"{{testKey}}-{Guid.NewGuid()}";

        // Test single-key blocking pop with non-existent key (should timeout)
        SortedSetEntry? result = await client.SortedSetBlockingPopAsync(key1, Order.Ascending, 0.1);
        Assert.Null(result);

        SortedSetEntry[] multiResult = await client.SortedSetBlockingPopAsync(key1, 1, Order.Ascending, 0.1);
        Assert.Empty(multiResult);

        // Test multi-key blocking pop with non-existent keys (should timeout)
        SortedSetPopResult popResult = await client.SortedSetBlockingPopAsync([key1, key2], 1, Order.Ascending, 0.1);
        Assert.True(popResult.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetPopSingleKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test pop from non-existent key (min by default)
        Assert.Null(await client.SortedSetPopAsync(key));
        await Assert.ThrowsAsync<ArgumentException>(async () => await client.SortedSetPopAsync(key, 2));

        // Test pop from non-existent key (max)
        Assert.Null(await client.SortedSetPopAsync(key, Order.Descending));
        await Assert.ThrowsAsync<ArgumentException>(async () => await client.SortedSetPopAsync(key, 2, Order.Descending));

        // Add test data
        await client.SortedSetAddAsync(key, [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0)
        ]);

        // Test single pop min (default)
        SortedSetEntry? minResult = await client.SortedSetPopAsync(key);
        Assert.NotNull(minResult);
        Assert.Equal("member2", minResult.Value.Element);
        Assert.Equal(8.2, minResult.Value.Score);

        // Test single pop max
        SortedSetEntry? maxResult = await client.SortedSetPopAsync(key, Order.Descending);
        Assert.NotNull(maxResult);
        Assert.Equal("member3", maxResult.Value.Element);
        Assert.Equal(15.0, maxResult.Value.Score);

        // Add more test data for multiple pop tests
        await client.SortedSetAddAsync(key, [
            new("member4", 20.0),
            new("member5", 5.0)
        ]);

        // Test "multiple" pop min (default)
        await Assert.ThrowsAsync<ArgumentException>(async () => await client.SortedSetPopAsync(key, 2));
        SortedSetEntry[] multiMinResult = await client.SortedSetPopAsync(key, 1);
        Assert.Single(multiMinResult);
        Assert.Equal("member5", multiMinResult[0].Element);
        Assert.Equal(5.0, multiMinResult[0].Score);

        // Test "multiple" pop max
        await Assert.ThrowsAsync<ArgumentException>(async () => await client.SortedSetPopAsync(key, 2, Order.Descending));
        SortedSetEntry[] multiMaxResult = await client.SortedSetPopAsync(key, 1, Order.Descending);
        Assert.Single(multiMaxResult);
        Assert.Equal("member4", multiMaxResult[0].Element);
        Assert.Equal(20.0, multiMaxResult[0].Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRandomMember(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test random member from non-existent key
        Assert.Equal(ValkeyValue.Null, await client.SortedSetRandomMemberAsync(key));
        Assert.Empty(await client.SortedSetRandomMembersAsync(key, 2));
        Assert.Empty(await client.SortedSetRandomMembersWithScoresAsync(key, 2));

        // Add test data
        await client.SortedSetAddAsync(key, [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0)
        ]);

        // Test single random member
        ValkeyValue? result = await client.SortedSetRandomMemberAsync(key);
        Assert.NotNull(result);
        Assert.Contains(result.Value.ToString(), new[] { "member1", "member2", "member3" });

        // Test multiple random members
        ValkeyValue[] multiResult = await client.SortedSetRandomMembersAsync(key, 2);
        Assert.Equal(2, multiResult.Length);

        // Test random members with scores
        SortedSetEntry[] scoreResult = await client.SortedSetRandomMembersWithScoresAsync(key, 2);
        Assert.Equal(2, scoreResult.Length);
        Assert.All(scoreResult, entry =>
        {
            Assert.Contains(entry.Element.ToString(), new[] { "member1", "member2", "member3" });
            Assert.True(entry.Score > 0);
        });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeAndStore(BaseClient client)
    {
        string keyPrefix = "{sortedSetKey}-";
        string sourceKey = $"{keyPrefix}source-{Guid.NewGuid()}";
        string destKey = $"{keyPrefix}dest-{Guid.NewGuid()}";

        // Test range and store from non-existent key
        Assert.Equal(0, await client.SortedSetRangeAndStoreAsync(sourceKey, destKey, 0, -1));

        // Add test data
        await client.SortedSetAddAsync(sourceKey, [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0)
        ]);

        // Test range and store by rank (default)
        long result = await client.SortedSetRangeAndStoreAsync(sourceKey, destKey, 0, 1);
        Assert.Equal(2, result);

        ValkeyValue[] stored = await client.SortedSetRangeByRankAsync(destKey);
        Assert.Equal(2, stored.Length);
        Assert.Equal("member2", stored[0]);
        Assert.Equal("member1", stored[1]);

        // Test range and store by score
        string destKey2 = $"{keyPrefix}dest2-{Guid.NewGuid()}";
        result = await client.SortedSetRangeAndStoreAsync(sourceKey, destKey2, 8.0, 11.0, SortedSetOrder.ByScore);
        Assert.Equal(2, result);

        stored = await client.SortedSetRangeByRankAsync(destKey2);
        Assert.Equal(2, stored.Length);
        Assert.Equal("member2", stored[0]);
        Assert.Equal("member1", stored[1]);

        // Test range and store with skip and take
        string destKey3 = $"{keyPrefix}dest3-{Guid.NewGuid()}";
        result = await client.SortedSetRangeAndStoreAsync(sourceKey, destKey3, 0, -1, SortedSetOrder.ByRank, Exclude.None, Order.Ascending, 1, 1);
        Assert.Equal(1, result);

        stored = await client.SortedSetRangeByRankAsync(destKey3);
        Assert.Single(stored);
        Assert.Equal("member1", stored[0]);

        // Test range and store with descending order
        string destKey4 = $"{keyPrefix}dest4-{Guid.NewGuid()}";
        result = await client.SortedSetRangeAndStoreAsync(sourceKey, destKey4, 0, 1, SortedSetOrder.ByRank, Exclude.None, Order.Descending);
        Assert.Equal(2, result);

        stored = await client.SortedSetRangeByRankAsync(destKey4);
        Assert.Equal(2, stored.Length);
        Assert.Equal("member1", stored[0]);
        Assert.Equal("member3", stored[1]);

        // Test range and store by lexicographical order
        string lexSourceKey = $"{keyPrefix}lexsource-{Guid.NewGuid()}";
        string destKey5 = $"{keyPrefix}dest5-{Guid.NewGuid()}";

        // Add test data with same scores for lexicographical ordering
        await client.SortedSetAddAsync(lexSourceKey, [
            new("apple", 1.0),
            new("banana", 1.0),
            new("cherry", 1.0)
        ]);

        result = await client.SortedSetRangeAndStoreAsync(lexSourceKey, destKey5, "a", "c", SortedSetOrder.ByLex);
        Assert.Equal(2, result);

        stored = await client.SortedSetRangeByRankAsync(destKey5);
        Assert.Equal(2, stored.Length);
        Assert.Equal("apple", stored[0]);
        Assert.Equal("banana", stored[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRank(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test rank of non-existent member
        Assert.Null(await client.SortedSetRankAsync(key, "member"));

        // Add test data
        await client.SortedSetAddAsync(key, [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0)
        ]);

        // Test ascending rank (default)
        Assert.Equal(0, await client.SortedSetRankAsync(key, "member2"));
        Assert.Equal(1, await client.SortedSetRankAsync(key, "member1"));
        Assert.Equal(2, await client.SortedSetRankAsync(key, "member3"));
        Assert.Null(await client.SortedSetRankAsync(key, "nonexistent"));

        // Test ascending rank (explicit)
        Assert.Equal(0, await client.SortedSetRankAsync(key, "member2", Order.Ascending));
        Assert.Equal(1, await client.SortedSetRankAsync(key, "member1", Order.Ascending));
        Assert.Equal(2, await client.SortedSetRankAsync(key, "member3", Order.Ascending));

        // Test descending rank (reverse rank)
        Assert.Equal(2, await client.SortedSetRankAsync(key, "member2", Order.Descending));
        Assert.Equal(1, await client.SortedSetRankAsync(key, "member1", Order.Descending));
        Assert.Equal(0, await client.SortedSetRankAsync(key, "member3", Order.Descending));
        Assert.Null(await client.SortedSetRankAsync(key, "nonexistent", Order.Descending));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRemoveRangeByValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test remove from non-existent key
        Assert.Equal(0, await client.SortedSetRemoveRangeByValueAsync(key, "a", "z"));

        // Add test data with same scores for lexicographical ordering
        await client.SortedSetAddAsync(key, [
            new("apple", 1.0),
            new("banana", 1.0),
            new("cherry", 1.0),
            new("date", 1.0)
        ]);

        // Test remove range by value
        long result = await client.SortedSetRemoveRangeByValueAsync(key, "b", "d");
        Assert.Equal(2, result); // banana and cherry

        ValkeyValue[] remaining = await client.SortedSetRangeByRankAsync(key);
        Assert.Equal(2, remaining.Length);
        Assert.Equal("apple", remaining[0]);
        Assert.Equal("date", remaining[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRemoveRangeByRank(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test remove from non-existent key
        Assert.Equal(0, await client.SortedSetRemoveRangeByRankAsync(key, 0, 1));

        // Add test data
        await client.SortedSetAddAsync(key, [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0),
            new("member4", 12.0)
        ]);

        // Test remove range by rank
        long result = await client.SortedSetRemoveRangeByRankAsync(key, 1, 2);
        Assert.Equal(2, result); // member1 and member4

        ValkeyValue[] remaining = await client.SortedSetRangeByRankAsync(key);
        Assert.Equal(2, remaining.Length);
        Assert.Equal("member2", remaining[0]);
        Assert.Equal("member3", remaining[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRemoveRangeByScore(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test remove from non-existent key
        Assert.Equal(0, await client.SortedSetRemoveRangeByScoreAsync(key, 1.0, 10.0));

        // Add test data
        await client.SortedSetAddAsync(key, [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0),
            new("member4", 12.0)
        ]);

        // Test remove range by score
        long result = await client.SortedSetRemoveRangeByScoreAsync(key, 10.0, 13.0);
        Assert.Equal(2, result); // member1 and member4

        ValkeyValue[] remaining = await client.SortedSetRangeByRankAsync(key);
        Assert.Equal(2, remaining.Length);
        Assert.Equal("member2", remaining[0]);
        Assert.Equal("member3", remaining[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetScan(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test scan on non-existent key
        List<SortedSetEntry> items = [];
        await foreach (SortedSetEntry item in client.SortedSetScanAsync(key))
        {
            items.Add(item);
        }
        Assert.Empty(items);

        // Add test data
        await client.SortedSetAddAsync(key, [
            new("member1", 10.5),
            new("member2", 8.2),
            new("member3", 15.0)
        ]);

        // Test scan
        List<SortedSetEntry> scanItems = [];
        await foreach (SortedSetEntry item in client.SortedSetScanAsync(key))
        {
            scanItems.Add(item);
        }
        Assert.Equal(3, scanItems.Count);
        Assert.All(scanItems, item => Assert.Contains(item.Element.ToString(), new[] { "member1", "member2", "member3" }));

        // Test scan with pattern
        List<SortedSetEntry> patternItems = [];
        await foreach (SortedSetEntry item in client.SortedSetScanAsync(key, "member*"))
        {
            patternItems.Add(item);
        }
        Assert.Equal(3, patternItems.Count);
        Assert.All(patternItems, item => Assert.Contains(item.Element.ToString(), new[] { "member1", "member2", "member3" }));

        // Test scan with pageOffset
        List<SortedSetEntry> offsetItems = [];
        await foreach (SortedSetEntry item in client.SortedSetScanAsync(key, pageOffset: 1))
        {
            offsetItems.Add(item);
        }
        Assert.Equal(2, offsetItems.Count); // Should skip the first item
    }


    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetScanAsync_LargeDataset(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Create 25000 members
        SortedSetEntry[] members = [.. Enumerable.Range(0, 25000).Select(i => new SortedSetEntry($"member{i}", i))];
        await client.SortedSetAddAsync(key, members);

        // Test 1: Scan all members with default settings
        List<SortedSetEntry> allScanned = [];
        await foreach (var entry in client.SortedSetScanAsync(key))
        {
            allScanned.Add(entry);
        }
        Assert.Equal(25000, allScanned.Count);

        // Test 2: Scan with pattern matching (should find members 1000-1999)
        List<SortedSetEntry> patternScanned = [];
        await foreach (var entry in client.SortedSetScanAsync(key, "member1*"))
        {
            Assert.StartsWith("member1", entry.Element);
            patternScanned.Add(entry);
        }
        Assert.Equal(11111, patternScanned.Count);  // At least member1, member10-19, member100-199, etc.

        // Test 3: Scan with small page size to test pagination
        List<SortedSetEntry> smallPageScanned = [];
        await foreach (var entry in client.SortedSetScanAsync(key, pageSize: 100))
        {
            smallPageScanned.Add(entry);
        }
        Assert.Equal(25000, smallPageScanned.Count);

        // Test 4: Use pageOffset to skip first 500 results per pagination
        List<SortedSetEntry> offsetResults = [];
        await foreach (var entry in client.SortedSetScanAsync(key, pageSize: 1000, pageOffset: 500))
        {
            offsetResults.Add(entry);
        }
        Assert.Equal(12500, offsetResults.Count);

        Assert.Equal(25000, await client.SortedSetLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetScore(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test score of non-existent member
        Assert.Null(await client.SortedSetScoreAsync(key, "member"));

        // Add test data
        await client.SortedSetAddAsync(key, "member1", 10.5);

        // Test score
        Assert.Equal(10.5, await client.SortedSetScoreAsync(key, "member1"));
        Assert.Null(await client.SortedSetScoreAsync(key, "nonexistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetUnion(BaseClient client)
    {
        string keyPrefix = "{sortedSetKey}-";
        string key1 = $"{keyPrefix}1-{Guid.NewGuid()}";
        string key2 = $"{keyPrefix}2-{Guid.NewGuid()}";

        // Test union with non-existent keys
        Assert.Empty(await client.SortedSetCombineAsync(SetOperation.Union, [key1, key2]));
        Assert.Empty(await client.SortedSetCombineWithScoresAsync(SetOperation.Union, [key1, key2]));

        // Add test data
        await client.SortedSetAddAsync(key1, [
            new("member1", 10.5),
            new("member2", 8.2)
        ]);
        await client.SortedSetAddAsync(key2, [
            new("member2", 5.0),
            new("member3", 15.0)
        ]);

        // Test union
        ValkeyValue[] unionResult = await client.SortedSetCombineAsync(SetOperation.Union, [key1, key2]);
        Assert.Equal(3, unionResult.Length);
        string[] unionStrings = [.. unionResult.Select(v => v.ToString())];
        Assert.Contains("member1", unionStrings);
        Assert.Contains("member2", unionStrings);
        Assert.Contains("member3", unionStrings);

        // Test union with scores
        SortedSetEntry[] unionWithScores = await client.SortedSetCombineWithScoresAsync(SetOperation.Union, [key1, key2]);
        Assert.Equal(3, unionWithScores.Length);
        SortedSetEntry member2Entry = unionWithScores.First(e => e.Element.ToString() == "member2");
        Assert.Equal(13.2, member2Entry.Score); // 8.2 + 5.0
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetUnionAndStore(BaseClient client)
    {
        string keyPrefix = "{sortedSetKey}-";
        string key1 = $"{keyPrefix}1-{Guid.NewGuid()}";
        string key2 = $"{keyPrefix}2-{Guid.NewGuid()}";
        string destKey = $"{keyPrefix}dest-{Guid.NewGuid()}";

        // Test union and store with non-existent keys
        Assert.Equal(0, await client.SortedSetCombineAndStoreAsync(SetOperation.Union, destKey, key1, key2));

        // Add test data
        await client.SortedSetAddAsync(key1, [
            new("member1", 10.5),
            new("member2", 8.2)
        ]);
        await client.SortedSetAddAsync(key2, [
            new("member2", 5.0),
            new("member3", 15.0)
        ]);

        // Test union and store
        long result = await client.SortedSetCombineAndStoreAsync(SetOperation.Union, destKey, key1, key2);
        Assert.Equal(3, result);

        ValkeyValue[] stored = await client.SortedSetRangeByRankAsync(destKey);
        Assert.Equal(3, stored.Length);
        string[] storedStrings = [.. stored.Select(v => v.ToString())];
        Assert.Contains("member1", storedStrings);
        Assert.Contains("member2", storedStrings);
        Assert.Contains("member3", storedStrings);
    }
}
