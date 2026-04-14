// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> set commands.
/// </summary>
public class SetCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region SetAddAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetAddAsync_SingleValue_ReturnsTrue(IDatabaseAsync db)
    {
        string key = $"ser-sadd1-{Guid.NewGuid()}";

        Assert.True(await db.SetAddAsync(key, "member1"));
        Assert.False(await db.SetAddAsync(key, "member1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetAddAsync_MultiValue_ReturnsAddedCount(IDatabaseAsync db)
    {
        string key = $"ser-sadd2-{Guid.NewGuid()}";

        Assert.Equal(3, await db.SetAddAsync(key, ["a", "b", "c"]));
        Assert.Equal(1, await db.SetAddAsync(key, ["c", "d"]));
    }

    #endregion
    #region SetRemoveAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetRemoveAsync_SingleValue_ReturnsTrue(IDatabaseAsync db)
    {
        string key = $"ser-srem1-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, "member1");

        Assert.True(await db.SetRemoveAsync(key, "member1"));
        Assert.False(await db.SetRemoveAsync(key, "member1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetRemoveAsync_MultiValue_ReturnsRemovedCount(IDatabaseAsync db)
    {
        string key = $"ser-srem2-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b", "c"]);

        Assert.Equal(2, await db.SetRemoveAsync(key, ["a", "b"]));
        Assert.Equal(0, await db.SetRemoveAsync(key, ["x"]));
    }

    #endregion
    #region SetMembersAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetMembersAsync_ReturnsAllMembers(IDatabaseAsync db)
    {
        string key = $"ser-smembers-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b", "c"]);

        ValkeyValue[] members = await db.SetMembersAsync(key);
        Assert.Equal(3, members.Length);
        Assert.Equivalent(new ValkeyValue[] { "a", "b", "c" }, members);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetMembersAsync_NonExistentKey_ReturnsEmpty(IDatabaseAsync db)
    {
        ValkeyValue[] members = await db.SetMembersAsync($"ser-smembers-{Guid.NewGuid()}");
        Assert.Empty(members);
    }

    #endregion
    #region SetLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetLengthAsync_ReturnsCardinality(IDatabaseAsync db)
    {
        string key = $"ser-scard-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b", "c"]);

        Assert.Equal(3, await db.SetLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetLengthAsync_NonExistentKey_ReturnsZero(IDatabaseAsync db)
        => Assert.Equal(0, await db.SetLengthAsync($"ser-scard-{Guid.NewGuid()}"));

    #endregion
    #region SetPopAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetPopAsync_SinglePop_ReturnsMember(IDatabaseAsync db)
    {
        string key = $"ser-spop1-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b", "c"]);

        ValkeyValue popped = await db.SetPopAsync(key);
        Assert.False(popped.IsNull);
        Assert.Contains(popped.ToString(), new[] { "a", "b", "c" });
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetPopAsync_MultiPop_ReturnsMembers(IDatabaseAsync db)
    {
        string key = $"ser-spop2-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b", "c"]);

        ValkeyValue[] popped = await db.SetPopAsync(key, 2);
        Assert.Equal(2, popped.Length);
        foreach (ValkeyValue v in popped)
        {
            Assert.Contains(v.ToString(), new[] { "a", "b", "c" });
        }
    }

    #endregion
    #region SetRandomMemberAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetRandomMemberAsync_ReturnsMember(IDatabaseAsync db)
    {
        string key = $"ser-srand1-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b", "c"]);

        ValkeyValue member = await db.SetRandomMemberAsync(key);
        Assert.False(member.IsNull);
        Assert.Contains(member.ToString(), new[] { "a", "b", "c" });
    }

    #endregion
    #region SetRandomMembersAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetRandomMembersAsync_ReturnsRequestedCount(IDatabaseAsync db)
    {
        string key = $"ser-srand2-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b", "c"]);

        ValkeyValue[] members = await db.SetRandomMembersAsync(key, 2);
        Assert.Equal(2, members.Length);
        foreach (ValkeyValue v in members)
        {
            Assert.Contains(v.ToString(), new[] { "a", "b", "c" });
        }
    }

    #endregion
    #region SetContainsAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetContainsAsync_SingleValue_ReturnsMembership(IDatabaseAsync db)
    {
        string key = $"ser-sismember-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, "member1");

        Assert.True(await db.SetContainsAsync(key, "member1"));
        Assert.False(await db.SetContainsAsync(key, "nonexistent"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetContainsAsync_MultiValue_ReturnsMembershipArray(IDatabaseAsync db)
    {
        string key = $"ser-smismember-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key, ["a", "b"]);

        bool[] results = await db.SetContainsAsync(key, ["a", "b", "c"]);
        Assert.Equal(3, results.Length);
        Assert.True(results[0]);
        Assert.True(results[1]);
        Assert.False(results[2]);
    }

    #endregion
    #region SetMoveAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetMoveAsync_MovesMember(IDatabaseAsync db)
    {
        string src = $"{{ser-smove}}-src-{Guid.NewGuid()}";
        string dst = $"{{ser-smove}}-dst-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(src, "member1");

        Assert.True(await db.SetMoveAsync(src, dst, "member1"));
        Assert.False(await db.SetContainsAsync(src, "member1"));
        Assert.True(await db.SetContainsAsync(dst, "member1"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetMoveAsync_NonExistentMember_ReturnsFalse(IDatabaseAsync db)
    {
        string src = $"{{ser-smove}}-src2-{Guid.NewGuid()}";
        string dst = $"{{ser-smove}}-dst2-{Guid.NewGuid()}";

        Assert.False(await db.SetMoveAsync(src, dst, "nonexistent"));
    }

    #endregion
    #region SetCombineAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAsync_Union_TwoKey_ReturnsUnion(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-u1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-u2-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b"]);
        _ = await db.SetAddAsync(key2, ["b", "c"]);

        var result = await db.SetCombineAsync(SetOperation.Union, key1, key2);
        Assert.Equal(3, result.Length);
        Assert.Equivalent(new ValkeyValue[] { "a", "b", "c" }, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAsync_Intersect_TwoKey_ReturnsIntersection(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-i1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-i2-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b", "c", "d"]);

        var result = await db.SetCombineAsync(SetOperation.Intersect, key1, key2);
        Assert.Equal(2, result.Length);
        Assert.Equivalent(new ValkeyValue[] { "b", "c" }, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAsync_Difference_TwoKey_ReturnsDifference(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-d1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-d2-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b", "c", "d"]);

        var result = await db.SetCombineAsync(SetOperation.Difference, key1, key2);
        Assert.Equivalent(new ValkeyValue[] { "a" }, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAsync_Union_MultiKey_ReturnsUnion(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-mu1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-mu2-{Guid.NewGuid()}";
        string key3 = $"{{ser-set}}-mu3-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b"]);
        _ = await db.SetAddAsync(key2, ["b", "c"]);
        _ = await db.SetAddAsync(key3, ["c", "d"]);

        ValkeyKey[] keys = [key1, key2, key3];
        var result = await db.SetCombineAsync(SetOperation.Union, keys);
        Assert.Equal(4, result.Length);
        Assert.Equivalent(new ValkeyValue[] { "a", "b", "c", "d" }, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAsync_Intersect_MultiKey_ReturnsIntersection(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-mi1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-mi2-{Guid.NewGuid()}";
        string key3 = $"{{ser-set}}-mi3-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b", "c", "d"]);
        _ = await db.SetAddAsync(key3, ["b", "c", "e"]);

        ValkeyKey[] keys = [key1, key2, key3];
        var result = await db.SetCombineAsync(SetOperation.Intersect, keys);
        Assert.Equal(2, result.Length);
        Assert.Equivalent(new ValkeyValue[] { "b", "c" }, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAsync_Difference_MultiKey_ReturnsDifference(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-md1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-md2-{Guid.NewGuid()}";
        string key3 = $"{{ser-set}}-md3-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b"]);
        _ = await db.SetAddAsync(key3, ["c"]);

        ValkeyKey[] keys = [key1, key2, key3];
        var result = await db.SetCombineAsync(SetOperation.Difference, keys);
        Assert.Equivalent(new ValkeyValue[] { "a" }, result);
    }

    #endregion
    #region SetCombineAndStoreAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAndStoreAsync_Union_TwoKey_StoresResult(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-su1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-su2-{Guid.NewGuid()}";
        string dest = $"{{ser-set}}-su-dest-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b"]);
        _ = await db.SetAddAsync(key2, ["b", "c"]);

        long count = await db.SetCombineAndStoreAsync(SetOperation.Union, dest, key1, key2);
        Assert.Equal(3, count);
        Assert.Equivalent(new ValkeyValue[] { "a", "b", "c" }, await db.SetMembersAsync(dest));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAndStoreAsync_Intersect_TwoKey_StoresResult(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-si1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-si2-{Guid.NewGuid()}";
        string dest = $"{{ser-set}}-si-dest-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b", "c", "d"]);

        long count = await db.SetCombineAndStoreAsync(SetOperation.Intersect, dest, key1, key2);
        Assert.Equal(2, count);
        Assert.Equivalent(new ValkeyValue[] { "b", "c" }, await db.SetMembersAsync(dest));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAndStoreAsync_Difference_TwoKey_StoresResult(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-sd1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-sd2-{Guid.NewGuid()}";
        string dest = $"{{ser-set}}-sd-dest-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b", "c", "d"]);

        long count = await db.SetCombineAndStoreAsync(SetOperation.Difference, dest, key1, key2);
        Assert.Equal(1, count);
        Assert.Equivalent(new ValkeyValue[] { "a" }, await db.SetMembersAsync(dest));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAndStoreAsync_Union_MultiKey_StoresResult(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-msu1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-msu2-{Guid.NewGuid()}";
        string key3 = $"{{ser-set}}-msu3-{Guid.NewGuid()}";
        string dest = $"{{ser-set}}-msu-dest-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b"]);
        _ = await db.SetAddAsync(key2, ["b", "c"]);
        _ = await db.SetAddAsync(key3, ["c", "d"]);

        ValkeyKey[] keys = [key1, key2, key3];
        long count = await db.SetCombineAndStoreAsync(SetOperation.Union, dest, keys);
        Assert.Equal(4, count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAndStoreAsync_Intersect_MultiKey_StoresResult(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-msi1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-msi2-{Guid.NewGuid()}";
        string key3 = $"{{ser-set}}-msi3-{Guid.NewGuid()}";
        string dest = $"{{ser-set}}-msi-dest-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b", "c", "d"]);
        _ = await db.SetAddAsync(key3, ["b", "c", "e"]);

        ValkeyKey[] keys = [key1, key2, key3];
        long count = await db.SetCombineAndStoreAsync(SetOperation.Intersect, dest, keys);
        Assert.Equal(2, count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAndStoreAsync_Difference_MultiKey_StoresResult(IDatabaseAsync db)
    {
        string key1 = $"{{ser-set}}-msd1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-msd2-{Guid.NewGuid()}";
        string key3 = $"{{ser-set}}-msd3-{Guid.NewGuid()}";
        string dest = $"{{ser-set}}-msd-dest-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b"]);
        _ = await db.SetAddAsync(key3, ["c"]);

        ValkeyKey[] keys = [key1, key2, key3];
        long count = await db.SetCombineAndStoreAsync(SetOperation.Difference, dest, keys);
        Assert.Equal(1, count);
    }

    #endregion
    #region SetIntersectionLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetIntersectionLengthAsync_ReturnsCardinality(IDatabaseAsync db)
    {
        SkipUtils.IfSetInterCardNotSupported();

        string key1 = $"{{ser-set}}-sic1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-sic2-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["b", "c", "d"]);

        ValkeyKey[] keys = [key1, key2];
        Assert.Equal(2, await db.SetIntersectionLengthAsync(keys));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetIntersectionLengthAsync_WithLimit_RespectsLimit(IDatabaseAsync db)
    {
        SkipUtils.IfSetInterCardNotSupported();

        string key1 = $"{{ser-set}}-sicl1-{Guid.NewGuid()}";
        string key2 = $"{{ser-set}}-sicl2-{Guid.NewGuid()}";
        _ = await db.SetAddAsync(key1, ["a", "b", "c"]);
        _ = await db.SetAddAsync(key2, ["a", "b", "c", "d"]);

        ValkeyKey[] keys = [key1, key2];
        Assert.Equal(1, await db.SetIntersectionLengthAsync(keys, limit: 1));
    }

    #endregion
}
