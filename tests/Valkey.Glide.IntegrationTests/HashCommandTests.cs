// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

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
        await client.HashSetAsync(key, entries);

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
        await client.HashSetAsync(key, entries);

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
        await client.HashSetAsync(key, entries);

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
        await client.HashSetAsync(key, entries);

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
        await client.HashSetAsync(key, entries);

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
        await client.HashSetAsync(key, entries);

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
        await client.HashSetAsync(key, entries);

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
    public async Task TestHashSetWithWhen(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Initial set should succeed
        Assert.True(await client.HashSetAsync(key, "field1", "value1"));

        // Overwriting existing value should return false
        Assert.False(await client.HashSetAsync(key, "field1", "value1-updated", When.Always));
        // Value should be updated
        Assert.Equal("value1-updated", await client.HashGetAsync(key, "field1"));

        // Set with When.NotExists should fail for existing field
        Assert.False(await client.HashSetAsync(key, "field1", "should-not-update", When.NotExists));
        Assert.Equal("value1-updated", await client.HashGetAsync(key, "field1"));

        // Set with When.NotExists should succeed for non-existing field
        Assert.True(await client.HashSetAsync(key, "field2", "value2", When.NotExists));
        Assert.Equal("value2", await client.HashGetAsync(key, "field2"));

        // Set with When.Exists should throw an exception
        await Assert.ThrowsAsync<ArgumentException>(
            () => client.HashSetAsync(key, "field1", "should-not-update", When.Exists));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashIncrement_Long(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test increment on non-existing field (should start from 0)
        Assert.Equal(5, await client.HashIncrementAsync(key, "counter", 5));
        Assert.Equal("5", await client.HashGetAsync(key, "counter"));

        // Test increment with default value (1)
        Assert.Equal(6, await client.HashIncrementAsync(key, "counter"));
        Assert.Equal("6", await client.HashGetAsync(key, "counter"));

        // Test increment with negative value (decrement)
        Assert.Equal(3, await client.HashIncrementAsync(key, "counter", -3));
        Assert.Equal("3", await client.HashGetAsync(key, "counter"));

        // Test increment on existing non-numeric field should throw
        await client.HashSetAsync(key, "text_field", "not_a_number");
        await Assert.ThrowsAsync<RequestException>(
            () => client.HashIncrementAsync(key, "text_field", 1));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashIncrement_Double(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Test increment on non-existing field (should start from 0)
        Assert.Equal(2.5, await client.HashIncrementAsync(key, "float_counter", 2.5));
        Assert.Equal("2.5", await client.HashGetAsync(key, "float_counter"));

        // Test increment with positive value
        Assert.Equal(5.0, await client.HashIncrementAsync(key, "float_counter", 2.5));
        Assert.Equal("5", await client.HashGetAsync(key, "float_counter"));

        // Test increment with negative value (decrement)
        Assert.Equal(2.5, await client.HashIncrementAsync(key, "float_counter", -2.5));
        Assert.Equal("2.5", await client.HashGetAsync(key, "float_counter"));

        // Test increment on existing non-numeric field should throw
        await client.HashSetAsync(key, "text_field", "not_a_number");
        await Assert.ThrowsAsync<RequestException>(
            () => client.HashIncrementAsync(key, "text_field", 1.5));
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
        await client.HashSetAsync(key, entries);

        // Test getting all keys
        keys = await client.HashKeysAsync(key);
        Assert.Equal(3, keys.Length);

        // Sort the keys for consistent testing
        string[] sortedKeys = [.. keys.Select(k => k.ToString()).OrderBy(k => k)];
        Assert.Equal(["field1", "field2", "field3"], sortedKeys);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashScan(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set up test data
        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3"),
            new HashEntry("test_field", "test_value")
        ];
        await client.HashSetAsync(key, entries);

        // Test scan all entries
        var allEntries = new List<HashEntry>();
        await foreach (var entry in client.HashScanAsync(key))
        {
            allEntries.Add(entry);
        }
        Assert.Equal(4, allEntries.Count);

        // Test scan with pattern
        var testEntries = new List<HashEntry>();
        await foreach (var entry in client.HashScanAsync(key, "test*"))
        {
            testEntries.Add(entry);
        }
        Assert.Single(testEntries);
        Assert.Equal("test_field", testEntries[0].Name.ToString());
        Assert.Equal("test_value", testEntries[0].Value.ToString());

        // Test scan on non-existing key
        var emptyEntries = new List<HashEntry>();
        await foreach (var entry in client.HashScanAsync("nonexistent"))
        {
            emptyEntries.Add(entry);
        }
        Assert.Empty(emptyEntries);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashScanNoValues(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Set up test data
        HashEntry[] entries =
        [
            new HashEntry("field1", "value1"),
            new HashEntry("field2", "value2"),
            new HashEntry("field3", "value3"),
            new HashEntry("test_field", "test_value")
        ];
        await client.HashSetAsync(key, entries);

        // Test scan all field names
        var allFields = new List<ValkeyValue>();
        await foreach (var field in client.HashScanNoValuesAsync(key))
        {
            allFields.Add(field);
        }
        Assert.Equal(4, allFields.Count);

        // Verify we only get field names, not values
        var fieldNames = allFields.Select(f => f.ToString()).ToHashSet();
        Assert.Contains("field1", fieldNames);
        Assert.Contains("field2", fieldNames);
        Assert.Contains("field3", fieldNames);
        Assert.Contains("test_field", fieldNames);

        // Test scan with pattern - should only return field names matching pattern
        var testFields = new List<ValkeyValue>();
        await foreach (var field in client.HashScanNoValuesAsync(key, "test*"))
        {
            testFields.Add(field);
        }
        Assert.Single(testFields);
        Assert.Equal("test_field", testFields[0].ToString());

        // Test scan on non-existing key
        var emptyFields = new List<ValkeyValue>();
        await foreach (var field in client.HashScanNoValuesAsync("nonexistent"))
        {
            emptyFields.Add(field);
        }
        Assert.Empty(emptyFields);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashScan_LargeDataset(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Create 25000 hash entries with mixed patterns
        HashEntry[] entries = [.. Enumerable.Range(0, 25000).Select(i => new HashEntry($"field{i}", $"value{i}"))];
        await client.HashSetAsync(key, entries);

        // Test 1: Scan all entries with default settings
        List<HashEntry> allScanned = [];
        await foreach (var entry in client.HashScanAsync(key))
        {
            allScanned.Add(entry);
        }
        Assert.Equal(25000, allScanned.Count);

        // Test 2: Scan with pattern matching (should find fields 1000-1999)
        List<HashEntry> patternScanned = [];
        await foreach (var entry in client.HashScanAsync(key, "field1*"))
        {
            Assert.StartsWith("field1", entry.Name);
            patternScanned.Add(entry);
        }
        Assert.Equal(11111, patternScanned.Count);  // field1, field10-19, field100-199, etc.

        // Test 3: Scan with small page size to test pagination
        List<HashEntry> smallPageScanned = [];
        await foreach (var entry in client.HashScanAsync(key, pageSize: 1000, pageOffset: 500))
        {
            smallPageScanned.Add(entry);
        }
        Assert.Equal(12500, smallPageScanned.Count);

        Assert.Equal(25000, await client.HashLengthAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHashScanNoValues_LargeDataset(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Create 25000 hash entries with mixed patterns
        HashEntry[] entries = [.. Enumerable.Range(0, 25000).Select(i => new HashEntry($"field{i}", $"value{i}"))];
        await client.HashSetAsync(key, entries);

        // Test 1: Scan all field names with default settings
        List<ValkeyValue> allScanned = [];
        await foreach (var field in client.HashScanNoValuesAsync(key))
        {
            allScanned.Add(field);
        }
        Assert.Equal(25000, allScanned.Count);

        // Test 2: Scan with pattern matching (should find fields 1000-1999)
        List<ValkeyValue> patternScanned = [];
        await foreach (var field in client.HashScanNoValuesAsync(key, "field1*"))
        {
            Assert.StartsWith("field1", field);
            patternScanned.Add(field);
        }
        Assert.Equal(11111, patternScanned.Count);  // field1, field10-19, field100-199, etc.

        // Test 3: Scan with small page size to test pagination
        List<ValkeyValue> smallPageScanned = [];
        await foreach (var field in client.HashScanNoValuesAsync(key, pageSize: 1000, pageOffset: 500))
        {
            smallPageScanned.Add(field);
        }
        Assert.Equal(12500, smallPageScanned.Count);

        Assert.Equal(25000, await client.HashLengthAsync(key));
    }
}
