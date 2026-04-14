// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> list commands.
/// </summary>
public class ListCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region ListRightPopLeftPushAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPopLeftPushAsync_MovesElement(IDatabaseAsync db)
    {
        string source = $"ser-rpoplpush-src-{Guid.NewGuid()}";
        string dest = $"ser-rpoplpush-dest-{Guid.NewGuid()}";

        // Setup source list: [a, b, c]
        _ = await db.ListRightPushAsync(source, ["a", "b", "c"]);

        // RPOPLPUSH pops from right of source (c) and pushes to left of dest
        ValkeyValue result = await db.ListRightPopLeftPushAsync(source, dest);
        Assert.Equal("c", result.ToString());

        // Source should now be [a, b]
        ValkeyValue[] sourceList = await db.ListRangeAsync(source);
        Assert.Equal(["a", "b"], [.. sourceList.Select(v => v.ToString())]);

        // Dest should now be [c]
        ValkeyValue[] destList = await db.ListRangeAsync(dest);
        Assert.Equal(["c"], [.. destList.Select(v => v.ToString())]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPopLeftPushAsync_EmptySource_ReturnsNull(IDatabaseAsync db)
    {
        string source = $"ser-rpoplpush-empty-{Guid.NewGuid()}";
        string dest = $"ser-rpoplpush-dest-{Guid.NewGuid()}";

        ValkeyValue result = await db.ListRightPopLeftPushAsync(source, dest);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPopLeftPushAsync_SameSourceAndDest_RotatesList(IDatabaseAsync db)
    {
        string key = $"ser-rpoplpush-rotate-{Guid.NewGuid()}";

        // Setup list: [a, b, c]
        _ = await db.ListRightPushAsync(key, ["a", "b", "c"]);

        // RPOPLPUSH with same source and dest rotates the list
        ValkeyValue result = await db.ListRightPopLeftPushAsync(key, key);
        Assert.Equal("c", result.ToString());

        // List should now be [c, a, b]
        ValkeyValue[] list = await db.ListRangeAsync(key);
        Assert.Equal(["c", "a", "b"], [.. list.Select(v => v.ToString())]);
    }

    #endregion

    #region ListRightPushAsync/ListLeftPushAsync CommandFlags-only overloads

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPushAsync_WithFlagsOnly_Works(IDatabaseAsync db)
    {
        string key = $"ser-rpush-flags-{Guid.NewGuid()}";

        // Test single value with flags only (no When)
        long count = await db.ListRightPushAsync(key, "a", CommandFlags.None);
        Assert.Equal(1, count);

        // Test multiple values with flags only (no When)
        count = await db.ListRightPushAsync(key, ["b", "c"], CommandFlags.None);
        Assert.Equal(3, count);

        ValkeyValue[] list = await db.ListRangeAsync(key);
        Assert.Equal(["a", "b", "c"], [.. list.Select(v => v.ToString())]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListLeftPushAsync_WithFlagsOnly_Works(IDatabaseAsync db)
    {
        string key = $"ser-lpush-flags-{Guid.NewGuid()}";

        // Test single value with flags only (no When)
        long count = await db.ListLeftPushAsync(key, "a", CommandFlags.None);
        Assert.Equal(1, count);

        // Test multiple values with flags only (no When)
        count = await db.ListLeftPushAsync(key, ["b", "c"], CommandFlags.None);
        Assert.Equal(3, count);

        ValkeyValue[] list = await db.ListRangeAsync(key);
        Assert.Equal(["c", "b", "a"], [.. list.Select(v => v.ToString())]);
    }

    #endregion

    #region ListLeftPushAsync/ListRightPushAsync with When parameter (SER-specific)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListLeftPushAsync_WithWhen_Always_CreatesListIfNotExists(IDatabaseAsync db)
    {
        string key = $"ser-lpush-when-always-{Guid.NewGuid()}";

        // When.Always should create the list if it doesn't exist
        long count = await db.ListLeftPushAsync(key, "value1", When.Always);
        Assert.Equal(1, count);

        count = await db.ListLeftPushAsync(key, ["value2", "value3"], When.Always);
        Assert.Equal(3, count);

        ValkeyValue[] list = await db.ListRangeAsync(key);
        Assert.Equal(["value3", "value2", "value1"], [.. list.Select(v => v.ToString())]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListLeftPushAsync_WithWhen_Exists_OnlyPushesIfKeyExists(IDatabaseAsync db)
    {
        string key = $"ser-lpush-when-exists-{Guid.NewGuid()}";

        // When.Exists should NOT create the list if it doesn't exist
        long count = await db.ListLeftPushAsync(key, "value1", When.Exists);
        Assert.Equal(0, count);

        // Create the list first
        _ = await db.ListLeftPushAsync(key, "initial");

        // Now When.Exists should work
        count = await db.ListLeftPushAsync(key, "value2", When.Exists);
        Assert.Equal(2, count);

        count = await db.ListLeftPushAsync(key, ["value3", "value4"], When.Exists);
        Assert.Equal(4, count);

        ValkeyValue[] list = await db.ListRangeAsync(key);
        Assert.Equal(["value4", "value3", "value2", "initial"], [.. list.Select(v => v.ToString())]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPushAsync_WithWhen_Always_CreatesListIfNotExists(IDatabaseAsync db)
    {
        string key = $"ser-rpush-when-always-{Guid.NewGuid()}";

        // When.Always should create the list if it doesn't exist
        long count = await db.ListRightPushAsync(key, "value1", When.Always);
        Assert.Equal(1, count);

        count = await db.ListRightPushAsync(key, ["value2", "value3"], When.Always);
        Assert.Equal(3, count);

        ValkeyValue[] list = await db.ListRangeAsync(key);
        Assert.Equal(["value1", "value2", "value3"], [.. list.Select(v => v.ToString())]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task ListRightPushAsync_WithWhen_Exists_OnlyPushesIfKeyExists(IDatabaseAsync db)
    {
        string key = $"ser-rpush-when-exists-{Guid.NewGuid()}";

        // When.Exists should NOT create the list if it doesn't exist
        long count = await db.ListRightPushAsync(key, "value1", When.Exists);
        Assert.Equal(0, count);

        // Create the list first
        _ = await db.ListRightPushAsync(key, "initial");

        // Now When.Exists should work
        count = await db.ListRightPushAsync(key, "value2", When.Exists);
        Assert.Equal(2, count);

        count = await db.ListRightPushAsync(key, ["value3", "value4"], When.Exists);
        Assert.Equal(4, count);

        ValkeyValue[] list = await db.ListRangeAsync(key);
        Assert.Equal(["initial", "value2", "value3", "value4"], [.. list.Select(v => v.ToString())]);
    }

    #endregion
}
