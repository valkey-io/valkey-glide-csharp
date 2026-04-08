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
            () => db.HashGetAsync("key", ["field"], UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashGetAllAsync_ThrowsOnCommandFlags(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashGetAllAsync("key", UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashSetAsync(IDatabaseAsync db)
    {
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashSetAsync("key", "field", "value", When.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashSetAsync("key", [new HashEntry("field", "value")], UnsupportedFlag));
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
            () => db.HashDeleteAsync("key", ["field"], UnsupportedFlag));
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
            () => db.HashFieldExpireAsync("key", ["field"], TimeSpan.FromSeconds(60), ExpireWhen.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldExpireAsync("key", ["field"], DateTime.UtcNow.AddMinutes(5), ExpireWhen.Always, UnsupportedFlag));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetExpireDateTimeAsync(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetExpireDateTimeAsync("key", ["field"], UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldGetTimeToLiveAsync(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetTimeToLiveAsync("key", ["field"], UnsupportedFlag));

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task HashFieldPersistAsync(IDatabaseAsync db)
        => _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldPersistAsync("key", ["field"], UnsupportedFlag));

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
        ValkeyValue[] fields = ["field"];
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetAndSetExpiryAsync("key", fields, TimeSpan.FromSeconds(60), false, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldGetAndSetExpiryAsync("key", fields, DateTime.UtcNow.AddMinutes(5), UnsupportedFlag));
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
        HashEntry[] entries = [new("field", "value")];
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldSetAndSetExpiryAsync("key", entries, TimeSpan.FromSeconds(60), false, When.Always, UnsupportedFlag));
        _ = await Assert.ThrowsAsync<NotImplementedException>(
            () => db.HashFieldSetAndSetExpiryAsync("key", entries, DateTime.UtcNow.AddMinutes(5), When.Always, UnsupportedFlag));
    }

    #endregion
}
