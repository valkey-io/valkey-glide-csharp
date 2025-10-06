// Copyright Valkey GLIDE Project Contributors - SPDX Identifier: Apache-2.0

namespace Valkey.Glide.IntegrationTests;

public class HyperLogLogCommandTests(TestConfiguration config)
{
    public TestConfiguration Config { get; } = config;

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogAdd_SingleElement(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string element = "test_element";

        // Add single element
        bool result = await client.HyperLogLogAddAsync(key, element);
        Assert.True(result);

        // Adding same element again should return false
        result = await client.HyperLogLogAddAsync(key, element);
        Assert.False(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogAdd_MultipleElements(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        ValkeyValue[] elements = ["element1", "element2", "element3"];

        // Add multiple elements
        bool result = await client.HyperLogLogAddAsync(key, elements);
        Assert.True(result);

        // Adding same elements again should return false
        result = await client.HyperLogLogAddAsync(key, elements);
        Assert.False(result);

        // Adding mix of new and existing elements
        ValkeyValue[] mixedElements = ["element1", "element4", "element5"];
        result = await client.HyperLogLogAddAsync(key, mixedElements);
        Assert.True(result); // Should return true because element4 and element5 are new
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogAdd_EmptyKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        string element = "test_element";

        // Add to non-existent key should create it
        bool result = await client.HyperLogLogAddAsync(key, element);
        Assert.True(result);

        // Verify key exists
        Assert.True(await client.KeyExistsAsync(key));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogAdd_BinaryData(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();
        byte[] binaryElement = [0x00, 0x01, 0x02, 0xFF, 0xFE];

        // Add binary data
        bool result = await client.HyperLogLogAddAsync(key, binaryElement);
        Assert.True(result);

        // Adding same binary data again should return false
        result = await client.HyperLogLogAddAsync(key, binaryElement);
        Assert.False(result);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogLength_SingleKey(BaseClient client)
    {
        string key = Guid.NewGuid().ToString();

        // Empty HyperLogLog should have count 0
        long count = await client.HyperLogLogLengthAsync(key);
        Assert.Equal(0, count);

        // Add elements and check count
        await client.HyperLogLogAddAsync(key, "element1");
        await client.HyperLogLogAddAsync(key, "element2");
        await client.HyperLogLogAddAsync(key, "element3");

        count = await client.HyperLogLogLengthAsync(key);
        Assert.True(count > 0); // Should be approximately 3, but HLL is probabilistic
        Assert.True(count <= 10); // Should be reasonable approximation
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogLength_MultipleKeys(BaseClient client)
    {
        // Use keys with same hash tag to ensure they map to same slot in cluster mode
        string prefix = "{hll}";
        string key1 = prefix + Guid.NewGuid().ToString();
        string key2 = prefix + Guid.NewGuid().ToString();
        string key3 = prefix + Guid.NewGuid().ToString();

        // Add different elements to different HLLs
        await client.HyperLogLogAddAsync(key1, ["a", "b", "c"]);
        await client.HyperLogLogAddAsync(key2, ["c", "d", "e"]);
        await client.HyperLogLogAddAsync(key3, ["f", "g", "h"]);

        // Count union of all HLLs
        long unionCount = await client.HyperLogLogLengthAsync([key1, key2, key3]);
        Assert.True(unionCount > 0);
        Assert.True(unionCount <= 15); // Should be reasonable approximation for union

        // Union should be >= individual counts
        long count1 = await client.HyperLogLogLengthAsync(key1);
        long count2 = await client.HyperLogLogLengthAsync(key2);
        long count3 = await client.HyperLogLogLengthAsync(key3);
        
        Assert.True(unionCount >= Math.Max(Math.Max(count1, count2), count3));
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogLength_NonExistentKey(BaseClient client)
    {
        string nonExistentKey = Guid.NewGuid().ToString();

        // Non-existent key should return 0
        long count = await client.HyperLogLogLengthAsync(nonExistentKey);
        Assert.Equal(0, count);

        // Multiple non-existent keys should return 0 (use same hash tag for cluster compatibility)
        string prefix = "{hll}";
        count = await client.HyperLogLogLengthAsync([prefix + nonExistentKey, prefix + Guid.NewGuid().ToString()]);
        Assert.Equal(0, count);
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogMerge_TwoKeys(BaseClient client)
    {
        // Use keys with same hash tag to ensure they map to same slot in cluster mode
        string prefix = "{hll}";
        string source1 = prefix + Guid.NewGuid().ToString();
        string source2 = prefix + Guid.NewGuid().ToString();
        string destination = prefix + Guid.NewGuid().ToString();

        // Add different elements to source HLLs
        await client.HyperLogLogAddAsync(source1, ["a", "b", "c"]);
        await client.HyperLogLogAddAsync(source2, ["c", "d", "e"]);

        // Merge into destination
        await client.HyperLogLogMergeAsync(destination, source1, source2);

        // Check that destination has merged cardinality
        long destCount = await client.HyperLogLogLengthAsync(destination);
        long source1Count = await client.HyperLogLogLengthAsync(source1);
        long source2Count = await client.HyperLogLogLengthAsync(source2);

        Assert.True(destCount > 0);
        Assert.True(destCount >= Math.Max(source1Count, source2Count)); // Should be at least as large as largest source
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogMerge_MultipleKeys(BaseClient client)
    {
        // Use keys with same hash tag to ensure they map to same slot in cluster mode
        string prefix = "{hll}";
        string source1 = prefix + Guid.NewGuid().ToString();
        string source2 = prefix + Guid.NewGuid().ToString();
        string source3 = prefix + Guid.NewGuid().ToString();
        string destination = prefix + Guid.NewGuid().ToString();

        // Add different elements to source HLLs
        await client.HyperLogLogAddAsync(source1, ["a", "b"]);
        await client.HyperLogLogAddAsync(source2, ["c", "d"]);
        await client.HyperLogLogAddAsync(source3, ["e", "f"]);

        // Merge all sources into destination
        await client.HyperLogLogMergeAsync(destination, [source1, source2, source3]);

        // Check that destination has merged cardinality
        long destCount = await client.HyperLogLogLengthAsync(destination);
        Assert.True(destCount > 0);
        Assert.True(destCount <= 10); // Should be reasonable approximation
    }

    [Theory(DisableDiscoveryEnumeration = true)]
    [MemberData(nameof(Config.TestClients), MemberType = typeof(TestConfiguration))]
    public async Task TestHyperLogLogMerge_EmptySource(BaseClient client)
    {
        // Use keys with same hash tag to ensure they map to same slot in cluster mode
        string prefix = "{hll}";
        string source1 = prefix + Guid.NewGuid().ToString();
        string emptySource = prefix + Guid.NewGuid().ToString();
        string destination = prefix + Guid.NewGuid().ToString();

        // Add elements to only one source
        await client.HyperLogLogAddAsync(source1, ["a", "b", "c"]);
        // emptySource remains empty

        // Merge with empty source
        await client.HyperLogLogMergeAsync(destination, source1, emptySource);

        // Destination should have same cardinality as source1
        long destCount = await client.HyperLogLogLengthAsync(destination);
        long source1Count = await client.HyperLogLogLengthAsync(source1);
        
        Assert.Equal(source1Count, destCount);
    }
}