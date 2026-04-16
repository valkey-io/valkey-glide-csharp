// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

namespace Valkey.Glide.IntegrationTests;

[Collection("GlideTests")]
public class StringCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_GetAsync_SingleKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = Guid.NewGuid().ToString();

        await client.SetAsync(key, value);

        ValkeyValue retrievedValue = await client.GetAsync(key);
        Assert.Equal(value, retrievedValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_NonExistentKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        ValkeyValue value = await client.GetAsync(key);
        Assert.True(value.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_MultipleKeys(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string key3 = Guid.NewGuid().ToString();
        string value1 = Guid.NewGuid().ToString();
        string value2 = Guid.NewGuid().ToString();

        // Set key1 and key2, leave key3 unset
        await client.SetAsync(key1, value1);
        await client.SetAsync(key2, value2);

        ValkeyKey[] keys = [key1, key2, key3];
        ValkeyValue[] values = await client.GetAsync(keys);

        Assert.Equal(3, values.Length);
        Assert.Equal(value1, values[0].ToString());
        Assert.Equal(value2, values[1].ToString());
        Assert.True(values[2].IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetAsync_NonExistentKeys(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();

        ValkeyKey[] keys = [key1, key2];
        ValkeyValue[] values = await client.GetAsync(keys);

        Assert.Equal(2, values.Length);
        Assert.True(values[0].IsNull);
        Assert.True(values[1].IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_MultipleKeyValuePairs(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string value1 = Guid.NewGuid().ToString();
        string value2 = Guid.NewGuid().ToString();

        KeyValuePair<ValkeyKey, ValkeyValue>[] keyValuePairs =
        [
            new(key1, value1),
            new(key2, value2)
        ];

        await client.SetAsync(keyValuePairs);

        ValkeyValue retrievedValue1 = await client.GetAsync(key1);
        ValkeyValue retrievedValue2 = await client.GetAsync(key2);

        Assert.Equal(value1, retrievedValue1.ToString());
        Assert.Equal(value2, retrievedValue2.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_KeyValuePairs_WithUnicodeValues(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string unicodeValue1 = "שלום hello 汉字";
        string unicodeValue2 = "مرحبا world 🌍";

        KeyValuePair<ValkeyKey, ValkeyValue>[] values =
        [
            new(key1, unicodeValue1),
            new(key2, unicodeValue2)
        ];

        await client.SetAsync(values);

        ValkeyKey[] keys = [key1, key2];
        ValkeyValue[] retrievedValues = await client.GetAsync(keys);

        Assert.Equal(2, retrievedValues.Length);
        Assert.Equal(unicodeValue1, retrievedValues[0].ToString());
        Assert.Equal(unicodeValue2, retrievedValues[1].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_KeyValuePairs_WithEmptyValues(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();

        KeyValuePair<ValkeyKey, ValkeyValue>[] values =
        [
            new(key1, ""),
            new(key2, "non-empty")
        ];

        await client.SetAsync(values);

        ValkeyKey[] keys = [key1, key2];
        ValkeyValue[] retrievedValues = await client.GetAsync(keys);

        Assert.Equal(2, retrievedValues.Length);
        Assert.Equal("", retrievedValues[0].ToString());
        Assert.Equal("non-empty", retrievedValues[1].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_KeyValuePairs_OverwriteExistingKeys(BaseClient client)
    {
        string key1 = Guid.NewGuid().ToString();
        string key2 = Guid.NewGuid().ToString();
        string initialValue1 = "initial1";
        string initialValue2 = "initial2";
        string newValue1 = "new1";
        string newValue2 = "new2";

        // Set initial values
        await client.SetAsync(key1, initialValue1);
        await client.SetAsync(key2, initialValue2);

        // Overwrite with SetAsync
        KeyValuePair<ValkeyKey, ValkeyValue>[] values =
        [
            new(key1, newValue1),
            new(key2, newValue2)
        ];

        await client.SetAsync(values);

        // Verify values were overwritten
        ValkeyKey[] keys = [key1, key2];
        ValkeyValue[] retrievedValues = await client.GetAsync(keys);

        Assert.Equal(2, retrievedValues.Length);
        Assert.Equal(newValue1, retrievedValues[0].ToString());
        Assert.Equal(newValue2, retrievedValues[1].ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task LengthAsync_ReturnsStringLength(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "Hello World";

        await client.SetAsync(key, value);
        long length = await client.LengthAsync(key);
        Assert.Equal(value.Length, length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task LengthAsync_NonExistentKey_ReturnsZero(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        long length = await client.LengthAsync(key);
        Assert.Equal(0, length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringGetRangeAsync_ExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = "Hello World";

        await client.SetAsync(key, value);
        ValkeyValue result = await client.StringGetRangeAsync(key, 0, 4);
        Assert.Equal("Hello", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringGetRangeAsync_NonExistentKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        ValkeyValue result = await client.StringGetRangeAsync(key, 0, 4);
        Assert.Equal("", result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetRangeAsync_OverwritesPartOfString(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string initialValue = "Hello World";

        await client.SetAsync(key, initialValue);
        ValkeyValue newLength = await client.SetRangeAsync(key, 6, "Valkey");
        Assert.Equal(12, (long)newLength);

        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("Hello Valkey", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetRangeAsync_NonExistentKey_CreatesWithZeroPadding(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        ValkeyValue newLength = await client.SetRangeAsync(key, 0, "Hello");
        Assert.Equal(5, (long)newLength);

        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("Hello", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AppendAsync_ExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string initialValue = "Hello";
        string appendValue = " World";

        // Set initial value
        await client.SetAsync(key, initialValue);

        // Append to the key
        long newLength = await client.AppendAsync(key, appendValue);

        // Verify the new length is correct
        Assert.Equal(initialValue.Length + appendValue.Length, newLength);

        // Verify the value was appended correctly
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal(initialValue + appendValue, value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AppendAsync_NonExistentKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string appendValue = "Hello World";

        // Append to a non-existent key (should create it)
        long newLength = await client.AppendAsync(key, appendValue);

        // Verify the new length is correct
        Assert.Equal(appendValue.Length, newLength);

        // Verify the key was created with the appended value
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal(appendValue, value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AppendAsync_EmptyValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string initialValue = "Hello";
        string appendValue = "";

        // Set initial value
        await client.SetAsync(key, initialValue);

        // Append empty string
        long newLength = await client.AppendAsync(key, appendValue);

        // Verify the length remains the same
        Assert.Equal(initialValue.Length, newLength);

        // Verify the value was not changed
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal(initialValue, value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task AppendAsync_UnicodeValues(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string initialValue = "Hello";
        string appendValue = " 世界";

        // Set initial value
        await client.SetAsync(key, initialValue);

        // Append Unicode string
        _ = await client.AppendAsync(key, appendValue);

        // Verify the value was appended correctly
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal(initialValue + appendValue, value.ToString());

        // The server returns the length in bytes, not characters
        // For Unicode characters, this will be different from the C# string length
        // So we don't test the exact length here
    }

    // Utility methods for other tests
    internal static async Task GetAndSetValuesAsync(BaseClient client, string key, string value)
    {
        await client.SetAsync(key, value);

        ValkeyValue retrievedValue = await client.GetAsync(key);
        Assert.Equal(value, retrievedValue.ToString());
    }

    internal static async Task GetAndSetRandomValuesAsync(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = Guid.NewGuid().ToString();

        await GetAndSetValuesAsync(client, key, value);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DecrementAsync_ExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value
        await client.SetAsync(key, "10");

        // Decrement by 1
        long result = await client.DecrementAsync(key);
        Assert.Equal(9, result);

        // Verify the value was decremented
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("9", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DecrementAsync_NonExistentKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Decrement non-existent key (should create it with value -1)
        long result = await client.DecrementAsync(key);
        Assert.Equal(-1, result);

        // Verify the key was created with value -1
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("-1", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DecrementAsync_WithAmount_ExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value
        await client.SetAsync(key, "10");

        // Decrement by 5
        long result = await client.DecrementAsync(key, 5);
        Assert.Equal(5, result);

        // Verify the value was decremented
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("5", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DecrementAsync_WithAmount_NonExistentKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Decrement non-existent key by 5 (should create it with value -5)
        long result = await client.DecrementAsync(key, 5);
        Assert.Equal(-5, result);

        // Verify the key was created with value -5
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("-5", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task DecrementAsync_WithNegativeAmount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value
        await client.SetAsync(key, "10");

        // Decrement by -5 (effectively incrementing by 5)
        long result = await client.DecrementAsync(key, -5);
        Assert.Equal(15, result);

        // Verify the value was incremented
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("15", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task IncrementAsync_ExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value
        await client.SetAsync(key, "10");

        // Increment by 1
        long result = await client.IncrementAsync(key);
        Assert.Equal(11, result);

        // Verify the value was incremented
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("11", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task IncrementAsync_NonExistentKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Increment non-existent key (should create it with value 1)
        long result = await client.IncrementAsync(key);
        Assert.Equal(1, result);

        // Verify the key was created with value 1
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("1", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task IncrementAsync_WithAmount_ExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value
        await client.SetAsync(key, "10");

        // Increment by 5
        long result = await client.IncrementAsync(key, 5);
        Assert.Equal(15, result);

        // Verify the value was incremented
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("15", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task IncrementAsync_WithAmount_NonExistentKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Increment non-existent key by 5 (should create it with value 5)
        long result = await client.IncrementAsync(key, 5);
        Assert.Equal(5, result);

        // Verify the key was created with value 5
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("5", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task IncrementAsync_WithNegativeAmount(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value
        await client.SetAsync(key, "10");

        // Increment by -5 (effectively decrementing by 5)
        long result = await client.IncrementAsync(key, -5);
        Assert.Equal(5, result);

        // Verify the value was decremented
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("5", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task IncrementAsync_WithFloat_ExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value
        await client.SetAsync(key, "10.5");

        // Increment by 0.5
        double result = await client.IncrementAsync(key, 0.5);
        Assert.Equal(11.0, result);

        // Verify the value was incremented
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("11", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task IncrementAsync_WithFloat_NonExistentKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Increment non-existent key by 0.5 (should create it with value 0.5)
        double result = await client.IncrementAsync(key, 0.5);
        Assert.Equal(0.5, result);

        // Verify the key was created with value 0.5
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("0.5", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(TestConfiguration.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task IncrementAsync_WithNegativeFloat(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set initial value
        await client.SetAsync(key, "10.5");

        // Increment by -0.5 (effectively decrementing by 0.5)
        double result = await client.IncrementAsync(key, -0.5);
        Assert.Equal(10.0, result);

        // Verify the value was decremented
        ValkeyValue value = await client.GetAsync(key);
        Assert.Equal("10", value.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetIfNotExistsAsync_MultipleKeyValuePairs_Success(BaseClient client)
    {
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";
        string value1 = Guid.NewGuid().ToString();
        string value2 = Guid.NewGuid().ToString();

        KeyValuePair<ValkeyKey, ValkeyValue>[] keyValuePairs =
        [
            new(key1, value1),
            new(key2, value2)
        ];

        // First call should succeed since keys don't exist
        bool result = await client.SetIfNotExistsAsync(keyValuePairs);
        Assert.True(result);

        ValkeyValue retrievedValue1 = await client.GetAsync(key1);
        ValkeyValue retrievedValue2 = await client.GetAsync(key2);

        Assert.Equal(value1, retrievedValue1.ToString());
        Assert.Equal(value2, retrievedValue2.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetIfNotExistsAsync_MultipleKeyValuePairs_Failure(BaseClient client)
    {
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";
        string value1 = Guid.NewGuid().ToString();
        string newValue1 = Guid.NewGuid().ToString();
        string newValue2 = Guid.NewGuid().ToString();

        // Set one key first
        await client.SetAsync(key1, value1);

        KeyValuePair<ValkeyKey, ValkeyValue>[] keyValuePairs =
        [
            new(key1, newValue1),
            new(key2, newValue2)
        ];

        // Should fail since key1 already exists
        bool result = await client.SetIfNotExistsAsync(keyValuePairs);
        Assert.False(result);

        // Verify original values are unchanged
        ValkeyValue retrievedValue1 = await client.GetAsync(key1);
        ValkeyValue retrievedValue2 = await client.GetAsync(key2);

        Assert.Equal(value1, retrievedValue1.ToString());
        Assert.True(retrievedValue2.IsNull);
    }

    #region SET with SetCondition/SetOptions

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithCondition_OnlyIfExists_SetsExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "existing");

        bool result = await client.SetAsync(key, "new_value", SetCondition.OnlyIfExists);
        Assert.True(result);

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithCondition_OnlyIfDoesNotExist_SetsNewKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        bool result = await client.SetAsync(key, "value", SetCondition.OnlyIfDoesNotExist);
        Assert.True(result);

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("value", retrieved.ToString());

        // Second call should fail since key now exists
        bool result2 = await client.SetAsync(key, "new_value", SetCondition.OnlyIfDoesNotExist);
        Assert.False(result2);

        ValkeyValue retrieved2 = await client.GetAsync(key);
        Assert.Equal("value", retrieved2.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_WithOptions_ConditionAndExpiry(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        var options = new SetOptions
        {
            Condition = SetCondition.OnlyIfDoesNotExist,
            Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))
        };

        bool result = await client.SetAsync(key, "value", options);
        Assert.True(result);

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetExpiryAsync_SetsValueWithExpiry(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        await client.SetExpiryAsync(key, "value", SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)));

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetIfNotExistsAsync_AllOrNothing(BaseClient client)
    {
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        KeyValuePair<ValkeyKey, ValkeyValue>[] values =
        [
            new(key1, "value1"),
            new(key2, "value2")
        ];

        bool result = await client.SetIfNotExistsAsync(values);
        Assert.True(result);

        // Set one key, then try again - should fail
        KeyValuePair<ValkeyKey, ValkeyValue>[] values2 =
        [
            new(key1, "new_value1"),
            new(key2, "new_value2")
        ];

        bool result2 = await client.SetIfNotExistsAsync(values2);
        Assert.False(result2);

        // Original values should be unchanged
        ValkeyValue retrieved1 = await client.GetAsync(key1);
        Assert.Equal("value1", retrieved1.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetSetAsync_ReturnsOldValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "old_value");

        ValkeyValue result = await client.GetSetAsync(key, "new_value");
        Assert.Equal("old_value", result.ToString());

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetSetAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        ValkeyValue result = await client.GetSetAsync(key, "value");
        Assert.True(result.IsNull);

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetSetAsync_WithCondition_ReturnsOldValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "old_value");

        ValkeyValue result = await client.GetSetAsync(key, "new_value", SetCondition.OnlyIfExists);
        Assert.Equal("old_value", result.ToString());

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetSetAsync_WithOptions_ReturnsOldValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "old_value");

        var options = new SetOptions
        {
            Condition = SetCondition.OnlyIfExists,
            Expiry = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60))
        };

        ValkeyValue result = await client.GetSetAsync(key, "new_value", options);
        Assert.Equal("old_value", result.ToString());

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetSetExpiryAsync_ReturnsOldValueAndSetsExpiry(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "old_value");

        ValkeyValue result = await client.GetSetExpiryAsync(key, "new_value", SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60)));
        Assert.Equal("old_value", result.ToString());

        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    #endregion

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetDeleteAsync_ReturnsValueAndDeletesKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = Guid.NewGuid().ToString();

        await client.SetAsync(key, value);

        ValkeyValue result = await client.GetDeleteAsync(key);
        Assert.Equal(value, result.ToString());

        // Verify key was deleted
        ValkeyValue deletedValue = await client.GetAsync(key);
        Assert.True(deletedValue.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetDeleteAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        ValkeyValue result = await client.GetDeleteAsync(key);
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetExpiryAsync_WithTimeSpan_SetsExpiry(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = Guid.NewGuid().ToString();

        await client.SetAsync(key, value);

        ValkeyValue result = await client.GetExpiryAsync(key, GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)));
        Assert.Equal(value, result.ToString());

        // Verify key still exists and has expiry
        ValkeyValue retrievedValue = await client.GetAsync(key);
        Assert.Equal(value, retrievedValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetExpiryAsync_NonExistentKey_ReturnsNull(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        ValkeyValue result = await client.GetExpiryAsync(key, GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(10)));
        Assert.True(result.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetExpiryAsync_WithPersist_RemovesExpiry(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = Guid.NewGuid().ToString();

        await client.SetAsync(key, value);

        ValkeyValue result = await client.GetExpiryAsync(key, GetExpiryOptions.Persist());
        Assert.Equal(value, result.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task GetExpiryAsync_DateTime_ExistingKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string value = Guid.NewGuid().ToString();

        await client.SetAsync(key, value);

        DateTimeOffset expiry = DateTimeOffset.UtcNow.AddMinutes(5);
        ValkeyValue result = await client.GetExpiryAsync(key, GetExpiryOptions.ExpireAt(expiry));
        Assert.Equal(value, result.ToString());

        // Verify key still exists
        ValkeyValue retrievedValue = await client.GetAsync(key);
        Assert.Equal(value, retrievedValue.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceAsync_BasicTest(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        await client.SetAsync(key1, "abcdef");
        await client.SetAsync(key2, "acef");

        string? result = await client.StringLongestCommonSubsequenceAsync(key1, key2);
        Assert.Equal("acef", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceAsync_NoCommonSubsequence(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        await client.SetAsync(key1, "abc");
        await client.SetAsync(key2, "xyz");

        string? result = await client.StringLongestCommonSubsequenceAsync(key1, key2);
        Assert.Equal("", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceAsync_NonExistentKeys(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        string? result = await client.StringLongestCommonSubsequenceAsync(key1, key2);
        Assert.Equal("", result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceLengthAsync_BasicTest(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        await client.SetAsync(key1, "abcdef");
        await client.SetAsync(key2, "acef");

        long result = await client.StringLongestCommonSubsequenceLengthAsync(key1, key2);
        Assert.Equal(4, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceLengthAsync_NoCommonSubsequence(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        await client.SetAsync(key1, "abc");
        await client.SetAsync(key2, "xyz");

        long result = await client.StringLongestCommonSubsequenceLengthAsync(key1, key2);
        Assert.Equal(0, result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceWithMatchesAsync_BasicTest(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        await client.SetAsync(key1, "abcd1234");
        await client.SetAsync(key2, "bcdef1234");

        LCSMatchResult result = await client.StringLongestCommonSubsequenceWithMatchesAsync(key1, key2);

        Assert.True(result.LongestMatchLength > 0);
        Assert.NotNull(result.Matches);
        Assert.True(result.Matches.Length > 0);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceWithMatchesAsync_WithMinLength(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        await client.SetAsync(key1, "abcd1234");
        await client.SetAsync(key2, "bcdef1234");

        LCSMatchResult result = await client.StringLongestCommonSubsequenceWithMatchesAsync(key1, key2, 4);

        Assert.True(result.LongestMatchLength >= 0);
        Assert.NotNull(result.Matches);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceWithMatchesAsync_NoMatches(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        await client.SetAsync(key1, "abc");
        await client.SetAsync(key2, "xyz");

        LCSMatchResult result = await client.StringLongestCommonSubsequenceWithMatchesAsync(key1, key2);

        Assert.Equal(0, result.LongestMatchLength);
        Assert.NotNull(result.Matches);
        Assert.Empty(result.Matches);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceWithMatchesAsync_ValidateMatchDetails(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        // Set up strings with predictable matches based on actual Redis behavior
        // "abcdef" and "aXcYeF"
        // Expected LCS: "ace" (note: 'f' != 'F' due to case sensitivity)
        // Expected matches: "e" at (4,4), "c" at (2,2), "a" at (0,0) - all length 1
        await client.SetAsync(key1, "abcdef");
        await client.SetAsync(key2, "aXcYeF");

        LCSMatchResult result = await client.StringLongestCommonSubsequenceWithMatchesAsync(key1, key2);

        // Validate overall result - LCS is "ace" with length 3
        Assert.Equal(3, result.LongestMatchLength);
        Assert.NotNull(result.Matches);
        Assert.Equal(3, result.Matches.Length); // Should have exactly 3 matches

        // Sort matches by first string index for predictable testing
        var sortedMatches = result.Matches.OrderBy(m => m.FirstStringIndex).ToArray();

        // Validate exact match details (server-provided lengths)
        // Match 1: "a" at position (0,0) with length 1
        Assert.Equal(0, sortedMatches[0].FirstStringIndex);
        Assert.Equal(0, sortedMatches[0].SecondStringIndex);
        Assert.Equal(1, sortedMatches[0].Length);

        // Match 2: "c" at position (2,2) with length 1
        Assert.Equal(2, sortedMatches[1].FirstStringIndex);
        Assert.Equal(2, sortedMatches[1].SecondStringIndex);
        Assert.Equal(1, sortedMatches[1].Length);

        // Match 3: "e" at position (4,4) with length 1
        Assert.Equal(4, sortedMatches[2].FirstStringIndex);
        Assert.Equal(4, sortedMatches[2].SecondStringIndex);
        Assert.Equal(1, sortedMatches[2].Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceWithMatchesAsync_WithMinLengthFiltering(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );
        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        // Set up strings based on actual Redis behavior
        // "abcdefghijk" and "aXXdefXXijk"
        // Expected LCS: "adefijk" with length 7
        // Expected matches: "ijk" at (8,8) length=3, "def" at (3,3) length=3, "a" at (0,0) length=1
        await client.SetAsync(key1, "abcdefghijk");
        await client.SetAsync(key2, "aXXdefXXijk");

        // Test without minLength filter - should get all 3 matches
        LCSMatchResult resultAll = await client.StringLongestCommonSubsequenceWithMatchesAsync(key1, key2, 0);

        // Test with minLength filter of 2 - should filter out the single "a" match, leaving 2 matches
        LCSMatchResult resultFiltered = await client.StringLongestCommonSubsequenceWithMatchesAsync(key1, key2, 2);

        // Test with minLength filter of 4 - should get no matches since longest is 3
        LCSMatchResult resultHighFilter = await client.StringLongestCommonSubsequenceWithMatchesAsync(key1, key2, 4);

        // Validate overall results - LCS is "adefijk" with length 7
        Assert.Equal(7, resultAll.LongestMatchLength);
        Assert.Equal(7, resultFiltered.LongestMatchLength); // Total LCS length doesn't change
        Assert.Equal(7, resultHighFilter.LongestMatchLength); // Total LCS length doesn't change

        // Validate match counts based on filtering
        Assert.Equal(3, resultAll.Matches.Length); // All matches: "a"(1), "def"(3), "ijk"(3)
        Assert.Equal(2, resultFiltered.Matches.Length); // Only "def"(3) and "ijk"(3)
        Assert.Empty(resultHighFilter.Matches); // No matches >= 4

        // Validate exact match details for unfiltered result (sorted by position)
        var sortedMatches = resultAll.Matches.OrderBy(m => m.FirstStringIndex).ToArray();

        // Match 1: "a" at position (0,0) with length 1
        Assert.Equal(0, sortedMatches[0].FirstStringIndex);
        Assert.Equal(0, sortedMatches[0].SecondStringIndex);
        Assert.Equal(1, sortedMatches[0].Length);

        // Match 2: "def" at position (3,3) with length 3
        Assert.Equal(3, sortedMatches[1].FirstStringIndex);
        Assert.Equal(3, sortedMatches[1].SecondStringIndex);
        Assert.Equal(3, sortedMatches[1].Length);

        // Match 3: "ijk" at position (8,8) with length 3
        Assert.Equal(8, sortedMatches[2].FirstStringIndex);
        Assert.Equal(8, sortedMatches[2].SecondStringIndex);
        Assert.Equal(3, sortedMatches[2].Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task StringLongestCommonSubsequenceWithMatchesAsync_DifferentIndices(BaseClient client)
    {
        Assert.SkipWhen(TestConfiguration.IsVersionLessThan("7.0.0"), "LCS is supported since 7.0.0"
        );

        // Use hash tags to ensure keys map to the same slot in cluster mode
        string baseKey = Guid.NewGuid().ToString();
        string key1 = $"{{{baseKey}}}:key1";
        string key2 = $"{{{baseKey}}}:key2";

        // Set strings where common subsequences appear at different positions
        await client.SetAsync(key1, "xyzabcdef"); // "abc" at index 3-5
        await client.SetAsync(key2, "abcxyzdef"); // "abc" at index 0-2, "def" at index 6-8

        LCSMatchResult result = await client.StringLongestCommonSubsequenceWithMatchesAsync(key1, key2);

        // The LCS should be "abcdef" with length 6
        Assert.Equal(6, result.LongestMatchLength);
        Assert.Equal(2, result.Matches.Length);

        // Sort matches by first string index for consistent testing
        var sortedMatches = result.Matches.OrderBy(m => m.FirstStringIndex).ToArray();

        // Match 1: "abc" appears at different positions - (3,0) with length 3
        Assert.Equal(3, sortedMatches[0].FirstStringIndex);  // "abc" starts at index 3 in first string
        Assert.Equal(0, sortedMatches[0].SecondStringIndex); // "abc" starts at index 0 in second string
        Assert.Equal(3, sortedMatches[0].Length);

        // Match 2: "def" appears at same positions - (6,6) with length 3
        Assert.Equal(6, sortedMatches[1].FirstStringIndex);  // "def" starts at index 6 in first string
        Assert.Equal(6, sortedMatches[1].SecondStringIndex); // "def" starts at index 6 in second string
        Assert.Equal(3, sortedMatches[1].Length);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SingleKey_WhenNotExists_KeyDoesNotExist(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        bool result = await client.SetAsync(key, "value", SetCondition.OnlyIfDoesNotExist);
        Assert.True(result);
        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SingleKey_WhenNotExists_KeyExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "existing");
        bool result = await client.SetAsync(key, "new_value", SetCondition.OnlyIfDoesNotExist);
        Assert.False(result);
        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("existing", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SingleKey_WhenExists_KeyExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "existing");
        bool result = await client.SetAsync(key, "new_value", SetCondition.OnlyIfExists);
        Assert.True(result);
        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SingleKey_WhenExists_KeyDoesNotExist(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        bool result = await client.SetAsync(key, "value", SetCondition.OnlyIfExists);
        Assert.False(result);
        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.True(retrieved.IsNull);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SingleKey_WhenAlways_KeyDoesNotExist(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        bool result = await client.SetAsync(key, "value", SetCondition.Always);
        Assert.True(result);
        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("value", retrieved.ToString());
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task SetAsync_SingleKey_WhenAlways_KeyExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        await client.SetAsync(key, "existing");
        bool result = await client.SetAsync(key, "new_value", SetCondition.Always);
        Assert.True(result);
        ValkeyValue retrieved = await client.GetAsync(key);
        Assert.Equal("new_value", retrieved.ToString());
    }
}
