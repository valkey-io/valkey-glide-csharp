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

    #endregion
}
