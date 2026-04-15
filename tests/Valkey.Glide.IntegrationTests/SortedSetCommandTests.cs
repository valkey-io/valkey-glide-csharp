// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class SortedSetCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;
    private static readonly TimeSpan BlockingTimeout = TimeSpan.FromSeconds(2);

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
        var members = new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
        };

        // Test adding multiple new members
        Assert.Equal(3, await client.SortedSetAddAsync(key, members));

        // Test adding mix of new and existing members
        var newMembers = new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 20.0, // Update existing
            ["member4"] = 12.0, // Add new
        };
        Assert.Equal(1, await client.SortedSetAddAsync(key, newMembers)); // Only member4 is new
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithNotExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial member
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));

        // Try to add existing member with NX (should fail)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetAddCondition.OnlyIfNotExists));

        // Add new member with NX (should succeed)
        Assert.True(await client.SortedSetAddAsync(key, "member2", 8.0, SortedSetAddCondition.OnlyIfNotExists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Try to update non-existing member with XX (should fail)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 10.5, SortedSetAddCondition.OnlyIfExists));

        // Add member normally first
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));

        // Update existing member with XX (should succeed)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetAddCondition.OnlyIfExists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithGreaterThan(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial member
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.0));

        // Update with higher score using GT (should succeed)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetAddCondition.OnlyIfNotExistsOrGreaterThan));

        // Try to update with lower score using GT (should fail)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 5.0, SortedSetAddCondition.OnlyIfNotExistsOrGreaterThan));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithLessThan(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial member
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.0));

        // Update with lower score using LT (should succeed)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 5.0, SortedSetAddCondition.OnlyIfNotExistsOrLessThan));

        // Try to update with higher score using LT (should fail)
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetAddCondition.OnlyIfNotExistsOrLessThan));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_MultipleWithConditions(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Add initial members
        var initialMembers = new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 8.0,
        };
        Assert.Equal(2, await client.SortedSetAddAsync(key, initialMembers));

        // Try to add with NX (should only add new members)
        var nxMembers = new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 15.0, // Existing, should not update
            ["member3"] = 12.0, // New, should add
        };
        Assert.Equal(1, await client.SortedSetAddAsync(key, nxMembers, SortedSetAddCondition.OnlyIfNotExists));

        // Update existing members with XX
        var xxMembers = new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 20.0, // Existing, should update
            ["member4"] = 5.0,  // New, should not add
        };
        Assert.Equal(0, await client.SortedSetAddAsync(key, xxMembers, SortedSetAddCondition.OnlyIfExists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_NegativeScores(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test with negative scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", -10.5));
        Assert.True(await client.SortedSetAddAsync(key, "member2", -5.0));

        var members = new Dictionary<ValkeyValue, double>
        {
            ["member3"] = -15.0,
            ["member4"] = 0.0
        };

        Assert.Equal(2, await client.SortedSetAddAsync(key, members));
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
        => _ = await Assert.ThrowsAsync<RequestException>(async ()
            => await client.SortedSetAddAsync(Guid.NewGuid().ToString(), new Dictionary<ValkeyValue, double>()));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetAdd_WithOptions(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test overload with default options
        Assert.True(await client.SortedSetAddAsync(key, "member1", 10.5));

        // Test with OnlyIfExists option
        Assert.False(await client.SortedSetAddAsync(key, "member1", 15.0, SortedSetAddCondition.OnlyIfExists));

        // Test multi-member overload
        var members = new Dictionary<ValkeyValue, double> { ["member2"] = 8.0 };
        Assert.Equal(1, await client.SortedSetAddAsync(key, members));
        Assert.Equal(0, await client.SortedSetAddAsync(key, members, SortedSetAddCondition.OnlyIfExists));
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
        var entries = new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
            ["member4"] = 12.0,
        };
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
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.SortedSetRemoveAsync(key, []));

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
        Assert.Equal(0, await client.SortedSetCardAsync(key));
        Assert.Equal(0, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0))));

        // Add members with different scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.5));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 5.0));
        Assert.True(await client.SortedSetAddAsync(key, "member4", 8.0));

        // Test cardinality (ZCARD)
        Assert.Equal(4, await client.SortedSetCardAsync(key));

        // Test count with range parameters (ZCOUNT)
        Assert.Equal(2, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Inclusive(2.0), ScoreBound.Inclusive(6.0))));
        Assert.Equal(1, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Exclusive(2.5), ScoreBound.Inclusive(5.0))));
        Assert.Equal(1, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Inclusive(2.5), ScoreBound.Exclusive(5.0))));
        Assert.Equal(0, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Exclusive(2.5), ScoreBound.Exclusive(5.0))));

        // Test with no matches
        Assert.Equal(0, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Inclusive(15.0), ScoreBound.Inclusive(20.0))));

        // Remove a member and test both modes
        Assert.True(await client.SortedSetRemoveAsync(key, "member2"));
        Assert.Equal(3, await client.SortedSetCardAsync(key));
        Assert.Equal(1, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Inclusive(2.0), ScoreBound.Inclusive(6.0))));
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
        Assert.Equal(0, await client.SortedSetCountAsync(key, ScoreRange.All));

        // Add members with different scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.5));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 5.0));
        Assert.True(await client.SortedSetAddAsync(key, "member4", 10.0));

        // Test count with default range (all elements)
        Assert.Equal(4, await client.SortedSetCountAsync(key, ScoreRange.All));

        // Test count with specific range
        Assert.Equal(2, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Inclusive(2.0), ScoreBound.Inclusive(6.0))));

        // Test count with exclusive bounds
        Assert.Equal(1, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Exclusive(2.5), ScoreBound.Inclusive(5.0))));  // Exclude member2 (2.5), include member3 (5.0)
        Assert.Equal(1, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Inclusive(2.5), ScoreBound.Exclusive(5.0))));   // Include member2 (2.5), exclude member3 (5.0)
        Assert.Equal(0, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Exclusive(2.5), ScoreBound.Exclusive(5.0))));   // Exclude both member2 and member3

        // Test count with infinity bounds
        Assert.Equal(4, await client.SortedSetCountAsync(key, ScoreRange.All));

        // Test count with no matches
        Assert.Equal(0, await client.SortedSetCountAsync(key, ScoreRange.Between(ScoreBound.Inclusive(15.0), ScoreBound.Inclusive(20.0))));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByRankAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        ValkeyValue[] result = await client.SortedSetRangeAsync(key);
        Assert.Empty(result);

        // Add members with scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.0));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 3.0));
        Assert.True(await client.SortedSetAddAsync(key, "member4", 4.0));

        // Test default range (all elements, ascending)
        result = await client.SortedSetRangeAsync(key);
        Assert.Equal(4, result.Length);
        Assert.Equal("member1", result[0]);
        Assert.Equal("member2", result[1]);
        Assert.Equal("member3", result[2]);
        Assert.Equal("member4", result[3]);

        // Test specific range
        result = await client.SortedSetRangeAsync(key, new() { Range = RankRange.Between(1, 2) });
        Assert.Equal(2, result.Length);
        Assert.Equal("member2", result[0]);
        Assert.Equal("member3", result[1]);

        // Test descending order
        result = await client.SortedSetRangeAsync(key, new() { Range = RankRange.Between(0, 1), Order = Order.Descending });
        Assert.Equal(2, result.Length);
        Assert.Equal("member4", result[0]);
        Assert.Equal("member3", result[1]);

        // Test negative indices
        result = await client.SortedSetRangeAsync(key, new() { Range = RankRange.Between(-2, -1) });
        Assert.Equal(2, result.Length);
        Assert.Equal("member3", result[0]);
        Assert.Equal("member4", result[1]);

        // Test single element range
        result = await client.SortedSetRangeAsync(key, new() { Range = RankRange.Between(0, 0) });
        _ = Assert.Single(result);
        Assert.Equal("member1", result[0]);

        // Test out of range
        result = await client.SortedSetRangeAsync(key, new() { Range = RankRange.Between(10, 20) });
        Assert.Empty(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByRankWithScoresAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        SortedSetScoreResult[] result = await client.SortedSetRangeWithScoresAsync(key);
        Assert.Empty(result);

        // Add members with scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.5));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.5));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 3.5));

        // Test default range (all elements, ascending)
        result = await client.SortedSetRangeWithScoresAsync(key);
        Assert.Equal(3, result.Length);
        Assert.Equal("member1", result[0].Member);
        Assert.Equal(1.5, result[0].Score);
        Assert.Equal("member2", result[1].Member);
        Assert.Equal(2.5, result[1].Score);
        Assert.Equal("member3", result[2].Member);
        Assert.Equal(3.5, result[2].Score);

        // Test specific range
        result = await client.SortedSetRangeWithScoresAsync(key, new() { Range = RankRange.Between(0, 1) });
        Assert.Equal(2, result.Length);
        Assert.Equal("member1", result[0].Member);
        Assert.Equal(1.5, result[0].Score);
        Assert.Equal("member2", result[1].Member);
        Assert.Equal(2.5, result[1].Score);

        // Test descending order
        result = await client.SortedSetRangeWithScoresAsync(key, new() { Range = RankRange.Between(0, 1), Order = Order.Descending });
        Assert.Equal(2, result.Length);
        Assert.Equal("member3", result[0].Member);
        Assert.Equal(3.5, result[0].Score);
        Assert.Equal("member2", result[1].Member);
        Assert.Equal(2.5, result[1].Score);

        // Test single element
        result = await client.SortedSetRangeWithScoresAsync(key, new() { Range = RankRange.Between(1, 1) });
        _ = Assert.Single(result);
        Assert.Equal("member2", result[0].Member);
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
        ValkeyValue[] result = await client.SortedSetRangeAsync(key);
        Assert.Equal(3, result.Length);
        Assert.Equal("neginf", result[0]);
        Assert.Equal("zero", result[1]);
        Assert.Equal("posinf", result[2]);

        // Test with scores
        SortedSetScoreResult[] resultWithScores = await client.SortedSetRangeWithScoresAsync(key);
        Assert.Equal(3, resultWithScores.Length);
        Assert.Equal("neginf", resultWithScores[0].Member);
        Assert.True(double.IsNegativeInfinity(resultWithScores[0].Score));
        Assert.Equal("zero", resultWithScores[1].Member);
        Assert.Equal(0.0, resultWithScores[1].Score);
        Assert.Equal("posinf", resultWithScores[2].Member);
        Assert.True(double.IsPositiveInfinity(resultWithScores[2].Score));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByScoreWithScoresAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        SortedSetScoreResult[] result = await client.SortedSetRangeWithScoresAsync(key, new() { Range = ScoreRange.All });
        Assert.Empty(result);

        // Add members with scores
        Assert.True(await client.SortedSetAddAsync(key, "member1", 1.0));
        Assert.True(await client.SortedSetAddAsync(key, "member2", 2.5));
        Assert.True(await client.SortedSetAddAsync(key, "member3", 5.0));
        Assert.True(await client.SortedSetAddAsync(key, "member4", 10.0));

        // Test default range (all elements, ascending)
        result = await client.SortedSetRangeWithScoresAsync(key, new() { Range = ScoreRange.All });
        Assert.Equal(4, result.Length);
        Assert.Equal("member1", result[0].Member);
        Assert.Equal(1.0, result[0].Score);
        Assert.Equal("member2", result[1].Member);
        Assert.Equal(2.5, result[1].Score);
        Assert.Equal("member3", result[2].Member);
        Assert.Equal(5.0, result[2].Score);
        Assert.Equal("member4", result[3].Member);
        Assert.Equal(10.0, result[3].Score);

        // Test specific score range
        result = await client.SortedSetRangeWithScoresAsync(key, new()
        {
            Range = ScoreRange.Between(ScoreBound.Inclusive(2.0), ScoreBound.Inclusive(6.0))
        });
        Assert.Equal(2, result.Length);
        Assert.Equal("member2", result[0].Member);
        Assert.Equal(2.5, result[0].Score);
        Assert.Equal("member3", result[1].Member);
        Assert.Equal(5.0, result[1].Score);

        // Test descending order
        result = await client.SortedSetRangeWithScoresAsync(key, new()
        {
            Range = ScoreRange.Between(ScoreBound.Inclusive(2.0), ScoreBound.Inclusive(6.0)),
            Order = Order.Descending
        });
        Assert.Equal(2, result.Length);
        Assert.Equal("member3", result[0].Member);
        Assert.Equal(5.0, result[0].Score);
        Assert.Equal("member2", result[1].Member);
        Assert.Equal(2.5, result[1].Score);

        // Test with exclusions
        result = await client.SortedSetRangeWithScoresAsync(key, new()
        {
            Range = ScoreRange.Between(ScoreBound.Exclusive(2.5), ScoreBound.Inclusive(5.0))
        });
        _ = Assert.Single(result);
        Assert.Equal("member3", result[0].Member);
        Assert.Equal(5.0, result[0].Score);

        // Test with limit
        result = await client.SortedSetRangeWithScoresAsync(key, new()
        {
            Range = ScoreRange.All,
            Offset = 1,
            Count = 2
        });
        Assert.Equal(2, result.Length);
        Assert.Equal("member2", result[0].Member);
        Assert.Equal(2.5, result[0].Score);
        Assert.Equal("member3", result[1].Member);
        Assert.Equal(5.0, result[1].Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeByValueAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existent key
        ValkeyValue[] result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z"))
        });
        Assert.Empty(result);

        // Add members with same score for lexicographical ordering
        Assert.True(await client.SortedSetAddAsync(key, "apple", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "banana", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "cherry", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "date", 0.0));

        // Test specific range
        result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.Between(LexBound.Inclusive("b"), LexBound.Inclusive("d"))
        });
        Assert.Equal(2, result.Length);
        Assert.Equal("banana", result[0]);
        Assert.Equal("cherry", result[1]);

        // Test with exclusions
        result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.Between(LexBound.Exclusive("banana"), LexBound.Inclusive("cherry"))
        });
        _ = Assert.Single(result);
        Assert.Equal("cherry", result[0]);

        // Test with limit
        result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z")),
            Offset = 1,
            Count = 2
        });
        Assert.Equal(2, result.Length);
        Assert.Equal("banana", result[0]);
        Assert.Equal("cherry", result[1]);

        // Test full range
        result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.All
        });
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
        ValkeyValue[] result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.All,
            Order = Order.Descending
        });
        Assert.Empty(result);

        // Add members with same score for lexicographical ordering
        Assert.True(await client.SortedSetAddAsync(key, "apple", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "banana", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "cherry", 0.0));
        Assert.True(await client.SortedSetAddAsync(key, "date", 0.0));

        // Test ascending order (default)
        result = await client.SortedSetRangeAsync(key, new() { Range = LexRange.All });
        Assert.Equal(4, result.Length);
        Assert.Equal("apple", result[0]);
        Assert.Equal("banana", result[1]);
        Assert.Equal("cherry", result[2]);
        Assert.Equal("date", result[3]);

        // Test descending order
        result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.All,
            Order = Order.Descending
        });
        Assert.Equal(4, result.Length);
        Assert.Equal("date", result[0]);
        Assert.Equal("cherry", result[1]);
        Assert.Equal("banana", result[2]);
        Assert.Equal("apple", result[3]);

        // Test specific range with descending order
        result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.Between(LexBound.Inclusive("b"), LexBound.Inclusive("d")),
            Order = Order.Descending
        });
        Assert.Equal(2, result.Length);
        Assert.Equal("cherry", result[0]);
        Assert.Equal("banana", result[1]);

        // Test with limit and descending order
        result = await client.SortedSetRangeAsync(key, new()
        {
            Range = LexRange.All,
            Order = Order.Descending,
            Offset = 1,
            Count = 2
        });
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
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
        });
        _ = await client.SortedSetAddAsync(key2, new Dictionary<ValkeyValue, double>
        {
            ["member2"] = 15.0,
            ["member3"] = 25.0,
        });

        // Test union
        ValkeyValue[] result = await client.SortedSetUnionAsync([key1, key2]);
        Assert.Equal(3, result.Length);
        Assert.Contains((ValkeyValue)"member1", result);
        Assert.Contains((ValkeyValue)"member2", result);
        Assert.Contains((ValkeyValue)"member3", result);

        // Test intersection
        result = await client.SortedSetInterAsync([key1, key2]);
        _ = Assert.Single(result);
        Assert.Contains((ValkeyValue)"member2", result);

        // Test difference
        result = await client.SortedSetDiffAsync([key1, key2]);
        _ = Assert.Single(result);
        Assert.Contains((ValkeyValue)"member1", result);

        // Test with non-existent key
        result = await client.SortedSetUnionAsync([key1, key3]);
        Assert.Equal(2, result.Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetCombineWithScores(BaseClient client)
    {
        string key1 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key2 = $"{{sortedSetKey}}-{Guid.NewGuid()}";

        // Setup test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
        });
        _ = await client.SortedSetAddAsync(key2, new Dictionary<ValkeyValue, double>
        {
            ["member2"] = 15.0,
            ["member3"] = 25.0,
        });

        // Test union with scores
        SortedSetScoreResult[] scoreResults = await client.SortedSetUnionWithScoreAsync([key1, key2]);
        Assert.Equal(3, scoreResults.Length);

        // Test intersection with scores
        scoreResults = await client.SortedSetInterWithScoreAsync([key1, key2]);
        _ = Assert.Single(scoreResults);
        Assert.Equal(35.0, scoreResults[0].Score); // Sum aggregation: 20 + 15
        Assert.Equal("member2", scoreResults[0].Member);

        // Test with weights
        scoreResults = await client.SortedSetUnionWithScoreAsync(new Dictionary<ValkeyKey, double> { [key1] = 2.0, [key2] = 0.5 });
        Assert.Equal(3, scoreResults.Length);
        SortedSetScoreResult member2 = scoreResults.First(r => r.Member == "member2");
        Assert.Equal(47.5, member2.Score); // (20 * 2) + (15 * 0.5)
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetCombineAndStore(BaseClient client)
    {
        string key1 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key2 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string destKey = $"{{sortedSetKey}}-{Guid.NewGuid()}";

        // Setup test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
        });
        _ = await client.SortedSetAddAsync(key2, new Dictionary<ValkeyValue, double>
        {
            ["member2"] = 15.0,
            ["member3"] = 25.0,
        });

        // Test union and store
        long result = await client.SortedSetUnionAndStoreAsync(destKey, [key1, key2]);
        Assert.Equal(3, result);

        // Verify stored result
        long count = await client.SortedSetCardAsync(destKey);
        Assert.Equal(3, count);

        // Test intersection and store with multiple keys
        result = await client.SortedSetInterAndStoreAsync(destKey, [key1, key2]);
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
        Assert.Equal(10.5, await client.SortedSetIncrementByAsync(key, "member1", 10.5));

        // Test increment on existing member
        Assert.Equal(15.5, await client.SortedSetIncrementByAsync(key, "member1", 5.0));

        // Test negative increment
        Assert.Equal(12.5, await client.SortedSetIncrementByAsync(key, "member1", -3.0));

        // Test increment by zero
        Assert.Equal(12.5, await client.SortedSetIncrementByAsync(key, "member1", 0.0));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetInterCard(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZINTERCARD is supported since 7.0.0"
        );
        string key1 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key2 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string key3 = $"{{sortedSetKey}}-{Guid.NewGuid()}";
        string emptyKey = $"{{sortedSetKey}}-{Guid.NewGuid()}";

        // Setup test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
            ["member3"] = 30.0,
        });
        _ = await client.SortedSetAddAsync(key2, new Dictionary<ValkeyValue, double>
        {
            ["member2"] = 15.0,
            ["member3"] = 25.0,
            ["member4"] = 35.0,
        });
        _ = await client.SortedSetAddAsync(key3, new Dictionary<ValkeyValue, double>
        {
            ["member3"] = 40.0,
            ["member5"] = 50.0,
        });

        // Test intersection of two sets
        long result = await client.SortedSetInterCardAsync([key1, key2]);
        Assert.Equal(2, result); // member2, member3

        // Test intersection of three sets
        result = await client.SortedSetInterCardAsync([key1, key2, key3]);
        Assert.Equal(1, result); // member3

        // Test with limit
        result = await client.SortedSetInterCardAsync([key1, key2], 1);
        Assert.Equal(1, result);

        // Test with non-existent key
        result = await client.SortedSetInterCardAsync([key1, emptyKey]);
        Assert.Equal(0, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetLexCount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Setup test data with same scores for lexicographical ordering
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["apple"] = 0.0,
            ["banana"] = 0.0,
            ["cherry"] = 0.0,
            ["date"] = 0.0,
        });

        // Test full range
        long result = await client.SortedSetLexCountAsync(key, LexRange.Between(
            LexBound.Inclusive("a"), LexBound.Inclusive("z")));
        Assert.Equal(4, result);

        // Test specific range
        result = await client.SortedSetLexCountAsync(key, LexRange.Between(
            LexBound.Inclusive("b"), LexBound.Inclusive("d")));
        Assert.Equal(2, result); // banana, cherry

        // Test with exclusions
        result = await client.SortedSetLexCountAsync(key, LexRange.Between(
            LexBound.Exclusive("b"), LexBound.Exclusive("d")));
        Assert.Equal(2, result);

        // Test with exclusions
        result = await client.SortedSetLexCountAsync(key, LexRange.Between(
            LexBound.Exclusive("banana"), LexBound.Exclusive("date")));
        Assert.Equal(1, result); // cherry

        // Test with non-existent key
        result = await client.SortedSetLexCountAsync(Guid.NewGuid().ToString(), LexRange.Between(
            LexBound.Inclusive("a"), LexBound.Inclusive("z")));
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
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
            ["member3"] = 30.0,
        });

        // Test pop min (ascending)
        var result = await client.SortedSetPopMinAsync([key1, key2], 2);
        Assert.False(result.IsNull);
        Assert.Equal(key1, result.Key);
        Assert.Equal(2, result.Entries.Length);
        Assert.Equal("member1", result.Entries[0].Element);
        Assert.Equal(10.0, result.Entries[0].Score);
        Assert.Equal("member2", result.Entries[1].Element);
        Assert.Equal(20.0, result.Entries[1].Score);

        // Test pop max (descending)
        result = await client.SortedSetPopMaxAsync([key1], 1);
        Assert.False(result.IsNull);
        Assert.Equal(key1, result.Key);
        _ = Assert.Single(result.Entries);
        Assert.Equal("member3", result.Entries[0].Element);
        Assert.Equal(30.0, result.Entries[0].Score);

        // Test pop from empty sets
        result = await client.SortedSetPopMinAsync([emptyKey], 1);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetScores(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Setup test data
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 20.0,
            ["member3"] = 30.5,
        });

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
        _ = Assert.Single(result);
        Assert.Null(result[0]);

        // Test with empty members array
        _ = await Assert.ThrowsAsync<RequestException>(async () => await client.SortedSetScoresAsync(key, []));
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
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
            ["member3"] = 30.0,
        });

        // Test single-key blocking pop with MIN order (single element)
        SortedSetScoreResult result = Assert.NotNull(await client.SortedSetPopMinAsync([key1], BlockingTimeout));
        Assert.Equal("member1", result.Member);
        Assert.Equal(10.0, result.Score);

        // Test single-key blocking pop with MAX order (single element)
        result = Assert.NotNull(await client.SortedSetPopMaxAsync([key1], BlockingTimeout));
        Assert.Equal("member3", result.Member);
        Assert.Equal(30.0, result.Score);

        // Test single-key blocking pop with next element
        result = Assert.NotNull(await client.SortedSetPopMinAsync([key1], BlockingTimeout));
        Assert.Equal("member2", result.Member);
        Assert.Equal(20.0, result.Score);

        // Add more test data for multi-key tests
        _ = await client.SortedSetAddAsync(key2, new Dictionary<ValkeyValue, double>
        {
            ["member4"] = 40.0,
            ["member5"] = 50.0,
        });

        // Test multi-key blocking pop with multiple elements
        SortedSetPopResult popResult = await client.SortedSetPopMinAsync([key1, key2], 2, BlockingTimeout);
        Assert.False(popResult.IsNull);
        Assert.Equal(key2, popResult.Key);
        Assert.Equal(2, popResult.Entries.Length);
        Assert.Equal("member4", popResult.Entries[0].Element);
        Assert.Equal(40.0, popResult.Entries[0].Score);
        Assert.Equal("member5", popResult.Entries[1].Element);
        Assert.Equal(50.0, popResult.Entries[1].Score);

        // Test timeout with empty keys
        Assert.Null(await client.SortedSetPopMinAsync([key1], BlockingTimeout));
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
        SortedSetScoreResult? result = await client.SortedSetPopMinAsync([key1], BlockingTimeout);
        Assert.Null(result);

        // Test multi-key blocking pop with non-existent keys (should timeout)
        SortedSetPopResult popResult = await client.SortedSetPopMinAsync([key1, key2], 1, BlockingTimeout);
        Assert.True(popResult.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetPopSingleKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test pop from non-existent key (min)
        Assert.Null(await client.SortedSetPopMinAsync(key));

        // Test pop from non-existent key (max)
        Assert.Null(await client.SortedSetPopMaxAsync(key));

        // Add test data
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
        });

        // Test single pop min
        SortedSetScoreResult? minResult = await client.SortedSetPopMinAsync(key);
        _ = Assert.NotNull(minResult);
        Assert.Equal("member2", minResult.Value.Member);
        Assert.Equal(8.2, minResult.Value.Score);

        // Test single pop max
        SortedSetScoreResult? maxResult = await client.SortedSetPopMaxAsync(key);
        _ = Assert.NotNull(maxResult);
        Assert.Equal("member3", maxResult.Value.Member);
        Assert.Equal(15.0, maxResult.Value.Score);

        // Add more test data for multiple pop tests
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member4"] = 20.0,
            ["member5"] = 5.0,
        });

        // Test multiple pop min
        SortedSetScoreResult[] multiMinResult = await client.SortedSetPopMinAsync(key, 2);
        Assert.Equal(2, multiMinResult.Length);
        Assert.Equal("member5", multiMinResult[0].Member);
        Assert.Equal(5.0, multiMinResult[0].Score);

        // Test multiple pop max
        SortedSetScoreResult[] multiMaxResult = await client.SortedSetPopMaxAsync(key, 2);
        Assert.Equal(2, multiMaxResult.Length);
        Assert.Equal("member4", multiMaxResult[0].Member);
        Assert.Equal(20.0, multiMaxResult[0].Score);

        // Test pop from empty set
        Assert.Null(await client.SortedSetPopMinAsync(key));
        Assert.Null(await client.SortedSetPopMaxAsync(key));
        Assert.Empty(await client.SortedSetPopMinAsync(key, 1));
        Assert.Empty(await client.SortedSetPopMaxAsync(key, 1));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRandomMember(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test random member from non-existent key
        Assert.Equal(ValkeyValue.Null, await client.SortedSetRandomMemberAsync(key));
        Assert.Empty(await client.SortedSetRandomMembersAsync(key, 2));

        // Add test data
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
        });

        // Test single random member
        ValkeyValue? result = await client.SortedSetRandomMemberAsync(key);
        _ = Assert.NotNull(result);
        Assert.Contains(result.Value.ToString(), new[] { "member1", "member2", "member3" });

        // Test multiple random members
        ValkeyValue[] multiResult = await client.SortedSetRandomMembersAsync(key, 2);
        Assert.Equal(2, multiResult.Length);

        // Test random member with score (GLIDE-native)
        SortedSetScoreResult? scoreResult = await client.SortedSetRandomMemberWithScoreAsync(key);
        _ = Assert.NotNull(scoreResult);
        Assert.Contains(scoreResult.Value.Member.ToString(), new[] { "member1", "member2", "member3" });
        Assert.True(scoreResult.Value.Score > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRangeAndStore(BaseClient client)
    {
        string keyPrefix = "{sortedSetKey}-";
        string sourceKey = $"{keyPrefix}source-{Guid.NewGuid()}";
        string destKey = $"{keyPrefix}dest-{Guid.NewGuid()}";

        // Test range and store from non-existent key
        Assert.Equal(0, await client.SortedSetRangeAndStoreAsync(sourceKey, destKey));

        // Add test data
        _ = await client.SortedSetAddAsync(sourceKey, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
        });

        // Test range and store by rank (default)
        long result = await client.SortedSetRangeAndStoreAsync(sourceKey, destKey, new() { Range = RankRange.Between(0, 1) });
        Assert.Equal(2, result);

        ValkeyValue[] stored = await client.SortedSetRangeAsync(destKey);
        Assert.Equal(2, stored.Length);
        Assert.Equal("member2", stored[0]);
        Assert.Equal("member1", stored[1]);

        // Test range and store by score
        string destKey2 = $"{keyPrefix}dest2-{Guid.NewGuid()}";
        result = await client.SortedSetRangeAndStoreAsync(sourceKey, destKey2, new()
        {
            Range = ScoreRange.Between(ScoreBound.Inclusive(8.0), ScoreBound.Inclusive(11.0))
        });
        Assert.Equal(2, result);

        stored = await client.SortedSetRangeAsync(destKey2);
        Assert.Equal(2, stored.Length);
        Assert.Equal("member2", stored[0]);
        Assert.Equal("member1", stored[1]);

        // Test range and store with offset/count
        string destKey3 = $"{keyPrefix}dest3-{Guid.NewGuid()}";
        result = await client.SortedSetRangeAndStoreAsync(sourceKey, destKey3, new()
        {
            Range = ScoreRange.All,
            Offset = 1,
            Count = 1
        });
        Assert.Equal(1, result);

        stored = await client.SortedSetRangeAsync(destKey3);
        _ = Assert.Single(stored);
        Assert.Equal("member1", stored[0]);

        // Test range and store with descending order
        string destKey4 = $"{keyPrefix}dest4-{Guid.NewGuid()}";
        result = await client.SortedSetRangeAndStoreAsync(sourceKey, destKey4, new()
        {
            Range = RankRange.Between(0, 1),
            Order = Order.Descending
        });
        Assert.Equal(2, result);

        stored = await client.SortedSetRangeAsync(destKey4);
        Assert.Equal(2, stored.Length);
        Assert.Equal("member1", stored[0]);
        Assert.Equal("member3", stored[1]);

        // Test range and store by lexicographical order
        string lexSourceKey = $"{keyPrefix}lexsource-{Guid.NewGuid()}";
        string destKey5 = $"{keyPrefix}dest5-{Guid.NewGuid()}";

        // Add test data with same scores for lexicographical ordering
        _ = await client.SortedSetAddAsync(lexSourceKey, new Dictionary<ValkeyValue, double>
        {
            ["apple"] = 1.0,
            ["banana"] = 1.0,
            ["cherry"] = 1.0
        });

        result = await client.SortedSetRangeAndStoreAsync(lexSourceKey, destKey5, new()
        {
            Range = LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("c"))
        });
        Assert.Equal(2, result);

        stored = await client.SortedSetRangeAsync(destKey5);
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
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0
        });

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
        Assert.Equal(0, await client.SortedSetRemoveRangeAsync(key,
            LexRange.Between(LexBound.Inclusive("a"), LexBound.Inclusive("z"))));

        // Add test data with same scores for lexicographical ordering
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["apple"] = 1.0,
            ["banana"] = 1.0,
            ["cherry"] = 1.0,
            ["date"] = 1.0,
        });

        // Test remove range by value
        long result = await client.SortedSetRemoveRangeAsync(key,
            LexRange.Between(LexBound.Inclusive("b"), LexBound.Inclusive("d")));
        Assert.Equal(2, result); // banana and cherry

        ValkeyValue[] remaining = await client.SortedSetRangeAsync(key);
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
        Assert.Equal(0, await client.SortedSetRemoveRangeAsync(key, RankRange.Between(0, 1)));

        // Add test data
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
            ["member4"] = 12.0,
        });

        // Test remove range by rank
        long result = await client.SortedSetRemoveRangeAsync(key, RankRange.Between(1, 2));
        Assert.Equal(2, result); // member1 and member4

        ValkeyValue[] remaining = await client.SortedSetRangeAsync(key);
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
        Assert.Equal(0, await client.SortedSetRemoveRangeAsync(key,
            ScoreRange.Between(ScoreBound.Inclusive(1.0), ScoreBound.Inclusive(10.0))));

        // Add test data
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
            ["member4"] = 12.0,
        });

        // Test remove range by score
        long result = await client.SortedSetRemoveRangeAsync(key,
            ScoreRange.Between(ScoreBound.Inclusive(10.0), ScoreBound.Inclusive(13.0)));
        Assert.Equal(2, result); // member1 and member4

        ValkeyValue[] remaining = await client.SortedSetRangeAsync(key);
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
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
        });

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
        var members = Enumerable.Range(0, 25000).ToDictionary(i => (ValkeyValue)$"member{i}", i => (double)i);
        _ = await client.SortedSetAddAsync(key, members);

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

        Assert.Equal(25000, await client.SortedSetCardAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetScore(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test score of non-existent member
        Assert.Null(await client.SortedSetScoreAsync(key, "member"));

        // Add test data
        _ = await client.SortedSetAddAsync(key, "member1", 10.5);

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
        Assert.Empty(await client.SortedSetUnionAsync([key1, key2]));
        Assert.Empty(await client.SortedSetUnionWithScoreAsync([key1, key2]));

        // Add test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
        });
        _ = await client.SortedSetAddAsync(key2, new Dictionary<ValkeyValue, double>
        {
            ["member2"] = 5.0,
            ["member3"] = 15.0,
        });

        // Test union
        ValkeyValue[] unionResult = await client.SortedSetUnionAsync([key1, key2]);
        Assert.Equal(3, unionResult.Length);
        Assert.Contains((ValkeyValue)"member1", unionResult);
        Assert.Contains((ValkeyValue)"member2", unionResult);
        Assert.Contains((ValkeyValue)"member3", unionResult);

        // Test union with scores
        SortedSetScoreResult[] unionWithScores = await client.SortedSetUnionWithScoreAsync([key1, key2]);
        Assert.Equal(3, unionWithScores.Length);
        SortedSetScoreResult member2Entry = unionWithScores.First(r => r.Member == "member2");
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
        Assert.Equal(0, await client.SortedSetUnionAndStoreAsync(destKey, [key1, key2]));

        // Add test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
        });
        _ = await client.SortedSetAddAsync(key2, new Dictionary<ValkeyValue, double>
        {
            ["member2"] = 5.0,
            ["member3"] = 15.0,
        });

        // Test union and store
        long result = await client.SortedSetUnionAndStoreAsync(destKey, [key1, key2]);
        Assert.Equal(3, result);

        ValkeyValue[] stored = await client.SortedSetRangeAsync(destKey);
        Assert.Equal(3, stored.Length);
        string[] storedStrings = [.. stored.Select(v => v.ToString())];
        Assert.Contains("member1", storedStrings);
        Assert.Contains("member2", storedStrings);
        Assert.Contains("member3", storedStrings);
    }

    #region Multi-Key Pop (ZMPOP)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetPopMinAsync_MultiKey(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZMPOP is supported since 7.0.0");

        string key1 = $"{{testKey}}-{Guid.NewGuid()}";
        string key2 = $"{{testKey}}-{Guid.NewGuid()}";

        // Pop from non-existent keys
        ValkeyKey[] keys = [key1, key2];
        Assert.Null(await client.SortedSetPopMinAsync(keys));

        // Add test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
        });

        // Pop single min from multi-key
        SortedSetScoreResult? result = await client.SortedSetPopMinAsync(keys);
        _ = Assert.NotNull(result);
        Assert.Equal("member1", result.Value.Member);
        Assert.Equal(10.0, result.Value.Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetPopMaxAsync_MultiKey(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZMPOP is supported since 7.0.0");

        string key1 = $"{{testKey}}-{Guid.NewGuid()}";

        // Add test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
        });

        // Pop single max from multi-key
        ValkeyKey[] keys = [key1];
        SortedSetScoreResult? result = await client.SortedSetPopMaxAsync(keys);
        _ = Assert.NotNull(result);
        Assert.Equal("member2", result.Value.Member);
        Assert.Equal(20.0, result.Value.Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetPopMinAsync_MultiKey_WithCount(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZMPOP is supported since 7.0.0");

        string key1 = $"{{testKey}}-{Guid.NewGuid()}";
        string key2 = $"{{testKey}}-{Guid.NewGuid()}";

        // Pop from non-existent keys
        ValkeyKey[] keys = [key1, key2];
        Assert.True((await client.SortedSetPopMinAsync(keys, 2)).IsNull);

        // Add test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
            ["member3"] = 30.0,
        });

        // Pop multiple min from multi-key
        var result = await client.SortedSetPopMinAsync(keys, 2);
        Assert.False(result.IsNull);
        Assert.Equal(key1, result.Key);
        Assert.Equal(2, result.Entries.Length);
        Assert.Equal("member1", result.Entries[0].Element);
        Assert.Equal(10.0, result.Entries[0].Score);
        Assert.Equal("member2", result.Entries[1].Element);
        Assert.Equal(20.0, result.Entries[1].Score);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetPopMaxAsync_MultiKey_WithCount(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "ZMPOP is supported since 7.0.0");

        string key1 = $"{{testKey}}-{Guid.NewGuid()}";

        // Add test data
        _ = await client.SortedSetAddAsync(key1, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.0,
            ["member2"] = 20.0,
            ["member3"] = 30.0,
        });

        // Pop multiple max from multi-key
        ValkeyKey[] keys = [key1];
        var result = await client.SortedSetPopMaxAsync(keys, 2);
        Assert.False(result.IsNull);
        Assert.Equal(key1, result.Key);
        Assert.Equal(2, result.Entries.Length);
        Assert.Equal("member3", result.Entries[0].Element);
        Assert.Equal(30.0, result.Entries[0].Score);
        Assert.Equal("member2", result.Entries[1].Element);
        Assert.Equal(20.0, result.Entries[1].Score);
    }

    #endregion
    #region SortedSetRandomMemberWithScoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestSortedSetRandomMemberWithScoreAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Non-existent key
        Assert.Null(await client.SortedSetRandomMemberWithScoreAsync(key));

        // Add test data
        _ = await client.SortedSetAddAsync(key, new Dictionary<ValkeyValue, double>
        {
            ["member1"] = 10.5,
            ["member2"] = 8.2,
            ["member3"] = 15.0,
        });

        // Get random member with score
        SortedSetScoreResult? result = await client.SortedSetRandomMemberWithScoreAsync(key);
        _ = Assert.NotNull(result);
        Assert.Contains(result.Value.Member.ToString(), new[] { "member1", "member2", "member3" });
        Assert.True(result.Value.Score > 0);
    }

    #endregion
}
