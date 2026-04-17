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
    {
        ValkeyKey[] keys = ["key1", "key2"];
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringBitOperationAsync(Bitwise.And, "result", keys, UnsupportedFlag));
    }

    #endregion
    #region Generic Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyCopyAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyCopyAsync("src", "dest", flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyDeleteAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyDeleteAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyDeleteAsync([], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyDumpAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyDumpAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyEncodingAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyEncodingAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyExistsAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyExistsAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyExistsAsync([], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyExpireAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyExpireAsync("key", TimeSpan.Zero, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyExpireAsync("key", TimeSpan.Zero, ExpireWhen.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyExpireAsync("key", DateTime.MinValue, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyExpireAsync("key", DateTime.MinValue, ExpireWhen.Always, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyExpireTimeAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyExpireTimeAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyFrequencyAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyFrequencyAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyIdleTimeAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyIdleTimeAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyMoveAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyMoveAsync("key", 0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyPersistAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyPersistAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyRandomAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyRandomAsync(UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyRefCountAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyRefCountAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyRenameAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyRenameAsync("key", "newkey", When.Always, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyRenameNXAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyRenameNXAsync("key", "newkey", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyRestoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyRestoreAsync("key", [], TimeSpan.Zero, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyRestoreAsync("key", [], DateTime.MinValue, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyTimeToLiveAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyTimeToLiveAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyTouchAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyTouchAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyTouchAsync([], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyTypeAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyTypeAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task KeyUnlinkAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyUnlinkAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.KeyUnlinkAsync([], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortAsync("key", 0, -1, Order.Ascending, SortType.Numeric, default, null, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortAndStoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortAndStoreAsync("dest", "key", 0, -1, Order.Ascending, SortType.Numeric, default, null, UnsupportedFlag));

    #endregion
    #region Geospatial Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoAddAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoAddAsync("key", 0, 0, "member", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoAddAsync("key", new GeoEntry(0, 0, "member"), UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoAddAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoDistanceAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoDistanceAsync("key", "m1", "m2", GeoUnit.Meters, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoHashAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoHashAsync("key", "member", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoHashAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoPositionAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoPositionAsync("key", "member", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoPositionAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        var shape = new GeoSearchCircle(100, GeoUnit.Meters);
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoSearchAsync("key", "member", shape, flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoSearchAsync("key", 0, 0, shape, flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task GeoSearchAndStoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        var shape = new GeoSearchCircle(100, GeoUnit.Meters);
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoSearchAndStoreAsync("src", "dest", "member", shape, flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.GeoSearchAndStoreAsync("src", "dest", 0, 0, shape, flags: UnsupportedFlag));
    }

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
    public async Task HashIncrementAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
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
    public async Task HashFieldExpireAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldExpireAsync("key", [], TimeSpan.Zero, flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldExpireAsync("key", [], DateTime.MinValue, flags: UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetExpireDateTimeAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetExpireDateTimeAsync("key", [], UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetTimeToLiveAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetTimeToLiveAsync("key", [], UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldPersistAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
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
            () => db.HashFieldGetAndSetExpiryAsync("key", "field", TimeSpan.Zero, flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetAndSetExpiryAsync("key", "field", DateTime.MinValue, UnsupportedFlag));
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
            () => db.HashFieldSetAndSetExpiryAsync("key", "field", "value", TimeSpan.Zero, flags: UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldSetAndSetExpiryAsync("key", "field", "value", DateTime.MinValue, flags: UnsupportedFlag));
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
    #region List Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListGetByIndexAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListGetByIndexAsync("key", 0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListInsertAfterAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListInsertAfterAsync("key", "pivot", "value", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListInsertBeforeAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListInsertBeforeAsync("key", "pivot", "value", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListLeftPopAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListLeftPopAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListLeftPopAsync("key", 1, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListLeftPopAsync([], 1, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListLeftPushAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListLeftPushAsync("key", "value", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListLeftPushAsync("key", "value", When.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListLeftPushAsync("key", [], UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListLeftPushAsync("key", [], When.Always, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListLengthAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListMoveAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListMoveAsync("src", "dest", ListSide.Left, ListSide.Right, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListPositionAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListPositionAsync("key", "element", 1, 0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListPositionsAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListPositionsAsync("key", "element", 1, 1, 0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRangeAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRangeAsync("key", 0, -1, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRemoveAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRemoveAsync("key", "value", 0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPopAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRightPopAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRightPopAsync("key", 1, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRightPopAsync([], 1, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPopLeftPushAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRightPopLeftPushAsync("src", "dest", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPushAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRightPushAsync("key", "value", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRightPushAsync("key", "value", When.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRightPushAsync("key", [], UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListRightPushAsync("key", [], When.Always, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListSetByIndexAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListSetByIndexAsync("key", 0, "value", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListTrimAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ListTrimAsync("key", 0, -1, UnsupportedFlag));

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
    #region Scripting Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_String_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ScriptEvaluateAsync("return 1", null, null, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_ByteArray_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ScriptEvaluateAsync([], null, null, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_LuaScript_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ScriptEvaluateAsync(LuaScript.Prepare("return 1"), null, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ScriptEvaluateAsync_LoadedLuaScript_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.ScriptEvaluateAsync(
                new LoadedLuaScript(LuaScript.Prepare("return 1"), [], "return 1"), null, UnsupportedFlag));

    #endregion
    #region Set Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetAddAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetAddAsync("key", "value", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetAddAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetRemoveAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetRemoveAsync("key", "value", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetRemoveAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetMembersAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetMembersAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetLengthAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetIntersectionLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetIntersectionLengthAsync([], 0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetPopAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetPopAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetPopAsync("key", 2, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetCombineAsync(SetOperation.Union, "key1", "key2", UnsupportedFlag));
        ValkeyKey[] keys = ["key1", "key2"];
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetCombineAsync(SetOperation.Union, keys, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetCombineAndStoreAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetCombineAndStoreAsync(SetOperation.Union, "dest", "key1", "key2", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetCombineAndStoreAsync(SetOperation.Union, "dest", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetContainsAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetContainsAsync("key", "value", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetContainsAsync("key", [], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetRandomMemberAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetRandomMemberAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetRandomMembersAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetRandomMembersAsync("key", 2, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetMoveAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SetMoveAsync("src", "dst", "value", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SetScanAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            async () => { await foreach (var _ in db.SetScanAsync("key", flags: UnsupportedFlag)) { } });

    #endregion
    #region Sorted Set Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetAddAsync("key", "member", 1.0, SortedSetWhen.Always, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetAddAsync_Multi_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetAddAsync("key", [], SortedSetWhen.Always, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetUpdateAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetUpdateAsync("key", "member", 1.0, SortedSetWhen.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetUpdateAsync("key", [], SortedSetWhen.Always, UnsupportedFlag));
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
    public async Task SortedSetRangeByValueAsync_WithSkip_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.SortedSetRangeByValueAsync("key", "a", "z", Exclude.None, 0, -1, UnsupportedFlag));

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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task SortedSetScanAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            async () => { await foreach (var _ in db.SortedSetScanAsync("key", flags: UnsupportedFlag)) { } });

    #endregion
    #region String Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringAppendAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringAppendAsync("key", "value", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringDecrementAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringDecrementAsync("key", 1, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringDecrementAsync("key", 1.0, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringGetAsync("key", UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringGetAsync([], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetDeleteAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringGetDeleteAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetRangeAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringGetRangeAsync("key", 0, 5, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetSetAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringGetSetAsync("key", "value", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetSetExpiryAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringGetSetExpiryAsync("key", TimeSpan.Zero, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringGetSetExpiryAsync("key", DateTime.MinValue, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringIncrementAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringIncrementAsync("key", 1, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringIncrementAsync("key", 1.0, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringLengthAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringLongestCommonSubsequenceAsync("key1", "key2", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceLengthAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringLongestCommonSubsequenceLengthAsync("key1", "key2", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceWithMatchesAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringLongestCommonSubsequenceWithMatchesAsync("key1", "key2", 0, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAndGetAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringSetAndGetAsync("key", "value", flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringSetAsync("key", "value", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_MultiKey_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringSetAsync([], When.Always, UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetRangeAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.StringSetRangeAsync("key", 0, "value", UnsupportedFlag));

    #endregion
    #region Server Management Commands

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigGetAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.ConfigGetAsync(flags: UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigSetAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.ConfigSetAsync("lfu-decay-time", "1", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_ConfigResetStatisticsAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.ConfigResetStatisticsAsync(UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_LolwutAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.LolwutAsync(UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_TimeAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.TimeAsync(UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_LastSaveAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.LastSaveAsync(UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_FlushDatabaseAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.FlushDatabaseAsync(UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestServers), MemberType = typeof(TestConfiguration))]
    public async Task IServer_FlushAllDatabasesAsync_UnsupportedFlags_Throws(IServer server)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => server.FlushAllDatabasesAsync(UnsupportedFlag));

    #endregion
}
