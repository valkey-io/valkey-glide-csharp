// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests that StackExchange.Redis methods throw <see cref="NotImplementedException"/> for unsupported <see cref="CommandFlags"/>.
/// </summary>
public class CommandFlagsTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region Constants

    private const CommandFlags UnsupportedFlag = CommandFlags.DemandMaster;

    #endregion
    #region Hash Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashGetAsync("key", "field", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashGetAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAllAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashGetAllAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashSetAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashSetAsync("key", "field", "value", When.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashSetAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashIncrementAsync(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashIncrementAsync("key", "field", 1, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashIncrementAsync("key", "field", 1.0, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashDeleteAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashDeleteAsync("key", "field", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashDeleteAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashExistsAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashExistsAsync("key", "field", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashKeysAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashKeysAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashLengthAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashStringLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashStringLengthAsync("key", "field", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashValuesAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashValuesAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldExpireAsync(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldExpireAsync("key", [], TimeSpan.FromSeconds(60), flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldExpireAsync("key", [], DateTime.UtcNow.AddMinutes(5), flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetExpireDateTimeAsync(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetExpireDateTimeAsync("key", [], UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetTimeToLiveAsync(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetTimeToLiveAsync("key", [], UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldPersistAsync(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldPersistAsync("key", [], UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashRandomFieldAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashRandomFieldAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashRandomFieldsAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashRandomFieldsAsync("key", 2, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashRandomFieldsWithValuesAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashRandomFieldsWithValuesAsync("key", 2, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiryAsync_SingleField_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetAndSetExpiryAsync("key", "field", TimeSpan.FromSeconds(60), false, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetAndSetExpiryAsync("key", "field", DateTime.UtcNow.AddMinutes(5), UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetAndSetExpiryAsync_MultiField_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetAndSetExpiryAsync("key", [], flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetAndSetExpiryAsync("key", [], flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiryAsync_SingleField_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldSetAndSetExpiryAsync("key", "field", "value", TimeSpan.FromSeconds(60), false, When.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldSetAndSetExpiryAsync("key", "field", "value", DateTime.UtcNow.AddMinutes(5), When.Always, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldSetAndSetExpiryAsync_MultiField_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldSetAndSetExpiryAsync("key", [], flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldSetAndSetExpiryAsync("key", [], flags: UnsupportedFlag));
    }

    #endregion
    #region Bitmap Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetBitAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringGetBitAsync("key", 0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetBitAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringSetBitAsync("key", 0, true, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringBitCountAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringBitCountAsync("key", 0, -1, StringIndexType.Byte, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringBitPositionAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringBitPositionAsync("key", true, 0, -1, StringIndexType.Byte, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringBitOperationAsync_TwoKeys_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringBitOperationAsync(Bitwise.And, "result", "key1", "key2", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringBitOperationAsync_MultipleKeys_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(()
            => db.StringBitOperationAsync(Bitwise.And, "result", [], UnsupportedFlag));

    #endregion
    #region Sorted Set Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetAddAsync("key", "member", 1.0, SortedSetWhen.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetAddAsync("key", [], SortedSetWhen.Always, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetUpdateAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetUpdateAsync("key", "member", 1.0, flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetUpdateAsync("key", [], flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRemoveAsync("key", "member", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRemoveAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetLengthAsync("key", flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetIncrementAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetIncrementAsync("key", "member", 1.0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetDecrementAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetDecrementAsync("key", "member", 1.0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetIntersectionLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetIntersectionLengthAsync([], flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetLengthByValueAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetLengthByValueAsync("key", "a", "z", flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetPopAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetPopAsync("key", flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetPopAsync("key", 1, flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetPopAsync([], 1, flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRandomMemberAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRandomMemberAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRandomMembersAsync("key", 2, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRandomMembersWithScoresAsync("key", 2, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByRankAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRangeByRankAsync("key", flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRangeByRankWithScoresAsync("key", flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByScoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRangeByScoreAsync("key", flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRangeByScoreWithScoresAsync("key", flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeByValueAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRangeByValueAsync("key", flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRangeAndStoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRangeAndStoreAsync("src", "dest", 0, -1, flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRankAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRankAsync("key", "member", flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveRangeByValueAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRemoveRangeByValueAsync("key", "a", "z", flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveRangeByRankAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRemoveRangeByRankAsync("key", 0, -1, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetRemoveRangeByScoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRemoveRangeByScoreAsync("key", 0, 10, flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetScoreAsync("key", "member", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetScoresAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetCombineAsync(SetOperation.Union, [], flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetCombineWithScoresAsync(SetOperation.Union, [], flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetCombineAndStoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetCombineAndStoreAsync(SetOperation.Union, "dest", "k1", "k2", flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetCombineAndStoreAsync(SetOperation.Union, "dest", [], flags: UnsupportedFlag));
    }

    #endregion
    #region PubSub Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task PublishAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.PublishAsync(
                ValkeyChannel.Literal("test-channel"),
                "hello",
                UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task SubscriptionChannelsAsync_ThrowsOnCommandFlags(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.SubscriptionChannelsAsync(flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task SubscriptionPatternCountAsync_ThrowsOnCommandFlags(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.SubscriptionPatternCountAsync(flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task SubscriptionSubscriberCountAsync_ThrowsOnCommandFlags(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.SubscriptionSubscriberCountAsync(
                ValkeyChannel.Literal("test-channel"),
                flags: UnsupportedFlag));

    #endregion
}
