// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

using Valkey.Glide.Commands.Options;

using static Valkey.Glide.Errors;

namespace Valkey.Glide.IntegrationTests;

public class HashCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

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

        HashEntry[] result = await client.HashGetAllAsync(key);
        Assert.Equal(3, result.Length);

        // Sort the results for consistent testing
        Array.Sort(result, (a, b) => string.Compare(a.Name.ToString(), b.Name.ToString()));

        Assert.Equal("field1", result[0].Name);
        Assert.Equal("value1", result[0].Value);
        Assert.Equal("field2", result[1].Name);
        Assert.Equal("value2", result[1].Value);
        Assert.Equal("field3", result[2].Name);
        Assert.Equal("value3", result[2].Value);
    }

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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashExists(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        _ = await client.HashSetAsync(key, "field1", "value1");

        Assert.True(await client.HashExistsAsync(key, "field1"));
        Assert.False(await client.HashExistsAsync(key, "nonexistent"));
    }

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

        ValkeyValue[] values = await client.HashValuesAsync(key);
        Assert.Equal(3, values.Length);

        // Sort the values for consistent testing
        Array.Sort(values, (a, b) => string.Compare(a.ToString(), b.ToString()));

        Assert.Equal("value1", values[0]);
        Assert.Equal("value2", values[1]);
        Assert.Equal("value3", values[2]);
    }

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

        HashEntry[] randomEntries = await client.HashRandomFieldsWithValuesAsync(key, 2);
        Assert.Equal(2, randomEntries.Length);

        foreach (HashEntry entry in randomEntries)
        {
            string fieldName = entry.Name.ToString();
            string fieldValue = entry.Value.ToString();

            Assert.Contains(fieldName, new[] { "field1", "field2", "field3" });
            Assert.Equal("value" + fieldName[5..], fieldValue);
        }
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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashKeys(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test on non-existing key
        ValkeyValue[] keys = await client.HashKeysAsync(key);
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
        Assert.Equal(3, keys.Length);

        // Sort the keys for consistent testing
        string[] sortedKeys = [.. keys.Select(k => k.ToString()).OrderBy(k => k)];
        Assert.Equal(["field1", "field2", "field3"], sortedKeys);
    }

    // Hash Field Expire Commands (Valkey 9.0+)

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashGetEx(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HGETEX is supported since Valkey 9.0.0"
        );

        string key = Guid.NewGuid().ToString();

        // Set up test data
        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3")
        ];
        _ = await client.HashSetAsync(key, entries.Select(e => new KeyValuePair<ValkeyValue, ValkeyValue>(e.Name, e.Value)));

        // Test HGETEX with expiry in seconds
        var options = new HashGetExOptions().SetExpiry(HGetExExpiry.Seconds(60));
        ValkeyValue[]? values = await client.HashGetExAsync(key, ["field1", "field2"], options);
        Assert.NotNull(values);
        Assert.Equal(2, values.Length);
        Assert.Equal("value1", values[0]);
        Assert.Equal("value2", values[1]);

        // Test HGETEX with expiry in milliseconds
        var msOptions = new HashGetExOptions().SetExpiry(HGetExExpiry.Milliseconds(5000));
        values = await client.HashGetExAsync(key, ["field3"], msOptions);
        Assert.NotNull(values);
        _ = Assert.Single(values);
        Assert.Equal("value3", values[0]);

        // Test HGETEX with persist option
        var persistOptions = new HashGetExOptions().SetExpiry(HGetExExpiry.Persist());
        values = await client.HashGetExAsync(key, ["field1"], persistOptions);
        Assert.NotNull(values);
        _ = Assert.Single(values);
        Assert.Equal("value1", values[0]);

        // Test HGETEX on non-existing key
        values = await client.HashGetExAsync("nonexistent", ["field1"], options);
        Assert.NotNull(values);
        Assert.Equal(ValkeyValue.Null, values[0]);

        // Test HGETEX with non-existing fields
        values = await client.HashGetExAsync(key, ["nonexistent1", "nonexistent2"], options);
        Assert.NotNull(values);
        Assert.Equal(2, values.Length);
        Assert.Equal(ValkeyValue.Null, values[0]);
        Assert.Equal(ValkeyValue.Null, values[1]);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashSetEx(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HSETEX is supported since Valkey 9.0.0"
        );

        string key = Guid.NewGuid().ToString();

        // Test HSETEX with expiry in seconds
        var fieldValueMap = new Dictionary<ValkeyValue, ValkeyValue>
        {
            { "field1", "value1" },
            { "field2", "value2" }
        };
        var options = new HashSetExOptions().SetExpiry(ExpirySet.Seconds(60));
        Assert.Equal(1L, await client.HashSetExAsync(key, fieldValueMap, options));

        // Verify values were set
        Assert.Equal("value1", await client.HashGetAsync(key, "field1"));
        Assert.Equal("value2", await client.HashGetAsync(key, "field2"));

        // Test HSETEX with NX condition (should fail for existing fields)
        var nxOptions = new HashSetExOptions().SetOnlyIfNoneExist();
        Assert.Equal(0L, await client.HashSetExAsync(key, fieldValueMap, nxOptions));

        // Test HSETEX with XX condition (should succeed for existing fields)
        var xxOptions = new HashSetExOptions().SetOnlyIfAllExist();
        var updateMap = new Dictionary<ValkeyValue, ValkeyValue> { { "field1", "updated_value1" } };
        Assert.Equal(1L, await client.HashSetExAsync(key, updateMap, xxOptions));
        Assert.Equal("updated_value1", await client.HashGetAsync(key, "field1"));

        // Test HSETEX with KEEPTTL option
        var keepTtlOptions = new HashSetExOptions().SetExpiry(ExpirySet.KeepExisting());
        var keepTtlMap = new Dictionary<ValkeyValue, ValkeyValue> { { "field3", "value3" } };
        Assert.Equal(1L, await client.HashSetExAsync(key, keepTtlMap, keepTtlOptions));
        Assert.Equal("value3", await client.HashGetAsync(key, "field3"));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashPersist(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HPERSIST is supported since Valkey 9.0.0"
        );

        string key = Guid.NewGuid().ToString();

        // First, let's test a simple case - set a field without expiry and try to persist it
        _ = await client.HashSetAsync(key, "field1", "value1");

        // First test HTTL to see if it works (should return no expiry)
        TimeToLiveResult[] ttlResults = await client.HashTimeToLiveAsync(key, ["field1"]);
        _ = Assert.Single(ttlResults);
        Assert.True(ttlResults[0].Exists);
        Assert.False(ttlResults[0].HasExpiry);

        // Now test HPERSIST (should return -1 for field exists but has no expiry)
        long[] results = await client.HashPersistAsync(key, ["field1"]);
        _ = Assert.Single(results);
        Assert.Equal(-1, results[0]); // Field exists but has no expiry

        // Test HPERSIST on non-existing field (should return -2)
        results = await client.HashPersistAsync(key, ["nonexistent"]);
        _ = Assert.Single(results);
        Assert.Equal(-2, results[0]); // Field does not exist

        // Test HPERSIST on non-existing key
        results = await client.HashPersistAsync("nonexistent_key", ["field1"]);
        _ = Assert.Single(results);
        Assert.Equal(-2, results[0]); // Key does not exist
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashExpire(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HEXPIRE is supported since Valkey 9.0.0"
        );

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
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HPEXPIRE is supported since Valkey 9.0.0"
        );

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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashExpireAt(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HEXPIREAT is supported since Valkey 9.0.0"
        );

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
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HPEXPIREAT is supported since Valkey 9.0.0"
        );

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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashExpireTime(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HPEXPIRETIME is supported since Valkey 9.0.0"
        );

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

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashTimeToLive(BaseClient client)
    {
        Assert.SkipWhen(
            TestConfiguration.SERVER_VERSION < new Version("9.0.0"),
            "HPTTL is supported since Valkey 9.0.0"
        );

        string key = Guid.NewGuid().ToString();

        // Set up test data with expiry
        _ = await client.HashSetAsync(key, "field1", "value1");
        _ = await client.HashSetAsync(key, "field2", "value2"); // No expiry

        // Set expiry for field1
        _ = await client.HashExpireAsync(key, ["field1"], TimeSpan.FromSeconds(60));

        // Test multi-field
        TimeToLiveResult[] results = await client.HashTimeToLiveAsync(key, ["field1", "field2", "nonexistent"]);
        Assert.Equal(3, results.Length);
        Assert.True(results[0].HasExpiry);
        _ = Assert.NotNull(results[0].TimeToLive);
        Assert.True(results[0].TimeToLive!.Value.TotalSeconds is > 0 and <= 60);
        Assert.True(results[1].Exists);
        Assert.False(results[1].HasExpiry); // field2 has no expiry
        Assert.False(results[2].Exists); // nonexistent field

        // Test single-field
        TimeToLiveResult singleResult = await client.HashTimeToLiveAsync(key, "field1");
        Assert.True(singleResult.HasExpiry);
        _ = Assert.NotNull(singleResult.TimeToLive);
        Assert.True(singleResult.TimeToLive!.Value.TotalSeconds is > 0 and <= 60);
    }
}
