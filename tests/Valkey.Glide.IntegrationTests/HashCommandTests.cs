// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class HashCommandTests(TestConfiguration config)
{
    // TODO #280: Cleanup this class
    public TestConfiguration Config { get; } = config;

    #region HashSetAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashSet_HashGet(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test single field set and get
        Assert.True(await client.HashSetAsync(key, "field1", "value1"));
        Assert.Equal("value1", await client.HashGetAsync(key, "field1"));

        // Test multiple fields set and get
        HashEntry[] entries =
        [
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        ValkeyValue[] values = await client.HashGetAsync(key, ["field1", "field2", "field3"]);
        Assert.Equal(3, values.Length);
        Assert.Equal("value1", values[0]);
        Assert.Equal("value2", values[1]);
        Assert.Equal("value3", values[2]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashSet_SingleField(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Initial set should return true (new field)
        Assert.True(await client.HashSetAsync(key, "field1", "value1"));

        // Overwriting existing value should return false (not new)
        Assert.False(await client.HashSetAsync(key, "field1", "value1-updated"));
        Assert.Equal("value1-updated", await client.HashGetAsync(key, "field1"));
    }

    #endregion
    #region HashSetIfNotExistsAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashSetIfNotExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set on non-existing field should succeed
        Assert.True(await client.HashSetIfNotExistsAsync(key, "field1", "value1"));
        Assert.Equal("value1", await client.HashGetAsync(key, "field1"));

        // Set on existing field should fail
        Assert.False(await client.HashSetIfNotExistsAsync(key, "field1", "should-not-update"));
        Assert.Equal("value1", await client.HashGetAsync(key, "field1"));

        // Set on new field should succeed
        Assert.True(await client.HashSetIfNotExistsAsync(key, "field2", "value2"));
        Assert.Equal("value2", await client.HashGetAsync(key, "field2"));
    }

    #endregion
    #region HashGetAllAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashGetAll(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        IDictionary<ValkeyValue, ValkeyValue> result = await client.HashGetAllAsync(key);
        Assert.Equal(3, result.Count);

        Assert.Equal("value1", result["field1"]);
        Assert.Equal("value2", result["field2"]);
        Assert.Equal("value3", result["field3"]);
    }

    #endregion
    #region HashDeleteAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashDelete(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set up test data
        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3"),
            new HashEntry("field4", "value4")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        // Test single field delete
        Assert.True(await client.HashDeleteAsync(key, "field1"));
        Assert.False(await client.HashExistsAsync(key, "field1"));

        // Test multiple fields delete
        Assert.Equal(2, await client.HashDeleteAsync(key, ["field2", "field3"]));
        Assert.False(await client.HashExistsAsync(key, "field2"));
        Assert.False(await client.HashExistsAsync(key, "field3"));
        Assert.True(await client.HashExistsAsync(key, "field4"));
    }

    #endregion
    #region HashExistsAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        _ = await client.HashSetAsync(key, "field1", "value1");

        Assert.True(await client.HashExistsAsync(key, "field1"));
        Assert.False(await client.HashExistsAsync(key, "nonexistent"));
    }

    #endregion
    #region HashLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashLength(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        Assert.Equal(0, await client.HashLengthAsync(key));

        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        Assert.Equal(3, await client.HashLengthAsync(key));
    }

    #endregion
    #region HashStringLengthAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashStringLength(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        _ = await client.HashSetAsync(key, "field1", "value1");
        _ = await client.HashSetAsync(key, "field2", "value-with-longer-content");

        Assert.Equal(6, await client.HashStringLengthAsync(key, "field1"));
        Assert.Equal(25, await client.HashStringLengthAsync(key, "field2"));
    }

    #endregion
    #region HashValuesAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashValues(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        ICollection<ValkeyValue> values = await client.HashValuesAsync(key);
        Assert.Equal(3, values.Count);
        Assert.Contains((ValkeyValue)"value1", values);
        Assert.Contains((ValkeyValue)"value2", values);
        Assert.Contains((ValkeyValue)"value3", values);
    }

    #endregion
    #region HashKeysAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashKeys(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existing key
        ISet<ValkeyValue> keys = await client.HashKeysAsync(key);
        Assert.Empty(keys);

        // Set up test data
        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        // Test getting all keys
        keys = await client.HashKeysAsync(key);
        Assert.Equal(3, keys.Count);
        Assert.Contains((ValkeyValue)"field1", keys);
        Assert.Contains((ValkeyValue)"field2", keys);
        Assert.Contains((ValkeyValue)"field3", keys);
    }

    #endregion
    #region HashIncrementByAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashIncrement_Long(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test increment on non-existing field (should start from 0)
        Assert.Equal(5, await client.HashIncrementByAsync(key, "counter", 5));
        Assert.Equal("5", await client.HashGetAsync(key, "counter"));

        // Test increment with default value (1)
        Assert.Equal(6, await client.HashIncrementByAsync(key, "counter"));
        Assert.Equal("6", await client.HashGetAsync(key, "counter"));

        // Test increment with negative value (decrement)
        Assert.Equal(3, await client.HashIncrementByAsync(key, "counter", -3));
        Assert.Equal("3", await client.HashGetAsync(key, "counter"));

        // Test increment on existing non-numeric field should throw
        _ = await client.HashSetAsync(key, "text_field", "not_a_number");
        _ = await Assert.ThrowsAsync<RequestException>(
            () => client.HashIncrementByAsync(key, "text_field", 1));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashIncrement_Double(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test increment on non-existing field (should start from 0)
        Assert.Equal(2.5, await client.HashIncrementByAsync(key, "float_counter", 2.5));
        Assert.Equal("2.5", await client.HashGetAsync(key, "float_counter"));

        // Test increment with positive value
        Assert.Equal(5.0, await client.HashIncrementByAsync(key, "float_counter", 2.5));
        Assert.Equal("5", await client.HashGetAsync(key, "float_counter"));

        // Test increment with negative value (decrement)
        Assert.Equal(2.5, await client.HashIncrementByAsync(key, "float_counter", -2.5));
        Assert.Equal("2.5", await client.HashGetAsync(key, "float_counter"));

        // Test increment on existing non-numeric field should throw
        _ = await client.HashSetAsync(key, "text_field", "not_a_number");
        _ = await Assert.ThrowsAsync<RequestException>(
            () => client.HashIncrementByAsync(key, "text_field", 1.5));
    }

    #endregion
    #region HashRandomFieldAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashRandomField(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        // Test single random field
        ValkeyValue randomField = await client.HashRandomFieldAsync(key);
        Assert.Contains(randomField.ToString(), new[] { "field1", "field2", "field3" });

        // Test multiple random fields
        ValkeyValue[] randomFields = await client.HashRandomFieldsAsync(key, 2);
        Assert.Equal(2, randomFields.Length);
        foreach (ValkeyValue field in randomFields)
        {
            Assert.Contains(field.ToString(), new[] { "field1", "field2", "field3" });
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashRandomFieldsWithValues(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        ICollection<KeyValuePair<ValkeyValue, ValkeyValue>> randomEntries = await client.HashRandomFieldsWithValuesAsync(key, 2);
        Assert.Equal(2, randomEntries.Count);

        foreach (var entry in randomEntries)
        {
            string fieldName = entry.Key.ToString();
            string fieldValue = entry.Value.ToString();

            Assert.Contains(fieldName, new[] { "field1", "field2", "field3" });
            Assert.Equal("value" + fieldName[5..], fieldValue);
        }
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashRandomFieldWithValue(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        // Test single random field with value
        KeyValuePair<ValkeyValue, ValkeyValue>? entry = await client.HashRandomFieldWithValueAsync(key);
        _ = Assert.NotNull(entry);
        string fieldName = entry.Value.Key.ToString();
        Assert.Contains(fieldName, new[] { "field1", "field2", "field3" });
        Assert.Equal("value" + fieldName[5..], entry.Value.Value.ToString());
    }

    #endregion
    #region HashGetExpiryAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashGetExpiry(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Set up test data
        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        // Test HGETEX with expiry as duration
        var options = GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60));
        ValkeyValue[] values = await client.HashGetExpiryAsync(key, ["field1", "field2"], options);
        Assert.Equal(2, values.Length);
        Assert.Equal("value1", values[0]);
        Assert.Equal("value2", values[1]);

        // Test HGETEX with persist option
        var persistOptions = GetExpiryOptions.Persist();
        values = await client.HashGetExpiryAsync(key, ["field1"], persistOptions);
        _ = Assert.Single(values);
        Assert.Equal("value1", values[0]);

        // Test HGETEX on non-existing key
        values = await client.HashGetExpiryAsync("nonexistent", ["field1"], options);
        Assert.Equal(ValkeyValue.Null, values[0]);

        // Test HGETEX with non-existing fields
        values = await client.HashGetExpiryAsync(key, ["nonexistent1", "nonexistent2"], options);
        Assert.NotNull(values);
        Assert.Equal(2, values.Length);
        Assert.Equal(ValkeyValue.Null, values[0]);
        Assert.Equal(ValkeyValue.Null, values[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashGetExpiry_SingleField(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Set up test data
        _ = await client.HashSetAsync(key, "field1", "value1");

        // Test HGETEX single field with expiry as duration
        var expireInOptions = GetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60));
        ValkeyValue value = await client.HashGetExpiryAsync(key, "field1", expireInOptions);
        Assert.Equal("value1", value);

        // Verify TTL was set
        TimeToLiveResult ttlResult = await client.HashTimeToLiveAsync(key, "field1");
        Assert.True(ttlResult.HasTimeToLive);
        Assert.True(ttlResult.TimeToLive!.Value.TotalSeconds is > 0 and <= 60);

        // Test HGETEX single field with expiry as timestamp
        DateTimeOffset expiry = DateTimeOffset.UtcNow.AddMinutes(5);
        var expireAtOptions = GetExpiryOptions.ExpireAt(expiry);
        value = await client.HashGetExpiryAsync(key, "field1", expireAtOptions);
        Assert.Equal("value1", value);

        // Verify expiry was set
        ExpireTimeResult expireTimeResult = await client.HashExpireTimeAsync(key, "field1");
        Assert.True(expireTimeResult.HasExpiry);
        _ = Assert.NotNull(expireTimeResult.Expiry);

        // Test HGETEX single field with persist option
        var persistOptions = GetExpiryOptions.Persist();
        value = await client.HashGetExpiryAsync(key, "field1", persistOptions);
        Assert.Equal("value1", value);

        // Verify expiry was removed
        ttlResult = await client.HashTimeToLiveAsync(key, "field1");
        Assert.True(ttlResult.Exists);
        Assert.False(ttlResult.HasTimeToLive);

        // Test HGETEX single field that doesn't exist
        value = await client.HashGetExpiryAsync(key, "nonexistent", expireInOptions);
        Assert.Equal(ValkeyValue.Null, value);

        // Test HGETEX single field on non-existing key
        value = await client.HashGetExpiryAsync("nonexistent_key", "field1", expireInOptions);
        Assert.Equal(ValkeyValue.Null, value);
    }


    #endregion
    #region HashSetExpiryAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashSetExpiry(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Test HSETEX with expiry as duration
        KeyValuePair<ValkeyValue, ValkeyValue>[] hashFieldsAndValues =
        [
            new("field1", "value1"),
            new("field2", "value2")
        ];
        var options = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60));
        Assert.True(await client.HashSetExpiryAsync(key, hashFieldsAndValues, options));

        // Verify values were set
        Assert.Equal("value1", await client.HashGetAsync(key, "field1"));
        Assert.Equal("value2", await client.HashGetAsync(key, "field2"));

        // Test HSETEX with FNX condition (should fail for existing fields)
        Assert.False(await client.HashSetExpiryAsync(key, hashFieldsAndValues, options, HashSetCondition.OnlyIfNoneExist));

        // Test HSETEX with FXX condition (should succeed for existing fields)
        KeyValuePair<ValkeyValue, ValkeyValue>[] updateFields = [new("field1", "updated_value1")];
        Assert.True(await client.HashSetExpiryAsync(key, updateFields, options, HashSetCondition.OnlyIfAllExist));
        Assert.Equal("updated_value1", await client.HashGetAsync(key, "field1"));

        // Test HSETEX with KEEPTTL option
        var keepTtlOptions = SetExpiryOptions.KeepTimeToLive();
        KeyValuePair<ValkeyValue, ValkeyValue>[] keepTtlFields = [new("field3", "value3")];
        Assert.True(await client.HashSetExpiryAsync(key, keepTtlFields, keepTtlOptions));
        Assert.Equal("value3", await client.HashGetAsync(key, "field3"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashSetExpiry_SingleField(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Test HSETEX single field with expiry as duration
        var expireInOptions = SetExpiryOptions.ExpireIn(TimeSpan.FromSeconds(60));
        Assert.True(await client.HashSetExpiryAsync(key, "field1", "value1", expireInOptions));
        Assert.Equal("value1", await client.HashGetAsync(key, "field1"));

        // Verify TTL was set
        TimeToLiveResult ttlResult = await client.HashTimeToLiveAsync(key, "field1");
        Assert.True(ttlResult.HasTimeToLive);
        Assert.True(ttlResult.TimeToLive!.Value.TotalSeconds is > 0 and <= 60);

        // Test HSETEX single field with expiry as timestamp
        DateTimeOffset expiry = DateTimeOffset.UtcNow.AddMinutes(5);
        var expireAtOptions = SetExpiryOptions.ExpireAt(expiry);
        Assert.True(await client.HashSetExpiryAsync(key, "field2", "value2", expireAtOptions));
        Assert.Equal("value2", await client.HashGetAsync(key, "field2"));

        // Verify expiry was set
        ExpireTimeResult expireTimeResult = await client.HashExpireTimeAsync(key, "field2");
        Assert.True(expireTimeResult.HasExpiry);
        _ = Assert.NotNull(expireTimeResult.Expiry);

        // Test HSETEX single field with KEEPTTL option
        var keepTtlOptions = SetExpiryOptions.KeepTimeToLive();
        Assert.True(await client.HashSetExpiryAsync(key, "field1", "updated_value1", keepTtlOptions));
        Assert.Equal("updated_value1", await client.HashGetAsync(key, "field1"));

        // Verify TTL was preserved
        ttlResult = await client.HashTimeToLiveAsync(key, "field1");
        Assert.True(ttlResult.HasTimeToLive);
        Assert.True(ttlResult.TimeToLive!.Value.TotalSeconds is > 0 and <= 60);

        // Test HSETEX single field with FNX condition on new field (should succeed)
        Assert.True(await client.HashSetExpiryAsync(key, "new_field", "new_value", expireInOptions, HashSetCondition.OnlyIfNoneExist));
        Assert.Equal("new_value", await client.HashGetAsync(key, "new_field"));

        // Test HSETEX single field with FNX condition on existing field (should fail)
        Assert.False(await client.HashSetExpiryAsync(key, "field1", "should_not_set", expireInOptions, HashSetCondition.OnlyIfNoneExist));
        Assert.Equal("updated_value1", await client.HashGetAsync(key, "field1"));

        // Test HSETEX single field with FXX condition on existing field (should succeed)
        Assert.True(await client.HashSetExpiryAsync(key, "field1", "exist_updated", expireInOptions, HashSetCondition.OnlyIfAllExist));
        Assert.Equal("exist_updated", await client.HashGetAsync(key, "field1"));

        // Test HSETEX single field with FXX condition on new field (should fail)
        Assert.False(await client.HashSetExpiryAsync(key, "nonexistent_field", "value", expireInOptions, HashSetCondition.OnlyIfAllExist));
        Assert.False(await client.HashExistsAsync(key, "nonexistent_field"));
    }


    #endregion
    #region HashPersistAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashPersist(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // First, let's test a simple case - set a field without expiry and try to persist it
        _ = await client.HashSetAsync(key, "field1", "value1");

        // First test HTTL to see if it works (should return no expiry)
        TimeToLiveResult[] ttlResults = await client.HashTimeToLiveAsync(key, ["field1"]);
        _ = Assert.Single(ttlResults);
        Assert.True(ttlResults[0].Exists);
        Assert.False(ttlResults[0].HasTimeToLive);

        // Now test HPERSIST (should return ConditionNotMet for field exists but has no expiry)
        HashPersistResult[] results = await client.HashPersistAsync(key, ["field1"]);
        _ = Assert.Single(results);
        Assert.Equal(HashPersistResult.NoExpiry, results[0]);

        // Test HPERSIST on non-existing field (should return NoSuchField)
        results = await client.HashPersistAsync(key, ["nonexistent"]);
        _ = Assert.Single(results);
        Assert.Equal(HashPersistResult.NoField, results[0]);

        // Test HPERSIST on non-existing key
        results = await client.HashPersistAsync("nonexistent_key", ["field1"]);
        _ = Assert.Single(results);
        Assert.Equal(HashPersistResult.NoField, results[0]);
    }

    #endregion
    #region HashExpireAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashExpire(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Set up test data
        _ = await client.HashSetAsync(key, "field1", "value1");
        _ = await client.HashSetAsync(key, "field2", "value2");

        // Test HEXPIRE with no conditions
        ExpireResult[] results = await client.HashExpireAsync(key, ["field1", "field2"], TimeSpan.FromSeconds(60));
        Assert.Equal(2, results.Length);
        Assert.Equal(ExpireResult.Success, results[0]);
        Assert.Equal(ExpireResult.Success, results[1]);

        // Test HEXPIRE with NX condition (should fail for fields with existing expiry)
        results = await client.HashExpireAsync(key, ["field1"], TimeSpan.FromSeconds(120), ExpireCondition.OnlyIfNotExists);
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.ConditionNotMet, results[0]);

        // Test HEXPIRE with XX condition (should succeed for fields with existing expiry)
        results = await client.HashExpireAsync(key, ["field1"], TimeSpan.FromSeconds(30), ExpireCondition.OnlyIfExists);
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.Success, results[0]);

        // Test HEXPIRE on non-existing field
        results = await client.HashExpireAsync(key, ["nonexistent"], TimeSpan.FromSeconds(60));
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.NoSuchField, results[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashPExpire(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Set up test data
        _ = await client.HashSetAsync(key, "field1", "value1");

        // Test HPEXPIRE
        ExpireResult[] results = await client.HashExpireAsync(key, ["field1"], TimeSpan.FromMilliseconds(5000));
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.Success, results[0]);

        // Test HPEXPIRE on non-existing field
        results = await client.HashExpireAsync(key, ["nonexistent"], TimeSpan.FromMilliseconds(5000));
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.NoSuchField, results[0]);
    }


    #endregion
    #region HashExpireAtAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashExpireAt(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Set up test data
        _ = await client.HashSetAsync(key, "field1", "value1");

        // Test HEXPIREAT
        DateTimeOffset expiry = DateTimeOffset.UtcNow.AddMinutes(5);
        ExpireResult[] results = await client.HashExpireAtAsync(key, ["field1"], expiry);
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.Success, results[0]);

        // Test HEXPIREAT on non-existing field
        results = await client.HashExpireAtAsync(key, ["nonexistent"], expiry);
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.NoSuchField, results[0]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashPExpireAt(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Set up test data
        _ = await client.HashSetAsync(key, "field1", "value1");

        // Test HPEXPIREAT
        DateTimeOffset expiry = DateTimeOffset.UtcNow.AddMinutes(5);
        ExpireResult[] results = await client.HashExpireAtAsync(key, ["field1"], expiry);
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.Success, results[0]);

        // Test HPEXPIREAT on non-existing field
        results = await client.HashExpireAtAsync(key, ["nonexistent"], expiry);
        _ = Assert.Single(results);
        Assert.Equal(ExpireResult.NoSuchField, results[0]);
    }

    #endregion
    #region HashExpireTimeAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashExpireTime(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Set up test data with expiry
        _ = await client.HashSetAsync(key, "field1", "value1");
        _ = await client.HashSetAsync(key, "field2", "value2"); // No expiry

        // Set expiry for field1
        DateTimeOffset expiry = DateTimeOffset.UtcNow.AddMinutes(5);
        _ = await client.HashExpireAtAsync(key, ["field1"], expiry);

        // Test single-field
        ExpireTimeResult singleResult = await client.HashExpireTimeAsync(key, "field1");
        Assert.True(singleResult.HasExpiry);
        _ = Assert.NotNull(singleResult.Expiry);

        // Test multi-field
        ExpireTimeResult[] results = await client.HashExpireTimeAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equal(3, results.Length);
        Assert.True(results[0].HasExpiry);
        _ = Assert.NotNull(results[0].Expiry);
        Assert.True(results[1].Exists);
        Assert.False(results[1].HasExpiry);
        Assert.Null(results[1].Expiry);
        Assert.False(results[2].Exists);
        Assert.Null(results[2].Expiry);
    }

    #endregion
    #region HashTimeToLiveAsync

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashTimeToLive(BaseClient client)
    {
        SkipUtils.IfHashExpireNotSupported();

        string key = Guid.NewGuid().ToString();

        // Set up test data with expiry
        _ = await client.HashSetAsync(key, "field1", "value1");
        _ = await client.HashSetAsync(key, "field2", "value2"); // No expiry

        // Set expiry for field1
        _ = await client.HashExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        // Test multi-field
        TimeToLiveResult[] results = await client.HashTimeToLiveAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equal(3, results.Length);
        Assert.True(results[0].HasTimeToLive);
        _ = Assert.NotNull(results[0].TimeToLive);
        Assert.True(results[0].TimeToLive!.Value.TotalSeconds is > 0 and <= 60);
        Assert.True(results[1].Exists);
        Assert.False(results[1].HasTimeToLive);
        Assert.False(results[2].Exists);

        // Test single-field
        TimeToLiveResult singleResult = await client.HashTimeToLiveAsync(key, "field1");
        Assert.True(singleResult.HasTimeToLive);
        _ = Assert.NotNull(singleResult.TimeToLive);
        Assert.True(singleResult.TimeToLive!.Value.TotalSeconds is > 0 and <= 60);
    }

    #endregion
}
