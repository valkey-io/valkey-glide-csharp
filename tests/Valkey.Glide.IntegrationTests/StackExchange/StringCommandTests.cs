// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests.StackExchange;

/// <summary>
/// Tests for <see cref="IDatabaseAsync"/> string commands.
/// </summary>
public class StringCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    #region StringAppendAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringAppendAsync_AppendsToExistingKey(IDatabaseAsync db)
    {
        string key = $"ser-append-{Guid.NewGuid()}";
        string initialValue = "Hello";
        string appendValue = " World";

        // Set initial value via StringSetAsync (SER layer)
        _ = await db.StringSetAsync(key, initialValue, CommandFlags.None);

        // Append via SER layer
        long newLength = await db.StringAppendAsync(key, appendValue);

        Assert.Equal(initialValue.Length + appendValue.Length, newLength);

        ValkeyValue result = await db.StringGetAsync(key, CommandFlags.None);
        Assert.Equal(initialValue + appendValue, result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringAppendAsync_CreatesKeyIfNotExists(IDatabaseAsync db)
    {
        string key = $"ser-append-new-{Guid.NewGuid()}";
        string appendValue = "Hello World";

        // Append to a non-existent key (should create it)
        long newLength = await db.StringAppendAsync(key, appendValue);

        Assert.Equal(appendValue.Length, newLength);

        ValkeyValue result = await db.StringGetAsync(key, CommandFlags.None);
        Assert.Equal(appendValue, result.ToString());
    }

    #endregion

    #region StringDecrementAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringDecrementAsync_Long_Default_Decrements(IDatabaseAsync db)
    {
        string key = $"ser-decr-default-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "10", CommandFlags.None);

        long result = await db.StringDecrementAsync(key);
        Assert.Equal(9, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringDecrementAsync_Long_CustomValue_Decrements(IDatabaseAsync db)
    {
        string key = $"ser-decr-custom-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "10", CommandFlags.None);

        long result = await db.StringDecrementAsync(key, 5);
        Assert.Equal(5, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringDecrementAsync_Long_NonExistentKey_DecrementsFromZero(IDatabaseAsync db)
    {
        string key = $"ser-decr-nonexist-{Guid.NewGuid()}";

        long result = await db.StringDecrementAsync(key);
        Assert.Equal(-1, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringDecrementAsync_Double_CustomValue_Decrements(IDatabaseAsync db)
    {
        string key = $"ser-decr-double-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "10.5", CommandFlags.None);

        double result = await db.StringDecrementAsync(key, 0.5);
        Assert.Equal(10.0, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringDecrementAsync_Double_NonExistentKey_DecrementsFromZero(IDatabaseAsync db)
    {
        string key = $"ser-decr-double-nonexist-{Guid.NewGuid()}";

        double result = await db.StringDecrementAsync(key, 1.5);
        Assert.Equal(-1.5, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringDecrementAsync_ThrowsExceptions(IDatabaseAsync db)
    {
        string key = $"ser-decr-throws-{Guid.NewGuid()}";

        // Set a non-numeric value
        _ = await db.StringSetAsync(key, "not-a-number", CommandFlags.None);

        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => db.StringDecrementAsync(key));
    }

    #endregion

    #region StringIncrementAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringIncrementAsync_Long_Default_Increments(IDatabaseAsync db)
    {
        string key = $"ser-incr-default-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "10", CommandFlags.None);

        long result = await db.StringIncrementAsync(key);
        Assert.Equal(11, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringIncrementAsync_Long_CustomValue_Increments(IDatabaseAsync db)
    {
        string key = $"ser-incr-custom-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "10", CommandFlags.None);

        long result = await db.StringIncrementAsync(key, 5);
        Assert.Equal(15, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringIncrementAsync_Long_NonExistentKey_IncrementsFromZero(IDatabaseAsync db)
    {
        string key = $"ser-incr-nonexist-{Guid.NewGuid()}";

        long result = await db.StringIncrementAsync(key);
        Assert.Equal(1, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringIncrementAsync_Double_CustomValue_Increments(IDatabaseAsync db)
    {
        string key = $"ser-incr-double-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "10.5", CommandFlags.None);

        double result = await db.StringIncrementAsync(key, 0.5);
        Assert.Equal(11.0, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringIncrementAsync_Double_NonExistentKey_IncrementsFromZero(IDatabaseAsync db)
    {
        string key = $"ser-incr-double-nonexist-{Guid.NewGuid()}";

        double result = await db.StringIncrementAsync(key, 1.5);
        Assert.Equal(1.5, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringIncrementAsync_ThrowsExceptions(IDatabaseAsync db)
    {
        string key = $"ser-incr-throws-{Guid.NewGuid()}";

        // Set a non-numeric value
        _ = await db.StringSetAsync(key, "not-a-number", CommandFlags.None);

        _ = await Assert.ThrowsAsync<Errors.RequestException>(() => db.StringIncrementAsync(key));
    }

    #endregion

    #region StringGetAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetAsync_ReturnsValue(IDatabaseAsync db)
    {
        string key = $"ser-get-{Guid.NewGuid()}";
        string value = "hello";

        _ = await db.StringSetAsync(key, value, CommandFlags.None);

        ValkeyValue result = await db.StringGetAsync(key);
        Assert.Equal(value, result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetAsync_NonExistentKey_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-get-nonexist-{Guid.NewGuid()}";

        ValkeyValue result = await db.StringGetAsync(key);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetAsync_MultipleKeys_ReturnsValues(IDatabaseAsync db)
    {
        string key1 = $"ser-mget-1-{Guid.NewGuid()}";
        string key2 = $"ser-mget-2-{Guid.NewGuid()}";
        string value1 = "value1";
        string value2 = "value2";

        _ = await db.StringSetAsync(key1, value1, CommandFlags.None);
        _ = await db.StringSetAsync(key2, value2, CommandFlags.None);

        ValkeyKey[] keys = [key1, key2];
        ValkeyValue[] results = await db.StringGetAsync(keys);

        Assert.Equal(2, results.Length);
        Assert.Equal(value1, results[0].ToString());
        Assert.Equal(value2, results[1].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetAsync_MultipleKeys_MixedExistence(IDatabaseAsync db)
    {
        string key1 = $"ser-mget-mix-1-{Guid.NewGuid()}";
        string key2 = $"ser-mget-mix-2-{Guid.NewGuid()}";
        string nonExistentKey = $"ser-mget-mix-none-{Guid.NewGuid()}";
        string value1 = "value1";

        _ = await db.StringSetAsync(key1, value1, CommandFlags.None);

        ValkeyKey[] keys = [key1, nonExistentKey, key2];
        ValkeyValue[] results = await db.StringGetAsync(keys);

        Assert.Equal(3, results.Length);
        Assert.Equal(value1, results[0].ToString());
        Assert.True(results[1].IsNull);
        Assert.True(results[2].IsNull);
    }

    #endregion

    #region StringGetDeleteAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetDeleteAsync_ReturnsValueAndDeletesKey(IDatabaseAsync db)
    {
        string key = $"ser-getdel-{Guid.NewGuid()}";
        string value = "delete-me";

        _ = await db.StringSetAsync(key, value, CommandFlags.None);

        ValkeyValue result = await db.StringGetDeleteAsync(key);
        Assert.Equal(value, result.ToString());

        // Verify key was deleted
        ValkeyValue afterDelete = await db.StringGetAsync(key);
        Assert.True(afterDelete.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetDeleteAsync_NonExistentKey_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-getdel-nonexist-{Guid.NewGuid()}";

        ValkeyValue result = await db.StringGetDeleteAsync(key);
        Assert.True(result.IsNull);
    }

    #endregion

    #region StringGetSetExpiryAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetSetExpiryAsync_TimeSpan_ReturnsValueAndSetsExpiry(IDatabaseAsync db)
    {
        string key = $"ser-getex-ts-{Guid.NewGuid()}";
        string value = "expire-me";

        _ = await db.StringSetAsync(key, value, CommandFlags.None);

        ValkeyValue result = await db.StringGetSetExpiryAsync(key, TimeSpan.FromSeconds(60));
        Assert.Equal(value, result.ToString());

        // Verify key still exists
        ValkeyValue afterExpiry = await db.StringGetAsync(key);
        Assert.Equal(value, afterExpiry.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetSetExpiryAsync_DateTime_ReturnsValueAndSetsExpiry(IDatabaseAsync db)
    {
        string key = $"ser-getex-dt-{Guid.NewGuid()}";
        string value = "expire-me-abs";

        _ = await db.StringSetAsync(key, value, CommandFlags.None);

        DateTime expiry = DateTime.UtcNow.AddMinutes(5);
        ValkeyValue result = await db.StringGetSetExpiryAsync(key, expiry);
        Assert.Equal(value, result.ToString());

        // Verify key still exists
        ValkeyValue afterExpiry = await db.StringGetAsync(key);
        Assert.Equal(value, afterExpiry.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetSetExpiryAsync_NullTimeSpan_PersistsKey(IDatabaseAsync db)
    {
        string key = $"ser-getex-persist-{Guid.NewGuid()}";
        string value = "persist-me";

        _ = await db.StringSetAsync(key, value, CommandFlags.None);

        // First set an expiry
        _ = await db.StringGetSetExpiryAsync(key, TimeSpan.FromSeconds(60));

        // Then remove it with null
        ValkeyValue result = await db.StringGetSetExpiryAsync(key, null);
        Assert.Equal(value, result.ToString());

        // Verify key still exists
        ValkeyValue afterPersist = await db.StringGetAsync(key);
        Assert.Equal(value, afterPersist.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetSetExpiryAsync_NonExistentKey_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-getex-nonexist-{Guid.NewGuid()}";

        ValkeyValue result = await db.StringGetSetExpiryAsync(key, TimeSpan.FromSeconds(60));
        Assert.True(result.IsNull);
    }

    #endregion

    #region StringSetAsync (SET/SETNX/SETEX/PSETEX)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_SetsValue(IDatabaseAsync db)
    {
        string key = $"ser-set-{Guid.NewGuid()}";

        bool result = await db.StringSetAsync(key, "hello");
        Assert.True(result);

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("hello", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_WhenNotExists_SetsOnlyNewKey(IDatabaseAsync db)
    {
        string key = $"ser-setnx-{Guid.NewGuid()}";

        bool result = await db.StringSetAsync(key, "value", when: When.NotExists);
        Assert.True(result);

        bool result2 = await db.StringSetAsync(key, "new_value", when: When.NotExists);
        Assert.False(result2);

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_WhenExists_SetsOnlyExistingKey(IDatabaseAsync db)
    {
        string key = $"ser-setxx-{Guid.NewGuid()}";

        bool result = await db.StringSetAsync(key, "value", when: When.Exists);
        Assert.False(result);

        _ = await db.StringSetAsync(key, "initial", CommandFlags.None);

        bool result2 = await db.StringSetAsync(key, "updated", when: When.Exists);
        Assert.True(result2);

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("updated", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_WithExpiry_SetsWithTtl(IDatabaseAsync db)
    {
        string key = $"ser-setex-{Guid.NewGuid()}";

        bool result = await db.StringSetAsync(key, "value", TimeSpan.FromSeconds(60));
        Assert.True(result);

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_WithKeepTtl_RetainsExpiry(IDatabaseAsync db)
    {
        string key = $"ser-keepttl-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "value", TimeSpan.FromSeconds(60));

        bool result = await db.StringSetAsync(key, "new_value", keepTtl: true);
        Assert.True(result);

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_MultipleKeys_SetsAll(IDatabaseAsync db)
    {
        string key1 = $"ser-mset-1-{Guid.NewGuid()}";
        string key2 = $"ser-mset-2-{Guid.NewGuid()}";

        KeyValuePair<ValkeyKey, ValkeyValue>[] values =
        [
            new(key1, "value1"),
            new(key2, "value2")
        ];

        bool result = await db.StringSetAsync(values);
        Assert.True(result);

        ValkeyValue retrieved1 = await db.StringGetAsync(key1);
        ValkeyValue retrieved2 = await db.StringGetAsync(key2);
        Assert.Equal("value1", retrieved1.ToString());
        Assert.Equal("value2", retrieved2.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAsync_MultipleKeys_WhenNotExists_AllOrNothing(IDatabaseAsync db)
    {
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:ser-msetnx-1";
        string key2 = $"{{{baseKey}}}:ser-msetnx-2";

        KeyValuePair<ValkeyKey, ValkeyValue>[] values =
        [
            new(key1, "value1"),
            new(key2, "value2")
        ];

        bool result = await db.StringSetAsync(values, When.NotExists);
        Assert.True(result);

        // Try again - should fail since keys exist
        bool result2 = await db.StringSetAsync(values, When.NotExists);
        Assert.False(result2);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAndGetAsync_ReturnsOldValue(IDatabaseAsync db)
    {
        string key = $"ser-setandget-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "old_value", CommandFlags.None);

        ValkeyValue result = await db.StringSetAndGetAsync(key, "new_value");
        Assert.Equal("old_value", result.ToString());

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAndGetAsync_NonExistentKey_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-setandget-nonexist-{Guid.NewGuid()}";

        ValkeyValue result = await db.StringSetAndGetAsync(key, "value");
        Assert.True(result.IsNull);

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetAndGetAsync_WithExpiry_ReturnsOldValueAndSetsExpiry(IDatabaseAsync db)
    {
        string key = $"ser-setandget-expiry-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "old_value", CommandFlags.None);

        ValkeyValue result = await db.StringSetAndGetAsync(key, "new_value", TimeSpan.FromSeconds(60));
        Assert.Equal("old_value", result.ToString());

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetSetAsync_ReturnsOldValue(IDatabaseAsync db)
    {
        string key = $"ser-getset-{Guid.NewGuid()}";

        _ = await db.StringSetAsync(key, "old_value", CommandFlags.None);

        ValkeyValue result = await db.StringGetSetAsync(key, "new_value");
        Assert.Equal("old_value", result.ToString());

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringGetSetAsync_NonExistentKey_ReturnsNull(IDatabaseAsync db)
    {
        string key = $"ser-getset-nonexist-{Guid.NewGuid()}";

        ValkeyValue result = await db.StringGetSetAsync(key, "value");
        Assert.True(result.IsNull);

        ValkeyValue retrieved = await db.StringGetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    #endregion

    #region StringSetRangeAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetRangeAsync_OverwritesPartOfString(IDatabaseAsync db)
    {
        string key = $"ser-setrange-{Guid.NewGuid()}";
        string initialValue = "Hello World";

        _ = await db.StringSetAsync(key, initialValue, CommandFlags.None);

        ValkeyValue newLength = await db.StringSetRangeAsync(key, 6, "Valkey");
        Assert.Equal(12, (long)newLength);

        ValkeyValue result = await db.StringGetAsync(key);
        Assert.Equal("Hello Valkey", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringSetRangeAsync_NonExistentKey_CreatesWithZeroPadding(IDatabaseAsync db)
    {
        string key = $"ser-setrange-nonexist-{Guid.NewGuid()}";

        ValkeyValue newLength = await db.StringSetRangeAsync(key, 5, "test");
        Assert.Equal(9, (long)newLength);

        ValkeyValue result = await db.StringGetAsync(key);
        // First 5 bytes are zero-padded, followed by "test"
        Assert.Equal("\0\0\0\0\0test", result.ToString());
    }

    #endregion

    #region StringLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringLengthAsync_ReturnsStringLength(IDatabaseAsync db)
    {
        string key = $"ser-strlen-{Guid.NewGuid()}";
        string value = "Hello World";

        _ = await db.StringSetAsync(key, value, CommandFlags.None);

        long length = await db.StringLengthAsync(key);
        Assert.Equal(value.Length, length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestDatabases), MemberType = typeof(TestConfiguration))]
    public async Task StringLengthAsync_NonExistentKey_ReturnsZero(IDatabaseAsync db)
    {
        string key = $"ser-strlen-nonexist-{Guid.NewGuid()}";

        long length = await db.StringLengthAsync(key);
        Assert.Equal(0, length);
    }

    #endregion
}
